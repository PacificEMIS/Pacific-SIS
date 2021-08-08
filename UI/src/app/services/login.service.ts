import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { CheckUserEmailAddressViewModel, UserViewModel } from '../models/user.model';
import { JwtHelperService } from '@auth0/angular-jwt';
import { CryptoService } from '../services/Crypto.service';
import { LanguageModel } from '../models/language.model';
import { DefaultValuesService } from '../common/default-values.service';
@Injectable({
  providedIn: 'root'
})
export class LoginService {
  apiUrl: string = environment.apiURL;
  httpOptions: { headers: any; };
  constructor(private http: HttpClient,
    public jwtHelper: JwtHelperService,
    private cryptoService: CryptoService,
    private defaultValuesService: DefaultValuesService) {
      this.httpOptions = {
        headers: new HttpHeaders({
          'Cache-Control': 'no-cache',
          'Pragma': 'no-cache',
        })
      }
     }

  getAllLanguage(obj: LanguageModel) {
    obj= this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName + "/Common/getAllLanguage";
    return this.http.post<LanguageModel>(apiurl, obj,this.httpOptions)
  }

  getAllLanguageForLogin(obj: LanguageModel) {
    obj= this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName + "/Common/getAllLanguageForLogin";
    return this.http.post<LanguageModel>(apiurl, obj,this.httpOptions)
  }

  ValidateLogin(obj: UserViewModel) {
    obj.schoolId = this.defaultValuesService.getSchoolID() === 0 ? null : this.defaultValuesService.getSchoolID();
    obj._tenantName= this.defaultValuesService.getTenent();
    obj._userName = this.defaultValuesService.getUserName();
    obj._token= this.defaultValuesService.getToken();
    obj.password = this.cryptoService.encrypt(obj.password);
    let apiurl = this.apiUrl + obj._tenantName + "/User/ValidateLogin";
    return this.http.post<UserViewModel>(apiurl, obj,this.httpOptions)
  }

  public isAuthenticated(): boolean {
    const token = this.defaultValuesService.getToken();
    if(this.defaultValuesService.getToken()) {
      return !this.jwtHelper.isTokenExpired(token);
    } else {
      return false;
    }
  }

  checkUserLoginEmail(obj : CheckUserEmailAddressViewModel){
    obj= this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName + "/User/checkUserLoginEmail";
    return this.http.post<CheckUserEmailAddressViewModel>(apiurl, obj,this.httpOptions)
  }

}
