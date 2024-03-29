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
import { StudentReenrollList } from '../../../models/student-re-enroll-list.model';
import { Router } from '@angular/router';
import { fadeInUp400ms } from '../../../../@vex/animations/fade-in-up.animation';
import { fadeInRight400ms } from '../../../../@vex/animations/fade-in-right.animation';
import { stagger40ms } from '../../../../@vex/animations/stagger.animation';
import { StudentEnrollmentModel, StudentEnrollmentSchoolListModel, StudentListModel, StudentMasterModel } from '../../../models/student.model';
import { StudentService } from '../../../services/student.service';
import { MatTableDataSource } from '@angular/material/table';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FormControl, NgForm } from '@angular/forms';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { MatPaginator, MatPaginatorIntl } from '@angular/material/paginator';
import { EnrollmentCodeListView } from '../../../models/enrollment-code.model';
import { MatCheckbox } from '@angular/material/checkbox';
import { LoaderService } from '../../../services/loader.service';
import { Subject } from 'rxjs';
import { MatSlideToggle } from '@angular/material/slide-toggle';
import { SharedFunction } from '../../shared/shared-function';
import { ExcelService } from '../../../services/excel.service';
import { DatePipe } from '@angular/common';
import { PageRolesPermission } from '../../../common/page-roles-permissions.service';
import { Permissions, RolePermissionListViewModel } from '../../../models/roll-based-access.model';
import { CommonService } from '../../../services/common.service';
import { DefaultValuesService } from '../../../common/default-values.service';
import { RollBasedAccessService } from '../../../services/roll-based-access.service';
import { ImageCropperService } from '../../../services/image-cropper.service';
import { ModuleIdentifier } from '../../../enums/module-identifier.enum';
import { SchoolCreate } from '../../../enums/school-create.enum';
import { AdvancedSearchExpansionModel } from 'src/app/models/common.model';
import { CalendarService } from 'src/app/services/calendar.service';
import { GetCalendarAndHolidayListModel } from 'src/app/models/calendar.model';
import { MatSort } from '@angular/material/sort';
@Component({
  selector: 'vex-student-re-enroll',
  templateUrl: './student-re-enroll.component.html',
  styleUrls: ['./student-re-enroll.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms,
    fadeInRight400ms
  ]
})
export class StudentReEnrollComponent implements OnInit {
  @ViewChild('f') currentForm: NgForm;
  totalCount: number = 0;
  pageNumber: number;
  pageSize: number;
  submitReenroll: boolean = true;
  showSuccessMessage: boolean = false;
  selectedSchoollName: string;
  studentName: string = '';
  showAllSchools: boolean = false;
  studentsNameList: string;
  startDate = new Date();
  endDate: string;
  loading: boolean = false;
  reenrollSchoolId: number;
  gradeId: number;
  moduleIdentifier = ModuleIdentifier;
  createMode = SchoolCreate;
  selectedSchoolId: number;
  gradeLevelTitle: string;
  enrollmentCode: number;
  enrollmentDate: Date=new Date();
  searchCtrl: FormControl;
  showAdvanceSearchPanel: boolean = false;
  getAllStudent: StudentListModel = new StudentListModel();
  studentReenrollList: MatTableDataSource<any>;
  destroySubject$: Subject<void> = new Subject();
  schoolListWithGradeLevelsAndEnrollCodes = [];
  enrollmentCodelist = [];
  gradeLavelList = [];
  listOfStudent = [];
  selectedStudent = [];
  searchValue: any = null;
  toggleValues: any = null;
  studentMasterList: [StudentMasterModel] = [new StudentMasterModel()];
  cloneStudentEnrollment: StudentEnrollmentModel = new StudentEnrollmentModel();
  studentEnrollmentModel: StudentEnrollmentModel = new StudentEnrollmentModel();
  schoolListWithGrades: StudentEnrollmentSchoolListModel = new StudentEnrollmentSchoolListModel();
  enrollmentCodeListView: EnrollmentCodeListView = new EnrollmentCodeListView();
  advancedSearchExpansionModel: AdvancedSearchExpansionModel = new AdvancedSearchExpansionModel();
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild('masterCheckBox') private masterCheckBox: MatCheckbox;
  @ViewChild(MatSort) sort: MatSort;
  permissions: Permissions;
  isFromAdvancedSearch: boolean = false;
  columns = [
    { label: 'studentCheck', property: 'studentCheck', type: 'text', visible: true },
    { label: 'name', property: 'lastFamilyName', type: 'text', visible: true },
    { label: 'studentID', property: 'studentId', type: 'text', visible: true },
    { label: 'lastGradeLevel', property: 'gradeLevelTitle', type: 'text', visible: true },
    // { label: 'mobilePhone', property: 'mobilePhone', type: 'text', visible: true },
    // { label: 'personalEmail', property: 'personalEmail', type: 'text', visible: true },
    { label: 'schoolName', property: 'schoolName', type: 'text', visible: true },
    { label: 'enrollmentDate', property: 'enrollmentDate', type: 'text', visible: true },
    { label: 'exitDate', property: 'exitDate', type: 'text', visible: true },
    { label: 'exitCode', property: 'exitCode', type: 'text', visible: true }
  ];
  getAllCalendarHoliday: GetCalendarAndHolidayListModel = new GetCalendarAndHolidayListModel();

  constructor(public translateService: TranslateService, private router: Router,
    private studentService: StudentService,
    private snackbar: MatSnackBar,
    private loaderService: LoaderService,
    private commonFunction: SharedFunction,
    private excelService: ExcelService,
    private pageRolePermissions: PageRolesPermission,
    public defaultValuesService: DefaultValuesService,
    private datePipe: DatePipe,
    private commonService: CommonService,
    private rollBasedAccessService: RollBasedAccessService,
    private imageCropperService: ImageCropperService,
    private paginatorObj: MatPaginatorIntl,
    private calendarService: CalendarService,
  ) {
    this.advancedSearchExpansionModel.accessInformation = false;
    this.advancedSearchExpansionModel.searchBirthdays = false;
    this.advancedSearchExpansionModel.enrollmentInformation = false;
    this.advancedSearchExpansionModel.reEnrollmentInformation = true;
    this.advancedSearchExpansionModel.addressInformation = false;
    this.advancedSearchExpansionModel.personalContactInformation = false;
    this.advancedSearchExpansionModel.includeInactiveStudents = false;
    paginatorObj.itemsPerPageLabel = translateService.instant('itemsPerPage');
    //translateService.use('en');
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });
  }

  ngOnInit(): void {
    this.permissions = this.pageRolePermissions.checkPageRolePermission()
    this.reenrollSchoolId = this.defaultValuesService.getSchoolID();
    this.getAllStudent.schoolId = this.defaultValuesService.getSchoolID();
    this.searchCtrl = new FormControl();
    this.searchForReEnrollStudent();
    this.getAllSchoolListWithGradeLevelsAndEnrollCodes();
  }

  ngAfterViewInit() {

    //  Sorting
    this.getAllStudent = new StudentListModel();
    this.sort.sortChange.subscribe((res) => {
      this.getAllStudent.pageNumber = this.pageNumber
      this.getAllStudent.pageSize = this.pageSize;
      this.getAllStudent.sortingModel.sortColumn = res.active;
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
      if (res.direction == "") {
        this.getAllStudent.sortingModel = null;
        this.searchForReEnrollStudent();
        this.getAllStudent = new StudentListModel();
        this.getAllStudent.sortingModel = null;
      } else {
        this.getAllStudent.sortingModel.sortDirection = res.direction;
        this.searchForReEnrollStudent();
      }
    });

    //  Searching
    this.searchCtrl.valueChanges.pipe(debounceTime(500), distinctUntilChanged()).subscribe((term) => {
      if (term != '') {
        let filterParams = [
          {
            columnName: null,
            filterValue: term,
            filterOption: 3
          }
        ]
        if (this.sort.active != undefined && this.sort.direction != "") {
          this.getAllStudent.sortingModel.sortColumn = this.sort.active;
          this.getAllStudent.sortingModel.sortDirection = this.sort.direction;
        }
        if (this.showAllSchools) {
          this.getAllStudent.schoolId = 0;
        }
        else {
        
          this.getAllStudent.schoolId=this.defaultValuesService.getSchoolID();
        }
        Object.assign(this.getAllStudent, { filterParams: filterParams });
        this.getAllStudent.pageNumber = 1;
        this.paginator.pageIndex = 0;
        this.getAllStudent.pageSize = this.pageSize;
        this.searchForReEnrollStudent();
      }
      else {
        if (this.showAllSchools) {
          this.getAllStudent.schoolId = 0;
        }
        else {
         
          this.getAllStudent.schoolId=this.defaultValuesService.getSchoolID();
        }
        Object.assign(this.getAllStudent, { filterParams: null });
        this.getAllStudent.pageNumber = this.paginator.pageIndex + 1;
        this.getAllStudent.pageSize = this.pageSize;
        if (this.sort.active != undefined && this.sort.direction != "") {
          this.getAllStudent.sortingModel.sortColumn = this.sort.active;
          this.getAllStudent.sortingModel.sortDirection = this.sort.direction;
        }
        this.searchForReEnrollStudent();
      }
    })
  }

  showAdvanceSearch() {
    this.showAdvanceSearchPanel = true;
  }

  showStudentDetails(element) {
    this.imageCropperService.enableUpload({ module: this.moduleIdentifier.STUDENT, upload: true, mode: this.createMode.VIEW });
    this.studentService.setStudentId(element.studentId);
    this.defaultValuesService.setSchoolID(element.schoolId, true);
    // this.defaultValuesService.setAcademicYear(element.studentEnrollment[0]?.academicYear, true);
    this.getPermissionForStudent();
  }

  getPermissionForStudent() {
    let rolePermissionListView: RolePermissionListViewModel = new RolePermissionListViewModel();
    this.rollBasedAccessService.getAllRolePermission(rolePermissionListView).subscribe((res: RolePermissionListViewModel) => {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
      } else{
        let permittedDetails = this.pageRolePermissions.getPermittedSubCategories('/school/students', res);
       if (permittedDetails.length) {
            this.studentService.setCategoryId(0);
            this.studentService.setCategoryTitle(permittedDetails[0].title);
            this.router.navigate([permittedDetails[0].path], {state: res});
        } else {
          this.defaultValuesService.setSchoolID(undefined);
          this.snackbar.open('Student didnot have permission to view details.', '', {
            duration: 10000
          });
        }
      }
    });
  }
  
  getPageEvent(event) {
    if (this.sort.active != undefined && this.sort.direction != "") {
      this.getAllStudent.sortingModel.sortColumn = this.sort.active;
      this.getAllStudent.sortingModel.sortDirection = this.sort.direction;
    }
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
    this.searchForReEnrollStudent();
  }

  someComplete(): boolean {
    let indetermine = false;
    for (let user of this.listOfStudent) {
      for (let selectedUser of this.selectedStudent) {
        if (user.studentGuid === selectedUser.studentGuid) {
          indetermine = true;
        }
      }
    }
    if (indetermine) {
      this.masterCheckBox.checked = this.listOfStudent.every((item) => {
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
    this.listOfStudent.forEach(user => { user.checked = event; });
    this.studentReenrollList = new MatTableDataSource(this.listOfStudent);
    this.decideCheckUncheck();
  }

  onChangeSelection(eventStatus: boolean, studentGuid) {
    for (let item of this.listOfStudent) {
      if (item.studentGuid === studentGuid) {
        item.checked = eventStatus;
        break;
      }
    }
    this.studentReenrollList = new MatTableDataSource(this.listOfStudent);
    this.masterCheckBox.checked = this.listOfStudent.every((item) => {
      return item.checked;
    });

    this.decideCheckUncheck();
  }

  decideCheckUncheck() {
    this.listOfStudent.map((item) => {
      let isIdIncludesInSelectedList = false;
      if (item.checked) {
        for (let selectedUser of this.selectedStudent) {
          if (item.studentGuid === selectedUser.studentGuid) {
            isIdIncludesInSelectedList = true;
            break;
          }
        }
        if (!isIdIncludesInSelectedList) {
          this.selectedStudent.push(item);
        }
      } else {
        for (let selectedUser of this.selectedStudent) {
          if (item.studentGuid === selectedUser.studentGuid) {
            this.selectedStudent = this.selectedStudent.filter((user) => user.studentGuid !== item.studentGuid);
            break;
          }
        }
      }
      isIdIncludesInSelectedList = false;

    });
    this.selectedStudent = this.selectedStudent.filter((item) => item.checked);
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  searchForReEnrollStudent() {
    return new Promise((resolve, reject) => {

      if (this.getAllStudent.sortingModel?.sortColumn == "") {
        this.getAllStudent.sortingModel = null
      }
      if (this.showAllSchools) {
        this.getAllStudent.schoolId = 0;
      }
      else {
      
        this.getAllStudent.schoolId = this.defaultValuesService.getSchoolID();
      }
      this.studentService.searchStudentListForReenroll(this.getAllStudent, this.getAllStudent.schoolId).subscribe(data => {
        if (data._failure) {
          this.commonService.checkTokenValidOrNot(data._message);
          if (data.studentMaster === null) {
            this.studentReenrollList = new MatTableDataSource([]);
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
          } else {
            this.studentReenrollList = new MatTableDataSource([]);
            this.listOfStudent = [];
            this.totalCount = 0;
            this.selectedStudent = [];
          }
        } else {
          this.selectedStudent = [];
          this.totalCount = data.totalCount;
          this.pageNumber = data.pageNumber;
          this.pageSize = data._pageSize;
          this.studentMasterList = data.studentMaster;

          this.studentMasterList.forEach(user => {
            user.checked = false
          });
          let response = this.studentMasterList.map((item) => {
            this.selectedStudent.map((selectedUser) => {
              if (item.studentGuid === selectedUser.studentGuid) {
                item.checked = true;
                return item;
              }
            });
            return item;
          });
          this.listOfStudent = response;
          this.masterCheckBox.checked = response.every((item) => {
            return item.checked;
          })
          this.studentReenrollList = new MatTableDataSource(data.studentMaster);
          this.getAllStudent = new StudentListModel();
        }
        resolve({});
      });
    });
  }

  getAllSchoolListWithGradeLevelsAndEnrollCodes() {
    this.studentService.studentEnrollmentSchoolList(this.schoolListWithGrades).subscribe(res => {
      if (res._failure) {
        this.commonService.checkTokenValidOrNot(res._message);
      }
      this.schoolListWithGradeLevelsAndEnrollCodes = res.schoolMaster;
      this.changeSchool(this.reenrollSchoolId);
    });
  }

  changeSchool(schoolId) {
    this.studentService.setSelectedSchoolId(schoolId);
    this.selectedSchoolId = schoolId;
    this.enrollmentCodelist = this.schoolListWithGradeLevelsAndEnrollCodes.filter(x => x.schoolId === +schoolId)[0].studentEnrollmentCode.filter(x => x.title === 'New' || x.title === 'Transferred In' || x.title === 'Rolled Over');

    this.gradeLavelList = this.schoolListWithGradeLevelsAndEnrollCodes.filter(x => x.schoolId === +schoolId)[0]?.gradelevels;
    this.getCalendarAndHolidayList(schoolId);
  }

  getCalendarAndHolidayList(schoolId: any) {
    this.getAllCalendarHoliday._tenantName = this.defaultValuesService.getDefaultTenant();
    this.getAllCalendarHoliday._token = this.defaultValuesService.getToken();
    this.getAllCalendarHoliday._userName = this.defaultValuesService.getUserName();
    this.getAllCalendarHoliday.schoolCalendar.academicYear = this.defaultValuesService.getAcademicYear();
    this.getAllCalendarHoliday.schoolCalendar.schoolId= schoolId;
    this.calendarService.GetCalendarAndHolidayList(this.getAllCalendarHoliday).subscribe((res: any) => {
    if (res._failure) {
      // this.snackbar.open('' + res._message, '', {
      //   duration: 10000
      // });
      this.enrollmentDate = null;
    }
    else {
      this.enrollmentDate = new Date();
      this.startDate = res?.schoolCalendar?.startDate;
      // this.endDate = res?.schoolCalendar?.endDate;
    }
  })
}

  changeGrade(gradeId) {
    this.gradeLevelTitle = this.schoolListWithGradeLevelsAndEnrollCodes.filter(x => x.schoolId == +this.selectedSchoolId)[0]?.gradelevels.filter(x => x.gradeId === +gradeId)[0]?.title;
  }

  reEnrollSelectedStudent() {
    this.currentForm.form.markAllAsTouched();
    if (this.currentForm.form.valid) {
      if (this.selectedStudent.length > 0) {
        this.getAllStudent.studentMaster = this.selectedStudent;
        this.getAllStudent.gradeLevelTitle = this.gradeLevelTitle;
        this.getAllStudent.gradeId = this.gradeId;
        this.getAllStudent.enrollmentDate = this.commonFunction.formatDateSaveWithoutTime(this.enrollmentDate);
        this.getAllStudent.enrollmentCode = this.enrollmentCode;
      
        this.getAllStudent.updatedBy=this.defaultValuesService.getUserName();
        this.studentService.reenrollmentForStudent(this.getAllStudent).subscribe(data => {
          if (data._failure) {
            this.commonService.checkTokenValidOrNot(data._message);

          }
          else {
            for (let student of data.studentMaster) {
              this.studentName += student.firstGivenName + ' ' + student.lastFamilyName + ', ';
            }
            this.studentsNameList = this.studentName.replace(/,\s*$/, "");
            this.selectedSchoollName = this.schoolListWithGradeLevelsAndEnrollCodes.filter(x => x.schoolId === +data.schoolId)[0].schoolName;
            this.showSuccessMessage = true;
            this.submitReenroll = false;
            this.currentForm.reset();
          }


        });

      } else {
        this.snackbar.open('Please select at least 1 student', '', {
          duration: 2000
        });
      }
    }
  }

  reenrollAnotherStudents() {
    this.getAllStudent = new StudentListModel();
    if (this.showAllSchools) {
      this.getAllStudent.schoolId = 0;
    }
    else {
      this.getAllStudent.schoolId=this.defaultValuesService.getSchoolID();
    }
    this.searchForReEnrollStudent().then(() => {
      this.showSuccessMessage = false;
      this.submitReenroll = true;
    });
    this.enrollmentDate=new Date();
    this.reenrollSchoolId = this.defaultValuesService.getSchoolID();
    this.getAllSchoolListWithGradeLevelsAndEnrollCodes();
  }

  exportToExcelReenroll() {

    if (this.studentReenrollList.data.length > 0) {
      let studentReenrollList = this.studentReenrollList?.data?.map((x) => {
        const middleName = x.middleName == null ? ' ' : ' ' + x.middleName + ' ';
        return {
          'Student Name': x.firstGivenName + middleName + x.lastFamilyName,
          'Student ID': x.studentInternalId,
          'Last Grade Level': x.studentEnrollment[0]?.gradeLevelTitle,
          'Mobile Phone': x.mobilePhone,
          'Personal Email': x.personalEmail,
          'Enrollment Date': this.datePipe.transform(x.studentEnrollment[0]?.enrollmentDate, 'MMM d, y'),
          'Exit Date': this.datePipe.transform(x.studentEnrollment[0]?.exitDate, 'MMM d, y'),
          'Exit Code': x.studentEnrollment[0]?.exitCode
        };
      });
      this.excelService.exportAsExcelFile(studentReenrollList, 'Students_Reenroll_List_')
    }
    else {
      this.snackbar.open('No Records Found. Failed to Export Students List', '', {
        duration: 5000
      });
    }
  }

  /* This is for get all data from the Advanced Search component and then call the API in this page 
  NOTE: We just get the filterParams Array from Search component
  */
  filterData(res) {
    this.isFromAdvancedSearch = true;
    this.getAllStudent = new StudentListModel();
    this.getAllStudent.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
    if (res) {
      this.getAllStudent.filterParams = res.filterParams;
      this.getAllStudent.includeInactive = res.inactiveStudents;
      this.getAllStudent.schoolId = res.searchAllSchool ? 0 : this.defaultValuesService.getSchoolID();
      this.showAllSchools = res.searchAllSchool;
      this.searchForReEnrollStudent();
    }
  }

  getSearchResult(res) {
    if (res?.data.studentMaster.length > 0) {
      this.totalCount = res?.data.totalCount;
      this.pageNumber = res.data.pageNumber;
      this.pageSize = res?.data.pageSize;
      this.showAllSchools = res.allSchool;
      this.studentReenrollList = new MatTableDataSource(res?.data.studentMaster);
      this.getAllStudent = new StudentListModel();
    }
    else {
      this.showAllSchools = res.allSchool;
      this.totalCount = 0;
      this.studentReenrollList = new MatTableDataSource([]);
      this.getAllStudent = new StudentListModel();
    }

  }

  getToggleValues(event) {
    this.toggleValues = event;
  }

  getSearchInput(event) {
    this.searchValue = event;
  }

  hideAdvanceSearch(event) {
    this.showAdvanceSearchPanel = false;

  }

}
