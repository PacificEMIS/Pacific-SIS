import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { DefaultValuesService } from '../common/default-values.service';
import { SchoolPreferenceAddViewModel } from '../models/school-preference.model';

@Injectable({
    providedIn: 'root'
})
export class SchoolPreferenceService {
    apiUrl: string = environment.apiURL;
    httpOptions: { headers: any; };
    constructor(private http: HttpClient, private defaultValuesService: DefaultValuesService,) {
        this.httpOptions = {
            headers: new HttpHeaders({
              'Cache-Control': 'no-cache',
              'Pragma': 'no-cache',
            })
          }
     }

    viewPreference(Obj: SchoolPreferenceAddViewModel) {
        Obj = this.defaultValuesService.getAllMandatoryVariable(Obj);
        Obj.schoolPreference.schoolId = this.defaultValuesService.getSchoolID();
        Obj.schoolPreference.tenantId = this.defaultValuesService.getTenantID();
        let apiurl = this.apiUrl + Obj._tenantName + '/Common/viewSchoolPreference';
        return this.http.post<SchoolPreferenceAddViewModel>(apiurl, Obj,this.httpOptions);
    }

    addUpdateSchoolPreference(Obj: SchoolPreferenceAddViewModel) {
        Obj = this.defaultValuesService.getAllMandatoryVariable(Obj);
        Obj.schoolPreference.schoolId = this.defaultValuesService.getSchoolID();
        Obj.schoolPreference.tenantId = this.defaultValuesService.getTenantID();
        Obj.schoolPreference.createdBy= this.defaultValuesService.getEmailId();
        Obj.schoolPreference.updatedBy= this.defaultValuesService.getEmailId();
        let apiurl = this.apiUrl + Obj._tenantName + '/Common/addUpdateSchoolPreference';
        return this.http.post<SchoolPreferenceAddViewModel>(apiurl, Obj,this.httpOptions);
    }
}