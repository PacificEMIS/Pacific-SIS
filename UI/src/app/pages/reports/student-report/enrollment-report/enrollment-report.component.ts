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

import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import icListAlt from '@iconify/icons-ic/twotone-list-alt';
import icSchool from '@iconify/icons-ic/twotone-school';
import { CommonService } from 'src/app/services/common.service';
import { MatTableDataSource } from '@angular/material/table';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FormControl } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatCheckbox } from '@angular/material/checkbox';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { LoaderService } from 'src/app/services/loader.service';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { fadeInRight400ms } from 'src/@vex/animations/fade-in-right.animation';
import { ReportService } from '../../../../services/report.service'
import { GetStudentEnrollmentReportModel } from 'src/app/models/report.model';
import { Subject } from 'rxjs';
import * as moment from 'moment';
import { MarkingPeriodListModel } from 'src/app/models/marking-period.model';
import { MarkingPeriodService } from 'src/app/services/marking-period.service';
import icPrint from '@iconify/icons-ic/twotone-print';
import { GetAllGradeLevelsModel } from 'src/app/models/grade-level.model';
import { GradeLevelService } from 'src/app/services/grade-level.service';
import { ExcelService } from "../../../../services/excel.service";
import { SharedFunction } from 'src/app/pages/shared/shared-function';
import { StudentInfoReportModel } from 'src/app/models/student-info-report.model';
import { StudentReportService } from 'src/app/services/student-report.service';

@Component({
  selector: 'vex-enrollment-report',
  templateUrl: './enrollment-report.component.html',
  styleUrls: ['./enrollment-report.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms,
    fadeInRight400ms
  ]
})
export class EnrollmentReportComponent implements OnInit, AfterViewInit {
  destroySubject$: Subject<void> = new Subject();
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild('masterCheckBox') masterCheckBox: MatCheckbox;
  markingPeriodListModel: MarkingPeriodListModel = new MarkingPeriodListModel();
  getAllGradeLevelsModel: GetAllGradeLevelsModel = new GetAllGradeLevelsModel();
  studentInfoReportModel: GetStudentEnrollmentReportModel = new GetStudentEnrollmentReportModel();
  studentEnrollmentInfoReportModel: StudentInfoReportModel = new StudentInfoReportModel();
  studentModelList: MatTableDataSource<any>;
  searchCtrl = new FormControl();
  icPrint = icPrint;
  icListAlt = icListAlt;
  icSchool = icSchool;
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  enrollmentData: any;
  loading: boolean;
  showAdvanceSearchPanel: boolean = false;
  searchValue;
  toggleValues;
  gradeLevelList = [];
  allPDFValues = []
  date = new Date();
  selectedReportBy: any;
  gradeLevels: any;
  dataForPdf: any
  toDay = new Date();
  generatedReportCardData;
  globalMarkingPeriodEndDate;
  globalMarkingPeriodStartDate;
  pageSource = 'Enrollment_Report'
  tempGradeLevel = '';
  isVisible: boolean = false;
  selectOptions: any = [
    {
      title: 'This School Year',
      id: 'school_year'
    },
    {
      title: 'This Month',
      id: 'this_month'
    },
  ]
  displayedColumns: string[] = ['studentName', 'studentId', 'alternateId', 'phone', 'gradeLevel', 'section', 'enrollmentDate', 'status'];
  columns = [
    // { label: '', property: 'studentCheck', type: 'text', visible: true },
    { label: 'Name', property: 'studentName', type: 'text', visible: true },
    { label: 'Student ID', property: 'studentId', type: 'text', visible: true },
    { label: 'Alternate ID', property: 'alternateId', type: 'text', visible: true },
    { label: 'Telephone', property: 'phone', type: 'text', visible: true },
    { label: 'Grade Level', property: 'gradeLevel', type: 'text', visible: true },
    { label: 'Section', property: 'section', type: 'text', visible: true },
    { label: 'Enrollment Date', property: 'enrollmentDate', type: 'text', visible: true },
    { label: 'Status', property: 'status', type: 'text', visible: false },
  ];

  constructor(public translateService: TranslateService,
    private commonService: CommonService,
    private snackbar: MatSnackBar,
    public defaultValuesService: DefaultValuesService,
    private loaderService: LoaderService,
    private reportService: ReportService,
    private markingPeriodService: MarkingPeriodService,
    private gradeLevelService: GradeLevelService,
    private excelService: ExcelService,
    private commonFunction: SharedFunction,
    private studentReportService: StudentReportService
  ) {
    this.defaultValuesService.setReportCompoentTitle.next("Student Enrollment Report")
    // translateService.use("en");
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });
    this.selectOptions[0].startDate = this.defaultValuesService.getFullYearStartDate();
    this.selectOptions[0].endDate = this.defaultValuesService.getFullYearEndDate();

    this.selectOptions[1].startDate = moment().startOf('month').format('YYYY-MM-DD');
    this.selectOptions[1].endDate = moment().endOf('month').format('YYYY-MM-DD');
    this.selectedReportBy = this.defaultValuesService.getMarkingPeriodTitle();

    this.studentInfoReportModel.markingPeriodStartDate = this.globalMarkingPeriodStartDate = this.commonFunction.formatDateSaveWithoutTime(this.defaultValuesService.getMarkingPeriodStartDate());
    this.studentInfoReportModel.markingPeriodEndDate = this.globalMarkingPeriodEndDate = this.commonFunction.formatDateSaveWithoutTime(this.defaultValuesService.getMarkingPeriodEndDate());
    this.getMarkingPeriod();
    this.getAllGradeLevelList()

  }

  ngOnInit(): void {
    this.studentInfoReportModel.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
    this.searchCtrl = new FormControl();
    this.getStudentEnrollmentWithDate();
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
        Object.assign(this.studentInfoReportModel, { filterParams: filterParams });
        this.studentInfoReportModel.pageNumber = 1;
        this.paginator.pageIndex = 0;
        this.studentInfoReportModel.pageSize = this.pageSize;
        this.getStudentEnrollmentList();
      }
      else {
        Object.assign(this.studentInfoReportModel, { filterParams: null });
        this.studentInfoReportModel.pageNumber = this.paginator.pageIndex + 1;
        this.studentInfoReportModel.pageSize = this.pageSize;
        this.getStudentEnrollmentList();
      }
    });
  }

  hideAdvanceSearch(event) {
    this.showAdvanceSearchPanel = false;
  }


  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  getAllGradeLevelList() {
    this.gradeLevelService.getAllGradeLevels(this.getAllGradeLevelsModel).subscribe(data => {
      if (data) {
        if (data._failure) {
          this.commonService.checkTokenValidOrNot(data._message);
          this.gradeLevelList = [];
          if (!data.tableGradelevelList) {
            this.snackbar.open(data._message, '', {
              duration: 1000
            });
          }
        } else {
          this.gradeLevelList = data.tableGradelevelList;
        }
      } else {
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
          duration: 1000
        });
      }

    });
  }

  exportAccessLogListToExcel() {
    this.studentInfoReportModel.pageNumber = 0;
    this.studentInfoReportModel.pageSize = 0
    this.reportService.getStudentEnrollmentReport(this.studentInfoReportModel).subscribe(data => {
      if (data._failure) {
        this.commonService.checkTokenValidOrNot(data._message);
        this.snackbar.open(data._message, '', {
          duration: 10000
        });
      } else {
        if (data.studentListViews.length > 0) {
          this.enrollmentData = data.studentListViews?.map((x) => {
            return {
              [this.defaultValuesService.translateKey("studentName")]: `${x.firstGivenName ? x.firstGivenName : ''} ${x.middleName ? x.middleName : ''} ${x.lastFamilyName ? x.lastFamilyName : ''}`,
              [this.defaultValuesService.translateKey("studentId")]: `${x.studentId ? x.studentId : '-'}`,
              [this.defaultValuesService.translateKey("alternateID")]: `${x.alternateId ? x.alternateId : '-'}`,
              [this.defaultValuesService.translateKey("gradeLevel")]: `${x.gradeLevelTitle ? x.gradeLevelTitle : '-'}`,
              [this.defaultValuesService.translateKey("section")]: `${x.sectionName ? x.sectionName : '-'}`,
              [this.defaultValuesService.translateKey("mobilePhone")]: `${x.mobilePhone ? x.mobilePhone : '-'}`,
            };
          });
          this.excelService.exportAsExcelFile(this.enrollmentData, "Student_Enrollment_Report");
        } else {
          this.snackbar.open(
            "No records found. failed to export Student Enrollment Report",
            "",
            {
              duration: 5000,
            }
          );
        }
      }
    })
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
      Object.assign(this.studentInfoReportModel, { filterParams: filterParams });
    }
    this.studentInfoReportModel.pageNumber = event.pageIndex + 1;
    this.studentInfoReportModel.pageSize = event.pageSize;
    this.defaultValuesService.setPageSize(event.pageSize);
    this.getStudentEnrollmentList();
  }

  /*this is for get all data from the Advanced Search component and then call the api in this page 
  NOTE: we just get ta filterParams Array from Srarch component
  */
  filterData(res) {
    this.studentInfoReportModel.filterParams = []
    this.studentInfoReportModel = new GetStudentEnrollmentReportModel();
    if (res) {
      this.studentInfoReportModel.markingPeriodStartDate = this.globalMarkingPeriodStartDate;
      this.studentInfoReportModel.markingPeriodEndDate = this.globalMarkingPeriodEndDate;
      this.showAdvanceSearchPanel = false;
      this.studentInfoReportModel.filterParams = res.filterParams;
      this.studentInfoReportModel.includeInactive = res.inactiveStudents;
      this.studentInfoReportModel.gradeLevel = this.tempGradeLevel;
      this.getStudentEnrollmentList();
    }
  }

  getSearchInput(event) {
    this.searchValue = event;
  }

  getToggleValues(event) {
    this.toggleValues = event;
    if (this.toggleValues.inactiveStudents === true) {
      this.columns[7].visible = true;
    } else if (this.toggleValues.inactiveStudents === false) {
      this.columns[7].visible = false;
    }
  }
  getReportBy(event) {
    this.studentInfoReportModel.includeInactive = false;
    this.studentInfoReportModel.filterParams = []
    const selectedOption = this.selectOptions.filter(x => x.title === event.value);
    if (event.value) {
      this.studentInfoReportModel.markingPeriodStartDate = this.globalMarkingPeriodStartDate = selectedOption[0].startDate;
      this.studentInfoReportModel.markingPeriodEndDate = this.globalMarkingPeriodEndDate = selectedOption[0].endDate;
    } else {
      this.studentInfoReportModel.markingPeriodStartDate = this.globalMarkingPeriodStartDate = null;
      this.studentInfoReportModel.markingPeriodEndDate = this.globalMarkingPeriodEndDate = null;
    }
  }

  getgrade(event) {
    this.studentInfoReportModel.gradeLevel = event.value
    this.tempGradeLevel = event.value; // for store selected value 
  }

  getStudentEnrollmentList() {
    this.isVisible=true;
    this.dataForPdf = []
    this.studentInfoReportModel.schoolId = this.defaultValuesService.getSchoolID();
    this.reportService.getStudentEnrollmentReport(this.studentInfoReportModel).subscribe(res => {
      if (res._failure) {
        if (res.studentListViews === null) {
          this.studentModelList = new MatTableDataSource([]);
          this.snackbar.open(res._message, '', {
            duration: 10000
          });
        } else {
          this.studentModelList = new MatTableDataSource([]);
          this.totalCount = null;
        }
      } else {
        this.totalCount = res.totalCount;
        this.pageNumber = res.pageNumber;
        this.pageSize = res._pageSize;
        this.studentModelList = new MatTableDataSource(res.studentListViews);
        this.dataForPdf = res
      }
    })
  }

  getStudentEnrollmentWithDate() {
    if (!this.selectedReportBy) {
      this.studentInfoReportModel.markingPeriodStartDate = this.studentInfoReportModel.markingPeriodStartDate ? this.commonFunction.formatDateSaveWithoutTime(this.studentInfoReportModel.markingPeriodStartDate) : null;
      this.studentInfoReportModel.markingPeriodEndDate = this.studentInfoReportModel.markingPeriodEndDate ? this.commonFunction.formatDateSaveWithoutTime(this.studentInfoReportModel.markingPeriodEndDate) : null;
      if (this.studentInfoReportModel.markingPeriodStartDate && this.studentInfoReportModel.markingPeriodEndDate) {
        if (this.studentInfoReportModel.markingPeriodStartDate <= this.studentInfoReportModel.markingPeriodEndDate) {
          this.getStudentEnrollmentList();
        } else {
          this.snackbar.open("To Date value should be greater than From Date value", "", {
            duration: 10000
          });
        }
      } else if (!this.studentInfoReportModel.markingPeriodStartDate && !this.studentInfoReportModel.markingPeriodEndDate) {
        this.snackbar.open("Choose From date and To date", "", {
          duration: 10000
        });
      } else if ((this.studentInfoReportModel.markingPeriodStartDate && !this.studentInfoReportModel.markingPeriodEndDate) || (!this.studentInfoReportModel.markingPeriodStartDate && this.studentInfoReportModel.markingPeriodEndDate)) {
        this.snackbar.open("Choose both From date and To date", "", {
          duration: 10000,
        });
      }
    } else {
      this.getStudentEnrollmentList();
    }
  }

  // For Generate particular student enrollment history
  viewStudentDetails(studentGuid) {
    this.getStudentInfoReport(studentGuid).then((res: any) => {
      res.schoolMasterData.map((item) => {
        item.studentMasterData.map((subItem) => {
          if (subItem.studentEnrollment) {
            subItem.studentEnrollment?.length > 0 ? subItem.studentEnrollment.reverse() : '';
            subItem.studentEnrollment.map((data, index) => {
              if (data.enrollmentCode === "Dropped Out" && subItem.studentEnrollment[index + 1]?.exitCode === "Dropped Out"
                && data.enrollmentDate === subItem.studentEnrollment[index + 1]?.exitDate) {
                subItem.studentEnrollment.splice((index + 1), 1);
              }
            });
          }
        })
      })
      this.generatedReportCardData = res;
      setTimeout(() => {
        this.generateReportForParticularStudent();
      }, 100 * this.generatedReportCardData.schoolMasterData.length);
    });
  }

  // For call getStudentInfoReport API
  getStudentInfoReport(studentGuid) {
    this.studentEnrollmentInfoReportModel.studentGuids = [studentGuid];
    this.studentEnrollmentInfoReportModel.isAddressInfo = false;
    this.studentEnrollmentInfoReportModel.isFamilyInfo = false;
    this.studentEnrollmentInfoReportModel.isMedicalInfo = false;
    this.studentEnrollmentInfoReportModel.isComments = false;

    return new Promise((resolve, reject) => {
      this.studentReportService.getStudentInfoReport(this.studentEnrollmentInfoReportModel).subscribe((res) => {
        if (res._failure) {
          this.commonService.checkTokenValidOrNot(res._message);
          this.snackbar.open(res._message, '', {
            duration: 1000
          });
        } else {
          resolve(res);
        }
      })
    });
  }

  createPDF() {
    this.allPDFValues = this.dataForPdf.studentListViews;
    setTimeout(() => {
      this.generatePDF()
    }, 100 * this.allPDFValues?.length);
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
  generatePDF() {
    let printContents, popupWin;
    printContents = document.getElementById('printSectionId').innerHTML;
    document.getElementById('printSectionId').className = 'block';
    popupWin = window.open('', '_blank', 'top=0,left=0,height=100%,width=auto');
    if(popupWin === null || typeof(popupWin)==='undefined'){
      document.getElementById('printSectionId').className = 'hidden';
      this.snackbar.open("User needs to allow the popup from the browser", '', {
        duration: 10000
      });
    } else {
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
  </style>
      </head>
  <body onload="window.print()">${printContents}</body>
    </html>`
    );
    popupWin.document.close();
    document.getElementById('printSectionId').className = 'hidden';
    return;
    }
  }

  generateReportForParticularStudent() {
    let printContents, popupWin;
    printContents = document.getElementById('printReportCardId').innerHTML;
    document.getElementById('printReportCardId').className = 'block';
    popupWin = window.open('', '_blank', 'top=0,left=0,height=100%,width=auto');
    if (popupWin === null || typeof (popupWin) === 'undefined') {
      document.getElementById('printReportCardId').className = 'hidden';
      this.snackbar.open("User needs to allow the popup from the browser", '', {
        duration: 10000
      });
    } else {
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
          }
          table {
            border-collapse: collapse;
            width: 100%;
          }
          .student-information-report {
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
              padding: 20px;
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
          .student-logo {
              padding: 20px;
          }
          .student-logo div {
              width: 100%;
              height: 100%;
              border: 1px solid rgb(136, 136, 136);
              border-radius: 3px;
          }
          .student-logo img {
              width: 100%;
          }
          .student-details {
              padding: 20px;
              vertical-align: top;
          }
          .student-details h4 {
              font-size: 22px;
              font-weight: 600;
              margin-bottom: 10px;
          }
          .student-details span {
              color: #817e7e;
              padding: 0 15px;
              font-size: 20px;
          }
          .student-details p {
              color: #121212;
              font-size: 16px;
          }
          .student-details table {
              border-collapse: separate;
              border-spacing:0;
              border-radius: 10px;
          }
          .student-details table td {
              border-left: 1px solid #000;
              border-bottom: 1px solid #000;
              padding: 8px 10px;
          }
          .student-details table td b, .student-details table span {
              color: #000;
              font-size: 16px;
          }
          .student-details table td b {
              font-weight: 600;
          }
          .student-details table td:first-child {
              border-left: none;
          }
          .student-details table tr:last-child td {
              border-bottom: none;
          }
          .card {
              border-radius: 5px;
              padding: 20px;
              box-shadow: none;
              display: flex;
          }
          .p-20 {
              padding: 20px;
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
          .bg-black {
              background-color: #000;
          }
          .rounded-3 {
              border-radius: 3px;
          }
          .text-white {
              color: #fff;
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
          .information-table td {
              font-size: 16px;
          }
          .information-table tr:first-child td:first-child {
              border-top-left-radius: 10px;
          }
          .information-table tr:first-child td:last-child {
              border-top-right-radius: 10px;
          }
          table td {
              vertical-align: top;
          }

          .report-header .header-left {
            width: 65%;
          }
          .report-header .header-right {
            width: 35%;
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
          .information-table tr:first-child th:first-child {
            border-top-left-radius: 10px;
            border-bottom-left-radius: 10px;
          }
          .information-table tr:first-child th:last-child {
            border-top-right-radius: 10px;
            border-bottom-right-radius: 10px;
          }
          .address-information td:first-child, .address-information td:last-child {
            width: 49%;
          }
          .address-information td:nth-child(2) {
            width: 2%;
          }
          .family-information td:first-child, .family-information td:nth-child(3) {
            width: 49%;
          }
          .family-information td, .address-information td{
            border-radius: 5px;
          }
          .family-information td:nth-child(2) {
            width: 2%;
          }
          .family-information tr:last-child td {
            border-bottom: none;
          }

    </style>
        </head>
    <body onload="window.print()">${printContents}</body>
      </html>`
      );
      popupWin.document.close();
      document.getElementById('printReportCardId').className = 'hidden';
      return;
    }
  }


  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }
}
