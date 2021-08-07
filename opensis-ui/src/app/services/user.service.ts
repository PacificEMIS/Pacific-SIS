import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { DefaultValuesService } from '../common/default-values.service';
import { UserLogoutModel } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  apiUrl: string = environment.apiURL;
  httpOptions: { headers: any; };
  constructor(private http: HttpClient, private defaultValuesService: DefaultValuesService) { 
    this.httpOptions = {
      headers: new HttpHeaders({
        'Cache-Control': 'no-cache',
        'Pragma': 'no-cache',
      })
    }
  }

  userLogout() {
    let obj: UserLogoutModel = new UserLogoutModel();
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.email = this.defaultValuesService.getEmailId();
    let apiurl = this.apiUrl + obj._tenantName + "/User/logOutForUser";
    return this.http.post<UserLogoutModel>(apiurl, obj,this.httpOptions)
  }

}