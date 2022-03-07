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

import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { StudentAttendanceService } from '../../../../../services/student-attendance.service';
import { AddUpdateStudentAttendanceModel, GetAllStudentAttendanceListModel, StudentAttendanceModel } from '../../../../../models/take-attendance-list.model';
import { AddCommentsComponent } from './add-comments/add-comments.component';
import { MatSnackBar } from '@angular/material/snack-bar';
import { StudentScheduleService } from '../../../../../services/student-schedule.service';
import { GetAllAttendanceCodeModel } from '../../../../../models/attendance-code.model';
import { AttendanceCodeService } from '../../../../../services/attendance-code.service';
import {SharedFunction} from '../../../../shared/shared-function'
import { LoaderService } from '../../../../../services/loader.service';
import { ScheduleStudentListViewModel } from '../../../../../models/student-schedule.model';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { CommonService } from 'src/app/services/common.service';
@Component({
  selector: 'vex-period-attendance',
  templateUrl: './period-attendance.component.html',
  styleUrls: ['./period-attendance.component.scss']
})
export class PeriodAttendanceComponent implements OnInit {
  pageStatus = "Teacher Function";
  portalAccess: boolean;
  displayedColumns: string[] = ['students', 'attendanceCodes', 'comments'];
  staffDetails;
  showShortName:boolean=false;
  getAllAttendanceCodeModel:GetAllAttendanceCodeModel=new GetAllAttendanceCodeModel()
  scheduleStudentListViewModel: ScheduleStudentListViewModel = new ScheduleStudentListViewModel()
  addUpdateStudentAttendanceModel : AddUpdateStudentAttendanceModel = new AddUpdateStudentAttendanceModel();
  isAttendanceDateToday=true;
  studentAttendanceList: GetAllStudentAttendanceListModel= new GetAllStudentAttendanceListModel();
  actionButtonTitle:string = 'submit';
  loading:boolean;
  commentsArray = [];
  getTheIndexNumbersForDroppedStudentForCourseSection=[];
  active;
  allStudentList=[];
  constructor(
    private dialog: MatDialog,
    public translateService:TranslateService,
    private studentAttendanceService: StudentAttendanceService,
    private router: Router,
    private snackbar: MatSnackBar,
    private studentScheduleService:StudentScheduleService,
    private attendanceCodeService:AttendanceCodeService,
    private commonFunction: SharedFunction,
    private defaultValuesService: DefaultValuesService,
    private commonService: CommonService,

    ) { 
    //translateService.use('en');
    this.staffDetails = this.studentAttendanceService.getStaffDetails();
    Object.keys(this.staffDetails).length > 0 ? '' : this.router.navigate(['/school', 'staff', 'teacher-functions', 'take-attendance']);

  
    this.staffDetails.attendanceDate = this.commonFunction.formatDateSaveWithoutTime(this.staffDetails.attendanceDate);
   }

  ngOnInit(): void {
    this.getScheduledStudentList();
    this.isToday();
  }

  isToday(){
   let inputDate = new Date(this.staffDetails.attendanceDate);
   let todaysDate = new Date();
    if(inputDate.setHours(0,0,0,0) != todaysDate.setHours(0,0,0,0)) {
      this.isAttendanceDateToday=false
    }
  }

  getScheduledStudentList(){
    this.scheduleStudentListViewModel.sortingModel=null;
    this.scheduleStudentListViewModel.courseSectionId=this.staffDetails.courseSectionId;
    this.scheduleStudentListViewModel.pageNumber=0;
    this.scheduleStudentListViewModel.pageSize = 0;
    this.scheduleStudentListViewModel.sortingModel= null;
    this.scheduleStudentListViewModel.attendanceDate = this.staffDetails.attendanceDate;
    this.scheduleStudentListViewModel.includeInactive = true;
    this.studentScheduleService.searchScheduledStudentForGroupDrop(this.scheduleStudentListViewModel).subscribe((res)=>{
    if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
        if(res.scheduleStudentForView==null){
          this.snackbar.open(res._message, '', {
            duration: 10000
          });
          this.scheduleStudentListViewModel.scheduleStudentForView=[];
        }else{
          this.scheduleStudentListViewModel.scheduleStudentForView=res.scheduleStudentForView;
        }
       
      } else {
        this.scheduleStudentListViewModel.scheduleStudentForView=res.scheduleStudentForView;
        this.allStudentList=res.scheduleStudentForView;
        this.getAllAttendanceCode();
        this.scheduleStudentListViewModel.scheduleStudentForView.map((x,index)=>{
          if(x.isDropped === true){
            this.getTheIndexNumbersForDroppedStudentForCourseSection.push(index)
          }
         })
      }
    })
  }

  getAllAttendanceCode() {
    this.getAllAttendanceCodeModel.attendanceCategoryId=this.staffDetails.attendanceCategoryId;
    this.attendanceCodeService.getAllAttendanceCode(this.getAllAttendanceCodeModel).subscribe((res: any)=>{
     if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
        if(res.attendanceCodeList===null){
          this.getAllAttendanceCodeModel.attendanceCodeList=[];
          this.snackbar.open('' + res._message, '', {
            duration: 10000
          });
        } else{
          this.getAllAttendanceCodeModel.attendanceCodeList=[];
        }
      }else{
        this.getAllAttendanceCodeModel.attendanceCodeList=res.attendanceCodeList;
        this.scheduleStudentListViewModel.scheduleStudentForView.map((item,i)=>{
          this.initializeDefaultValues(item,i);
          if(this.scheduleStudentListViewModel.scheduleStudentForView.length!=i+1){
            this.addUpdateStudentAttendanceModel.studentAttendance.push(new StudentAttendanceModel());
          }
        });
        this.getStudentAttendanceList();

      }
    })
    }

    getStudentAttendanceList(){
      this.active=0;
      this.studentAttendanceList = {...this.setDefaultDataInStudentAttendance(this.studentAttendanceList)}
      this.studentAttendanceService.getAllStudentAttendanceList(this.studentAttendanceList).subscribe((res)=>{
        if (typeof (res) == 'undefined') {
          this.studentAttendanceList.studentAttendance = [];
        }
        else {
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
            this.studentAttendanceList.studentAttendance = [];
            // if (!res.studentAttendance) {
            //   this.snackbar.open(res._message, '', {
            //     duration: 5000
            //   });
            // this.updateStudentAttendanceList();
            // }
          } else {
            this.studentAttendanceList.studentAttendance = res.studentAttendance;
            this.allStudentList.map((withOutAttendence, index) => {
              res.studentAttendance.map((val) => {
                if (withOutAttendence.studentId === val.studentId) {
                  this.allStudentList[index] = val;
                  this.allStudentList[index].customIndex = index
                }
              })
            })
            this.active=this.studentAttendanceList.studentAttendance.length
            this.updateStudentAttendanceList();
          }
    
        }
      })
    }

     
    initializeDefaultValues(item,i){
      this.addUpdateStudentAttendanceModel.studentAttendance[i].studentId=item.studentId;
      this.addUpdateStudentAttendanceModel.studentAttendance[i].attendanceCategoryId=this.staffDetails.attendanceCategoryId;
      this.addUpdateStudentAttendanceModel.studentAttendance[i].attendanceDate=this.staffDetails.attendanceDate;
      this.addUpdateStudentAttendanceModel.studentAttendance[i].blockId=this.staffDetails.blockId;
      this.addUpdateStudentAttendanceModel.studentAttendance[i].updatedBy=this.defaultValuesService.getUserGuidId();
      this.getAllAttendanceCodeModel.attendanceCodeList.map((element) => {
        if (element.defaultCode) {
          this.addUpdateStudentAttendanceModel.studentAttendance[i].attendanceCode = element.attendanceCode1.toString();
        }
      });
      this.addUpdateStudentAttendanceModel.studentAttendance[i].comments='';

    }
  
    updateStudentAttendanceList(){
      for(let studentAttendance of this.studentAttendanceList.studentAttendance){
        this.addUpdateStudentAttendanceModel.studentAttendance.forEach((addUpdateStudentAttendance, index)=>{
          if(addUpdateStudentAttendance.studentId==studentAttendance.studentId){
            addUpdateStudentAttendance.attendanceCode=studentAttendance.attendanceCode.toString();
            this.commentsArray[index] = studentAttendance.studentAttendanceComments
            addUpdateStudentAttendance.studentAttendanceComments[0].comment = studentAttendance.studentAttendanceComments[0].comment;
            addUpdateStudentAttendance.studentAttendanceComments[0].membershipId = studentAttendance.studentAttendanceComments[0].membershipId;

            this.actionButtonTitle='update';
          }
        })
      }      
    }


  addComments(index){
    let studentName=this.scheduleStudentListViewModel.scheduleStudentForView[index].firstGivenName+' '+this.scheduleStudentListViewModel.scheduleStudentForView[index].lastFamilyName
    this.dialog.open(AddCommentsComponent, {
      width: '700px',
      data: {studentName,commentData: this.commentsArray[index], type: this.actionButtonTitle}
    }).afterClosed().subscribe((res)=>{
      if(res?.submit){
        if(res.response.comment.trim().length > 0) {
          if(this.commentsArray[index]?.studentAttendanceComments) {
            this.commentsArray[index].studentAttendanceComments[0].comment = res.response.comment.trim();
            this.commentsArray[index].studentAttendanceComments[0].membershipId = +this.defaultValuesService.getuserMembershipID();
          } else {
            this.commentsArray[index] = [res.response];
          }
        
          this.addUpdateStudentAttendanceModel.studentAttendance[index].studentAttendanceComments[0].comment = res.response.comment.trim();
          this.addUpdateStudentAttendanceModel.studentAttendance[index].studentAttendanceComments[0].membershipId = +this.defaultValuesService.getuserMembershipID();
        }
       }
    });
  }

  addUpdateStudentAttendance() {
    this.addUpdateStudentAttendanceModel.studentAttendance.map((data,index)=>{
      this.getTheIndexNumbersForDroppedStudentForCourseSection.map(val=>{
        if(val === index){
          this.addUpdateStudentAttendanceModel.studentAttendance[val].attendanceCode = 0;
          this.addUpdateStudentAttendanceModel.studentAttendance[val].attendanceCategoryId = 0;
        }
      })
     })
    this.addUpdateStudentAttendanceModel={...this.setDefaultDataInStudentAttendance(this.addUpdateStudentAttendanceModel)};
      this.studentAttendanceService.addUpdateStudentAttendance(this.addUpdateStudentAttendanceModel).subscribe((res)=>{
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
        } else {
          this.snackbar.open(res._message, '', {
            duration: 10000
          }); 
          this.getScheduledStudentList();         
        }
      })
    
  }

  giveClass(value) {
    if (value.stateCode == 'Present') {
      return { 'present': true, 'active': false };
    } else if (value.stateCode == 'Absent') {
      return { 'absent': true, 'active': false };
    } else if (value.stateCode == 'Half Day') {
      return { 'tardy': true, 'active': false };
    }
  }
  giveClassAfterAttendence(value ,index ){
    if (value.stateCode == 'Present') {
      return { 'present': true, 'active': value.attendanceCode1 ===  this.allStudentList[index].attendanceCode ? true : false };
    } else if (value.stateCode == 'Absent') {
      return { 'absent': true, 'active': value.attendanceCode1 ===  this.allStudentList[index].attendanceCode ? true : false };
    } else if (value.stateCode == 'Half Day') {
      return { 'tardy': true, 'active': value.attendanceCode1 ===  this.allStudentList[index].attendanceCode ? true : false };
    }
  }

  setDefaultDataInStudentAttendance(attendanceModel){
    attendanceModel.courseId=this.staffDetails.courseId;
    attendanceModel.courseSectionId=this.staffDetails.courseSectionId;
    attendanceModel.attendanceDate=this.staffDetails.attendanceDate;
    attendanceModel.periodId=this.staffDetails.periodId;
    attendanceModel.staffId=this.staffDetails.staffId;
    attendanceModel.updatedBy = this.defaultValuesService.getUserGuidId();
    return attendanceModel;
  }

  

}
