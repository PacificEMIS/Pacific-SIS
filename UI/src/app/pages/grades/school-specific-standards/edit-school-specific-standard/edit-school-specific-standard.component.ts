/***********************************************************************************
openSIS is a free student information system for public and non-public
schools from Open Solutions for Education, Inc.Website: www.os4ed.com.

Visit the openSIS product website at https://opensis.com to learn more.
If you have question regarding this software or the license, please contact
via the website.

The software is released under the terms of the GNU Affero General Public License as
published by the Free Software Foundation, version 3 of the License.
See https://www.gnu.org/licenses/agpl-3.0.en.html.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

Copyright (c) Open Solutions for Education, Inc.

All rights reserved.
***********************************************************************************/

import { Component, ElementRef, Inject, OnDestroy, OnInit, Optional, ViewChild } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import icClose from '@iconify/icons-ic/twotone-close';
import icAdd from '@iconify/icons-ic/add-circle-outline';
import { fadeInUp400ms } from '../../../../../@vex/animations/fade-in-up.animation';
import { stagger60ms } from '../../../../../@vex/animations/stagger.animation';
import { GetAllGradeLevelsModel } from '../../../../models/grade-level.model';
import { GradeLevelService } from '../../../../services/grade-level.service';
import { LoaderService } from '../../../../services/loader.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CheckStandardRefNoModel, GradeStandardSubjectCourseListModel, SchoolSpecificStandarModel } from '../../../../models/grades.model';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { GradesService } from '../../../../services/grades.service';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs/internal/Subject';
import { CommonService } from 'src/app/services/common.service';
import { DefaultValuesService } from 'src/app/common/default-values.service';

@Component({
  selector: 'vex-edit-school-specific-standard',
  templateUrl: './edit-school-specific-standard.component.html',
  styleUrls: ['./edit-school-specific-standard.component.scss'],
  animations: [
    stagger60ms,
    fadeInUp400ms
  ]
})
export class EditSchoolSpecificStandardComponent implements OnInit, OnDestroy{
  @ViewChild('subject') subject: ElementRef;
  icClose = icClose;
  icAdd = icAdd;

  gradeLevelList: GetAllGradeLevelsModel = new GetAllGradeLevelsModel();
  schoolSpecificStandard:SchoolSpecificStandarModel= new SchoolSpecificStandarModel();
  subjectList:GradeStandardSubjectCourseListModel=new GradeStandardSubjectCourseListModel();
  courseList:GradeStandardSubjectCourseListModel=new GradeStandardSubjectCourseListModel();
  standardRefNoModel:CheckStandardRefNoModel=new CheckStandardRefNoModel();
  form:FormGroup;
  editMode:boolean;
  editDetails;
  modalActionButton="submit"
  modalDialogTitle="addNewStandard"
  destroySubject$: Subject<void> = new Subject();
  loading:boolean;
  checkStandardRefNo=false;
  constructor(private dialogRef: MatDialogRef<EditSchoolSpecificStandardComponent>,
    @Optional() @Inject(MAT_DIALOG_DATA) public data: any,
    private gradeLevelService: GradeLevelService,
    private snackbar: MatSnackBar,
    private fb: FormBuilder,
    private gradesService:GradesService,
    private loaderService: LoaderService,
    private commonService: CommonService,
    private defaultValuesService: DefaultValuesService
    ) { 
      this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
        this.loading = val;
      });

      if(this.data.editMode){
        this.editMode = this.data.editMode;
        this.editDetails= this.data.schoolSpecificStandards;
        this.modalActionButton="update";
        this.modalDialogTitle="updateStandard"
       }else{
        this.editMode = this.data.editMode;
       }
       this.gradeLevelList= this.data.parentData.gradeLevelList,
       this.subjectList= this.data.parentData.subjectList,
       this.courseList= this.data.parentData.courseList
    }

  ngOnInit(): void {
    this.form = this.fb.group(
      {
        standardRefNo: ['',[Validators.required]],
        gradeLevel: ['',Validators.required],
        domain: [''],
        subject: ['',Validators.required],
        course: ['',Validators.required],
        topic: ['',Validators.required],
        standardDetails: ['',Validators.required],
      });
     
    if(this.editMode){
      this.form.patchValue({
        standardRefNo:this.editDetails.standardRefNo,
        gradeLevel: this.editDetails.gradeLevel,
        domain: this.editDetails.domain,
        subject: this.editDetails.subject,
        course: this.editDetails.course,
        topic: this.editDetails.topic,
        standardDetails: this.editDetails.standardDetails,
      })
      this.form.controls['standardRefNo'].disable();
    }
  }
  ngAfterViewInit(){

    // this.form.controls['standardRefNo'].setErrors({ 'nomatch': false });
    this.form.controls['standardRefNo'].valueChanges.pipe(debounceTime(500),distinctUntilChanged()).subscribe((term)=>{
      if(term!=''){
          this.standardRefNoModel.standardRefNo = term;
          this.checkStandardRefNo=true;
          this.gradesService.checkStandardRefNo(this.standardRefNoModel).pipe(debounceTime(500),distinctUntilChanged()).subscribe(data => {
            if(data._failure){
              this.commonService.checkTokenValidOrNot(data._message);
            }
            this.checkStandardRefNo=false;
            if (data.isValidStandardRefNo) {
              this.form.controls['standardRefNo'].setErrors(null);}
            else {
              this.form.controls['standardRefNo'].markAsTouched();
              this.form.controls['standardRefNo'].setErrors({ 'nomatch': true });
            }
          });
        
      }else{
        this.form.controls['standardRefNo'].markAsTouched();
      }
    })
  }

  // activeSubjectTextBox(){
  //   document.getElementById("subject").className='hidden';
  //   document.getElementById("subject1").classList.remove('hidden');
  //   document.getElementById("subjectFocus").focus();
  //   this.form.controls.subject.markAsTouched();
  // }

  // activeCourseTextBox(){
  //   document.getElementById("course").className='hidden';
  //   document.getElementById("course1").classList.remove('hidden');
  //   document.getElementById("courseFocus").focus();

  // }

  submit(){
    this.form.markAllAsTouched();
    if(this.form.valid){
    if(this.editMode){
      this.updateSchoolSpecificStandards();
    }else{
      this.addSchoolSpecificStandards();
    }
  }else{
    const firstElementWithError = document.querySelector('mat-select.ng-invalid, textarea.ng-invalid, input.ng-invalid .custom-scroll');
    if (firstElementWithError) {
      firstElementWithError.scrollIntoView({ behavior: 'smooth' });
    }
  }

  }

  updateSchoolSpecificStandards(){
    this.schoolSpecificStandard.gradeUsStandard.standardRefNo=this.editDetails.standardRefNo;
    this.schoolSpecificStandard.gradeUsStandard.gradeStandardId=this.editDetails.gradeStandardId;
    this.schoolSpecificStandard.gradeUsStandard.subject=this.form.value.subject;
    this.schoolSpecificStandard.gradeUsStandard.course=this.form.value.course;
    this.schoolSpecificStandard.gradeUsStandard.gradeLevel=this.form.value.gradeLevel;
    this.schoolSpecificStandard.gradeUsStandard.domain=this.form.value.domain;
    this.schoolSpecificStandard.gradeUsStandard.topic=this.form.value.topic;
    this.schoolSpecificStandard.gradeUsStandard.standardDetails=this.form.value.standardDetails;

    this.gradesService.updateGradeUsStandard(this.schoolSpecificStandard).subscribe((res)=>{
      if (typeof (res) == 'undefined') {
        this.snackbar.open('Failed to Update School Specific Standard ' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }else
    if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
        this.snackbar.open( res._message, '', {
          duration: 10000
        });
      } else {
        this.snackbar.open(res._message , '', {
          duration: 10000
        });
        this.dialogRef.close(true);
      }
    });
  }

  addSchoolSpecificStandards(){
    this.schoolSpecificStandard.gradeUsStandard.standardRefNo=this.form.value.standardRefNo;
    this.schoolSpecificStandard.gradeUsStandard.subject=this.form.value.subject;
    this.schoolSpecificStandard.gradeUsStandard.course=this.form.value.course;
    this.schoolSpecificStandard.gradeUsStandard.gradeLevel=this.form.value.gradeLevel;
    this.schoolSpecificStandard.gradeUsStandard.domain=this.form.value.domain;
    this.schoolSpecificStandard.gradeUsStandard.topic=this.form.value.topic;
    this.schoolSpecificStandard.gradeUsStandard.standardDetails=this.form.value.standardDetails;

    this.gradesService.addGradeUsStandard(this.schoolSpecificStandard).subscribe((res)=>{
      if (typeof (res) == 'undefined') {
        this.snackbar.open('Failed to Add School Specific Standard ' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }else
    if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
        this.snackbar.open(res._message, '', {
          duration: 10000
        });
      } else {
        this.snackbar.open(res._message, '', {
          duration: 10000
        });
        this.dialogRef.close(true);
      }
    });

  }

  ngOnDestroy(){
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }
}
