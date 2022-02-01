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

import { Component, OnInit, ViewChild, Output, EventEmitter, OnDestroy, Input } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MatAccordion } from '@angular/material/expansion';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CountryModel } from '../../../../../models/country.model';
import { LanguageModel } from '../../../../../models/language.model';
import { GetAllStaffModel, StaffMasterSearchModel } from '../../../../../models/staff.model';
import { SearchFilterAddViewModel } from '../../../../../models/search-filter.model';
import { GetAllMembersList } from '../../../../../models/membership.model';
import { CommonLOV } from '../../../../../pages/shared-module/lov/common-lov';
import { SectionService } from '../../../../../services/section.service';
import { CommonService } from '../../../../../services/common.service';
import { LoginService } from '../../../../../services/login.service';
import { StaffService } from '../../../../../services/staff.service';
import { SharedFunction } from '../../../../../pages/shared/shared-function';
import { MembershipService } from '../../../../../services/membership.service';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { FilterParamsForAdvancedSearch } from 'src/app/models/common.model';

@Component({
  selector: 'vex-search-staff',
  templateUrl: './search-staff.component.html',
  styleUrls: ['./search-staff.component.scss']
})
export class SearchStaffComponent implements OnInit {

  @ViewChild(MatAccordion) accordion: MatAccordion;
  @Output() showHideAdvanceSearch = new EventEmitter<any>();
  @Output() searchList = new EventEmitter<any>();
  @Input() filterJsonParams;
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
  checkSearchRecord = 0;
  params = [];
  countryListArr = [];
  ethnicityList = [];
  raceList = [];
  genderList = [];
  maritalStatusList = [];
  languageList;
  getAllMembersList: GetAllMembersList = new GetAllMembersList();

  constructor(
    private commonLOV: CommonLOV,
    private snackbar: MatSnackBar,
    private sectionService: SectionService,
    private commonService: CommonService,
    private loginService: LoginService,
    private staffService: StaffService,
    private commonFunction: SharedFunction,
    private membershipService: MembershipService,
    private defaultValuesService: DefaultValuesService
  ) { }

  ngOnInit(): void {
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
          this.countryListArr = [];
          if (!data.tableCountry) {
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
          }
        } else {
          this.countryListArr = data.tableCountry?.sort((a, b) => { return a.name < b.name ? -1 : 1; })

        }
      }
    })
  }

  GetAllLanguage() {
    this.languages._tenantName = this.defaultValuesService.getTenantName();
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
    this.checkSearchRecord =1;
    this.search();
  }

  search() {
    this.getAllStaff.filterParams = [];
    for (let key in this.staffMasterSearchModel) {
      if (this.staffMasterSearchModel.hasOwnProperty(key))
        if (this.staffMasterSearchModel[key] !== null && this.staffMasterSearchModel[key] !== '') {
          this.getAllStaff.filterParams.push(new FilterParamsForAdvancedSearch());
          const lastIndex = this.getAllStaff.filterParams.length - 1;
          if (key === 'joiningDate' || key === 'endDate' || key === 'dob') {
            this.getAllStaff.filterParams[lastIndex].columnName = key;
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
            this.checkSearchRecord = 0;
          }
        }
      }
      );
    }

    this.getAllStaff.sortingModel = null;
    this.getAllStaff.dobStartDate = this.commonFunction.formatDateSaveWithoutTime(this.dobStartDate);
    this.getAllStaff.dobEndDate = this.commonFunction.formatDateSaveWithoutTime(this.dobEndDate);
    this.commonService.setSearchResult(this.getAllStaff.filterParams);
    this.staffService.getAllStaffList(this.getAllStaff).subscribe(res => {
    if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
        this.searchList.emit([]);
        this.snackbar.open(res._message, '', {
          duration: 10000
        });
      } else {
        this.searchList.emit(res);
        this.showHideAdvanceSearch.emit({ showSaveFilter: this.showSaveFilter, hide: false });
        this.checkSearchRecord = 0;
      }
    });
  }

  resetData() {
    this.checkSearchRecord =2;
    this.currentForm.reset();
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
