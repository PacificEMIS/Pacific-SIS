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

import { AfterViewInit, Component, OnInit, ViewChild } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { TranslateService } from "@ngx-translate/core";
import icSearch from "@iconify/icons-ic/search";
import { MatDatepicker, MatDatepickerInputEvent } from "@angular/material/datepicker";
import { StudentAttendanceService } from "../../../services/student-attendance.service";
import { attendance, CourseSectionForAttendanceViewModel, StudentAttendanceAddViewModel } from "../../../models/attendance-administrative.model";
import { CommonService } from "../../../services/common.service";
import { CourseSectionList } from "../../../models/teacher-schedule.model";
import { AttendanceCodeService } from "../../../services/attendance-code.service";
import { AttendanceCodeCategoryModel, AttendanceCodeModel, GetAllAttendanceCategoriesListModel, GetAllAttendanceCodeModel } from "../../../models/attendance-code.model";
import { StudentScheduleService } from "../../../services/student-schedule.service";
import { MatTableDataSource } from "@angular/material/table";
import { MatPaginator, MatPaginatorIntl } from "@angular/material/paginator";
import { ScheduleStudentForView, ScheduleStudentListViewModel } from "../../../models/student-schedule.model";
import { SelectionModel } from "@angular/cdk/collections";
import { MatCheckbox } from "@angular/material/checkbox";
import { MatSnackBar } from "@angular/material/snack-bar";
import { LoaderService } from "../../../services/loader.service";
import { debounceTime, distinctUntilChanged, takeUntil } from "rxjs/operators";
import { Subject } from "rxjs";
import { FormControl, NgForm } from "@angular/forms";
import { stagger40ms, stagger60ms } from "../../../../@vex/animations/stagger.animation";
import { fadeInUp400ms } from "../../../../@vex/animations/fade-in-up.animation";
import { fadeInRight400ms } from "../../../../@vex/animations/fade-in-right.animation";
import { SharedFunction } from "../../shared/shared-function";
import { StudentAttendanceModel } from "../../../models/take-attendance-list.model";
import { DefaultValuesService } from "../../../common/default-values.service";
import { scheduleType } from "../../../enums/takeAttendanceList.enum";
import { AdvancedSearchExpansionModel } from "src/app/models/common.model";


export interface StudentList {
  studentChecked: boolean;
  studentName: string;
  studentId: number;
  alternateId: string
  grade: string;
  section: string;
  phone: number;
}
@Component({
  selector: "vex-add-absences",
  templateUrl: "./add-absences.component.html",
  styleUrls: ["./add-absences.component.scss"],
  animations: [
    stagger60ms,
    fadeInUp400ms,
    stagger40ms,
    fadeInRight400ms
  ]
})
export class AddAbsencesComponent implements OnInit, AfterViewInit {
  icSearch = icSearch;
  parentData;
  @ViewChild('f') currentForm: NgForm;
  destroySubject$: Subject<void> = new Subject();
  courseSectionViewList = [];
  listOfStudent = [];
  selectedStudent = [];
  totalCount: number = 0;
  pageNumber: number;
  pageSize: number;
  loading: boolean;
  myHolidayFilter;
  disabledAdvancedSearch = false;
  searchCtrl: FormControl;
  durationStartDate: string;
  durationEndDate: string;
  attendanceCategoryModel: AttendanceCodeCategoryModel = new AttendanceCodeCategoryModel();
  getAllAttendanceCategoriesListModel: GetAllAttendanceCategoriesListModel = new GetAllAttendanceCategoriesListModel();
  attendanceCodeModel: AttendanceCodeModel = new AttendanceCodeModel();
  getAllAttendanceCodeModel: GetAllAttendanceCodeModel = new GetAllAttendanceCodeModel()
  attendanceCategories = []
  attendanceCodeList: [];
  courseSectionList: CourseSectionForAttendanceViewModel = new CourseSectionForAttendanceViewModel();
  displayedColumns: string[] = ['studentChecked', 'studentName', 'studentId', 'alternateId', 'grade', 'section', 'phone'];
  studentMasterList: ScheduleStudentForView[];
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild('masterCheckBox') masterCheckBox: MatCheckbox;
  studentDetails: MatTableDataSource<ScheduleStudentForView>;
  scheduleStudentListViewModel: ScheduleStudentListViewModel = new ScheduleStudentListViewModel();
  studentAttendanceAddViewModel: StudentAttendanceAddViewModel = new StudentAttendanceAddViewModel();
  selection: SelectionModel<ScheduleStudentForView> = new SelectionModel<ScheduleStudentForView>(true, []);
  advancedSearchExpansionModel : AdvancedSearchExpansionModel = new AdvancedSearchExpansionModel();
  courseSectionData;
  showAdvanceSearchPanel: boolean;
  filterJsonParams: any;
  searchCount: any;
  searchValue: any;
  toggleValues: any;
  attendance = [new attendance()];
  isFromAdvancedSearch: boolean = false;
  myHolidayDates = [];
  calendarDays = [];
  blockDays = [];
  disabledDates;
  weeks = [
    { name: 'Sunday', id: 0 },
    { name: 'Monday', id: 1 },
    { name: 'Tuesday', id: 2 },
    { name: 'Wednesday', id: 3 },
    { name: 'Thursday', id: 4 },
    { name: 'Friday', id: 5 },
    { name: 'Saturday', id: 6 }
  ];

  constructor(private dialog: MatDialog, public translateService: TranslateService,
    private studentAttendanceService: StudentAttendanceService,
    private commonService: CommonService,
    private attendanceCodeService: AttendanceCodeService,
    private studentScheduleService: StudentScheduleService,
    private loaderService: LoaderService,
    private commonFunction: SharedFunction,
    private defaultValueService: DefaultValuesService,
    private snackbar: MatSnackBar,
    private paginatorObj: MatPaginatorIntl) {
      this.advancedSearchExpansionModel.searchAllSchools = false;
      paginatorObj.itemsPerPageLabel = translateService.instant('itemsPerPage');
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });
  }

  public CLOSE_ON_SELECTED = false;
  public init = new Date();
  public resetModel = new Date(0);
  public model = [];
  @ViewChild("picker", { static: true }) _picker: MatDatepicker<Date>;

  public dateClass = (date: Date) => {
    if (this._findDate(date) !== -1) {
      return ["selected"];
    }
    return [];
  };

  public dateChanged(event: MatDatepickerInputEvent<Date>): void {
    this.durationStartDate;
    this.durationEndDate;
    if (event.value) {
      const date = this.commonFunction.formatDateSaveWithoutTime(event.value);
      const index = this._findDate(date);
      if (index === -1) {
        this.model.push(date);
      } else {
        this.model.splice(index, 1);
      }
      this.resetModel = new Date(0);
      if (!this.CLOSE_ON_SELECTED) {
        const closeFn = this._picker.close;
        this._picker.close = () => { };
        this._picker[
          "_popupComponentRef"
        ].instance._calendar.monthView._createWeekCells();
        setTimeout(() => {
          this._picker.close = closeFn;
        });
      }
    }
  }

  public remove(date: Date): void {
    const index = this._findDate(date);
    this.model.splice(index, 1);
  }

  private _findDate(date: Date | string): number {
    return this.model.map((m) => m).indexOf(date);
  }



  ngOnInit(): void {
    this.searchCtrl = new FormControl();
    this.courseSectionListForAttendanceAdministration();
    this.getAllAttendanceCategory();
  }


  ngAfterViewInit() {
    this.scheduleStudentListViewModel = new ScheduleStudentListViewModel();

    //  Searching
    this.searchCtrl.valueChanges.pipe(debounceTime(500), distinctUntilChanged()).subscribe((term) => {
      if (term !== '') {
        this.callWithFilterValue(term);
      } else {
        this.callWithoutFilterValue();
      }
    });
  }
  callWithFilterValue(term) {
    const searchValue: string = term.toString();
    const filterParams = [
      {
        columnName: null,
        filterValue: searchValue.trim(),
        filterOption: 1
      }
    ];

    Object.assign(this.scheduleStudentListViewModel, { filterParams });
    this.scheduleStudentListViewModel.pageNumber = 1;
    this.paginator.pageIndex = 0;
    this.scheduleStudentListViewModel.pageSize = this.pageSize;
    this.getStudentListByCourseSection(this.courseSectionData?.courseSectionId);
  }

  callWithoutFilterValue() {
    Object.assign(this.scheduleStudentListViewModel, { filterParams: null });
    this.scheduleStudentListViewModel.pageNumber = this.paginator.pageIndex + 1;
    this.scheduleStudentListViewModel.pageSize = this.pageSize;
    this.getStudentListByCourseSection(this.courseSectionData?.courseSectionId);
  }

  // selection
  someComplete(): boolean {
    let indetermine = false;
    for (let user of this.listOfStudent) {
      for (let selectedUser of this.selectedStudent) {
        if (user.StudentId == selectedUser.StudentId) {
          indetermine = true;
        }
      }
    }
    if (indetermine) {
      this.masterCheckBox.checked = this.listOfStudent.every((item) => {
        return item.checked;
      })
      if (this.masterCheckBox.checked) {
        return false;
      } else {
        return true;
      }
    }

  }

  setAll(event) {
    this.listOfStudent.forEach(user => {
      user.checked = event;
    });
    this.studentDetails = new MatTableDataSource(this.listOfStudent);
    this.decideCheckUncheck();
  }

  onChangeSelection(eventStatus: boolean, id) {
    for (let item of this.listOfStudent) {
      if (item.studentId == id) {
        item.checked = eventStatus;
        break;
      }
    }
    this.studentDetails = new MatTableDataSource(this.listOfStudent);
    this.masterCheckBox.checked = this.listOfStudent.every((item) => {
      return item.checked;
    });

    this.decideCheckUncheck();
  }

  decideCheckUncheck() {
    this.listOfStudent.map((item) => {
      let isIdIncludesInSelectedList = false;
      if (item.checked) {
        for (let selectedUser of this.selectedStudent) {
          if (item.studentId == selectedUser.studentId) {
            isIdIncludesInSelectedList = true;
            break;
          }
        }
        if (!isIdIncludesInSelectedList) {
          this.selectedStudent.push(item);
        }
      } else {
        for (let selectedUser of this.selectedStudent) {
          if (item.studentId == selectedUser.studentId) {
            this.selectedStudent = this.selectedStudent.filter((user) => user.studentId != item.studentId);
            break;
          }
        }
      }
      isIdIncludesInSelectedList = false;

    });
    this.selectedStudent = this.selectedStudent.filter((item) => item.checked);
  }

  getPageEvent(event) {
    this.scheduleStudentListViewModel.pageNumber = event.pageIndex + 1;
    this.scheduleStudentListViewModel._pageSize = event.pageSize;
    this.getStudentListByCourseSection(this.courseSectionData.courseSectionId);
  }



  // Get All Attendance Category
  getAllAttendanceCategory() {
    this.attendanceCodeService.getAllAttendanceCodeCategories(this.getAllAttendanceCategoriesListModel).subscribe((res: any) => {
      if (res._failure) {
        this.commonService.checkTokenValidOrNot(res._message);
        if (res.attendanceCodeCategoriesList === null) {
          this.attendanceCategories = [];

        } else {
          this.attendanceCategories = [];
        }
      } else {
        this.attendanceCategories = res.attendanceCodeCategoriesList;

      }
    });
  }

  // Get All Attendance Codes
  getAllAttendanceCode(catId: number) {
    this.getAllAttendanceCodeModel.attendanceCategoryId = catId;
    this.attendanceCodeService.getAllAttendanceCode(this.getAllAttendanceCodeModel).subscribe((res: any) => {
      if (res._failure) {
        this.commonService.checkTokenValidOrNot(res._message);
        if (res.attendanceCodeList === null) {
          this.attendanceCodeList = [];

        } else {
          this.attendanceCodeList = [];
        }
      } else {
        this.attendanceCodeList = res.attendanceCodeList;
      }
    });
  }

  selectCategory(event) {
    this.getAllAttendanceCode(event.value)
  }

  selectCourseSection(event) {
    this.model = [];
    this.myHolidayDates = event.holidayList.map(x => {
      return new Date(x);
    });
    this.disabledDates = event.attendanceDays;
    if (event.scheduleType === scheduleType.calendarSchedule) {
      this.calendarDays = event.attendanceDays.map(x => {
        return new Date(x);
      });
    }
    if (event.scheduleType === scheduleType.blockSchedule) {
      this.blockDays = event.attendanceDays.map(x => {
        return new Date(x);
      });
    }
    if (event.scheduleType === scheduleType.FixedSchedule || event.scheduleType === scheduleType.variableSchedule) {
      this.myHolidayFilter = (d: Date): boolean => {
        const time = d.getTime();
        const day = (d || new Date()).getDay();
        return (!this.myHolidayDates.find(x => x.getTime() == time) && this.disabledDates.includes(day));
      }
    } else if (event.scheduleType === scheduleType.calendarSchedule) {
      this.myHolidayFilter = (d: Date): boolean => {
        const time = d.getTime();
        return (this.calendarDays.find(x => x.getTime() == time) && !this.myHolidayDates.find(x => x.getTime() == time));
      }
    }
    else if (event.scheduleType === scheduleType.blockSchedule) {
      this.myHolidayFilter = (d: Date): boolean => {
        const time = d.getTime();
        return (this.blockDays.find(x => x.getTime() == time) && !this.myHolidayDates.find(x => x.getTime() == time));
      }
    }
    this.parentData = { courseSectionId: event.courseSectionId };
    this.courseSectionData = event;
    this.studentAttendanceAddViewModel.courseId = event.courseId;
    this.durationEndDate = event.durationEndDate;
    this.durationStartDate = event.durationStartDate;
    this.disabledAdvancedSearch = true;
    this.getStudentListByCourseSection(event.courseSectionId);
  }

  findMeetingDays(courseSectionList) {
    courseSectionList = courseSectionList.map((item) => {
      if (item.scheduleType === scheduleType.FixedSchedule || item.scheduleType === scheduleType.variableSchedule) {
        let days = item.meetingDays.split('|')
        days.map((day) => {
          for (let [i, weekDay] of this.weeks.entries()) {
            if (weekDay.name == day.trim()) {
              item.meetingDays = item.meetingDays + weekDay.id;
              item.attendanceDays = item.attendanceDays ? item.attendanceDays + weekDay.id.toString() : weekDay.id.toString();
              break;
            }
          }
        })
      } else if (item.scheduleType === scheduleType.calendarSchedule) {
        item.attendanceDays = [];
        item.courseCalendarSchedule.map((calendarSchedule) => {
          item.attendanceDays.push(calendarSchedule.date);
        });
      } else if (item.scheduleType === scheduleType.blockSchedule) {
        item.attendanceDays = [];
        item.bellScheduleList.map((blockSchedule) => {
          item.attendanceDays.push(blockSchedule.bellScheduleDate);
        });
      }
      return item;
    });
    return courseSectionList;
  }


  // Get All Course Sections
  courseSectionListForAttendanceAdministration() {
    this.studentAttendanceService.courseSectionListForAttendanceAdministration(this.courseSectionList).subscribe((res: CourseSectionForAttendanceViewModel) => {
      if (res._failure) {
        this.commonService.checkTokenValidOrNot(res._message);
        if (res.courseSectionViewList === null) {
          this.courseSectionViewList = [];
        } else {
          this.courseSectionViewList = [];
        }
      } else {
        this.courseSectionViewList = this.findMeetingDays(res.courseSectionViewList);
      }
    });
  }

  getStudentListByCourseSection(courseSectionId) {
    this.selection = new SelectionModel<ScheduleStudentForView>(true, []);
    this.scheduleStudentListViewModel.sortingModel = null;
    this.scheduleStudentListViewModel.courseSectionIds = [courseSectionId]
    this.studentScheduleService.searchScheduledStudentForGroupDrop(this.scheduleStudentListViewModel).subscribe(data => {
      if (data._failure) {
        this.commonService.checkTokenValidOrNot(data._message);
        this.snackbar.open('' + data._message, '', {
          duration: 10000
        });
        this.studentDetails = new MatTableDataSource([]);
        this.totalCount = this.isFromAdvancedSearch ? 0 : null;
        this.searchCount = this.isFromAdvancedSearch ? 0 : null;
        this.isFromAdvancedSearch = false;
      } else {
        this.totalCount = data.totalCount;
        this.searchCount = data.totalCount ? data.totalCount : null;
        this.pageNumber = data.pageNumber;
        this.pageSize = data._pageSize;
        this.studentMasterList = data.scheduleStudentForView;
        this.studentDetails = new MatTableDataSource(this.studentMasterList);
        this.studentMasterList.forEach(user => {
          user.checked = false
        });
        let response = this.studentMasterList.map((item) => {
          this.selectedStudent.map((selectedUser) => {
            if (item.studentId == selectedUser.studentId) {
              item.checked = true;
              return item;
            }
          });
          return item;
        });
        this.listOfStudent = response;
        this.scheduleStudentListViewModel = new ScheduleStudentListViewModel();
        this.isFromAdvancedSearch = false;
      }
    });
  }

  /* This is for get all data from the Advanced Search component and then call the API in this page 
    NOTE: We just get the filterParams Array from Search component
    */
  filterData(res) {
    this.isFromAdvancedSearch = true;
    this.scheduleStudentListViewModel = new ScheduleStudentListViewModel();
    this.scheduleStudentListViewModel.pageSize = this.defaultValueService.getPageSize() ? this.defaultValueService.getPageSize() : 10;
    if (res) {
      this.scheduleStudentListViewModel.filterParams = res.filterParams;
      this.scheduleStudentListViewModel.includeInactive = res.inactiveStudents;
      this.scheduleStudentListViewModel.dobStartDate = this.commonFunction.formatDateSaveWithoutTime(res.dobStartDate);
      this.scheduleStudentListViewModel.dobEndDate = this.commonFunction.formatDateSaveWithoutTime(res.dobEndDate);
      this.getStudentListByCourseSection(res.courseSectionId);
    }
  }

  getSearchResult(res) {
    if (res.totalCount) {
      this.searchCount = res.totalCount;
      this.totalCount = res.totalCount;
    }
    else {
      this.searchCount = 0;
      this.totalCount = 0;
    }
    this.scheduleStudentListViewModel.scheduleStudentForView = res.scheduleStudentForView;
    this.pageNumber = res.pageNumber;
    this.pageSize = res._pageSize;
    this.studentDetails = new MatTableDataSource(res.scheduleStudentForView);
  }
  getToggleValues(event) {
    this.toggleValues = event;
    if (event.inactiveStudents === true) {
      //this.columns[6].visible = true;
    }
    else if (event.inactiveStudents === false) {
      //this.columns[6].visible = false;
    }
  }
  getSearchInput(event) {
    this.searchValue = event;
  }
  showAdvanceSearch() {
    this.showAdvanceSearchPanel = true;
    this.filterJsonParams = null;
  }

  hideAdvanceSearch(event) {
    this.showAdvanceSearchPanel = false;
  }

  addAbsence() {
    this.currentForm.form.markAllAsTouched();
    if (this.currentForm.form.valid) {
      if (this.model.length < 1) {
        this.snackbar.open(this.defaultValueService.translateKey('absenceDateIsRequired'), '', {
          duration: 10000
        });
      }
      else {
        if (this.selectedStudent.length > 0) {
          this.model.map((m) => {
            this.selectedStudent.map((item) => {
              this.attendance.push({
                studentId: item.studentId,
                attendanceDate: m
              })
            })
          })
          this.attendance.splice(0, 1);
          this.studentAttendanceAddViewModel.studentAttendance = this.attendance;
          this.studentAttendanceService.addAbsences(this.studentAttendanceAddViewModel).subscribe((res: StudentAttendanceAddViewModel) => {
            if (res._failure) {
              this.commonService.checkTokenValidOrNot(res._message);
              this.snackbar.open(res._message, '', {
                duration: 10000
              });
            } else {
              this.snackbar.open(res._message, '', {
                duration: 10000
              });
              this.studentAttendanceAddViewModel = res;
            }
          });

        }
        else {
          this.snackbar.open('Please select at least 1 student', '', {
            duration: 10000
          });
        }
      }


    }

  }




}
