
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { DefaultValuesService } from '../common/default-values.service';
import { RolloverReadinessViewModel, RolloverViewModel } from '../models/roll-over.model';


@Injectable({
    providedIn: 'root'
})
export class RollOverService {
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

    rollover(obj: RolloverViewModel) {
        obj = this.defaultValuesService.getAllMandatoryVariable(obj);
        obj.schoolRollover.schoolId = this.defaultValuesService.getSchoolID();
        obj.schoolRollover.tenantId = this.defaultValuesService.getTenantID();
        obj.schoolRollover.CreatedBy = this.defaultValuesService.getUserGuidId();
        let apiurl = this.apiUrl + obj._tenantName + '/Rollover/rollover';
        return this.http.post<RolloverViewModel>(apiurl, obj, this.httpOptions);
    }

    getPreflightSummary(obj: RolloverReadinessViewModel) {
        obj = this.defaultValuesService.getAllMandatoryVariable(obj);
        obj.schoolId = this.defaultValuesService.getSchoolID();
        obj.tenantId = this.defaultValuesService.getTenantID();
        obj.academicYear = this.defaultValuesService.getAcademicYear();
        let apiurl = this.apiUrl + obj._tenantName + '/Rollover/preflightSummary';
        return this.http.post<RolloverReadinessViewModel>(apiurl, obj, this.httpOptions);
    }
}