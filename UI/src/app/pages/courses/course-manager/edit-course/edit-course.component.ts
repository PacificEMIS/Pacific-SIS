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
import { stagger60ms } from '../../../../../@vex/animations/stagger.animation';
import {AddCourseModel,GetAllProgramModel,GetAllSubjectModel,GetAllCourseListModel,CourseStandardModel} from '../../../../models/course-manager.model';
import {CourseManagerService} from '../../../../services/course-manager.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import {GradeLevelService} from '../../../../services/grade-level.service';
import {GetAllGradeLevelsModel } from '../../../../models/grade-level.model';
import {MassUpdateProgramModel,MassUpdateSubjectModel} from '../../../../models/course-manager.model';
import { MatDialog } from '@angular/material/dialog';
import { GetAllSchoolSpecificListModel, GradeStandardSubjectCourseListModel, SchoolSpecificStandarModel, StandardView } from '../../../../models/grades.model';
import { GradesService } from '../../../../services/grades.service';
import { LayoutService } from 'src/@vex/services/layout.service';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { CommonService } from 'src/app/services/common.service';
@Component({
  selector: 'vex-edit-course',
  templateUrl: './edit-course.component.html',
  styleUrls: ['./edit-course.component.scss'],
  animations: [
    stagger60ms,
    fadeInUp400ms
  ]
})
export class EditCourseComponent implements OnInit {
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
  constructor(
    private courseManager: CourseManagerService,
    private snackbar: MatSnackBar,
    private dialog: MatDialog,
    private fb: FormBuilder,
    private gradeLevelService: GradeLevelService,
    private defaultValuesService: DefaultValuesService,
    private gradesService: GradesService,
    private dialogRef: MatDialogRef<EditCourseComponent>,
    private layoutService: LayoutService,
    private commonService: CommonService,
    @Inject(MAT_DIALOG_DATA) public data) {
      this.layoutService.collapseSidenav();
  }
  ngOnInit(): void {
    this.form = this.fb.group({
      subject:['',[Validators.required]],
      course:['',[Validators.required]],
      gradeLevel:['',[Validators.required]],
    })
    if(this.data.mode === "EDIT"){
      this.courseModalTitle="editCourse";
        this.courseModalActionTitle="update"     
      this.addCourseModel.course = this.data.editDetails;
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
            this.updatedCheckedStandardList.push(obj1);
            this.checkedStandardList.push(obj1);
            this.nonDuplicateCheckedStandardList.push(obj1);
          });
        }
      }
      
      
    }
    this.getAllProgramList();
    this.getAllSubjectList();
    this.getAllGradeLevelList();
    this.getAllCourse();
    this.getAllSubjectStandardList();
    this.getAllCourseStandardList();
  }
  getAllProgramList(){   
    this.courseManager.GetAllProgramsList(this.getAllProgramModel).subscribe(data => {          
      if(data){
       if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
          this.programList=[];
          if(!data.programList){
            this.snackbar.open(data._message, '', {
              duration: 1000
            }); 
          }
        }else{
          this.programList=data.programList;
        }
      }else{
        this.snackbar.open(sessionStorage.getItem('httpError'), '', {
          duration: 1000
        }); 
      }     
    });
  }
  getAllSubjectList(){   
    this.courseManager.GetAllSubjectList(this.getAllSubjectModel).subscribe(data => {          
      if(data){
       if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
          this.subjectList=[];
          if(!data.subjectList){
            this.snackbar.open(data._message, '', {
              duration: 1000
            }); 
          }
        }else{
          this.subjectList=data.subjectList;
        }
      }else{
        this.snackbar.open(sessionStorage.getItem('httpError'), '', {
          duration: 1000
        }); 
      }     
    });
  }
  getAllGradeLevelList(){ 
    this.gradeLevelService.getAllGradeLevels(this.getAllGradeLevelsModel).subscribe(data => {  
      if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
        }        
      this.gradeLevelList=data.tableGradelevelList;      
    });
  }
  checkAllStandard(ev){
  
    if(ev.checked){
      this.schoolSpecificList.forEach(val=>{
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
        this.checkedStandardList.push(obj2)
      })     
      this.schoolSpecificList.forEach(item => item.selected = true);
     
    }else{
      this.checkedStandardList = [];
      this.schoolSpecificList.forEach(item => item.selected = false);
    }
  }
  singleCheckbox(event,data) {
    if(event.checked){
      this.checkedStandardList.push(data);
    }else{
      let findIndexArray = this.checkedStandardList.findIndex(x => x.gradeStandardId === data.gradeStandardId);
      this.checkedStandardList.splice(findIndexArray, 1);
    }

  }

 
  getAllSubjectStandardList(){    
    this.gradesService.getAllSubjectStandardList(this.gradeStandardSubjectList).subscribe((res) => {
      if (typeof (res) == 'undefined') {
        this.snackbar.open('Standard Subject List failed. ' + sessionStorage.getItem("httpError"), '', {
          duration: 10000
        });
      }
      else {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          this.gStdSubjectList=[];
            if (!res.gradeUsStandardList) {
              this.snackbar.open(res._message, '', {
                duration: 10000
              });
            }
        }
        else {
          this.gStdSubjectList=res.gradeUsStandardList;         
        }
      }
    })
  }
  getAllCourseStandardList(){   
    this.gradesService.getAllCourseStandardList(this.gradeStandardCourseList).subscribe((res) => {
      if (typeof (res) == 'undefined') {
        this.snackbar.open('Standard Course List failed. ' + sessionStorage.getItem("httpError"), '', {
          duration: 10000
        });
      }
      else {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          this.gStdCourseList=[];
            if (!res.gradeUsStandardList) {
              this.snackbar.open(res._message, '', {
                duration: 10000
              });
            }
        }
        else {
          this.gStdCourseList=res.gradeUsStandardList;
        }
      }
    })
  }
  getAllCourse(){
    this.courseManager.GetAllCourseList(this.getAllCourseListModel).subscribe(data => {
      if(data){
       if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
          this.courseList=[];
          if(!data.courseViewModelList){
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
          }
        }else{      
          this.courseList=data.courseViewModelList;              
        }
      }else{
        this.snackbar.open(sessionStorage.getItem("httpError"), '', {
          duration: 10000
        });
      }
    });
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
          filterValue: this.form.value.gradeLevel,
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
    if(this.form.value.subject !== null && this.form.value.course !== null && this.form.value.gradeLevel !== null){
      this.schoolSpecificStandardsList.sortingModel = null; 
      this.gradesService.getAllGradeUsStandardList(this.schoolSpecificStandardsList).subscribe(res => {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          if(!res.gradeUsStandardList){
            this.snackbar.open( res._message, '', {
              duration: 10000
            });
          }
        } else {    
          let obj={
            subject: this.form.value.subject,
            course: this.form.value.course,
            grade: this.form.value.gradeLevel
          }
          this.gradeSubjectCourse.push(obj);
          let Ids = [];
          for (let [i, val] of this.gradeSubjectCourse.entries()) {
            Ids[i] =  this.gradeSubjectCourse[i].subject
              + this.gradeSubjectCourse[i].course
              + this.gradeSubjectCourse[i].grade
          }    
        
          let checkDuplicate = Ids.sort().some((item, i) => {
            if (item == Ids[i + 1]) {                
              return true;
            } else {
              return false;
            }
          })
          if(checkDuplicate === false){
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
              obj2["selected"] = false;
              this.schoolSpecificList.push(obj2)
            });
            this.schoolSpecificList.map((item)=>{
              this.nonDuplicateCheckedStandardList.map((standard)=>{
                if(standard.gradeStandardId==item.gradeStandardId){
                  item.selected=true;
                }
              });
            });
          }                
          this.schoolSpecificListCount = res.gradeUsStandardList.length;     
          if(this.schoolSpecificListCount === 0){
            this.schoolSpecificList = [];
            this.gradeSubjectCourse=[];
          } 
        }
      });    
    }
    
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
        let obj ={};
        obj["programId"] = 0
        obj["programName"] = this.addCourseModel.course.courseProgram;
        obj["tenantId"]= sessionStorage.getItem("tenantId");
        obj["schoolId"] = +sessionStorage.getItem("selectedSchoolId");  
        obj["createdBy"] = sessionStorage.getItem("email");       
        obj["updatedBy"]=  sessionStorage.getItem("email");       
        this.massUpdateProgramModel.programList.push(obj); 
        this.courseManager.AddEditPrograms(this.massUpdateProgramModel).subscribe(data => {     
          if(typeof(data)=='undefined'){
            this.snackbar.open('Program Submission failed. ' + sessionStorage.getItem("httpError"), '', {
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
        let courseObj = {};
        courseObj["subjectId"] = 0;
        courseObj["subjectName"] = this.addCourseModel.course.courseSubject;
        courseObj["tenantId"] = this.defaultValuesService.getTenantID();
        courseObj["schoolId"] = this.defaultValuesService.getSchoolID();
        courseObj["createdBy"] = this.defaultValuesService.getEmailId();
        courseObj["updatedBy"] =  this.defaultValuesService.getEmailId();
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
              this.snackbar.open('Subject Submission failed. ' + sessionStorage.getItem("httpError"), '', {
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
            obj.createdBy = this.defaultValuesService.getEmailId();
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
            this.snackbar.open('Course Updation failed. ' + sessionStorage.getItem("httpError"), '', {
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
            obj.createdBy = this.defaultValuesService.getEmailId();
            this.addCourseModel.course.courseStandard.push(obj);
          });
        }
        this.addCourseModel.course.courseStandard.splice(0, 1);

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
            this.snackbar.open('Course Submission failed. ' + sessionStorage.getItem("httpError"), '', {
              duration: 10000
            });
          }
        });
      }
    }
  }
  selectStandards() {
    this.currentForm.form.controls.courseTitle.markAllAsTouched();
    if (this.currentForm.form.controls.courseTitle.value === undefined){
      this.currentForm.controls.courseTitle.setErrors({ required: true });
    }else{
      this.addStandard = true;
      this.schoolSpecificList = [];
      this.gradeSubjectCourse = [];
      this.schoolSpecificListCount = 0;
      this.form.reset();
    }
  }
  closeStandardsSelection(){
    this.addStandard = false;
  }

  checkValidCreditHour(val){
    if(val<0){
      this.addCourseModel.course.creditHours=Math.abs(val);
    }
  }

}
