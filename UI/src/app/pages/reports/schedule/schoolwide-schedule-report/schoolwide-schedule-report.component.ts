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

import { Component, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { days } from 'src/app/common/static-data';
import { CalendarDataViewModel, GetSchoolwideScheduleReportModel } from 'src/app/models/report.model';
import { SharedFunction } from 'src/app/pages/shared/shared-function';
import { LoaderService } from 'src/app/services/loader.service';
import { ReportService } from 'src/app/services/report.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'vex-schoolwide-schedule-report',
  templateUrl: './schoolwide-schedule-report.component.html',
  styleUrls: ['./schoolwide-schedule-report.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class SchoolwideScheduleReportComponent implements OnInit, OnDestroy {

  currentTab: string;
  dayIndex: number;
  startDate;
  endDate;
  currentWeek = [];
  initialRoutineDate: Date;
  endRoutineDate: Date;
  todayDate = new Date().toISOString().split("T")[0];
  days = days;
  getSchoolwideScheduleReportModel: GetSchoolwideScheduleReportModel = new GetSchoolwideScheduleReportModel();
  loading: boolean;
  destroySubject$: Subject<void> = new Subject();
  periodList = [];
  dayWithCourseList = [];
  calendarDataViewModel: CalendarDataViewModel = new CalendarDataViewModel();
  isPreviousButtonEnabled: boolean;
  isNextButtonEnabled: boolean;
  weekDaysList = [];
  holidayList = [];

  constructor(
    public defaultValuesService: DefaultValuesService,
    private reportService: ReportService,
    private loaderService: LoaderService,
    private snackbar: MatSnackBar,
    private commonFunction: SharedFunction,
    public translateService: TranslateService
  ) {
    this.defaultValuesService.setReportCompoentTitle.next("Schoolwide Schedule Report");
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });
  }

  ngOnInit(): void {
    this.currentTab = days[new Date().getDay()];
    this.dayIndex = new Date().getDay();
    this.renderCurrentWeek();
  }

  startDateChange() {
    this.currentTab = days[new Date(this.startDate).getDay()];
    this.dayIndex = new Date(this.startDate).getDay();

    const startDate = new Date(this.startDate);
    const cloneStartDate = startDate;
    for (let i = 0; i < 7; i++) {
      if (startDate.getDay() === 0) {
        this.initialRoutineDate = startDate;
        break;
      } else {
        cloneStartDate.setDate(startDate.getDate() - 1);
        if (cloneStartDate.getDay() === 0) {
          this.initialRoutineDate = cloneStartDate;
          break;
        }
      }
    }

    const cloneInitialDate = new Date(this.initialRoutineDate);
    this.endRoutineDate = new Date(cloneInitialDate.setDate(startDate.getDate() + 6));
    if (!this.currentWeek.some(item => this.commonFunction.formatDateSaveWithoutTime(item) === this.commonFunction.formatDateSaveWithoutTime(this.startDate))) {
      this.currentWeek = [];
      for (const d = new Date(this.initialRoutineDate); d <= this.endRoutineDate; d.setDate(d.getDate() + 1)) {
        this.currentWeek.push(new Date(d));
      }
      this.getSchoolwideScheduleReport();
    } else {
      this.startDate = this.currentWeek[0];
      this.endDate = this.currentWeek[6];
    }
  }

  endDateChange() {
    this.currentTab = days[new Date(this.endDate).getDay()];
    this.dayIndex = new Date(this.endDate).getDay();

    const endDate = new Date(this.endDate);
    const cloneEndDate = endDate;
    for (let i = 0; i < 7; i++) {
      if (endDate.getDay() === 0) {
        this.initialRoutineDate = endDate;
        break;
      } else {
        cloneEndDate.setDate(endDate.getDate() - 1);
        if (cloneEndDate.getDay() === 0) {
          this.initialRoutineDate = cloneEndDate;
          break;
        }
      }
    }

    const cloneInitialDate = new Date(this.initialRoutineDate);
    this.endRoutineDate = new Date(cloneInitialDate.setDate(endDate.getDate() + 6));
    if (!this.currentWeek.some(item => this.commonFunction.formatDateSaveWithoutTime(item) === this.commonFunction.formatDateSaveWithoutTime(this.endDate))) {
      this.currentWeek = [];
      for (const d = new Date(this.initialRoutineDate); d <= this.endRoutineDate; d.setDate(d.getDate() + 1)) {
        this.currentWeek.push(new Date(d));
      }
      this.getSchoolwideScheduleReport();
    } else {
      this.startDate = this.currentWeek[0];
      this.endDate = this.currentWeek[6];
    }
  }

  changeTab(status: string, index: number) {
    this.currentTab = status;
    this.dayIndex = index;
  }

  // Rendering the current week
  renderCurrentWeek() {
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
    this.endRoutineDate = new Date(cloneInitialDate.setDate(today.getDate() + 6));

    for (const d = new Date(this.initialRoutineDate); d <= this.endRoutineDate; d.setDate(d.getDate() + 1)) {
      this.currentWeek.push(new Date(d));
    }
    this.getSchoolwideScheduleReport();
  }

  changeWeek(direction: string) {
    if (direction === "prev") {
      this.previousWeek();
    } else if (direction === "next") {
      this.nextWeek();
    }
  }

  previousWeek() {
    const initialDate = new Date(this.initialRoutineDate);
    let count = 0;
    for (const d = new Date(initialDate.setDate(initialDate.getDate() - 7)); d < this.initialRoutineDate; d.setDate(d.getDate() + 1)) {
      this.currentWeek[count] = new Date(d);
      count++;
    }
    this.initialRoutineDate = this.currentWeek[0];
    this.endRoutineDate = this.currentWeek[this.currentWeek.length - 1];
    this.getSchoolwideScheduleReport();
  }

  nextWeek() {
    const endDate = new Date(this.endRoutineDate);
    for (let i = 0; i < 7; i++) {
      this.currentWeek[i] = new Date(endDate.setDate(endDate.getDate() + 1));
    }
    this.initialRoutineDate = this.currentWeek[0];
    this.endRoutineDate = this.currentWeek[this.currentWeek.length - 1];
    this.getSchoolwideScheduleReport();
  }

  getSchoolwideScheduleReport() {
    this.startDate = this.currentWeek[0];
    this.endDate = this.currentWeek[6];
    this.getSchoolwideScheduleReportModel.startDate = this.commonFunction.formatDateSaveWithoutTime(this.currentWeek[0]);
    this.getSchoolwideScheduleReportModel.endDate = this.commonFunction.formatDateSaveWithoutTime(this.currentWeek[6]);

    this.reportService.getSchoolwideScheduleReport(this.getSchoolwideScheduleReportModel).subscribe(res => {
      if (res) {
        if (res._failure) {
          this.snackbar.open(res._message, '', { duration: 5000 });
          this.calendarDataViewModel = res?.calendarDataView ? res.calendarDataView : null;
          this.weekDaysList = this.calendarDataViewModel ? this.calendarDataViewModel?.days?.split('')?.map(item => +item) : [];
          this.isPreviousButtonEnabled = !this.currentWeek.some(item => this.commonFunction.formatDateSaveWithoutTime(item) === this.commonFunction.formatDateSaveWithoutTime(this.calendarDataViewModel.startDate));
          this.isNextButtonEnabled = !this.currentWeek.some(item => this.commonFunction.formatDateSaveWithoutTime(item) === this.commonFunction.formatDateSaveWithoutTime(this.calendarDataViewModel.endDate));
          this.periodList = res?.blockListForView.length ? res?.blockListForView?.filter(item => item.blockId === 1)[0]?.blockPeriod?.sort((a, b) => a.periodSortOrder - b.periodSortOrder) : [];
          this.dayWithCourseList = res.dayWithCourseList.length ? this.modifyDataSet(res) : null;
          !this.dayWithCourseList ? this.modifyHolidayList() : '';
        } else {
          this.calendarDataViewModel = res?.calendarDataView ? res.calendarDataView : null;
          this.weekDaysList = this.calendarDataViewModel ? this.calendarDataViewModel?.days?.split('')?.map(item => +item) : [];
          this.isPreviousButtonEnabled = !this.currentWeek.some(item => this.commonFunction.formatDateSaveWithoutTime(item) === this.commonFunction.formatDateSaveWithoutTime(this.calendarDataViewModel.startDate));
          this.isNextButtonEnabled = !this.currentWeek.some(item => this.commonFunction.formatDateSaveWithoutTime(item) === this.commonFunction.formatDateSaveWithoutTime(this.calendarDataViewModel.endDate));
          this.periodList = res?.blockListForView.length ? res?.blockListForView?.filter(item => item.blockId === 1)[0]?.blockPeriod?.sort((a, b) => a.periodSortOrder - b.periodSortOrder) : [];
          this.dayWithCourseList = res.dayWithCourseList.length ? this.modifyDataSet(res) : null;
          !this.dayWithCourseList ? this.modifyHolidayList() : '';
        }
      } else {
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', { duration: 5000 });
      }
    });
  }

  modifyDataSet(res) {
    this.holidayList = [];
    if (res?.dayWithCourseList?.length) {
      res?.dayWithCourseList?.map(day => {
        if (day?.courseListModel?.length) {
          day?.courseListModel?.map(course => {
            if (course?.staffListModels?.length) {
              course?.staffListModels?.map(staff => {
                staff.modiFiedcourseSectionListModels = [];
                this.periodList.map(item => staff.modiFiedcourseSectionListModels?.push({ courseSectionName: '' }));
                if (staff?.courseSectionListModels?.length) {
                  staff?.courseSectionListModels?.map(courseSection => {
                    this.periodList.map((period, index) => {
                      if (period.periodId === courseSection.periodId) {
                        staff.modiFiedcourseSectionListModels[index].courseSectionName = staff.modiFiedcourseSectionListModels[index].courseSectionName ? staff.modiFiedcourseSectionListModels[index].courseSectionName + ", " + courseSection.courseSectionName : courseSection.courseSectionName;
                      }
                    });
                  });
                }
              });
            }
          });
        }
        this.holidayList.push(day?.isHoliday);
      });
      return res?.dayWithCourseList;
    }
  }

  modifyHolidayList() {
    this.holidayList = [null, null, null, null, null, null, null];
    this.currentWeek.map((day, index) => {
      this.holidayList[index] = this.calendarDataViewModel?.calendarEvents?.length ? this.calendarDataViewModel.calendarEvents.some(event => this.commonFunction.formatDateSaveWithoutTime(event.startDate) <= this.commonFunction.formatDateSaveWithoutTime(day) && this.commonFunction.formatDateSaveWithoutTime(event.endDate) >= this.commonFunction.formatDateSaveWithoutTime(day)) : null;
    });
  }

  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }

}
