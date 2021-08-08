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

import { Component, Inject, OnInit, Optional } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import icClose from '@iconify/icons-ic/twotone-close';
import { fadeInUp400ms } from '../../../../../@vex/animations/fade-in-up.animation';
import { stagger60ms } from '../../../../../@vex/animations/stagger.animation';
import {AddGradeLevelModel, GelAllGradeEquivalencyModel} from '../../../../models/grade-level.model';
import { GradeLevelService } from '../../../../services/grade-level.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AgeRangeList, EducationalStage } from '../../../../models/common.model';
import { CommonService } from 'src/app/services/common.service';

@Component({
  selector: 'vex-edit-grade-levels',
  templateUrl: './edit-grade-levels.component.html',
  styleUrls: ['./edit-grade-levels.component.scss'],
  animations: [
    stagger60ms,
    fadeInUp400ms
  ]
})
export class EditGradeLevelsComponent implements OnInit {
  icClose = icClose;
  form: FormGroup;
  addGradeLevelData:AddGradeLevelModel = new AddGradeLevelModel();
  updateGradeLevelData:AddGradeLevelModel = new AddGradeLevelModel();
  getGradeEquivalencyList:GelAllGradeEquivalencyModel = new GelAllGradeEquivalencyModel();
  ageRangeList:AgeRangeList = new AgeRangeList();
  educationStageList:EducationalStage = new EducationalStage();
  nextGradeLevelList:[];
  editMode:boolean;
  modalTitle="addGradeLevel";
  modalActionButtonTitle="submit";
  editDetails;
  nextGradeIdInString:string;
  constructor(private dialogRef: MatDialogRef<EditGradeLevelsComponent>,
    @Optional() @Inject(MAT_DIALOG_DATA) public data: any,
     private fb: FormBuilder,
     private gradeLevelService:GradeLevelService,
    private commonService: CommonService,
     private snackbar: MatSnackBar) {
    }

  ngOnInit(): void {
      this.getGradeEquivalencyList=this.data.gradeLevelEquivalencyList;
      this.ageRangeList=this.data.ageRangeList;
      this.educationStageList=this.data.educationalStage;
      if(this.data.editMode){
      this.editMode = this.data.editMode;
      this.editDetails = this.data.editDetails;
      this.nextGradeLevelList=this.data.gradeLevels;
     }else{
       this.nextGradeLevelList=this.data.gradeLevels;
     }
    
      this.form = this.fb.group(
      {
        title: ['', [Validators.required, Validators.maxLength(50)]],
        shortName: ['', [Validators.required, Validators.maxLength(5)]],
        sortOrder: ['',[Validators.required,Validators.min(1)]],
        equivalencyId:['',Validators.required],
        ageRangeId:["null"],
        iscedCode:["null"],
        nextGradeId: ["null"]
      });
      
    if(this.editMode){
      this.modalTitle="editGradeLevel"
      this.modalActionButtonTitle="update";
      this.callGradeLevelView();
    }
  }

  callGradeLevelView() {
    this.form.controls.title.patchValue(this.editDetails.title);
    this.form.controls.shortName.patchValue(this.editDetails.shortName);
    this.form.controls.sortOrder.patchValue(this.editDetails.sortOrder);
    this.form.controls.equivalencyId.patchValue(this.editDetails.equivalencyId);
    if (this.editDetails.ageRangeId === null) {
      this.form.controls.ageRangeId.patchValue(this.editDetails.ageRangeId = 'null');
    } else {
      this.form.controls.ageRangeId.patchValue(this.editDetails.ageRangeId);
    }
    if (this.editDetails.iscedCode === null) {
      this.form.controls.iscedCode.patchValue(this.editDetails.iscedCode = 'null');
    } else {
      this.form.controls.iscedCode.patchValue(this.editDetails.iscedCode);
    }
    if (this.editDetails.nextGradeId === null) {
      this.form.controls.nextGradeId.patchValue(this.editDetails.nextGradeId = 'null');
    } else {
      this.form.controls.nextGradeId.patchValue(this.editDetails.nextGradeId);
    }
  }

  disableNextGrade(nextLevel) {
    let status = false;
    this.nextGradeLevelList.map((element: any) => {
      if (element.nextGradeId === nextLevel.gradeId) {
        return status = true;
      }
    });
    return status;
  }

  submit() {
    this.form.markAllAsTouched();
    if (this.form.valid) {
      if (this.editMode) {
        this.updateGradeLevel();
      } else {
        this.addGradeLevel();
      }
    }
  }

  getGradeEquivalency(){
    this.gradeLevelService.getAllGradeEquivalency(this.getGradeEquivalencyList).subscribe((res)=>{
      if (typeof (res) == 'undefined') {
        this.snackbar.open(sessionStorage.getItem("httpError"), '', {
          duration: 10000
        });
      }
      else {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          this.getGradeEquivalencyList= new GelAllGradeEquivalencyModel();
          if (!res.gradeEquivalencyList) {
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
        }
        else {
          this.getGradeEquivalencyList = res;
        }
      }
    })
  }

  addGradeLevel(){
    this.addGradeLevelData.tblGradelevel.schoolId=+sessionStorage.getItem("selectedSchoolId");;
    this.addGradeLevelData.tblGradelevel.title = this.form.value.title;
    this.addGradeLevelData.tblGradelevel.shortName =this.form.value.shortName;
    this.addGradeLevelData.tblGradelevel.sortOrder=this.form.value.sortOrder;

    this.form.value.equivalencyId=='null'?
    this.addGradeLevelData.tblGradelevel.equivalencyId = null:this.addGradeLevelData.tblGradelevel.equivalencyId = this.form.value.equivalencyId

    this.form.value.ageRangeId=='null'?
    this.addGradeLevelData.tblGradelevel.ageRangeId = null:this.addGradeLevelData.tblGradelevel.ageRangeId = +this.form.value.ageRangeId

    this.form.value.iscedCode=='null'?
    this.addGradeLevelData.tblGradelevel.iscedCode = null:this.addGradeLevelData.tblGradelevel.iscedCode = +this.form.value.iscedCode
   
    this.form.value.nextGradeId=='null'?
    this.addGradeLevelData.tblGradelevel.nextGradeId = null:this.addGradeLevelData.tblGradelevel.nextGradeId = this.form.value.nextGradeId
   
  
    this.addGradeLevelData._token=sessionStorage.getItem("token");
    this.addGradeLevelData._tenantName=sessionStorage.getItem("tenant");
    this.gradeLevelService.addGradelevel(this.addGradeLevelData).subscribe((res)=>{
      if (typeof (res) == 'undefined') {
        this.snackbar.open('Failed to Add Grade Level ' + sessionStorage.getItem("httpError"), '', {
          duration: 10000
        });
      }else
    if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
        this.snackbar.open( res._message, '', {
          duration: 10000
        });
      } else {
        this.snackbar.open(res._message, '', {
          duration: 10000
        });
        this.dialogRef.close(true);
      }
    })
  }

  updateGradeLevel(){
    this.updateGradeLevelData.tblGradelevel.schoolId = this.editDetails.schoolId;
    this.updateGradeLevelData.tblGradelevel.gradeId = this.editDetails.gradeId;
    this.updateGradeLevelData._tenantName=sessionStorage.getItem("tenant");
    this.updateGradeLevelData._token=sessionStorage.getItem("token");
    this.updateGradeLevelData.tblGradelevel.title=this.form.value.title;
    this.updateGradeLevelData.tblGradelevel.shortName=this.form.value.shortName;
    this.updateGradeLevelData.tblGradelevel.sortOrder=this.form.value.sortOrder;
    this.form.value.equivalencyId=='null'?
    this.updateGradeLevelData.tblGradelevel.equivalencyId = null:this.updateGradeLevelData.tblGradelevel.equivalencyId = this.form.value.equivalencyId;

    this.form.value.ageRangeId=='null'?
    this.updateGradeLevelData.tblGradelevel.ageRangeId = null:this.updateGradeLevelData.tblGradelevel.ageRangeId = +this.form.value.ageRangeId;

    this.form.value.iscedCode=='null'?
    this.updateGradeLevelData.tblGradelevel.iscedCode = null:this.updateGradeLevelData.tblGradelevel.iscedCode = +this.form.value.iscedCode;
   
    this.form.value.nextGradeId=='null'?
    this.updateGradeLevelData.tblGradelevel.nextGradeId = null:this.updateGradeLevelData.tblGradelevel.nextGradeId = this.form.value.nextGradeId;

    this.gradeLevelService.updateGradelevel(this.updateGradeLevelData).subscribe((res)=>{
      if (typeof (res) == 'undefined') {
        this.snackbar.open('Failed to Update Grade Level ' + sessionStorage.getItem("httpError"), '', {
          duration: 10000
        });
      }else if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
        this.snackbar.open( res._message, '', {
          duration: 10000
        });
      } else {
        this.snackbar.open(res._message, '', {
          duration: 10000
        }
        );
        this.dialogRef.close(true);
      }
    })
  }

 

}
