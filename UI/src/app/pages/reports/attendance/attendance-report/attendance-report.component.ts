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

import { Component, OnInit, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { GetAllAttendanceCodeModel, GetStudentAttendanceReport } from 'src/app/models/attendance-code.model';
import { SharedFunction } from 'src/app/pages/shared/shared-function';
import { AttendanceCodeService } from 'src/app/services/attendance-code.service';
import { MatPaginator } from "@angular/material/paginator";
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { FormControl } from '@angular/forms';
import { CommonService } from 'src/app/services/common.service';
import { MatTableDataSource } from '@angular/material/table';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ExcelService } from "../../../../services/excel.service";
import { Subject } from 'rxjs';
import { LoaderService } from 'src/app/services/loader.service';
import { GradeLevelService } from 'src/app/services/grade-level.service';
import { GetAllGradeLevelsModel } from 'src/app/models/grade-level.model';
import { MarkingPeriodService } from 'src/app/services/marking-period.service';
import { MarkingPeriodListModel } from 'src/app/models/marking-period.model';
import moment from 'moment';

export interface StudentListData {
  date: string;
  studentName: string;
  studentId: string;
  grade: string;
  periodAttendanceStatus: string;
}



@Component({
  selector: 'vex-attendance-report',
  templateUrl: './attendance-report.component.html',
  styleUrls: ['./attendance-report.component.scss']
})
export class AttendanceReportComponent implements OnInit {
  getStudentAttendanceReportModel: GetStudentAttendanceReport = new GetStudentAttendanceReport();

  displayedColumns: string[] = ['date', 'studentName', 'studentId', 'grade', 'periodAttendanceStatus'];
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  searchCtrl: FormControl;
  totalCount: number = 0;
  pageSize: number;
  allAttendence: MatTableDataSource<any>;
  pageNumber: number;
  getAllAttendanceCodeModel: GetAllAttendanceCodeModel = new GetAllAttendanceCodeModel();
  isVisible: boolean = false;
  destroySubject$: Subject<void> = new Subject();
  loading: boolean;
  selectedReportBy: any;
  globalMarkingPeriodEndDate;
  globalMarkingPeriodStartDate;
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
  tempGradeLevel = '';
  gradeLevelList = [];
  getAllGradeLevelsModel: GetAllGradeLevelsModel = new GetAllGradeLevelsModel();
  markingPeriodListModel: MarkingPeriodListModel = new MarkingPeriodListModel();

  constructor(public translateService: TranslateService,
    public attendanceCodeService: AttendanceCodeService,
    private commonFunction: SharedFunction,
    private commonService: CommonService,
    private snackbar: MatSnackBar,
    public defaultValuesService: DefaultValuesService,
    private loaderService: LoaderService,
    private gradeLevelService: GradeLevelService,
    private markingPeriodService: MarkingPeriodService,
    private excelService: ExcelService,
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
    this.getMarkingPeriod();
    this.getAllGradeLevelList()

  }

  ngOnInit(): void {
    this.getStudentAttendanceReportModel.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
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

  getReportBy(event) {
    const selectedOption = this.selectOptions.filter(x => x.title === event.value);
    if (event.value) {
      this.getStudentAttendanceReportModel.markingPeriodStartDate = this.globalMarkingPeriodStartDate = selectedOption[0].startDate;
      this.getStudentAttendanceReportModel.markingPeriodEndDate = this.globalMarkingPeriodEndDate = selectedOption[0].endDate;
    } else {
      this.getStudentAttendanceReportModel.markingPeriodStartDate = this.globalMarkingPeriodStartDate = null;
      this.getStudentAttendanceReportModel.markingPeriodEndDate = this.globalMarkingPeriodEndDate = null;
    }
  }

  getgrade(event) {
    this.getStudentAttendanceReportModel.gradeLevel = event.value
    this.tempGradeLevel = event.value; // for store selected value 
  }

  onSearch() {
    if (!this.selectedReportBy) {
      this.getStudentAttendanceReportModel.markingPeriodStartDate = this.getStudentAttendanceReportModel.markingPeriodStartDate ? this.commonFunction.formatDateSaveWithoutTime(this.getStudentAttendanceReportModel.markingPeriodStartDate) : null;
      this.getStudentAttendanceReportModel.markingPeriodEndDate = this.getStudentAttendanceReportModel.markingPeriodEndDate ? this.commonFunction.formatDateSaveWithoutTime(this.getStudentAttendanceReportModel.markingPeriodEndDate) : null;
      if (this.getStudentAttendanceReportModel.markingPeriodStartDate && this.getStudentAttendanceReportModel.markingPeriodEndDate) {
        if (this.getStudentAttendanceReportModel.markingPeriodStartDate <= this.getStudentAttendanceReportModel.markingPeriodEndDate) {
          this.getStudentAttendanceReport();
        } else {
          this.snackbar.open("To Date value should be greater than From Date value", "", {
            duration: 10000
          });
        }
      } else if (!this.getStudentAttendanceReportModel.markingPeriodStartDate && !this.getStudentAttendanceReportModel.markingPeriodEndDate) {
        this.snackbar.open("Choose From date and To date", "", {
          duration: 10000
        });
      } else if ((this.getStudentAttendanceReportModel.markingPeriodStartDate && !this.getStudentAttendanceReportModel.markingPeriodEndDate) || (!this.getStudentAttendanceReportModel.markingPeriodStartDate && this.getStudentAttendanceReportModel.markingPeriodEndDate)) {
        this.snackbar.open("Choose both From date and To date", "", {
          duration: 10000,
        });
      }
    } else {
      this.getStudentAttendanceReport();
    }
  }

  callWithoutFilterValue() {
    Object.assign(this.getStudentAttendanceReportModel, { filterParams: null });
    this.getStudentAttendanceReportModel.pageNumber = this.paginator.pageIndex + 1;
    this.getStudentAttendanceReportModel.pageSize = this.pageSize;
    this.getStudentAttendanceReport();
  }

  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  callWithFilterValue(term) {
    let filterParams = [
      {
        columnName: null,
        filterValue: term,
        filterOption: 4
      }
    ]
    Object.assign(this.getStudentAttendanceReportModel, { filterParams: filterParams });
    this.getStudentAttendanceReportModel.pageNumber = 1;
    this.paginator.pageIndex = 0;
    this.getStudentAttendanceReportModel.pageSize = this.pageSize;
    this.getStudentAttendanceReport();
  }

  getStudentAttendanceReport() {
    this.attendanceCodeService.getStudentAttendanceReport(this.getStudentAttendanceReportModel).subscribe((res: any) => {
      if (res) {
        if (res._failure) {
          this.commonService.checkTokenValidOrNot(res._message);
          this.allAttendence = new MatTableDataSource([]);
          this.snackbar.open(res._message, '', {
            duration: 10000
          });
          this.isVisible = false;
        }
        else {
          this.isVisible = true;
          this.totalCount = res.totalCount;
          this.pageNumber = res.pageNumber;
          this.pageSize = res._pageSize;
          res.studendAttendanceAdministrationList.map(x => {
            let List = [];                                              //for blank dataSet array

            x.periodsName.includes(',') ? x.studentAttendanceList = x.periodsName.split(",") : x.studentAttendanceList = x.periodsName;         //for checking the string has ',' or not

            if (Array.isArray(x.studentAttendanceList)) {                //for split by '|'
              for (let elemnt of x.studentAttendanceList) {
                List.push(elemnt.split('|'))
              }
            } else {
              List.push(x.studentAttendanceList.split('|'))
            }

            x.studentAttendanceList = List;                               //for push the custom dataSet
          })
          this.allAttendence = new MatTableDataSource(res.studendAttendanceAdministrationList);
        }
      }
      else {
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
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
      Object.assign(this.getStudentAttendanceReportModel, { filterParams: filterParams });
    }
    this.getStudentAttendanceReportModel.pageNumber = event.pageIndex + 1;
    this.getStudentAttendanceReportModel.pageSize = event.pageSize;
    this.getStudentAttendanceReport();
  }

  exportAccessLogListToExcel() {
    this.getStudentAttendanceReportModel.pageNumber = 0
    this.getStudentAttendanceReportModel.pageSize = 0
    if (this.totalCount > 0) {
      this.attendanceCodeService.getStudentAttendanceExcelReport(this.getStudentAttendanceReportModel).subscribe((res: any) => {
        if (res) {
          if (res._failure) {
            this.commonService.checkTokenValidOrNot(res._message);
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
          else {
            if (res.studentAttendanceReportForExcel.length > 0) {
              let object = {}, studentList = [];
              res.studentAttendanceReportForExcel.map(item => {
                for (let objectVal in item) {
                  Object.assign(object, {
                    [this.defaultValuesService.translateKey(objectVal)]: objectVal === "attendanceDate" ?
                      this.commonFunction.formatDateYearAtLast(item[objectVal]) : item[objectVal] ? item[objectVal] : objectVal === "attendanceDate" ||
                        objectVal === "studentName" || objectVal === "studentInternalId" || objectVal === "gradeLevelTitle" ? '-' : 'X'
                  });
                }
                studentList.push(JSON.parse(JSON.stringify(object)));

              })
              this.excelService.exportAsExcelFile(studentList, 'Student_Attendance_Report');
            }
          }
        }
        else {
          this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
      })
    }
  }

  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }

}
