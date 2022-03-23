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
import { AbsenceListByStudent, GetStudentAbsenceReport, StudentListForAbsenceSummary } from 'src/app/models/absence-summary.model';
import { GetAllAttendanceCodeModel } from 'src/app/models/attendance-code.model';
import { MarkingPeriodListModel } from 'src/app/models/marking-period.model';
import { BlockListViewModel } from 'src/app/models/school-period.model';
import { SharedFunction } from 'src/app/pages/shared/shared-function';
import { CommonService } from 'src/app/services/common.service';
import { LoaderService } from 'src/app/services/loader.service';
import { MarkingPeriodService } from 'src/app/services/marking-period.service';
import { ReportService } from 'src/app/services/report.service';
import { SchoolPeriodService } from 'src/app/services/school-period.service';
import moment from 'moment';
import { ExcelService } from 'src/app/services/excel.service';
@Component({
  selector: 'vex-absence-summary-details',
  templateUrl: './absence-summary-details.component.html',
  styleUrls: ['./absence-summary-details.component.scss']
})
export class AbsenceSummaryDetailsComponent implements OnInit {

  displayedColumns: string[] = ['date', 'attendance', 'adminOfficeComment', 'teacherComment'];
  getStudentAbsenceReportModel: GetStudentAbsenceReport = new GetStudentAbsenceReport();
  studentLists = [];
  studentDetails: any;
  blockListViewModel: BlockListViewModel = new BlockListViewModel();
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  searchCtrl: FormControl;
  totalCount: number = 0;
  pageSize: number;
  allAttendence: MatTableDataSource<any>;
  pageNumber: number;
  absenceListByStudent: AbsenceListByStudent = new AbsenceListByStudent();
  getAllAttendanceCodeModel: GetAllAttendanceCodeModel = new GetAllAttendanceCodeModel();
  isVisible: boolean = false;
  loading: boolean;
  destroySubject$: Subject<void> = new Subject();
  selectedReportBy: any;
  globalMarkingPeriodEndDate;
  globalMarkingPeriodStartDate;
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

  periodList = []
  data: any;
  
  constructor(public translateService: TranslateService,
    private router: Router,
    private commonFunction: SharedFunction,
    private commonService: CommonService,
    private paginatorObj: MatPaginatorIntl,
    private reportService: ReportService,
    private schoolPeriodService: SchoolPeriodService,
    private snackbar: MatSnackBar,
    private excelService: ExcelService,
    public defaultValuesService: DefaultValuesService,
    private loaderService: LoaderService,
    private markingPeriodService: MarkingPeriodService,) {
    //this.getMarkingPeriod();
    this.selectOptions= this.router.getCurrentNavigation()?.extras?.state?.selectOptions;
    this.getDropDownData();
    this.studentDetails = this.router.getCurrentNavigation()?.extras?.state?.studentdata;
    this.getStudentAbsenceReportModel = this.router.getCurrentNavigation()?.extras?.state?.dropdownValues;
    this.selectedReportBy = this.router.getCurrentNavigation()?.extras?.state?.selectedReportBy;
    if (this.studentDetails) {
      this.getStudentAbsenceReportModel.studentId = this.studentDetails.studentId;
      this.getAbsenceListByStudent();
    }
    else {
      this.router.navigate(['/school', 'reports', 'attendance', 'absence-summary']);
    }
    paginatorObj.itemsPerPageLabel = translateService.instant('itemsPerPage');
    this.defaultValuesService.setReportCompoentTitle.next("Absence Summary");
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });
    this.getDropDownData();
  }
  ngOnInit(): void {
    this.getStudentAbsenceReportModel.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
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


  getPageEvent(event) {
    this.getStudentAbsenceReportModel.pageNumber = event.pageIndex + 1;
    this.getStudentAbsenceReportModel.pageSize = event.pageSize;
    this.getAbsenceListByStudent();
  }

  onSearch() {
    if (this.getStudentAbsenceReportModel.periodId === 'daily') {
      this.getStudentAbsenceReportModel.periodId = null;
    }
    if (!this.selectedReportBy) {

      this.getStudentAbsenceReportModel.markingPeriodStartDate = this.getStudentAbsenceReportModel.markingPeriodStartDate ? this.commonFunction.formatDateSaveWithoutTime(this.getStudentAbsenceReportModel.markingPeriodStartDate) : null;
      this.getStudentAbsenceReportModel.markingPeriodEndDate = this.getStudentAbsenceReportModel.markingPeriodEndDate ? this.commonFunction.formatDateSaveWithoutTime(this.getStudentAbsenceReportModel.markingPeriodEndDate) : null;
      if (this.getStudentAbsenceReportModel.markingPeriodStartDate && this.getStudentAbsenceReportModel.markingPeriodEndDate) {
        if (this.getStudentAbsenceReportModel.markingPeriodStartDate <= this.getStudentAbsenceReportModel.markingPeriodEndDate) {
          this.getAbsenceListByStudent();
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
      this.getAbsenceListByStudent();
    }
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
      
      this.reportService.getAbsenceListByStudent(this.getStudentAbsenceReportModel).subscribe(res => {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          this.snackbar.open('Failed to Export Students absence sumary.' + res._message, '', {
            duration: 10000
          });
        } else {
          if (res.studendList.length > 0) {
            let studentList;
            studentList = res.studendList?.map((x) => {
              return {
                [this.defaultValuesService.translateKey('date')]: x.absenceDate,
                [this.defaultValuesService.translateKey('attendance')]: x.attendance,
                [this.defaultValuesService.translateKey('adminOfficeComment')]: x.adminComment,
                [this.defaultValuesService.translateKey('teacherComment')]: x.teacherComment
              };
            });
            this.excelService.exportAsExcelFile(studentList, 'Student_absence_sumary_list')
          } else {
            this.snackbar.open('No Records Found. Failed to Export Student absence sumary', '', {
              duration: 5000
            });
          }
        }
      });
  }

  getAbsenceListByStudent() {
    this.reportService.getAbsenceListByStudent(this.getStudentAbsenceReportModel).subscribe((data: any) => {
      if (data) {
        if (data._failure) {
          this.commonService.checkTokenValidOrNot(data._message);
          this.studentLists = [];
          this.snackbar.open(data._message, '', {
            duration: 10000
          });
        } else {
          this.absenceListByStudent = data;
          this.studentLists = data.studendList;
          this.totalCount = data.totalCount;
          this.pageNumber = data.pageNumber;
          this.pageSize = data._pageSize;
        }
      }
      else {
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
        this.studentLists = [];
      }
    });
  }

}
