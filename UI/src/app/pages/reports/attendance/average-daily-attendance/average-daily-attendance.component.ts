import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { AverageDailyAttendanceReportModel } from 'src/app/models/attendance-code.model';
import { SharedFunction } from 'src/app/pages/shared/shared-function';
import { AttendanceCodeService } from 'src/app/services/attendance-code.service';
import { CommonService } from 'src/app/services/common.service';
import { MarkingPeriodService } from 'src/app/services/marking-period.service';
import { MarkingPeriodListModel } from 'src/app/models/marking-period.model';
import { MatTableDataSource } from '@angular/material/table';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FormControl } from '@angular/forms';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { LoaderService } from 'src/app/services/loader.service';
import { Subject } from 'rxjs';
import moment from 'moment';
import { ExcelService } from 'src/app/services/excel.service';


@Component({
  selector: 'vex-average-daily-attendance',
  templateUrl: './average-daily-attendance.component.html',
  styleUrls: ['./average-daily-attendance.component.scss']
})
export class AverageDailyAttendanceComponent implements OnInit {
  isVisible: boolean = false;
  destroySubject$: Subject<void> = new Subject();
  averageDailyAttendanceReportModel: AverageDailyAttendanceReportModel = new AverageDailyAttendanceReportModel();
  selectedReportBy: any;
  markingPeriodListModel: MarkingPeriodListModel = new MarkingPeriodListModel();
  averageDailyAttendance: any;
  averageDailyAttendanceTable: MatTableDataSource<any>;
  totalCount: number;
  searchCtrl = new FormControl();
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
  loading: boolean;
  excelDataSet = {
    'gradeLevel': 'Total',
    'students': 0,
    'daysPossible': 0,
    'attendancePossible': 0,
    'present': 0, 'absent': 0,
    'other': 0, 'notTaken': 0,
    'ada': 0,
    'avgAttendance': 0,
    'avgAbsent': 0
  }
  displayedColumns: string[] = ['grade', 'students', 'daysPossible', 'attendancePossible', 'present', 'absent', 'other', 'notTaken', 'ada', 'avgAttendance', 'avgAbsent'];
  columns = [
    { label: 'Grade', property: 'grade', type: 'text', visible: true },
    { label: 'Students', property: 'students', type: 'text', visible: true },
    { label: 'Days Possible', property: 'daysPossible', type: 'text', visible: true },
    { label: 'Attendance Possible', property: 'attendancePossible', type: 'text', visible: true },
    { label: 'Present', property: 'present', type: 'text', visible: true },
    { label: 'Absent', property: 'absent', type: 'text', visible: true },
    { label: 'Other', property: 'other', type: 'text', visible: true },
    { label: 'Not Taken', property: 'notTaken', type: 'text', visible: true },
    { label: 'ADA', property: 'ada', type: 'text', visible: true },
    { label: 'Avg Attendance', property: 'avgAttendance', type: 'text', visible: true },
    { label: 'avgAbsent', property: 'avgAbsent', type: 'text', visible: true },
  ];
  constructor(public translateService: TranslateService,
    private attendenceService: AttendanceCodeService,
    private commonFunction: SharedFunction,
    private markingPeriodService: MarkingPeriodService,
    private commonService: CommonService,
    private loaderService: LoaderService,
    private excelService: ExcelService,
    private snackbar: MatSnackBar,
    public defaultValuesService: DefaultValuesService,
  ) {
    // translateService.use("en");
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });
    this.selectOptions[0].startDate = this.defaultValuesService.getFullYearStartDate();
    this.selectOptions[0].endDate = this.defaultValuesService.getFullYearEndDate();

    this.selectOptions[1].startDate = moment().startOf('month').format('YYYY-MM-DD');
    this.selectOptions[1].endDate = moment().endOf('month').format('YYYY-MM-DD');
    this.selectedReportBy = this.defaultValuesService.getMarkingPeriodTitle();
    this.averageDailyAttendanceReportModel.markingPeriodStartDate = this.commonFunction.formatDateSaveWithoutTime(this.defaultValuesService.getMarkingPeriodStartDate());
    this.averageDailyAttendanceReportModel.markingPeriodEndDate = this.commonFunction.formatDateSaveWithoutTime(this.defaultValuesService.getMarkingPeriodEndDate());
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
        Object.assign(this.averageDailyAttendanceReportModel, { filterParams: filterParams });
        this.getAllAverageDailyAttendance();
      }
      else {
        Object.assign(this.averageDailyAttendanceReportModel, { filterParams: null });
        this.getAllAverageDailyAttendance();
      }
    });
  }

  getlAverageDailyAttendance() {
    if (!this.selectedReportBy) {
      this.averageDailyAttendanceReportModel.markingPeriodStartDate = this.averageDailyAttendanceReportModel.markingPeriodStartDate ? this.commonFunction.formatDateSaveWithoutTime(this.averageDailyAttendanceReportModel.markingPeriodStartDate) : null;
      this.averageDailyAttendanceReportModel.markingPeriodEndDate = this.averageDailyAttendanceReportModel.markingPeriodEndDate ? this.commonFunction.formatDateSaveWithoutTime(this.averageDailyAttendanceReportModel.markingPeriodEndDate) : null;
      if (this.averageDailyAttendanceReportModel.markingPeriodStartDate && this.averageDailyAttendanceReportModel.markingPeriodEndDate) {
        if (this.averageDailyAttendanceReportModel.markingPeriodStartDate <= this.averageDailyAttendanceReportModel.markingPeriodEndDate) {
          this.getAllAverageDailyAttendance();
        } else {
          this.snackbar.open("To Date value should be greater than From Date value", "", {
            duration: 10000
          });
        }
      } else if (!this.averageDailyAttendanceReportModel.markingPeriodStartDate && !this.averageDailyAttendanceReportModel.markingPeriodEndDate) {
        this.snackbar.open("Choose From date and To date", "", {
          duration: 10000
        });
      } else if ((this.averageDailyAttendanceReportModel.markingPeriodStartDate && !this.averageDailyAttendanceReportModel.markingPeriodEndDate) || (!this.averageDailyAttendanceReportModel.markingPeriodStartDate && this.averageDailyAttendanceReportModel.markingPeriodEndDate)) {
        this.snackbar.open("Choose both From date and To date", "", {
          duration: 10000,
        });
      }
    } else {
      this.getAllAverageDailyAttendance();
    }
  }

  getAllAverageDailyAttendance() {

    // this.averageDailyAttendanceReportModel.markingPeriodStartDate = this.commonFunction.formatDateSaveWithoutTime(this.defaultValuesService.getMarkingPeriodStartDate())
    // this.averageDailyAttendanceReportModel.markingPeriodEndDate = this.commonFunction.formatDateSaveWithoutTime(this.defaultValuesService.getMarkingPeriodEndDate())

    this.attendenceService.getAverageDailyAttendanceReport(this.averageDailyAttendanceReportModel).subscribe(res => {
      this.isVisible = true;
      this.averageDailyAttendance = res;
      if (this.averageDailyAttendance._failure) {
        // if (this.averageDailyAttendance.averageDailyAttendanceReport === null) {
        //   this.averageDailyAttendanceTable = new MatTableDataSource([this.averageDailyAttendance.averageDailyAttendanceReport]);
        //   this.snackbar.open(this.averageDailyAttendance._message, '', {
        //     duration: 10000
        //   });
        // } else {
        //   this.averageDailyAttendanceTable = new MatTableDataSource([this.averageDailyAttendance.averageDailyAttendanceReport]);
        // }
        this.snackbar.open(this.averageDailyAttendance._message, '', {
          duration: 10000
        });
        this.averageDailyAttendanceTable = new MatTableDataSource([this.averageDailyAttendance.averageDailyAttendanceReport])
        this.totalCount = 0;
      } else {
        this.totalCount = this.averageDailyAttendance.averageDailyAttendanceReport.length;
        /** This dataSet is for the the last Total object */
        /** This forIn is for remove all data from final object */
        for (let a in this.excelDataSet) {
          if (a !== "gradeLevel")
            this.excelDataSet[a] = 0
        }
        /** dataSet Start */
        this.averageDailyAttendance.averageDailyAttendanceReport.map(val => {
          this.excelDataSet.students += val.students
          this.excelDataSet.daysPossible += val.daysPossible
          this.excelDataSet.attendancePossible += val.attendancePossible
          this.excelDataSet.present += val.present
          this.excelDataSet.absent += val.absent
          this.excelDataSet.other += val.other
          this.excelDataSet.notTaken += val.notTaken
          this.excelDataSet.ada += val.ada
          this.excelDataSet.avgAttendance += val.avgAttendance
          this.excelDataSet.avgAbsent += val.avgAbsent
        })
        this.excelDataSet.ada /= this.totalCount
        this.excelDataSet.avgAttendance /= this.totalCount
        this.excelDataSet.avgAbsent /= this.totalCount
        /** dataSet End */

        this.averageDailyAttendance.averageDailyAttendanceReport.push(this.excelDataSet)
        this.averageDailyAttendanceTable = new MatTableDataSource(this.averageDailyAttendance.averageDailyAttendanceReport);

        // this.averageDailyAttendanceTable.data.push(this.excelDataSet)
      }
    })
  }

  exportAccessLogListToExcel() {
    let excelData = this.averageDailyAttendance.averageDailyAttendanceReport?.map((x) => {
      return {
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
    this.excelService.exportAsExcelFile(excelData, "Student_Enrollment_Report");
  }

  getReportBy(event) {
    const selectedOption = this.selectOptions.filter(x => x.title === event.value);
    if (event.value) {
      this.averageDailyAttendanceReportModel.markingPeriodStartDate = selectedOption[0].startDate;
      this.averageDailyAttendanceReportModel.markingPeriodEndDate = selectedOption[0].endDate;
    } else {
      this.averageDailyAttendanceReportModel.markingPeriodStartDate = null;
      this.averageDailyAttendanceReportModel.markingPeriodEndDate = null;
    }
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


  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }
}
