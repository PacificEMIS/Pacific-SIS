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

import { ConstantPool } from "@angular/compiler";
import { Component, OnInit, ViewChild } from "@angular/core";
import { FormControl } from "@angular/forms";
import { MatDialog } from "@angular/material/dialog";
import { MatPaginator, MatPaginatorIntl } from "@angular/material/paginator";
import { MatSnackBar } from "@angular/material/snack-bar";
import { MatSort } from "@angular/material/sort";
import { MatTableDataSource } from "@angular/material/table";
import icSearch from '@iconify/icons-ic/search';
import { TranslateService } from "@ngx-translate/core";
import { debounceTime, distinctUntilChanged, timeout } from "rxjs/operators";
import { DefaultValuesService } from "../../../common/default-values.service";
import { StudentAttendanceListViewModel, StudentDailyAttendanceListViewModel } from "../../../models/attendance-administrative.model";
import { AttendanceCode, GetAllAttendanceCodeModel } from "../../../models/attendance-code.model";
import { AttendanceCodeService } from "../../../services/attendance-code.service";
import { CommonService } from "../../../services/common.service";
import { LoaderService } from "../../../services/loader.service";
import { StudentAttendanceService } from "../../../services/student-attendance.service";
import { SharedFunction } from "../../shared/shared-function";
import { AddCommentsComponent } from "./add-comments/add-comments.component";
import { StudentAttendanceCommentComponent } from "./student-attendance-comment/student-attendance-comment.component";
import { fadeInUp400ms } from '../../../../@vex/animations/fade-in-up.animation';
import { stagger40ms, stagger60ms } from '../../../../@vex/animations/stagger.animation';
import { fadeInRight400ms } from "../../../../@vex/animations/fade-in-right.animation";
import { AdvancedSearchExpansionModel } from "src/app/models/common.model";

export interface StudentList {
  studentName: string;
  studentId: number;
  grade: string;
  section: string;
  present: string;
  attendance: string;
  comment: string;
}
@Component({
  selector: "vex-administration",
  templateUrl: "./administration.component.html",
  styleUrls: ["./administration.component.scss"],
  animations: [
    stagger60ms,
    fadeInUp400ms,
    stagger40ms,
    fadeInRight400ms
  ]
})
export class AdministrationComponent implements OnInit {
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  parentData;
  icSearch = icSearch;
  displayedColumns: string[] = ['studentName', 'studentId', 'grade', 'section', 'present', 'attendance', 'comment'];
  getAllStudent: StudentAttendanceListViewModel = new StudentAttendanceListViewModel();
  getAllAttendanceCodeModel: GetAllAttendanceCodeModel = new GetAllAttendanceCodeModel();
  studentDailyAttendanceListViewModel: StudentDailyAttendanceListViewModel = new StudentDailyAttendanceListViewModel();
  advancedSearchExpansionModel: AdvancedSearchExpansionModel = new AdvancedSearchExpansionModel();
  attendanceCodeList = [];
  totalCount = 0;
  pageNumber: number;
  pageSize: number;
  loading: boolean = false;
  searchCtrl: FormControl;
  filterJsonParams;
  showAdvanceSearchPanel: boolean = false;
  allStudentlist: MatTableDataSource<any> = new MatTableDataSource<any>();
  searchValue: any;
  toggleValues: any;
  searchCount: number;
  disabledAdvancedSearch: boolean = false;
  attendanceNoToString:string;
  cloneStudentlist;
  isFromAdvancedSearch: boolean = false;
  constructor(
    private dialog: MatDialog,
    private loaderService: LoaderService,
    private snackbar: MatSnackBar,
    private defaultValuesService: DefaultValuesService,
    private commonService: CommonService,
    private commonFunction: SharedFunction,
    private studentAttendanceService: StudentAttendanceService,
    private attendanceCodeService: AttendanceCodeService,
    private defaultValueService: DefaultValuesService,
    private paginatorObj: MatPaginatorIntl,
    private translateService: TranslateService
  ) {
    this.advancedSearchExpansionModel.accessInformation = false;
    this.advancedSearchExpansionModel.searchBirthdays = false;
    this.advancedSearchExpansionModel.enrollmentInformation = false;
    paginatorObj.itemsPerPageLabel = translateService.instant('itemsPerPage');
    this.loaderService.isLoading.subscribe((val) => {
      this.loading = val;
    });
  }

  ngOnInit(): void {
    this.searchCtrl = new FormControl();
    this.getAllAttendanceCode();
    this.studentAttendanceService.isSubmitted.subscribe(res => {
      if (res) this.getAllStudentAttendanceListForAdministration();
    });
  }

  // Get All Attendance Codes
  getAllAttendanceCode() {
    this.getAllAttendanceCodeModel.attendanceCategoryId = 1;
    this.attendanceCodeService.getAllAttendanceCode(this.getAllAttendanceCodeModel).subscribe((res: any) => {
      if (res._failure) {
        this.commonService.checkTokenValidOrNot(res._message);
        if (res.attendanceCodeList === null) {
          this.attendanceCodeList = [];
        } else {
          this.attendanceCodeList = [];
        }
      } else {
        this.attendanceCodeList = res.attendanceCodeList;

      }
    });
  }

  addComments(index) {
    let studentName = this.getAllStudent.studendAttendanceAdministrationList[index].firstGivenName + ' ' + this.getAllStudent.studendAttendanceAdministrationList[index].lastFamilyName
    this.dialog.open(AddCommentsComponent, {
      width: '500px',
      data: { studentName, comments: this.getAllStudent.studendAttendanceAdministrationList[index].attendanceComment }
    }).afterClosed().subscribe((res) => {
      if (!res?.submit) {
        return
      }
      if (this.getAllStudent.studendAttendanceAdministrationList[index].attendanceComment || res.comments?.trim()) {
        this.getAllStudent.studendAttendanceAdministrationList[index].attendanceComment = res.comments.trim();
      }
    });
  }

  openStudentAttendanceComment(element) {
    this.dialog.open(StudentAttendanceCommentComponent, {
      width: '900px',
      data: element
    }).afterClosed().subscribe(res => {
      if (res === true || res === null || res === undefined) {
        this.allStudentlist = new MatTableDataSource(JSON.parse(this.cloneStudentlist));
      }
    })
  }


  getAllStudentList(event) {
    this.disabledAdvancedSearch = true;
    this.getAllStudent.attendanceDate = this.commonFunction.formatDateSaveWithoutTime(event.value);
    this.getAllStudent.attendanceCode= null;
    this.parentData = { attendanceDate: this.getAllStudent.attendanceDate, attendanceCode: this.getAllStudent.attendanceCode }
    this.getAllStudentAttendanceListForAdministration();
  }

  attendanceCodeSelected() {
    if (this.getAllStudent.attendanceDate) {
      this.parentData = { attendanceDate: this.getAllStudent.attendanceDate, attendanceCode: this.getAllStudent.attendanceCode }
      this.getAllStudentAttendanceListForAdministration();
    }
    else {
      this.snackbar.open('Please select Attendance Date', '', {
        duration: 10000
      });
    }
  }

  ngAfterViewInit() {
    this.getAllStudent = new StudentAttendanceListViewModel();
    //  Searching
    this.searchCtrl.valueChanges.pipe(debounceTime(500), distinctUntilChanged()).subscribe((term) => {
      if (term !== '') {
        this.callWithFilterValue(term);
      } else {
        this.callWithoutFilterValue();
      }
    });
  }
  callWithFilterValue(term) {
    const searchValue: string = term.toString();
    const filterParams = [
      {
        columnName: null,
        filterValue: searchValue.trim(),
        filterOption: 1
      }
    ];
    Object.assign(this.getAllStudent, { filterParams });
    this.getAllStudent.pageNumber = 1;
    this.paginator.pageIndex = 0;
    this.getAllStudent.pageSize = this.pageSize;
    this.getAllStudentAttendanceListForAdministration();
  }

  callWithoutFilterValue() {
    Object.assign(this.getAllStudent, { filterParams: null });
    this.getAllStudent.pageNumber = this.paginator.pageIndex + 1;
    this.getAllStudent.pageSize = this.pageSize;
    this.getAllStudentAttendanceListForAdministration();
  }

  /* This is for get all data from the Advanced Search component and then call the API in this page 
  NOTE: We just get the filterParams Array from Search component
  */
  filterData(res) {
    this.isFromAdvancedSearch = true;
    this.getAllStudent = new StudentAttendanceListViewModel();
    this.getAllStudent.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
    if (res) {
      this.getAllStudent.filterParams = res.filterParams;
      this.getAllStudent.attendanceCode = res.attendanceCode;
      this.getAllStudent.attendanceDate = res.attendanceDate;
      this.getAllStudent.includeInactive = res.inactiveStudents;
      this.getAllStudent.searchAllSchool = res.searchAllSchool;
      this.getAllStudentAttendanceListForAdministration();
    }
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
    this.getAllStudent.studendAttendanceAdministrationList = res.studendAttendanceAdministrationList;
    this.pageNumber = res.pageNumber;
    this.pageSize = res._pageSize;
    this.cloneStudentlist = JSON.stringify(res.studendAttendanceAdministrationList);
    this.allStudentlist = new MatTableDataSource(JSON.parse(this.cloneStudentlist));
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

  getPageEvent(event) {

    if (this.searchCtrl.value != null && this.searchCtrl.value.trim() !== '') {
      const filterParams = [
        {
          columnName: null,
          filterValue: this.searchCtrl.value,
          filterOption: 1
        }
      ];
      Object.assign(this.getAllStudent, { filterParams });
    }
    this.getAllStudent.pageNumber = event.pageIndex + 1;
    this.getAllStudent.pageSize = event.pageSize;
    this.getAllStudentAttendanceListForAdministration();
  }

  getAllStudentAttendanceListForAdministration() {
    this.studentAttendanceService.getAllStudentAttendanceListForAdministration(this.getAllStudent).subscribe(
      (res: StudentAttendanceListViewModel) => {
        if (res) {
          if (res._failure) {
            this.commonService.checkTokenValidOrNot(res._message);
            this.allStudentlist = new MatTableDataSource([]);
            this.totalCount = null;
          } else {
            this.getAllStudent.studendAttendanceAdministrationList = res.studendAttendanceAdministrationList;
            this.totalCount = res.totalCount;
            this.pageNumber = res.pageNumber;
            this.pageSize = res._pageSize;
            this.cloneStudentlist = JSON.stringify(res.studendAttendanceAdministrationList);
            this.allStudentlist = new MatTableDataSource(JSON.parse(this.cloneStudentlist));

          }
        }
        else {
        }
      }
    );
  }

  onAttendanceSelected(attendance, element) {
    this.attendanceNoToString=(attendance==1) ? 'Present' : (attendance==2) ? 'Absent' : 'Half Day';
    if (this.studentDailyAttendanceListViewModel.studentDailyAttendanceList.length > 1) {
      let index = this.studentDailyAttendanceListViewModel.studentDailyAttendanceList.findIndex(x => x.studentId == element.studentId);
      if (index > 0) {
        this.studentDailyAttendanceListViewModel.studentDailyAttendanceList[index] = {
          tenantId: element.tenantId,
          schoolId: element.schoolId,
          gradeScaleId: null,
          studentId: element.studentId,
          gradeId: element.gradeId,
          sectionId: element.sectionId,
          attendanceCode: this.attendanceNoToString,
          attendanceDate: this.getAllStudent.attendanceDate,
          attendanceMinutes: null,
          attendanceComment: element.attendanceComment,
          createdBy: this.defaultValueService.getUserGuidId(),
          updatedBy: null,
          createdOn: null,
          updatedOn: null,
        };
      }
      else {
        this.studentDailyAttendanceListViewModel.studentDailyAttendanceList.push(
          {
            tenantId: element.tenantId,
            schoolId: element.schoolId,
            gradeScaleId: null,
            studentId: element.studentId,
            gradeId: element.gradeId,
            sectionId: element.sectionId,
            attendanceCode: this.attendanceNoToString,
            attendanceDate: this.getAllStudent.attendanceDate,
            attendanceMinutes: null,
            attendanceComment: element.attendanceComment,
            createdBy: this.defaultValueService.getUserGuidId(),
            updatedBy: null,
            createdOn: null,
            updatedOn: null,
          }
        );
      }
    }
    else {
      this.studentDailyAttendanceListViewModel.studentDailyAttendanceList.push(
        {
          tenantId: element.tenantId,
          schoolId: element.schoolId,
          gradeScaleId: null,
          studentId: element.studentId,
          gradeId: element.gradeId,
          sectionId: element.sectionId,
          attendanceCode: this.attendanceNoToString,
          attendanceDate: this.getAllStudent.attendanceDate,
          attendanceMinutes: null,
          attendanceComment: element.attendanceComment,
          createdBy: this.defaultValueService.getUserGuidId(),
          updatedBy: null,
          createdOn: null,
          updatedOn: null,
        }
      );
    }

  }

  submitDailyAttendance() {
    this.studentDailyAttendanceListViewModel.studentDailyAttendanceList.splice(0, 1);
    this.studentDailyAttendanceListViewModel.attendanceDate = this.getAllStudent.attendanceDate;
    this.studentAttendanceService.updateStudentDailyAttendance(this.studentDailyAttendanceListViewModel).subscribe(
      (res: StudentDailyAttendanceListViewModel) => {
        if (res) {
          if (res._failure) {
            this.commonService.checkTokenValidOrNot(res._message);
            this.snackbar.open('' + res._message, '', {
              duration: 10000
            });
          } else {
            this.getAllAttendanceCode();
            this.getAllStudentAttendanceListForAdministration();
            this.snackbar.open('' + res._message, '', {
              duration: 10000
            });
          }
        }
        else {
          this.snackbar.open(this.defaultValueService.getHttpError(), '', {
            duration: 10000
          });
        }
      }
    );
    
  }




  showAdvanceSearch() {
    this.showAdvanceSearchPanel = true;
    this.filterJsonParams = null;
  }

  hideAdvanceSearch(event) {
    this.showAdvanceSearchPanel = false;
  }

}
