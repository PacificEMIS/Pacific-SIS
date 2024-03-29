import { Component, OnInit, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatCheckbox } from '@angular/material/checkbox';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableDataSource } from '@angular/material/table';
import { TranslateService } from '@ngx-translate/core';
import moment from 'moment';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { fadeInRight400ms } from 'src/@vex/animations/fade-in-right.animation';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { filterParams } from 'src/app/models/get-access-log.model';
import { GradeScaleListView } from 'src/app/models/grades.model';
import { GetStudentProgressReportModel } from 'src/app/models/report.model';
import { StudentListByDateRangeModel, StudentListModel } from 'src/app/models/student.model';
import { CommonService } from 'src/app/services/common.service';
import { ExcelService } from 'src/app/services/excel.service';
import { GradesService } from 'src/app/services/grades.service';
import { LoaderService } from 'src/app/services/loader.service';
import { ReportService } from 'src/app/services/report.service';
import { StudentService } from 'src/app/services/student.service';
import { MatPaginatorIntl } from '@angular/material/paginator';
import { CourseManagerService } from 'src/app/services/course-manager.service';
import { CourseSectionByStaffModel } from 'src/app/models/course-manager.model';
import { ProfilesTypes } from 'src/app/enums/profiles.enum';
import { ScheduleStudentListViewModel } from 'src/app/models/student-schedule.model';
import { StudentScheduleService } from 'src/app/services/student-schedule.service';
import { AdvancedSearchExpansionModel } from 'src/app/models/common.model';
import { SharedFunction } from 'src/app/pages/shared/shared-function';
export interface StudentListsData {
  studentCheck: boolean;
  studentName: string;
  studentId: string;
  alternateId: string;
  grade: string;
  section: string;
  phone: string;
}

export const studentListsData: StudentListsData[] = [
  { studentCheck: true, studentName: 'Arthur Boucher', studentId: 'STD0012', alternateId: 'STD0012', grade: 'Grade 11', section: 'Section A', phone: '7654328967' },
  { studentCheck: true, studentName: 'Sophia Brown', studentId: 'STD0015', alternateId: 'STD0015', grade: 'Grade 10', section: 'Section B', phone: '5654328967' },
  { studentCheck: true, studentName: 'Wang Wang', studentId: 'STD0035', alternateId: 'STD0035', grade: 'Grade 11', section: 'Section A', phone: '7654328967' },
  { studentCheck: true, studentName: 'Clare Garcia', studentId: 'STD0102', alternateId: 'STD0102', grade: 'Grade 11', section: 'Section A', phone: '9854328967' },
  { studentCheck: true, studentName: 'Amelia Jones', studentId: 'STD0067', alternateId: 'STD0067', grade: 'Grade 11', section: 'Section B', phone: '9654328967' },
  { studentCheck: true, studentName: 'Arthur Boucher', studentId: 'STD0013', alternateId: 'STD0013', grade: 'Grade 9', section: 'Section A', phone: '7654328967' },
  { studentCheck: true, studentName: 'Sophia Brown', studentId: 'STD0052', alternateId: 'STD0052', grade: 'Grade 9', section: 'Section B', phone: '7654328967' },
  { studentCheck: true, studentName: 'Wang Wang', studentId: 'STD0035', alternateId: 'STD0035', grade: 'Grade 10', section: 'Section A', phone: '6543212367' },
  { studentCheck: true, studentName: 'Clare Garcia', studentId: 'STD0102', alternateId: 'STD0102', grade: 'Grade 11', section: 'Section A', phone: '7654328967' },
  { studentCheck: true, studentName: 'Amelia Jones', studentId: 'STD0067', alternateId: 'STD0067', grade: 'Grade 9', section: 'Section A', phone: '9654328967' },
];

@Component({
  selector: 'vex-progress-report',
  templateUrl: './progress-report.component.html',
  styleUrls: ['./progress-report.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms,
    fadeInRight400ms
  ]
})
export class ProgressReportComponent implements OnInit {

  // displayedColumns: string[] = ['studentCheck', 'studentName', 'studentId', 'alternateId', 'grade', 'section', 'phone'];
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

  studentLists = studentListsData;
  getAllStudent: StudentListModel = new StudentListModel();
  getStudentProgressReportModel: GetStudentProgressReportModel = new GetStudentProgressReportModel();
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
  today: Date = new Date();
  gradeScaleListView: GradeScaleListView = new GradeScaleListView();
  filterParams: filterParams[] = [];

  defaultStudentPhoto = "/9j/4QAYRXhpZgAASUkqAAgAAAAAAAAAAAAAAP/sABFEdWNreQABAAQAAABkAAD/4QMtaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wLwA8P3hwYWNrZXQgYmVnaW49Iu+7vyIgaWQ9Ilc1TTBNcENlaGlIenJlU3pOVGN6a2M5ZCI/PiA8eDp4bXBtZXRhIHhtbG5zOng9ImFkb2JlOm5zOm1ldGEvIiB4OnhtcHRrPSJBZG9iZSBYTVAgQ29yZSA3LjAtYzAwMCA3OS5kYWJhY2JiLCAyMDIxLzA0LzE0LTAwOjM5OjQ0ICAgICAgICAiPiA8cmRmOlJERiB4bWxuczpyZGY9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkvMDIvMjItcmRmLXN5bnRheC1ucyMiPiA8cmRmOkRlc2NyaXB0aW9uIHJkZjphYm91dD0iIiB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iIHhtbG5zOnhtcE1NPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvbW0vIiB4bWxuczpzdFJlZj0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL3NUeXBlL1Jlc291cmNlUmVmIyIgeG1wOkNyZWF0b3JUb29sPSJBZG9iZSBQaG90b3Nob3AgMjIuNSAoV2luZG93cykiIHhtcE1NOkluc3RhbmNlSUQ9InhtcC5paWQ6QUE3MTBDNDQwNDk2MTFFQzg4Q0Y5N0JCOEU0Q0FGNTkiIHhtcE1NOkRvY3VtZW50SUQ9InhtcC5kaWQ6QUE3MTBDNDUwNDk2MTFFQzg4Q0Y5N0JCOEU0Q0FGNTkiPiA8eG1wTU06RGVyaXZlZEZyb20gc3RSZWY6aW5zdGFuY2VJRD0ieG1wLmlpZDpBQTcxMEM0MjA0OTYxMUVDODhDRjk3QkI4RTRDQUY1OSIgc3RSZWY6ZG9jdW1lbnRJRD0ieG1wLmRpZDpBQTcxMEM0MzA0OTYxMUVDODhDRjk3QkI4RTRDQUY1OSIvPiA8L3JkZjpEZXNjcmlwdGlvbj4gPC9yZGY6UkRGPiA8L3g6eG1wbWV0YT4gPD94cGFja2V0IGVuZD0iciI/Pv/uAA5BZG9iZQBkwAAAAAH/2wCEAAEBAQEBAQEBAQEBAQEBAQEB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"

  toggleMenu = {
    assignedDate: false,
    excludeUngradedEcAssignments: true,
    dueDate: true,
    excludeUngradedAssignmentsNotDue: false,
  }

  gradeScaleList: any;
  courseSectionByStaffModel: CourseSectionByStaffModel = new CourseSectionByStaffModel();
  profile = ProfilesTypes;
  selectedSubject = 'all';
  selectedCourse = 'all';
  selectedCourseSection = 'all';
  subjectDetails = [];
  isFromAdvancedSearch: boolean = false;
  scheduleStudentListViewModel: ScheduleStudentListViewModel = new ScheduleStudentListViewModel();
  advancedSearchExpansionModel: AdvancedSearchExpansionModel = new AdvancedSearchExpansionModel();
  filterParameters = [];
  
  constructor(
    public translateService: TranslateService,
    private studentService: StudentService,
    private commonService: CommonService,
    private snackbar: MatSnackBar,
    public defaultValuesService: DefaultValuesService,
    private loaderService: LoaderService,
    private reportService: ReportService,
    private gradesService: GradesService,
    private excelService: ExcelService,
    private paginatorObj: MatPaginatorIntl,
    private courseManagerService: CourseManagerService,
    private studentScheduleService: StudentScheduleService,
    private commonFunction: SharedFunction
  ) {
    this.advancedSearchExpansionModel.accessInformation = false;
    this.advancedSearchExpansionModel.enrollmentInformation = true;
    this.advancedSearchExpansionModel.searchAllSchools = false;
    this.loaderService.isLoading.subscribe((val) => {
      this.loading = val;
    });
    paginatorObj.itemsPerPageLabel = translateService.instant('itemsPerPage');
  
  }

  ngOnInit(): void {
    this.getAllStudent.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
    this.getAllStudentList();
    if(this.defaultValuesService.getUserMembershipType() === this.profile.Teacher || this.defaultValuesService.getUserMembershipType() === this.profile.HomeroomTeacher) {
      this.scheduleStudentListViewModel.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
      this.getCourseSectionByStaff().then(()=>{
        this.getStudentListByCourseSection();
      });
    } else {
      this.getAllStudent.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
      this.getAllStudentList();
    }
    
    this.searchCtrl = new FormControl();
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
        Object.assign(this.getAllStudent, { filterParams: filterParams });
        this.filterParams = filterParams;
        this.getAllStudent.pageNumber = 1;
        this.paginator.pageIndex = 0;
        this.getAllStudent.pageSize = this.pageSize;
        this.getAllStudentList();
      }
      else {
        Object.assign(this.getAllStudent, { filterParams: null });
        this.filterParams = null;
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
    this.studentService.GetAllStudentList(this.getAllStudent).subscribe(data => {
      if (data._failure) {
        this.commonService.checkTokenValidOrNot(data._message);
        if (data.studentListViews === null) {
          this.totalCount = this.isFromAdvancedSearch ? 0 : null;
          this.searchCount = this.isFromAdvancedSearch ? 0 : null;
          this.studentModelList = new MatTableDataSource([]);
          this.snackbar.open(data._message, '', {
            duration: 10000
          });
          this.isFromAdvancedSearch = false;
        } else {
          this.studentModelList = new MatTableDataSource([]);
          this.totalCount = this.isFromAdvancedSearch ? 0 : null;
          this.searchCount = this.isFromAdvancedSearch ? 0 : null;
          this.isFromAdvancedSearch = false;
        }
      } else {
        this.totalCount = data.totalCount;
        this.searchCount = data.totalCount;
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
        this.getAllStudent = new StudentListModel();
        this.isFromAdvancedSearch = false;
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
    this.getAllStudent.filterParams = this.filterParameters;
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

  /* This is for get all data from the Advanced Search component and then call the API in this page 
  NOTE: We just get the filterParams Array from Search component
  */
  filterData(res) {
    this.filterParameters = res.filterParams;
    this.isFromAdvancedSearch = true;
    this.getAllStudent = new StudentListModel();
    this.scheduleStudentListViewModel = new ScheduleStudentListViewModel();
    this.getAllStudent.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
    this.scheduleStudentListViewModel.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
    if (res) {
      if (this.defaultValuesService.getUserMembershipType() === this.profile.HomeroomTeacher || this.defaultValuesService.getUserMembershipType() === this.profile.Teacher) {
        this.scheduleStudentListViewModel.filterParams = res.filterParams;
        this.scheduleStudentListViewModel.includeInactive = res.inactiveStudents;
        this.scheduleStudentListViewModel.dobStartDate = this.commonFunction.formatDateSaveWithoutTime(res.dobStartDate);
        this.scheduleStudentListViewModel.dobEndDate = this.commonFunction.formatDateSaveWithoutTime(res.dobEndDate);
        this.getStudentListByCourseSection();
      } else {
        this.getAllStudent.filterParams = res.filterParams;
        this.getAllStudent.includeInactive = res.inactiveStudents;
        this.getAllStudent.searchAllSchool = res.searchAllSchool;
        this.getAllStudent.dobStartDate = this.commonFunction.formatDateSaveWithoutTime(res.dobStartDate);
        this.getAllStudent.dobEndDate = this.commonFunction.formatDateSaveWithoutTime(res.dobEndDate);
        this.defaultValuesService.sendIncludeInactiveFlag(res.inactiveStudents);
        this.defaultValuesService.sendAllSchoolFlag(res.searchAllSchool);
        this.getAllStudentList();
      }
    }
  }

  getSearchResult(res) {
    this.getAllStudent = new StudentListModel();
    res.studentListViews = this.defaultValuesService.getUserMembershipType() === this.profile.Teacher || this.defaultValuesService.getUserMembershipType() === this.profile.HomeroomTeacher ? res.scheduleStudentForView : res.studentListViews;
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
    this.getAllStudent = new StudentListModel();
  }

  generateProgressReport() {
    if (this.selectedStudents.length === 0) {
      this.snackbar.open('Please select any student to generate report.', '', {
        duration: 2000
      });
      return;
    }
    // return;

    this.addAndGenerateProgressReportCard().then((res: any) => {
      this.generatedReportCardData = res

      if (!this.getStudentProgressReportModel.totalsOnly) {
        this.calculateAndGenerateData();
      } else {
        this.generatedReportCardData.schoolMasterListData.map((schoolDetails) => {
          schoolDetails.studentMasterListData.map((studentDetails) => {
            studentDetails.courseSectionListData.map((courseSection) => {
              courseSection.letterGrade = this.gradeFromPercent(courseSection.total, courseSection.gradeData);
              // courseSection.letterWeightedGrade = this.gradeFromPercent(courseSection.totalWeightedGrade.split('%')[0], courseSection.gradeData);
            })
          })
        })
      }
      setTimeout(() => {
        if (this.getStudentProgressReportModel.totalsOnly) {
          this.generatePdfForTotal();
        } else {
          this.generatePdfForAssignmentDetails();
        }
      }, 200 * this.generatedReportCardData.schoolMasterListData.length);
    });

  }

  calculateAndGenerateData() {
    this.generatedReportCardData.schoolMasterListData.map((schoolDetails) => {
      schoolDetails.studentMasterListData.map((studentDetails) => {
        studentDetails.courseSectionListData.map((courseSection) => {
          let totalAllowedMarks = 0;
          let totalAssignmentPoint = 0;
          let totalGradePercentage = 0;
          let totalWeightGrade = 0;
          let gradeStarCount = 0;

          // for grade = * only remove from array and for grade = 0 it will hide in html but it will count
          if (this.toggleMenu.excludeUngradedEcAssignments) {
            courseSection.gradeBookGradeListData = courseSection.gradeBookGradeListData.filter(x => x.grade !== '*' && x.allowedMarks !== '*');
          }
          if(this.toggleMenu.excludeUngradedAssignmentsNotDue){
            courseSection.gradeBookGradeListData = courseSection.gradeBookGradeListData.filter(x=> x.grade !== '*' && moment(this.commonFunction.formatDateSaveWithoutTime(new Date())).isSameOrBefore(this.commonFunction.formatDateSaveWithoutTime(x.dueDate)));
          }
          courseSection.gradeBookGradeListData.map((gradeDetails) => {
            // assignmentPoint should be null and due date future date

            totalAssignmentPoint += gradeDetails.assignmentPoint ? Number(gradeDetails.assignmentPoint) : 0;

            if (gradeDetails.grade !== '*' && gradeDetails.allowedMarks !== '*') {              
              totalGradePercentage += gradeDetails.grade ? Number(gradeDetails.grade) : 0;
              totalAllowedMarks += gradeDetails.allowedMarks ? Number(gradeDetails.allowedMarks) : 0;
              if (Number(gradeDetails.grade) === 0 && this.toggleMenu.excludeUngradedEcAssignments) {
                gradeDetails.hide = true;
              }
            } else {
              gradeStarCount += 1;
            }
            totalWeightGrade += gradeDetails.wieghtedGrade ? Number(gradeDetails.wieghtedGrade) : 0;
          });

          courseSection.total = totalAllowedMarks + '/' + totalAssignmentPoint;
          courseSection.totalGrade = (totalGradePercentage / (courseSection.gradeBookGradeListData.length - gradeStarCount)).toFixed(2);
          courseSection.totalWeightedGrade = (totalWeightGrade / (courseSection.gradeBookGradeListData.length - gradeStarCount)).toFixed(2);

          if (courseSection.gradeScaleType === 'Numeric' || courseSection.gradeScaleType === 'Ungraded') {
            courseSection.letterGrade = '';
            courseSection.letterWeightedGrade = '';
          } else if (courseSection.gradeScaleType === 'Teacher_Scale') {
            courseSection.letterGrade = this.gradeFromPercent(courseSection.totalGrade, courseSection.gradeData);
            courseSection.letterWeightedGrade = this.gradeFromPercent(courseSection.totalWeightedGrade, courseSection.gradeData);
          } else if (!courseSection.gradeScaleType) {
            courseSection.letterGrade = this.gradeFromPercent(courseSection.totalGrade, courseSection.gradeData);
            courseSection.letterWeightedGrade = this.gradeFromPercent(courseSection.totalWeightedGrade, courseSection.gradeData);
          }
        })
      })
    })
  }

  gradeFromPercent(percent, gradeData) {
    let returnvalue;
    let sortedData = [];
    sortedData = gradeData.sort((a, b) => b.breakoff - a.breakoff);
    sortedData.map((item, i) => {
      if (i === 0) {
        if (Number(percent) >= item.breakoff) {
          returnvalue = item.title;
        }
      } else {
        if (Number(percent) >= item.breakoff && Number(percent) < sortedData[i - 1].breakoff) {
          returnvalue = item.title;
        }
      }
    });
    return returnvalue;
  }

  addAndGenerateProgressReportCard() {
    this.getStudentProgressReportModel.studentGuids = this.selectedStudents.map((item) => {
      return item.studentGuid
    })


    return new Promise((resolve, reject) => {
      this.reportService.getStudentProgressReport(this.getStudentProgressReportModel).subscribe((res) => {
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

  generatePdfForTotal() {
    let printContents, popupWin;
    printContents = document.getElementById('printReportCardIdForTotal').innerHTML;
    document.getElementById('printReportCardIdForTotal').className = 'block';
    popupWin = window.open('', '_blank', 'top=0,left=0,height=100%,width=auto');
    if(popupWin === null || typeof(popupWin)==='undefined'){
      document.getElementById('printReportCardIdForTotal').className = 'hidden';
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
          .student-logo {
              padding: 20px 10px;
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
              padding: 20px 10px;
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
              width: 33.33%;
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
              background-color: #EAEAEA;
              border-radius: 5px;
              padding: 20px;
              box-shadow: none;
              display: flex;
          }
          .p-20 {
              padding: 20px;
          }
          .p-y-20 {
              padding-top: 20px;
              padding-bottom: 20px;
          }
          .p-x-10 {
              padding-left: 10px;
              padding-right: 10px;
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
          .m-r-10 {
              margin-right: 10px;
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
          .information-table {
              border: 1px solid #000;
              border-collapse: separate;
              border-spacing: 0;
              border-radius: 10px;
              margin-bottom: 30px;
          }
          .information-table th {
              border-bottom: 1px solid #000;
              padding: 8px 5px;
              text-align: left;
              vertical-align: top;
          }
          .information-table td {
              padding: 8px 5px;
              border-bottom: 1px solid #000;
          }
          .information-table tr:first-child th:first-child {
              border-top-left-radius: 10px;
          }
          .information-table tr:first-child th:last-child {
              border-top-right-radius: 10px;
          }
          .information-table tr:last-child td:first-child {
              border-bottom-left-radius: 10px;
          }
          .information-table tr:last-child td:last-child {
              border-bottom-right-radius: 10px;
          }
          .information-table tr:last-child td {
              border-bottom: none;
          }
          table td {
              vertical-align: top;
          }
          .bullet {
              width: 5px;
              height: 5px;
              border-radius: 100%;
              background-color: #000;
              margin: 0 20px;
              vertical-align: middle;
          }
          .report-header .header-left {
              width: 68%;
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
    document.getElementById('printReportCardIdForTotal').className = 'hidden';
    return;
    }
  }

  generatePdfForAssignmentDetails() {
    let printContents, popupWin;
    printContents = document.getElementById('printReportCardIdForAssignmentDetails').innerHTML;
    document.getElementById('printReportCardIdForAssignmentDetails').className = 'block';
    popupWin = window.open('', '_blank', 'top=0,left=0,height=100%,width=auto');
    if(popupWin === null || typeof(popupWin)==='undefined'){
      document.getElementById('printReportCardIdForAssignmentDetails').className = 'hidden';
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
          .student-logo {
              padding: 20px 10px;
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
              padding: 20px 10px;
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
              width: 33.33%;
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
              background-color: #EAEAEA;
              border-radius: 5px;
              padding: 20px;
              box-shadow: none;
              display: flex;
          }
          .p-20 {
              padding: 20px;
          }
          .p-y-20 {
              padding-top: 20px;
              padding-bottom: 20px;
          }
          .p-x-10 {
              padding-left: 10px;
              padding-right: 10px;
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
          .m-r-10 {
              margin-right: 10px;
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
          .information-table {
              border: 1px solid #000;
              border-collapse: separate;
              border-spacing: 0;
              border-radius: 10px;
              margin-bottom: 30px;
          }
          .information-table th {
              border-bottom: 1px solid #000;
              padding: 8px 5px;
              text-align: left;
              vertical-align: top;
          }
          .information-table td {
              padding: 8px 5px;
              border-bottom: 1px solid #000;
          }
          .information-table tr:first-child th:first-child {
              border-top-left-radius: 10px;
          }
          .information-table tr:first-child th:last-child {
              border-top-right-radius: 10px;
          }
          .information-table tr:last-child td:first-child {
              border-bottom-left-radius: 10px;
          }
          .information-table tr:last-child td:last-child {
              border-bottom-right-radius: 10px;
          }
          .information-table tr:last-child td {
              border-bottom: none;
          }
          table td {
              vertical-align: top;
          }
          .bullet {
              width: 5px;
              height: 5px;
              border-radius: 100%;
              background-color: #000;
              margin: 0 20px;
              vertical-align: middle;
          }
          .report-header .header-left {
              width: 68%;
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
    document.getElementById('printReportCardIdForAssignmentDetails').className = 'hidden';
    return;
    }
  }


  exportToExcel() {
    if (!this.totalCount) {
      this.snackbar.open('No Record Found. Failed to Export Students List', '', {
        duration: 5000
      });
      return;
    }
    let getStudentModelForExcel: StudentListModel = new StudentListModel();
    getStudentModelForExcel.pageNumber = 0;
    getStudentModelForExcel.pageSize = 0;
    getStudentModelForExcel.filterParams = this.filterParams ? this.filterParams : [];
    getStudentModelForExcel.sortingModel = null;
    this.studentService.GetAllStudentList(getStudentModelForExcel).subscribe(data => {
      if (data) {
        if (data._failure) {
          this.commonService.checkTokenValidOrNot(data._message);
          this.snackbar.open(data._message, '', {
            duration: 10000
          });
        } else {
          if (data.studentListViews.length > 0) {
            let formatData = [];
            let studentList;
            formatData = this.createDataSetForTable(data.studentListViews);
            studentList = formatData.map((item) => {
              const middleName = item.middleName ? ' ' + item.middleName + ' ' : ' ';
              return {
                [this.defaultValuesService.translateKey('name')]: item.firstGivenName + middleName + item.lastFamilyName,
                [this.defaultValuesService.translateKey('studentID')]: item.studentInternalId,
                [this.defaultValuesService.translateKey('alternateID')]: item.alternateId,
                [this.defaultValuesService.translateKey('gradeLevel')]: item.gradeLevelTitle,
                [this.defaultValuesService.translateKey('section')]: item.sectionName,
                [this.defaultValuesService.translateKey('telephone')]: item.homePhone,
                [this.defaultValuesService.translateKey('schoolName')]: item.schoolName
              };
            });
            this.excelService.exportAsExcelFile(studentList, 'Student_Progress Report');
          } else {
            this.snackbar.open('No Records Found. Failed to Export Students List', '', {
              duration: 5000
            });
          }
        }
      }
    });
  }

  createDataSetForTable(rawData) {
    rawData.map(item => {
      if (item.exitDate && item.exitCode) {
        item.exitDate = this.formatDate(item.exitDate);
        item.enrollmentDate = null;
        item.enrollmentCode = null;
      } else if (item.enrollmentDate) {
        item.enrollmentDate = this.formatDate(item.enrollmentDate);
      }
    });
    return rawData;
  }

  formatDate(date) {
    if (date) {
      return moment(date).format('MMM DD, YYYY');
    }
  }
  getCourseSectionByStaff() {
    return new Promise((resolve, reject)=> {
    this.courseSectionByStaffModel.staffId = this.defaultValuesService.getUserId();
    this.courseManagerService.getCourseSectionByStaff(this.courseSectionByStaffModel).subscribe((data: any) => {
      if (data._failure) {
 this.snackbar.open(data._message, '', {
            duration: 10000
          });
      } else {
        this.subjectDetails = data.subjectViewModels;
      }
      resolve('')
    });
  });
  }

  getStudentListByCourseSection() {
    if (this.scheduleStudentListViewModel.sortingModel?.sortColumn === "") {
      this.scheduleStudentListViewModel.sortingModel = null;
    }
    this.scheduleStudentListViewModel.courseSectionIds = [];

    if(this.selectedSubject === 'all') {      
      this.subjectDetails.map((subject)=>{
        subject.coursesViewModels.map((course)=>{
          course.courseSectionsViewModels.map((courseSection)=>{
            this.scheduleStudentListViewModel.courseSectionIds.push(courseSection.courseSectionId)
          })
        })
      })
    } else if(this.selectedCourse === 'all') {
      this.subjectDetails[this.selectedSubject].coursesViewModels.map((course)=>{
        course.courseSectionsViewModels.map((courseSection)=>{
          this.scheduleStudentListViewModel.courseSectionIds.push(courseSection.courseSectionId)
        })
      })
    } else if(this.selectedCourseSection === 'all') {
      this.subjectDetails[this.selectedSubject].coursesViewModels[this.selectedCourse].courseSectionsViewModels.map((courseSection)=>{
          this.scheduleStudentListViewModel.courseSectionIds.push(courseSection.courseSectionId)
      })
    } else {
      this.scheduleStudentListViewModel.courseSectionIds = [this.subjectDetails[this.selectedSubject].coursesViewModels[this.selectedCourse].courseSectionsViewModels[this.selectedCourseSection].courseSectionId]
    }

    this.studentScheduleService.searchScheduledStudentForGroupDrop(this.scheduleStudentListViewModel).subscribe((data: any) => {
      data.studentListViews = data.scheduleStudentForView ? data.scheduleStudentForView : null;
      delete data.scheduleStudentForView;
      if (data._failure) {
        if (data.studentListViews === null) {
          this.totalCount = this.isFromAdvancedSearch ? 0 : null;
          this.searchCount = this.isFromAdvancedSearch ? 0 : null;
          this.studentModelList = new MatTableDataSource([]);
          this.snackbar.open(data._message, '', {
            duration: 10000
          });
          this.isFromAdvancedSearch = false;
        } else {
          this.studentModelList = new MatTableDataSource([]);
          this.totalCount = this.isFromAdvancedSearch ? 0 : null;
          this.searchCount = this.isFromAdvancedSearch ? 0 : null;
          this.isFromAdvancedSearch = false;
        }
      } else {
        this.totalCount = data.totalCount;
        this.searchCount = data.totalCount ? data.totalCount : null;
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
        // this.scheduleStudentListViewModel = new ScheduleStudentListViewModel();
        this.isFromAdvancedSearch = false;
      }
    });
  }

}
