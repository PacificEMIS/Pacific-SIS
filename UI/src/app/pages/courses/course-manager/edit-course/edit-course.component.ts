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

import { Component, OnInit ,Inject,ViewChild} from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, NgForm, Validators ,FormGroup} from '@angular/forms';
import icClose from '@iconify/icons-ic/twotone-close';
import icEdit from '@iconify/icons-ic/twotone-edit';
import icDelete from '@iconify/icons-ic/twotone-delete';
import icAdd from '@iconify/icons-ic/twotone-add';
import icList from '@iconify/icons-ic/twotone-list-alt';
import icInfo from '@iconify/icons-ic/info';
import icRemove from '@iconify/icons-ic/remove-circle';
import icBack from '@iconify/icons-ic/baseline-arrow-back';
import icExpand from '@iconify/icons-ic/outline-add-box';
import icCollapse from '@iconify/icons-ic/outline-indeterminate-check-box';
import { fadeInUp400ms } from '../../../../../@vex/animations/fade-in-up.animation';
import { stagger60ms,stagger40ms } from '../../../../../@vex/animations/stagger.animation';
import {AddCourseModel,GetAllProgramModel,GetAllSubjectModel,GetAllCourseListModel,CourseStandardModel, SubjectModel, ProgramsModel} from '../../../../models/course-manager.model';
import {CourseManagerService} from '../../../../services/course-manager.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import {GradeLevelService} from '../../../../services/grade-level.service';
import {GetAllGradeLevelsModel } from '../../../../models/grade-level.model';
import {MassUpdateProgramModel,MassUpdateSubjectModel} from '../../../../models/course-manager.model';
import { MatDialog } from '@angular/material/dialog';
import { GetAllSchoolSpecificListModel, GradeStandardSubjectCourseListModel, SchoolSpecificStandarModel, StandardView } from '../../../../models/grades.model';
import { GradesService } from '../../../../services/grades.service';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { CommonService } from 'src/app/services/common.service';
import { MatPaginator } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { LoaderService } from 'src/app/services/loader.service';
import {animate, state, style, transition, trigger} from '@angular/animations';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { MatCheckbox } from '@angular/material/checkbox';
@Component({
  selector: 'vex-edit-course',
  templateUrl: './edit-course.component.html',
  styleUrls: ['./edit-course.component.scss'],
  animations: [
    stagger60ms,
    fadeInUp400ms,
    stagger40ms,
    trigger('detailExpand', [
      state('collapsed', style({height: '0px', minHeight: '0'})),
      state('expanded', style({height: '*'})),
      transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
  ]
})
export class EditCourseComponent implements OnInit {
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  displayedColumns: string[] = ['studentCheck', 'standard_ref_no', 'topic', 'standard_details'];

  expandedElement;
  @ViewChild('f') currentForm: NgForm;
  icClose = icClose;
  icEdit = icEdit;
  icDelete = icDelete;
  icAdd = icAdd;
  icList = icList;
  icInfo = icInfo;
  icRemove = icRemove;
  icBack = icBack;
  icExpand = icExpand;
  icCollapse = icCollapse;
  addStandard = false;
  addCourseModel: AddCourseModel = new AddCourseModel();
  getAllProgramModel: GetAllProgramModel = new GetAllProgramModel();
  getAllSubjectModel: GetAllSubjectModel = new GetAllSubjectModel();
  getAllGradeLevelsModel:GetAllGradeLevelsModel= new GetAllGradeLevelsModel();
  massUpdateProgramModel:MassUpdateProgramModel =  new MassUpdateProgramModel();
  massUpdateSubjectModel:MassUpdateSubjectModel =  new MassUpdateSubjectModel();
  getAllCourseListModel: GetAllCourseListModel = new GetAllCourseListModel();
  schoolSpecificStandardsList:GetAllSchoolSpecificListModel=new GetAllSchoolSpecificListModel();
  gradeStandardSubjectList:GradeStandardSubjectCourseListModel=new GradeStandardSubjectCourseListModel();
  gradeStandardCourseList:GradeStandardSubjectCourseListModel=new GradeStandardSubjectCourseListModel();
  programList=[];
  subjectList=[];
  courseList=[];
  totalCount: number=0;
  pageNumber: number;
  pageSize: number;
  gradeLevelList=[];
  addProgramMode=false;
  addSubjectMode=false;
  gStdSubjectList;
  gStdCourseList;
  form:FormGroup;
  schoolSpecificList=[];
  schoolSpecificListCount=0;
  checkedStandardList=[];
  updatedCheckedStandardList=[];
  courseId;
  commonCoreStandardsModelList;
  courseModalTitle="addCourse";
  courseModalActionTitle="submit";
  checkAllNonTrades: boolean = false
  ischecked: boolean = false
  checkAllTrades: boolean = false;
  currentStandardDetailsIndex:number;
  nonDuplicateCheckedStandardList=[];
  addNewProgramFlag:boolean=false;
  addNewSubjectFlag:boolean=false;
  gradeSubjectCourse=[];
  showUsCommonCoreStandards: boolean = true;
  gradeStandardId: any;
  loading:boolean;
  destroySubject$: Subject<void> = new Subject();
  @ViewChild('masterCheckBox') masterCheckBox: MatCheckbox;

  constructor(
    private courseManager: CourseManagerService,
    private snackbar: MatSnackBar,
    private dialog: MatDialog,
    private fb: FormBuilder,
    private gradeLevelService: GradeLevelService,
    public defaultValuesService: DefaultValuesService,
    private gradesService: GradesService,
    private dialogRef: MatDialogRef<EditCourseComponent>,
    private loaderService: LoaderService,
    private commonService: CommonService,
    @Inject(MAT_DIALOG_DATA) public data) {
      this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((currentState) => {
      this.loading = currentState;
    });
      this.gStdSubjectList= this.data.gStdSubjectList;
        this.gStdCourseList= this.data.gStdCourseList;
        this.programList= this.data.programList;
        this.subjectList= this.data.subjectList;
        this.courseList= this.data.courseList;
        this.gradeLevelList= this.data.gradeLevelList;
  }
  ngOnInit(): void {
    this.form = this.fb.group({
      subject:[''],
      course:[''],
      gradeLevel:[''],
    })
    if(this.data.mode === "EDIT"){
      this.courseModalTitle="editCourse";
        this.courseModalActionTitle="update"  
        delete this.data.editDetails?.academicYear;
      this.addCourseModel.course = this.data.editDetails;
      this.addCourseModel.course.creditHours = this.addCourseModel.course.creditHours ? parseFloat(this.addCourseModel.course.creditHours).toFixed(3) : null;
      this.courseId = this.data.editDetails.courseId;
      if(this.data.editDetails.courseStandard !== undefined){
        if(this.data.editDetails.courseStandard.length > 0){
          this.data.editDetails.courseStandard.forEach(value=>{
            let obj1={};
            obj1["tenantId"] = value.gradeUsStandard.tenantId;
            obj1["schoolId"] = value.gradeUsStandard.schoolId;
            obj1["standardRefNo"] = value.gradeUsStandard.standardRefNo;
            obj1["gradeStandardId"]= value.gradeUsStandard.gradeStandardId;
            obj1["gradeLevel"] = value.gradeUsStandard.gradeLevel;
            obj1["domain"] = value.gradeUsStandard.domain;
            obj1["subject"] = value.gradeUsStandard.subject;
            obj1["course"] = value.gradeUsStandard.course;
            obj1["topic"] = value.gradeUsStandard.topic;
            obj1["standardDetails"] = value.gradeUsStandard.standardDetails;
            obj1["isSchoolSpecific"] = value.gradeUsStandard.isSchoolSpecific;
            obj1["createdBy"] = value.gradeUsStandard.createdBy;
            obj1["createdOn"] = value.gradeUsStandard.createdOn;
            obj1["updatedBy"] = value.gradeUsStandard.updatedBy;
            obj1["updatedOn"] =value.gradeUsStandard.updatedOn;
            obj1["courseId"] =value.courseId;
            obj1["checked"] = true;
            this.updatedCheckedStandardList.push(obj1);
            this.checkedStandardList.push(obj1);
            this.nonDuplicateCheckedStandardList.push(obj1);
          });
        }
      }
      
      
    }
  }

  filterSchoolSpecificStandardsList(){
    this.form.markAllAsTouched();
      let filterParams= [
        {
          columnName: "subject",
          filterValue: this.form.value.subject,
          filterOption: 11
        },
        {
          columnName: "course",
          filterValue: this.form.value.course,
          filterOption: 11
        },
        {
          columnName: "gradeLevel",
          filterValue: this.form.value.gradeLevel=="all"?null:this.form.value.gradeLevel,
          filterOption: 11
        }
      ]
      
      
      
        Object.assign(this.schoolSpecificStandardsList, { filterParams: filterParams });
       
      
      this.getAllSchoolSpecificList(); 
  }
  showStandardDetails(index){   
    this.currentStandardDetailsIndex=index;
  } 
  
  goToCourse(){
    this.addStandard = false;
    this.nonDuplicateCheckedStandardList = this.checkedStandardList.reduce((unique, o) => {
      if(!unique.some(obj => obj.gradeStandardId === o.gradeStandardId)) {
        unique.push(o);
      }
      return unique;
  },[]);
   
  }
  removeStandard(checkedStandard){   
    let findIndexArray = this.nonDuplicateCheckedStandardList.findIndex(x => x.gradeStandardId === checkedStandard.gradeStandardId);
    this.nonDuplicateCheckedStandardList.splice(findIndexArray, 1);
  }
  getAllSchoolSpecificList(){   
    if(this.form.valid){
      this.schoolSpecificStandardsList.sortingModel = null; 
      this.schoolSpecificStandardsList.IsSchoolSpecific = !this.showUsCommonCoreStandards;
      this.gradesService.getAllGradeUsStandardList(this.schoolSpecificStandardsList).subscribe(res => {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          if(!res.gradeUsStandardList){
            this.snackbar.open( res._message, '', {
              duration: 10000
            });
          }
        } else {  
          this.schoolSpecificList=[];
            res.gradeUsStandardList.forEach(val=>{
              let obj2={};
              obj2["tenantId"] = val.tenantId;
              obj2["schoolId"] = val.schoolId;
              obj2["standardRefNo"] = val.standardRefNo;
              obj2["gradeStandardId"] = val.gradeStandardId;
              obj2["gradeLevel"] = val.gradeLevel;
              obj2["domain"] = val.domain;
              obj2["subject"] = val.subject;
              obj2["course"] = val.course;
              obj2["topic"] = val.topic;
              obj2["standardDetails"] = val.standardDetails;
              obj2["checked"] = false;
              this.schoolSpecificList.push(obj2)
            });
            this.schoolSpecificList.map((item)=>{
              this.nonDuplicateCheckedStandardList.map((standard)=>{
                if(standard.gradeStandardId==item.gradeStandardId){
                  item.checked=true;
                }
              });
            });
          this.totalCount = res.totalCount;
          this.pageNumber = res.pageNumber;
          this.pageSize = res._pageSize;
          this.commonCoreStandardsModelList = new MatTableDataSource(this.schoolSpecificList);              
          this.schoolSpecificListCount = res.gradeUsStandardList.length;     
          if(this.schoolSpecificListCount === 0){
            this.schoolSpecificList = [];
            this.gradeSubjectCourse=[];
          } 
        }
      });  
      
    }
    
  }

// page event
  getPageEvent(event) {
    this.schoolSpecificStandardsList.pageNumber = event.pageIndex + 1;
    this.schoolSpecificStandardsList.pageSize = event.pageSize;
    this.getAllSchoolSpecificList();
  }

  someComplete(){
    let indetermine = false;
    for (let standard of this.schoolSpecificList) {
      for (let selectedStandard of this.checkedStandardList) {
        if (standard.gradeStandardId === selectedStandard.gradeStandardId) {
          indetermine = true;
        }
      }
    }
    if (indetermine) {
      // this.masterCheckBox.checked = this.schoolSpecificList.every((item) => {
      //   return item.checked;
      // })
      // if (this.masterCheckBox.checked) {
      //   return false;
      // } else {
      //   return true;
      // }
    }
  }

  setAll(event){
    this.schoolSpecificList.forEach(item => {
      item.checked = event;
    });
    this.commonCoreStandardsModelList = new MatTableDataSource(this.schoolSpecificList);
    this.decideCheckUncheck();
  }

  onChangeSelection(eventStatus: boolean, id){
    for (let item of this.schoolSpecificList) {
      if (item.gradeStandardId === id) {
        item.checked = eventStatus;
        break;
      }
    }
    this.commonCoreStandardsModelList = new MatTableDataSource(this.schoolSpecificList);
    this.masterCheckBox.checked = this.schoolSpecificList.every((item) => {
      return item.checked;
    });

    this.decideCheckUncheck();
  }


  decideCheckUncheck() {
    this.schoolSpecificList.map((item) => {
      let isIdIncludesInSelectedList = false;
      if (item.checked) {
        for (let selectedUser of this.checkedStandardList) {
          if (item.gradeStandardId == selectedUser.gradeStandardId) {
            isIdIncludesInSelectedList = true;
            break;
          }
        }
        if (!isIdIncludesInSelectedList) {
          this.checkedStandardList.push(item);
        }
      } else {
        for (let selectedUser of this.checkedStandardList) {
          if (item.gradeStandardId == selectedUser.gradeStandardId) {
            this.checkedStandardList = this.checkedStandardList.filter((user) => user.gradeStandardId != item.gradeStandardId);
            break;
          }
        }
      }
      isIdIncludesInSelectedList = false;

    });
    this.checkedStandardList = this.checkedStandardList.filter((item) => item.checked);
  }

  changeCategory(status) {
    this.totalCount=0;
    this.commonCoreStandardsModelList=new MatTableDataSource([]);
    this.showUsCommonCoreStandards = status;
    if(status) {
      this.form.controls.gradeLevel.setValidators(Validators.required);
      this.form.controls.subject.clearValidators();
      this.form.controls.course.clearValidators();
    } else {
      this.form.controls.gradeLevel.setValidators(Validators.required);
      this.form.controls.subject.setValidators(Validators.required);
      this.form.controls.course.setValidators(Validators.required);
    }
    this.form.controls.gradeLevel.updateValueAndValidity();
    this.form.controls.subject.updateValueAndValidity();
    this.form.controls.course.updateValueAndValidity();
  }

  saveProgram(){
    document.getElementById("program").className='hidden';
    document.getElementById("program1").classList.remove('hidden');
    document.getElementById("courseProgram").focus();
    this.currentForm.controls.courseProgram.markAsTouched();
    this.addNewProgramFlag = true;
    
  }
  saveSubject(){
    document.getElementById("subject").className='hidden';
    document.getElementById("subject1").classList.remove('hidden');
    document.getElementById("subjectFocus").focus();
    this.currentForm.controls.courseSubject.markAsTouched();
    this.addNewSubjectFlag = true;
  }
  submit(){
    if (this.currentForm.form.valid) {
      if(this.addProgramMode){
        let obj =new ProgramsModel();
        obj.programId = 0
        obj.programName = this.addCourseModel.course.courseProgram;
        obj.tenantId = this.defaultValuesService.getTenantID();
        obj.schoolId = this.defaultValuesService.getSchoolID();  
        obj.createdBy = this.defaultValuesService.getUserGuidId();       
        obj.updatedBy = this.defaultValuesService.getUserGuidId();       
        this.massUpdateProgramModel.programList.push(obj); 
        this.courseManager.AddEditPrograms(this.massUpdateProgramModel).subscribe(data => {     
          if(typeof(data)=='undefined'){
            this.snackbar.open('Program Submission failed. ' + this.defaultValuesService.getHttpError(), '', {
              duration: 10000
            });
          }
          else{
           if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
              this.snackbar.open( data._message, '', {
                duration: 10000
              });
            } 
            else{       
              
              this.snackbar.open(data._message, '', {
                duration: 10000
              })
            }        
          }      
        });
      }
      if (this.addSubjectMode){
        let courseObj = new SubjectModel();
        courseObj.subjectId = 0;
        courseObj.subjectName = this.addCourseModel.course.courseSubject;
        courseObj.tenantId = this.defaultValuesService.getTenantID();
        courseObj.schoolId = this.defaultValuesService.getSchoolID();
        courseObj.createdBy = this.defaultValuesService.getUserGuidId();
        courseObj.updatedBy =  this.defaultValuesService.getUserGuidId();
        this.massUpdateSubjectModel.subjectList.push(courseObj);
        this.courseManager.AddEditSubject(this.massUpdateSubjectModel).subscribe(
          data => {
            if (data){
             if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
                this.snackbar.open( data._message, '', {
                  duration: 10000
                });
              }
              else{
                this.snackbar.open(data._message, '', {
                  duration: 10000
                });
              }
            }else{
              this.snackbar.open('Subject Submission failed. ' + this.defaultValuesService.getHttpError(), '', {
                duration: 10000
              });
            }
        });
      }
      if (this.addNewProgramFlag){
        this.addCourseModel.programId = 0;
      }
      if (this.addNewSubjectFlag){
        this.addCourseModel.subjectId = 0;
      }
      if (this.data.mode === "EDIT"){
        this.addCourseModel.course.courseStandard = [new CourseStandardModel()];
        if(this.nonDuplicateCheckedStandardList.length > 0){
          this.nonDuplicateCheckedStandardList.forEach(val => {
            let obj: CourseStandardModel;
            obj = new CourseStandardModel();

            obj.tenantId = this.defaultValuesService.getTenantID();
            obj.schoolId = this.defaultValuesService.getSchoolID();
            if(val.hasOwnProperty("courseId")){
              obj.courseId = val.courseId;
            }else{
              obj.courseId = this.courseId;
            }
            obj.standardRefNo = val.standardRefNo;
            obj.gradeStandardId = val.gradeStandardId;
            obj.createdBy = this.defaultValuesService.getUserGuidId();
            this.addCourseModel.course.courseStandard.push(obj);
          });
        }
        this.addCourseModel.course.courseStandard.splice(0, 1);
        this.courseManager.UpdateCourse(this.addCourseModel).subscribe(data => {
          if (data){
           if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
              this.snackbar.open( data._message, '', {
                duration: 10000
              });
            } else {
              this.snackbar.open(data._message, '', {
                duration: 10000
              });
              this.dialogRef.close(true);
            }
          }else{
            this.snackbar.open('Course Updation failed. ' + this.defaultValuesService.getHttpError(), '', {
              duration: 10000
            });
          }
        });
      }else{
        if (this.checkedStandardList.length > 0){
          this.checkedStandardList.forEach(val => {
            let obj: CourseStandardModel;
            obj = new CourseStandardModel();
            obj.tenantId = this.defaultValuesService.getTenantID();
            obj.schoolId = this.defaultValuesService.getSchoolID();
            obj.courseId = 0;
            obj.standardRefNo = val.standardRefNo;
            obj.gradeStandardId = val.gradeStandardId;
            obj.createdBy = this.defaultValuesService.getUserGuidId();
            this.addCourseModel.course.courseStandard.push(obj);
          });
        }
        this.addCourseModel.course.courseStandard.splice(0, 1);
        this.addCourseModel.grade_standard_id = this.gradeStandardId;
        this.courseManager.AddCourse(this.addCourseModel).subscribe(data => {
          if (data){
           if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
              this.snackbar.open( data._message, '', {
                duration: 10000
              });
            } else {
              this.snackbar.open(data._message, '', {
                duration: 10000
              });
              this.dialogRef.close(true);
            }
          }else{
            this.snackbar.open('Course Submission failed. ' + this.defaultValuesService.getHttpError(), '', {
              duration: 10000
            });
          }
        });
      }
    }
  }
  selectStandards() {
    this.currentForm.form.controls.courseTitle.markAllAsTouched();
    if (this.currentForm.form.controls.courseTitle.value === undefined) {
      this.currentForm.controls.courseTitle.setErrors({ required: true });
    }
    this.addStandard = true;
    this.schoolSpecificList = [];
    this.gradeSubjectCourse = [];
    this.schoolSpecificListCount = 0;
    this.totalCount= 0;
    this.form.reset();
  }

  changeCouse(event) {
    const data = this.gStdCourseList.find(x => x.course === event.value);
    this.gradeStandardId = data.gradeStandardId;
  }

  closeStandardsSelection(){
    this.addStandard = false;
  }

  checkValidCreditHour(val){
    if(val<0){
      this.addCourseModel.course.creditHours = Math.abs(val) + '';
    }
  }

  //credit hours 3 decimal places
  onCrediHoursBlur(event){
    if(event.target.value !== ''){
      this.addCourseModel.course.creditHours = parseFloat(this.addCourseModel.course.creditHours).toFixed(3)
    }
    }

    checkInputAndPrevent(event) {
      ["e", "E", "+", "-"].includes(event.key) && event.preventDefault();
    }

}
