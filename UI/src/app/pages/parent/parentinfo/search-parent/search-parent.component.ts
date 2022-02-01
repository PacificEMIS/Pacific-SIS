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

import { Component, OnInit, ViewChild, Output, Input, EventEmitter, OnDestroy, OnChanges, SimpleChanges, AfterViewInit } from '@angular/core';
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
import { userProfile} from '../../../../enums/studentAdd.enum';
import { GetAllParentModel, GetAllParentResponseModel, ParentAdvanceSearchModel } from '../../../../models/parent-info.model';
import { ParentInfoService } from '../../../../services/parent-info.service';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { MatSelect } from '@angular/material/select';
import { FilterParamsForAdvancedSearch } from 'src/app/models/common.model';

@Component({
  selector: 'vex-search-parent',
  templateUrl: './search-parent.component.html',
  styleUrls: ['./search-parent.component.scss']
})
export class SearchParentComponent implements OnInit,AfterViewInit,OnDestroy {

  userProfileEnum = Object.keys(userProfile);
  @ViewChild(MatAccordion) accordion: MatAccordion;
  @Output() showHideAdvanceSearch = new EventEmitter<boolean>();
  @Output() searchList = new EventEmitter<any>();
  @Output() existingFilterParams = new EventEmitter<any>();
  countryModel: CountryModel = new CountryModel();
  @ViewChild('f') currentForm: NgForm;
  destroySubject$: Subject<void> = new Subject();
  parentSearchModel: ParentAdvanceSearchModel = new ParentAdvanceSearchModel();
  getAllParentModel: GetAllParentModel = new GetAllParentModel();
  countryListArr = [];
  advanceSearchParams:filterParams[] = []
  @Output() searchValue = new EventEmitter<any>();
  countryCtrl: FormControl = new FormControl();
  countryFilterCtrl: FormControl = new FormControl();
  public filteredCountry: ReplaySubject<any> = new ReplaySubject<any>(1);
  @ViewChild('singleSelect') singleSelect: MatSelect;
  protected _onDestroy = new Subject<void>();

  constructor(
    private snackbar: MatSnackBar,
    private parentInfoService: ParentInfoService,
    private commonService: CommonService,
    private defaultValuesService: DefaultValuesService) {

  }

  protected setInitialValue() {
    this.filteredCountry
    .pipe(take(1), takeUntil(this._onDestroy))
    .subscribe(() => {
      this.singleSelect.compareWith = (a, b) => a && b && a.name === b.name;
    });
  }

  ngOnInit(): void {
    this.getAllCountry();
  }

  ngAfterViewInit(){
    this.countryValueChange();
    let existingParams = this.parentInfoService.getAdvanceSearchParams();
    if(existingParams){
      this.parentSearchModel = existingParams;
    }else{
      this.parentSearchModel = new ParentAdvanceSearchModel();
    }
  }
  countryValueChange(){
    this.countryFilterCtrl.valueChanges
    .pipe(takeUntil(this._onDestroy))
    .subscribe((res) => {
      this.filterNationalitis();
    });
  }
  filterNationalitis(){
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


  getAllCountry() {
    this.commonService.GetAllCountry(this.countryModel).pipe(takeUntil(this.destroySubject$)).subscribe(data => {
      if (typeof (data) == 'undefined') {
        this.countryListArr = [];
      }
      else {
       if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
          if (data.tableCountry) {
            this.countryListArr = [];
          } else {
            this.countryListArr = [];
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
          }
        } else {
          this.countryCtrl.setValue(data.tableCountry?.sort((a, b) => a.name < b.name ? -1 : 1));
          this.filteredCountry.next(data.tableCountry?.slice());
          this.countryListArr = data.tableCountry?.sort((a, b) => a.name < b.name ? -1 : 1);

        }
      }
    });
  }


  submit() {
    this.getAllParentModel.filterParams = [];
    if (Array.isArray(this.parentSearchModel.country)) {
      this.parentSearchModel.country = null;
    }
    for (let key in this.parentSearchModel) {
      if (this.parentSearchModel.hasOwnProperty(key))
      if (this.parentSearchModel[key]) {
        this.getAllParentModel.filterParams.push(new FilterParamsForAdvancedSearch());
        const lastIndex = this.getAllParentModel.filterParams.length - 1;
        this.getAllParentModel.filterParams[lastIndex].columnName = key;
        this.getAllParentModel.filterParams[lastIndex].filterValue = this.parentSearchModel[key];
      }
    }
    this.getAllParentList();
  }

  getAllParentList(){
  this.getAllParentModel.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
  this.getAllParentModel.sortingModel = null;
    this.parentInfoService.getAllParentInfo(this.getAllParentModel).pipe(takeUntil(this.destroySubject$)).subscribe(
      (res: GetAllParentResponseModel) => {
        if(res) {
        this.searchValue.emit(this.getAllParentModel.filterParams);
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
              if (res.parentInfoForView){
                    this.searchList.emit(res);
              }
              else{
                this.searchList.emit([]);
                this.snackbar.open( res._message, '', {
                  duration: 10000
                });
              }
          }
          else {
            this.searchList.emit(res);
            this.existingFilterParams.emit(this.getAllParentModel.filterParams)
            this.showHideAdvanceSearch.emit(true);
          }
        }else{
          this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
      }
    );
  }

  resetData() {
    this.currentForm.reset();
    this.parentSearchModel = new ParentAdvanceSearchModel();
    this.existingFilterParams.emit(null)
    this.submit();
  }

  hideAdvanceSearch() {
    this.showHideAdvanceSearch.emit(false);
  }

  onProfileChange(event){
    this.parentSearchModel.userProfile = event.toString();
  }

  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }

}
