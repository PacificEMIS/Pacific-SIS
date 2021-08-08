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

import { Component, OnInit, ViewEncapsulation } from "@angular/core";
import {
  CalendarEvent,
  CalendarView,
} from "angular-calendar";
import { Router} from '@angular/router';
import { Subject } from "rxjs";
import { StudentAttendanceService } from "../../../../../services/student-attendance.service";
import { SearchCourseSectionForStudentAttendance } from "../../../../../models/take-attendance-list.model";
import { MatSnackBar } from "@angular/material/snack-bar";
import { GetAllCourseListModel } from "../../../../../models/course-manager.model";
import { scheduleType } from "../../../../../enums/takeAttendanceList.enum";
import { days } from "../../../../../common/static-data";
import { color } from "../../../../../common/static-data";
import * as moment from 'moment';
import { LoaderService } from "../../../../../services/loader.service";
import { LayoutService } from "src/@vex/services/layout.service";
import { CommonService } from "src/app/services/common.service";
@Component({
  selector: "vex-course-section",
  templateUrl: "./course-section.component.html",
  styleUrls: ["./course-section.component.scss"],
  styles: [
    `
      .cal-month-view .bg-aqua,
      .cal-week-view .cal-day-columns .bg-aqua,
      .cal-day-view .bg-aqua {
        background-color: #ffdee4 !important;
      }
    `,
  ],
  encapsulation: ViewEncapsulation.None,
})
export class CourseSectionComponent implements OnInit {
  view: CalendarView = CalendarView.Month;
  viewDate: Date = new Date();
  // events: CalendarEvent[] = [];
  cssClass: string;
  refresh: Subject<any> = new Subject();
  pageStatus = "Teacher Function";
  staffDetails;
  studentAttendanceModel: SearchCourseSectionForStudentAttendance = new SearchCourseSectionForStudentAttendance();
  events: CalendarEvent[] = [];
  getAllCourseListModel: GetAllCourseListModel = new GetAllCourseListModel();
  courseSectionList = [];
  selectedCourseSectionForFilter = [];
  masterCalendarEvents: CalendarEvent<any>[];
  loading:boolean;

  constructor(
    private router: Router,
    private studentAttendanceService: StudentAttendanceService,
    private snackbar: MatSnackBar,
    private loaderService: LoaderService,
    private layoutService: LayoutService,
    private commonService: CommonService,
    ) {
      this.staffDetails = this.studentAttendanceService.getStaffDetails();
      Object.keys(this.staffDetails).length > 0 ? this.getAllScheduledCource() : this.router.navigate(['/school', 'staff', 'teacher-functions', 'take-attendance', 'course-section']);
      // this.getAllCourseSections();
      this.loaderService.isLoading.subscribe((val) => {
        this.loading = val;
      });
    this.layoutService.collapseSidenav();

  }

  ngOnInit(): void {}

  eventClicked(event: CalendarEvent, index: number): void {
      if(moment(event.meta.attendanceDate).isBefore(new Date())){
        Object.assign(this.staffDetails, event.meta);
        this.studentAttendanceService.setStaffDetails(this.staffDetails);
        this.router.navigate(['/school', 'staff', 'teacher-functions', 'take-attendance', 'period-attendance']);
      }else{
        this.snackbar.open('Future attendance is not allowed.', 'Ok', {
          duration: 1000
        });
      }
  }

  getAllScheduledCource() {
    this.studentAttendanceModel.staffId = this.staffDetails.staffId;
    this.studentAttendanceService.getAllCourcesForStudentAttendance(this.studentAttendanceModel).subscribe((data: any) => {
      if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
        this.snackbar.open('NO RECORD FOUND. ', '', {
          duration: 1000
        });
      } else {
        this.generateEventForCalendar(data.courseSectionViewList);
      }
    });
  }

  getAllCourseSections(){
    this.studentAttendanceService.getAllCourseSectionList(this.getAllCourseListModel).subscribe(data => {
      if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
        this.courseSectionList=[]
          if(!data.courseViewModelList){
            this.snackbar.open(data._message, '', {
              duration: 1000
            }); 
          }
                 
      } else {
        data.courseViewModelList.forEach((item: any)=>{
          item.course.courseSection.forEach((subItem)=>{
            this.courseSectionList.push(subItem);
          })
        });        
      }
    });

  }

  filterWithSectionName(courseSectionName, event) {
    if(event.target.checked) {
      this.selectedCourseSectionForFilter.push(courseSectionName);
    } else {
      const index =  this.selectedCourseSectionForFilter.findIndex(x=> x === courseSectionName);
      index !== -1 ? this.selectedCourseSectionForFilter.splice(index, 1) : '';
    }
    if(this.selectedCourseSectionForFilter.length > 0) {
      this.events = [];
    this.selectedCourseSectionForFilter.forEach((item)=>{
      this.masterCalendarEvents.forEach((subItem: any)=>{
        if(item.courseSectionId === subItem.meta.courseSectionId) {
          this.events.push(subItem);
        }
      })
    })
  } else {
    this.events = this.masterCalendarEvents;
  }
  this.refresh.next();
  }

  generateEventForCalendar(responseFromServer) {
    const arr = [];

    responseFromServer.forEach((item, index) => {
      let staticIndex = 0;
      staticIndex = color.length < staticIndex ? 0 : staticIndex;
      staticIndex = color.length < index + 1 ? staticIndex + 1 : 0;

      if (
        item.scheduleType === scheduleType.FixedSchedule ||
        item.scheduleType === scheduleType.variableSchedule
      ) {

        for (
          const dt = new Date(item.durationStartDate);
          dt <= new Date(item.durationEndDate);
          dt.setDate(dt.getDate() + 1)
        ) {
          const formatedDate = new Date(dt);
          const dayName = days[formatedDate.getDay()];
          if (item.scheduleType === scheduleType.FixedSchedule) {

            item.meetingDays.split("|").forEach(subItem => {
              if (dayName === subItem) {
                this.events.push({
                  title: item.courseSectionName + " - " + item.courseFixedSchedule.blockPeriod.periodTitle,
                  start: new Date(dt),
                  color: null,
                  meta: {
                    blockId: item.courseFixedSchedule.blockId,
                    periodId: item.courseFixedSchedule.blockPeriod.periodId,
                    courseSectionId: item.courseSectionId,
                    attendanceDate: new Date(dt),
                    takeAttendance: item.attendanceTaken,
                    courseId: item.courseId,
                    courseSectionName: item.courseSectionName,
                    periodTitle: item.courseFixedSchedule.blockPeriod.periodTitle,
                    attendanceCategoryId: item.attendanceCategoryId ? item.attendanceCategoryId : 1,
                    randomColor: color[color.length < index + 1 ? staticIndex - 1 : index]
                  },
                });
              }
            });
            
          } else if (item.scheduleType === scheduleType.variableSchedule) {
            item.courseVariableSchedule.forEach(subItem => {
              if (dayName === subItem.day) {
                this.events.push({
                  title: item.courseSectionName + " - " + subItem.blockPeriod.periodTitle,
                  start: new Date(dt),
                  color: null,
                  meta: {
                    blockId: subItem.blockId,
                    periodId: subItem.blockPeriod.periodId,
                    courseSectionId: item.courseSectionId,
                    attendanceDate: new Date(dt),
                    takeAttendance: subItem.takeAttendance,
                    courseId: item.courseId,
                    courseSectionName: item.courseSectionName,
                    periodTitle: subItem.blockPeriod.periodTitle,
                    attendanceCategoryId: item.attendanceCategoryId ? item.attendanceCategoryId : 1,
                    randomColor: color[color.length < index + 1 ? staticIndex - 1 : index]
                  },
                });
              }
            });
          }
        }
      } else {
        item.courseCalendarSchedule.forEach(subItem => {
          this.events.push({
            title: item.courseSectionName + " - " + subItem.blockPeriod.periodTitle,
            start: new Date(subItem.date),
            color: null,
            meta: {
              blockId: subItem.blockId,
              periodId: subItem.blockPeriod.periodId,
              courseSectionId: item.courseSectionId,
              attendanceDate: new Date(subItem.date),
              takeAttendance: subItem.takeAttendance,
              courseId: item.courseId,
              courseSectionName: item.courseSectionName,
              periodTitle: subItem.blockPeriod.periodTitle,
              attendanceCategoryId: item.attendanceCategoryId ? item.attendanceCategoryId : 1,
              randomColor: color[color.length < index + 1 ? staticIndex - 1 : index]
            },
          });
        });
      }
    });

    this.events.forEach((item)=>{
      if(item.meta.takeAttendance && this.courseSectionList.findIndex(x => x.courseSectionId === item.meta.courseSectionId) === -1){
        this.courseSectionList.push(item.meta);
      }
    });
    this.masterCalendarEvents = this.events
    this.refresh.next();
  }

}
