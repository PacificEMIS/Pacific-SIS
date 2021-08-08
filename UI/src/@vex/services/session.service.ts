import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { UserViewModel } from '../../app/models/user.model';
import { DefaultValuesService } from '../../app/common/default-values.service';
@Injectable({
  providedIn: 'root'
})
export class SessionService {
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

  RefreshToken(obj: UserViewModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.email = this.defaultValuesService.getEmailId();
    let apiurl = this.apiUrl + obj._tenantName + "/User/RefreshToken";
    return this.http.post<UserViewModel>(apiurl, obj,this.httpOptions)
  }

}
