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

import { Component, OnInit, AfterViewInit, ViewChild, Output, EventEmitter, OnDestroy, Input } from '@angular/core';
import { FormControl, NgForm } from '@angular/forms';
import { MatAccordion } from '@angular/material/expansion';
import { ReplaySubject, Subject } from 'rxjs';
import { take, takeUntil } from 'rxjs/operators';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CountryModel } from '../../../../models/country.model';
import { LanguageModel } from '../../../../models/language.model';
import { StudentListModel, StudentMasterSearchModel } from '../../../../models/student.model';
import { SearchFilterAddViewModel } from '../../../../models/search-filter.model';
import { StudentService } from '../../../../services/student.service';
import { CommonLOV } from '../../../shared-module/lov/common-lov';
import { SectionService } from '../../../../services/section.service';
import { CommonService } from '../../../../services/common.service';
import { DefaultValuesService } from '../../../../common/default-values.service';
import { LoginService } from '../../../../services/login.service';
import { SharedFunction } from '../../../shared/shared-function';
import { GetAllSectionModel } from '../../../../models/section.model';
import { MatSelect } from '@angular/material/select';
import { ScheduleStudentListViewModel } from '../../../../models/student-schedule.model';
import { StudentScheduleService } from '../../../../services/student-schedule.service';
import { ProfilesTypes } from '../../../../enums/profiles.enum';
import { FilterParamsForAdvancedSearch } from '../../../../models/common.model';
import { RollingOptionsEnum } from '../../../../enums/rolling-retention-option.enum';
import { GradeLevelService } from '../../../../services/grade-level.service';
import { GetAllGradeLevelsModel } from '../../../../models/grade-level.model';
import { EnrollmentCodesService } from '../../../../services/enrollment-codes.service';
import { EnrollmentCodeListView } from '../../../../models/enrollment-code.model';
import icListAlt from '@iconify/icons-ic/twotone-list-alt';
import { MatDialog } from '@angular/material/dialog';
import { CourseSectionComponent } from '../course-section/course-section.component';
import { GetAllStudentListForFinalGradeModel } from 'src/app/models/student-final-grade.model';
import { FinalGradeService } from 'src/app/services/final-grade.service';
import { CourseManagerService } from 'src/app/services/course-manager.service';
import { GetAllCourseListModel, GetAllProgramModel, GetAllSubjectModel } from 'src/app/models/course-manager.model';

@Component({
  selector: 'vex-search-student-edit-report-card-grades',
  templateUrl: './search-student.component.html',
  styleUrls: ['./search-student.component.scss']
})
export class SearchStudentComponentForEditReportCardGrades implements OnInit, AfterViewInit, OnDestroy {

  @ViewChild(MatAccordion) accordion: MatAccordion;
  icListAlt = icListAlt;
  @Output() showHideAdvanceSearch = new EventEmitter<any>();
  @Output() searchList = new EventEmitter<any>();
  @Output() searchValue = new EventEmitter<any>();
  @Output() toggelValues = new EventEmitter<any>();
  @Output() sendCourseSectionData = new EventEmitter<any>();
  @Input() filterJsonParams;
  @Input() incomingSearchValue;
  @Input() incomingCourseSectionValue;
  @Input() incomingToggleValues;
  rollingOptions = Object.keys(RollingOptionsEnum);
  countryModel: CountryModel = new CountryModel();
  languages: LanguageModel = new LanguageModel();
  @ViewChild('f') currentForm: NgForm;
  destroySubject$: Subject<void> = new Subject();
  studentMasterSearchModel: StudentMasterSearchModel = new StudentMasterSearchModel();
  getAllStudent: StudentListModel = new StudentListModel();
  scheduleStudentListViewModel: ScheduleStudentListViewModel = new ScheduleStudentListViewModel();
  getAllStudentListForFinalGradeModel: GetAllStudentListForFinalGradeModel = new GetAllStudentListForFinalGradeModel();
  searchFilterAddViewModel: SearchFilterAddViewModel = new SearchFilterAddViewModel();
  getAllSubjectModel: GetAllSubjectModel = new GetAllSubjectModel();
  getAllProgramModel: GetAllProgramModel = new GetAllProgramModel();
  getAllCourseListModel: GetAllCourseListModel = new GetAllCourseListModel();
  dobEndDate: string;
  dobStartDate: string;
  showSaveFilter = true;
  params = [];
  searchTitle: string = 'search';
  updateFilter: boolean = false;
  checkSearchRecord = 0;
  countryListArr = [];
  ethnicityList = [];
  raceList = [];
  genderList = [];
  suffixList = [];
  subjectList = [];
  programList = [];
  courseList = [];
  maritalStatusList = [];
  salutationList = [];
  sectionList = [];
  enrollmentList = [];
  languageList;
  courseSectionName;
  searchAllSchool: boolean;
  inactiveStudents = false;
  getAllGradeLevelsModel: GetAllGradeLevelsModel = new GetAllGradeLevelsModel();
  enrollmentCodelistView: EnrollmentCodeListView = new EnrollmentCodeListView();
  countryCtrl: FormControl = new FormControl();
  countryFilterCtrl: FormControl = new FormControl();
  public filteredCountry: ReplaySubject<any> = new ReplaySubject<any>(1);
  nationalityCtrl: FormControl = new FormControl();
  nationalityFilterCtrl: FormControl = new FormControl();
  public filteredNationality: ReplaySubject<any> = new ReplaySubject<any>(1);
  cobCtrl: FormControl = new FormControl();
  cobFilterCtrl: FormControl = new FormControl();
  public filteredCOB: ReplaySubject<any> = new ReplaySubject<any>(1);
  @ViewChild('singleSelect') singleSelect: MatSelect;
  protected _onDestroy = new Subject<void>();
  public userType: string;
  profiles = ProfilesTypes;

  constructor(
    private studentService: StudentService,
    private commonLOV: CommonLOV,
    private snackbar: MatSnackBar,
    private sectionService: SectionService,
    private commonService: CommonService,
    private defaultValuesService: DefaultValuesService,
    private loginService: LoginService,
    public enrollmentCodeService: EnrollmentCodesService,
    private gradeLevelService: GradeLevelService,
    private studentScheduleService: StudentScheduleService,
    private commonFunction: SharedFunction,
    private dialog: MatDialog,
    private finalGradeService: FinalGradeService,
    private courseManagerService: CourseManagerService,
  ) { }
  protected setInitialValue() {
    this.filteredCountry
      .pipe(take(1), takeUntil(this._onDestroy))
      .subscribe(() => {
        this.singleSelect.compareWith = (a, b) => a && b && a.name === b.name;
      });
    this.filteredNationality
      .pipe(take(1), takeUntil(this._onDestroy))
      .subscribe(() => {
        this.singleSelect.compareWith = (a, b) => a && b && a.name === b.name;
      });
    this.filteredCOB
      .pipe(take(1), takeUntil(this._onDestroy))
      .subscribe(() => {
        this.singleSelect.compareWith = (a, b) => a && b && a.name === b.name;
      });
  }

  ngOnInit(): void {
    this.getAllStudentListForFinalGradeModel.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
    if (this.incomingSearchValue) {
      this.inactiveStudents = this.incomingToggleValues.inactiveStudents;
      this.searchAllSchool = this.incomingToggleValues.searchAllSchool;
      this.studentMasterSearchModel = this.incomingSearchValue;
    }
    if (this.incomingCourseSectionValue) {
      this.studentMasterSearchModel.courseSection = this.incomingCourseSectionValue;
      this.courseSectionName = this.defaultValuesService.getCourseSectionName();
    }
    if (this.filterJsonParams !== null && this.filterJsonParams !== undefined) {
      this.updateFilter = true;
      this.searchTitle = 'searchAndUpdateFilter';
      let jsonResponse = JSON.parse(this.filterJsonParams.jsonList);
      for (let json of jsonResponse) {
        this.studentMasterSearchModel[json.columnName] = json.filterValue;
      }
    }

    this.initializeDropdownsInAddMode();
    this.userType = this.defaultValuesService.getUserMembershipType();
  }

  openCourseSection() {
    this.dialog.open(CourseSectionComponent, {
      width: '800px',
      data: {
        subjectList: this.subjectList,
        programList: this.programList,
        courseList: this.courseList
      }
    }).afterClosed().subscribe(data => {
      if (data) {
        this.studentMasterSearchModel.courseSection = data.courseSectionId;
        this.courseSectionName = data.courseSectionName;
        this.defaultValuesService.setCourseSectionName(data.courseSectionName)
      }
    });
  }

  ngAfterViewInit() {
    this.countryValueChange();
    this.nationalityValueChange();
    this.cobValueChange();
  }
  countryValueChange() {
    this.countryFilterCtrl.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe((res) => {
        this.filterCountries();
      });
  }
  nationalityValueChange() {
    this.nationalityFilterCtrl.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe((res) => {
        this.filterNationalitis();
      });
  }
  cobValueChange() {
    this.cobFilterCtrl.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe((res) => {
        this.filterCOB();
      });
  }
  filterCountries() {
    if (!this.countryListArr) {
      return;
    }
    let search = this.countryFilterCtrl.value;
    if (!search) {
      this.filteredCountry.next(this.countryListArr.slice());
      return;
    }
    else {
      search = search.toLowerCase();
    }
    this.filteredCountry.next(
      this.countryListArr.filter(country => country.name.toLowerCase().indexOf(search) > -1)
    );
  }
  filterNationalitis() {
    if (!this.countryListArr) {
      return;
    }
    let search = this.nationalityFilterCtrl.value;
    if (!search) {
      this.filteredNationality.next(this.countryListArr.slice());
      return;
    }
    else {
      search = search.toLowerCase();
    }
    this.filteredNationality.next(
      this.countryListArr.filter(country => country.name.toLowerCase().indexOf(search) > -1)
    );
  }
  filterCOB() {
    if (!this.countryListArr) {
      return;
    }
    let search = this.cobFilterCtrl.value;
    if (!search) {
      this.filteredCOB.next(this.countryListArr.slice());
      return;
    }
    else {
      search = search.toLowerCase();
    }
    this.filteredCOB.next(
      this.countryListArr.filter(country => country.name.toLowerCase().indexOf(search) > -1)
    );
  }

  initializeDropdownsInAddMode() {
    this.callLOVs();
    this.getAllCountry();
    this.GetAllLanguage();
    this.getAllSection();
    this.getAllSubjectList();
    this.getAllProgramList();
    this.getAllCourse();
    this.getAllGradeLevelList()
    this.getAllStudentEnrollmentCode();
  }

  callLOVs() {
    this.commonLOV.getLovByName('Salutation').pipe(takeUntil(this.destroySubject$)).subscribe((res) => {
      this.salutationList = res;
    });
    this.commonLOV.getLovByName('Suffix').pipe(takeUntil(this.destroySubject$)).subscribe((res) => {
      this.suffixList = res;
    });
    this.commonLOV.getLovByName('Gender').pipe(takeUntil(this.destroySubject$)).subscribe((res) => {
      this.genderList = res;
    });
    this.commonLOV.getLovByName('Race').pipe(takeUntil(this.destroySubject$)).subscribe((res) => {
      this.raceList = res;
    });
    this.commonLOV.getLovByName('Ethnicity').pipe(takeUntil(this.destroySubject$)).subscribe((res) => {
      this.ethnicityList = res;
    });
    this.commonLOV.getLovByName('Marital Status').pipe(takeUntil(this.destroySubject$)).subscribe((res) => {
      this.maritalStatusList = res;
    });
  }

  getAllCountry() {
    this.commonService.GetAllCountry(this.countryModel).pipe(takeUntil(this.destroySubject$)).subscribe(data => {
      if (typeof (data) == 'undefined') {
        this.countryListArr = [];
      }
      else {
        if (data._failure) {
          this.commonService.checkTokenValidOrNot(data._message);
          this.countryListArr = [];
          if (!data.tableCountry) {
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
          }
        } else {
          this.countryCtrl.setValue(data.tableCountry?.sort((a, b) => a.name < b.name ? -1 : 1));
          this.filteredCountry.next(data.tableCountry?.slice());
          this.nationalityCtrl.setValue(data.tableCountry?.sort((a, b) => a.name < b.name ? -1 : 1));
          this.filteredNationality.next(data.tableCountry?.slice());
          this.cobCtrl.setValue(data.tableCountry?.sort((a, b) => a.name < b.name ? -1 : 1));
          this.filteredCOB.next(data.tableCountry?.slice());
          this.countryListArr = data.tableCountry?.sort((a, b) => a.name < b.name ? -1 : 1)

        }
      }
    })
  }

  GetAllLanguage() {
    this.loginService.getAllLanguage(this.languages).pipe(takeUntil(this.destroySubject$)).subscribe((res) => {
      if (typeof (res) == 'undefined') {
        this.languageList = [];
      } else if (res._failure) {
        this.commonService.checkTokenValidOrNot(res._message);
        this.languageList = [];
        if (!res.tableLanguage) {
          this.snackbar.open(res._message, '', {
            duration: 10000
          });
        }
      }
      else {
        this.languageList = res.tableLanguage?.sort((a, b) => { return a.locale < b.locale ? -1 : 1; })

      }
    });
  }
  searchAllSchools(event) {
    if (event.checked) {
      this.searchAllSchool = true;
    }
    else {
      this.searchAllSchool = false;
    }
  }
  includeInactiveStudents(event) {
    if (event.checked) {
      this.inactiveStudents = true;
    }
    else {
      this.inactiveStudents = false;
    }
  }

  getAllStudentEnrollmentCode() {
    this.enrollmentCodelistView.isListView = true;
    this.enrollmentCodeService.getAllStudentEnrollmentCode(this.enrollmentCodelistView).subscribe(
      (res: EnrollmentCodeListView) => {
        if (res) {
          if (res._failure) {
            this.commonService.checkTokenValidOrNot(res._message);
            this.enrollmentList = [];
          }
          else {
            this.enrollmentList = res.studentEnrollmentCodeList;
          }
        }
      }
    );
  }

  getAllGradeLevelList() {
    this.gradeLevelService.getAllGradeLevels(this.getAllGradeLevelsModel).subscribe(data => {
      if (data._failure) {
        this.commonService.checkTokenValidOrNot(data._message);
      }
      this.getAllGradeLevelsModel.tableGradelevelList = data.tableGradelevelList;
    });
  }

  getAllSection() {
    let section: GetAllSectionModel = new GetAllSectionModel();
    this.sectionService.GetAllSection(section).pipe(takeUntil(this.destroySubject$)).subscribe(data => {
      if (data._failure) {
        this.commonService.checkTokenValidOrNot(data._message);
        if (!data.tableSectionsList) {
          this.snackbar.open(data._message, '', {
            duration: 10000
          });
        }
      }
      else {
        this.sectionList = data.tableSectionsList;
      }

    });
  }

  getAllSubjectList() {
    this.courseManagerService.GetAllSubjectList(this.getAllSubjectModel).subscribe(data => {
      if (data) {
        if (data._failure) {
          this.commonService.checkTokenValidOrNot(data._message);
          this.subjectList = [];
          if (!data.subjectList) {
            this.snackbar.open(data._message, '', {
              duration: 1000
            });
          }
        } else {
          this.subjectList = data.subjectList;
        }
      } else {
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
          duration: 1000
        });
      }
    });
  }

  getAllProgramList() {
    this.courseManagerService.GetAllProgramsList(this.getAllProgramModel).subscribe(data => {
      if (data) {
        if (data._failure) {
          this.commonService.checkTokenValidOrNot(data._message);
          this.programList = [];
          if (!data.programList) {
            this.snackbar.open(data._message, '', {
              duration: 1000
            });
          }
        } else {
          this.programList = data.programList;
        }
      } else {
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
          duration: 1000
        });
      }
    });
  }

  getAllCourse() {
    this.courseManagerService.GetAllCourseList(this.getAllCourseListModel).subscribe(data => {
      if (data) {
        if (data._failure) {
          this.commonService.checkTokenValidOrNot(data._message);
          this.courseList = [];
          if (!data.courseViewModelList) {
            this.snackbar.open(data._message, '', {
              duration: 1000
            });
          }
        } else {
          this.courseList = data.courseViewModelList;
        }
      } else {
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    })
  }

  submit() {
    this.checkSearchRecord = 1;
    this.search();
  }

  search() {
    this.getAllStudentListForFinalGradeModel.filterParams = [];
    if (Array.isArray(this.studentMasterSearchModel.nationality)) {
      this.studentMasterSearchModel.nationality = null;
    }
    if (Array.isArray(this.studentMasterSearchModel.homeAddressCountry)) {
      this.studentMasterSearchModel.homeAddressCountry = null;
    }
    if (Array.isArray(this.studentMasterSearchModel.countryOfBirth)) {
      this.studentMasterSearchModel.countryOfBirth = null;
    }
    for (let key in this.studentMasterSearchModel) {
      if (this.studentMasterSearchModel.hasOwnProperty(key)) {
        if (this.studentMasterSearchModel[key] !== null && this.studentMasterSearchModel[key] !== '' && this.studentMasterSearchModel[key] !== undefined) {
          this.getAllStudentListForFinalGradeModel.filterParams.push(new FilterParamsForAdvancedSearch());
          const lastIndex = this.getAllStudentListForFinalGradeModel.filterParams.length - 1;
          if (key === 'dob' || key === 'estimatedGradDate' || key === 'enrollmentDate') {
            this.getAllStudentListForFinalGradeModel.filterParams[lastIndex].columnName = key;
            this.getAllStudentListForFinalGradeModel.filterParams[lastIndex].filterOption = 11;
            this.getAllStudentListForFinalGradeModel.filterParams[lastIndex].filterValue = this.commonFunction.formatDateSaveWithoutTime(this.studentMasterSearchModel[key]);
          } else if (this.studentMasterSearchModel[key] === false) {
            this.getAllStudentListForFinalGradeModel.filterParams[lastIndex].columnName = '';
            this.getAllStudentListForFinalGradeModel.filterParams[lastIndex].filterValue = '';
          } else {
            this.getAllStudentListForFinalGradeModel.filterParams[lastIndex].columnName = key;
            this.getAllStudentListForFinalGradeModel.filterParams[lastIndex].filterValue = this.studentMasterSearchModel[key];
          }
        }
      }
    }

    this.getAllStudentListForFinalGradeModel.includeInactive = this.inactiveStudents;
    this.getAllStudentListForFinalGradeModel.dobStartDate = this.commonFunction.formatDateSaveWithoutTime(this.dobStartDate);
    this.getAllStudentListForFinalGradeModel.dobEndDate = this.commonFunction.formatDateSaveWithoutTime(this.dobEndDate);
    this.commonService.setSearchResult(this.getAllStudentListForFinalGradeModel.filterParams);
    this.finalGradeService.getAllStudentListForFinalGrade(this.getAllStudentListForFinalGradeModel).subscribe(data => {
      if (data._failure) {
        this.commonService.checkTokenValidOrNot(data._message);
        this.searchList.emit([]);
        this.toggelValues.emit({ inactiveStudents: this.inactiveStudents, searchAllSchool: this.searchAllSchool });
        this.searchValue.emit(this.currentForm.value);
        this.sendCourseSectionData.emit(this.studentMasterSearchModel.courseSection);
        this.snackbar.open('' + data._message, '', {
          duration: 10000
        });
        this.checkSearchRecord = 0;
      } else {
        this.searchList.emit(data);
        this.toggelValues.emit({ inactiveStudents: this.inactiveStudents, searchAllSchool: this.searchAllSchool });
        this.searchValue.emit(this.currentForm.value);
        this.sendCourseSectionData.emit(this.studentMasterSearchModel.courseSection);
        this.showHideAdvanceSearch.emit({ showSaveFilter: this.showSaveFilter, hide: false });
        this.checkSearchRecord = 0;
      }
    });
  }

  resetData() {
    this.checkSearchRecord = 2;
    this.currentForm.reset();
    this.studentMasterSearchModel = new StudentMasterSearchModel();
    this.inactiveStudents = false;
    this.searchAllSchool = false;
    this.search();
  }

  hideAdvanceSearch() {
    this.showHideAdvanceSearch.emit({ showSaveFilter: null, hide: false });
  }

  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }

}
