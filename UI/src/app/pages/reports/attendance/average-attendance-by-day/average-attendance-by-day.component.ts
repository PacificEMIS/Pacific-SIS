import { DatePipe } from '@angular/common';
import { AfterViewInit, Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatPaginatorIntl } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableDataSource } from '@angular/material/table';
import { TranslateService } from '@ngx-translate/core';
import moment from 'moment';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { AverageAttendanceByDayReportModel } from 'src/app/models/attendance-code.model';
import { MarkingPeriodListModel } from 'src/app/models/marking-period.model';
import { SharedFunction } from 'src/app/pages/shared/shared-function';
import { AttendanceCodeService } from 'src/app/services/attendance-code.service';
import { CommonService } from 'src/app/services/common.service';
import { ExcelService } from 'src/app/services/excel.service';
import { LoaderService } from 'src/app/services/loader.service';
import { MarkingPeriodService } from 'src/app/services/marking-period.service';

interface Attendance {
  date: string;
  grade: string;
  students: number;
  daysPossible: number;
  present: number;
  absent: number;
  others: number;
  notTaken: number;
  ada: string;
  avgAttendance: number;
  avgAbsent: number;
}

@Component({
  selector: 'vex-average-attendance-by-day',
  templateUrl: './average-attendance-by-day.component.html',
  styleUrls: ['./average-attendance-by-day.component.scss']
})
export class AverageAttendanceByDayComponent implements OnInit, AfterViewInit {
  columns = [
    { label: 'date', property: 'date', type: 'text', visible: true },
    { label: 'grade', property: 'grade', type: 'text', visible: true },
    { label: 'students', property: 'students', type: 'text', visible: true },
    { label: 'daysPossible', property: 'daysPossible', type: 'text', visible: true },
    { label: 'present', property: 'present', type: 'text', visible: true },
    { label: 'absent', property: 'absent', type: 'text', visible: true },
    { label: 'others', property: 'others', type: 'text', visible: true },
    { label: 'notTaken', property: 'notTaken', type: 'text', visible: true },
    { label: 'ada', property: 'ada', type: 'text', visible: true },
    { label: 'avgAttendance', property: 'avgAttendance', type: 'text', visible: true },
    { label: 'avgAbsent', property: 'avgAbsent', type: 'text', visible: true },
  ];

  averageAttendanceByDayReportModel: AverageAttendanceByDayReportModel = new AverageAttendanceByDayReportModel();
  markingPeriodListModel: MarkingPeriodListModel = new MarkingPeriodListModel();
  selectOptions: any = [
    {
      title: 'This School Year',
      id: 'school_year'
    },
    {
      title: 'This Month',
      id: 'this_month'
    },
  ];

  destroySubject$: Subject<void> = new Subject();
  loading: boolean;
  searchCtrl = new FormControl();
  selectedReportBy: any;
  averageAttendanceByDayTable: MatTableDataSource<any>;
  averageAttendanceByDay: any;
  isVisible: boolean = false;
  totalCount: number;
  pageSize: number;

  constructor(
    public translateService: TranslateService,
    private paginatorObj: MatPaginatorIntl,
    private defaultValuesService:DefaultValuesService,
    private loaderService: LoaderService,
    private commonFunction: SharedFunction,
    private markingPeriodService: MarkingPeriodService,
    private commonService: CommonService,
    private attendenceService: AttendanceCodeService,
    private snackbar: MatSnackBar,
    private excelService: ExcelService,
    private datepipe: DatePipe
    ) { 
      paginatorObj.itemsPerPageLabel = translateService.instant('itemsPerPage');
      this.defaultValuesService.setReportCompoentTitle.next("Average Attendance by Day");
    // translateService.use("en");
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });
    paginatorObj.itemsPerPageLabel = translateService.instant('itemsPerPage');
    this.selectOptions[0].startDate = this.defaultValuesService.getFullYearStartDate();
    this.selectOptions[0].endDate = this.defaultValuesService.getFullYearEndDate();

    this.selectOptions[1].startDate = moment().startOf('month').format('YYYY-MM-DD');
    this.selectOptions[1].endDate = moment().endOf('month').format('YYYY-MM-DD');
    this.selectedReportBy = this.defaultValuesService.getMarkingPeriodTitle();
    this.averageAttendanceByDayReportModel.markingPeriodStartDate = this.commonFunction.formatDateSaveWithoutTime(this.defaultValuesService.getMarkingPeriodStartDate());
    this.averageAttendanceByDayReportModel.markingPeriodEndDate = this.commonFunction.formatDateSaveWithoutTime(this.defaultValuesService.getMarkingPeriodEndDate());
    this.getMarkingPeriod()
  }

  ngOnInit(): void {
    this.searchCtrl = new FormControl();
  }

  ngAfterViewInit(): void {
    this.searchCtrl.valueChanges.pipe(debounceTime(500), distinctUntilChanged()).subscribe((term) => {
      if (term.trim().length > 0) {
        let filterParams = [
          {
            columnName: null,
            filterValue: term,
            filterOption: 3
          }
        ];
        Object.assign(this.averageAttendanceByDayReportModel, { filterParams: filterParams });
        this.getAllAverageAttendanceByDay();
      }
      else {
        Object.assign(this.averageAttendanceByDayReportModel, { filterParams: null });
        this.getAllAverageAttendanceByDay();
      }
    });
  }

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
            id: data.schoolYearsView[i]?.markingPeriodId
          });
          if (data.schoolYearsView[i].children.length > 0) {
            data.schoolYearsView[i].children.map((item: any) => {
              this.selectOptions.push({
                title: item?.title,
                startDate: item?.startDate,
                endDate: item?.endDate,
                id: item?.markingPeriodId
              });
              if (item.children.length > 0) {
                item.children.map((subItem: any) => {
                  this.selectOptions.push({
                    title: subItem?.title,
                    startDate: subItem?.startDate,
                    endDate: subItem?.endDate,
                    id: subItem?.markingPeriodId
                  });
                  if (subItem.children.length > 0) {
                    subItem.children.map((subOfSubItem: any) => {
                      this.selectOptions.push({
                        title: subOfSubItem?.title,
                        startDate: subOfSubItem?.startDate,
                        endDate: subOfSubItem?.endDate,
                        id: subOfSubItem?.markingPeriodId
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

  checkAndGetAverageAttendanceByDay() {
    if (!this.selectedReportBy) {
      this.averageAttendanceByDayReportModel.markingPeriodStartDate = this.averageAttendanceByDayReportModel.markingPeriodStartDate ? this.commonFunction.formatDateSaveWithoutTime(this.averageAttendanceByDayReportModel.markingPeriodStartDate) : null;
      this.averageAttendanceByDayReportModel.markingPeriodEndDate = this.averageAttendanceByDayReportModel.markingPeriodEndDate ? this.commonFunction.formatDateSaveWithoutTime(this.averageAttendanceByDayReportModel.markingPeriodEndDate) : null;
      if (this.averageAttendanceByDayReportModel.markingPeriodStartDate && this.averageAttendanceByDayReportModel.markingPeriodEndDate) {
        if (this.averageAttendanceByDayReportModel.markingPeriodStartDate <= this.averageAttendanceByDayReportModel.markingPeriodEndDate) {
          this.getAllAverageAttendanceByDay();
        } else {
          this.snackbar.open("To Date value should be greater than From Date value", "", {
            duration: 10000
          });
        }
      } else if (!this.averageAttendanceByDayReportModel.markingPeriodStartDate && !this.averageAttendanceByDayReportModel.markingPeriodEndDate) {
        this.snackbar.open("Choose From date and To date", "", {
          duration: 10000
        });
      } else if ((this.averageAttendanceByDayReportModel.markingPeriodStartDate && !this.averageAttendanceByDayReportModel.markingPeriodEndDate) || (!this.averageAttendanceByDayReportModel.markingPeriodStartDate && this.averageAttendanceByDayReportModel.markingPeriodEndDate)) {
        this.snackbar.open("Choose both From date and To date", "", {
          duration: 10000,
        });
      }
    } else {
      this.getAllAverageAttendanceByDay();
    }
  }

  getAllAverageAttendanceByDay() {
    this.attendenceService.getAverageAttendanceByDayReport(this.averageAttendanceByDayReportModel).subscribe(res => {
      this.averageAttendanceByDay = res;
      if (this.averageAttendanceByDay._failure) {
        this.isVisible = false;
        this.snackbar.open(this.averageAttendanceByDay._message, '', {
          duration: 10000
        });
        this.averageAttendanceByDayTable = new MatTableDataSource([this.averageAttendanceByDay.averageDailyAttendanceReport])
        this.totalCount = 0;
      } else {
        this.isVisible = true;
        this.totalCount = this.averageAttendanceByDay.totalCount;
        this.averageAttendanceByDayTable = new MatTableDataSource(this.averageAttendanceByDay.averageDailyAttendanceReport);
      }
    })
  }

  getReportBy(event) {
    const selectedOption = this.selectOptions.filter(x => x.title === event.value);
    if (event.value) {
      this.averageAttendanceByDayReportModel.markingPeriodStartDate = selectedOption[0].startDate;
      this.averageAttendanceByDayReportModel.markingPeriodEndDate = selectedOption[0].endDate;
    } else {
      this.averageAttendanceByDayReportModel.markingPeriodStartDate = null;
      this.averageAttendanceByDayReportModel.markingPeriodEndDate = null;
    }
  }

  exportAccessLogListToExcel() {
    let excelData = this.averageAttendanceByDay.averageDailyAttendanceReport?.map((x) => {
      return {
        [this.defaultValuesService.translateKey("date")]: `${x.date ? this.datepipe.transform(x.date, 'yyyy-MM-dd') : '--'}`,
        [this.defaultValuesService.translateKey("gradeLevel")]: `${x.gradeLevel ? x.gradeLevel : '-'}`,
        [this.defaultValuesService.translateKey("students")]: `${x.students ? x.students : 0}`,
        [this.defaultValuesService.translateKey("daysPossible")]: `${x.daysPossible ? x.daysPossible : 0}`,
        [this.defaultValuesService.translateKey("attendancePossible")]: `${x.attendancePossible ? x.attendancePossible : 0}`,
        [this.defaultValuesService.translateKey("present")]: `${x.present ? x.present : 0}`,
        [this.defaultValuesService.translateKey("absent")]: `${x.absent ? x.absent : 0}`,
        [this.defaultValuesService.translateKey("other")]: `${x.other ? x.other : 0}`,
        [this.defaultValuesService.translateKey("notTaken")]: `${x.notTaken ? x.notTaken : 0}`,
        [this.defaultValuesService.translateKey("ada")]: `${x.ada ? x.ada : 0}`,
        [this.defaultValuesService.translateKey("avgAttendance")]: `${x.avgAttendance ? x.avgAttendance : 0}`,
        [this.defaultValuesService.translateKey("avgAbsent")]: `${x.avgAbsent ? x.avgAbsent : 0}`,
      };
    });
    this.excelService.exportAsExcelFile(excelData, "Students_Avg_Attendance_By_Day_Report");
  }

  getPageEvent(event) {
    this.averageAttendanceByDayReportModel.pageNumber = event.pageIndex + 1;
    this.averageAttendanceByDayReportModel.pageSize = event.pageSize;
    this.defaultValuesService.setPageSize(event.pageSize);
    this.getAllAverageAttendanceByDay();
  }

  toggleColumnVisibility(column, event) {    
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }
}
