import { Component, OnInit, ViewEncapsulation } from "@angular/core";
import icEdit from "@iconify/icons-ic/twotone-edit";
import icDelete from "@iconify/icons-ic/twotone-delete";
import icMoreVert from "@iconify/icons-ic/more-vert";
import icInfo from "@iconify/icons-ic/twotone-info";
import icPendingActions from "@iconify/icons-ic/twotone-pending-actions";
import icCalendarToday from "@iconify/icons-ic/twotone-calendar-today";
import { MatDialog } from "@angular/material/dialog";
import { TranslateService } from "@ngx-translate/core";
import { CalendarEvent, CalendarView } from "angular-calendar";
import {
  animate,
  state,
  style,
  transition,
  trigger,
} from "@angular/animations";
import { StaffService } from "../../../../services/staff.service";
import {
  CourseSectionViewModel,
  ScheduledCourseSectionsForStaffModel,
  ScheduledCourseSectionTableModel,
  ScheduleDetailsModel,
} from "../../../../models/staff.model";
import { scheduleType } from "../../../../enums/takeAttendanceList.enum";
import { GetMarkingPeriodTitleListModel } from "../../../../models/marking-period.model";
import { MatSnackBar } from "@angular/material/snack-bar";
import { MarkingPeriodService } from "../../../../services/marking-period.service";
import { Transform24to12Pipe } from "../../../shared-module/user-define-pipe/transform-24to12.pipe";
import { days, uniqueColors, weeks } from "../../../../common/static-data";
import { SharedFunction } from "../../../shared/shared-function";
import {
  RoutineView,
  RoutineViewEvent,
  RoutineViewModel,
} from "../../../../models/student-schedule.model";
import { DefaultValuesService } from "../../../../common/default-values.service";
import { ExcelService } from "../../../../services/excel.service";
import { CommonService } from "src/app/services/common.service";
import { GetStaffPrintScheduleReportModel } from "src/app/models/report.model";
import { ReportService } from "src/app/services/report.service";
@Component({
  selector: "vex-staff-course-schedule",
  templateUrl: "./staff-course-schedule.component.html",
  styleUrls: ["./staff-course-schedule.component.scss"],
  encapsulation: ViewEncapsulation.None,
  animations: [
    trigger("detailExpand", [
      state(
        "collapsed",
        style({ height: "0px", minHeight: "0", visibility: "hidden" })
      ),
      state("expanded", style({ height: "*", visibility: "visible" })),
      transition(
        "expanded <=> collapsed",
        animate("225ms cubic-bezier(0.4, 0.0, 0.2, 1)")
      ),
    ]),
  ],
})
export class StaffCourseScheduleComponent implements OnInit {
  icInfo = icInfo;
  icPendingActions = icPendingActions;
  icCalendarToday = icCalendarToday;
  icEdit = icEdit;
  icDelete = icDelete;
  icMoreVert = icMoreVert;

  SCHEDULE_TYPE = scheduleType;
  view: CalendarView | string = "routineView";
  viewDate: Date = new Date();
  events: CalendarEvent[] = [];
  CalendarView = CalendarView;
  scheduleSwitch = true;
  courseSectionDataTable: ScheduledCourseSectionTableModel[] = [];
  weeks = weeks;
  days = days;
  todayDate = new Date().toISOString().split("T")[0];
  routineViewBasedOn = "0"; // 0 for Period base, 1 for Time base
  currentWeek = [];
  initialRoutineDate: Date;
  endRoutingDate: Date;
  routineViewWithEvent: RoutineViewModel = new RoutineViewModel();

  displayedColumns = [
    "courseSection",
    "period",
    "markingPeriod",
    "time",
    "room",
    "meetingDays",
  ];
  displayedVariableColumns = ["day", "period", "time", "room"];
  displayedCalendarColumns = ["date", "day", "period", "time", "room"];
  staffCourseReport = null;
  uniqueColors = uniqueColors;
  expandedElement: any;
  scheduledCourseSectionModel: ScheduledCourseSectionsForStaffModel =
    new ScheduledCourseSectionsForStaffModel();
  getMarkingPeriodTitleListModel: GetMarkingPeriodTitleListModel =
    new GetMarkingPeriodTitleListModel();
  viewStartTime;
  viewEndTime;
  staffPrintScheduleReportModel: GetStaffPrintScheduleReportModel = new GetStaffPrintScheduleReportModel();
  staffPrintScheduleReportData;
  today: Date = new Date();
  courseSectionIds: number[] = [];
  constructor(
    private dialog: MatDialog,
    public translateService: TranslateService,
    private snackBar: MatSnackBar,
    public commonFunction: SharedFunction,
    private markingPeriodService: MarkingPeriodService,
    private staffService: StaffService,
    private excelService: ExcelService,
    private defaultService: DefaultValuesService,
    private commonService: CommonService,
    private reportService: ReportService,
  ) {
    //translateService.use('en');
  }

  ngOnInit(): void {
    this.getAllMarkingPeriodList().then(()=>{
      this.getScheduledCourseSections();
    });
    this.renderRoutineView();
  }

  isExpansionDetailRow = (i: number, row: Object) => {
    return row.hasOwnProperty("detailRow");
  };

  getAllMarkingPeriodList() {
    return new Promise((resolve, rej) => {
    this.getMarkingPeriodTitleListModel.academicYear =
      this.defaultService.getAcademicYear();
    this.markingPeriodService
      .getAllMarkingPeriodList(this.getMarkingPeriodTitleListModel)
      .subscribe((data) => {
       if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
          this.getMarkingPeriodTitleListModel.getMarkingPeriodView = [];
          if (!this.getMarkingPeriodTitleListModel?.getMarkingPeriodView) {
            this.snackBar.open(data._message, "", {
              duration: 1000,
            });
          }
        } else {
          this.getMarkingPeriodTitleListModel.getMarkingPeriodView =
            data.getMarkingPeriodView;
            resolve([]);
        }
      });
    });
  }

  getScheduledCourseSections() {
    this.scheduledCourseSectionModel.staffId = this.staffService.getStaffId();
    this.staffService
      .getScheduledCourseSectionsForStaff(this.scheduledCourseSectionModel)
      .subscribe((res) => {
        if (res) {
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
            this.scheduledCourseSectionModel.courseSectionViewList = [];
            if (!this.scheduledCourseSectionModel.courseSectionViewList) {
              this.snackBar.open(res._message, "", {
                duration: 1000,
              });
            }
          } else {
            res.courseSectionViewList = this.findMarkingPeriodTitleById(
              res.courseSectionViewList
            );
            this.scheduledCourseSectionModel = res;
            this.courseSectionIds = res.courseSectionViewList?.map(item => item.courseSectionId) || [];
            this.createTableDataset(res.courseSectionViewList);
            this.generateEvents();
          }
        }
      });
  }

  createTableDataset(courseSectionList: CourseSectionViewModel[]) {
    courseSectionList.map((courseSection) => {
      if (courseSection.scheduleType === this.SCHEDULE_TYPE.FixedSchedule) {
        courseSection.cloneMeetingDays = courseSection.meetingDays;
        const days = courseSection.cloneMeetingDays.split("|");
        courseSection.cloneMeetingDays = "";
        days.map((day) => {
          for (const [i, weekDay] of this.weeks.entries()) {
            if (weekDay.name.toLowerCase() == day.trim().toLowerCase()) {
              courseSection.cloneMeetingDays =
                courseSection.cloneMeetingDays + weekDay.id;
              break;
            }
          }
        });
        this.courseSectionDataTable.push({
          scheduleType: this.SCHEDULE_TYPE.FixedSchedule,
          weekDays: courseSection.weekDays,
          courseSection: courseSection.courseSectionName,
          period: courseSection.courseFixedSchedule.blockPeriod.periodTitle,
          time:
            Transform24to12Pipe.prototype.transform(
              courseSection.courseFixedSchedule.blockPeriod.periodStartTime
            ) +
            " - " +
            Transform24to12Pipe.prototype.transform(
              courseSection.courseFixedSchedule.blockPeriod.periodEndTime
            ),
          markingPeriod: courseSection.mpTitle,
          room: courseSection.courseFixedSchedule.rooms.title,
          meetingDays: courseSection.cloneMeetingDays,
        });
      } else if (
        courseSection.scheduleType === this.SCHEDULE_TYPE.variableSchedule
      ) {
        let scheduleDetails: ScheduleDetailsModel[] = [];
        courseSection.courseVariableSchedule?.map((varSchedule) => {
          scheduleDetails.push({
            day: varSchedule.day.toString(),
            period: varSchedule.blockPeriod.periodTitle,
            room: varSchedule.rooms.title,
            time:
              Transform24to12Pipe.prototype.transform(
                varSchedule.blockPeriod.periodStartTime
              ) +
              " - " +
              Transform24to12Pipe.prototype.transform(
                varSchedule.blockPeriod.periodEndTime
              ),
          });
        });
        this.courseSectionDataTable.push({
          scheduleType: this.SCHEDULE_TYPE.variableSchedule,
          rowExpand: true,
          courseSection: courseSection.courseSectionName,
          period: "-",
          time: "-",
          markingPeriod: courseSection.mpTitle,
          room: "-",
          scheduleDetails,
        });
      } else if (
        courseSection.scheduleType === this.SCHEDULE_TYPE.calendarSchedule
      ) {
        let scheduleDetails: ScheduleDetailsModel[] = [];
        courseSection.courseCalendarSchedule?.map((calSchedule) => {
          scheduleDetails.push({
            date: calSchedule.date,
            day: this.dayToName(new Date(calSchedule.date).getDay()),
            period: calSchedule.blockPeriod.periodTitle,
            room: calSchedule.rooms.title,
            time:
              Transform24to12Pipe.prototype.transform(
                calSchedule.blockPeriod.periodStartTime
              ) +
              " - " +
              Transform24to12Pipe.prototype.transform(
                calSchedule.blockPeriod.periodEndTime
              ),
          });
        });
        this.courseSectionDataTable.push({
          scheduleType: this.SCHEDULE_TYPE.calendarSchedule,
          rowExpand: true,
          courseSection: courseSection.courseSectionName,
          period: "-",
          time: "-",
          markingPeriod: courseSection.mpTitle,
          room: "-",
          scheduleDetails,
        });
      } else if(courseSection.scheduleType === this.SCHEDULE_TYPE.blockSchedule) {
        let scheduleDetails = [];
        courseSection.bellScheduleList?.map((bellSchedule) => {
          let block;
          courseSection.courseBlockSchedule.map((subItem)=>{
            if(subItem.blockId === bellSchedule.blockId) {
              block = subItem;
            }
          })
          scheduleDetails.push({
            date: bellSchedule.bellScheduleDate,
            day: this.dayToName(new Date(bellSchedule.bellScheduleDate).getDay()),
            period: block.blockPeriod.periodTitle,
            room: block.rooms.title,
            time:
              Transform24to12Pipe.prototype.transform(
                block.blockPeriod.periodStartTime
              ) +
              " - " +
              Transform24to12Pipe.prototype.transform(
                block.blockPeriod.periodEndTime
              ),
          });
        });

        this.courseSectionDataTable.push({
          scheduleType: this.SCHEDULE_TYPE.blockSchedule,
          rowExpand: true,
          courseSection: courseSection.courseSectionName,
          period: "-",
          time: "-",
          markingPeriod: courseSection.mpTitle,
          room: "-",
          scheduleDetails,
        });
      }
    });
    const rows = [];
    this.courseSectionDataTable.forEach((element) => {
      if (element.hasOwnProperty("rowExpand")) {
        rows.push(element, { detailRow: true, element });
      } else {
        rows.push(element);
      }
    });
    this.staffCourseReport = rows;
  }

  dayToName(day) {
    for (let weekDay of weeks) {
      if (day === weekDay.id) {
        return weekDay.name;
      }
    }
    return null;
  }

  changeCalendarView(calendarType) {
    this.scheduleSwitch = false;
    this.view = calendarType;    
  }

  listAll() {
    this.scheduleSwitch = true;
  }

  findMarkingPeriodTitleById(courseSectionList) {
    courseSectionList = courseSectionList.map((item) => {
      if (item.yrMarkingPeriodId) {
        item.yrMarkingPeriodId = "0_" + item.yrMarkingPeriodId;
      } else if (item.smstrMarkingPeriodId) {
        item.smstrMarkingPeriodId = "1_" + item.smstrMarkingPeriodId;
      } else if (item.qtrMarkingPeriodId) {
        item.qtrMarkingPeriodId = "2_" + item.qtrMarkingPeriodId;
      } else if (item.prgrsprdMarkingPeriodId) {
        item.prgrsprdMarkingPeriodId = "3_" + item.prgrsprdMarkingPeriodId;
      }

      if (
        item.yrMarkingPeriodId ||
        item.smstrMarkingPeriodId ||
        item.qtrMarkingPeriodId ||
        item.prgrsprdMarkingPeriodId
      ) {
        for (let markingPeriod of this.getMarkingPeriodTitleListModel
          .getMarkingPeriodView) {
          if (markingPeriod.value == item.yrMarkingPeriodId) {
            item.mpTitle = markingPeriod.text;
            break;
          } else if (markingPeriod.value == item.smstrMarkingPeriodId) {
            item.mpTitle = markingPeriod.text;
            break;
          } else if (markingPeriod.value == item.qtrMarkingPeriodId) {
            item.mpTitle = markingPeriod.text;
            break;
          } else if (markingPeriod.value == item.prgrsprdMarkingPeriodId) {
            item.mpTitle = markingPeriod.text;
            break;
          }
        }
      } else {
        item.mpTitle = "Custom";
      }
      return item;
    });
    return courseSectionList;
  }

  generateEvents() {
    this.scheduledCourseSectionModel.courseSectionViewList.forEach((item) => {
      let uniqueColorClass;
      uniqueColorClass = this.generateUniqueColor(item);
      const effectiveEndDate = new Date(
        new Date(item.durationEndDate).setDate(
          new Date(item.durationEndDate).getDate() + 1
        )
      );
      for (
        let dt = new Date(item.durationStartDate);
        dt <= effectiveEndDate;
        dt.setDate(dt.getDate() + 1)
      ) {
        let formatedDate: string | Date = new Date(dt);
        const dayName = days[formatedDate.getDay()];
        formatedDate = formatedDate.toLocaleDateString();

        if (item.courseFixedSchedule) {
          this.renderFixedSchedule(
            item,
            dayName,
            formatedDate,
            uniqueColorClass
          );
        } else if (item.courseVariableSchedule?.length > 0) {
          this.renderVariableSchedule(
            item,
            dayName,
            formatedDate,
            uniqueColorClass
          );
        } else if (item.courseCalendarSchedule?.length > 0) {
          this.renderCalendarSchedule(
            item,
            dayName,
            formatedDate,
            uniqueColorClass
          );
        } 
      }
     if(item.bellScheduleList?.length > 0) {
        this.renderBlockSchedule(
          item,
          uniqueColorClass
        );
      }
    });
    this.createDatasetForRoutine();
  }

  renderFixedSchedule(item, dayName, formatedDate, uniqueColorClass) {
    const startTime = item.courseFixedSchedule.blockPeriod.periodStartTime;
    const startDateTime = new Date(formatedDate + " " + startTime);

    const endTime = item.courseFixedSchedule.blockPeriod.periodEndTime;
    const endDateTime = new Date(formatedDate + " " + endTime);
    item.meetingDays?.split("|").forEach((day) => {
      if (dayName === day) {
        this.pushItemIntoEvent(item,startDateTime, endDateTime, uniqueColorClass, item.courseFixedSchedule.blockPeriod)
      }
    });
  }

  renderVariableSchedule(item, dayName, formatedDate, uniqueColorClass) {
    item.courseVariableSchedule.forEach((variableSchedule) => {
      const startTime = variableSchedule.blockPeriod.periodStartTime;
      const startDateTime = new Date(formatedDate + " " + startTime);
      const endTime = variableSchedule.blockPeriod.periodEndTime;
      const endDateTime = new Date(formatedDate + " " + endTime);
      if (dayName === variableSchedule.day) {
          this.pushItemIntoEvent(item,startDateTime, endDateTime, uniqueColorClass, variableSchedule.blockPeriod)
      }
    });
  }

  renderBlockSchedule(item, uniqueColorClass) {
    item.bellScheduleList.forEach((blockSchedule) => {      
      let startDateTime;
      let endDateTime;
      let blockPeriod;
      item.courseBlockSchedule.map((subItem)=>{
        if(subItem.blockId === blockSchedule.blockId) {
          blockPeriod = subItem.blockPeriod;
          const startTime = subItem.blockPeriod.periodStartTime;          
           startDateTime = new Date(blockSchedule.bellScheduleDate.split('T')[0] + " " + startTime);
          const endTime = subItem.blockPeriod.periodEndTime;
           endDateTime = new Date(blockSchedule.bellScheduleDate.split('T')[0] + " " + endTime);
        }
      })
      this.pushItemIntoEvent(item,startDateTime, endDateTime, uniqueColorClass, blockPeriod)
    });
  }


  pushItemIntoEvent(item,startDateTime, endDateTime, uniqueColorClass, blockPeriod ){
    this.events.push({
      title:
        item.courseSectionName +
        "" +
        (item.courseGradeLevel ? " - " + item.courseGradeLevel : ""),
      start: startDateTime,
      end: endDateTime,
      color: { primary: uniqueColorClass.textColorInHex, secondary: "white" },
      meta: {
        color: uniqueColorClass,
        monthTitle:
          Transform24to12Pipe.prototype.transform(
            blockPeriod.periodStartTime
          ) +
          " - " +
          item.courseSectionName,
        scheduleDetails: item,
        gradeLevel: item.courseGradeLevel,
        periodDetails: blockPeriod,
      },
    });
  }

  renderCalendarSchedule(item, dayName, formatedDate, uniqueColorClass) {
    item.courseCalendarSchedule.forEach((calendarSchedule) => {
      const startTime = calendarSchedule.blockPeriod.periodStartTime;
      const startDateTime = new Date(formatedDate + " " + startTime);

      const endTime = calendarSchedule.blockPeriod.periodEndTime;
      const endDateTime = new Date(formatedDate + " " + endTime);

      const scheduledDate = new Date(
        this.commonFunction.serverToLocalDateAndTime(calendarSchedule.date)
      ).toLocaleDateString();
      const currentDate = formatedDate;
      if (currentDate === scheduledDate) {
        this.pushItemIntoEvent(item,startDateTime, endDateTime, uniqueColorClass, calendarSchedule.blockPeriod)
      }
    });
  }

  

  generateUniqueColor(item) {
    let uniqueColorClass = {
      backgroundColor: null,
      textColor: null,
      textColorInHex:null
    };

    const foundIndex = this.uniqueColors.findIndex(
      (color) => color.id === item.courseSectionId
    );

    if (foundIndex !== -1) {
      uniqueColorClass.backgroundColor =
        this.uniqueColors[foundIndex].backgroundColor;
      uniqueColorClass.textColor = this.uniqueColors[foundIndex].textColor;
      uniqueColorClass.textColorInHex = this.uniqueColors[foundIndex].textColorInHex;
    } else {
      let foundUniqueColor = false;
      for (let [i, color] of uniqueColors.entries()) {
        if (!color.used) {
          uniqueColorClass.backgroundColor = color.backgroundColor;
          uniqueColorClass.textColor = color.textColor;
          uniqueColorClass.textColorInHex = color.textColorInHex;

          this.uniqueColors[i].used = true;
          this.uniqueColors[i].id = item.courseSectionId;

          foundUniqueColor = true;
          break;
        }
      }
      if (!foundUniqueColor) {
        uniqueColorClass.backgroundColor = this.uniqueColors[0].backgroundColor;
        uniqueColorClass.textColor = this.uniqueColors[0].textColor;
        uniqueColorClass.textColorInHex = this.uniqueColors[0].textColorInHex;
      }
    }
    return uniqueColorClass;
  }

  renderRoutineView() {
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
  createDatasetForRoutine() {
    this.routineViewWithEvent.routineView = [];
    this.events.map((item,index) => {
      const foundIndex = this.routineViewWithEvent.routineView?.findIndex(
        (routine) => {
          return (
            routine.blockId === item.meta.periodDetails.blockId &&
            routine.periodId === item.meta.periodDetails.periodId
          );
        }
      );
      if (foundIndex !== -1) {
        let event: RoutineViewEvent;
        event = {
          date: item.start.toISOString().split("T")[0],
          courseSectionName: item.meta.scheduleDetails.courseSectionName,
          staffName: item.meta.teacherName,
          gradeLevel: item.meta.gradeLevel,
          color: item.meta.color.textColor,
        };
        this.routineViewWithEvent.routineView[foundIndex].events.push(event);
      } else {
        let eachEvent: RoutineView = new RoutineView();
        eachEvent = {
          periodId: item.meta.periodDetails.periodId,
          blockId: item.meta.periodDetails.blockId,
          periodName: item.meta.periodDetails.periodTitle,
          periodStartTime: item.meta.periodDetails.periodStartTime,
          periodEndTime: item.meta.periodDetails.periodEndTime,
          events: [],
          filteredEvents: [],
        };
        let event: RoutineViewEvent;
        event = {
          date: item.start.toISOString().split("T")[0],
          courseSectionName: item.meta.scheduleDetails.courseSectionName,
          staffName: item.meta.teacherName,
          gradeLevel: item.meta.gradeLevel,
          color: item.meta.color.textColor,
        };
        eachEvent.events.push(event);
        this.routineViewWithEvent.routineView.push(eachEvent);
      }
      if (index === 0) {
        this.viewStartTime = item.start.getHours();
        this.viewEndTime = item.end.getHours();
      } else {
        if (this.viewStartTime > item.start.getHours())  this.viewStartTime = item.start.getHours();
        if (this.viewEndTime < item.end.getHours()) this.viewEndTime = item.end.getHours();
      }
    });

    this.filterEventsForRoutineView();
  }

  filterEventsForRoutineView() {
    let count = 0;
    for (
      const dt = new Date(this.initialRoutineDate);
      dt <= this.endRoutingDate;
      dt.setDate(dt.getDate() + 1)
    ) {
      this.routineViewWithEvent.routineView.map((period) => {
        let filtered = [];
        period.events.forEach((event) => {
          if (
            new Date(event.date).getTime() ==
            new Date(dt.toISOString().split("T")[0]).getTime()
          ) {
            filtered.push(event);
          }
        });
        period.filteredEvents[count] = filtered;
      });
      count = count + 1;
    }
  }

  previousWeek() {
    let initialDate = new Date(this.initialRoutineDate);
    let count = 0;
    for (
      let d = new Date(initialDate.setDate(initialDate.getDate() - 7));
      d < this.initialRoutineDate;
      d.setDate(d.getDate() + 1)
    ) {
      this.currentWeek[count] = new Date(d);
      count++;
    }
    this.initialRoutineDate = this.currentWeek[0];
    this.endRoutingDate = this.currentWeek[this.currentWeek.length - 1];
    this.filterEventsForRoutineView();
  }

  nextWeek() {
    let endDate = new Date(this.endRoutingDate);
    for (let i = 0; i < 7; i++) {
      this.currentWeek[i] = new Date(endDate.setDate(endDate.getDate() + 1));
    }
    this.initialRoutineDate = this.currentWeek[0];
    this.endRoutingDate = this.currentWeek[this.currentWeek.length - 1];
    this.filterEventsForRoutineView();
  }

  exportScheduledCourses() {
    if(!this.courseSectionDataTable?.length){
      this.snackBar.open('No Records Found. Failed to Export Scheduled Course Sections', '', {
        duration: 5000
      });
      return;
    }
      let scheduledCoursesSectionList;
      scheduledCoursesSectionList = this.courseSectionDataTable?.map((item) => {
        return {
          [this.defaultService.translateKey('courseSection')]: item.courseSection,
          [this.defaultService.translateKey('period')]:item.period,
          [this.defaultService.translateKey('markingPeriod')]: item.markingPeriod,
          [this.defaultService.translateKey('time')]: item.time,
          [this.defaultService.translateKey('room')]: item.room,
          [this.defaultService.translateKey('meetingDays')]: item.scheduleType
        };
      });
      this.excelService.exportAsExcelFile(scheduledCoursesSectionList, 'Scheduled_Course_Secion_List_');
  }
  getSlicedCourseSection(item: string): string {
    if(item?.length >= 10){
      return item.slice(0, 10) + '....';
    }else{
      return item;
    }
  }

  getStaffPrintScheduleReport() {
    return new Promise((resolve, reject) => {
      this.reportService.getStaffPrintScheduleReport(this.staffPrintScheduleReportModel).subscribe(res => {
        if (res) {
          if (res._failure) {
            this.commonService.checkTokenValidOrNot(res._message);
            this.snackBar.open(res._message, '', {
              duration: 10000
            });
          } else {
            resolve(res);
          }
        } else {
          this.snackBar.open(this.defaultService.getHttpError(), '', {
            duration: 10000
          });
        }
      });
    });
  }

  printSchedule() {
    if (!this.courseSectionIds.length) {
      this.snackBar.open(this.defaultService.translateKey('thisStaffHasNotBeenScheduledIntoAnyCourse'), '', {
        duration: 5000
      });
      return;
    }

    this.staffPrintScheduleReportModel.staffId = this.staffService.getStaffId();
    this.staffPrintScheduleReportModel.courseSectionIds = this.courseSectionIds;

    this.getStaffPrintScheduleReport().then((res: GetStaffPrintScheduleReportModel) => {
      this.staffPrintScheduleReportData = res;
      this.staffPrintScheduleReportData.weekPages = this.buildWeekPages(res);

      setTimeout(() => {
        this.generatePDF();
      }, 100);
    });
  }

  private buildWeekPages(res: GetStaffPrintScheduleReportModel) {
    const dayNames = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

    const parseLocalDate = (s: any): Date => {
      const m = String(s).match(/^(\d{4})-(\d{2})-(\d{2})/);
      if (m) return new Date(+m[1], +m[2] - 1, +m[3]);
      const d = new Date(s);
      return new Date(d.getFullYear(), d.getMonth(), d.getDate());
    };
    const ymd = (d: Date) => `${d.getFullYear()}-${d.getMonth()}-${d.getDate()}`;

    const entries: { date: Date; key: string; periodId: number; periodName: string; roomName: string; courseName: string; courseSectionName: string; }[] = [];

    res.staffDetails?.courseDetailsViewModelList?.forEach(course => {
      course.courseSectionDetailsViewModelList?.forEach(section => {
        section.dayDetailsViewModelList?.forEach(day => {
          day.datePeriodRoomDetailsViewModelList?.forEach(d => {
            const date = parseLocalDate(d.date);
            entries.push({
              date,
              key: ymd(date),
              periodId: d.periodId,
              periodName: d.periodName,
              roomName: d.roomName,
              courseName: course.courseName,
              courseSectionName: section.courseSectionName,
            });
          });
        });
      });
    });

    if (!entries.length) return [];

    const periodMap = new Map<number, string>();
    entries.forEach(e => { if (!periodMap.has(e.periodId)) periodMap.set(e.periodId, e.periodName); });
    const periods = Array.from(periodMap.entries())
      .map(([id, name]) => ({ id, name }))
      .sort((a, b) => a.id - b.id);

    const dowSet = new Set<number>();
    entries.forEach(e => dowSet.add(e.date.getDay()));
    const daysOfWeek = Array.from(dowSet).sort((a, b) => a - b);

    const weekMap = new Map<string, typeof entries>();
    entries.forEach(e => {
      const sunday = new Date(e.date);
      sunday.setDate(sunday.getDate() - sunday.getDay());
      const key = ymd(sunday);
      if (!weekMap.has(key)) weekMap.set(key, []);
      weekMap.get(key).push(e);
    });

    return Array.from(weekMap.entries())
      .map(([key, weekEntries]) => {
        const [y, mo, d] = key.split('-').map(Number);
        return { sunday: new Date(y, mo, d), weekEntries };
      })
      .sort((a, b) => a.sunday.getTime() - b.sunday.getTime())
      .map(({ sunday, weekEntries }) => {
        const weekDays = daysOfWeek.map(dow => {
          const dt = new Date(sunday);
          dt.setDate(dt.getDate() + dow);
          return { date: dt, dayName: dayNames[dow], dow, key: ymd(dt) };
        });
        const rows = periods.map(p => ({
          periodName: p.name,
          cells: weekDays.map(wd => ({
            entries: weekEntries.filter(e => e.periodId === p.id && e.key === wd.key),
          })),
        }));
        const saturday = new Date(sunday);
        saturday.setDate(saturday.getDate() + 6);
        return { weekStart: sunday, weekEnd: saturday, days: weekDays, rows };
      });
  }

  generatePDF() {
    let printContents, popupWin;
    printContents = document.getElementById('staffPrintSectionId').innerHTML;
    document.getElementById('staffPrintSectionId').className = 'block';
    popupWin = window.open('', '_blank', 'top=0,left=0,height=100%,width=auto');
    if (popupWin === null || typeof (popupWin) === 'undefined') {
      document.getElementById('staffPrintSectionId').className = 'hidden';
      this.snackBar.open("User needs to allow the popup from the browser", '', {
        duration: 10000
      });
    } else {
      popupWin.document.open();
      popupWin.document.write(`
      <html>
        <head>
          <title>Print tab</title>
          <style>
          @page { size: landscape; margin: 12mm; }
          h1, h2, h3, h4, h5, h6, p { margin: 0; }
          body { -webkit-print-color-adjust: exact; print-color-adjust: exact; font-family: Arial, sans-serif; background-color: #fff; color: #121212; }
          .week-page { page-break-after: always; }
          .week-page:last-child { page-break-after: auto; }
          .report-header { display: flex; align-items: center; border-bottom: 2px solid #000; padding-bottom: 8px; margin-bottom: 8px; }
          .school-logo { width: 56px; height: 56px; border-radius: 50%; border: 1px solid #cacaca; margin-right: 12px; overflow: hidden; flex-shrink: 0; }
          .school-logo img { width: 100%; height: 100%; object-fit: cover; }
          .header-info { flex: 1; }
          .header-info h4 { font-size: 16px; font-weight: 600; }
          .header-info p { font-size: 12px; color: #555; }
          .header-right { text-align: right; }
          .header-right .title { font-size: 14px; font-weight: 600; }
          .header-right .powered { font-size: 11px; color: #666; }
          .staff-line { display: flex; justify-content: space-between; align-items: center; padding: 6px 0; font-size: 13px; }
          .staff-line .name { font-size: 15px; font-weight: 600; }
          .staff-line .meta { color: #555; }
          .week-range { background: #f0f0f0; padding: 6px 10px; font-size: 13px; font-weight: 600; margin-bottom: 6px; border-left: 4px solid #1976d2; }
          table.timetable-grid { border-collapse: collapse; width: 100%; table-layout: fixed; }
          table.timetable-grid th, table.timetable-grid td { border: 1px solid #888; padding: 6px 8px; vertical-align: top; font-size: 12px; }
          table.timetable-grid thead th { background-color: #e3f2fd; text-align: center; font-weight: 600; }
          table.timetable-grid thead th .day-date { font-size: 10px; color: #555; font-weight: 400; display: block; margin-top: 2px; }
          table.timetable-grid .period-col { width: 110px; background-color: #f0f0f0; font-weight: 600; text-align: center; }
          table.timetable-grid td.day-cell { height: 60px; }
          .cell-entry { padding: 2px 0; }
          .cell-entry + .cell-entry { border-top: 1px dashed #ccc; margin-top: 2px; padding-top: 4px; }
          .cell-entry .course-section { font-weight: 600; }
          .cell-entry .room { font-size: 11px; color: #555; }
          </style>
        </head>
        <body onload="window.print()">${printContents}</body>
      </html>`
      );
      popupWin.document.close();
      document.getElementById('staffPrintSectionId').className = 'hidden';
      return;
    }
  }
}
