import { Component, OnInit, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatPaginator, MatPaginatorIntl } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableDataSource } from '@angular/material/table';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { GetStudentAbsenceReport, StudentListForAbsenceSummary } from 'src/app/models/absence-summary.model';
import { GetAllAttendanceCodeModel, GetStudentAttendanceReport } from 'src/app/models/attendance-code.model';
import { GetAllGradeLevelsModel } from 'src/app/models/grade-level.model';
import { MarkingPeriodListModel } from 'src/app/models/marking-period.model';
import { LoaderService } from 'src/app/services/loader.service';
import { MarkingPeriodService } from 'src/app/services/marking-period.service';
import moment from 'moment';
import { SharedFunction } from 'src/app/pages/shared/shared-function';
import { CommonService } from 'src/app/services/common.service';
import { SchoolPeriodService } from 'src/app/services/school-period.service';
import { BlockListViewModel } from 'src/app/models/school-period.model';
import { ReportService } from 'src/app/services/report.service';
import { ExcelService } from 'src/app/services/excel.service';


@Component({
  selector: 'vex-absence-summary',
  templateUrl: './absence-summary.component.html',
  styleUrls: ['./absence-summary.component.scss']
})
export class AbsenceSummaryComponent implements OnInit {
  getStudentAbsenceReportModel: GetStudentAbsenceReport = new GetStudentAbsenceReport();
  displayedColumns: string[] = ['studentName', 'studentId', 'alternateId', 'grade', 'phone', 'absent', 'halfDay'];
  studentLists = [];
  parentData :any ={
    markingPeriodStartDate: null,
    markingPeriodEndDate: null,
    periodId: null
  };
  blockListViewModel: BlockListViewModel = new BlockListViewModel();
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  searchCtrl: FormControl;
  totalCount: number = 0;
  pageSize: number;
  allAttendence: MatTableDataSource<any>;
  pageNumber: number;
  studentListForAbsenceSummary: StudentListForAbsenceSummary = new StudentListForAbsenceSummary();
  getAllAttendanceCodeModel: GetAllAttendanceCodeModel = new GetAllAttendanceCodeModel();
  isVisible: boolean = false;
  destroySubject$: Subject<void> = new Subject();
  loading: boolean;
  selectedReportBy: any;
  globalMarkingPeriodEndDate;
  globalMarkingPeriodStartDate;
  filterJsonParams;
  showAdvanceSearchPanel: boolean = false;
  allStudentlist: MatTableDataSource<any> = new MatTableDataSource<any>();
  searchValue: any;
  toggleValues: any;
  searchCount: number;
  disabledAdvancedSearch: boolean = false;
  selectOptions: any = [
    {
      title: 'This School Year',
      subTitle: 'this_school_year'
    },
    {
      title: 'This Month',
      subTitle: 'this_school_month'
    },
  ];
  tempGradeLevel = '';
  gradeLevelList = [];
  periodList = []
  markingPeriodListModel: MarkingPeriodListModel = new MarkingPeriodListModel();
  constructor(
    public translateService: TranslateService,
    private router: Router,
    private commonFunction: SharedFunction,
    private commonService: CommonService,
    private paginatorObj: MatPaginatorIntl,
    private reportService: ReportService,
    private excelService: ExcelService,
    private schoolPeriodService: SchoolPeriodService,
    private snackbar: MatSnackBar,
    public defaultValuesService: DefaultValuesService,
    private loaderService: LoaderService,
    private markingPeriodService: MarkingPeriodService,
  ) {
    paginatorObj.itemsPerPageLabel = translateService.instant('itemsPerPage');
    this.defaultValuesService.setReportCompoentTitle.next("Absence Summary");
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });
    this.selectOptions[0].startDate = this.defaultValuesService.getFullYearStartDate();
    this.selectOptions[0].endDate = this.defaultValuesService.getFullYearEndDate();

    this.selectOptions[1].startDate = moment().startOf('month').format('YYYY-MM-DD');
    this.selectOptions[1].endDate = moment().endOf('month').format('YYYY-MM-DD');
    this.selectedReportBy = this.defaultValuesService.getMarkingPeriodTitle();
    this.getMarkingPeriod();
    this.getDropDownData();
  }
  ngOnInit(): void {
    this.getStudentAbsenceReportModel.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
    this.searchCtrl = new FormControl();
  }
  ngAfterViewInit() {
    this.searchCtrl.valueChanges.pipe(debounceTime(500), distinctUntilChanged()).subscribe((term) => {
      if (term != '') {
        this.callWithFilterValue(term);
      } else {
        this.callWithoutFilterValue()
      }
    });
  }

  // For get all marking period
 getMarkingPeriod() {
  this.markingPeriodService.GetMarkingPeriod(this.markingPeriodListModel).subscribe((data: any) => {
    if (data._failure) {
      this.commonService.checkTokenValidOrNot(data._message);
    } else {
      for (let i = 0; i < data.schoolYearsView.length; i++) {
        this.selectOptions.push({
          title: data.schoolYearsView[i]?.title,
          startDate: data.schoolYearsView[i]?.startDate,
          endDate: data.schoolYearsView[i]?.endDate,
          subTitle: data.schoolYearsView[i]?.title
        });
        if (data.schoolYearsView[i].children.length > 0) {
          data.schoolYearsView[i].children.map((item: any) => {
            this.selectOptions.push({
              title: item?.title,
              startDate: item?.startDate,
              endDate: item?.endDate,
              subTitle: item?.title
            });
            if (item.children.length > 0) {
              item.children.map((subItem: any) => {
                this.selectOptions.push({
                  title: subItem?.title,
                  startDate: subItem?.startDate,
                  endDate: subItem?.endDate,
                  subTitle: subItem?.title
                });
                if (subItem.children.length > 0) {
                  subItem.children.map((subOfSubItem: any) => {
                    this.selectOptions.push({
                      title: subOfSubItem?.title,
                      startDate: subOfSubItem?.startDate,
                      endDate: subOfSubItem?.endDate,
                      subTitle: subOfSubItem?.title
                    });
                  });
                }
              });
            }
          });
        }
      }
    }
  });
}

getReportBy(event) {
  if (event.value) {
    const selectedOption = this.selectOptions.filter(x => x.subTitle === event.value);
    this.getStudentAbsenceReportModel.markingPeriodStartDate = this.commonFunction.formatDateSaveWithoutTime(selectedOption[0].startDate);
    this.getStudentAbsenceReportModel.markingPeriodEndDate = this.commonFunction.formatDateSaveWithoutTime(selectedOption[0].endDate);
  } else {
    this.getStudentAbsenceReportModel.markingPeriodStartDate = null;
    this.getStudentAbsenceReportModel.markingPeriodEndDate = null;
  }
}

  callWithoutFilterValue() {
    Object.assign(this.getStudentAbsenceReportModel, { filterParams: null });
    this.getStudentAbsenceReportModel.pageNumber = this.paginator.pageIndex + 1;
    this.getStudentAbsenceReportModel.pageSize = this.pageSize;
    this.getStudentAbsenceReport();
  }

  callWithFilterValue(term) {
    let filterParams = [
      {
        columnName: null,
        filterValue: term,
        filterOption: 4
      }
    ]
    Object.assign(this.getStudentAbsenceReportModel, { filterParams: filterParams });
    this.getStudentAbsenceReportModel.pageNumber = 1;
    this.paginator.pageIndex = 0;
    this.getStudentAbsenceReportModel.pageSize = this.pageSize;
    this.getStudentAbsenceReport();
  }

  getPageEvent(event) {
    if (this.searchCtrl.value != null && this.searchCtrl.value != "") {
      let filterParams = [
        {
          columnName: null,
          filterValue: this.searchCtrl.value,
          filterOption: 3
        }
      ]
      Object.assign(this.getStudentAbsenceReportModel, { filterParams: filterParams });
    }
    this.getStudentAbsenceReportModel.pageNumber = event.pageIndex + 1;
    this.getStudentAbsenceReportModel.pageSize = event.pageSize;
    this.getStudentAbsenceReport();
  }

  onSearch() {
    this.parentData.markingPeriodStartDate = this.getStudentAbsenceReportModel.markingPeriodStartDate ? this.commonFunction.formatDateSaveWithoutTime(this.getStudentAbsenceReportModel.markingPeriodStartDate) : null;
    this.parentData.markingPeriodEndDate = this.getStudentAbsenceReportModel.markingPeriodEndDate ? this.commonFunction.formatDateSaveWithoutTime(this.getStudentAbsenceReportModel.markingPeriodEndDate) : null;
    this.disabledAdvancedSearch = true;
    if(this.getStudentAbsenceReportModel.periodId ==='daily'){
      this.getStudentAbsenceReportModel.periodId= null;
    }
    this.parentData.periodId = this.getStudentAbsenceReportModel.periodId;
    if (!this.selectedReportBy) {
      
      this.getStudentAbsenceReportModel.markingPeriodStartDate = this.getStudentAbsenceReportModel.markingPeriodStartDate ? this.commonFunction.formatDateSaveWithoutTime(this.getStudentAbsenceReportModel.markingPeriodStartDate) : null;
      this.getStudentAbsenceReportModel.markingPeriodEndDate = this.getStudentAbsenceReportModel.markingPeriodEndDate ? this.commonFunction.formatDateSaveWithoutTime(this.getStudentAbsenceReportModel.markingPeriodEndDate) : null;
      if (this.getStudentAbsenceReportModel.markingPeriodStartDate && this.getStudentAbsenceReportModel.markingPeriodEndDate) {
        if (this.getStudentAbsenceReportModel.markingPeriodStartDate <= this.getStudentAbsenceReportModel.markingPeriodEndDate) {
          this.getStudentAbsenceReport();
        } else {
          this.snackbar.open("To Date value should be greater than From Date value", "", {
            duration: 10000
          });
        }
      } else if (!this.getStudentAbsenceReportModel.markingPeriodStartDate && !this.getStudentAbsenceReportModel.markingPeriodEndDate) {
        this.snackbar.open("Choose From date and To date", "", {
          duration: 10000
        });
      } else if ((this.getStudentAbsenceReportModel.markingPeriodStartDate && !this.getStudentAbsenceReportModel.markingPeriodEndDate) || (!this.getStudentAbsenceReportModel.markingPeriodStartDate && this.getStudentAbsenceReportModel.markingPeriodEndDate)) {
        this.snackbar.open("Choose both From date and To date", "", {
          duration: 10000,
        });
      }
    } else {
      if (!this.getStudentAbsenceReportModel.markingPeriodStartDate && !this.getStudentAbsenceReportModel.markingPeriodEndDate) {
        this.selectOptions.map(x => {
          if (x.title === this.selectedReportBy) {
            this.getStudentAbsenceReportModel.markingPeriodStartDate = x.startDate
            this.getStudentAbsenceReportModel.markingPeriodEndDate = x.endDate
          }
        })
      }
      this.getStudentAbsenceReport();
    }
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

  getSearchResult(res) {
    if (res.totalCount) {
      this.searchCount = res.totalCount;
      this.totalCount = res.totalCount;
    }
    else {
      this.searchCount = 0;
      this.totalCount = 0;
    }
    this.studentLists = res.studendAttendanceList;
    this.pageNumber = res.pageNumber;
    this.pageSize = res._pageSize;
  }

  getDropDownData() {
    this.schoolPeriodService.getAllBlockList(this.blockListViewModel).subscribe((res: BlockListViewModel) => {
      res.getBlockListForView.forEach(element => {
        element.blockPeriod.forEach(index => {
          this.periodList.push({ periodId: index.periodId, periodTitle: index.periodTitle })
        })
      })
    })

  }

  exportToExcel(){
    if (!this.totalCount) {
      this.snackbar.open('No Record Found. Failed to Student absence sumary', '', {
        duration: 5000
      });
      return;
    }
    this.getStudentAbsenceReportModel.pageNumber = 0;
      this.getStudentAbsenceReportModel.pageSize = 0;
      
      this.reportService.getAllStudentAbsenceList(this.getStudentAbsenceReportModel).subscribe(res => {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          this.snackbar.open('Failed to Export Students Absence Summary List.' + res._message, '', {
            duration: 10000
          });
        } else {
          if (res.studendAttendanceList.length > 0) {
            let studentList;
            studentList = res.studendAttendanceList?.map((x) => {
              return {
                [this.defaultValuesService.translateKey('studentName')]: x.firstGivenName +" "+ x.lastFamilyName,
                [this.defaultValuesService.translateKey('studentId')]: x.studentInternalId,
                [this.defaultValuesService.translateKey('alternateId')]: x.studentAlternetId,
                [this.defaultValuesService.translateKey('grade')]: x.gradeLevelTitle,
                [this.defaultValuesService.translateKey('phone')]: x.homePhone,
                [this.defaultValuesService.translateKey('absent')]: x.absentCount,
                [this.defaultValuesService.translateKey('halfDay')]: x.halfDayCount
              };
            });
            this.excelService.exportAsExcelFile(studentList, 'Students_absence_summary_list')
          } else {
            this.snackbar.open('No Records Found. Failed to Export Students Absence Summary List', '', {
              duration: 5000
            });
          }
        }
      });
  }

  getStudentAbsenceReport() {
    this.reportService.getAllStudentAbsenceList(this.getStudentAbsenceReportModel).subscribe((data: any) => {
      if (data) {
        if (data._failure) {
          this.commonService.checkTokenValidOrNot(data._message);
        } else {
          this.studentListForAbsenceSummary = data;
          this.studentLists = data.studendAttendanceList;
          this.totalCount = data.totalCount;
          this.pageNumber = data.pageNumber;
          this.pageSize = data._pageSize;
        }
      }
      else {
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    });
  }



  viewAttendanceSummaryDetails(element) {
    // this.router.navigate(['/school', 'attendance', 'missing-attendance', 'missing-attendance-details']);
    this.router.navigate(['/school', 'reports', 'attendance', 'absence-summary', 'absence-summary-details'],{ state: { studentdata: element, dropdownValues: this.getStudentAbsenceReportModel,selectedReportBy: this.selectedReportBy, selectOptions: this.selectOptions } });
  }


  showAdvanceSearch() {
    this.showAdvanceSearchPanel = true;
    this.filterJsonParams = null;
  }

  hideAdvanceSearch(event) {
    this.showAdvanceSearchPanel = false;
  }

}
