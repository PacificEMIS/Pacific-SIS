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

import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit, TemplateRef, ViewChild, ViewEncapsulation } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { CalendarEvent, CalendarEventAction, CalendarEventTimesChangedEvent, CalendarMonthViewBeforeRenderEvent, CalendarMonthViewDay, CalendarView, CalendarWeekViewBeforeRenderEvent, DAYS_OF_WEEK } from 'angular-calendar';
import { addDays, addHours, endOfDay, endOfMonth, endOfWeek, format, isSameDay, isSameMonth, startOfDay, startOfMonth, startOfWeek, subDays } from 'date-fns';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CalendarEditComponent } from './calendar-edit/calendar-edit.component';
import { AddCalendarComponent } from './add-calendar/add-calendar.component';
import { AddEventComponent } from './add-event/add-event.component';
import icAdd from '@iconify/icons-ic/add';
import icEdit from '@iconify/icons-ic/edit';
import icDelete from '@iconify/icons-ic/delete';
import icWarning from '@iconify/icons-ic/warning';
import icChevronLeft from '@iconify/icons-ic/twotone-chevron-left';
import icChevronRight from '@iconify/icons-ic/twotone-chevron-right';
import { FormControl } from '@angular/forms';
import { CalendarService } from '../../../services/calendar.service';
import { CalendarAddViewModel, CalendarBellScheduleModel, CalendarBellScheduleViewModel, CalendarListModel, CalendarModel } from '../../../models/calendar.model';
import { GetAllMembersList } from '../../../models/membership.model';
import { MembershipService } from '../../../services/membership.service';
import { CalendarEventService } from '../../../services/calendar-event.service';
import { CalendarEventAddViewModel, CalendarEventListViewModel, CalendarEventModel } from '../../../models/calendar-event.model';
import { HttpClient, HttpParams } from '@angular/common/http';
import { find, map } from 'rxjs/operators';
import { TranslateService } from '@ngx-translate/core';
import { ConfirmDialogComponent } from '../../shared-module/confirm-dialog/confirm-dialog.component';
import * as moment from 'moment';
import { LoaderService } from '../../../services/loader.service';
import { Permissions, RolePermissionListViewModel, RolePermissionViewModel } from '../../../models/roll-based-access.model';
import { RollBasedAccessService } from '../../../services/roll-based-access.service';
import { CryptoService } from '../../../services/Crypto.service';
import { DefaultValuesService } from '../../../common/default-values.service';
import { SharedFunction } from '../../shared/shared-function';
import { PageRolesPermission } from '../../../common/page-roles-permissions.service';
import { SchoolPeriodService } from 'src/app/services/school-period.service';
import { BlockListViewModel } from 'src/app/models/school-period.model';
import { CourseManagerService } from 'src/app/services/course-manager.service';
import { days } from "../../../common/static-data";
import { CommonService } from 'src/app/services/common.service';

const colors: any = {
  blue: {
    primary: '#5c77ff',
    secondary: '#FFFFFF'
  },
  yellow: {
    primary: '#ffc107',
    secondary: '#FDF1BA'
  },
  red: {
    primary: '#f44336',
    secondary: '#FFFFFF'
  }
};

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'vex-calendar',
  templateUrl: './calendar.component.html',
  styleUrls: ['./calendar.component.scss'],
  encapsulation: ViewEncapsulation.None
})

export class CalendarComponent implements OnInit {
  @ViewChild('modalContent', { static: true }) modalContent: TemplateRef<any>;
  isMarkingPeriod: string;
  getCalendarList: CalendarListModel = new CalendarListModel();
  getAllMembersList: GetAllMembersList = new GetAllMembersList();
  getAllCalendarEventList: CalendarEventListViewModel = new CalendarEventListViewModel();
  calendarAddViewModel = new CalendarAddViewModel();
  calendarEventAddViewModel = new CalendarEventAddViewModel();
  showCalendarView: boolean;
  view: CalendarView = CalendarView.Month;
  calendars: CalendarModel[];
  activeDayIsOpen = true;
  weekendDays: number[];
  filterDays = [];
  CalendarView = CalendarView;
  viewDate: Date = new Date();
  selectedCalendar = new CalendarModel();
  icChevronLeft = icChevronLeft;
  icChevronRight = icChevronRight;
  icAdd = icAdd;
  icEdit = icEdit;
  icDelete = icDelete;
  icWarning = icWarning;
  events$: Observable<CalendarEvent<{ calendar: CalendarEventModel }>[]>;
  refresh: Subject<any> = new Subject();
  calendarFrom: FormControl;
  cssClass: string;
  loading: boolean;
  permissions: Permissions;
  blockListViewModel: BlockListViewModel = new BlockListViewModel();
  calendarBellScheduleModel: CalendarBellScheduleModel = new CalendarBellScheduleModel();
  CalendarBellScheduleViewModel: CalendarBellScheduleViewModel = new CalendarBellScheduleViewModel();
  bellScheduleList = [];
  periodList = [];
  valueList = [1, '46', 3, 4, 5]
  weekHeader;
  valueSetCount: number;
  userType : string
  calendarStartDate: string;
  calendarEndDate: string;
  // currentdate: any;
  // oneJan: any;
    // selectedBlockTitle : string
  displayedColumns: string[]
  eventList = [];
  allData = [];
  maxEndDateForSessionCalendar: Date;
  minSchoolYearStartDate: Date;
  maxSchoolYearEndDate: Date;
  isSessionCalender: boolean;
  constructor(
    private http: HttpClient,
    private dialog: MatDialog,
    private snackbar: MatSnackBar,
    public translate: TranslateService,
    private membershipService: MembershipService,
    private calendarEventService: CalendarEventService,
    private calendarService: CalendarService,
    public rollBasedAccessService: RollBasedAccessService,
    private loaderService: LoaderService,
    private cdr: ChangeDetectorRef,
    private pageRolePermissions: PageRolesPermission,
    private cryptoService: CryptoService,
    private commonFunction: SharedFunction,
    public defaultValuesService: DefaultValuesService,
    private schoolPeriodService: SchoolPeriodService,
    private courseManagerService: CourseManagerService,
    private commonService: CommonService,
  ) {
    this.valueSetCount = 0;
    this.weekHeader = [];
    this.translate.setDefaultLang('en');    
    
    this.loaderService.isLoading.subscribe((res) => {
      this.loading = res;
    });
    this.calendarEventService.currentEvent.subscribe(
      res => {
        if (res) {
          this.getAllCalendarEvent();
        }
      }
    )
    // getting membershipType from session storage
    this.userType = this.defaultValuesService.getUserMembershipType()
    this.eventList = [];
    this.displayedColumns = ['title', 'type', 'startDate', 'endDate', 'notes'];
  }

  ngOnInit(): void {
    this.permissions = this.pageRolePermissions.checkPageRolePermission();
    this.isMarkingPeriod = this.defaultValuesService.getMarkingPeriodId();
    // if (JSON.parse(this.isMarkingPeriod)) {
      this.getAllBellSchedule().then(() => {
        this.getAllCalendar();
        this.getAllMemberList();
        this.getAllPeriodList();
      });

    // }
  }

  changeCalendar(event) {
    if (event.sessionCalendar === true){
      this.isSessionCalender = true;
    } else {
      this.isSessionCalender = false;
    }
    this.calendarStartDate = event.startDate;
    this.calendarEndDate = event.endDate;
    this.getDays(event.days);
    this.calendarService.setCalendarId(event.calenderId);
    this.getAllCalendarEvent();
  }

  ngAfterViewChecked() {
    this.cdr.detectChanges();
  }

  getAllPeriodList() {
    this.schoolPeriodService.getAllBlockList(this.blockListViewModel).subscribe(data => {
      if (data._failure) {
        this.commonService.checkTokenValidOrNot(data._message);
        this.periodList = [];
        if (!data.getBlockListForView) {
          this.snackbar.open(data._message, '', {
            duration: 10000
          });
        }
      } else {
        this.blockListViewModel = data;
        if (data.getBlockListForView.length > 0) {
          this.periodList = data.getBlockListForView;
        }

      }
    });
  }

  changeBlockPeriod(event, day) {
    this.calendarBellScheduleModel.bellSchedule.blockId = event.value !== '' ? event.value : null;
    this.calendarBellScheduleModel.bellSchedule.bellScheduleDate = this.commonFunction.formatDateSaveWithoutTime(day.date);
    this.courseManagerService.addEditBellSchedule(this.calendarBellScheduleModel).subscribe((res) => {
      if (res._failure) {
        this.commonService.checkTokenValidOrNot(res._message);
        this.snackbar.open(res._message, '', {
          duration: 1000
        });
      } else {
        day.blockId = event.value;
        this.snackbar.open(res._message, '', {
          duration: 1000
        });
        this.getAllBellSchedule();
      }

    });

  }

  getAllBellSchedule() {
    return new Promise((resolve, reject) => {
      this.courseManagerService.getAllBellSchedule(this.CalendarBellScheduleViewModel).subscribe((res: any) => {
        if (res._failure) {
          this.commonService.checkTokenValidOrNot(res._message);
          resolve('');

        } else {
          this.bellScheduleList = res.bellScheduleList;
          resolve('');
        }
      });
    })
  }

  //Show all members
  getAllMemberList() {
    this.membershipService.getAllMembers(this.getAllMembersList).subscribe(
      (res) => {
        if (typeof (res) == 'undefined') {
          this.snackbar.open('No Member Found. ' + this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
        else {
          if (res._failure) {
            this.commonService.checkTokenValidOrNot(res._message);
            if (!res.getAllMemberList) {
              this.snackbar.open('No Member Found. ' + res._message, '', {
                duration: 10000
              });
            }
          }
          else {
            this.getAllMembersList = res;
          }
        }
      });
  }

  getHoliDay(event) {
    let events = [];
    events = event;
    if (events.filter(x => x?.meta?.calendar?.isHoliday).length > 0) {
      return true;
    } else {
      return false;
    }
  }

  checkEvent(events) {
    if(events.filter(y => y?.meta?.calendar?.isHoliday).length > 0) {
      return false;
    } else {
      return true;
    }
  }

  checkDate(date) {
    if (moment(date).isBetween(this.minSchoolYearStartDate, this.maxSchoolYearEndDate)) {
      return true;
    } else {
      return false;
    }
  }

  getEventColor(event){
    let events = [];
    events = event;
    return events.filter(x => x?.meta?.calendar?.isHoliday)[0].meta.calendar.eventColor;
  }

  //Show all calendar
  getAllCalendar() {
    this.calendarService.getAllCalendar(this.getCalendarList).subscribe((data:any) => {
      if (data._failure) {
        this.commonService.checkTokenValidOrNot(data._message);
      }
      this.calendars = data.calendarList;
      this.maxEndDateForSessionCalendar = data.maxEndDateForSessionCalendar;
      this.minSchoolYearStartDate = data.minSchoolYearStartDate;
      this.maxSchoolYearEndDate = data.maxSchoolYearEndDate;
      this.showCalendarView = false;
      if (this.calendars.length !== 0) {
        this.showCalendarView = true;
        const defaultCalender = this.calendars.find(element => element.defaultCalender === true);
        if (defaultCalender != null) {
          this.selectedCalendar = defaultCalender;
          if (this.selectedCalendar.sessionCalendar === true){
            this.isSessionCalender = true;
          } else {
            this.isSessionCalender = false;
          }
          this.calendarStartDate = this.selectedCalendar.startDate;
          this.calendarEndDate = this.selectedCalendar.endDate;
          this.calendarService.setCalendarId(this.selectedCalendar.calenderId);
          this.getDays(this.selectedCalendar.days);
          this.getAllCalendarEvent();
          this.getAllBellSchedule();
        }
        this.refresh.next();
      }

    });

  }

  // Rendar all events in calendar
  getAllCalendarEvent() {
    this.eventList = [];
    this.getAllCalendarEventList.calendarId = [];
    this.getAllCalendarEventList.calendarId.push(this.calendarService.getCalendarId());
    this.events$ = this.calendarEventService.getAllCalendarEvent(this.getAllCalendarEventList).pipe(
      map(({ calendarEventList }: { calendarEventList: CalendarEventModel[] }) => {
        this.eventList = calendarEventList;
        return calendarEventList.map((calendar: CalendarEventModel) => {
          return {
            id: calendar.eventId,
            title: calendar.title,
            start: new Date(calendar.startDate),
            end: new Date(calendar.endDate),
            allDay: true,
            type: 'calendar',
            meta: {
              calendar,
            },
            draggable: true
          };
        });
      })
    );
    this.events$.subscribe(data => {
      this.allData = [...data];
    });
    this.refresh.next();
  }

  getDays(days: string) {
    const calendarDays = days;
    let allDays = [0, 1, 2, 3, 4, 5, 6];
    let splitDays = calendarDays.split('').map(x => +x);
    this.filterDays = allDays.filter(f => !splitDays.includes(f));
    this.weekendDays = this.filterDays;
    this.cssClass = 'bg-aqua';
    this.refresh.next();
  }

  //for rendar weekends
  beforeMonthViewRender(renderEvent: CalendarMonthViewBeforeRenderEvent): void {
    renderEvent.body.forEach((day: any) => {
      const dayOfMonth = day.date.getDay();
      if (this.filterDays.includes(dayOfMonth)) {
        day.cssClass = this.cssClass;
      }
      if (this.bellScheduleList.length) {
        this.bellScheduleList.map((item) => {
          if (this.commonFunction.formatDateSaveWithoutTime(item.bellScheduleDate) === this.commonFunction.formatDateSaveWithoutTime(day.date)) {
            day.blockId = item.blockId;
            this.findDayBlockTitle(day, item.blockId)
          } else {
            if (!day.blockId) {
              day.blockId = "";
              day.blockTitle = "All Day"
            }
          }
        });
      }
    });
  }

  beforeWeekViewRender(renderEvent: CalendarWeekViewBeforeRenderEvent) {
    this.weekHeader = renderEvent.header;
    this.weekHeader.map((day: any) => {
      if (this.bellScheduleList.length) {
        this.bellScheduleList.map((item) => {
          if (this.commonFunction.formatDateSaveWithoutTime(item.bellScheduleDate) === this.commonFunction.formatDateSaveWithoutTime(day.date)) {
            day.blockId = item.blockId;
            this.findWeekBlockTitle(day, item.blockId)
          } else {
            if (!day.blockId) {
              day.blockId = "";
              day.blockTitle = "All Day"
            }
          }
        });
      }
    })
  }

  findDayBlockTitle(day: any, blockId : number) {
    this.periodList.forEach(item=>{
      if (blockId == item.blockId) {
        day.blockTitle = item.blockTitle
      }
    }) 
  }
  
  findWeekBlockTitle(day: any, blockId : number) {
    this.periodList.forEach(item=>{
      if (blockId == item.blockId) {
        day.blockTitle = item.blockTitle
      }
    }) 
  }

  // open event modal for view
  viewEvent(eventData) {    
    this.dialog.open(AddEventComponent, {
      data: { allMembers: this.getAllMembersList, membercount: this.getAllMembersList.getAllMemberList.length, calendarEvent: eventData },
      width: '600px'
    }).afterClosed().subscribe(data => {
      if (data === 'submitedEvent') {
        this.getAllCalendarEvent();
      }
    });

  }
  private formatDate(date: Date): string {
    if (date === undefined || date === null) {
      return undefined;
    } else {
      return moment(date).format('YYYY-MM-DDTHH:mm:ss.SSS');
    }
  }

  //drag and drop event
  eventTimesChanged({ event, newStart, newEnd, }: CalendarEventTimesChangedEvent): void {
    event.start = newStart;
    event.end = newEnd;
    this.calendarEventAddViewModel.schoolCalendarEvent = event.meta.calendar;
    this.calendarEventAddViewModel.schoolCalendarEvent.startDate = this.commonFunction.formatDateSaveWithoutTime(newStart);
    this.calendarEventAddViewModel.schoolCalendarEvent.endDate = this.commonFunction.formatDateSaveWithoutTime(newEnd);
    delete this.calendarEventAddViewModel.schoolCalendarEvent.academicYear;
    this.calendarEventService.updateCalendarEvent(this.calendarEventAddViewModel).subscribe(data => {
      if (data._failure) {
        this.commonService.checkTokenValidOrNot(data._message);
        this.snackbar.open(data._message, '', {
          duration: 10000
        });
      }
      else {
        this.getAllCalendarEvent();
      }
    });
    this.refresh.next();
  }

  //Open modal for add new calendar
  openAddNewCalendar() {
    this.dialog.open(AddCalendarComponent, {
      data: { allMembers: this.getAllMembersList, membercount: this.getAllMembersList?.getAllMemberList?.length, calendarListCount: this.calendars?.length, sessionCalendar:false},
      width: '600px'
    }).afterClosed().subscribe(data => {
      if (data === 'submited') {
        this.getAllBellSchedule().then(() => {
          this.getAllCalendar();
        });
      }
    });
  }

  // Open calendar confirm modal
  deleteCalendarConfirm(event) {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: "400px",
      data: {
        title: this.defaultValuesService.translateKey('areYouSure'),
        message: this.defaultValuesService.translateKey('youAreAboutToDelete') + event.title
      }
    });
    // listen to response
    dialogRef.afterClosed().subscribe(dialogResult => {
      // if user pressed yes dialogResult will be true, 
      // if user pressed no - it will be false
      if (dialogResult) {
        this.deleteCalendar(event.calenderId);
      }
    });
  }

  deleteCalendar(id: number) {
    this.calendarAddViewModel.schoolCalendar.schoolId = this.defaultValuesService.getSchoolID();
    this.calendarAddViewModel.schoolCalendar.tenantId = this.defaultValuesService.getTenantID()
    this.calendarAddViewModel.schoolCalendar.calenderId = id;
    this.calendarService.deleteCalendar(this.calendarAddViewModel).subscribe(
      (res) => {
        if (res._failure) {
          this.commonService.checkTokenValidOrNot(res._message);
          this.snackbar.open(res._message, '', {
            duration: 10000
          });
        } else {
          this.snackbar.open(res._message, '', {
            duration: 10000
          });
          this.getAllCalendar();
        }
      });

  };

  // Edit calendar which is selected in dropdown
  openEditCalendar(event) {
    this.dialog.open(AddCalendarComponent, {
      data: { allMembers: this.getAllMembersList, membercount: this.getAllMembersList.getAllMemberList.length, calendar: event, sessionCalendar: event.sessionCalendar, maxEndDateForSessionCalendar: this.maxEndDateForSessionCalendar},
      width: '600px'
    }).afterClosed().subscribe(data => {
      if (data === 'submited') {
        this.getAllCalendar();
      }
    });
  }

  // Open add new event by clicking calendar day
  openAddNewEvent(event) {
    if (this.permissions?.add && this.defaultValuesService.checkAcademicYear()) {
      if (!moment(event.date).isBetween(this.calendarStartDate, this.calendarEndDate, undefined, '[]')) return;
      if (event.inMonth) {
        this.dialog.open(AddEventComponent, {
          data: { allMembers: this.getAllMembersList, membercount: this.getAllMembersList.getAllMemberList.length, day: event },
          width: '600px'
        }).afterClosed().subscribe(data => {
          if (data === 'submitedEvent') {
            this.getAllCalendarEvent();
          }
        });
      }
      else {
        if (event.isWeekend) {
          this.snackbar.open(this.defaultValuesService.translateKey('cannotAddEventInWeekend'), '', {
            duration: 2000
          });
        }
        if (!event.isWeekend) {
          this.snackbar.open(this.defaultValuesService.translateKey('cannotAddEventInPreviousMonth'), '', {
            duration: 2000
          });
        }
      }
    }
    else {
      this.snackbar.open(this.defaultValuesService.translateKey('HaveNotAnyPermissionToAdd'), '', {
        duration: 2000
      });
    }

  }

  // getWeekNumber(viewDate) {
  //   console.log(viewDate);
  //   this.currentdate = new Date(viewDate);
  //   this.oneJan = new Date(this.currentdate.getFullYear(), 0, 1);
  //   let numberOfDays = Math.floor((this.currentdate - this.oneJan) / (24 * 60 * 60 * 1000));
  //   let result = Math.ceil((this.currentdate.getDay() + 1 + numberOfDays) / 7);
  //   console.log(result);
  //   return result;
  // }

  isBeforeCalendar(viewDate) {
    let calendarStartDate = this.commonFunction.formatDateSaveWithoutTime(this.calendarStartDate);
    let currentViewDate = this.commonFunction.formatDateSaveWithoutTime(viewDate);
    let calStartMonth = new Date(this.calendarStartDate).getMonth() + 1;
    let calStartYear = new Date(this.calendarStartDate).getFullYear();
    let currentViewMonth = new Date(viewDate).getMonth() + 1;
    let currentViewYear = new Date(viewDate).getFullYear();

    if (this.view === CalendarView.Month) {
      return currentViewMonth <= calStartMonth && currentViewYear <= calStartYear ? true : false;
    } else if (this.view === CalendarView.Week) {

    } else if (this.view === CalendarView.Day) {
      return currentViewDate <= calendarStartDate ? true : false;
    }
  }

  isAfterCalendar(viewDate) {
    let calendarEndDate = this.commonFunction.formatDateSaveWithoutTime(this.calendarEndDate);
    let currentViewDate = this.commonFunction.formatDateSaveWithoutTime(viewDate);
    let calEndMonth = new Date(this.calendarEndDate).getMonth() + 1;
    let calEndYear = new Date(this.calendarEndDate).getFullYear();
    let currentViewMonth = new Date(viewDate).getMonth() + 1;
    let currentViewYear = new Date(viewDate).getFullYear();

    if (this.view === CalendarView.Month) {
      return currentViewMonth >= calEndMonth && currentViewYear >= calEndYear ? true : false;
    } else if (this.view === CalendarView.Week) {

    } else if (this.view === CalendarView.Day) {
      return currentViewDate >= calendarEndDate ? true : false;
    }
  }

  isBetweenCalendar(date) {
    return moment(date).isBetween(this.calendarStartDate, this.calendarEndDate, undefined, '[]');
  }

}
