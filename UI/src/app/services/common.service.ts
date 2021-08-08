import { LovList, LovAddView } from '../models/lov.model';
import { CountryModel, CountryAddModel } from '../models/country.model';
import { StateModel } from '../models/state.model';
import { CityModel } from '../models/city.model';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { LanguageModel, LanguageAddModel } from '../models/language.model';
import { ReleaseNumberAddViewModel } from '../models/release-number-model';
import { SearchFilterAddViewModel, SearchFilterListViewModel } from '../models/search-filter.model';
import { AgeRangeList, EducationalStage, ResetPasswordModel, BulkDataImportExcelHeader, ChangePasswordViewModel } from '../models/common.model';
import { DefaultValuesService } from '../common/default-values.service';
import { CryptoService } from './Crypto.service';
import { BehaviorSubject, fromEvent, merge, Observable, Observer } from 'rxjs';
import { LoginService } from './login.service';
import { Router } from '@angular/router';
import { UserService } from './user.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { map } from 'rxjs/operators';
import { TranslateService } from '@ngx-translate/core';
import { MatDialog } from '@angular/material/dialog';
@Injectable({
  providedIn: 'root'
})
export class CommonService {
  apiUrl: string = environment.apiURL;
  private searchResult;
  private moduleName: string;

  private changeLanguage = new BehaviorSubject(false);
  changedLanguage = this.changeLanguage.asObservable();

  private triggerUserActivity = new BehaviorSubject(false);
  triggeredUserActivity = this.triggerUserActivity.asObservable();
  httpOptions: { headers: any; };


  constructor(
    private http: HttpClient,
    private defaultValuesService: DefaultValuesService,
    private cryptoService: CryptoService,
    private loginService: LoginService,
    private router: Router,
    private userService: UserService,
    private snackbar: MatSnackBar,
    private defaultValueService: DefaultValuesService,
    public translate: TranslateService,
    private dialog: MatDialog, 
  ) { this.httpOptions = {
    headers: new HttpHeaders({
      'Cache-Control': 'no-cache',
      'Pragma': 'no-cache',
    })
  }}
  GetAllCountry(obj: CountryModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName + "/Common/getAllCountries";
    return this.http.post<CountryModel>(apiurl, obj,this.httpOptions)
  }

  AddCountry(obj: CountryAddModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.country.createdBy= this.defaultValuesService.getEmailId();
    let apiurl = this.apiUrl + obj._tenantName + "/Common/addCountry";
    return this.http.post<CountryAddModel>(apiurl, obj,this.httpOptions)
  }

  UpdateCountry(obj: CountryAddModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.country.updatedBy= this.defaultValuesService.getEmailId();
    let apiurl = this.apiUrl + obj._tenantName + "/Common/updateCountry";
    return this.http.put<CountryAddModel>(apiurl, obj,this.httpOptions)
  }

  DeleteCountry(obj: CountryAddModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName + "/Common/deleteCountry";
    return this.http.post<CountryAddModel>(apiurl, obj,this.httpOptions)
  }

  GetAllState(obj: StateModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName + "/Common/getAllStatesByCountry";
    return this.http.post<StateModel>(apiurl, obj,this.httpOptions)
  }
  GetAllCity(obj: CityModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName + "/Common/getAllCitiesByState";
    return this.http.post<CityModel>(apiurl, obj,this.httpOptions)
  }
  GetAllLanguage(obj: LanguageModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName + "/Common/getAllLanguage";
    return this.http.post<LanguageModel>(apiurl, obj,this.httpOptions)
  }

  AddLanguage(obj: LanguageAddModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.language.createdBy= this.defaultValuesService.getEmailId();
    let apiurl = this.apiUrl + obj._tenantName + "/Common/addLanguage";
    return this.http.post<LanguageAddModel>(apiurl, obj,this.httpOptions)
  }

  UpdateLanguage(obj: LanguageAddModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.language.updatedBy= this.defaultValuesService.getEmailId();
    let apiurl = this.apiUrl + obj._tenantName + "/Common/updateLanguage";
    return this.http.post<LanguageAddModel>(apiurl, obj,this.httpOptions)
  }

  DeleteLanguage(obj: LanguageAddModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName + "/Common/deleteLanguage";
    return this.http.post<LanguageAddModel>(apiurl, obj,this.httpOptions)
  }
  getAllDropdownValues(obj: LovList) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName + "/Common/getAllDropdownValues";
    return this.http.post<LovList>(apiurl, obj,this.httpOptions);
  }
  addDropdownValue(obj: LovAddView) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.dropdownValue.schoolId = this.defaultValuesService.getSchoolID();
    obj.dropdownValue.tenantId = this.defaultValuesService.getTenantID();
    obj.dropdownValue.createdBy= this.defaultValuesService.getEmailId();
    let apiurl = this.apiUrl + obj._tenantName + "/Common/addDropdownValue";
    return this.http.post<LovAddView>(apiurl, obj,this.httpOptions);
  }
  updateDropdownValue(obj: LovAddView) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.dropdownValue.schoolId = this.defaultValuesService.getSchoolID();
    obj.dropdownValue.tenantId = this.defaultValuesService.getTenantID();
    obj.dropdownValue.updatedBy= this.defaultValuesService.getEmailId();
    let apiurl = this.apiUrl + obj._tenantName + "/Common/updateDropdownValue";
    return this.http.put<LovAddView>(apiurl, obj,this.httpOptions);
  }
  deleteDropdownValue(obj: LovAddView) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName + "/Common/deleteDropdownValue";
    return this.http.post<LovAddView>(apiurl, obj,this.httpOptions);
  }

  getReleaseNumber(obj: ReleaseNumberAddViewModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName + "/Common/getReleaseNumber";
    return this.http.post<ReleaseNumberAddViewModel>(apiurl, obj,this.httpOptions);
  }

  addSearchFilter(obj: SearchFilterAddViewModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.searchFilter.createdBy= this.defaultValuesService.getEmailId();
    let apiurl = this.apiUrl + obj._tenantName + "/Common/addSearchFilter";
    return this.http.post<SearchFilterAddViewModel>(apiurl, obj,this.httpOptions);
  }

  updateSearchFilter(obj: SearchFilterAddViewModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.searchFilter.updatedBy= this.defaultValuesService.getEmailId();
    let apiurl = this.apiUrl + obj._tenantName + "/Common/updateSearchFilter";
    return this.http.put<SearchFilterAddViewModel>(apiurl, obj,this.httpOptions);
  }

  deleteSearchFilter(obj: SearchFilterAddViewModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName + "/Common/deleteSearchFilter";
    return this.http.post<SearchFilterAddViewModel>(apiurl, obj,this.httpOptions);
  }

  getAllSearchFilter(obj: SearchFilterListViewModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName + "/Common/getAllSearchFilter";
    return this.http.post<SearchFilterListViewModel>(apiurl, obj,this.httpOptions);
  }

  getAllGradeAgeRange(obj: AgeRangeList) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName + "/Common/getAllGradeAgeRange";
    return this.http.post<AgeRangeList>(apiurl, obj,this.httpOptions)
  }

  getAllGradeEducationalStage(obj: EducationalStage) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName + "/Common/getAllGradeEducationalStage";
    return this.http.post<EducationalStage>(apiurl, obj,this.httpOptions)
  }

  resetPassword(obj: ResetPasswordModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.userMaster.tenantId = this.defaultValuesService.getTenantID();
    obj.userMaster.schoolId = this.defaultValuesService.getSchoolID();
    obj.userMaster.updatedBy = this.defaultValuesService.getEmailId();
    obj.userMaster.passwordHash = this.cryptoService.encrypt(obj.userMaster.passwordHash);
    let apiurl = this.apiUrl + obj._tenantName + "/Common/resetPassword";
    return this.http.post<ResetPasswordModel>(apiurl, obj,this.httpOptions)
  }
  getAllFieldList(obj: BulkDataImportExcelHeader) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName + "/Common/getAllFieldList";
    return this.http.post<BulkDataImportExcelHeader>(apiurl, obj,this.httpOptions)
  }

  changePassword(obj: ChangePasswordViewModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.userId= +sessionStorage.getItem('userId');
    obj.emailAddress= this.defaultValuesService.getEmailId();
    obj.newPasswordHash = this.cryptoService.encrypt(obj.newPasswordHash);
    obj.currentPasswordHash = this.cryptoService.encrypt(obj.currentPasswordHash);
    let apiurl = this.apiUrl + obj._tenantName + "/Common/changePassword";
    return this.http.post<ChangePasswordViewModel>(apiurl, obj,this.httpOptions)
  }

  setSearchResult(result) {
    this.searchResult = result;
  }
  getSearchResult() {
    return this.searchResult;
  }

  setModuleName(moduleName) {
    this.moduleName = moduleName;
  }
  getModuleName() {
    return this.moduleName;
  }

  triggerLanguageChange(data) {
    this.changeLanguage.next(data);
  }

  setUserActivity(data) {
    this.triggerUserActivity.next(data);
  }

  checkAndRoute() {
    if (this.loginService.isAuthenticated()) {
      if (this.defaultValuesService.getuserMembershipName() === 'Teacher') {
        this.router.navigate(['/school', 'teacher', 'dashboards']);
      } else {
        this.router.navigate(['/school', 'dashboards']);
      }
    } else {
      this.router.navigate(['/']);
    }
  }

  checkAndRouteTo404() {
    if (this.loginService.isAuthenticated()) {
      this.router.navigate(['/school', 'error', '404']);
    } else {
      this.router.navigate(['/error', '404']);
    }
  }

  logoutUser() {
    if (this.loginService.isAuthenticated()) {
    this.userService.userLogout().subscribe((res) => {
      if (res) {
      if(res._failure){
        this.checkTokenValidOrNot(res._message);
          this.snackbar.open(res._message, '', {
            duration: 10000
          });
        } else {
          this.clearStorage();
          this.router.navigateByUrl('/');
        }
      }
    });
  } else {
    this.clearStorage();
    this.router.navigateByUrl('/');
  }
  }

  checkTokenValidOrNot(message: string) {
    if((message).toLowerCase() === 'token not valid') {
      this.clearStorage();
      this.router.navigateByUrl('/');

    }
  }

  clearStorage(){
    this.dialog.closeAll();

      let schoolId = sessionStorage.getItem('selectedSchoolId');
      sessionStorage.clear();
      localStorage.removeItem(this.defaultValueService.permissionListKeyName);
      if(schoolId){
      sessionStorage.setItem('selectedSchoolId',schoolId);
      }
      this.setUserActivity(true);

      sessionStorage.setItem("tenant",  this.defaultValuesService.getDefaultTenant());
      this.translate.addLangs(['en', 'fr']);
      this.translate.setDefaultLang('en');
    }

    isOnline() {
      return merge<boolean>(
        fromEvent(window, 'offline').pipe(map(() => false)),
        fromEvent(window, 'online').pipe(map(() => true)),
        new Observable((sub: Observer<boolean>) => {
          sub.next(navigator.onLine);
          sub.complete();
        }));
        // return new Observable((sub: Observer<boolean>) => {
        //   sub.next(navigator.onLine);
        //   sub.complete();
        // });
    }
}

