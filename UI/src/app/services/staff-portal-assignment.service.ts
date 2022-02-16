import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { DefaultValuesService } from '../common/default-values.service';
import { AddAssignmentModel, AddAssignmentTypeModel, GetAllAssignmentsModel } from '../models/staff-portal-assignment.model';

@Injectable({
  providedIn: 'root'
})
export class StaffPortalAssignmentService {
  apiUrl: string = environment.apiURL;
    httpOptions: { headers: any; };

  constructor(
      private http: HttpClient,
      private defaultValuesService: DefaultValuesService
  ) { 
    this.httpOptions = {
        headers: new HttpHeaders({
          'Cache-Control': 'no-cache',
          'Pragma': 'no-cache',
        })
      }
  }
  
  addAssignmentType(obj: AddAssignmentTypeModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.assignmentType.createdBy= this.defaultValuesService.getUserGuidId();
    obj.assignmentType.schoolId= this.defaultValuesService.getSchoolID();
    obj.assignmentType.tenantId= this.defaultValuesService.getTenantID();
    obj.markingPeriodStartDate = this.defaultValuesService.getMarkingPeriodStartDate();
    obj.markingPeriodEndDate = this.defaultValuesService.getMarkingPeriodEndDate();
    const apiurl = this.apiUrl + obj._tenantName + '/StaffPortalAssignment/addAssignmentType';
    return this.http.post<AddAssignmentTypeModel>(apiurl, obj,this.httpOptions);
}

getAllAssignmentType(obj: GetAllAssignmentsModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.academicYear = this.defaultValuesService.getAcademicYear();
    obj.markingPeriodStartDate = this.defaultValuesService.getMarkingPeriodStartDate();
    obj.markingPeriodEndDate = this.defaultValuesService.getMarkingPeriodEndDate();
    const apiurl = this.apiUrl + obj._tenantName + '/StaffPortalAssignment/getAllAssignmentType';
    return this.http.post<GetAllAssignmentsModel>(apiurl, obj,this.httpOptions);
}

updateAssignmentType(obj: AddAssignmentTypeModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.assignmentType.updatedBy= this.defaultValuesService.getUserGuidId();
    obj.assignmentType.schoolId= this.defaultValuesService.getSchoolID();
    obj.assignmentType.tenantId= this.defaultValuesService.getTenantID();
    const apiurl = this.apiUrl + obj._tenantName + '/StaffPortalAssignment/updateAssignmentType';
    return this.http.put<AddAssignmentTypeModel>(apiurl, obj,this.httpOptions);
}

deleteAssignmentType(obj: AddAssignmentTypeModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.assignmentType.schoolId= this.defaultValuesService.getSchoolID();
    obj.assignmentType.tenantId= this.defaultValuesService.getTenantID();
    const apiurl = this.apiUrl + obj._tenantName + '/StaffPortalAssignment/deleteAssignmentType';
    return this.http.post<AddAssignmentTypeModel>(apiurl, obj,this.httpOptions);
}

addAssignment(obj: AddAssignmentModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.assignment.createdBy= this.defaultValuesService.getUserGuidId();
    obj.assignment.schoolId= this.defaultValuesService.getSchoolID();
    obj.assignment.tenantId= this.defaultValuesService.getTenantID();
    const apiurl = this.apiUrl + obj._tenantName + '/StaffPortalAssignment/addAssignment';
    return this.http.post<AddAssignmentModel>(apiurl, obj,this.httpOptions);
}

updateAssignment(obj: AddAssignmentModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.assignment.updatedBy= this.defaultValuesService.getUserGuidId();
    obj.assignment.schoolId= this.defaultValuesService.getSchoolID();
    obj.assignment.tenantId= this.defaultValuesService.getTenantID();
    const apiurl = this.apiUrl + obj._tenantName + '/StaffPortalAssignment/updateAssignment';
    return this.http.put<AddAssignmentModel>(apiurl, obj,this.httpOptions);
}

deleteAssignment(obj: AddAssignmentModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.assignment.schoolId= this.defaultValuesService.getSchoolID();
    obj.assignment.tenantId= this.defaultValuesService.getTenantID();
    const apiurl = this.apiUrl + obj._tenantName + '/StaffPortalAssignment/deleteAssignment';
    return this.http.post<AddAssignmentModel>(apiurl, obj,this.httpOptions);
}

copyAssignmentForCourseSection(obj: AddAssignmentModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.assignment.updatedBy= this.defaultValuesService.getUserGuidId();
    obj.assignment.schoolId= this.defaultValuesService.getSchoolID();
    obj.assignment.tenantId= this.defaultValuesService.getTenantID();
    const apiurl = this.apiUrl + obj._tenantName + '/StaffPortalAssignment/copyAssignmentForCourseSection';
    return this.http.post<AddAssignmentModel>(apiurl, obj,this.httpOptions);
}

}

