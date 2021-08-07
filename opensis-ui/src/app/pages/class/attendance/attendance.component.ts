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

import { Component, OnChanges, OnDestroy, OnInit, SimpleChanges } from "@angular/core";
import { FormControl } from "@angular/forms";
import { MAT_DATE_FORMATS } from "@angular/material/core";
import { MatDialog } from "@angular/material/dialog";
import { TranslateService } from "@ngx-translate/core";
import * as _moment from 'moment';
import { fadeInRight400ms } from "../../../../@vex/animations/fade-in-right.animation";
import { fadeInUp400ms } from "../../../../@vex/animations/fade-in-up.animation";
import { stagger60ms } from "../../../../@vex/animations/stagger.animation";
import { AttendanceDetails } from "../../../models/attendance-details.model";
import { AddTeacherCommentsComponent } from "./add-teacher-comments/add-teacher-comments.component";
import { default as _rollupMoment } from 'moment';
import { CourseSectionClass, GetAllAttendanceCodeModel } from "../../../models/attendance-code.model";
import { ScheduleStudentListViewModel } from "../../../models/student-schedule.model";
import { AddUpdateStudentAttendanceModel, GetAllStudentAttendanceListModel, StudentAttendanceModel } from "../../../models/take-attendance-list.model";
import { StudentAttendanceService } from "../../../services/student-attendance.service";
import { Router } from "@angular/router";
import { MatSnackBar } from "@angular/material/snack-bar";
import { StudentScheduleService } from "../../../services/student-schedule.service";
import { AttendanceCodeService } from "../../../services/attendance-code.service";
import { SharedFunction } from "../../shared/shared-function";
import { days } from "../../../common/static-data";
import { scheduleType } from "../../../enums/takeAttendanceList.enum";
import { AddCommentsComponent } from "../../staff/teacher-function/take-attendance/period-attendance/add-comments/add-comments.component";
import { DefaultValuesService } from "../../../common/default-values.service";
import { StaffPortalService } from "../../../services/staff-portal.service";
import { takeUntil } from "rxjs/operators";
import { Subject } from "rxjs";
import { MatDatepickerInputEvent } from "@angular/material/datepicker";
import { GetAllStaffModel } from "../../../models/staff.model";
import { ScheduledCourseSectionViewModel } from "../../../models/dashboard.model";
import { CommonService } from "src/app/services/common.service";
const moment = _rollupMoment || _moment;

const MY_FORMATS = {
  parse: {
    dateInput: 'LL',
  },
  display: {
    dateInput: 'LL',
    monthYearLabel: 'MMM YYYY',
    dateA11yLabel: 'LL',
    monthYearA11yLabel: 'MMMM YYYY',
  },
};

@Component({
  selector: "vex-attendance",
  templateUrl: "./attendance.component.html",
  styleUrls: ["./attendance.component.scss"],
  animations: [fadeInRight400ms, stagger60ms, fadeInUp400ms],
  providers: [
    { provide: MAT_DATE_FORMATS, useValue: MY_FORMATS },
  ],
})

export class AttendanceComponent implements OnInit, OnDestroy {
  portalAccess: boolean;
  commentsArray = [];
  destroySubject$: Subject<void> = new Subject();
  displayedColumns: string[] = ['students', 'attendanceCodes', 'comments'];
  showShortName: boolean = false;
  getAllAttendanceCodeModel: GetAllAttendanceCodeModel = new GetAllAttendanceCodeModel()
  scheduleStudentListViewModel: ScheduleStudentListViewModel = new ScheduleStudentListViewModel()
  addUpdateStudentAttendanceModel: AddUpdateStudentAttendanceModel = new AddUpdateStudentAttendanceModel();
  isAttendanceDateToday = true;
  studentAttendanceList: GetAllStudentAttendanceListModel = new GetAllStudentAttendanceListModel();
  actionButtonTitle: string = 'submit';
  loading: boolean;
  courseSectionClass: CourseSectionClass = new CourseSectionClass();
  currentComponent: string;
  courseSection;
  selectedCourseSection;
  maxDate: string | Date = new Date();
  attendanceDate: string;
  isAnyMissingAttendance = false;
  constructor(public translateService: TranslateService, private dialog: MatDialog,
    private studentAttendanceService: StudentAttendanceService,
    private router: Router,
    private snackbar: MatSnackBar,
    private studentScheduleService: StudentScheduleService,
    private attendanceCodeService: AttendanceCodeService,
    private defaultValuesService: DefaultValuesService,
    private staffPortalService: StaffPortalService,
    private commonFunction: SharedFunction,
    private commonService: CommonService,
    ) {
    translateService.use("en");
    this.attendanceDate = this.commonFunction.formatDateSaveWithoutTime(new Date());
  }


  ngOnInit(): void {
    this.courseSection = JSON.parse(localStorage.getItem('selectedCourseSection'));
    this.generateClassForCourseSection(this.courseSection);
    this.missingAttendanceListForCourseSection();
    this.currentComponent = 'takeAttendance';
    this.isToday();
  }

  missingAttendanceListForCourseSection() {
    let getAllStaffModel: GetAllStaffModel = new GetAllStaffModel();
    getAllStaffModel.sortingModel = null;
    getAllStaffModel.staffId = +sessionStorage.getItem('userId');
    getAllStaffModel.courseSectionId = +this.courseSectionClass.courseSectionId;
    this.staffPortalService.missingAttendanceListForCourseSection(getAllStaffModel).subscribe((res: ScheduledCourseSectionViewModel) => {
      if (res) {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          if (!res.courseSectionViewList) {
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
        }
        else {
          this.isAnyMissingAttendance = !!res.missingAttendanceCount
        }
      }
      else {
        this.snackbar.open(sessionStorage.getItem("httpError"), '', {
          duration: 10000
        });
      }
    });
  }

  dateChange(date: MatDatepickerInputEvent<Date>) {
    this.isToday();
    this.courseSectionClass.attendanceDate = this.commonFunction.formatDateSaveWithoutTime(date.value);
    this.getScheduledStudentList();
  }

  isToday() {
    let inputDate = new Date(this.commonFunction.formatDateSaveWithoutTime(this.attendanceDate));
    let todaysDate = new Date();
    if (inputDate.setHours(0, 0, 0, 0) != todaysDate.setHours(0, 0, 0, 0)) {
      this.isAttendanceDateToday = false;
    }else{
      this.isAttendanceDateToday = true;
    }
  }
  changeComponent(step, status) {
    this.currentComponent = step;
    if (status) {
      this.selectedCourseSection = this.staffPortalService.getCourseSectionDetails();
      this.attendanceDate = this.commonFunction.formatDateSaveWithoutTime(this.selectedCourseSection.attendanceDate);
      this.isToday();
      this.scheduleStudentListViewModel.scheduleStudentForView = [];
      this.getAllAttendanceCodeModel.attendanceCodeList = [];
      this.studentAttendanceList.studentAttendance = [];
      this.attendanceDate = this.commonFunction.formatDateSaveWithoutTime(this.selectedCourseSection.attendanceDate);
      if (this.selectedCourseSection && this.selectedCourseSection.attendanceTaken) {
        this.courseSectionClass.blockId = this.selectedCourseSection.blockId;
        this.courseSectionClass.periodId = this.selectedCourseSection.periodId;
        this.courseSectionClass.courseId = this.selectedCourseSection.courseId;
        this.courseSectionClass.courseSectionId = this.selectedCourseSection.courseSectionId;
        this.courseSectionClass.takeAttendance = this.selectedCourseSection.attendanceTaken ? true : false;
        this.courseSectionClass.attendanceDate = this.commonFunction.formatDateSaveWithoutTime(this.selectedCourseSection.attendanceDate);
        this.getScheduledStudentList();
      }
    }
  }


  generateClassForCourseSection(courseSection) {
    const formatedDate = new Date(this.attendanceDate);
    const dayName = days[formatedDate.getDay()];

    if (courseSection.scheduleType === scheduleType.FixedSchedule) {
      if (courseSection.attendanceTaken) {

        this.courseSectionClass.blockId = courseSection.courseFixedSchedule.blockId;
        this.courseSectionClass.periodId = courseSection.courseFixedSchedule.blockPeriod.periodId;
        this.courseSectionClass.courseId = courseSection.courseId;
        this.courseSectionClass.courseSectionId = courseSection.courseSectionId;
        this.courseSectionClass.takeAttendance = courseSection.attendanceTaken ? true : false;
        this.courseSectionClass.attendanceDate = this.attendanceDate;

        this.getScheduledStudentList();
      }
    }

    else if (courseSection.scheduleType === scheduleType.variableSchedule) {
      courseSection.courseVariableSchedule.forEach(element => {
        if (dayName === element.day && element.takeAttendance) {

          this.courseSectionClass.blockId = element.blockId;
          this.courseSectionClass.periodId = element.periodId;
          this.courseSectionClass.courseId = element.courseId;
          this.courseSectionClass.courseSectionId = element.courseSectionId;
          this.courseSectionClass.takeAttendance = element.takeAttendance ? true : false;
          this.courseSectionClass.attendanceDate = this.attendanceDate;

          this.getScheduledStudentList();
        }

      });
    }
    else if (courseSection.scheduleType === scheduleType.calendarSchedule) {
      courseSection.courseCalendarSchedule.forEach(element => {
        if (dayName === days[ new Date(element?.date).getDay()] && element.takeAttendance) {

          this.courseSectionClass.blockId = element.blockId;
          this.courseSectionClass.periodId = element.periodId;
          this.courseSectionClass.courseId = element.courseId;
          this.courseSectionClass.courseSectionId = element.courseSectionId;
          this.courseSectionClass.takeAttendance = element.takeAttendance ? true : false;
          this.courseSectionClass.attendanceDate = this.attendanceDate;

          this.getScheduledStudentList();
        }

      });
    }
  }


  getScheduledStudentList() {
    this.scheduleStudentListViewModel.courseSectionId = +localStorage.getItem('courseSectionId');
    this.scheduleStudentListViewModel.pageNumber = 0;
    this.scheduleStudentListViewModel.pageSize = 0;
    this.scheduleStudentListViewModel.sortingModel = null;
    this.studentScheduleService.searchScheduledStudentForGroupDrop(this.scheduleStudentListViewModel).subscribe((res) => {
    if(res._failure){
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
        this.getAllAttendanceCode();

      }
    })
  }

  getAllAttendanceCode() {
    this.getAllAttendanceCodeModel.attendanceCategoryId = this.courseSection.attendanceCategoryId;
    this.attendanceCodeService.getAllAttendanceCode(this.getAllAttendanceCodeModel).subscribe((res: any) => {
    if(res._failure){
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

  getStudentAttendanceList() {
    this.studentAttendanceList = { ...this.setDefaultDataInStudentAttendance(this.studentAttendanceList) }
    this.studentAttendanceService.getAllStudentAttendanceList(this.studentAttendanceList).subscribe((res) => {
      if (typeof (res) == 'undefined') {
        this.studentAttendanceList.studentAttendance = [];
      }
      else {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
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
          this.studentAttendanceList.studentAttendance = res.studentAttendance;
          this.updateStudentAttendanceList();
        }

      }
    })
  }


  initializeDefaultValues(item, i) {
    this.addUpdateStudentAttendanceModel.studentAttendance[i].studentId = item.studentId;
    this.addUpdateStudentAttendanceModel.studentAttendance[i].attendanceCategoryId = this.courseSection.attendanceCategoryId;
    this.addUpdateStudentAttendanceModel.studentAttendance[i].attendanceDate = this.commonFunction.formatDateSaveWithoutTime(this.attendanceDate);
    this.addUpdateStudentAttendanceModel.studentAttendance[i].blockId = this.courseSectionClass.blockId;
    this.addUpdateStudentAttendanceModel.studentAttendance[i].updatedBy=this.defaultValuesService.getEmailId();
    this.getAllAttendanceCodeModel.attendanceCodeList.map((element) => {
      if (element.defaultCode) {
        this.addUpdateStudentAttendanceModel.studentAttendance[i].attendanceCode = element.attendanceCode1.toString();
      }
    });
    this.addUpdateStudentAttendanceModel.studentAttendance[i].comments = '';

  }

  updateStudentAttendanceList() {
    for (let studentAttendance of this.studentAttendanceList.studentAttendance) {
      this.addUpdateStudentAttendanceModel.studentAttendance.forEach((addUpdateStudentAttendance, index) => {
        if (addUpdateStudentAttendance.studentId == studentAttendance.studentId) {
          addUpdateStudentAttendance.attendanceCode = studentAttendance.attendanceCode.toString();
          this.commentsArray[index] = studentAttendance.studentAttendanceComments
          this.actionButtonTitle = 'update';
        }
      })
    }
  }


  addComments(index) {
    let studentName = this.scheduleStudentListViewModel.scheduleStudentForView[index].firstGivenName + ' ' + this.scheduleStudentListViewModel.scheduleStudentForView[index].lastFamilyName
    this.dialog.open(AddTeacherCommentsComponent, {
      width: '500px',
      data: { studentName, commentData: this.commentsArray[index], type: this.actionButtonTitle }
    }).afterClosed().subscribe((res) => {
      if (res?.submit) {
        if (res.response.comment?.trim().length > 0) {
          if (this.commentsArray[index]?.studentAttendanceComments) {
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
    this.addUpdateStudentAttendanceModel = { ...this.setDefaultDataInStudentAttendance(this.addUpdateStudentAttendanceModel) };
    this.studentAttendanceService.addUpdateStudentAttendance(this.addUpdateStudentAttendanceModel).subscribe((res) => {
    if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
        this.snackbar.open(res._message, '', {
          duration: 10000
        });
      } else {
        this.snackbar.open(res._message, '', {
          duration: 10000
        });
      }
    })

  }

  setDefaultDataInStudentAttendance(attendanceModel) {
    attendanceModel.courseId = this.courseSectionClass.courseId;
    attendanceModel.courseSectionId = this.courseSectionClass.courseSectionId;
    attendanceModel.attendanceDate = this.commonFunction.formatDateSaveWithoutTime(this.attendanceDate);;
    attendanceModel.periodId = this.courseSectionClass.periodId;
    attendanceModel.updatedBy = this.defaultValuesService.getEmailId();
    attendanceModel.staffId = +sessionStorage.getItem('userId');
    return attendanceModel;
  }

  ngOnDestroy(): void {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }

}
