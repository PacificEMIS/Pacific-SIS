import { AfterViewInit } from '@angular/core';
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

import {
  Component,
  OnInit,
  ViewChild,
  Output,
  EventEmitter,
  OnDestroy,
  Input,
} from "@angular/core";
import { FormControl, NgForm } from "@angular/forms";
import { MatAccordion } from "@angular/material/expansion";
import { StudentService } from "../../../../services/student.service";
import { CommonLOV } from "../../../shared-module/lov/common-lov";
import { SectionService } from "../../../../services/section.service";
import { CommonService } from "../../../../services/common.service";
import { LoginService } from "../../../../services/login.service";
import { MatSnackBar } from "@angular/material/snack-bar";
import icClose from "@iconify/icons-ic/twotone-close";
import { CountryModel } from "src/app/models/country.model";
import { LanguageModel } from "src/app/models/language.model";
import { ReplaySubject, Subject } from "rxjs";
import { StudentListModel, StudentMasterSearchModel } from "src/app/models/student.model";
import { SearchFilterAddViewModel } from "src/app/models/search-filter.model";
import { take, takeUntil } from "rxjs/operators";
import { SharedFunction } from "src/app/pages/shared/shared-function";
import { EnrollmentCodesService } from "src/app/services/enrollment-codes.service";
import { EnrollmentCodeListView } from "src/app/models/enrollment-code.model";
import { MatSlideToggle } from "@angular/material/slide-toggle";
import { MatSelect } from "@angular/material/select";

@Component({
  selector: "vex-search-student",
  templateUrl: "./search-student.component.html",
  styleUrls: ["./search-student.component.scss"],
})
export class SearchStudentComponent implements OnInit, AfterViewInit {
  icClose = icClose;
  @ViewChild(MatAccordion) accordion: MatAccordion;
  @Output() showHideAdvanceSearch = new EventEmitter<any>();
  @Output() searchList = new EventEmitter<any>();
  @Input() filterJsonParams;
  searchSchoolId:number = +sessionStorage.getItem('selectedSchoolId');
  countryModel: CountryModel = new CountryModel();
  languages: LanguageModel = new LanguageModel();
  @ViewChild('f') currentForm: NgForm;
  destroySubject$: Subject<void> = new Subject();
  studentMasterSearchModel: StudentMasterSearchModel = new StudentMasterSearchModel();
  getAllStudent: StudentListModel = new StudentListModel();
  searchFilterAddViewModel: SearchFilterAddViewModel = new SearchFilterAddViewModel();
  enrollmentCodeListView: EnrollmentCodeListView= new EnrollmentCodeListView();
  dobEndDate: string;
  dobStartDate: string;
  showAllSchools: boolean = false;
  checkSearchRecord = 0;
  showSaveFilter = true;
  params = [];
  searchTitle: string = 'search';
  updateFilter: boolean = false;
  countryListArr = [];
  ethnicityList = [];
  raceList = [];
  genderList = [];
  maritalStatusList = [];
  enrollmentList=[];
  languageList;
  countryCtrl: FormControl = new FormControl();
  countryFilterCtrl: FormControl = new FormControl();
  public filteredCountry: ReplaySubject<any> = new ReplaySubject<any>(1);
  nationalityCtrl: FormControl = new FormControl();
  nationalityFilterCtrl: FormControl = new FormControl();
  public filteredNationality: ReplaySubject<any> = new ReplaySubject<any>(1);
  countryOfBirthCtrl: FormControl = new FormControl();
  countryOfBirthFilterCtrl: FormControl = new FormControl();
  public filteredCountryOfBirth: ReplaySubject<any> = new ReplaySubject<any>(1);
  @ViewChild('singleSelect') singleSelect: MatSelect;
  protected _onDestroy = new Subject<void>();
  constructor(
    private studentService: StudentService,
    private commonLOV: CommonLOV,
    private snackbar: MatSnackBar,
    private commonService: CommonService,
    private loginService: LoginService,
    private commonFunction: SharedFunction,
    private enrollmentCodeService: EnrollmentCodesService
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
    this.filteredCountryOfBirth
    .pipe(take(1), takeUntil(this._onDestroy))
    .subscribe(() => {
      this.singleSelect.compareWith = (a, b) => a && b && a.name === b.name;
    });
  }

  ngOnInit(): void {
    this.initializeDropdownsInAddMode();
  }
  ngAfterViewInit(){
    this.countryValueChange();
    this.nationalityValueChange();
    this.countryOfBirthValueChange();
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
  countryOfBirthValueChange(){
    this.countryOfBirthCtrl.valueChanges
    .pipe(takeUntil(this._onDestroy))
    .subscribe((res) => {
      this.filterCountryOfBirth();
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
  filterCountryOfBirth(){
    if (!this.countryListArr) {
      return;
    }
    let search = this.countryOfBirthFilterCtrl.value;
    if (!search) {
      this.filteredCountryOfBirth.next(this.countryListArr.slice());
      return;
    }
    else {
      search = search.toLowerCase();
    }
    this.filteredCountryOfBirth.next(
      this.countryListArr.filter(country => country.name.toLowerCase().indexOf(search) > -1)
    );
  }

  initializeDropdownsInAddMode() {
    this.callLOVs();
    this.getAllCountry();
    this.GetAllLanguage();
    this.getAllStudentEnrollmentCode();
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
          this.countryOfBirthCtrl.setValue(data.tableCountry?.sort((a, b) => a.name < b.name ? -1 : 1));
          this.filteredCountryOfBirth.next(data.tableCountry?.slice());
          this.countryListArr = data.tableCountry?.sort((a, b) => a.name < b.name ? -1 : 1);

        }
      }
    })
  }

  GetAllLanguage() {
    this.languages._tenantName = sessionStorage.getItem("tenant");
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
    })
  }

  searchAllSchools(event: MatSlideToggle) {
    if (event.checked) {
      this.searchSchoolId = 0;
    }
    else {
      this.searchSchoolId = +sessionStorage.getItem('selectedSchoolId');
    }

  };
  
  submit(){
    this.checkSearchRecord=1;
    this.search();
  }

  search() {
    this.getAllStudent= new StudentListModel();
    this.params = [];
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
        if (this.studentMasterSearchModel[key] !== null && this.studentMasterSearchModel[key] !== '') {
          if (key === 'exitDate' || key === 'enrollmentDate' || key === 'dob') {
            this.params.push({ "columnName": key, "filterOption": 11, "filterValue": this.commonFunction.formatDateSaveWithoutTime(this.studentMasterSearchModel[key]) })
          }
          else {
            this.params.push({ "columnName": key, "filterOption": 11, "filterValue": this.studentMasterSearchModel[key] })
          }
        }
    }
    
    this.getAllStudent.filterParams = this.params;
    this.getAllStudent.sortingModel = null;
    this.getAllStudent.schoolId = this.searchSchoolId;
    this.studentService.searchStudentListForReenroll(this.getAllStudent,this.searchSchoolId).subscribe(data => {
     if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
        this.searchList.emit({data:data, allSchool:this.showAllSchools});
        this.snackbar.open('' + data._message, '', {
          duration: 10000
        });

      } else {
        this.searchList.emit({data:data, allSchool:this.showAllSchools});
        this.showHideAdvanceSearch.emit({ hide: false });
        this.checkSearchRecord=0;
      }
    });

  }

  getAllStudentEnrollmentCode(){
    this.enrollmentCodeService.getAllStudentEnrollmentCode(this.enrollmentCodeListView).subscribe(
      (res:EnrollmentCodeListView)=>{
        if(typeof(res)=='undefined'){
          this.snackbar.open('Enrollment code list failed. ' + sessionStorage.getItem("httpError"), '', {
            duration: 10000
          });
        }
        else{
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message); 
            this.enrollmentList = []
            if(!res.studentEnrollmentCodeList){
              this.snackbar.open(res._message, '', {
                duration: 10000
              });
            }
          } 
          else { 
            this.enrollmentList= res.studentEnrollmentCodeList.filter(x=> x.type==='Rolled Over' || x.type==='Drop (Transfer)' || x.type==='Drop');
            }
        }
      }
    )
  }

  resetData() {
    this.checkSearchRecord=2;
    this.currentForm.reset();
    this.showAllSchools= false;
    this.search();
  }

  hideAdvanceSearch() {
    this.showHideAdvanceSearch.emit({ hide: false });
  }

  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }
}
