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

import { Component, OnInit, ViewChild, Output, EventEmitter, OnDestroy, Input, AfterViewInit } from '@angular/core';
import { FormControl, NgForm } from '@angular/forms';
import { MatAccordion } from '@angular/material/expansion';
import { StudentService } from '../../../../services/student.service';
import { filterParams, StudentListModel, StudentMasterSearchModel } from '../../../../models/student.model';
import { GetAllSectionModel } from '../../../../models/section.model';
import { ReplaySubject, Subject } from 'rxjs';
import { take, takeUntil } from 'rxjs/operators';
import { CommonLOV } from '../../../shared-module/lov/common-lov';
import { SectionService } from '../../../../services/section.service';
import { CommonService } from '../../../../services/common.service';
import { LoginService } from '../../../../services/login.service';
import { CountryModel } from '../../../../models/country.model';
import { LanguageModel } from '../../../../models/language.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { StaffService } from '../../../../services/staff.service';
import { GetAllStaffModel, StaffMasterSearchModel } from '../../../../models/staff.model';
import { GetAllMembersList } from '../../../../models/membership.model';
import { MembershipService } from '../../../../services/membership.service';
import { SearchFilterAddViewModel } from '../../../../models/search-filter.model';
import { DefaultValuesService } from '../../../../common/default-values.service';
import { SharedFunction } from '../../../shared/shared-function';
import moment from 'moment';
import { MatSelect } from '@angular/material/select';
import { FilterParamsForAdvancedSearch } from 'src/app/models/common.model';

@Component({
  selector: 'vex-search-staff',
  templateUrl: './search-staff.component.html',
  styleUrls: ['./search-staff.component.scss']
})
export class SearchStaffComponent implements OnInit, AfterViewInit, OnDestroy {

  @ViewChild(MatAccordion) accordion: MatAccordion;
  @Output() showHideAdvanceSearch = new EventEmitter<any>();
  @Output() searchList = new EventEmitter<any>();
  @Input() filterJsonParams;
  @Output() searchValue = new EventEmitter<any>();
  @Input() incomingSearchValue;
  @Input() incomingToggleValues;
  @Output() toggelValues = new EventEmitter<any>();
  countryModel: CountryModel = new CountryModel();
  languages: LanguageModel = new LanguageModel();
  @ViewChild('f') currentForm: NgForm;
  destroySubject$: Subject<void> = new Subject();
  staffMasterSearchModel: StaffMasterSearchModel = new StaffMasterSearchModel();
  getAllStaff: GetAllStaffModel = new GetAllStaffModel();
  searchFilterAddViewModel: SearchFilterAddViewModel = new SearchFilterAddViewModel();
  dobEndDate: string;
  dobStartDate: string;
  searchTitle = 'search'
  showSaveFilter = true;
  updateFilter = false;
  params = [];
  countryListArr = [];
  ethnicityList = [];
  raceList = [];
  genderList = [];
  maritalStatusList = [];
  languageList;
  getAllMembersList: GetAllMembersList = new GetAllMembersList();
  searchAllSchool: boolean;
  inactiveStaff = false;
  checkSearchRecord = 0;
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
    private commonLOV: CommonLOV,
    private snackbar: MatSnackBar,
    private sectionService: SectionService,
    private commonService: CommonService,
    private loginService: LoginService,
    private staffService: StaffService,
    private defaultValuesService: DefaultValuesService,
    private commonFunction: SharedFunction,
    private membershipService: MembershipService
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
    this.getAllStaff.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
    if (this.incomingSearchValue) {
      this.inactiveStaff = this.incomingToggleValues.inactiveStaff;
      this.searchAllSchool = this.incomingToggleValues.searchAllSchool;
      this.staffMasterSearchModel = this.incomingSearchValue;
    }
    if (this.filterJsonParams !== null && this.filterJsonParams !== undefined) {
      this.updateFilter = true;
      this.searchTitle = 'searchAndUpdateFilter';
      let jsonResponse = JSON.parse(this.filterJsonParams.jsonList);
      for (let json of jsonResponse) {
        this.staffMasterSearchModel[json.columnName] = json.filterValue;
      }
    }

    this.initializeDropdownsInAddMode();
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
    this.getAllMembership();
  }

  callLOVs() {
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
      if (typeof (data) === 'undefined') {
        this.countryListArr = [];
      }
      else {
       if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
          if(data.tableCountry){
          this.countryListArr = [];
          }else{
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
            this.countryListArr = [];
          }
        } else {
          this.countryCtrl.setValue(data.tableCountry?.sort((a, b) => a.name < b.name ? -1 : 1));
          this.filteredCountry.next(data.tableCountry?.slice());
          this.nationalityCtrl.setValue(data.tableCountry?.sort((a, b) => a.name < b.name ? -1 : 1));
          this.filteredNationality.next(data.tableCountry?.slice());
          this.cobCtrl.setValue(data.tableCountry?.sort((a, b) => a.name < b.name ? -1 : 1));
          this.filteredCOB.next(data.tableCountry?.slice());
          this.countryListArr = data.tableCountry?.sort((a, b) => a.name < b.name ? -1 : 1);

        }
      }
    });
  }

  GetAllLanguage() {
    this.loginService.getAllLanguage(this.languages).pipe(takeUntil(this.destroySubject$)).subscribe((res) => {
      if (typeof (res) === 'undefined') {
        this.languageList = [];
      }else if(res._failure){
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
  includeInactiveStaff(event) {
    if (event.checked) {
      this.inactiveStaff = true;
    }
    else {
      this.inactiveStaff = false;
    }
  }

  getAllMembership() {
    this.membershipService.getAllMembers(this.getAllMembersList).subscribe((res) => {
      if (typeof (res) == 'undefined') {
        this.snackbar.open('Membership List failed. ' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
      else {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          this.getAllMembersList.getAllMemberList = [];
          if (!res.getAllMemberList) {
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
        }
        else {
          this.getAllMembersList.getAllMemberList = res.getAllMemberList.filter((item) => {
            return (item.profileType == 'School Administrator' || item.profileType == 'Admin Assistant'
              || item.profileType == 'Teacher' || item.profileType == 'Homeroom Teacher')
          });
        }
      }
    })
  }

  submit() {
    this.checkSearchRecord = 1;
    this.search();
  }

  search() {
    this.getAllStaff.filterParams = [];
    if (Array.isArray(this.staffMasterSearchModel.nationality)) {
      this.staffMasterSearchModel.nationality = null;
    }
    if (Array.isArray(this.staffMasterSearchModel.homeAddressCountry)) {
      this.staffMasterSearchModel.homeAddressCountry = null;
    }
    if (Array.isArray(this.staffMasterSearchModel.countryOfBirth)) {
      this.staffMasterSearchModel.countryOfBirth = null;
    }
    for (let key in this.staffMasterSearchModel) {
      if (this.staffMasterSearchModel.hasOwnProperty(key))
        if (this.staffMasterSearchModel[key] !== null && this.staffMasterSearchModel[key] !== '' && this.staffMasterSearchModel[key] !== undefined) {
          this.getAllStaff.filterParams.push(new FilterParamsForAdvancedSearch());
          const lastIndex = this.getAllStaff.filterParams.length - 1;
          if (key === 'joiningDate' || key === 'endDate' || key === 'dob') {
            this.getAllStaff.filterParams[lastIndex].columnName = key;
            this.getAllStaff.filterParams[lastIndex].filterOption = 11;
            this.getAllStaff.filterParams[lastIndex].filterValue = this.commonFunction.formatDateSaveWithoutTime(this.staffMasterSearchModel[key]);
          }
          else {
            this.getAllStaff.filterParams[lastIndex].columnName = key;
            this.getAllStaff.filterParams[lastIndex].filterValue = this.staffMasterSearchModel[key];
          }
        }
    }

    if (this.updateFilter) {
      this.showSaveFilter = false;
      this.searchFilterAddViewModel.searchFilter.filterId = this.filterJsonParams.filterId;
      this.searchFilterAddViewModel.searchFilter.module = 'Staff';
      this.searchFilterAddViewModel.searchFilter.jsonList = JSON.stringify(this.getAllStaff.filterParams);
      this.searchFilterAddViewModel.searchFilter.filterName = this.filterJsonParams.filterName;
      this.commonService.updateSearchFilter(this.searchFilterAddViewModel).subscribe((res) => {
        if (typeof (res) === 'undefined') {
          this.snackbar.open('Search filter updated failed' + this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
          this.checkSearchRecord = 0;
        }
        else {
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
            this.checkSearchRecord = 0;
          }
          else {
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
            this.checkSearchRecord = 0;
          }
        }
      }
      );
    }

    this.getAllStaff.sortingModel = null;
    this.getAllStaff.searchAllSchool = this.searchAllSchool;
    this.getAllStaff.includeInactive = this.inactiveStaff;
    this.getAllStaff.dobStartDate = this.commonFunction.formatDateSaveWithoutTime(this.dobStartDate);
    this.getAllStaff.dobEndDate = this.commonFunction.formatDateSaveWithoutTime(this.dobEndDate);
    this.commonService.setSearchResult(this.getAllStaff.filterParams);
    // this.getAllStaff.searchAllSchool = this.searchAllSchool;
    // this.getAllStaff.includeInactive = this.inactiveStaff;
    this.defaultValuesService.sendAllSchoolFlag(this.searchAllSchool);
      this.defaultValuesService.sendIncludeInactiveFlag(this.inactiveStaff);
    this.staffService.getAllStaffList(this.getAllStaff).subscribe(res => {
    if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
        this.searchList.emit([]);
        this.searchValue.emit(this.currentForm.value);
        this.toggelValues.emit({ inactiveStaff: this.inactiveStaff, searchAllSchool: this.searchAllSchool });
        this.snackbar.open(res._message, '', {
          duration: 10000
        });
        this.checkSearchRecord = 0;
      } else {
        let outStafflist = res;
        for (let staff of outStafflist.staffMaster) {
          if (staff.isActive === true || staff.isActive === null) {
          if (staff?.staffSchoolInfo[0]?.endDate) {
            let today = moment().format('DD-MM-YYYY').toString();
            let todayarr = today.split('-');
            let todayDate = +todayarr[0];
            let todayMonth = +todayarr[1];
            let todayYear = +todayarr[2];
            let endDate = moment(staff.staffSchoolInfo[0].endDate).format('DD-MM-YYYY').toString();
            let endDateArr = endDate.split('-');
            let endDateDate = +endDateArr[0];
            let endDateMonth = +endDateArr[1];
            let endDateYear = +endDateArr[2];
            if (endDateYear === todayYear) {
              if (endDateMonth === todayMonth) {
                if (endDateDate >= todayDate) {
                  staff.status = 'active';
                }
                else {
                  staff.status = 'inactive';
                }
              }
              else if (endDateMonth < todayMonth) {
                staff.status = 'inactive';
              }
              else {
                staff.status = 'active';
              }
            }
            else if (endDateYear < todayYear) {
              staff.status = 'inactive';
            }
            else {
              staff.status = 'active';
            }
          }
          else {
            staff.status = 'active';
          }
          } else {
            staff.status = 'inactive';
          }
        }
        this.searchList.emit(outStafflist);
        this.searchValue.emit(this.currentForm.value);
        this.toggelValues.emit({ inactiveStaff: this.inactiveStaff, searchAllSchool: this.searchAllSchool });
        this.showHideAdvanceSearch.emit({ showSaveFilter: this.showSaveFilter, hide: false });
        this.checkSearchRecord = 0;
      }
    });
  }

  resetData() {
    this.checkSearchRecord = 2;
    this.currentForm.reset();
    this.staffMasterSearchModel = new StaffMasterSearchModel();
    this.inactiveStaff = false;
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
