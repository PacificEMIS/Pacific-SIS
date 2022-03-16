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
import { CommonService } from 'src/app/services/common.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { SharedFunction } from 'src/app/pages/shared/shared-function';
import { LoaderService } from 'src/app/services/loader.service';
import { ExcelService } from 'src/app/services/excel.service';
import { GetScheduledAddDropReportModel, filterParams } from 'src/app/models/report.model';
import { MatTableDataSource } from '@angular/material/table';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { FormControl } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import * as moment from 'moment';
import { MatCheckbox } from '@angular/material/checkbox';
import { MarkingPeriodService } from 'src/app/services/marking-period.service';
import { MarkingPeriodListModel } from 'src/app/models/marking-period.model';

@Component({
  selector: 'vex-add-drop-report',
  templateUrl: './add-drop-report.component.html',
  styleUrls: ['./add-drop-report.component.scss']
})
export class AddDropReportComponent implements OnInit, AfterViewInit, OnDestroy {
  icPrint = icPrint;
  displayedColumns: string[] = ['studentCheck', 'studentName', 'studentId', 'course', 'courseSectionTeacher', 'enrolledDate', 'dropDate'];
  getScheduledAddDropReportModel: GetScheduledAddDropReportModel = new GetScheduledAddDropReportModel();
  addDropScheduleList: MatTableDataSource<any>;
  scheduleListForPDF;
  addDropScheduleListForPDF;
  listOfStudents = [];
  selectedStudents = [];
  totalCount: number = 0;
  pageNumber: number;
  pageSize: number;
  today: Date = new Date();
  destroySubject$: Subject<void> = new Subject();
  loading: boolean;
  searchCtrl: FormControl;
  filterParams: filterParams[] = [];
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild('masterCheckBox') masterCheckBox: MatCheckbox;
  markingPeriodListModel: MarkingPeriodListModel = new MarkingPeriodListModel();

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
    this.getScheduledAddDropReportModel.markingPeriodStartDate = this.commonFunction.formatDateSaveWithoutTime(this.defaultValuesService.getMarkingPeriodStartDate());
    this.getScheduledAddDropReportModel.markingPeriodEndDate = this.commonFunction.formatDateSaveWithoutTime(this.defaultValuesService.getMarkingPeriodEndDate());
    this.getMarkingPeriod();
  }

  ngOnInit(): void {
    this.getScheduledAddDropReportModel.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
    this.searchCtrl = new FormControl();
    this.generateReportScheduleList();
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
        Object.assign(this.getScheduledAddDropReportModel, { filterParams: filterParams });
        this.filterParams = filterParams;
        this.getScheduledAddDropReportModel.pageNumber = 1;
        this.paginator.pageIndex = 0;
        this.getScheduledAddDropReportModel.pageSize = this.pageSize;
        this.getScheduledAddDropReport();
      }
      else {
        Object.assign(this.getScheduledAddDropReportModel, { filterParams: null });
        this.filterParams = null;
        this.getScheduledAddDropReportModel.pageNumber = this.paginator.pageIndex + 1;
        this.getScheduledAddDropReportModel.pageSize = this.pageSize;
        this.getScheduledAddDropReport();
      }
    });
  }

  // For searching add drop schedule list by date range
  generateReportScheduleList() {
    if (!this.selectedReportBy) {
      this.getScheduledAddDropReportModel.markingPeriodStartDate = this.getScheduledAddDropReportModel.markingPeriodStartDate ? this.commonFunction.formatDateSaveWithoutTime(this.getScheduledAddDropReportModel.markingPeriodStartDate) : null;
      this.getScheduledAddDropReportModel.markingPeriodEndDate = this.getScheduledAddDropReportModel.markingPeriodEndDate ? this.commonFunction.formatDateSaveWithoutTime(this.getScheduledAddDropReportModel.markingPeriodEndDate) : null;
      if (this.getScheduledAddDropReportModel.markingPeriodStartDate && this.getScheduledAddDropReportModel.markingPeriodEndDate) {
        if (this.getScheduledAddDropReportModel.markingPeriodStartDate <= this.getScheduledAddDropReportModel.markingPeriodEndDate) {
          this.getScheduledAddDropReport();
        } else {
          this.snackbar.open("To Date value should be greater than From Date value", "", {
            duration: 10000
          });
        }
      } else if (!this.getScheduledAddDropReportModel.markingPeriodStartDate && !this.getScheduledAddDropReportModel.markingPeriodEndDate) {
        this.snackbar.open("Choose From date and To date", "", {
          duration: 10000
        });
      } else if ((this.getScheduledAddDropReportModel.markingPeriodStartDate && !this.getScheduledAddDropReportModel.markingPeriodEndDate) || (!this.getScheduledAddDropReportModel.markingPeriodStartDate && this.getScheduledAddDropReportModel.markingPeriodEndDate)) {
        this.snackbar.open("Choose both From date and To date", "", {
          duration: 10000,
        });
      }
    } else {
      this.getScheduledAddDropReport();
    }
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
      this.getScheduledAddDropReportModel.markingPeriodStartDate = this.commonFunction.formatDateSaveWithoutTime(selectedOption[0].startDate);
      this.getScheduledAddDropReportModel.markingPeriodEndDate = this.commonFunction.formatDateSaveWithoutTime(selectedOption[0].endDate);
    } else {
      this.getScheduledAddDropReportModel.markingPeriodStartDate = null;
      this.getScheduledAddDropReportModel.markingPeriodEndDate = null;
    }
  }

  // For format the date
  formatDate(date) {
    return date ? moment(date).format('MMM DD, YYYY') : null;
  }

  // For get all data from API
  getScheduledAddDropReport() {
    this.reportService.getScheduledAddDropReport(this.getScheduledAddDropReportModel).subscribe(data => {
      if (data) {
        if (data._failure) {
          this.commonService.checkTokenValidOrNot(data._message);
          if (data.studentCoursesectionScheduleList === null) {
            this.addDropScheduleList = new MatTableDataSource([]);
            this.totalCount = null;
            this.snackbar.open('' + data._message, '', {
              duration: 10000
            });
          } else {
            this.addDropScheduleList = new MatTableDataSource([]);
            this.totalCount = null;
          }
        } else {
          this.totalCount = data.totalCount;
          this.pageNumber = data.pageNumber;
          this.pageSize = data._pageSize;
          this.scheduleListForPDF = data;
          this.addDropScheduleListForPDF = data.studentCoursesectionScheduleList;
          data.studentCoursesectionScheduleList.forEach((student) => {
            student.checked = false;
          });
          this.listOfStudents = data.studentCoursesectionScheduleList.map((item) => {
            this.selectedStudents.map((selectedUser) => {
              if (item.schoolId === selectedUser.schoolId && item.courseId === selectedUser.courseId && item.courseSectionId === selectedUser.courseSectionId && item.studentGuid === selectedUser.studentGuid) {
                item.checked = true;
                return item;
              }
            });
            return item;
          });
          this.masterCheckBox.checked = this.listOfStudents.every((item) => {
            return item.checked;
          });
          this.addDropScheduleList = new MatTableDataSource(data.studentCoursesectionScheduleList);
        }
      } else {
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    });
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
      Object.assign(this.getScheduledAddDropReportModel, { filterParams: filterParams });
      this.filterParams = filterParams;
    }
    this.getScheduledAddDropReportModel.pageNumber = event.pageIndex + 1;
    this.getScheduledAddDropReportModel.pageSize = event.pageSize;
    this.defaultValuesService.setPageSize(event.pageSize);
    this.getScheduledAddDropReport();
  }

  // For export add drop schedule list to excel
  exportAddDropScheduleListToExcel() {
    if (!this.totalCount) {
      this.snackbar.open('No Record Found. Failed to Export Students List', '', {
        duration: 5000
      });
      return;
    }
    let getScheduledAddDropReportModelForExcel: GetScheduledAddDropReportModel = new GetScheduledAddDropReportModel();
    getScheduledAddDropReportModelForExcel.pageNumber = 0;
    getScheduledAddDropReportModelForExcel.pageSize = 0;
    getScheduledAddDropReportModelForExcel.markingPeriodStartDate = this.getScheduledAddDropReportModel.markingPeriodStartDate;
    getScheduledAddDropReportModelForExcel.markingPeriodEndDate = this.getScheduledAddDropReportModel.markingPeriodEndDate;
    getScheduledAddDropReportModelForExcel.filterParams = this.filterParams;
    this.reportService.getScheduledAddDropReport(getScheduledAddDropReportModelForExcel).subscribe(data => {
      if (data) {
        if (data._failure) {
          this.commonService.checkTokenValidOrNot(data._message);
          this.snackbar.open(data._message, '', {
            duration: 10000
          });
        } else {
          if (data.studentCoursesectionScheduleList.length > 0) {
            let scheduleList;
            scheduleList = data.studentCoursesectionScheduleList.map((item) => {
              const staffName = item.staffName ? ' - ' + item.staffName : '';
              return {
                [this.defaultValuesService.translateKey('studentName')]: item.studentName,
                [this.defaultValuesService.translateKey('studentId')]: item.studentInternalId,
                [this.defaultValuesService.translateKey('course')]: item.courseName,
                [this.defaultValuesService.translateKey('courseSectionTeacher')]: item.courseSectionName + staffName,
                [this.defaultValuesService.translateKey('enrolledDate')]: this.formatDate(item.enrolledDate),
                [this.defaultValuesService.translateKey('dropDate')]: this.formatDate(item.dropDate)
              };
            });
            this.excelService.exportAsExcelFile(scheduleList, 'Schedule_Add_Drop Report');
          } else {
            this.snackbar.open('No Records Found. Failed to Export Students List', '', {
              duration: 5000
            });
          }
        }
      }
    });
  }

  // For print add drop schedule List
  printAddDropScheduleList() {
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
    }, 100 * this.addDropScheduleListForPDF.length);
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
    </style>
        </head>
    <body onload="window.print()">${printContents}</body>
      </html>`
    );
    popupWin.document.close();
    document.getElementById('printSectionId').className = 'hidden';

    return;
  }

  someComplete(): boolean {
    let indetermine = false;
    for (let user of this.listOfStudents) {
      for (let selectedUser of this.selectedStudents) {
        if (user.schoolId === selectedUser.schoolId && user.courseId === selectedUser.courseId && user.courseSectionId === selectedUser.courseSectionId && user.studentGuid === selectedUser.studentGuid) {
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
    this.addDropScheduleList = new MatTableDataSource(this.listOfStudents);
    this.decideCheckUncheck();
  }

  onChangeSelection(eventStatus: boolean, schoolId, courseId, courseSectionId, studentGuid) {
    for (let item of this.listOfStudents) {
      if (item.schoolId === schoolId && item.courseId === courseId && item.courseSectionId === courseSectionId && item.studentGuid === studentGuid) {
        item.checked = eventStatus;
        break;
      }
    }
    this.addDropScheduleList = new MatTableDataSource(this.listOfStudents);
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
          if (item.schoolId === selectedUser.schoolId && item.courseId === selectedUser.courseId && item.courseSectionId === selectedUser.courseSectionId && item.studentGuid === selectedUser.studentGuid) {
            isIdIncludesInSelectedList = true;
            break;
          }
        }
        if (!isIdIncludesInSelectedList) {
          this.selectedStudents.push(item);
        }
      }
      //  else {
      //   for (let selectedUser of this.selectedStudents) {
      //     if (item.schoolId === selectedUser.schoolId && item.courseId === selectedUser.courseId && item.courseSectionId === selectedUser.courseSectionId && item.studentGuid === selectedUser.studentGuid) {
      //       // this.selectedStudents = this.selectedStudents.filter((user) => user.schoolId !== item.schoolId && user.courseId !== item.courseId && user.courseSectionId !== item.courseSectionId && user.studentGuid !== item.studentGuid);
      //       // break;
      //     }
      //   }
      // }
      isIdIncludesInSelectedList = false;

    });
    this.selectedStudents = this.selectedStudents.filter((item) => item.checked);
  }

  // For generate add drop report for selected students
  createAddDropReportForSelectedStudents() {
    if (!this.selectedStudents?.length) {
      this.snackbar.open('Select at least one student.', '', {
        duration: 3000
      });
      return;
    }
    this.loading = true;
    setTimeout(() => {
      this.generatePDFForSelectedStudents();
      this.loading = false;
    }, 100 * this.selectedStudents.length);
  }

  // For open the print window
  generatePDFForSelectedStudents() {
    let printContents, popupWin;
    printContents = document.getElementById('printSectionForSelectedStudentId').innerHTML;
    document.getElementById('printSectionForSelectedStudentId').className = 'block';
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
          </style>
        </head>
    <body onload="window.print()">${printContents}</body>
      </html>`
    );
    popupWin.document.close();
    document.getElementById('printSectionForSelectedStudentId').className = 'hidden';

    return;
  }

  // For destroy the isLoading subject.
  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }

}
