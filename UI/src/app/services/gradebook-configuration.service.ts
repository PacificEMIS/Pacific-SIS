import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { DefaultValuesService } from '../common/default-values.service';
import { FinalGradingMarkingPeriodList, GradebookConfigurationAddViewModel } from '../models/gradebook-configuration.model';

@Injectable({
    providedIn: 'root'
})
export class GradeBookConfigurationService {

    apiUrl: string = environment.apiURL;
    httpOptions: { headers: any; };
    constructor(private http: HttpClient, private defaultValuesService: DefaultValuesService) { 
        this.httpOptions = {
        headers: new HttpHeaders({
          'Cache-Control': 'no-cache',
          'Pragma': 'no-cache',
        })
      }}

    addUpdateGradebookConfiguration(obj: GradebookConfigurationAddViewModel) {
        obj = this.defaultValuesService.getAllMandatoryVariable(obj);
        obj.gradebookConfiguration.schoolId = this.defaultValuesService.getSchoolID();
        obj.gradebookConfiguration.tenantId = this.defaultValuesService.getTenantID();
        obj.gradebookConfiguration.createdBy= this.defaultValuesService.getEmailId();
        obj.gradebookConfiguration.updatedBy= this.defaultValuesService.getEmailId();
        let apiurl = this.apiUrl + obj._tenantName + "/StaffPortalGradebook/addUpdateGradebookConfiguration";
        return this.http.post<GradebookConfigurationAddViewModel>(apiurl, obj,this.httpOptions)
    }

    viewGradebookConfiguration(obj: GradebookConfigurationAddViewModel) {
        obj.gradebookConfiguration.academicYear = this.defaultValuesService.getAcademicYear();
        obj.gradebookConfiguration.schoolId = this.defaultValuesService.getSchoolID();
        obj.gradebookConfiguration.tenantId = this.defaultValuesService.getTenantID();
        obj = this.defaultValuesService.getAllMandatoryVariable(obj);
        let apiurl = this.apiUrl + obj._tenantName + "/StaffPortalGradebook/viewGradebookConfiguration";
        return this.http.post<GradebookConfigurationAddViewModel>(apiurl, obj,this.httpOptions)
    }

    populateFinalGrading(obj: FinalGradingMarkingPeriodList) {
        obj = this.defaultValuesService.getAllMandatoryVariable(obj);
        obj.academicYear = this.defaultValuesService.getAcademicYear();
        let apiurl = this.apiUrl + obj._tenantName + "/StaffPortalGradebook/populateFinalGrading";
        return this.http.post<FinalGradingMarkingPeriodList>(apiurl, obj,this.httpOptions)
    }


}