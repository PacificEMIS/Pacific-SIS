import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormControl, NgForm } from '@angular/forms';
import { MatAccordion } from '@angular/material/expansion';
import { MatSelect } from '@angular/material/select';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ReplaySubject, Subject } from 'rxjs';
import { take, takeUntil } from 'rxjs/operators';
import { DefaultValuesService } from '../../../../common/default-values.service';
import { ProfilesTypes } from '../../../../enums/profiles.enum';
import { CountryModel } from '../../../../models/country.model';
import { LanguageModel } from '../../../../models/language.model';
import { SearchFilterAddViewModel } from '../../../../models/search-filter.model';
import { GetAllSectionModel } from '../../../../models/section.model';
import { ScheduleStudentListViewModel } from '../../../../models/student-schedule.model';
import { StudentListModel, StudentMasterSearchModel } from '../../../../models/student.model';
import { CommonLOV } from '../../../../pages/shared-module/lov/common-lov';
import { SharedFunction } from '../../../../pages/shared/shared-function';
import { CommonService } from '../../../../services/common.service';
import { LoginService } from '../../../../services/login.service';
import { SectionService } from '../../../../services/section.service';
import { StudentScheduleService } from '../../../../services/student-schedule.service';
import { StudentService } from '../../../../services/student.service';
import { FilterParamsForAdvancedSearch } from 'src/app/models/common.model';

@Component({
  selector: 'vex-group-assign-search-student',
  templateUrl: './search-student.component.html',
  styleUrls: ['./search-student.component.scss']
})
export class SearchStudentComponent implements OnInit {

  @ViewChild(MatAccordion) accordion: MatAccordion;
  @Output() showHideAdvanceSearch = new EventEmitter<any>();
  @Output() searchList = new EventEmitter<any>();
  @Output() searchValue = new EventEmitter<any>();
  @Output() toggelValues = new EventEmitter<any>();
  @Input() filterJsonParams;
  @Input() incomingSearchValue;
  @Input() incomingToggleValues;
  countryModel: CountryModel = new CountryModel();
  languages: LanguageModel = new LanguageModel();
  @ViewChild('f') currentForm: NgForm;
  destroySubject$: Subject<void> = new Subject();
  studentMasterSearchModel: StudentMasterSearchModel = new StudentMasterSearchModel();
  getAllStudent: StudentListModel = new StudentListModel();
  scheduleStudentListViewModel: ScheduleStudentListViewModel= new ScheduleStudentListViewModel();
  searchFilterAddViewModel: SearchFilterAddViewModel = new SearchFilterAddViewModel();
  dobEndDate: string;
  dobStartDate: string;
  showSaveFilter= true;
  params = [];
  searchTitle :string= 'search';
  updateFilter: boolean = false;
  countryListArr = [];
  ethnicityList = [];
  raceList = [];
  genderList = [];
  suffixList = [];
  maritalStatusList = [];
  salutationList = [];
  sectionList = [];
  languageList;
  searchAllSchool: boolean;
  inactiveStudents = false;
  profiles=ProfilesTypes;
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

  constructor(
    private studentService: StudentService,
    private commonLOV: CommonLOV,
    private snackbar: MatSnackBar,
    private sectionService: SectionService,
    private commonService: CommonService,
    private defaultValuesService: DefaultValuesService,
    private loginService: LoginService,
    private studentScheduleService: StudentScheduleService,
    private commonFunction: SharedFunction,
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
    this.getAllStudent.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
    if (this.incomingSearchValue){
      this.inactiveStudents = this.incomingToggleValues.inactiveStudents;
      this.searchAllSchool = this.incomingToggleValues.searchAllSchool;
      this.studentMasterSearchModel = this.incomingSearchValue;
    }
    if (this.filterJsonParams !== null && this.filterJsonParams !== undefined) {
      this.updateFilter = true;
      this.searchTitle='searchAndUpdateFilter';
      let jsonResponse = JSON.parse(this.filterJsonParams.jsonList);
      for (let json of jsonResponse) {
        this.studentMasterSearchModel[json.columnName] = json.filterValue;
      }
    }

    this.initializeDropdownsInAddMode();
  }
  ngAfterViewInit(){
    this.countryValueChange();
    this.nationalityValueChange();
    this.cobValueChange();
  }
  countryValueChange(){
    this.countryFilterCtrl.valueChanges
    .pipe(takeUntil(this._onDestroy))
    .subscribe((res) => {
      this.filterCountries();
    });
  }
  nationalityValueChange(){
    this.nationalityFilterCtrl.valueChanges
    .pipe(takeUntil(this._onDestroy))
    .subscribe((res) => {
      this.filterNationalitis();
    });
  }
  cobValueChange(){
    this.cobFilterCtrl.valueChanges
    .pipe(takeUntil(this._onDestroy))
    .subscribe((res) => {
      this.filterCOB();
    });
  }
  filterCountries(){
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
  filterNationalitis(){
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
  filterCOB(){
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
       if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
          this.countryListArr = [];
          if(!data.tableCountry){
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
      }else if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
        this.languageList = [];
        if(!res.tableLanguage){
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
  searchAllSchools(event){
    if (event.checked){
      this.searchAllSchool = true;
    }
    else{
      this.searchAllSchool = false;
    }
  }
  includeInactiveStudents(event){
    if (event.checked){
      this.inactiveStudents = true;
    }
    else{
      this.inactiveStudents = false;
    }
  }

  getAllSection() {
    let section: GetAllSectionModel = new GetAllSectionModel();
    this.sectionService.GetAllSection(section).pipe(takeUntil(this.destroySubject$)).subscribe(data => {
     if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
          if(!data.tableSectionsList){
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


  submit() {
    this.scheduleStudentListViewModel.filterParams = [];
    this.getAllStudent.filterParams = [];
    if (Array.isArray(this.studentMasterSearchModel.nationality)){
      this.studentMasterSearchModel.nationality = null;
    }
    if (Array.isArray(this.studentMasterSearchModel.homeAddressCountry)){
      this.studentMasterSearchModel.homeAddressCountry = null;
    }
    if (Array.isArray(this.studentMasterSearchModel.countryOfBirth)){
      this.studentMasterSearchModel.countryOfBirth = null;
    }
    for (let key in this.studentMasterSearchModel) {
      if (this.studentMasterSearchModel.hasOwnProperty(key))
      if (this.studentMasterSearchModel[key] !== null && this.studentMasterSearchModel[key] !== '' && this.studentMasterSearchModel[key] !== undefined) {
        if (this.defaultValuesService.getUserMembershipType() === 'Homeroom Teacher' || this.defaultValuesService.getUserMembershipType() === 'Teacher') {
          this.scheduleStudentListViewModel.filterParams.push(new FilterParamsForAdvancedSearch());
          const lastIndex = this.scheduleStudentListViewModel.filterParams.length - 1;
          if (key === 'dob') {
            this.scheduleStudentListViewModel.filterParams[lastIndex].columnName = key;
            this.scheduleStudentListViewModel.filterParams[lastIndex].filterOption = 11;
            this.scheduleStudentListViewModel.filterParams[lastIndex].filterValue = this.commonFunction.formatDateSaveWithoutTime(this.studentMasterSearchModel[key]);
          } else {
            this.scheduleStudentListViewModel.filterParams[lastIndex].columnName = key;
            this.scheduleStudentListViewModel.filterParams[lastIndex].filterValue = this.studentMasterSearchModel[key];
          }
        } else {
          this.getAllStudent.filterParams.push(new FilterParamsForAdvancedSearch());
          const lastIndex = this.getAllStudent.filterParams.length - 1;
          if (key === 'dob') {
            this.getAllStudent.filterParams[lastIndex].columnName = key;
            this.getAllStudent.filterParams[lastIndex].filterOption = 11;
            this.getAllStudent.filterParams[lastIndex].filterValue = this.commonFunction.formatDateSaveWithoutTime(this.studentMasterSearchModel[key]);
          } else {
            this.getAllStudent.filterParams[lastIndex].columnName = key;
            this.getAllStudent.filterParams[lastIndex].filterValue = this.studentMasterSearchModel[key];
          }
        }
      }
    }



    if (this.updateFilter) {
      this.showSaveFilter = false;
      this.searchFilterAddViewModel.searchFilter.filterId = this.filterJsonParams.filterId;
      this.searchFilterAddViewModel.searchFilter.module = 'Student';
      if (this.defaultValuesService.getUserMembershipType() === 'Homeroom Teacher' || this.defaultValuesService.getUserMembershipType() === 'Teacher') {
        this.searchFilterAddViewModel.searchFilter.jsonList = JSON.stringify(this.scheduleStudentListViewModel.filterParams);
      } else {
        this.searchFilterAddViewModel.searchFilter.jsonList = JSON.stringify(this.getAllStudent.filterParams);
      }
      this.searchFilterAddViewModel.searchFilter.filterName = this.filterJsonParams.filterName;
      this.commonService.updateSearchFilter(this.searchFilterAddViewModel).subscribe((res) => {
        if (typeof (res) === 'undefined') {
          this.snackbar.open('Search filter updated failed' + this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
        else {
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
          else {
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
            
          }
        }
      }
      );
    }

    if (this.defaultValuesService.getUserMembershipType() === this.profiles.HomeroomTeacher || this.defaultValuesService.getUserMembershipType() === this.profiles.Teacher) {
      this.scheduleStudentListViewModel.staffId = this.defaultValuesService.getUserId();
      this.scheduleStudentListViewModel.academicYear = this.defaultValuesService.getAcademicYear();
      this.scheduleStudentListViewModel.searchAllSchool = this.searchAllSchool;
      this.scheduleStudentListViewModel.includeInactive = this.inactiveStudents;
      this.scheduleStudentListViewModel.sortingModel = null;
      this.scheduleStudentListViewModel.dobStartDate = this.commonFunction.formatDateSaveWithoutTime(this.dobStartDate);
      this.scheduleStudentListViewModel.dobEndDate = this.commonFunction.formatDateSaveWithoutTime(this.dobEndDate);
      this.commonService.setSearchResult(this.scheduleStudentListViewModel.filterParams);
      this.studentScheduleService.searchScheduledStudentForGroupDrop(this.scheduleStudentListViewModel).subscribe(data => {
       if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
          this.searchList.emit([]);
          this.toggelValues.emit({inactiveStudents: this.inactiveStudents, searchAllSchool: this.searchAllSchool});
          this.searchValue.emit(this.currentForm.value);
          this.snackbar.open('' + data._message, '', {
            duration: 10000
          });
  
        } else {
          this.searchList.emit(data);
          this.toggelValues.emit({inactiveStudents: this.inactiveStudents, searchAllSchool: this.searchAllSchool});
          this.searchValue.emit(this.currentForm.value);
          this.showHideAdvanceSearch.emit({ showSaveFilter: this.showSaveFilter , hide: false});
        }
      });
    }
    else{
      this.getAllStudent.searchAllSchool = this.searchAllSchool;
      this.getAllStudent.includeInactive = this.inactiveStudents;
      this.getAllStudent.sortingModel = null;
      this.getAllStudent.dobStartDate = this.commonFunction.formatDateSaveWithoutTime(this.dobStartDate);
      this.getAllStudent.dobEndDate = this.commonFunction.formatDateSaveWithoutTime(this.dobEndDate);
      this.commonService.setSearchResult(this.getAllStudent.filterParams);
      this.studentService.GetAllStudentList(this.getAllStudent).subscribe(data => {
       if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
          this.searchList.emit([]);
          this.toggelValues.emit({inactiveStudents: this.inactiveStudents, searchAllSchool: this.searchAllSchool});
          this.searchValue.emit(this.currentForm.value);
          this.snackbar.open('' + data._message, '', {
            duration: 10000
          });
  
        } else {
          this.searchList.emit(data);
          this.toggelValues.emit({inactiveStudents: this.inactiveStudents, searchAllSchool: this.searchAllSchool});
          this.searchValue.emit(this.currentForm.value);
          this.showHideAdvanceSearch.emit({ showSaveFilter: this.showSaveFilter , hide: false});
        }
      });
    }
    
  }

  resetData() {
    this.currentForm.reset();
    this.inactiveStudents = false;
    this.searchAllSchool = false;
    this.studentMasterSearchModel = new StudentMasterSearchModel();
    this.submit();
  }

  hideAdvanceSearch() {
    this.showHideAdvanceSearch.emit({ showSaveFilter: null , hide: false});
  }

  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }

}
