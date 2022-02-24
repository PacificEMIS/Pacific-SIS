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

import { AfterViewInit, Component, ElementRef, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { TranslateService } from "@ngx-translate/core";
import icSearch from '@iconify/icons-ic/search';
import { GetAllGradeLevelsModel } from "../../../models/grade-level.model";
import { GradeLevelService } from "../../../services/grade-level.service";
import { MatSnackBar } from "@angular/material/snack-bar";
import { StudentListModel } from '../../../models/student.model';
import { StudentService } from "../../../services/student.service";
import { MatTableDataSource } from "@angular/material/table";
import { FormControl } from "@angular/forms";
import { MatPaginator } from "@angular/material/paginator";
import { MatSort } from "@angular/material/sort";
import { debounceTime, distinctUntilChanged, switchMap, takeUntil } from "rxjs/operators";
import { stagger40ms } from '../../../../@vex/animations/stagger.animation';
import { LoaderService } from '../../../services/loader.service';
import { forkJoin, Subject } from "rxjs";
import { fadeInUp400ms } from "../../../../@vex/animations/fade-in-up.animation";
import { fadeInRight400ms } from "../../../../@vex/animations/fade-in-right.animation";
import { MatCheckbox } from "@angular/material/checkbox";
import { GetStudentTranscriptModel, StudentTranscript } from "../../../models/student-transcript.model";
import { StudentTranscriptService } from "../../../services/student-transcript.service";
import { map } from 'rxjs/operators';
import { Permissions } from "../../../models/roll-based-access.model";
import { PageRolesPermission } from "../../../common/page-roles-permissions.service";
import { CommonService } from "src/app/services/common.service";
import { DefaultValuesService } from 'src/app/common/default-values.service';

@Component({
  selector: "vex-transcripts",
  templateUrl: "./transcripts.component.html",
  styleUrls: ["./transcripts.component.scss"],
  animations: [
    fadeInUp400ms,
    stagger40ms,
    fadeInRight400ms
  ]
})
export class TranscriptsComponent implements OnInit, OnDestroy {
  icSearch = icSearch;
  columns = [
    { label: '', property: 'selectedStudent', type: 'text', visible: true },
    { label: 'Name', property: 'firstGivenName', type: 'text', visible: true },
    { label: 'Student ID', property: 'studentInternalId', type: 'text', visible: true },
    { label: 'Alternate ID', property: 'alternateId', type: 'text', visible: true },
    { label: 'Grade Level', property: 'gradeLevelTitle', type: 'text', visible: true },
    { label: 'Section', property: 'sectionId', type: 'text', visible: true },
    { label: 'Telephone', property: 'homePhone', type: 'text', visible: true },
    { label: 'Status', property: 'status', type: 'text', visible: false },
  ];

  loading: boolean;
  destroySubject$: Subject<void> = new Subject();
  totalCount = 0;
  pageNumber: number;
  pageSize: number;
  searchValue;
  searchCount;
  searchCtrl: FormControl;
  getAllGradeLevels: GetAllGradeLevelsModel = new GetAllGradeLevelsModel();
  getAllStudent: StudentListModel = new StudentListModel();
  studentList: MatTableDataSource<any>;
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild('masterCheckBox') masterCheckBox: MatCheckbox;
  showAdvanceSearchPanel: boolean = false;
  toggleValues;
  listOfStudents = [];
  selectedStudents = []
  selectedGradeLevels = [];
  studentTranscipt = new StudentTranscript();
  getStudentTranscriptModel: GetStudentTranscriptModel = new GetStudentTranscriptModel();
  gradeLevelError: boolean;
  pdfByteArrayForTranscript: string;
  pdfGenerateLoader: boolean = false;
  permissions: Permissions;
  generatedTranscriptData;
  generatedTranscriptDataForPDF = {
    "tenantId": "1e93c7bf-0fae-42bb-9e09-a1cedc8c0355",
    "schoolId": 66,
    "schoolLogo": null,
    "studentPhoto": null,
    "gradeLagend": null,
    "gradeLavels": null,
    "studentsDetailsForTranscripts": [
      {
        "schoolName": "International High School",
        "schoolPicture": null,
        "streetAddress1": "1600 Pennsylvania Avenue NW",
        "streetAddress2": null,
        "city": "D.C",
        "state": "Washington",
        "district": "Mumbai",
        "zip": "20500",
        "country": null,
        "principalName": "Alice George",
        "gradeList": [
          
        ],
        "gradeLevelDetailsForTranscripts": [
          {
            "markingPeriodDetailsForTranscripts": [
              {
                "reportCardDetailsForTranscripts": [
                  {
                    "courseSectionName": "Test Course 1 (UC-NOA/MS)",
                    "courseCode": "FSC",
                    "creditHours": 0.0,
                    "creditEarned": 0.0,
                    "grade": "F",
                    "gpValue": 0.000
                  }
                ],
                "markingPeriodTitle": "Quarter 1",
                "creditAttemped": 0.0,
                "creditEarned": 0.0,
                "gpa": null
              },
              {
                "reportCardDetailsForTranscripts": [
                  {
                    "courseSectionName": "Test Course 1 (UC-NOA/MS)",
                    "courseCode": "FSC",
                    "creditHours": 1.250,
                    "creditEarned": 1.250,
                    "grade": "C",
                    "gpValue": 2.50000
                  }
                ],
                "markingPeriodTitle": "Quarter 2",
                "creditAttemped": 1.250,
                "creditEarned": 1.250,
                "gpa": 2.00
              },
              {
                "reportCardDetailsForTranscripts": [
                  {
                    "courseSectionName": "Test Course 4 (WC-WA/MS)",
                    "courseCode": "FSC",
                    "creditHours": 0.875,
                    "creditEarned": 0.875,
                    "grade": "B",
                    "gpValue": 3.50000
                  },
                  {
                    "courseSectionName": "Test Course 1 (UC-NOA/MS)",
                    "courseCode": "FSC",
                    "creditHours": 1.250,
                    "creditEarned": 1.250,
                    "grade": "D",
                    "gpValue": 1.25000
                  }
                ],
                "markingPeriodTitle": "Quarter 3",
                "creditAttemped": 2.125,
                "creditEarned": 2.125,
                "gpa": 2.235
              },
              {
                "reportCardDetailsForTranscripts": [
                  {
                    "courseSectionName": "Test Course 1 (UC-NOA/MS)",
                    "courseCode": "FSC",
                    "creditHours": 1.250,
                    "creditEarned": 1.250,
                    "grade": "A",
                    "gpValue": 5.00000
                  }
                ],
                "markingPeriodTitle": "Quarter 4",
                "creditAttemped": 1.250,
                "creditEarned": 1.250,
                "gpa": 4.00
              },
              {
                "reportCardDetailsForTranscripts": [
                  {
                    "courseSectionName": "Test Course 1 (UC-NOA/MS)",
                    "courseCode": "FSC",
                    "creditHours": 2.500,
                    "creditEarned": 2.500,
                    "grade": "A",
                    "gpValue": 10.00000
                  }
                ],
                "markingPeriodTitle": "Semester 1",
                "creditAttemped": 2.500,
                "creditEarned": 2.500,
                "gpa": 4.00
              },
              {
                "reportCardDetailsForTranscripts": [
                  {
                    "courseSectionName": "Test Course 1 (UC-NOA/MS)",
                    "courseCode": "FSC",
                    "creditHours": 2.500,
                    "creditEarned": 2.500,
                    "grade": "B",
                    "gpValue": 7.50000
                  }
                ],
                "markingPeriodTitle": "Semester 2",
                "creditAttemped": 2.500,
                "creditEarned": 2.500,
                "gpa": 3.00
              },
              {
                "reportCardDetailsForTranscripts": [
                  {
                    "courseSectionName": "Test Course 1 (UC-NOA/MS)",
                    "courseCode": "FSC",
                    "creditHours": 5.000,
                    "creditEarned": 5.000,
                    "grade": "B",
                    "gpValue": 15.00000
                  },
                  {
                    "courseSectionName": "Test Course 4 (WC-WA/MS)",
                    "courseCode": "FSC",
                    "creditHours": 0.0,
                    "creditEarned": 0.0,
                    "grade": "A",
                    "gpValue": 0.000
                  }
                ],
                "markingPeriodTitle": "Full Year",
                "creditAttemped": 5.000,
                "creditEarned": 5.000,
                "gpa": 3.00
              }
            ],
            "gradeId": 53,
            "gradeLevelTitle": "Grade 9",
            "schoolName": "International High School",
            "schoolYear": "2021-2022"
          }
        ],
        "firstGivenName": "Shivam",
        "middleName": null,
        "lastFamilyName": "Maheshwari",
        "studentId": 1,
        "studentGuid": "8b976408-f0a8-40d7-8fef-48f7f81cc528",
        "studentPhoto": "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",
        "studentInternalId": "1",
        "dob": null,
        "homeAddressLineOne": null,
        "homeAddressLineTwo": null,
        "homeAddressCountry": null,
        "homeAddressCity": null,
        "homeAddressState": null,
        "homeAddressZip": null,
        "cumulativeGPA": 3.060,
        "totalCreditAttempeted": 14.625,
        "totalCreditEarned": 14.625
      },
      {
        "schoolName": "International High School",
        "schoolPicture": null,
        "streetAddress1": "1600 Pennsylvania Avenue NW",
        "streetAddress2": null,
        "city": "D.C",
        "state": "Washington",
        "district": "Mumbai",
        "zip": "20500",
        "country": null,
        "principalName": "Alice George",
        "gradeList": [
          
        ],
        "gradeLevelDetailsForTranscripts": [
          {
            "markingPeriodDetailsForTranscripts": [
              {
                "reportCardDetailsForTranscripts": [
                  {
                    "courseSectionName": "Test Course 1 (UC-NOA/MS)",
                    "courseCode": "FSC",
                    "creditHours": 0.0,
                    "creditEarned": 0.0,
                    "grade": "F",
                    "gpValue": 0.000
                  }
                ],
                "markingPeriodTitle": "Quarter 1",
                "creditAttemped": 0.0,
                "creditEarned": 0.0,
                "gpa": null
              },
              {
                "reportCardDetailsForTranscripts": [
                  {
                    "courseSectionName": "Test Course 1 (UC-NOA/MS)",
                    "courseCode": "FSC",
                    "creditHours": 1.250,
                    "creditEarned": 1.250,
                    "grade": "B",
                    "gpValue": 3.75000
                  }
                ],
                "markingPeriodTitle": "Quarter 2",
                "creditAttemped": 1.250,
                "creditEarned": 1.250,
                "gpa": 3.00
              },
              {
                "reportCardDetailsForTranscripts": [
                  {
                    "courseSectionName": "Test Course 4 (WC-WA/MS)",
                    "courseCode": "FSC",
                    "creditHours": 0.875,
                    "creditEarned": 0.875,
                    "grade": "B",
                    "gpValue": 3.50000
                  },
                  {
                    "courseSectionName": "Test Course 1 (UC-NOA/MS)",
                    "courseCode": "FSC",
                    "creditHours": 1.250,
                    "creditEarned": 1.250,
                    "grade": "B",
                    "gpValue": 3.75000
                  }
                ],
                "markingPeriodTitle": "Quarter 3",
                "creditAttemped": 2.125,
                "creditEarned": 2.125,
                "gpa": 3.412
              },
              {
                "reportCardDetailsForTranscripts": [
                  {
                    "courseSectionName": "Test Course 1 (UC-NOA/MS)",
                    "courseCode": "FSC",
                    "creditHours": 1.250,
                    "creditEarned": 1.250,
                    "grade": "D",
                    "gpValue": 1.25000
                  }
                ],
                "markingPeriodTitle": "Quarter 4",
                "creditAttemped": 1.250,
                "creditEarned": 1.250,
                "gpa": 1.00
              },
              {
                "reportCardDetailsForTranscripts": [
                  {
                    "courseSectionName": "Test Course 1 (UC-NOA/MS)",
                    "courseCode": "FSC",
                    "creditHours": 2.500,
                    "creditEarned": 2.500,
                    "grade": "F",
                    "gpValue": 0.00000
                  }
                ],
                "markingPeriodTitle": "Semester 1",
                "creditAttemped": 2.500,
                "creditEarned": 2.500,
                "gpa": null
              },
              {
                "reportCardDetailsForTranscripts": [
                  {
                    "courseSectionName": "Test Course 1 (UC-NOA/MS)",
                    "courseCode": "FSC",
                    "creditHours": 2.500,
                    "creditEarned": 2.500,
                    "grade": "B",
                    "gpValue": 7.50000
                  }
                ],
                "markingPeriodTitle": "Semester 2",
                "creditAttemped": 2.500,
                "creditEarned": 2.500,
                "gpa": 3.00
              },
              {
                "reportCardDetailsForTranscripts": [
                  {
                    "courseSectionName": "Test Course 1 (UC-NOA/MS)",
                    "courseCode": "FSC",
                    "creditHours": 5.000,
                    "creditEarned": 5.000,
                    "grade": "D",
                    "gpValue": 5.00000
                  },
                  {
                    "courseSectionName": "Test Course 4 (WC-WA/MS)",
                    "courseCode": "FSC",
                    "creditHours": 0.0,
                    "creditEarned": 0.0,
                    "grade": null,
                    "gpValue": 5.00000
                  }
                ],
                "markingPeriodTitle": "Full Year",
                "creditAttemped": 5.000,
                "creditEarned": 5.000,
                "gpa": 2.00
              }
            ],
            "gradeId": 54,
            "gradeLevelTitle": "Grade 10",
            "schoolName": "International High School",
            "schoolYear": "2021-2022"
          }
        ],
        "firstGivenName": "Joe",
        "middleName": null,
        "lastFamilyName": "Root",
        "studentId": 3,
        "studentGuid": "deef245a-b05a-43f8-a999-54132839ca17",
        "studentPhoto": null,
        "studentInternalId": "3",
        "dob": null,
        "homeAddressLineOne": null,
        "homeAddressLineTwo": null,
        "homeAddressCountry": null,
        "homeAddressCity": null,
        "homeAddressState": null,
        "homeAddressZip": null,
        "cumulativeGPA": 2.034,
        "totalCreditAttempeted": 14.625,
        "totalCreditEarned": 14.625
      }
    ],
    "_userName": "Debanti",
    "_tenantName": "opensis",
    "_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6Im9wZW5zaXNEZWJhbnRpfGRlYmFudGlAb3BlbnNpcy5jb218MWU5M2M3YmYtMGZhZS00MmJiLTllMDktYTFjZWRjOGMwMzU1IiwibmJmIjoxNjQ1NTI1Mjk3LCJleHAiOjE2NDU1MjcwOTcsImlhdCI6MTY0NTUyNTI5N30.nwt3snRHLFWT8JnpLEoFqq7tM47MVaFRhBGUvmygbeM",
    "_tokenExpiry": "0001-01-01T00:00:00+00:00",
    "_failure": false,
    "_message": null
  }
  constructor(
    public translateService: TranslateService,
    private gradeLevelService: GradeLevelService,
    private snackbar: MatSnackBar,
    private studentService: StudentService,
    private loaderService: LoaderService,
    private transcriptService: StudentTranscriptService,
    private pageRolePermissions: PageRolesPermission,
    private el: ElementRef,
    private commonService: CommonService,
    private defaultValuesService: DefaultValuesService
  ) {
    // translateService.use("en");
    this.getAllStudent.filterParams = null;
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });
  }

  ngOnInit(): void {
    this.getAllStudent.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
    this.searchCtrl = new FormControl();
    this.callAllStudent();
    this.getAllGradeLevel();
    this.permissions = this.pageRolePermissions.checkPageRolePermission();
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  ngAfterViewInit() {

    //  Sorting
    this.getAllStudent = new StudentListModel();
    this.sort.sortChange.subscribe((res) => {
      this.getAllStudent.pageNumber = this.pageNumber
      this.getAllStudent.pageSize = this.pageSize;
      this.getAllStudent.sortingModel.sortColumn = res.active;
      if (this.searchCtrl.value) {
        let filterParams = [
          {
            columnName: null,
            filterValue: this.searchCtrl.value,
            filterOption: 3
          }
        ]
        Object.assign(this.getAllStudent, { filterParams: filterParams });
      }
      if (res.direction == "") {
        this.getAllStudent.sortingModel = null;
        this.callAllStudent();
        this.getAllStudent = new StudentListModel();
        this.getAllStudent.sortingModel = null;
      } else {
        this.getAllStudent.sortingModel.sortDirection = res.direction;
        this.callAllStudent();
      }
    });
    //  Searching
    this.searchCtrl.valueChanges.pipe(debounceTime(500), distinctUntilChanged()).subscribe((term) => {
      if (term) {
        this.callWithSearchParams(term)
      }
      else {
        this.callWithoutSearchParams()
      }
    })
  }

  callWithSearchParams(term) {
    let filterParams = [
      {
        columnName: null,
        filterValue: term,
        filterOption: 3
      }
    ]
    if (this.sort.active && this.sort.direction) {
      this.getAllStudent.sortingModel.sortColumn = this.sort.active;
      this.getAllStudent.sortingModel.sortDirection = this.sort.direction;
    }
    Object.assign(this.getAllStudent, { filterParams: filterParams });
    this.getAllStudent.pageNumber = 1;
    this.paginator.pageIndex = 0;
    this.getAllStudent.pageSize = this.pageSize;
    this.callAllStudent();
  }
  callWithoutSearchParams() {
    Object.assign(this.getAllStudent, { filterParams: null });
    this.getAllStudent.pageNumber = this.paginator.pageIndex + 1;
    this.getAllStudent.pageSize = this.pageSize;
    if (this.sort.active != undefined && this.sort.direction != "") {
      this.getAllStudent.sortingModel.sortColumn = this.sort.active;
      this.getAllStudent.sortingModel.sortDirection = this.sort.direction;
    }
    this.callAllStudent();
  }

  resetStudentList() {
    this.searchCount = null;
    this.searchValue = null;
    this.callAllStudent();
  }

  onGradeLevelChange(event, gradeId) {
    if (event.checked) {
      this.selectedGradeLevels.push(gradeId);
    } else {
      this.selectedGradeLevels = this.selectedGradeLevels.filter(item => item !== gradeId);
    }
    this.selectedGradeLevels?.length > 0 ? this.gradeLevelError = false : this.gradeLevelError = true;

  }

  getPageEvent(event) {
    if (this.sort.active && this.sort.direction) {
      this.getAllStudent.sortingModel.sortColumn = this.sort.active;
      this.getAllStudent.sortingModel.sortDirection = this.sort.direction;
    }
    if (this.searchCtrl.value) {
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
    this.callAllStudent();
  }

  callAllStudent() {
    if (this.getAllStudent.sortingModel?.sortColumn == "") {
      this.getAllStudent.sortingModel = null;
    }
    this.studentService.GetAllStudentList(this.getAllStudent).subscribe(res => {
    if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
        if (res.studentListViews === null) {
          this.totalCount = null;
          this.studentList = new MatTableDataSource([]);
          this.snackbar.open(res._message, '', {
            duration: 10000
          });
        } else {
          this.studentList = new MatTableDataSource([]);
          this.totalCount = null;
        }
      } else {
        this.totalCount = res.totalCount;
        this.pageNumber = res.pageNumber;
        this.pageSize = res._pageSize;
        res.studentListViews.forEach((student) => {
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
        this.studentList = new MatTableDataSource(res.studentListViews);
        this.getAllStudent = new StudentListModel();
      }
    });
  }

  getSearchResult(res) {
    this.getAllStudent = new StudentListModel();
    if (res?.totalCount){
      this.searchCount = res.totalCount;
      this.totalCount = res.totalCount;
    }
    else{
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
    this.studentList = new MatTableDataSource(res?.studentListViews);
    this.getAllStudent = new StudentListModel();
  }

  getToggleValues(event) {
    this.toggleValues = event;
    if (event.inactiveStudents === true) {
      this.columns[7].visible = true;
    }
    else if (event.inactiveStudents === false) {
      this.columns[7].visible = false;
    }
  }

  hideAdvanceSearch(event) {
    this.showAdvanceSearchPanel = false;
  }

  getSearchInput(event) {
    this.searchValue = event;
  }

  getAllGradeLevel() {
    this.gradeLevelService.getAllGradeLevels(this.getAllGradeLevels).pipe(takeUntil(this.destroySubject$)).subscribe((res) => {
      if (res) {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          if (res.tableGradelevelList === null) {
            this.getAllGradeLevels.tableGradelevelList = [];
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          } else {
            this.getAllGradeLevels.tableGradelevelList = res.tableGradelevelList;
          }
        }
        else {
          this.getAllGradeLevels.tableGradelevelList = res.tableGradelevelList;
        }
      } else {
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    })
  }

  someComplete(): boolean {
    let indetermine = false;
    for (let user of this.listOfStudents) {
      for (let selectedUser of this.selectedStudents) {
        if (user.studentId == selectedUser.studentId) {
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
    this.studentList = new MatTableDataSource(this.listOfStudents);
    this.decideCheckUncheck();
  }

  onChangeSelection(eventStatus: boolean, id) {
    for (let item of this.listOfStudents) {
      if (item.studentId == id) {
        item.checked = eventStatus;
        break;
      }
    }
    this.studentList = new MatTableDataSource(this.listOfStudents);
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

  generateTranscript() {
    if (!this.selectedGradeLevels?.length) {
      this.gradeLevelError = true;
      const invalidGradeLevel: HTMLElement = this.el.nativeElement.querySelector(
        '.custom-scroll'
      );
      invalidGradeLevel.scrollIntoView({ behavior: 'smooth',block: 'center' });
      return;
    } else {
      this.gradeLevelError = false;
    }
    if (!this.selectedStudents?.length) {
      this.snackbar.open('Select at least one student.', '', {
        duration: 3000
      });
      return
    }
    this.fillUpNecessaryValues();
    this.fetchTranscript();
  }

  fillUpNecessaryValues() {
    this.getStudentTranscriptModel.gradeLavels = this.selectedGradeLevels.toString();
    this.getStudentTranscriptModel.studentsDetailsForTranscripts = [];
    this.selectedStudents?.map((item) => {
      this.getStudentTranscriptModel.studentsDetailsForTranscripts.push({
        studentId: item.studentId
        // studentGuid: item.studentGuid,
        // firstGivenName: item.firstGivenName,
        // middleName: item.middleName,
        // lastFamilyName: item.lastFamilyName,
      })
    });
  }

  getTranscriptForStudents() {
    return new Promise((resolve, reject) => {
      this.transcriptService.getTranscriptForStudents(this.getStudentTranscriptModel).subscribe(res => {
        if (res._failure) {
          this.commonService.checkTokenValidOrNot(res._message);
          this.snackbar.open(res._message, '', {
            duration: 1000
          });
          this.pdfGenerateLoader = false;
        } else {
          resolve(res);
          this.pdfGenerateLoader = false;
        }
      })
    });
  }
  
  
  fetchTranscript() {
    this.pdfGenerateLoader = true;
    this.getTranscriptForStudents().then((res: any) => {
      this.generatedTranscriptData = res;
      setTimeout(() => {
        this.generatePDF();
      }, 100 * this.generatedTranscriptData?.studentsDetailsForTranscripts.length);
    });
  }

  // fetchTranscript() {
  //   this.pdfGenerateLoader = true;
  //   this.transcriptService.addTranscriptForStudent(this.studentTranscipt).pipe(
  //     takeUntil(this.destroySubject$),
  //     switchMap((dataAfterAddingStudentRecords) => {
  //       let generateTranscriptObservable$
  //       if (!dataAfterAddingStudentRecords._failure) {
  //         generateTranscriptObservable$ = this.transcriptService.generateTranscriptForStudent(this.studentTranscipt)
  //       } else {
  //         this.snackbar.open(dataAfterAddingStudentRecords._message, '', {
  //           duration: 3000
  //         });
  //       }
  //       return forkJoin(generateTranscriptObservable$);
  //     })
  //   ).subscribe((res: any) => {
  //     this.pdfGenerateLoader = false;
  //     let response = res[0]
  //     if (response._failure) { this.commonService.checkTokenValidOrNot(response._message);


  //       this.snackbar.open(response._message, '', {
  //         duration: 3000
  //       });
  //     } else {
  //       this.pdfByteArrayForTranscript = response.transcriptPdf;
  //     }
  //   });
  // }

  backToList() {
    this.pdfByteArrayForTranscript = null
  }

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
          body, h1, h2, h3, h4, h5, h6, p { margin: 0; }

          body { -webkit-print-color-adjust: exact; }
          
          table { border-collapse: collapse; width: 100%; }
          
          .float-left { float: left; }
          
          .float-right { float: right; }
          
          .text-center { text-align: center; }
          
          .text-right { text-align: right; }
          
          .ml-auto { margin-left: auto; }
          
          .m-auto { margin: auto; }
          
          .report-card { width: 900px; margin: auto; font-family: \"Roboto\", \"Helvetica Neue\"; }
          
          .report-card-header td { padding: 20px 10px; }
          
          .header-left h2 { font-weight: 400; font-size: 30px; }
          
          .header-left p { margin: 5px 0; font-size: 15px; }
          
          .header-right { color: #040404; text-align: center; }
          
          .student-info-header { padding: 0px 30px 20px; }
          
          .student-info-header td { padding-bottom: 20px; vertical-align: top; }
          
          .student-info-header .info-left { padding-top: 20px; width: 100%; }
          
          .student-info-header .info-left h2 { font-size: 16px; margin-bottom: 8px; font-weight: 600; }
          
          .student-info-header .info-left .title { width: 150px; display: inline-block; }
          
          .student-info-header .info-left span:not(.title) { font-weight: 400; }
          
          .student-info-header .info-left p { margin-bottom: 10px; color: #333; }
          
          .student-info-header .info-right { padding-left: 10px; }
          
          .semester-table { padding: 0 30px 30px; vertical-align: top; }
          
          .semester-table table { border: 1px solid #000; }
          
          .semester-table th, .semester-table td { border-bottom: 1px solid #000; padding: 8px 15px; }
          
          .semester-table th { text-align: left; background-color: #e5e5e5; }
          
          .semester-table caption { margin-bottom: 10px; text-align: left; }
          
          .semester-table caption h2 { font-size: 18px; }
          
          .gpa-table { padding: 0 30px 30px; }
          
          .gpa-table table { border: 1px solid #000; }
          
          .gpa-table caption h4 { text-align: left; margin-bottom: 10px; font-weight: 500; }
          
          .gpa-table th { padding: 8px 15px; background-color: #e5e5e5; text-align: left; border-bottom: 1px solid #000; }
          
          .gpa-table td { padding: 8px 15px; text-align: left; }
          
          .signature-table { padding: 40px 30px; }
          
          .sign { padding-bottom: 20px; }
          
          .short-sign { padding-top: 60px; }
          
          .long-line { width: 90%; margin-bottom: 10px; border-top: 2px solid #000; }
          
          .small-line { display: inline-block; width: 150px; border-top: 2px solid #000; }
          
          .name { margin: 8px 0; }
          
          .text-uppercase { text-transform: uppercase; }
          
          .header-middle p { font-size: 22px; }
          
          .report-card-header td.header-right { vertical-align: top; padding-top: 58px; font-weight: 500; }
          
          .bevaior-table tr td:first-child { width: 20px; font-weight: 500; }
          
          .bevaior-table td { border-right: 1px solid #333; }
          
          .comments-table h2 { text-align: left; }
          
          .semester-table .comments-table caption { margin-bottom: 0; }
          
          .semester-table .comments-table { border: none; }
          
          .comments-table td { border-bottom: 1px dashed #b7b4b4; padding: 35px 0 0 } 
          
          </style>
        </head>
    <body onload="window.print()">${printContents}</body>
      </html>`
    );
    popupWin.document.close();
    document.getElementById('printSectionId').className = 'hidden';

    return;
  }

  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }

}
