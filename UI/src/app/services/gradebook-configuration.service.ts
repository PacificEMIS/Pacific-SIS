import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { DefaultValuesService } from '../common/default-values.service';
import { FinalGradingMarkingPeriodList, GradebookConfigurationAddViewModel } from '../models/gradebook-configuration.model';
import { AddGradebookGradeByAssignmentTypeModel, AddGradebookGradeByStudentModel, AddGradebookGradeModel, ViewGradebookGradeByAssignmentTypeModel, ViewGradebookGradeByStudentModel, ViewGradebookGradeModel } from '../models/gradebook-grades.model';

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
        obj.gradebookConfiguration.createdBy= this.defaultValuesService.getUserGuidId();
        obj.gradebookConfiguration.updatedBy= this.defaultValuesService.getUserGuidId();
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

    viewGradebookGrade(obj: ViewGradebookGradeModel) {
        obj = this.defaultValuesService.getAllMandatoryVariable(obj);
        obj.academicYear = this.defaultValuesService.getAcademicYear();
        obj.markingPeriodStartDate =  this.defaultValuesService.getMarkingPeriodStartDate();
        obj.markingPeriodEndDate = this.defaultValuesService.getMarkingPeriodEndDate();
        
        let apiurl = this.apiUrl + obj._tenantName + "/StaffPortalGradebook/getGradebookGrade";
        return this.http.post<ViewGradebookGradeModel>(apiurl, obj,this.httpOptions)
    }

    addGradebookGrade(obj: AddGradebookGradeModel) {
        obj = this.defaultValuesService.getAllMandatoryVariable(obj);
        // obj.academicYear = this.defaultValuesService.getAcademicYear();
        obj.createdBy= this.defaultValuesService.getUserGuidId();
        let apiurl = this.apiUrl + obj._tenantName + "/StaffPortalGradebook/addGradebookGrade";
        return this.http.post<AddGradebookGradeModel>(apiurl, obj,this.httpOptions)
    }

    viewGradebookGradeByStudent(obj: ViewGradebookGradeByStudentModel) {
        obj = this.defaultValuesService.getAllMandatoryVariable(obj);
        obj.academicYear = this.defaultValuesService.getAcademicYear();
        obj.markingPeriodStartDate =  this.defaultValuesService.getMarkingPeriodStartDate();
        obj.markingPeriodEndDate =  this.defaultValuesService.getMarkingPeriodEndDate();

        let apiurl = this.apiUrl + obj._tenantName + "/StaffPortalGradebook/gradebookGradeByStudent";
        return this.http.post<ViewGradebookGradeByStudentModel>(apiurl, obj,this.httpOptions)
    }

    addGradebookGradeByStudent(obj: AddGradebookGradeByStudentModel) {
        obj = this.defaultValuesService.getAllMandatoryVariable(obj);
        // obj.academicYear = this.defaultValuesService.getAcademicYear();
        obj.createdBy= this.defaultValuesService.getUserGuidId();
        let apiurl = this.apiUrl + obj._tenantName + "/StaffPortalGradebook/addGradebookGradeByStudent";
        return this.http.post<AddGradebookGradeByStudentModel>(apiurl, obj,this.httpOptions)
    }

    viewGradebookGradeByAssignmentType(obj: ViewGradebookGradeByAssignmentTypeModel) {
        obj = this.defaultValuesService.getAllMandatoryVariable(obj);
        obj.academicYear = this.defaultValuesService.getAcademicYear();
        let apiurl = this.apiUrl + obj._tenantName + "/StaffPortalGradebook/gradebookGradeByAssignmentType";
        return this.http.post<ViewGradebookGradeByAssignmentTypeModel>(apiurl, obj,this.httpOptions)
    }

    addGradebookGradeByAssignmentType(obj: AddGradebookGradeByAssignmentTypeModel) {
        obj = this.defaultValuesService.getAllMandatoryVariable(obj);
        // obj.academicYear = this.defaultValuesService.getAcademicYear();
        obj.createdBy= this.defaultValuesService.getUserGuidId();
        let apiurl = this.apiUrl + obj._tenantName + "/StaffPortalGradebook/addgradebookGradeByAssignmentType";
        return this.http.post<AddGradebookGradeByAssignmentTypeModel>(apiurl, obj,this.httpOptions)
    }
}