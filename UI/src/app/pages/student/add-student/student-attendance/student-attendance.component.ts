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

import { Component, OnDestroy, OnInit } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { MatSnackBar } from "@angular/material/snack-bar";
import { TranslateService } from "@ngx-translate/core";
import { CalendarEvent, CalendarView } from "angular-calendar";
import { Subject } from "rxjs";
import { takeUntil } from "rxjs/operators";
import {
  AddUpdateStudentAttendanceModelFor360,
  StudentAttendanceModelFor360,
} from "../../../../models/take-attendance-list.model";
import { ConfirmDialogComponent } from "src/app/pages/shared-module/confirm-dialog/confirm-dialog.component";
import { StudentAttendanceService } from "src/app/services/student-attendance.service";
import { DefaultValuesService } from "../../../../common/default-values.service";
import { days } from "../../../../common/static-data";
import {
  AttendanceWeekViewModel,
  ScheduledCourseSectionListForStudent360Model,
  WeekEventsModel,
} from "../../../../models/student-schedule.model";
import { StudentScheduleService } from "../../../../services/student-schedule.service";
import { StudentService } from "../../../../services/student.service";
import { AttendanceCodeEnum } from "../../../../enums/attendance-code.enum";
import { PageRolesPermission } from "../../../../common/page-roles-permissions.service";
import { Permissions } from "../../../../models/roll-based-access.model";
import { CommonService } from "src/app/services/common.service";


@Component({
  selector: "vex-student-attendance",
  templateUrl: "./student-attendance.component.html",
  styleUrls: ["./student-attendance.component.scss"],
})
export class StudentAttendanceComponent implements OnInit, OnDestroy {
  viewDate: Date = new Date();
  view: CalendarView | string = "weekView";
  weekEvents: WeekEventsModel[] = [];
  CalendarView = CalendarView;
  events: CalendarEvent[] = [];

  scheduledCourseSectionList: ScheduledCourseSectionListForStudent360Model =
    new ScheduledCourseSectionListForStudent360Model();
  todayDate = new Date().toISOString().split("T")[0];
  currentWeek = [];
  days = days;
  initialRoutineDate: Date;
  endRoutingDate: Date;
  weeklyAttendanceView: AttendanceWeekViewModel = new AttendanceWeekViewModel();
  refresh: Subject<any> = new Subject();
  addUpdateStudentAttendanceModel360: AddUpdateStudentAttendanceModelFor360 =
    new AddUpdateStudentAttendanceModelFor360();
  destroySubject$: Subject<void> = new Subject();
  monthEvents = [];
  fullDayMinutes: number=0;
  halfDayMinutes: number=0;
  attendanceCode = AttendanceCodeEnum;
  permissions: Permissions;
  membershipType;
  constructor(
    public translateService: TranslateService,
    private studentScheduledService: StudentScheduleService,
    private studentService: StudentService,
    private dialog: MatDialog,
    private studentAttendanceService: StudentAttendanceService,
    private defaultService: DefaultValuesService,
    private snackbar: MatSnackBar,
    private pageRolePermissions: PageRolesPermission,
    private commonService: CommonService,
  ) {
    // translateService.use("en");
    this.defaultService.checkAcademicYear() && !this.studentService.getStudentId() ? this.studentService.redirectToGeneralInfo() : !this.defaultService.checkAcademicYear() && !this.studentService.getStudentId() ? this.studentService.redirectToStudentList() : '';
    this.renderAttendanceWeekCalendar();
    this.membershipType = this.defaultService.getUserMembershipType();
  }

  ngOnInit(): void {
    this.permissions = this.pageRolePermissions.checkPageRolePermission();
    this.getAllScheduledCourseSectionList();
  }

  changeCalendarView(calendarType) {
    this.view = calendarType;
    if (this.view === CalendarView.Month) {
      const currentDate = new Date(this.viewDate);
      const startDate = new Date(
        currentDate.getFullYear(),
        currentDate.getMonth() - 1,
        22
      );
      const endDate = new Date(
        currentDate.getFullYear(),
        currentDate.getMonth() + 1,
        8
      );
      if (!this.monthEvents.length) {
        this.getAllScheduledCourseSectionList(startDate, endDate);
      }
    } else {
    }
  }

  getAllScheduledCourseSectionList(startDate?, endDate?) {
    if (this.view == "weekView") {
      this.scheduledCourseSectionList.durationStartDate = this.currentWeek[0]
        ?.toISOString()
        .split("T")[0];
      this.scheduledCourseSectionList.durationEndDate = this.currentWeek[6]
        ?.toISOString()
        .split("T")[0];
    } else {
// this.scheduledCourseSectionList.durationStartDate ="2021-06-06" 
//       this.scheduledCourseSectionList.durationEndDate = "2021-06-08"
      this.scheduledCourseSectionList.durationStartDate = startDate
        ?.toISOString()
        .split("T")[0];
      this.scheduledCourseSectionList.durationEndDate = endDate
        ?.toISOString()
        .split("T")[0];
    }

    this.scheduledCourseSectionList.studentId =
      this.studentService.getStudentId();
    this.scheduledCourseSectionList.scheduleCourseSectionForView = [];
    this.studentScheduledService
      .scheduleCourseSectionListForStudent360(this.scheduledCourseSectionList)
      .pipe(takeUntil(this.destroySubject$))
      .subscribe((res) => {
        if (res) {
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
            if (res.scheduleCourseSectionForView?.length === 0) {
            } else {
              this.snackbar.open(res._message, "", {
                duration: 10000,
              });
            }
          } else {
            this.scheduledCourseSectionList = res;
            this.createDataSet();
            if(res?.schoolPreference){
              this.fullDayMinutes = res.schoolPreference.fullDayMinutes?res.schoolPreference.fullDayMinutes:0;
              this.halfDayMinutes = res.schoolPreference.halfDayMinutes?res.schoolPreference.halfDayMinutes:0;
            }
          }
        } else {
          this.snackbar.open(this.defaultService.getHttpError(), "", {
            duration: 10000,
          });
        }
      });
  }

  changeMonthView(direction) {
    const currentDate = new Date(this.viewDate);
    let startDate;
    let endDate;
    if (direction === "prev") {
      startDate = new Date(
        currentDate.getFullYear(),
        currentDate.getMonth() - 1,
        22
      );
      endDate = new Date(
        currentDate.getFullYear(),
        currentDate.getMonth() + 1,
        8
      );
    } else if (direction === "next") {
      startDate = new Date(
        currentDate.getFullYear(),
        currentDate.getMonth(),
        22
      );
      endDate = new Date(
        currentDate.getFullYear(),
        currentDate.getMonth() + 2,
        8
      );
    }
    this.monthEvents=[];
    this.events=[];
    this.getAllScheduledCourseSectionList(startDate, endDate);
  }

  createDataSet() {
    if(this.view=='weekView'){
      this.weekEvents = [];
    }else{
      this.monthEvents=[]
    }
    this.scheduledCourseSectionList.scheduleCourseSectionForView.map(
      (item: any) => {
        if (item.courseFixedSchedule) {
          this.createDatasetForFixedSchedule(item);
        } else if (item.courseVariableScheduleList?.length > 0) {
          this.createDatasetForVariableSchedule(item);
        } else if (item.courseCalendarScheduleList?.length > 0) {
          this.createDatasetForCalendarSchedule(item);
        } else if (item.courseBlockScheduleList?.length > 0) {
          this.createDatasetForBlockSchedule(item);
        }
      }
    );
    if (this.view === "weekView") {
      this.filterWeekEventsBasedOnPeriod();
    }else{
      this.calculateAverageAttendance();
    }
  }

  // For Block Schedule
  createDatasetForBlockSchedule(item) {
    if (this.view === "weekView") {
      item.bellScheduleList.map((x) => {
        this.weekEvents.push({
          courseId: item.courseId,
          courseSectionId: item.courseSectionId,
          takeAttendance: item.attendanceTaken,
          blockId: x.blockId,
          periodId: item.courseBlockScheduleList[0].periodId,
          periodTitle: item.courseBlockScheduleList[0].block.blockTitle,
          day: this.getWeekDay(x.bellScheduleDate),
          takenAttendanceList: item.studentAttendanceList,
          attendanceList: item.attendanceCodeCategories,
        });
      })
    } else {
      if (item.studentAttendanceList?.length > 0) {
        let periodRunningInMinutes = this.calculateDiffBetweenTimes(
          item.courseBlockScheduleList[0].blockPeriod.periodStartTime,
          item.courseBlockScheduleList[0].blockPeriod.periodEndTime
        );
        item.studentAttendanceList = this.findAttendanceList(item, periodRunningInMinutes);
        item.studentAttendanceList.map(students=>{
          students.fullDayMinutes=item.fullDayMinutes;
          students.halfDayMinutes=item.halfDayMinutes;
        })
        this.monthEvents.push(...item.studentAttendanceList);
      }
    }
  }

  getWeekDay(date){
    let tempDate=new Date(date)
    return tempDate.getDay();
  }

  createDatasetForFixedSchedule(item) {
    if (!item.attendanceTaken) {
      return;
    }
    if (this.view === "weekView") {
      item.dayOfWeek?.split("|").map((day) => {
        this.weekEvents.push({
          courseId: item.courseId,
          courseSectionId: item.courseSectionId,
          takeAttendance: item.attendanceTaken,
          blockId: item.courseFixedSchedule.blockPeriod.blockId,
          periodId: item.courseFixedSchedule.blockPeriod.periodId,
          periodTitle: item.courseFixedSchedule.blockPeriod.periodTitle,
          day: this.getDayInNumberFromDayName(day),
          dayDate: null,
          takenAttendanceList: item.studentAttendanceList,
          attendanceList: item.attendanceCodeCategories,
        });
      });
    } else {
      if (item.studentAttendanceList?.length > 0) {
        let periodRunningInMinutes = this.calculateDiffBetweenTimes(
          item.courseFixedSchedule.blockPeriod.periodStartTime,
          item.courseFixedSchedule.blockPeriod.periodEndTime
        );
        item.studentAttendanceList = this.findAttendanceList(item,periodRunningInMinutes);
        item.studentAttendanceList.map(students=>{
          students.fullDayMinutes=item.fullDayMinutes;
          students.halfDayMinutes=item.halfDayMinutes;
        })
        this.monthEvents.push(...item.studentAttendanceList);
      }
    }
  }

  findAttendanceList(item,periodRunningInMinutes){
    item.studentAttendanceList = item.studentAttendanceList.map(
      (attendanceList) => {
        attendanceList.periodRunningInMinutes = periodRunningInMinutes;
        for (const attendance of item?.attendanceCodeCategories
          ?.attendanceCode) {
          if (
            attendance.attendanceCategoryId ===
              attendanceList.attendanceCategoryId &&
            attendance.attendanceCode1 === attendanceList.attendanceCode
          ) {
            attendanceList.attendanceStatus = attendance.stateCode;
            break;
          }
        }
        return attendanceList;
      }
    );
    return item.studentAttendanceList;
  }

  
  createDatasetForVariableSchedule(item) {
    if (this.view == "weekView") {
      item.courseVariableScheduleList?.map((day) => {
        if (day.takeAttendance) {
          this.weekEvents.push({
            courseId: item.courseId,
            courseSectionId: item.courseSectionId,
            takeAttendance: day.takeAttendance,
            blockId: day.blockPeriod.blockId,
            periodId: day.blockPeriod.periodId,
            periodTitle: day.blockPeriod.periodTitle,
            day: this.getDayInNumberFromDayName(day.day),
            dayDate: null,
            takenAttendanceList: item.studentAttendanceList,
            attendanceList: item.attendanceCodeCategories,
          });
        }
      });
    } else {
      if(item.studentAttendanceList?.length>0){
        let periodRunningInMinutes = this.calculateDiffBetweenTimes(
          item.courseVariableScheduleList[0].blockPeriod.periodStartTime,
          item.courseVariableScheduleList[0].blockPeriod.periodEndTime
        );
        item.studentAttendanceList = this.findAttendanceList(item,periodRunningInMinutes)
        item.studentAttendanceList.map(students=>{
          students.fullDayMinutes=item.fullDayMinutes;
          students.halfDayMinutes=item.halfDayMinutes;
        })
        this.monthEvents.push(...item.studentAttendanceList);
      }
    }
  }

  createDatasetForCalendarSchedule(item) {
    if(this.view=='weekView'){
    item.courseCalendarScheduleList?.map((day) => {
      if (day.takeAttendance) {
        const dayFromDate = new Date(day.date).getDay();
        this.weekEvents.push({
          courseId: item.courseId,
          courseSectionId: item.courseSectionId,
          takeAttendance: day.takeAttendance,
          blockId: day.blockPeriod.blockId,
          periodId: day.blockPeriod.periodId,
          periodTitle: day.blockPeriod.periodTitle,
          takenAttendanceList: item.studentAttendanceList,
          day: dayFromDate,
          dayDate: new Date(day.date).getDate(),
          attendanceList: item.attendanceCodeCategories,
        });
      }
    });
  }else{
    if(item.studentAttendanceList?.length>0){
      let periodRunningInMinutes = this.calculateDiffBetweenTimes(
        item.courseCalendarScheduleList[0].blockPeriod.periodStartTime,
        item.courseCalendarScheduleList[0].blockPeriod.periodEndTime
      );
      item.studentAttendanceList = this.findAttendanceList(item,periodRunningInMinutes);
      item.studentAttendanceList.map(students=>{
        students.fullDayMinutes=item.fullDayMinutes;
        students.halfDayMinutes=item.halfDayMinutes;
      })
      this.monthEvents.push(...item.studentAttendanceList);
    }
  }

  }

  filterWeekEventsBasedOnPeriod() {
    this.weeklyAttendanceView.attendanceWeekView = [];
    this.weekEvents?.map((item) => {
      let isSameBlockPeriodFound = false;
      for (const eachDay of this.weeklyAttendanceView.attendanceWeekView) {
        if (eachDay.blockId === item.blockId && eachDay.periodId === item.periodId) {
          eachDay.days = eachDay.days ? eachDay.days + "|" + item.day : item.day.toString();
          eachDay.dayDate = eachDay.dayDate ? eachDay.dayDate + "|" + item.dayDate : item?.dayDate?.toString();
          const cloneTakenAttendanceDays = this.findTakenDays(item.takenAttendanceList, item);
          const takenAttendanceDays = this.generateDayFromDate(item.takenAttendanceList, item);
          if (cloneTakenAttendanceDays) {
            cloneTakenAttendanceDays.map((attendanceObj, index) => {
              if (attendanceObj?.attendanceCode) {
                eachDay.cloneTakenAttendanceDays[index] = attendanceObj;
              }
            });
          }
          eachDay.takenAttendanceDays = eachDay.takenAttendanceDays && takenAttendanceDays ? eachDay.takenAttendanceDays + "|" + takenAttendanceDays : eachDay.takenAttendanceDays ? eachDay.takenAttendanceDays : takenAttendanceDays ? takenAttendanceDays : null;
          eachDay.courseIds = eachDay.courseIds.toString() + "|" + item.courseId.toString();
          eachDay.courseSectionIds = eachDay.courseSectionIds.toString() + "|" + item.courseSectionId.toString();
          eachDay.takenAttendanceList.push(...item.takenAttendanceList);
          isSameBlockPeriodFound = true;
          break;
        }
      }
      if (!isSameBlockPeriodFound) {
        this.weeklyAttendanceView.attendanceWeekView.push({
          blockId: item.blockId,
          periodId: item.periodId,
          attendanceTaken: false,
          periodTitle: item.periodTitle,
          courseIds: item.courseId,
          courseSectionIds: item.courseSectionId,
          days: item.day.toString(),
          dayDate: item.dayDate?.toString(),
          cloneTakenAttendanceDays: this.findTakenDays(
            item.takenAttendanceList, item
          ),
          takenAttendanceDays: this.generateDayFromDate(
            item.takenAttendanceList, item
          ),
          takenAttendanceList: item.takenAttendanceList,
          attendanceList: item.attendanceList,
        });
      }
    });
  }

  calculateAverageAttendance(){
    // date filter, allPeriodMins & halfPeriodMins wrt periods, totalMinsAttended, color
    let listOfEventDatesWithAttendanceDateMinutesAndStatus = [];
    let attendanceDateList = [];

    //array of dates
    this.monthEvents?.forEach(x=>{
      attendanceDateList.push(x.attendanceDate);
    })

    //filtering unique attendance dates
    attendanceDateList = attendanceDateList.filter((x, i, a) => a.indexOf(x) === i);

    //filtering month events as per Date and courseSectionId
    // this.monthEvents = this.monthEvents.filter((value, index, self) =>
    //     index === self.findIndex((t) => (
    //     t.attendanceDate === value.attendanceDate && t.courseSectionId === value.courseSectionId
    //   ))
    // )

    //counting allPeriodMins, halfPeriodMins, totalPeriodAttendedMins and attendanceStatus for each month date
    attendanceDateList.map(date=>{
      let allPeriodMins = 0;
      let totalPeriodAttendedMins = 0;
      let attendanceStatus = '';
      let fullDayMinutes=0;
      let halfDayMinutes=0;
      this.monthEvents.map((item:any)=>{
        if (date == item.attendanceDate) {
          allPeriodMins += item.periodRunningInMinutes;
          fullDayMinutes = item.fullDayMinutes;
          halfDayMinutes = item.halfDayMinutes;
          if(item.attendanceStatus == 'Present') {
            totalPeriodAttendedMins += item.periodRunningInMinutes;
          } else if(item.attendanceStatus == 'Half Day') {
            totalPeriodAttendedMins += item.periodRunningInMinutes/2;
          }
        }
      })
      if (totalPeriodAttendedMins >= fullDayMinutes) {
        attendanceStatus = 'Present';
      } else if(totalPeriodAttendedMins < fullDayMinutes && totalPeriodAttendedMins >= halfDayMinutes){
        attendanceStatus = 'Half Day';
      } else {
        attendanceStatus = 'Absent';
      }
      listOfEventDatesWithAttendanceDateMinutesAndStatus.push({attendanceDate:date, attendanceStatus:attendanceStatus});
    })

    //Assigning attendanceStatus to dates where attendance is taken
    listOfEventDatesWithAttendanceDateMinutesAndStatus?.map((date)=>{
        let index = this.events.findIndex((event)=>event.start.getTime()===new Date(date.attendanceDate).getTime());
        if(index!==-1){
          this.events[index]={
            title: 's',
            start: new Date(date.attendanceDate),
            meta: {
              attendanceStatus:date.attendanceStatus,
              // presentCountInMinutes: this.countIfPresent(date.attendanceStatus,date.periodRunningInMinutes,this.events[index].meta.presentCountInMinutes),
              // totalPeriodInMinutes:this.events[index].meta.totalPeriodInMinutes+date.periodRunningInMinutes
            }
          }
        }else{
          this.events.push({
            title: 's',
            start: new Date(date.attendanceDate),
            meta: {
              attendanceStatus:date.attendanceStatus,
              // presentCountInMinutes: this.countIfPresent(date.attendanceStatus,date.periodRunningInMinutes),
              // totalPeriodInMinutes:date.periodRunningInMinutes
            }
          })
        }
    });
    this.comparePresentCountWithSchoolPreference();
  }

  comparePresentCountWithSchoolPreference(){
    this.events=this.events.map((item)=>{
      // let presentCount = item.meta.presentCountInMinutes; // in minutes
      let calculatedColour: string;
      // if(this.fullDayMinutes===0 && this.halfDayMinutes===0){
      //   if(presentCount>0){
      //     calculatedColour="bg-green"
      //   }else{
      //     calculatedColour="bg-red"
      //   }
      // }else if(this.fullDayMinutes>0 && this.halfDayMinutes===0){
      //   if(presentCount>=this.fullDayMinutes){
      //     calculatedColour="bg-green"
      //   }else{
      //     calculatedColour="bg-red"
      //   }
      // }else if(this.fullDayMinutes===0 && this.halfDayMinutes>0){
      //   if(presentCount>=this.halfDayMinutes){
      //     calculatedColour="bg-green"
      //   }else{
      //     calculatedColour="bg-red"
      //   }
      // }else if(this.fullDayMinutes && this.halfDayMinutes){
      //   if(presentCount>=this.fullDayMinutes){
      //     calculatedColour = 'bg-green'
      //   }else if(presentCount>=this.halfDayMinutes && presentCount<this.fullDayMinutes){
      //     calculatedColour = 'bg-amber'
      //   }else{
      //     calculatedColour = 'bg-red'
      //   }
      // }

      if(item.meta.attendanceStatus === 'Present'){
        calculatedColour = 'bg-green'
      }else if(item.meta.attendanceStatus === 'Half Day'){
        calculatedColour = 'bg-amber'
      }else if(item.meta.attendanceStatus === 'Absent'){
        calculatedColour = 'bg-red'
      }else{
        calculatedColour = 'bg-gray'
      }

      item.meta.bgColor = calculatedColour;
      return item;
    });
    this.refresh.next();
  }

  countIfPresent(attendanceStatus, periodInMinutes, existingPresentCount=null){
    if(attendanceStatus.toLowerCase()===this.attendanceCode.Present.toLowerCase()){
      if(existingPresentCount!=null){
        return (existingPresentCount+periodInMinutes)
      }else{
        return periodInMinutes;
      }
    }
    if(existingPresentCount){
      return existingPresentCount
    }else{
      return 0;
    }
  }

  findTakenDays(takenDaysList, item) {
    const filterTakenDaysList = takenDaysList.filter(x => (x.blockId === item.blockId) && (x.periodId === item.periodId));
    const takenDays = [null, null, null, null, null, null, null];
    if (filterTakenDaysList?.length)
      filterTakenDaysList?.map((day) => {
        takenDays[new Date(day.attendanceDate).getDay()] = { attendanceCode: day.attendanceCode, stateCode: day.attendanceCodeNavigation.stateCode };
      });
    return takenDays;
  }

  getDayInNumberFromDayName(dayName: string) {
    let index = null;
    days.map((day, i) => {
      if (day.toLowerCase() === dayName.toLowerCase()) {
        index = i;
      }
    });
    return index;
  }

  generateDayFromDate(takenAttendanceList, item) {
    let days;
    const filterTakenAttendanceList = takenAttendanceList.filter(x => (x.blockId === item.blockId) && (x.periodId === item.periodId));
    if (filterTakenAttendanceList)
      filterTakenAttendanceList?.map((takenDay) => {
        const day = new Date(takenDay.attendanceDate).getDay();
        days = days ? days + "|" + day : day.toString();
      });
    return days;
  }

  //  rendering the current week
  renderAttendanceWeekCalendar() {
    const today = new Date(this.todayDate);
    const cloneToday = today;
    for (let i = 0; i < 7; i++) {
      if (today.getDay() === 0) {
        this.initialRoutineDate = today;
        break;
      } else {
        cloneToday.setDate(today.getDate() - 1);
        if (cloneToday.getDay() === 0) {
          this.initialRoutineDate = cloneToday;
          break;
        }
      }
    }

    const cloneInitialDate = new Date(this.initialRoutineDate);
    this.endRoutingDate = new Date(
      cloneInitialDate.setDate(today.getDate() + 6)
    );

    for (
      const d = new Date(this.initialRoutineDate);
      d <= this.endRoutingDate;
      d.setDate(d.getDate() + 1)
    ) {
      this.currentWeek.push(new Date(d));
    }
  }

  confirmWeekChange(direction: string) {
    if (
      this.addUpdateStudentAttendanceModel360.studentAttendance?.length === 0
    ) {
      this.changeWeek(direction);
      return;
    }
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: "400px",
      data: {
        title: this.defaultService.translateKey("areYouSure"),
        message: this.defaultService.translateKey(
          "youWillLoseTheSelectedAttendanceDoYouWantToContinue"
        ),
        fromAttendance: true,
      },
    });
    dialogRef.afterClosed().subscribe((dialogResult) => {
      if (dialogResult) {
        this.addUpdateStudentAttendanceModel360.studentAttendance = [];
        this.changeWeek(direction);
      }
    });
  }

  changeWeek(direction) {
    if (direction === "prev") {
      this.previousWeek();
    } else if (direction === "next") {
      this.nextWeek();
    }
  }

  previousWeek() {
    const initialDate = new Date(this.initialRoutineDate);
    let count = 0;
    for (
      const d = new Date(initialDate.setDate(initialDate.getDate() - 7));
      d < this.initialRoutineDate;
      d.setDate(d.getDate() + 1)
    ) {
      this.currentWeek[count] = new Date(d);
      count++;
    }
    this.initialRoutineDate = this.currentWeek[0];
    this.endRoutingDate = this.currentWeek[this.currentWeek.length - 1];
    this.getAllScheduledCourseSectionList();
  }

  nextWeek() {
    const endDate = new Date(this.endRoutingDate);
    for (let i = 0; i < 7; i++) {
      this.currentWeek[i] = new Date(endDate.setDate(endDate.getDate() + 1));
    }
    this.initialRoutineDate = this.currentWeek[0];
    this.endRoutingDate = this.currentWeek[this.currentWeek.length - 1];
    this.getAllScheduledCourseSectionList();
  }

  onAttendanceChange(periodDetails, day) {
    let attendanceCode = periodDetails?.cloneTakenAttendanceDays[day]?.attendanceCode;
    let attendanceCategoryId;
    let staffId;
    let studentAttendanceComments = [];
    let attendanceDate;

    for (const takenDay of periodDetails.takenAttendanceList) {
      if (new Date(takenDay.attendanceDate).getDay() == day) {
        staffId = takenDay.staffId;
        studentAttendanceComments = takenDay.studentAttendanceComments;
        attendanceDate = takenDay.attendanceDate.split("T")[0];
        if (typeof (periodDetails.courseIds) === "string") {
          periodDetails.courseIds?.split("|").map(item => {
            if (+item === takenDay.courseId) periodDetails.courseId = takenDay.courseId;
          });
        } else {
          periodDetails.courseId = periodDetails.courseIds;
        }
        if (typeof (periodDetails.courseSectionIds) === "string") {
          periodDetails.courseSectionIds?.split("|").map(item => {
            if (+item === takenDay.courseSectionId) periodDetails.courseSectionId = takenDay.courseSectionId;
          });
        } else {
          periodDetails.courseSectionId = periodDetails.courseSectionIds;
        }
        break;
      }
    }
    periodDetails?.attendanceList.attendanceCode?.map((item) => {
      if (!attendanceCategoryId) {
        if (item.attendanceCode1 === +attendanceCode) {
          attendanceCategoryId = item.attendanceCategoryId;
        }
      }
    });
    const index =
      this.addUpdateStudentAttendanceModel360.studentAttendance.findIndex(
        (item) => {
          return (
            item.courseId === periodDetails.courseId &&
            item.courseSectionId === periodDetails.courseSectionId &&
            item.blockId === periodDetails.blockId &&
            item.periodId === periodDetails.periodId &&
            item.attendanceDate === attendanceDate
          );
        }
      );
    if (index !== -1) {
      this.addUpdateStudentAttendanceModel360.studentAttendance[index] =
        this.studentAttendanceDataSetForUpdate(
          periodDetails,
          day,
          attendanceCategoryId,
          attendanceCode,
          staffId,
          studentAttendanceComments
        );
    } else {
      this.addUpdateStudentAttendanceModel360.studentAttendance.push(
        this.studentAttendanceDataSetForUpdate(
          periodDetails,
          day,
          attendanceCategoryId,
          attendanceCode,
          staffId,
          studentAttendanceComments
        )
      );
    }
  }

  calculateDiffBetweenTimes(startTime, endTime) {
    let date_start: any = new Date();
    let date_end: any = new Date();
    const time_start = startTime.split(":");
    const time_end = endTime.split(":");
    date_start.setHours(time_start[0], time_start[1], 0);
    date_end.setHours(time_end[0], time_end[1], 0);

    return (date_end - date_start) / 1000 / 60;
  }

  studentAttendanceDataSetForUpdate(
    periodDetails,
    day,
    attendanceCategoryId,
    attendanceCode,
    staffId,
    studentAttendanceComments
  ) {
    const studentAttendanceFor360: StudentAttendanceModelFor360 = {
      studentId: this.studentService.getStudentId(),
      schoolId: this.defaultService.getSchoolID(),
      courseId: periodDetails.courseId,
      courseSectionId: periodDetails.courseSectionId,
      attendanceCategoryId,
      attendanceCode,
      studentAttendanceComments,
      staffId,
      attendanceDate: new Date(this.currentWeek[day])
        .toISOString()
        .split("T")[0],
      blockId: periodDetails.blockId,
      periodId: periodDetails.periodId,
      updatedBy: this.defaultService.getUserName(),
    };
    return studentAttendanceFor360;
  }

  onAttendanceUpdate() {
    this.addUpdateStudentAttendanceModel360.studentId =
      this.studentService.getStudentId();
    if (
      this.addUpdateStudentAttendanceModel360.studentAttendance?.length === 0
    ) {
      this.snackbar.open("Nothing to Update", "Ok", {
        duration: 3000,
      });
      return;
    }
    this.studentAttendanceService
      .addUpdateStudentAttendanceForStudent360(
        this.addUpdateStudentAttendanceModel360
      )
      .pipe(takeUntil(this.destroySubject$))
      .subscribe((res) => {
        if (res) {
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
            this.snackbar.open(res._message, "", {
              duration: 10000,
            });
          } else {
            this.snackbar.open(res._message, "", {
              duration: 10000,
            });
            this.addUpdateStudentAttendanceModel360.studentAttendance = [];
            this.monthEvents=[];
            this.events=[];
          }
        } else {
          this.snackbar.open(this.defaultService.getHttpError(), "", {
            duration: 10000,
          });
        }
      });
  }

  compareFn(obj1, obj2) {
    if (obj1 && obj2) {
      return obj1.attendanceCode === obj2.attendanceCode && obj1.stateCode === obj2.stateCode;
    }
  }

  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }
}
