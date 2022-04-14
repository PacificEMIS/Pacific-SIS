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

import { AfterViewInit, Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatCheckbox } from '@angular/material/checkbox';
import { MatPaginator, MatPaginatorIntl } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableDataSource } from '@angular/material/table';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { StudentListModel } from 'src/app/models/student.model';
import { LoaderService } from 'src/app/services/loader.service';
import { StudentService } from 'src/app/services/student.service';
import { fadeInRight400ms } from 'src/@vex/animations/fade-in-right.animation';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { ProfilesTypes } from 'src/app/enums/profiles.enum';
import { ScheduleStudentListViewModel } from 'src/app/models/student-schedule.model';
import { StudentScheduleService } from 'src/app/services/student-schedule.service';
import { ExcelService } from 'src/app/services/excel.service';
import { GetMarkingPeriodByCourseSectionModel } from 'src/app/models/marking-period.model';
import { MarkingPeriodService } from 'src/app/services/marking-period.service';
import { GetStudentFinalGradeReportModel } from 'src/app/models/report.model';
import { ReportService } from 'src/app/services/report.service';

@Component({
  selector: 'vex-student-final-grades',
  templateUrl: './student-final-grades.component.html',
  styleUrls: ['./student-final-grades.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms,
    fadeInRight400ms
  ]
})
export class StudentFinalGradesComponent implements OnInit, AfterViewInit, OnDestroy {
  currentTab: string = 'selectStudents';
  // displayedColumns: string[] = ['studentCheck', 'studentName', 'studentId', 'alternateId', 'grade', 'phone'];
  columns = [
    { label: '', property: 'studentCheck', type: 'text', visible: true },
    { label: 'Name', property: 'studentName', type: 'text', visible: true },
    { label: 'Student ID', property: 'studentId', type: 'text', visible: true },
    { label: 'Alternate ID', property: 'alternateId', type: 'text', visible: true },
    { label: 'Grade Level', property: 'grade', type: 'text', visible: true },
    { label: 'Telephone', property: 'phone', type: 'text', visible: true },
    { label: 'Status', property: 'status', type: 'text', visible: false }
  ];
  getAllStudentModel: StudentListModel = new StudentListModel();
  scheduleStudentListViewModel: ScheduleStudentListViewModel = new ScheduleStudentListViewModel();
  getMarkingPeriodByCourseSectionModel: GetMarkingPeriodByCourseSectionModel = new GetMarkingPeriodByCourseSectionModel();
  getStudentFinalGradeReportModel: GetStudentFinalGradeReportModel = new GetStudentFinalGradeReportModel();
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  loading: boolean;
  destroySubject$: Subject<void> = new Subject();
  studentList: MatTableDataSource<any>;
  listOfStudents = [];
  selectedStudents = [];
  markingPeriodList = [];
  selectedMarkingPeriods = [];
  selectedMarkingPeriodsName = [];
  markingPeriodError: boolean;
  searchCtrl: FormControl;
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild('masterCheckBox') masterCheckBox: MatCheckbox;
  showAdvanceSearchPanel: boolean = false;
  searchCount;
  searchValue;
  toggleValues;
  isAdmin: boolean;
  profiles = ProfilesTypes;
  studentFinalGradeReport = [];

  constructor(
    public translateService: TranslateService,
    private paginatorObj: MatPaginatorIntl,
    private defaultValuesService: DefaultValuesService,
    private snackbar: MatSnackBar,
    private studentService: StudentService,
    private studentScheduleService: StudentScheduleService,
    private loaderService: LoaderService,
    private excelService: ExcelService,
    private markingPeriodService: MarkingPeriodService,
    private reportService: ReportService,
    private el: ElementRef
  ) {
    paginatorObj.itemsPerPageLabel = translateService.instant('itemsPerPage');
    this.defaultValuesService.setReportCompoentTitle.next("Student Final Grades");
    // translateService.use("en");

    (this.defaultValuesService.getUserMembershipType() === this.profiles.Teacher || this.defaultValuesService.getUserMembershipType() === this.profiles.HomeroomTeacher) ? this.isAdmin = false : this.isAdmin = true;

    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });
  }

  ngOnInit(): void {
    this.getAllStudentModel.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
    this.scheduleStudentListViewModel.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
    this.searchCtrl = new FormControl();
    this.getAllMarkingPeriodList();
    this.callAllStudent();
  }

  ngAfterViewInit(): void {
    if (this.isAdmin) {
      // For searching
      this.searchCtrl.valueChanges.pipe(debounceTime(500), distinctUntilChanged()).subscribe((term) => {
        if (term.trim().length > 0) {
          let filterParams = [
            {
              columnName: null,
              filterValue: term,
              filterOption: 3
            }
          ];
          Object.assign(this.getAllStudentModel, { filterParams: filterParams });
          this.getAllStudentModel.pageNumber = 1;
          this.paginator.pageIndex = 0;
          this.getAllStudentModel.pageSize = this.pageSize;
          this.callAllStudent();
        }
        else {
          Object.assign(this.getAllStudentModel, { filterParams: null });
          this.getAllStudentModel.pageNumber = this.paginator.pageIndex + 1;
          this.getAllStudentModel.pageSize = this.pageSize;
          this.callAllStudent();
        }
      });
    } else {
      // For searching
      this.searchCtrl.valueChanges.pipe(debounceTime(500), distinctUntilChanged()).subscribe((term) => {
        if (term.trim().length > 0) {
          let filterParams = [
            {
              columnName: null,
              filterValue: term,
              filterOption: 3
            }
          ];
          Object.assign(this.scheduleStudentListViewModel, { filterParams: filterParams });
          this.scheduleStudentListViewModel.pageNumber = 1;
          this.paginator.pageIndex = 0;
          this.scheduleStudentListViewModel.pageSize = this.pageSize;
          this.callAllStudent();
        }
        else {
          Object.assign(this.scheduleStudentListViewModel, { filterParams: null });
          this.scheduleStudentListViewModel.pageNumber = this.paginator.pageIndex + 1;
          this.scheduleStudentListViewModel.pageSize = this.pageSize;
          this.callAllStudent();
        }
      });
    }
  }

  // For get all marking period list from API
  getAllMarkingPeriodList() {
    this.getMarkingPeriodByCourseSectionModel.isReportCard = true;
    this.markingPeriodService.getMarkingPeriodsByCourseSection(this.getMarkingPeriodByCourseSectionModel).subscribe((res) => {
      if (res) {
        if (res._failure) {
          this.markingPeriodList = [];
          if (!res.getMarkingPeriodView) {
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
        } else {
          res.getMarkingPeriodView.map((item: any) => item.checked = false);
          this.markingPeriodList = res.getMarkingPeriodView;
        }
      } else {
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    });
  }

  // For get all student list from API
  callAllStudent() {
    if (this.isAdmin) {
      if (this.getAllStudentModel.sortingModel?.sortColumn === "") {
        this.getAllStudentModel.sortingModel = null;
      }
      this.studentService.GetAllStudentList(this.getAllStudentModel).subscribe(res => {
        if (res) {
          if (res._failure) {
            if (res.studentListViews === null) {
              this.totalCount = null;
              this.studentList = new MatTableDataSource([]);
              this.snackbar.open(res._message, '', {
                duration: 10000
              });
            } else {
              this.studentList = new MatTableDataSource([]);
              this.totalCount = null;
            }
          } else {
            this.totalCount = res.totalCount;
            this.pageNumber = res.pageNumber;
            this.pageSize = res._pageSize;
            res.studentListViews.forEach((student) => {
              student.checked = false;
            });
            this.listOfStudents = res.studentListViews.map((item) => {
              this.selectedStudents.map((selectedUser) => {
                if (item.studentId === selectedUser.studentId) {
                  item.checked = true;
                  return item;
                }
              });
              return item;
            });

            this.masterCheckBox.checked = this.listOfStudents.every((item) => {
              return item.checked;
            });
            this.studentList = new MatTableDataSource(res.studentListViews);
          }
        } else {
          this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
      });
    } else {
      this.scheduleStudentListViewModel.staffId = this.defaultValuesService.getUserId();
      this.scheduleStudentListViewModel.academicYear = this.defaultValuesService.getAcademicYear();
      if (this.scheduleStudentListViewModel.sortingModel?.sortColumn == "") {
        this.scheduleStudentListViewModel.sortingModel = null;
      }
      this.studentScheduleService.searchScheduledStudentForGroupDrop(this.scheduleStudentListViewModel).subscribe(data => {
        if (data) {
          if (data._failure) {
            if (data.scheduleStudentForView === null) {
              this.totalCount = null;
              this.studentList = new MatTableDataSource([]);
              this.snackbar.open(data._message, '', {
                duration: 10000
              });
            } else {
              this.studentList = new MatTableDataSource([]);
              this.totalCount = null;
            }
          } else {
            this.totalCount = data.totalCount;
            this.pageNumber = data.pageNumber;
            this.pageSize = data._pageSize;
            data.scheduleStudentForView.forEach((student) => {
              student.checked = false;
            });
            this.listOfStudents = data.scheduleStudentForView.map((item) => {
              this.selectedStudents.map((selectedUser) => {
                if (item.studentId === selectedUser.studentId) {
                  item.checked = true;
                  return item;
                }
              });
              return item;
            });

            this.masterCheckBox.checked = this.listOfStudents.every((item) => {
              return item.checked;
            });
            this.studentList = new MatTableDataSource(data.scheduleStudentForView);
          }
        } else {
          this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
      });
    }
  }

  // For server side pagination
  getPageEvent(event) {
    if (this.isAdmin) {
      if (this.searchCtrl.value != null && this.searchCtrl.value != "") {
        let filterParams = [
          {
            columnName: null,
            filterValue: this.searchCtrl.value,
            filterOption: 3
          }
        ]
        Object.assign(this.getAllStudentModel, { filterParams: filterParams });
      }
      this.getAllStudentModel.pageNumber = event.pageIndex + 1;
      this.getAllStudentModel.pageSize = event.pageSize;
      this.defaultValuesService.setPageSize(event.pageSize);
      this.callAllStudent();
    } else {
      if (this.searchCtrl.value != null && this.searchCtrl.value != "") {
        let filterParams = [
          {
            columnName: null,
            filterValue: this.searchCtrl.value,
            filterOption: 3
          }
        ]
        Object.assign(this.scheduleStudentListViewModel, { filterParams: filterParams });
      }
      this.scheduleStudentListViewModel.pageNumber = event.pageIndex + 1;
      this.scheduleStudentListViewModel.pageSize = event.pageSize;
      this.defaultValuesService.setPageSize(event.pageSize);
      this.callAllStudent();
    }
  }

  someComplete(): boolean {
    let indetermine = false;
    for (let user of this.listOfStudents) {
      for (let selectedUser of this.selectedStudents) {
        if (user.studentId === selectedUser.studentId) {
          indetermine = true;
        }
      }
    }
    if (indetermine) {
      this.masterCheckBox.checked = this.listOfStudents.every((item) => {
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
    this.listOfStudents.forEach(user => { user.checked = event; });
    this.studentList = new MatTableDataSource(this.listOfStudents);
    this.decideCheckUncheck();
  }

  onChangeSelection(eventStatus: boolean, studentId) {
    for (let item of this.listOfStudents) {
      if (item.studentId === studentId) {
        item.checked = eventStatus;
        break;
      }
    }
    this.studentList = new MatTableDataSource(this.listOfStudents);
    this.masterCheckBox.checked = this.listOfStudents.every((item) => {
      return item.checked;
    });

    this.decideCheckUncheck();
  }

  decideCheckUncheck() {
    this.listOfStudents.map((item) => {
      let isIdIncludesInSelectedList = false;
      if (item.checked) {
        for (let selectedUser of this.selectedStudents) {
          if (item.studentId === selectedUser.studentId) {
            isIdIncludesInSelectedList = true;
            break;
          }
        }
        if (!isIdIncludesInSelectedList) {
          this.selectedStudents.push(item);
        }
      }
      else {
        for (let selectedUser of this.selectedStudents) {
          if (item.studentId === selectedUser.studentId) {
            this.selectedStudents = this.selectedStudents.filter((user) => user.studentId !== item.studentId);
            break;
          }
        }
      }
      isIdIncludesInSelectedList = false;
    });
    this.selectedStudents = this.selectedStudents.filter((item) => item.checked);
  }

  getSearchResult(res) {
    this.getAllStudentModel = new StudentListModel();
    this.scheduleStudentListViewModel = new ScheduleStudentListViewModel();
    if (res?.totalCount) {
      this.searchCount = res.totalCount;
      this.totalCount = res.totalCount;
    }
    else {
      this.searchCount = 0;
      this.totalCount = 0;
    }
    this.pageNumber = res.pageNumber;
    this.pageSize = res.pageSize;
    if (this.isAdmin) {
      if (res && res.studentListViews) {
        res?.studentListViews?.forEach((student) => {
          student.checked = false;
        });
        this.listOfStudents = res.studentListViews.map((item) => {
          this.selectedStudents.map((selectedUser) => {
            if (item.studentId === selectedUser.studentId) {
              item.checked = true;
              return item;
            }
          });
          return item;
        });

        this.masterCheckBox.checked = this.listOfStudents.every((item) => {
          return item.checked;
        });
      }
      this.studentList = new MatTableDataSource(res?.studentListViews);
    } else {
      if (res && res.scheduleStudentForView) {
        res?.scheduleStudentForView?.forEach((student) => {
          student.checked = false;
        });
        this.listOfStudents = res.scheduleStudentForView.map((item) => {
          this.selectedStudents.map((selectedUser) => {
            if (item.studentId === selectedUser.studentId) {
              item.checked = true;
              return item;
            }
          });
          return item;
        });

        this.masterCheckBox.checked = this.listOfStudents.every((item) => {
          return item.checked;
        });
      }
      this.studentList = new MatTableDataSource(res?.scheduleStudentForView);
    }
  }

  getToggleValues(event) {
    this.toggleValues = event;
    if (event.inactiveStudents === true) {
      this.columns[6].visible = true;
    }
    else if (event.inactiveStudents === false) {
      this.columns[6].visible = false;
    }
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  hideAdvanceSearch(event) {
    this.showAdvanceSearchPanel = false;
  }

  getSearchInput(event) {
    this.searchValue = event;
  }

  // For export student list to excel
  exportStudentListToExcel() {
    if (!this.totalCount) {
      this.snackbar.open('No Record Found. Failed to Export Students List', '', {
        duration: 5000
      });
      return;
    }
    if (this.isAdmin) {
      const getAllStudent: StudentListModel = new StudentListModel();
      getAllStudent.pageNumber = 0;
      getAllStudent.pageSize = 0;
      getAllStudent.sortingModel = null;
      this.studentService.GetAllStudentList(getAllStudent).subscribe(res => {
        if (res._failure) {
          this.snackbar.open('Failed to Export Student List.' + res._message, '', {
            duration: 10000
          });
        } else {
          if (res.studentListViews.length > 0) {
            let studentList;
            studentList = res.studentListViews?.map((x) => {
              const middleName = x.middleName == null ? ' ' : ' ' + x.middleName + ' ';
              return {
                [this.defaultValuesService.translateKey('studentName')]: x.firstGivenName + middleName + x.lastFamilyName,
                [this.defaultValuesService.translateKey('studentId')]: x.studentInternalId,
                [this.defaultValuesService.translateKey('alternateId')]: x.alternateId,
                [this.defaultValuesService.translateKey('grade')]: x.gradeLevelTitle,
                [this.defaultValuesService.translateKey('phone')]: x.homePhone,
              };
            });
            this.excelService.exportAsExcelFile(studentList, 'Students_List_')
          } else {
            this.snackbar.open('No Records Found. Failed to Export Students List', '', {
              duration: 5000
            });
          }
        }
      });
    } else {
      const scheduleStudentListViewModel: ScheduleStudentListViewModel = new ScheduleStudentListViewModel();
      scheduleStudentListViewModel.pageNumber = 0;
      scheduleStudentListViewModel.pageSize = 0;
      scheduleStudentListViewModel.sortingModel = null;
      scheduleStudentListViewModel.staffId = this.defaultValuesService.getUserId();
      scheduleStudentListViewModel.academicYear = this.defaultValuesService.getAcademicYear();
      this.studentScheduleService.searchScheduledStudentForGroupDrop(scheduleStudentListViewModel).subscribe(res => {
        if (res._failure) {
          this.snackbar.open('Failed to Export Student List.' + res._message, '', {
            duration: 10000
          });
        } else {
          if (res.scheduleStudentForView.length > 0) {
            let studentList;
            studentList = res.scheduleStudentForView?.map((x) => {
              const middleName = x.middleName == null ? ' ' : ' ' + x.middleName + ' ';
              return {
                [this.defaultValuesService.translateKey('studentName')]: x.firstGivenName + middleName + x.lastFamilyName,
                [this.defaultValuesService.translateKey('studentId')]: x.studentInternalId,
                [this.defaultValuesService.translateKey('alternateId')]: x.alternateId,
                [this.defaultValuesService.translateKey('grade')]: x.gradeLevel,
                [this.defaultValuesService.translateKey('phone')]: x.homePhone
              };
            });
            this.excelService.exportAsExcelFile(studentList, 'Students_List_')
          } else {
            this.snackbar.open('No Records Found. Failed to Export Students List', '', {
              duration: 5000
            });
          }
        }
      });
    }
  }

  changeTab(status) {
    if (status === 'generateReport' && this.studentFinalGradeReport?.length > 0) {
      this.currentTab = status;
    } else if (status === 'selectStudents') {
      this.currentTab = status;
    }
  }

  changeValue(event, markingPeriod) {
    if (event.checked) {
      this.selectedMarkingPeriods.push(markingPeriod.value);
    } else {
      this.selectedMarkingPeriods.splice(this.selectedMarkingPeriods.findIndex(x => x === markingPeriod.value), 1);
    }
    this.markingPeriodError = this.selectedMarkingPeriods.length > 0 ? false : true;
  }

  getStudentFinalGradeReport() {
    return new Promise((resolve, reject) => {
      this.reportService.getStudentFinalGradeReport(this.getStudentFinalGradeReportModel).subscribe(res => {
        if (res) {
          if (res._failure) {
            this.studentFinalGradeReport = [];
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          } else {
            resolve(res);
          }
        } else {
          this.studentFinalGradeReport = [];
          this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
      });
    });
  }

  createGradeListForSelectedStudents() {
    if (!this.selectedMarkingPeriods.length) {
      this.markingPeriodError = true;
      const invalidMarkingPeriod: HTMLElement = this.el.nativeElement.querySelector('.markingPeriod-scroll');
      invalidMarkingPeriod.scrollIntoView({ behavior: 'smooth', block: 'center' });
      return;
    }

    if (!this.selectedStudents.length) {
      this.snackbar.open('Select at least one student.', '', {
        duration: 3000
      });
      return;
    }

    this.getStudentFinalGradeReportModel.markingPeriods = this.selectedMarkingPeriods.toString();
    this.getStudentFinalGradeReportModel.studentIds = this.selectedStudents.map(item => item.studentId);

    this.getStudentFinalGradeReport().then((res: GetStudentFinalGradeReportModel) => {
      this.selectedMarkingPeriodsName = res.studentDetailsViews[0].courseSectionDetailsViews[0].markingPeriodDetailsViews;
      this.studentFinalGradeReport = res.studentDetailsViews;
      this.currentTab = 'generateReport';
    });
  }

  // For export Final Grade Report to excel
  exportFinalGradeReportToExcel() {
    let object = {}, object1 = {}, object2 = {}, studentList = [], isFirstRow = true;
    if (this.studentFinalGradeReport.length) {
      this.studentFinalGradeReport.map((item) => {
        const middleName = item.middleName == null ? ' ' : ' ' + item.middleName + ' ';
        Object.assign(object, {
          [this.defaultValuesService.translateKey('studentName')]: item.firstGivenName + middleName + item.lastFamilyName
        });
        if (item.courseSectionDetailsViews?.length) {
          item.courseSectionDetailsViews?.map(subItem => {
            const staffMiddleName = subItem.staffMiddleName == null ? ' ' : ' ' + subItem.staffMiddleName + ' ';
            Object.assign(object1, {
              [this.defaultValuesService.translateKey('course')]: subItem.courseSectionName,
              [this.defaultValuesService.translateKey('teacher')]: subItem.staffFirstGivenName + staffMiddleName + subItem.staffLastFamilyName,
              [this.defaultValuesService.translateKey('absYtdMp')]: subItem.absYTD !== null ? subItem.absYTD : 'N/A',
            });
            if (subItem.markingPeriodDetailsViews?.length) {
              subItem.markingPeriodDetailsViews?.map(subOfSubItem => {
                if (this.getStudentFinalGradeReportModel.parcentage) {
                  const percentage = subOfSubItem.percentage !== null ? `(${subOfSubItem.percentage}%)` : "(N/A)";
                  Object.assign(object2, {
                    [subOfSubItem.markingPeriodName + " " + '(%)']: (subOfSubItem.grade !== null ? subOfSubItem.grade : 'N/A') + " " + percentage
                  });
                } else {
                  Object.assign(object2, {
                    [subOfSubItem.markingPeriodName]: subOfSubItem.grade !== null ? subOfSubItem.grade : 'N/A'
                  });
                }
              });
            }
            Object.assign(object2, {
              [this.defaultValuesService.translateKey('comments')]: subItem.comments !== null ? subItem.comments : 'N/A',
            });
            Object.assign(object1, object2);
            if (!this.getStudentFinalGradeReportModel.teacher) delete object1[this.defaultValuesService.translateKey('teacher')];
            if (!this.getStudentFinalGradeReportModel.periodByPeriodAbsences) delete object1[this.defaultValuesService.translateKey('absYtdMp')];
            if (!this.getStudentFinalGradeReportModel.comments) delete object1[this.defaultValuesService.translateKey('comments')];
            if (isFirstRow) {
              Object.assign(object, object1);
              studentList.push(JSON.parse(JSON.stringify(object)));
              isFirstRow = false;
            } else {
              studentList.push(JSON.parse(JSON.stringify(object1)));
            }
          });
        }
        isFirstRow = true;
      });
      this.excelService.exportAsExcelFile(studentList, 'Student_Final_Grades Report');
    }
  }

  // For destroy the isLoading subject.
  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }

}
