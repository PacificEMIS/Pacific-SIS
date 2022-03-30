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

import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import icPrint from '@iconify/icons-ic/twotone-print';
import { ReportService } from 'src/app/services/report.service';
import { GetStudentAddDropReportModel, filterParams } from 'src/app/models/report.model';
import { CommonService } from 'src/app/services/common.service';
import { MatTableDataSource } from '@angular/material/table';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { SharedFunction } from 'src/app/pages/shared/shared-function';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { LoaderService } from 'src/app/services/loader.service';
import { MatPaginator } from '@angular/material/paginator';
import { FormControl } from '@angular/forms';
import { ExcelService } from 'src/app/services/excel.service';
import * as moment from 'moment';
import { GetAllGradeLevelsModel } from 'src/app/models/grade-level.model';
import { GradeLevelService } from 'src/app/services/grade-level.service';
import { MarkingPeriodListModel } from 'src/app/models/marking-period.model';
import { MarkingPeriodService } from 'src/app/services/marking-period.service';

@Component({
  selector: 'vex-add-drop-report',
  templateUrl: './add-drop-report.component.html',
  styleUrls: ['./add-drop-report.component.scss']
})

export class AddDropReportComponent implements OnInit, OnDestroy, AfterViewInit {
  icPrint = icPrint;
  displayedColumns: string[] = ['studentName', 'studentId', 'school', 'enrolledDate', 'enrollmentCode', 'droppedDate', 'dropCode'];
  destroySubject$: Subject<void> = new Subject();
  loading: boolean;
  getStudentAddDropReportModel: GetStudentAddDropReportModel = new GetStudentAddDropReportModel();
  addDropStudentList: MatTableDataSource<any>;
  reportSchoolInfo;
  studentEnrollmentList = [];
  totalCount: number = 0;
  pageNumber: number;
  pageSize: number;
  today: Date = new Date();
  searchCtrl: FormControl;
  filterParams: filterParams[] = [];
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  getAllGradeLevelsModel: GetAllGradeLevelsModel = new GetAllGradeLevelsModel();
  markingPeriodListModel: MarkingPeriodListModel = new MarkingPeriodListModel();
  gradeLevelList = [];
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
  selectedReportBy: any;

  constructor(public translateService: TranslateService,
    private reportService: ReportService,
    private commonService: CommonService,
    private snackbar: MatSnackBar,
    public defaultValuesService: DefaultValuesService,
    private commonFunction: SharedFunction,
    private loaderService: LoaderService,
    private excelService: ExcelService,
    private gradeLevelService: GradeLevelService,
    private markingPeriodService: MarkingPeriodService
  ) {
    this.defaultValuesService.setReportCompoentTitle.next("Add / Drop Report");
    // translateService.use("en");
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });
    this.selectOptions[0].startDate = this.defaultValuesService.getFullYearStartDate();
    this.selectOptions[0].endDate = this.defaultValuesService.getFullYearEndDate();

    this.selectOptions[1].startDate = moment().startOf('month').format('YYYY-MM-DD');
    this.selectOptions[1].endDate = moment().endOf('month').format('YYYY-MM-DD');

    this.selectedReportBy = this.defaultValuesService.getMarkingPeriodTitle();
    this.getStudentAddDropReportModel.markingPeriodStartDate = this.commonFunction.formatDateSaveWithoutTime(this.defaultValuesService.getMarkingPeriodStartDate());
    this.getStudentAddDropReportModel.markingPeriodEndDate = this.commonFunction.formatDateSaveWithoutTime(this.defaultValuesService.getMarkingPeriodEndDate());
    this.getMarkingPeriod();
    this.getAllGradeLevelList();
  }

  ngOnInit(): void {
    this.getStudentAddDropReportModel.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
    this.searchCtrl = new FormControl();
    this.getStudentAddDropReport();
  }

  ngAfterViewInit(): void {
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
        Object.assign(this.getStudentAddDropReportModel, { filterParams: filterParams });
        this.filterParams = filterParams;
        this.getStudentAddDropReportModel.pageNumber = 1;
        this.paginator.pageIndex = 0;
        this.getStudentAddDropReportModel.pageSize = this.pageSize;
        this.getStudentAddDropReport();
      }
      else {
        Object.assign(this.getStudentAddDropReportModel, { filterParams: null });
        this.filterParams = null;
        this.getStudentAddDropReportModel.pageNumber = this.paginator.pageIndex + 1;
        this.getStudentAddDropReportModel.pageSize = this.pageSize;
        this.getStudentAddDropReport();
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

  // For get all grade level list
  getAllGradeLevelList() {
    this.gradeLevelService.getAllGradeLevels(this.getAllGradeLevelsModel).subscribe(data => {
      if (data) {
        if (data._failure) {
          this.commonService.checkTokenValidOrNot(data._message);
          this.gradeLevelList = [];
          if (!data.tableGradelevelList) {
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
          }
        } else {
          this.gradeLevelList = data.tableGradelevelList;
        }
      } else {
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    });
  }

  // For searching add drop student list by date range
  generateReportStudentList() {
    if (!this.selectedReportBy) {
      this.getStudentAddDropReportModel.markingPeriodStartDate = this.getStudentAddDropReportModel.markingPeriodStartDate ? this.commonFunction.formatDateSaveWithoutTime(this.getStudentAddDropReportModel.markingPeriodStartDate) : null;
      this.getStudentAddDropReportModel.markingPeriodEndDate = this.getStudentAddDropReportModel.markingPeriodEndDate ? this.commonFunction.formatDateSaveWithoutTime(this.getStudentAddDropReportModel.markingPeriodEndDate) : null;
      if (this.getStudentAddDropReportModel.markingPeriodStartDate && this.getStudentAddDropReportModel.markingPeriodEndDate) {
        if (this.getStudentAddDropReportModel.markingPeriodStartDate <= this.getStudentAddDropReportModel.markingPeriodEndDate) {
          this.getStudentAddDropReport();
        } else {
          this.snackbar.open("To Date value should be greater than From Date value", "", {
            duration: 10000
          });
        }
      } else if (!this.getStudentAddDropReportModel.markingPeriodStartDate && !this.getStudentAddDropReportModel.markingPeriodEndDate) {
        this.snackbar.open("Choose From date and To date", "", {
          duration: 10000
        });
      } else if ((this.getStudentAddDropReportModel.markingPeriodStartDate && !this.getStudentAddDropReportModel.markingPeriodEndDate) || (!this.getStudentAddDropReportModel.markingPeriodStartDate && this.getStudentAddDropReportModel.markingPeriodEndDate)) {
        this.snackbar.open("Choose both From date and To date", "", {
          duration: 10000,
        });
      }
    } else {
      this.getStudentAddDropReport();
    }
  }

  getReportBy(event) {
    if (event.value) {
      const selectedOption = this.selectOptions.filter(x => x.subTitle === event.value);
      this.getStudentAddDropReportModel.markingPeriodStartDate = this.commonFunction.formatDateSaveWithoutTime(selectedOption[0].startDate);
      this.getStudentAddDropReportModel.markingPeriodEndDate = this.commonFunction.formatDateSaveWithoutTime(selectedOption[0].endDate);
    } else {
      this.getStudentAddDropReportModel.markingPeriodStartDate = null;
      this.getStudentAddDropReportModel.markingPeriodEndDate = null;
    }
  }

  // For format the date
  formatDate(date) {
    return date ? moment(date).format('MMM DD, YYYY') : null;
  }

  // For get all data from API
  getStudentAddDropReport() {
    this.reportService.getStudentAddDropReport(this.getStudentAddDropReportModel).subscribe(data => {
      if (data) {
        if (data._failure) {
          this.commonService.checkTokenValidOrNot(data._message);
          if (data.studentEnrollmentList === null) {
            this.addDropStudentList = new MatTableDataSource([]);
            this.totalCount = null;
            this.snackbar.open('' + data._message, '', {
              duration: 10000
            });
          } else {
            this.addDropStudentList = new MatTableDataSource([]);
            this.totalCount = null;
          }
        } else {
          this.totalCount = data.totalCount;
          this.pageNumber = data.pageNumber;
          this.pageSize = data._pageSize;
          this.reportSchoolInfo = data;
          this.studentEnrollmentList = this.createDataSetForTable(data.studentEnrollmentList);
          this.addDropStudentList = new MatTableDataSource(this.studentEnrollmentList);
        }
      } else {
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    });
  }

  // For creating data set for table
  createDataSetForTable(rawData) {
    rawData.map(item => {
      item.enrollmentDate = this.formatDate(item.enrollmentDate);
      item.exitDate = this.formatDate(item.exitDate);
    });
    return rawData;
  }

  // For server side pagination
  getPageEvent(event) {
    if (this.searchCtrl.value != null && this.searchCtrl.value != "") {
      let filterParams = [
        {
          columnName: null,
          filterValue: this.searchCtrl.value,
          filterOption: 3
        }
      ]
      Object.assign(this.getStudentAddDropReportModel, { filterParams: filterParams });
      this.filterParams = filterParams;
    }
    this.getStudentAddDropReportModel.pageNumber = event.pageIndex + 1;
    this.getStudentAddDropReportModel.pageSize = event.pageSize;
    this.defaultValuesService.setPageSize(event.pageSize);
    this.getStudentAddDropReport();
  }

  // For export add drop student list to excel
  exportAddDropStudentListToExcel() {
    if (!this.totalCount) {
      this.snackbar.open('No Record Found. Failed to Export Students List', '', {
        duration: 5000
      });
      return;
    }
    let getStudentAddDropReportModelForExcel: GetStudentAddDropReportModel = new GetStudentAddDropReportModel();
    getStudentAddDropReportModelForExcel.pageNumber = 0;
    getStudentAddDropReportModelForExcel.pageSize = 0;
    getStudentAddDropReportModelForExcel.markingPeriodStartDate = this.getStudentAddDropReportModel.markingPeriodStartDate;
    getStudentAddDropReportModelForExcel.markingPeriodEndDate = this.getStudentAddDropReportModel.markingPeriodEndDate;
    getStudentAddDropReportModelForExcel.gradeLevel = this.getStudentAddDropReportModel.gradeLevel;
    getStudentAddDropReportModelForExcel.filterParams = this.filterParams;
    this.reportService.getStudentAddDropReport(getStudentAddDropReportModelForExcel).subscribe(data => {
      if (data) {
        if (data._failure) {
          this.commonService.checkTokenValidOrNot(data._message);
          this.snackbar.open(data._message, '', {
            duration: 10000
          });
        } else {
          if (data.studentEnrollmentList.length > 0) {
            let formatData = [];
            let studentList;
            formatData = this.createDataSetForTable(data.studentEnrollmentList);
            studentList = formatData.map((item) => {
              const middleName = item.studentMaster.middleName ? ' ' + item.studentMaster.middleName + ' ' : ' ';
              return {
                [this.defaultValuesService.translateKey('studentName')]: item.studentMaster.firstGivenName + middleName + item.studentMaster.lastFamilyName,
                [this.defaultValuesService.translateKey('studentId')]: item.studentMaster.studentInternalId,
                [this.defaultValuesService.translateKey('school')]: item.schoolName,
                [this.defaultValuesService.translateKey('enrolledDate')]: item.enrollmentDate,
                [this.defaultValuesService.translateKey('enrollmentCode')]: item.enrollmentCode,
                [this.defaultValuesService.translateKey('droppedDate')]: item.exitDate,
                [this.defaultValuesService.translateKey('dropCode')]: item.exitCode
              };
            });
            this.excelService.exportAsExcelFile(studentList, 'Student_Add_Drop Report');
          } else {
            this.snackbar.open('No Records Found. Failed to Export Students List', '', {
              duration: 5000
            });
          }
        }
      }
    });
  }

  // For print add drop student List
  printAddDropStudentList() {
    if (!this.totalCount) {
      this.snackbar.open('No Record Found. Failed to Print Students List', '', {
        duration: 5000
      });
      return;
    }
    this.loading = true;
    setTimeout(() => {
      this.generatePDF();
      this.loading = false;
    }, 100 * this.studentEnrollmentList.length);
  }

  // For open the print window
  generatePDF() {
    let printContents, popupWin;
    printContents = document.getElementById('printSectionId').innerHTML;
    document.getElementById('printSectionId').className = 'block';
    popupWin = window.open('', '_blank', 'top=0,left=0,height=100%,width=auto');
    popupWin.document.open();
    popupWin.document.write(`
      <html>
        <head>
          <title>Print tab</title>
          <style>
    h1,
    h2,
    h3,
    h4,
    h5,
    h6,
    p {
      margin: 0;
    }
    body {
      -webkit-print-color-adjust: exact;
      font-family: Arial;
      background-color: #fff;
      margin: 0;
    }
    table {
      border-collapse: collapse;
      width: 100%;
      font-size: 14px;
    }
    .schedule-report {
        width: 1024px;
        margin: auto;
    }
    .float-left {
      float: left;
    }
    .float-right {
      float: right;
    }
    .text-center {
      text-align: center;
    }
    .text-right {
      text-align: right;
    }
    .text-left {
        text-align: left;
    }
    .ml-auto {
      margin-left: auto;
    }
    .m-auto {
      margin: auto;
    }
    .inline-block {
        display: inline-block;
    }
    .border-table {
        border: 1px solid #000;
    }
    .clearfix::after {
        display: block;
        clear: both;
        content: "";
      }
    .report-header {
        padding: 20px 0;
        border-bottom: 2px solid #000;
    }
    .school-logo {
        width: 80px;
        height: 80px;
        border-radius: 50%;
        border: 2px solid #cacaca;
        margin-right: 20px;
        text-align: center;
        overflow: hidden;
    }
    .school-logo img {
        width: 100%;
        overflow: hidden;
    }
    .report-header td {
        padding: 20px 8px 0;
    }
    .report-header td.generate-date {
        padding: 0;
    }
    .report-header .information h4 {
        font-size: 20px;
        font-weight: 600;
        padding: 10px 0;
    }
    .report-header .information p, .header-right p {
        font-size: 16px;
    }
    .header-right div {
        background-color: #000;
        color: #fff;
        font-size: 20px;
        padding: 5px 20px;
        font-weight: 600;
        margin-bottom: 8px;
    }
    .p-y-20 {
        padding-top: 20px;
        padding-bottom: 20px;
    }
    .p-x-8 {
        padding-left: 8px;
        padding-right: 8px;
    }
    .p-t-0 {
        padding-top: 0px;
    }
    .p-b-8 {
        padding-bottom: 8px;
    }
    .width-160 {
        width: 160px;
    }
    .m-r-20 {
        margin-right: 20px;
    }
    .m-b-5 {
        margin-bottom: 5px;
    }
    .m-b-8 {
        margin-bottom: 8px;
    }
    .m-b-20 {
        margin-bottom: 20px;
    }
    .m-b-15 {
        margin-bottom: 15px;
    }
    .m-b-10 {
        margin-bottom: 10px;
    }
    .m-t-20 {
        margin-top: 20px;
    }
    .m-b-15 {
        margin-bottom: 15px;
    }
    .font-bold {
        font-weight: 600;
    }
    .font-medium {
        font-weight: 500;
    }
    .f-s-20 {
        font-size: 20px;
    }
    .f-s-18 {
        font-size: 18px;
    }
    .f-s-16 {
        font-size: 16px;
    }
    .p-y-5 {
        padding-top: 5px;
        padding-bottom: 5px;
    }
    .p-x-10 {
        padding-left: 10px;
        padding-right: 10px;
    }
    .bg-slate {
        background-color: #E5E5E5;
    }
    .information-table {
        border: 1px solid #000;
        border-collapse: separate;
        border-spacing: 0;
        border-radius: 10px;
    }
    .information-table th {
        border-bottom: 1px solid #000;
        padding: 10 12px;
        text-align: left;
    }
    .information-table td {
        padding: 10px 12px;
        border-bottom: 1px solid #000;
    }
    .information-table tr:first-child th:first-child {
        border-top-left-radius: 10px;
    }
    .information-table tr:first-child th:last-child {
        border-top-right-radius: 10px;
    }
    .information-table tr:last-child td {
        border-bottom: none;
    }
    table td {
        vertical-align: top;
    }

    .report-header .header-left {
      width: 68%;
    }
    .report-header .header-right {
      width: 32%;
    }
    .report-header .information {
      width: calc(100% - 110px);
    }
    .information-table tr:last-child td:first-child {
      border-bottom-left-radius: 10px;
    }
    .information-table tr:last-child td:last-child {
        border-bottom-right-radius: 10px;
    }
    .bg-gray {
      background-color: #EAEAEA;
    }
    .radius-5 {
      border-radius: 5px;
    }

    </style>
        </head>
    <body onload="window.print()">${printContents}</body>
      </html>`
    );
    popupWin.document.close();
    document.getElementById('printSectionId').className = 'hidden';

    return;
  }

  // For destroy the isLoading subject.
  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }

}
