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
import { FormControl } from '@angular/forms';
import { MatCheckbox } from '@angular/material/checkbox';
import { MatPaginator, MatPaginatorIntl } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableDataSource } from '@angular/material/table';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { filterParams, GetHonorRollReportModel } from 'src/app/models/report.model';
import { SharedFunction } from 'src/app/pages/shared/shared-function';
import { CommonService } from 'src/app/services/common.service';
import { ExcelService } from 'src/app/services/excel.service';
import { LoaderService } from 'src/app/services/loader.service';
import { ReportService } from 'src/app/services/report.service';
import { fadeInRight400ms } from 'src/@vex/animations/fade-in-right.animation';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { DomSanitizer } from '@angular/platform-browser';
import { BackGroundImageEnum } from 'src/app/enums/bg-image.enum';

@Component({
  selector: 'vex-honor-roll',
  templateUrl: './honor-roll.component.html',
  styleUrls: ['./honor-roll.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms,
    fadeInRight400ms
  ]
})
export class HonorRollComponent implements OnInit, AfterViewInit, OnDestroy {
  // displayedColumns: string[] = ['studentCheck', 'studentName', 'studentId', 'alternateId', 'grade', 'section', 'phone', 'honorRoll'];
  columns = [
    { label: '', property: 'studentCheck', type: 'text', visible: true },
    { label: 'Name', property: 'studentName', type: 'text', visible: true },
    { label: 'Student ID', property: 'studentId', type: 'text', visible: true },
    { label: 'Alternate ID', property: 'alternateId', type: 'text', visible: true },
    { label: 'Grade Level', property: 'grade', type: 'text', visible: true },
    { label: 'Section', property: 'section', type: 'text', visible: true },
    { label: 'Telephone', property: 'phone', type: 'text', visible: true },
    { label: 'Honor Roll Name', property: 'honorRoll', type: 'text', visible: true },
    { label: 'Status', property: 'status', type: 'text', visible: false }
  ];
  studentLists: MatTableDataSource<any>;
  getHonorRollReportModel: GetHonorRollReportModel = new GetHonorRollReportModel();
  isCertificateHeader: boolean = false;
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  listOfStudents = [];
  selectedStudents = [];
  honorRollListForPDF;
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild('masterCheckBox') masterCheckBox: MatCheckbox;
  destroySubject$: Subject<void> = new Subject();
  loading: boolean;
  searchCtrl: FormControl;
  filterParams: filterParams[] = [];
  showAdvanceSearchPanel: boolean = false;
  searchCount;
  searchValue;
  toggleValues;
  pageSource = 'Honor_Roll_Report';
  backgroundImage: any;
  today: Date = new Date();
  schoolYear;
  markingPeriodStartDate;
  markingPeriodEndDate;

  constructor(public translateService: TranslateService,
    private paginatorObj: MatPaginatorIntl,
    private defaultValuesService: DefaultValuesService,
    private reportService: ReportService,
    private commonService: CommonService,
    private snackbar: MatSnackBar,
    private commonFunction: SharedFunction,
    private loaderService: LoaderService,
    private domSanitizer: DomSanitizer,
    private excelService: ExcelService
  ) {
    paginatorObj.itemsPerPageLabel = translateService.instant('itemsPerPage');
    this.defaultValuesService.setReportCompoentTitle.next("Honor Roll");

    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });

    this.schoolYear = this.defaultValuesService.getFullAcademicYear();
    this.markingPeriodStartDate = this.defaultValuesService.getMarkingPeriodStartDate();
    this.markingPeriodEndDate = this.defaultValuesService.getMarkingPeriodEndDate();

    this.backgroundImage = BackGroundImageEnum.backgroundImage;
  }

  ngOnInit(): void {
    this.getHonorRollReportModel.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
    this.searchCtrl = new FormControl();
    this.getHonorRollReport();
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
        Object.assign(this.getHonorRollReportModel, { filterParams: filterParams });
        this.filterParams = filterParams;
        this.getHonorRollReportModel.pageNumber = 1;
        this.paginator.pageIndex = 0;
        this.getHonorRollReportModel.pageSize = this.pageSize;
        this.getHonorRollReport();
      }
      else {
        Object.assign(this.getHonorRollReportModel, { filterParams: null });
        this.filterParams = null;
        this.getHonorRollReportModel.pageNumber = this.paginator.pageIndex + 1;
        this.getHonorRollReportModel.pageSize = this.pageSize;
        this.getHonorRollReport();
      }
    });
  }

  // For get all honor roll student list
  getHonorRollReport() {
    this.reportService.getHonorRollReport(this.getHonorRollReportModel).subscribe(data => {
      if (data) {
        if (data._failure) {
          this.commonService.checkTokenValidOrNot(data._message);
          if (data.honorRollViewForReports === null) {
            this.studentLists = new MatTableDataSource([]);
            this.totalCount = null;
            this.snackbar.open('' + data._message, '', {
              duration: 10000
            });
          } else {
            this.studentLists = new MatTableDataSource([]);
            this.totalCount = null;
          }
        } else {
          this.totalCount = data.totalCount;
          this.pageNumber = data.pageNumber;
          this.pageSize = data.pageSize;
          this.honorRollListForPDF = data;
          data.honorRollViewForReports.forEach((student) => {
            student.checked = false;
          });
          this.listOfStudents = data.honorRollViewForReports.map((item) => {
            this.selectedStudents.map((selectedUser) => {
              if (item.studentGuid === selectedUser.studentGuid) {
                item.checked = true;
                return item;
              }
            });
            return item;
          });
          this.masterCheckBox.checked = this.listOfStudents.every((item) => {
            return item.checked;
          });
          this.studentLists = new MatTableDataSource(data.honorRollViewForReports);
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
      Object.assign(this.getHonorRollReportModel, { filterParams: filterParams });
      this.filterParams = filterParams;
    }
    this.getHonorRollReportModel.pageNumber = event.pageIndex + 1;
    this.getHonorRollReportModel.pageSize = event.pageSize;
    this.defaultValuesService.setPageSize(event.pageSize);
    this.getHonorRollReport();
  }

  someComplete(): boolean {
    let indetermine = false;
    for (let user of this.listOfStudents) {
      for (let selectedUser of this.selectedStudents) {
        if (user.studentGuid === selectedUser.studentGuid) {
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
    this.studentLists = new MatTableDataSource(this.listOfStudents);
    this.decideCheckUncheck();
  }

  onChangeSelection(eventStatus: boolean, studentGuid) {
    for (let item of this.listOfStudents) {
      if (item.studentGuid === studentGuid) {
        item.checked = eventStatus;
        break;
      }
    }
    this.studentLists = new MatTableDataSource(this.listOfStudents);
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
          if (item.studentGuid === selectedUser.studentGuid) {
            isIdIncludesInSelectedList = true;
            break;
          }
        }
        if (!isIdIncludesInSelectedList) {
          if (item.honorRoll !== null) this.selectedStudents.push(item);
        }
      }
      else {
        for (let selectedUser of this.selectedStudents) {
          if (item.studentGuid === selectedUser.studentGuid) {
            this.selectedStudents = this.selectedStudents.filter((user) => user.studentGuid !== item.studentGuid);
            break;
          }
        }
      }
      isIdIncludesInSelectedList = false;
    });
    this.selectedStudents = this.selectedStudents.filter((item) => item.checked);
  }

  getSearchResult(res) {
    this.getHonorRollReportModel = new GetHonorRollReportModel();
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
    if (res && res.honorRollViewForReports) {
      res?.honorRollViewForReports?.forEach((student) => {
        student.checked = false;
      });
      this.listOfStudents = res.honorRollViewForReports.map((item) => {
        this.selectedStudents.map((selectedUser) => {
          if (item.studentGuid === selectedUser.studentGuid) {
            item.checked = true;
            return item;
          }
        });
        return item;
      });

      this.masterCheckBox.checked = this.listOfStudents.every((item) => {
        return item.checked;
      })
    }
    this.studentLists = new MatTableDataSource(res?.honorRollViewForReports);
  }

  getToggleValues(event) {
    this.toggleValues = event;
    if (event.inactiveStudents === true) {
      this.columns[8].visible = true;
    } else if (event.inactiveStudents === false) {
      this.columns[8].visible = false;
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

  // For export honor roll list to excel
  exportHonorRollListToExcel() {
    if (!this.totalCount) {
      this.snackbar.open('No Record Found. Failed to Export Honor Roll List', '', {
        duration: 5000
      });
      return;
    }
    let getHonorRollReportModelForExcel: GetHonorRollReportModel = new GetHonorRollReportModel();
    getHonorRollReportModelForExcel.pageNumber = 0;
    getHonorRollReportModelForExcel.pageSize = 0;
    getHonorRollReportModelForExcel.filterParams = this.filterParams;
    this.reportService.getHonorRollReport(getHonorRollReportModelForExcel).subscribe(data => {
      if (data) {
        if (data._failure) {
          this.commonService.checkTokenValidOrNot(data._message);
          this.snackbar.open(data._message, '', {
            duration: 10000
          });
        } else {
          if (data.honorRollViewForReports.length > 0) {
            let honorRollList;
            honorRollList = data.honorRollViewForReports.map((item) => {
              const middleName = item.middleName ? ' ' + item.middleName + ' ' : ' ';
              return {
                [this.defaultValuesService.translateKey('studentName')]: item.firstGivenName + middleName + item.lastFamilyName,
                [this.defaultValuesService.translateKey('studentId')]: item.studentInternalId,
                [this.defaultValuesService.translateKey('alternateId')]: item.alternateId,
                [this.defaultValuesService.translateKey('grade')]: item.gradeName,
                [this.defaultValuesService.translateKey('section')]: item.sectionName,
                [this.defaultValuesService.translateKey('phone')]: item.homePhone,
                [this.defaultValuesService.translateKey('honorRoll')]: item.honorRoll
              };
            });
            this.excelService.exportAsExcelFile(honorRollList, 'Honor_Roll Report');
          } else {
            this.snackbar.open('No Records Found. Failed to Honor Roll List', '', {
              duration: 5000
            });
          }
        }
      }
    });
  }

  // For generate honor roll report for selected students
  printHonorRollStudentList() {
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
          @import url('https://fonts.googleapis.com/css2?family=DM+Serif+Display:ital@0;1&display=swap');

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
                      /* font-family: 'DM Serif Display', serif; */
                      background-color: #fff;
                  }
          
                  table {
                      border-collapse: collapse;
                      width: 100%;
                      font-family: Arial, Helvetica, sans-serif;
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
          
                  .clearfix::after {
                      display: block;
                      clear: both;
                      content: "";
                  }
          
                  .p-20 {
                      padding: 20px;
                  }
          
                  .p-10 {
                      padding: 10px;
                  }
          
                  .p-y-40 {
                      padding-top: 40px;
                      padding-bottom: 40px;
                  }
          
                  .f-s-40 {
                      font-size: 40px;
                  }
          
                  .f-s-28 {
                      font-size: 28px;
                  }
          
                  .f-s-26 {
                      font-size: 26px;
                  }
          
                  .f-s-24 {
                      font-size: 24px;
                  }
          
                  .f-s-14 {
                      font-size: 14px;
                  }
          
                  .student-information-report {
                      width: 1024px;
                      margin: auto;
                  }
          
                  .honor-roll-bg {
                      height: 570px;
                      vertical-align: top;
                      background-size: contain;
                      background-position: center center;
                      background-repeat: no-repeat;
                  }
                  .honor-roll-bg h1 {
                      color: #254C86;
                      font-size: 50px;
                      font-weight: normal;
                      margin: 10px 0 75px 0;
                      font-family: 'DM Serif Display', serif;
                      position: relative;
                      display: inline-block;
                  }
                  .honor-roll-bg h1:after {
                      content: '';
                      position: absolute;
                      display: inline-block;
                      width: 100%;
                      left: 0;
                      bottom: 0;
                      border-bottom: 3px solid #254C86;
                  }
                  .honor-roll-bg h4 {
                      font-family: 'DM Serif Display', serif;
                      margin: 35px 0 30px;
                  }
                  .bullet {
                      width: 8px;
                      height: 8px;
                      background: #000;
                      display: inline-block;
                      vertical-align: middle;
                      border-radius: 100%;
                      margin: 0 40px;
                  }
                  .sig td {
                      width: 33.33%;
                  }
                  .sig td:first-child {
                     padding: 50px 0 20px 100px;
                  }
                  .sig td:last-child {
                     padding: 50px 100px 20px 0;
                  }
                  .long-line {
                      width: 100%;
                      height: 1px;
                      background: #000;
                  }
          
                  .letter-spacing-1 {
                      letter-spacing: 1px;
                  }
          
                  .letter-spacing-2 {
                      letter-spacing: 2px;
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
          
                  .report-header .information {
                    width: calc(100% - 110px);
                  }
          
                  .report-header .information h4 {
                    font-size: 20px;
                    font-weight: 600;
                    padding: 10px 0;
                }
          
                .report-header .information p, .header-right p {
                  font-size: 16px;
              }
          
              .report-header td {
                padding: 20px 8px 0;
            }
          
            .header-right div {
              background-color: #000;
              color: #fff;
              font-size: 20px;
              padding: 5px 20px;
              font-weight: 600;
              margin-bottom: 8px;
          }

          .p-b-8 {
            padding-bottom: 8px;
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

  // For destroy the isLoading subject.
  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }

}
