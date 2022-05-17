import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { DefaultValuesService } from '../common/default-values.service';
import { GetStudentListByHomeRoomStaffModel, StudentEffortGradeListModel } from '../models/student-effort-grade.model';

@Injectable({
    providedIn: 'root'
})
export class EffotrGradeService {
    apiUrl: string = environment.apiURL;
    userName = this.defaultValuesService.getUserName();
    httpOptions: { headers: any; };

    
    constructor(
        private http: HttpClient,
        private defaultValuesService: DefaultValuesService
    ) {this.httpOptions = {
        headers: new HttpHeaders({
          'Cache-Control': 'no-cache',
          'Pragma': 'no-cache',
        })
      } }

    addUpdateStudentEffortGrade(obj: GetStudentListByHomeRoomStaffModel) {
        obj = this.defaultValuesService.getAllMandatoryVariable(obj);
        obj.createdOrUpdatedBy = this.defaultValuesService.getUserGuidId();
        obj.markingPeriodStartDate = this.defaultValuesService.getMarkingPeriodStartDate();
        obj.markingPeriodEndDate = this.defaultValuesService.getMarkingPeriodEndDate();
        let apiurl = this.apiUrl + obj._tenantName + "/StudentEffortGrade/addUpdateStudentEffortGrade";
        return this.http.post<GetStudentListByHomeRoomStaffModel>(apiurl, obj,this.httpOptions)
    }

    getAllStudentEffortGradeList(obj: StudentEffortGradeListModel) {
        obj = this.defaultValuesService.getAllMandatoryVariable(obj);
        obj.academicYear = this.defaultValuesService.getAcademicYear();
        let apiurl = this.apiUrl + obj._tenantName + "/StudentEffortGrade/getAllStudentEffortGradeList";
        return this.http.post<StudentEffortGradeListModel>(apiurl, obj,this.httpOptions)
    }

    getStudentListByHomeRoomStaff(obj: GetStudentListByHomeRoomStaffModel) {
        obj = this.defaultValuesService.getAllMandatoryVariable(obj);
        obj.academicYear = this.defaultValuesService.getAcademicYear();
        obj.markingPeriodStartDate = this.defaultValuesService.getMarkingPeriodStartDate();
        obj.markingPeriodEndDate = this.defaultValuesService.getMarkingPeriodEndDate();
        obj.profilePhoto = true;
        let apiurl = this.apiUrl + obj._tenantName + "/StudentEffortGrade/GetStudentListByHomeRoomStaff";
        return this.http.post<GetStudentListByHomeRoomStaffModel>(apiurl, obj, this.httpOptions)
    }
}
