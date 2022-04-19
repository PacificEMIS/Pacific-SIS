import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatPaginator, MatPaginatorIntl } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableDataSource } from '@angular/material/table';
import { fadeInRight400ms } from '../../../../@vex/animations/fade-in-right.animation';
import { fadeInUp400ms } from '../../../../@vex/animations/fade-in-up.animation';
import { stagger40ms, stagger60ms } from '../../../../@vex/animations/stagger.animation';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs';
import icSearch from '@iconify/icons-ic/search';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { SearchFilter } from 'src/app/models/search-filter.model';
import { ScheduleStudentListViewModel } from 'src/app/models/student-schedule.model';
import { StudentListModel } from 'src/app/models/student.model';
import { CommonService } from 'src/app/services/common.service';
import { LoaderService } from 'src/app/services/loader.service';
import { StudentService } from 'src/app/services/student.service';
import { HistoricalMarkingPeriodService } from 'src/app/services/historical-marking-period.service';
import { FinalGradeService } from 'src/app/services/final-grade.service';
import { MarkingPeriodService } from 'src/app/services/marking-period.service';
import { GradesService } from 'src/app/services/grades.service';
import { GetAllStudentListForFinalGradeModel } from 'src/app/models/student-final-grade.model';
import { GetMarkingPeriodTitleListModel } from 'src/app/models/marking-period.model';
import { ResponseStudentReportCardGradesModel, StudentReportCardGradesModel } from 'src/app/models/report-card.model';
import { GradeScaleListView } from 'src/app/models/grades.model';
@Component({
  selector: 'vex-administration',
  templateUrl: './administration.component.html',
  styleUrls: ['./administration.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms,
    fadeInRight400ms,
    stagger60ms,
  ]
})
export class AdministrationComponent implements OnInit, OnDestroy,AfterViewInit {
  getAllStudent: StudentListModel = new StudentListModel();
  @ViewChild('reportPaginator', { static: false }) reportPaginator: MatPaginator;
  @ViewChild('histPaginator', { static: false }) histPaginator: MatPaginator;
  icSearch = icSearch;
  StudentModelList: MatTableDataSource<any>;
  destroySubject$: Subject<void> = new Subject();
 
  currentTab: string = 'editReportCardGrades';
  currentComponent: string = 'editReportGradesList';
  getAllStudentListForFinalGradeModel: GetAllStudentListForFinalGradeModel = new GetAllStudentListForFinalGradeModel();
  getMarkingPeriodTitleListModel: GetMarkingPeriodTitleListModel = new GetMarkingPeriodTitleListModel();
  studentReportCardGradesModel: StudentReportCardGradesModel = new StudentReportCardGradesModel();
  responseStudentReportCardGradesModel: ResponseStudentReportCardGradesModel = new ResponseStudentReportCardGradesModel();
  updateStudentReportCardGradesModel: ResponseStudentReportCardGradesModel = new ResponseStudentReportCardGradesModel();
  gradeScaleListView: GradeScaleListView = new GradeScaleListView();
  histSearchCtrl: FormControl;
  histTotalCount: number;
  histPageNumber: number;
  histPageSize: number;
  histSearchCount: number = null;
  showHistAdvanceSearchPanel: boolean = false;
  histFilterJsonParams;
  histToggleValues: any = null;
  histSearchValue: any = null;
  reportSearchCtrl: FormControl;
  reportTotalCount: number;
  reportPageNumber: number;
  reportPageSize: number;
  reportSearchCount: number = null;
  reportShowAdvanceSearchPanel: boolean = false;
  reportFilterJsonParams;
  reportSearchValue: any = null;
  reportToggleValues: any = null;
  courseSectionValue: any = null;
  gradeScaleList;
  fullAcademicYear;
  generateMarkingPeriodId: boolean = true;
  markingPeriodList = [];
  reportGradesList:MatTableDataSource<any>;
  displayedColumns: string[] = ['studentName', 'studentId', 'alternateId', 'gradeLevel', 'section', 'phone'];
  loading: boolean;
  displayedColumnsHistoricalGrades: string[] = ['studentName', 'studentId', 'alternateId', 'gradeLevel', 'section', 'phone'];
  histStudentDetails;
  studentPhoto: string;

  constructor(public translateService: TranslateService,
    private finalGradeService: FinalGradeService,
    private markingPeriodService: MarkingPeriodService,
    private gradesService: GradesService,
    private defaultValuesService: DefaultValuesService,
    private studentService: StudentService,
    private commonService: CommonService,
    private loaderService: LoaderService,
    private historicalMarkingPeriodService: HistoricalMarkingPeriodService,
    private snackbar: MatSnackBar,
    private paginatorObj: MatPaginatorIntl,
  ) {
    paginatorObj.itemsPerPageLabel = translateService.instant('itemsPerPage');
    // translateService.use('en');
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });
  }

  ngOnInit(): void {
    this.getAllStudentListForFinalGradeModel.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
    this.histSearchCtrl = new FormControl();
    this.getAllStudentListForFinalGrade();
    this.getAllMarkingPeriodList();
    this.fullAcademicYear = this.defaultValuesService.getFullAcademicYear();
    this.reportSearchCtrl = new FormControl();
   
  }

  changeTab(status) {
    this.currentTab = status;
    this.currentComponent = status == "editReportCardGrades" ? "editReportGradesList" : "historicalGradesList";
    this.generateMarkingPeriodId = true;
    if (this.currentComponent === 'editReportGradesList') {
      this.getAllStudentListForFinalGradeModel.pageNumber = 1;
      this.getAllStudentListForFinalGrade();
    }
    else{
      this.getAllStudent.pageNumber = 1;
      this.callAllStudent();
      this.ngAfterViewInit();
    }

  }

  changeComponent(step, element) {
    this.currentComponent = step;
    this.generateMarkingPeriodId = true;
    this.histStudentDetails = element;
    this.historicalMarkingPeriodService.setHistStudentDetails(element);
  }

  // For get all final graded student list 
  getAllStudentListForFinalGrade() {
    this.finalGradeService.getAllStudentListForFinalGrade(this.getAllStudentListForFinalGradeModel).subscribe(data => {
      if (data) {
        if (data._failure) {
          this.commonService.checkTokenValidOrNot(data._message);
          if (data.studentListViews === null) {
            this.reportGradesList = new MatTableDataSource([]);
            this.reportTotalCount = null;
            this.snackbar.open('' + data._message, '', {
              duration: 10000
            });
          } else {
            this.reportGradesList = new MatTableDataSource([]);
            this.reportTotalCount = null;
          }
        } else {
          this.reportTotalCount = data.totalCount;
          this.reportPageNumber = data.pageNumber;
          this.reportPageSize = data._pageSize;
          this.reportGradesList = new MatTableDataSource(data.studentListViews);
        }
      } else {
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    });
  }

  // For get all marking period list
  getAllMarkingPeriodList() {
    this.getMarkingPeriodTitleListModel.academicYear = this.defaultValuesService.getAcademicYear();
    this.markingPeriodService.getAllMarkingPeriodList(this.getMarkingPeriodTitleListModel).subscribe(data => {
      if (data._failure) {
        this.commonService.checkTokenValidOrNot(data._message);
        this.getMarkingPeriodTitleListModel.getMarkingPeriodView = [];
        if (!this.getMarkingPeriodTitleListModel?.getMarkingPeriodView) {
          this.snackbar.open(data._message, '', {
            duration: 1000
          });
        }
      } else {
        this.markingPeriodList = data.getMarkingPeriodView;
      }
    });
  }

  // For change the marking period
  ChangeMarkingPeriod(event) {
    this.studentReportCardGradesModel.markingPeriodId = event;
    this.generateMarkingPeriodId = false;
    this.getStudentReportCardGrades();
  }

  // For get all grade scale list
  getAllGradeScaleList() {
    this.gradesService.getAllGradeScaleList(this.gradeScaleListView).subscribe(data => {
      if (data._failure) {
        this.commonService.checkTokenValidOrNot(data._message);
        if (!data.gradeScaleList) {
          this.snackbar.open(data._message, '', {
            duration: 10000
          });
        }
      } else {
        this.gradeScaleList = data.gradeScaleList.filter(x => x.useAsStandardGradeScale === false);
      }
    });
  }

  // For get student report card grades
  getStudentReportCardGrades(element?, step?) {
    if (this.generateMarkingPeriodId) {
      this.studentReportCardGradesModel.studentId = element.studentId;
      this.markingPeriodList.map(item => {
        if (item.fullName === this.defaultValuesService.getMarkingPeriodTitle()) {
          this.studentReportCardGradesModel.markingPeriodId = item.value;
        }
      });
      this.finalGradeService.getStudentReportCardGrades(this.studentReportCardGradesModel).subscribe(data => {
        if (data) {
          if (data._failure) {
            this.commonService.checkTokenValidOrNot(data._message);
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
          } else {
            this.responseStudentReportCardGradesModel = data;
            this.getAllGradeScaleList();
            this.currentComponent = step;
          }
        } else {
          this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
      });
    } else {
      this.finalGradeService.getStudentReportCardGrades(this.studentReportCardGradesModel).subscribe(data => {
        if (data) {
          if (data._failure) {
            this.commonService.checkTokenValidOrNot(data._message);
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
          } else {
            this.responseStudentReportCardGradesModel = data;
            this.getAllGradeScaleList();
          }
        } else {
          this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
      });
    }
  }

  // For update student report card grades
  updateStudentReportCardGrades(res) {
    this.updateStudentReportCardGradesModel = res;
    let isValid = true;
    this.updateStudentReportCardGradesModel.courseSectionWithGradesViewModelList.map(item => {
      if (item.creditAttempted < item.creditEarned) {
        isValid = false;
      }
    });
    if (isValid) {
      this.finalGradeService.updateStudentReportCardGrades(this.updateStudentReportCardGradesModel).subscribe(data => {
        if (data) {
          if (data._failure) {
            this.commonService.checkTokenValidOrNot(data._message);
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
          } else {
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
            this.responseStudentReportCardGradesModel = data;
            this.generateMarkingPeriodId = false;
            this.getStudentReportCardGrades();
          }
        } else {
          this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
      });
    } else {
      this.snackbar.open('Credit Earned should be equal to or less than Credit Attempted', '', {
        duration: 10000
      });
    }
  }

  getReportPageEvent(event) {
      if (this.reportSearchCtrl.value != null && this.reportSearchCtrl.value != "") {
        let filterParams = [
          {
            columnName: null,
            filterValue: this.reportSearchCtrl.value,
            filterOption: 3
          }
        ]
        Object.assign(this.getAllStudentListForFinalGradeModel, { filterParams: filterParams });
      }
      this.getAllStudentListForFinalGradeModel.pageNumber = event.pageIndex + 1;
      this.getAllStudentListForFinalGradeModel.pageSize = event.pageSize;
      this.defaultValuesService.setPageSize(event.pageSize);
      this.getAllStudentListForFinalGrade();
    
  }

  getHistPageEvent(event){
    if (this.histSearchCtrl.value != null && this.histSearchCtrl.value != "") {
      let filterParams = [
        {
          columnName: null,
          filterValue: this.histSearchCtrl.value,
          filterOption: 3
        }
      ]
      Object.assign(this.getAllStudent, { filterParams: filterParams });
    }
    this.getAllStudent.pageNumber = event.pageIndex + 1;
    this.getAllStudent.pageSize = event.pageSize;
    this.defaultValuesService.setPageSize(event.pageSize);
    this.callAllStudent();
  }

  ngAfterViewInit() {
 //  Searching
    if (this.currentComponent === 'editReportGradesList') {
      this.reportSearchCtrl.valueChanges.pipe(debounceTime(500), distinctUntilChanged()).subscribe((term) => {
        if (term.trim().length > 0) {
          let filterParams = [
            {
              columnName: null,
              filterValue: term,
              filterOption: 3
            }
          ];
          Object.assign(this.getAllStudentListForFinalGradeModel, { filterParams: filterParams });
          this.getAllStudentListForFinalGradeModel.pageNumber = 1;
          this.reportPaginator.pageIndex = 0;
          this.getAllStudentListForFinalGradeModel.pageSize = this.reportPageSize;
          this.getAllStudentListForFinalGrade();
        }
        else {
          Object.assign(this.getAllStudentListForFinalGradeModel, { filterParams: null });
          this.getAllStudentListForFinalGradeModel.pageNumber = this.reportPaginator.pageIndex + 1;
          this.getAllStudentListForFinalGradeModel.pageSize = this.reportPageSize;
          this.getAllStudentListForFinalGrade();
        }
      });
    }
    else {
      this.histSearchCtrl.valueChanges.pipe(debounceTime(500), distinctUntilChanged()).subscribe((term) => {
        if (term != '') {
          let filterParams = [
            {
              columnName: null,
              filterValue: term,
              filterOption: 3
            }
          ]
          Object.assign(this.getAllStudent, { filterParams: filterParams });
          this.getAllStudent.pageNumber = 1;
          this.histPaginator.pageIndex = 0;
          this.getAllStudent.pageSize = this.histPageSize;
          this.callAllStudent();
        }
        else {
          Object.assign(this.getAllStudent, { filterParams: null });
          this.getAllStudent.pageNumber = this.histPaginator.pageIndex + 1;
          this.getAllStudent.pageSize = this.histPageSize;
          this.callAllStudent();
        }
      })
    }
  }

  getSearchResult(res) {
    if (this.currentComponent === 'editReportGradesList') {
      if (res.totalCount) {
        this.reportSearchCount = res.totalCount;
        this.reportTotalCount = res.totalCount;
      }
      else {
        this.reportSearchCount = 0;
        this.reportTotalCount = 0;
      }
      this.reportPageNumber = res.pageNumber;
      this.reportPageSize = res._pageSize;
      this.reportGradesList = new MatTableDataSource(res.studentListViews);
    }
    else {
      if (res.totalCount) {
        this.histSearchCount = res.totalCount;
        this.histTotalCount = res.totalCount;
      }
      else {
        this.histSearchCount = 0;
        this.histTotalCount = 0;
      }

      this.histPageNumber = res.pageNumber;
      this.histPageSize = res._pageSize;
      this.StudentModelList = new MatTableDataSource(res.studentListViews);
      this.getAllStudent = new StudentListModel();
    }
  }


  //Historical Student Advanced Search
  getHistToggleValues(event) {
    this.histToggleValues = event;
  }

  showHistAdvanceSearch() {
    this.showHistAdvanceSearchPanel = true;
    this.histFilterJsonParams = null;
  }

  hideHistAdvanceSearch(event) {
    this.showHistAdvanceSearchPanel = false;
  }
  getHistSearchInput(event) {
    this.histSearchValue = event;
  }

  //End Historical Student Advanced Search

  //Grade Student Advanced Search
  reportGetToggleValues(event) {
    this.reportToggleValues = event;
  }

  reportShowAdvanceSearch() {
    this.reportShowAdvanceSearchPanel = true;
    this.reportFilterJsonParams = null;
  }

  reportHideAdvanceSearch(event) {
    this.reportShowAdvanceSearchPanel = false;
  }

  reportGetSearchInput(event) {
    this.reportSearchValue = event;
  }

  courseSectionData(event) {
    this.courseSectionValue = event;
  }
// End Grade Student Advanced Search

  callAllStudent() {
    if (this.getAllStudent.sortingModel?.sortColumn == "") {
      this.getAllStudent.sortingModel = null;
    }
    this.studentService.GetAllStudentList(this.getAllStudent).subscribe(data => {
      if (data._failure) {
        this.commonService.checkTokenValidOrNot(data._message);
        if (data.studentListViews === null) {
          this.histTotalCount = null;
          this.StudentModelList = new MatTableDataSource([]);
          this.snackbar.open(data._message, '', {
            duration: 10000
          });
        } else {
          this.StudentModelList = new MatTableDataSource([]);
          this.histTotalCount = null;
        }
      } else {
        this.histTotalCount = data.totalCount;
        this.histPageNumber = data.pageNumber;
        this.histPageSize = data._pageSize;
        this.StudentModelList = new MatTableDataSource(data.studentListViews);
        this.getAllStudent = new StudentListModel();
      }
    });
  }

  itemHandler(event) {
    this.studentPhoto = event;
  }

  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }
}
