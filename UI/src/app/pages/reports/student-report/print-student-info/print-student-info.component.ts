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

import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import icListAlt from '@iconify/icons-ic/twotone-list-alt';
import icSchool from '@iconify/icons-ic/twotone-school';
import { StudentListByDateRangeModel, StudentListModel } from 'src/app/models/student.model';
import { StudentInfoReportModel } from 'src/app/models/student-info-report.model';
import { StudentService } from 'src/app/services/student.service';
import { CommonService } from 'src/app/services/common.service';
import { MatTableDataSource } from '@angular/material/table';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { LoaderService } from 'src/app/services/loader.service';
import { FormControl } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { MatPaginator, MatPaginatorIntl } from '@angular/material/paginator';
import { MatCheckbox } from '@angular/material/checkbox';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { fadeInRight400ms } from 'src/@vex/animations/fade-in-right.animation';
import { StudentReportService } from 'src/app/services/student-report.service';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'vex-print-student-info',
  templateUrl: './print-student-info.component.html',
  styleUrls: ['./print-student-info.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms,
    fadeInRight400ms
  ]
})
export class PrintStudentInfoComponent implements OnInit, AfterViewInit {
  icListAlt = icListAlt;
  icSchool = icSchool;
  columns = [
    { label: '', property: 'studentCheck', type: 'text', visible: true },
    { label: 'Name', property: 'studentName', type: 'text', visible: true },
    { label: 'Student ID', property: 'studentId', type: 'text', visible: true },
    { label: 'Alternate ID', property: 'alternateId', type: 'text', visible: true },
    { label: 'Grade Level', property: 'gradeLevel', type: 'text', visible: true },
    { label: 'Section', property: 'section', type: 'text', visible: true },
    { label: 'Telephone', property: 'phone', type: 'text', visible: true },
    { label: 'School Name', property: 'schoolName', type: 'text', visible: false },
    { label: 'Status', property: 'status', type: 'text', visible: false }
  ];
  getAllStudent: StudentListByDateRangeModel = new StudentListByDateRangeModel();
  studentInfoReportModel: StudentInfoReportModel = new StudentInfoReportModel();
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  listOfStudents = [];
  selectedStudents = []
  studentModelList: MatTableDataSource<any>;
  loading: boolean;
  searchCtrl = new FormControl();
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild('masterCheckBox') masterCheckBox: MatCheckbox;
  showAdvanceSearchPanel: boolean = false;
  searchCount;
  searchValue;
  toggleValues;
  generatedReportCardData: any;
  today = new Date();
  defaultStudentPhoto = "/9j/4QAYRXhpZgAASUkqAAgAAAAAAAAAAAAAAP/sABFEdWNreQABAAQAAABkAAD/4QMtaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wLwA8P3hwYWNrZXQgYmVnaW49Iu+7vyIgaWQ9Ilc1TTBNcENlaGlIenJlU3pOVGN6a2M5ZCI/PiA8eDp4bXBtZXRhIHhtbG5zOng9ImFkb2JlOm5zOm1ldGEvIiB4OnhtcHRrPSJBZG9iZSBYTVAgQ29yZSA3LjAtYzAwMCA3OS5kYWJhY2JiLCAyMDIxLzA0LzE0LTAwOjM5OjQ0ICAgICAgICAiPiA8cmRmOlJERiB4bWxuczpyZGY9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkvMDIvMjItcmRmLXN5bnRheC1ucyMiPiA8cmRmOkRlc2NyaXB0aW9uIHJkZjphYm91dD0iIiB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iIHhtbG5zOnhtcE1NPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvbW0vIiB4bWxuczpzdFJlZj0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL3NUeXBlL1Jlc291cmNlUmVmIyIgeG1wOkNyZWF0b3JUb29sPSJBZG9iZSBQaG90b3Nob3AgMjIuNSAoV2luZG93cykiIHhtcE1NOkluc3RhbmNlSUQ9InhtcC5paWQ6QUE3MTBDNDQwNDk2MTFFQzg4Q0Y5N0JCOEU0Q0FGNTkiIHhtcE1NOkRvY3VtZW50SUQ9InhtcC5kaWQ6QUE3MTBDNDUwNDk2MTFFQzg4Q0Y5N0JCOEU0Q0FGNTkiPiA8eG1wTU06RGVyaXZlZEZyb20gc3RSZWY6aW5zdGFuY2VJRD0ieG1wLmlpZDpBQTcxMEM0MjA0OTYxMUVDODhDRjk3QkI4RTRDQUY1OSIgc3RSZWY6ZG9jdW1lbnRJRD0ieG1wLmRpZDpBQTcxMEM0MzA0OTYxMUVDODhDRjk3QkI4RTRDQUY1OSIvPiA8L3JkZjpEZXNjcmlwdGlvbj4gPC9yZGY6UkRGPiA8L3g6eG1wbWV0YT4gPD94cGFja2V0IGVuZD0iciI/Pv/uAA5BZG9iZQBkwAAAAAH/2wCEAAEBAQEBAQEBAQEBAQEBAQEB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"
  homeAddressLogo: any;
  mailAddressLogo: any;


  constructor(
    public translateService: TranslateService,
    private studentService: StudentService,
    private commonService: CommonService,
    private snackbar: MatSnackBar,
    private defaultValuesService: DefaultValuesService,
    private loaderService: LoaderService,
    private studentReportService: StudentReportService,
    private domSanitizer: DomSanitizer,
    private paginatorObj: MatPaginatorIntl,
  ) {
    paginatorObj.itemsPerPageLabel = translateService.instant('itemsPerPage');
    this.loaderService.isLoading.subscribe((val) => {
      this.loading = val;
    });

    this.defaultValuesService.setReportCompoentTitle.next("Print Student Info");
    this.homeAddressLogo = this.domSanitizer.bypassSecurityTrustUrl("data:image/svg+xml;base64,PHN2ZyBpZD0iaG9tZSIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIiB3aWR0aD0iNTQuNDciIGhlaWdodD0iNTQuNDciIHZpZXdCb3g9IjAgMCA1NC40NyA1NC40NyI+DQogIDxnIGlkPSJHcm91cF8yNjQiIGRhdGEtbmFtZT0iR3JvdXAgMjY0IiB0cmFuc2Zvcm09InRyYW5zbGF0ZSg0Ni41OTggMzcuNjYxKSI+DQogICAgPGcgaWQ9Ikdyb3VwXzI2MyIgZGF0YS1uYW1lPSJHcm91cCAyNjMiPg0KICAgICAgPHBhdGggaWQ9IlBhdGhfMTE2NyIgZGF0YS1uYW1lPSJQYXRoIDExNjciIGQ9Ik00MzkuODE2LDM1NC4zMTJhMS4wNjMsMS4wNjMsMCwxLDAsLjMxMi43NTJBMS4wNzEsMS4wNzEsMCwwLDAsNDM5LjgxNiwzNTQuMzEyWiIgdHJhbnNmb3JtPSJ0cmFuc2xhdGUoLTQzOCAtMzU0KSIvPg0KICAgIDwvZz4NCiAgPC9nPg0KICA8ZyBpZD0iR3JvdXBfMjY2IiBkYXRhLW5hbWU9Ikdyb3VwIDI2NiI+DQogICAgPGcgaWQ9Ikdyb3VwXzI2NSIgZGF0YS1uYW1lPSJHcm91cCAyNjUiIHRyYW5zZm9ybT0idHJhbnNsYXRlKDAgMCkiPg0KICAgICAgPHBhdGggaWQ9IlBhdGhfMTE2OCIgZGF0YS1uYW1lPSJQYXRoIDExNjgiIGQ9Ik01My40MDYsNTIuMzQzSDQ4LjcyNVY0Mi40NDlhMS4wNjQsMS4wNjQsMCwwLDAtMi4xMjgsMHY5Ljg5NEgxMi45Nzl2LTEwYTEuMDY0LDEuMDY0LDAsMCwwLTIuMTI4LDB2MTBINy44NzNWMjQuMzY0TDI3LjIzNSw5LjI0Nyw0Ni42LDI0LjM2NFYzNC40N2ExLjA2NCwxLjA2NCwwLDEsMCwyLjEyOCwwVjI2LjAyNWwuMTg2LjE0NWExLjA2NCwxLjA2NCwwLDAsMCwxLjQwNy0uMDg2bDMuODM3LTMuODM3YTEuMDY0LDEuMDY0LDAsMCwwLS4xLTEuNTkxbC02LjQtNC45OTRWNUExLjA2NCwxLjA2NCwwLDAsMCw0Ni42LDMuOTM3SDQwLjQzMkExLjA2NCwxLjA2NCwwLDAsMCwzOS4zNjgsNVY5LjE4N0wyNy44OS4yMjZhMS4wNjQsMS4wNjQsMCwwLDAtMS4zMDksMEwuNDEyLDIwLjY1NWExLjA2NCwxLjA2NCwwLDAsMC0uMSwxLjU5MWwzLjgzNywzLjgzN2ExLjA2NCwxLjA2NCwwLDAsMCwxLjQwNy4wODZsLjE4Ni0uMTQ1VjUyLjM0M0gxLjA2NGExLjA2NCwxLjA2NCwwLDAsMCwwLDIuMTI4SDUzLjQwNmExLjA2NCwxLjA2NCwwLDEsMCwwLTIuMTI4Wk00MS41LDYuMDY1aDQuMDM4VjE0TDQxLjUsMTAuODQ4Wk00Ljk5MSwyMy45MTQsMi42NywyMS41OTIsMjcuMjM1LDIuNDE0LDUxLjgsMjEuNTkybC0yLjMyMSwyLjMyMUwyNy44OSw3LjA1OWExLjA2NCwxLjA2NCwwLDAsMC0xLjMwOSwwWiIgdHJhbnNmb3JtPSJ0cmFuc2xhdGUoMCAtMC4wMDEpIi8+DQogICAgPC9nPg0KICA8L2c+DQogIDxnIGlkPSJHcm91cF8yNjgiIGRhdGEtbmFtZT0iR3JvdXAgMjY4IiB0cmFuc2Zvcm09InRyYW5zbGF0ZSgxNS45NTggMjMuMTkyKSI+DQogICAgPGcgaWQ9Ikdyb3VwXzI2NyIgZGF0YS1uYW1lPSJHcm91cCAyNjciPg0KICAgICAgPHBhdGggaWQ9IlBhdGhfMTE2OSIgZGF0YS1uYW1lPSJQYXRoIDExNjkiIGQ9Ik0xNzEuNDksMjE4SDE1MS4wNjRBMS4wNjQsMS4wNjQsMCwwLDAsMTUwLDIxOS4wNjRWMjM5LjQ5YTEuMDY0LDEuMDY0LDAsMCwwLDEuMDY0LDEuMDY0SDE3MS40OWExLjA2NCwxLjA2NCwwLDAsMCwxLjA2NC0xLjA2NFYyMTkuMDY0QTEuMDY0LDEuMDY0LDAsMCwwLDE3MS40OSwyMThabS0xMS4yNzcsMjAuNDI2aC04LjA4NXYtOC4wODVoOC4wODVabTAtMTAuMjEzaC04LjA4NXYtOC4wODVoOC4wODVabTEwLjIxMywxMC4yMTNoLTguMDg1di04LjA4NWg4LjA4NVptMC0xMC4yMTNoLTguMDg1di04LjA4NWg4LjA4NVoiIHRyYW5zZm9ybT0idHJhbnNsYXRlKC0xNTAgLTIxOCkiLz4NCiAgICA8L2c+DQogIDwvZz4NCiAgPGcgaWQ9Ikdyb3VwXzI3MCIgZGF0YS1uYW1lPSJHcm91cCAyNzAiIHRyYW5zZm9ybT0idHJhbnNsYXRlKDEwLjg1MiAzNy4wNzcpIj4NCiAgICA8ZyBpZD0iR3JvdXBfMjY5IiBkYXRhLW5hbWU9Ikdyb3VwIDI2OSI+DQogICAgICA8cGF0aCBpZD0iUGF0aF8xMTcwIiBkYXRhLW5hbWU9IlBhdGggMTE3MCIgZD0iTTEwMy4wNjQsMzQ4LjUwNkExLjA2NCwxLjA2NCwwLDAsMCwxMDIsMzQ5LjU3di4wMTdhMS4wNjQsMS4wNjQsMCwxLDAsMi4xMjgsMHYtLjAxN0ExLjA2NCwxLjA2NCwwLDAsMCwxMDMuMDY0LDM0OC41MDZaIiB0cmFuc2Zvcm09InRyYW5zbGF0ZSgtMTAyIC0zNDguNTA2KSIvPg0KICAgIDwvZz4NCiAgPC9nPg0KPC9zdmc+DQo=");
    this.mailAddressLogo = this.domSanitizer.bypassSecurityTrustUrl("data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHdpZHRoPSI1NS42ODEiIGhlaWdodD0iNTUuNjgxIiB2aWV3Qm94PSIwIDAgNTUuNjgxIDU1LjY4MSI+DQogIDxnIGlkPSJtYWlsLWJveCIgdHJhbnNmb3JtPSJ0cmFuc2xhdGUoMCkiPg0KICAgIDxwYXRoIGlkPSJQYXRoXzExNzMiIGRhdGEtbmFtZT0iUGF0aCAxMTczIiBkPSJNNDQuMTUzLDEzLjkySDM5LjYyN1YxMC44NzVINTQuNTkzYTEuMDg4LDEuMDg4LDAsMCwwLC43NzEtMS44NTVMNTEuOCw1LjQzOGwzLjU2OC0zLjU4MkExLjA4OCwxLjA4OCwwLDAsMCw1NC41OTMsMEgzOC41MzlhMS4wODcsMS4wODcsMCwwLDAtMS4wODgsMS4wODhWMTMuOTJIMTcuN2MtLjAyLDAtLjA0LDAtLjA2LDBsLS4yMzksMEExMS41NDEsMTEuNTQxLDAsMCwwLDUuODgxLDI1LjAxM0gyLjE3NUEyLjE3OCwyLjE3OCwwLDAsMCwwLDI3LjE4OFY0MC42NzNhMi4xNzgsMi4xNzgsMCwwLDAsMi4xNzUsMi4xNzVIMjkuNThWNTQuNTkzYTEuMDg3LDEuMDg3LDAsMCwwLDEuMDg4LDEuMDg4aDYuMDlhMS4wODcsMS4wODcsMCwwLDAsMS4wODgtMS4wODhWNDIuODQ4SDU0LjU5M2ExLjA4NywxLjA4NywwLDAsMCwxLjA4OC0xLjA4OFYyNS40NDhBMTEuNTQxLDExLjU0MSwwLDAsMCw0NC4xNTMsMTMuOTJaTTQ5LjQ5LDQuNjdhMS4wODgsMS4wODgsMCwwLDAsMCwxLjUzNUw1MS45NzUsOC43SDM5LjYyN1YyLjE3NUg1MS45NzVaTTIyLjQsMjguNzQ2djEwLjM5bC01LjItNS4yWm0tNy41NTYsNC40OGEzLjYyMSwzLjYyMSwwLDAsMS01LjExNiwwTDMuNjkzLDI3LjE4OEgyMC44ODVaTTIuMTc1LDI4Ljc0Nmw1LjE5NSw1LjE5NUwyLjE3NSwzOS4xMzVaTTMuNzEzLDQwLjY3M2w1LjI1OC01LjI1OGE1LjgsNS44LDAsMCwwLDYuNjM2LDBsNS4yNTgsNS4yNThaTTM1LjY3LDUzLjUwNkgzMS43NTVWNDIuODQ4SDM1LjY3Wk01My41MDUsNDAuNjczSDI4LjkyOFYzNS44ODFhMS4wODgsMS4wODgsMCwwLDAtMi4xNzUsMHY0Ljc5MkgyNC41NzhWMjcuMTg4QTIuMTc4LDIuMTc4LDAsMCwwLDIyLjQsMjUuMDEzSDguMDU4YTkuMzUyLDkuMzUyLDAsMCwxLDE4LjY5NC40MzV2LjY0NmExLjA4OCwxLjA4OCwwLDAsMCwyLjE3NSwwdi0uNjQ2YTExLjUyLDExLjUyLDAsMCwwLTQuOC05LjM1M0gzNy40NTJ2NS43NjRhMS4wODgsMS4wODgsMCwwLDAsMi4xNzUsMFYxNi4xaDQuNTI2YTkuMzYzLDkuMzYzLDAsMCwxLDkuMzUzLDkuMzUzWm0wLDAiIHRyYW5zZm9ybT0idHJhbnNsYXRlKDAgMCkiLz4NCiAgICA8cGF0aCBpZD0iUGF0aF8xMTc0IiBkYXRhLW5hbWU9IlBhdGggMTE3NCIgZD0iTTI0Ny4xNDYsMjc0Ljk0MWExLjA4OCwxLjA4OCwwLDEsMCwuNzY5LjMxOUExLjA5LDEuMDksMCwwLDAsMjQ3LjE0NiwyNzQuOTQxWm0wLDAiIHRyYW5zZm9ybT0idHJhbnNsYXRlKC0yMTkuMyAtMjQ1LjA0MSkiLz4NCiAgPC9nPg0KPC9zdmc+DQo=");

  }

  ngOnInit(): void {
    this.getAllStudent.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
    this.getAllStudentList();
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
        Object.assign(this.getAllStudent, { filterParams: filterParams });
        this.getAllStudent.pageNumber = 1;
        this.paginator.pageIndex = 0;
        this.getAllStudent.pageSize = this.pageSize;
        this.getAllStudentList();
      }
      else {
        Object.assign(this.getAllStudent, { filterParams: null });
        this.getAllStudent.pageNumber = this.paginator.pageIndex + 1;
        this.getAllStudent.pageSize = this.pageSize;
        this.getAllStudentList();
      }
    });
  }

  // For get all student list
  getAllStudentList() {
    if (this.getAllStudent.sortingModel?.sortColumn === "") {
      this.getAllStudent.sortingModel = null;
    }
    this.studentService.getAllStudentListByDateRange(this.getAllStudent).subscribe(data => {
      if (data._failure) {
        this.commonService.checkTokenValidOrNot(data._message);
        if (data.studentListViews === null) {
          this.totalCount = null;
          this.studentModelList = new MatTableDataSource([]);
          this.snackbar.open(data._message, '', {
            duration: 10000
          });
        } else {
          this.studentModelList = new MatTableDataSource([]);
          this.totalCount = null;
        }
      } else {
        this.totalCount = data.totalCount;
        this.pageNumber = data.pageNumber;
        this.pageSize = data._pageSize;
        data.studentListViews.forEach((student) => {
          student.checked = false;
        });
        this.listOfStudents = data.studentListViews.map((item) => {
          this.selectedStudents.map((selectedUser) => {
            if (item.studentId == selectedUser.studentId) {
              item.checked = true;
              return item;
            }
          });
          return item;
        });

        this.masterCheckBox.checked = this.listOfStudents.every((item) => {
          return item.checked;
        })
        this.studentModelList = new MatTableDataSource(data.studentListViews);
        this.getAllStudent = new StudentListByDateRangeModel();
      }
    });
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
      Object.assign(this.getAllStudent, { filterParams: filterParams });
    }
    this.getAllStudent.pageNumber = event.pageIndex + 1;
    this.getAllStudent.pageSize = event.pageSize;
    this.defaultValuesService.setPageSize(event.pageSize);
    this.getAllStudentList();
  }



  someComplete(): boolean {
    let indetermine = false;
    for (let user of this.listOfStudents) {
      for (let selectedUser of this.selectedStudents) {
        if (user.studentId === selectedUser.studentId) {
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
    this.studentModelList = new MatTableDataSource(this.listOfStudents);
    this.decideCheckUncheck();
  }

  onChangeSelection(eventStatus: boolean, id) {
    for (let item of this.listOfStudents) {
      if (item.studentId == id) {
        item.checked = eventStatus;
        break;
      }
    }
    this.studentModelList = new MatTableDataSource(this.listOfStudents);
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
          if (item.studentId == selectedUser.studentId) {
            isIdIncludesInSelectedList = true;
            break;
          }
        }
        if (!isIdIncludesInSelectedList) {
          this.selectedStudents.push(item);
        }
      } else {
        for (let selectedUser of this.selectedStudents) {
          if (item.studentId == selectedUser.studentId) {
            this.selectedStudents = this.selectedStudents.filter((user) => user.studentId != item.studentId);
            break;
          }
        }
      }
      isIdIncludesInSelectedList = false;

    });
    this.selectedStudents = this.selectedStudents.filter((item) => item.checked);
  }

  hideAdvanceSearch(event) {
    this.showAdvanceSearchPanel = false;
  }

  getSearchInput(event) {
    this.searchValue = event;
  }

  getToggleValues(event) {
    this.toggleValues = event;
    if (event.inactiveStudents === true) {
      this.columns[8].visible = true;
    } else if (event.inactiveStudents === false) {
      this.columns[8].visible = false;
    }
    if (event.searchAllSchool === true) {
      this.columns[7].visible = true;
    } else if (event.searchAllSchool === false) {
      this.columns[7].visible = false;
    }
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  getSearchResult(res) {
    this.getAllStudent = new StudentListByDateRangeModel();
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
    if (res && res.studentListViews) {
      res?.studentListViews?.forEach((student) => {
        student.checked = false;
      });
      this.listOfStudents = res.studentListViews.map((item) => {
        this.selectedStudents.map((selectedUser) => {
          if (item.studentId == selectedUser.studentId) {
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
    this.studentModelList = new MatTableDataSource(res?.studentListViews);
    this.getAllStudent = new StudentListByDateRangeModel();
  }

  generateStudentInfoReport() {
    if (!this.studentInfoReportModel.isGeneralInfo && !this.studentInfoReportModel.isEnrollmentInfo && !this.studentInfoReportModel.isAddressInfo && !this.studentInfoReportModel.isFamilyInfo && !this.studentInfoReportModel.isMedicalInfo && !this.studentInfoReportModel.isComments) {
      this.snackbar.open('Please select any option to generate report.', '', {
        duration: 2000
      });
      return;
    }
    else if (this.selectedStudents.length === 0) {
      this.snackbar.open('Please select any student to generate report.', '', {
        duration: 2000
      });
      return;
    }
    this.addAndGenerateStudentInfoReportCard().then((res: any) => {

      res.schoolMasterData.map((item)=>{
        item.studentMasterData.map((subItem)=>{
          if(subItem.studentEnrollment) {
            subItem.studentEnrollment?.length > 0 ? subItem.studentEnrollment.reverse() : '';
            subItem.studentEnrollment.map((data, index)=>{
              if (data.enrollmentCode === "Dropped Out" && subItem.studentEnrollment[index + 1]?.exitCode === "Dropped Out" 
              && data.enrollmentDate === subItem.studentEnrollment[index + 1]?.exitDate) {                    
                      subItem.studentEnrollment.splice((index+1),1);
                  }
            })
          }
        })
      })
      this.generatedReportCardData = res;
      setTimeout(() => {
        this.generatePdf();
      }, 100 * this.generatedReportCardData.schoolMasterData.length);
    });
  }

  addAndGenerateStudentInfoReportCard() {
    this.studentInfoReportModel.studentGuids = this.selectedStudents.map((item) => {
      return item.studentGuid
    })


    return new Promise((resolve, reject) => {
      this.studentReportService.getStudentInfoReport(this.studentInfoReportModel).subscribe((res) => {
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

  generatePdf() {
    let printContents, popupWin;
    printContents = document.getElementById('printReportCardId').innerHTML;
    document.getElementById('printReportCardId').className = 'block';
    popupWin = window.open('', '_blank', 'top=0,left=0,height=100%,width=auto');
    if(popupWin === null || typeof(popupWin)==='undefined'){
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

}
