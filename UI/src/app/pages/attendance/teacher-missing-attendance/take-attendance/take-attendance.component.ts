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

import { Component, OnInit, Input } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { AddTeacherCommentsComponent } from './add-teacher-comments/add-teacher-comments.component';
import { SharedFunction } from "../../../shared/shared-function";
import { days } from "../../../../common/static-data";
import { scheduleType } from "../../../../enums/takeAttendanceList.enum";
import { CourseSectionClass, GetAllAttendanceCodeModel } from "../../../../models/attendance-code.model";
import { ScheduleStudentListViewModel } from "../../../../models/student-schedule.model";
import { StudentScheduleService } from "../../../../services/student-schedule.service";
import { DefaultValuesService } from "../../../../common/default-values.service";
import { CommonService } from "../../../../services/common.service";
import { MatSnackBar } from "@angular/material/snack-bar";
import { AddUpdateStudentAttendanceModel, GetAllStudentAttendanceListModel, StudentAttendanceModel } from "../../../../models/take-attendance-list.model";
import { AttendanceCodeService } from "../../../../services/attendance-code.service";
import { StudentAttendanceService } from "../../../../services/student-attendance.service";
import { GetAllStaffModel } from "../../../../models/staff.model";
import { StaffPortalService } from "../../../../services/staff-portal.service";
import { ScheduledCourseSectionViewModel } from "../../../../models/dashboard.model";
import { LoaderService } from "../../../../services/loader.service";
import { DatePipe } from "@angular/common";
import { Subject } from "rxjs";


@Component({
  selector: 'vex-take-attendance',
  templateUrl: './take-attendance.component.html',
  styleUrls: ['./take-attendance.component.scss']
})
export class TakeAttendanceComponent implements OnInit {

  destroySubject$: Subject<void> = new Subject();

  displayedColumns: string[] = ['students', 'attendanceCodes', 'comments'];
  attendanceDate: string;
  courseSectionClass: CourseSectionClass = new CourseSectionClass();
  scheduleStudentListViewModel: ScheduleStudentListViewModel = new ScheduleStudentListViewModel()
  getAllAttendanceCodeModel: GetAllAttendanceCodeModel = new GetAllAttendanceCodeModel()
  addUpdateStudentAttendanceModel: AddUpdateStudentAttendanceModel = new AddUpdateStudentAttendanceModel();
  courseSection;
  studentAttendanceList: GetAllStudentAttendanceListModel = new GetAllStudentAttendanceListModel();
  commentsArray = [];
  myHolidayDates = [];
  myHolidayFilter;
  actionButtonTitle: string = 'submit';
  currentComponent: string;
  isAttendanceDateToday = true;
  // isAnyMissingAttendance = false;
  showShortName: boolean = false;
  getDataFromParents: any;
  loading: boolean;
  courseSectionNameValue: string;
  studentLength: number = 0;
  courseGradeLevel;
  getTheIndexNumbersForDroppedStudentForCourseSection=[];
  active;
  allStudentList=[];


  constructor(private dialog: MatDialog,
    public translateService: TranslateService,
    private commonFunction: SharedFunction,
    private studentScheduleService: StudentScheduleService,
    public defaultValuesService: DefaultValuesService,
    private commonService: CommonService,
    private snackbar: MatSnackBar,
    private attendanceCodeService: AttendanceCodeService,
    private studentAttendanceService: StudentAttendanceService,
    private staffPortalService: StaffPortalService,
    private loaderService: LoaderService,
    private datepipe: DatePipe,



  ) {
    // translateService.use('en');
    this.attendanceDate = this.commonFunction.formatDateSaveWithoutTime(new Date());
    this.courseSection = this.defaultValuesService.getSelectedCourseSection();
    if (this.courseSection.courseGradeLevel != null) {
      this.courseGradeLevel = this.courseSection.courseGradeLevel;
    } else {
      this.courseGradeLevel = 'Grade Level';
    }
    this.loaderService.isLoading.subscribe((val) => {
      this.loading = val;
    });

  }

  ngOnInit(): void {
    this.myHolidayDates = this.courseSection.holidayList.map(x => {
      return new Date(x);
    });
    const disabledDates = this.courseSection.attendanceDays;
    this.myHolidayFilter = (d: Date): boolean => {
      const time = d.getTime();
      const day = (d || new Date()).getDay();
      return (!this.myHolidayDates.find(x => x.getTime() == time) && disabledDates.includes(day));
    }
    this.generateClassForCourseSection(this.courseSection);
    // this.missingAttendanceListForCourseSection();
    this.currentComponent = 'takeAttendance';
    this.isToday();

  }

  isToday() {
    let inputDate = new Date(this.commonFunction.formatDateSaveWithoutTime(this.attendanceDate));
    let todaysDate = new Date();
    if (inputDate.setHours(0, 0, 0, 0) != todaysDate.setHours(0, 0, 0, 0)) {
      this.isAttendanceDateToday = false;
    } else {
      this.isAttendanceDateToday = true;
    }
  }

  // missingAttendanceListForCourseSection() {
  //   let getAllStaffModel: GetAllStaffModel = new GetAllStaffModel();
  //   getAllStaffModel.sortingModel = null;
  //   getAllStaffModel.staffId = this.defaultValuesService.getUserId();
  //   getAllStaffModel.courseSectionId = this.courseSection.courseSectionId;
  //   this.staffPortalService.missingAttendanceListForCourseSection(getAllStaffModel).subscribe((res: ScheduledCourseSectionViewModel) => {
  //     if (res) {
  //       if (res._failure) {
  //         this.commonService.checkTokenValidOrNot(res._message);
  //         if (!res.courseSectionViewList) {
  //           this.snackbar.open(res._message, '', {
  //             duration: 10000
  //           });
  //         }
  //       }
  //       else {
  //         this.isAnyMissingAttendance = !!res.missingAttendanceCount
  //       }
  //     }
  //     else {
  //       this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
  //         duration: 10000
  //       });
  //     }
  //   });
  // }

  generateClassForCourseSection(courseSection) {
    const formatedDate = new Date(this.attendanceDate);
    const dayName = days[formatedDate.getDay()];

    // if (courseSection.scheduleType === scheduleType.FixedSchedule) {
    //   if (courseSection.attendanceTaken) {

    //     this.courseSectionClass.blockId = courseSection.blockId;
    //     this.courseSectionClass.periodId = courseSection.periodId;
    //     this.courseSectionClass.courseId = courseSection.courseId;
    //     this.courseSectionClass.courseSectionId = courseSection.courseSectionId;
    //     this.courseSectionClass.takeAttendance = courseSection.attendanceTaken ? true : false;
    //     this.courseSectionClass.attendanceDate = this.datepipe.transform(this.courseSection.attendanceDate, "yyyy-MM-dd");
    //     this.getScheduledStudentList();
    //   }
    // }

    // else if (courseSection.scheduleType === scheduleType.variableSchedule) {
    //   courseSection.courseVariableSchedule.forEach(element => {
    //     if (dayName === element.day && element.takeAttendance) {

    //       this.courseSectionClass.blockId = element.blockId;
    //       this.courseSectionClass.periodId = element.periodId;
    //       this.courseSectionClass.courseId = element.courseId;
    //       this.courseSectionClass.courseSectionId = element.courseSectionId;
    //       this.courseSectionClass.takeAttendance = element.takeAttendance ? true : false;
    //       this.courseSectionClass.attendanceDate = this.datepipe.transform(this.courseSection.attendanceDate, "yyyy-MM-dd");

    //       this.getScheduledStudentList();
    //     }

    //   });
    // }
    // else if (courseSection.scheduleType === scheduleType.calendarSchedule) {
    //   courseSection.courseCalendarSchedule.forEach(element => {
    //     if (dayName === days[new Date(element?.date).getDay()] && element.takeAttendance) {

    //       this.courseSectionClass.blockId = element.blockId;
    //       this.courseSectionClass.periodId = element.periodId;
    //       this.courseSectionClass.courseId = element.courseId;
    //       this.courseSectionClass.courseSectionId = element.courseSectionId;
    //       this.courseSectionClass.takeAttendance = element.takeAttendance ? true : false;
    //       this.courseSectionClass.attendanceDate = this.datepipe.transform(this.courseSection.attendanceDate, "yyyy-MM-dd");

    //       this.getScheduledStudentList();
    //     }

    //   });
    // }
    if (courseSection.attendanceTaken) {
          this.courseSectionClass.blockId = courseSection.blockId;
          this.courseSectionClass.periodId = courseSection.periodId;
          this.courseSectionClass.courseId = courseSection.courseId;
          this.courseSectionClass.courseSectionId = courseSection.courseSectionId;
          this.courseSectionClass.takeAttendance = courseSection.attendanceTaken ? true : false;
          this.courseSectionClass.attendanceDate = this.datepipe.transform(this.courseSection.attendanceDate, "yyyy-MM-dd");
          this.getScheduledStudentList();
        }
  }

  getScheduledStudentList() {
    this.scheduleStudentListViewModel.courseSectionIds = [this.courseSection.courseSectionId];
    this.scheduleStudentListViewModel.pageNumber = 0;
    this.scheduleStudentListViewModel.pageSize = 0;
    this.scheduleStudentListViewModel.sortingModel = null;
    this.scheduleStudentListViewModel.profilePhoto = true;
    this.scheduleStudentListViewModel.attendanceDate = this.commonFunction.formatDateSaveWithoutTime(this.courseSection.attendanceDate)
    // this.scheduleStudentListViewModel.includeInactive = true;
    this.studentScheduleService.searchScheduledStudentForGroupDrop(this.scheduleStudentListViewModel).subscribe((res) => {
      if (res._failure) {
        this.commonService.checkTokenValidOrNot(res._message);
        if (res.scheduleStudentForView == null) {
          this.snackbar.open(res._message, '', {
            duration: 10000
          });
          this.scheduleStudentListViewModel.scheduleStudentForView = [];
        } else {
          this.scheduleStudentListViewModel.scheduleStudentForView = res.scheduleStudentForView;
        }
      } else {
        this.scheduleStudentListViewModel.scheduleStudentForView = res.scheduleStudentForView;
        this.studentLength = this.scheduleStudentListViewModel.scheduleStudentForView.length;
        this.allStudentList=res.scheduleStudentForView;
        this.getAllAttendanceCode();
      }
    })
  }

  getAllAttendanceCode() {
    this.getAllAttendanceCodeModel.attendanceCategoryId = this.courseSection.attendanceCategoryId;
    this.attendanceCodeService.getAllAttendanceCode(this.getAllAttendanceCodeModel).subscribe((res: any) => {
      if (res._failure) {
        this.commonService.checkTokenValidOrNot(res._message);
        if (res.attendanceCodeList === null) {
          this.getAllAttendanceCodeModel.attendanceCodeList = [];
          this.snackbar.open('' + res._message, '', {
            duration: 10000
          });
        } else {
          this.getAllAttendanceCodeModel.attendanceCodeList = [];
        }
      } else {
        this.getAllAttendanceCodeModel.attendanceCodeList = res.attendanceCodeList;
        this.scheduleStudentListViewModel.scheduleStudentForView.map((item, i) => {
          this.initializeDefaultValues(item, i);
          if (this.scheduleStudentListViewModel.scheduleStudentForView.length != i + 1) {
            this.addUpdateStudentAttendanceModel.studentAttendance.push(new StudentAttendanceModel());
          }
        });
        this.getStudentAttendanceList();

      }
    })
  }

  initializeDefaultValues(item, i) {
    this.addUpdateStudentAttendanceModel.studentAttendance[i].studentId = item.studentId;
    this.addUpdateStudentAttendanceModel.studentAttendance[i].attendanceCategoryId = this.courseSection.attendanceCategoryId;
    this.addUpdateStudentAttendanceModel.studentAttendance[i].attendanceDate = this.datepipe.transform(this.courseSection.attendanceDate, "yyyy-MM-dd");
    this.addUpdateStudentAttendanceModel.studentAttendance[i].blockId = this.courseSectionClass.blockId;
    this.addUpdateStudentAttendanceModel.studentAttendance[i].updatedBy = this.defaultValuesService.getUserGuidId();
    this.getAllAttendanceCodeModel.attendanceCodeList.map((element) => {
      if (element.defaultCode) {
        this.addUpdateStudentAttendanceModel.studentAttendance[i].attendanceCode = element.attendanceCode1.toString();
      }
    });
    this.addUpdateStudentAttendanceModel.studentAttendance[i].comments = '';

  }

  getStudentAttendanceList() {
    this.active=0;
    this.studentAttendanceList = { ...this.setDefaultDataInStudentAttendance(this.studentAttendanceList) }
    this.studentAttendanceService.getAllStudentAttendanceList(this.studentAttendanceList).subscribe((res) => {
      if (typeof (res) == 'undefined') {
        this.studentAttendanceList.studentAttendance = [];
      }
      else {
        if (res._failure) {
          this.commonService.checkTokenValidOrNot(res._message);
          this.commentsArray = [];
          this.actionButtonTitle = 'submit';
          if (res.studentAttendance == null) {
            // this.snackbar.open(res._message, '', {
            //   duration: 5000
            // });
            this.studentAttendanceList.studentAttendance = [];
            this.updateStudentAttendanceList();

          } else {
            this.studentAttendanceList.studentAttendance = res.studentAttendance;
            this.updateStudentAttendanceList();
          }
        } else {
          this.commentsArray = [];
          this.actionButtonTitle = 'submit';
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

  setDefaultDataInStudentAttendance(attendanceModel) {
    attendanceModel.courseId = this.courseSectionClass.courseId;
    attendanceModel.courseSectionId = this.courseSectionClass.courseSectionId;
    attendanceModel.attendanceDate = this.datepipe.transform(this.courseSection.attendanceDate, "yyyy-MM-dd");
    attendanceModel.periodId = this.courseSectionClass.periodId;
    attendanceModel.updatedBy = this.defaultValuesService.getUserGuidId();
    attendanceModel.staffId = parseInt(this.defaultValuesService.getUserId());
    return attendanceModel;
  }

  updateStudentAttendanceList() {
    for (let studentAttendance of this.studentAttendanceList.studentAttendance) {
      this.addUpdateStudentAttendanceModel.studentAttendance.forEach((addUpdateStudentAttendance, index) => {
        if (addUpdateStudentAttendance.studentId == studentAttendance.studentId) {
          addUpdateStudentAttendance.attendanceCode = studentAttendance.attendanceCode.toString();
          addUpdateStudentAttendance.studentAttendanceComments[0].comment = studentAttendance.studentAttendanceComments[0]?.comment;
          addUpdateStudentAttendance.studentAttendanceComments[0].membershipId = studentAttendance.studentAttendanceComments[0]?.membershipId;
          this.commentsArray[index] = studentAttendance.studentAttendanceComments
          this.actionButtonTitle = 'update';
        }
      })
    }
  }

  addUpdateStudentAttendance() {
    this.loading = true;
    this.addUpdateStudentAttendanceModel.studentAttendance.map((data,index)=>{
      this.getTheIndexNumbersForDroppedStudentForCourseSection.map(val=>{
        if(val === index){
          this.addUpdateStudentAttendanceModel.studentAttendance[val].attendanceCode = 0;
          this.addUpdateStudentAttendanceModel.studentAttendance[val].attendanceCategoryId = 0;
        }
      })
    })
    this.addUpdateStudentAttendanceModel = { ...this.setDefaultDataInStudentAttendance(this.addUpdateStudentAttendanceModel) };
    this.studentAttendanceService.addUpdateStudentAttendance(this.addUpdateStudentAttendanceModel).subscribe((res) => {
      if (res._failure) {
        this.commonService.checkTokenValidOrNot(res._message);
        this.snackbar.open(res._message, '', {
          duration: 10000
        });
      } else {
        this.snackbar.open(res._message, '', {
          duration: 10000
        });
        // this.missingAttendanceListForCourseSection();
        this.scheduleStudentListViewModel.scheduleStudentForView = [];
        this.getAllAttendanceCodeModel.attendanceCodeList = [];
        this.studentAttendanceList.studentAttendance = [];
        this.addUpdateStudentAttendanceModel = new AddUpdateStudentAttendanceModel();
        this.getScheduledStudentList();
      }
    })

  }

  addComments(index) {
    let studentName = this.scheduleStudentListViewModel.scheduleStudentForView[index].firstGivenName + ' ' + this.scheduleStudentListViewModel.scheduleStudentForView[index].lastFamilyName
    this.dialog.open(AddTeacherCommentsComponent, {
      width: '500px',
      data: { studentName, commentData: this.commentsArray[index], type: this.actionButtonTitle }
    }).afterClosed().subscribe((res) => {
      if (res?.submit) {
          if (this.commentsArray[index]?.studentAttendanceComments) {
            this.commentsArray[index].studentAttendanceComments[0].comment = res.response.comment.trim();
            this.commentsArray[index].studentAttendanceComments[0].membershipId = +this.defaultValuesService.getuserMembershipID();
          } else {
            this.commentsArray[index] = [res.response];
          }

          this.addUpdateStudentAttendanceModel.studentAttendance[index].studentAttendanceComments[0].comment = res.response.comment?.trim();
          this.addUpdateStudentAttendanceModel.studentAttendance[index].studentAttendanceComments[0].membershipId = +this.defaultValuesService.getuserMembershipID();
      }
    });
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

  // addComments() {
  //   this.dialog.open(AddTeacherCommentsComponent, {
  //     width: '700px'
  //   });
  // }

  ngOnDestroy(): void {
    this.destroySubject$.next();
    this.destroySubject$.complete(); 
  }
}
