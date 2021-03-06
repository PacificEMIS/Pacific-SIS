import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { DefaultValuesService } from '../common/default-values.service';
import { CourseStandardForCourseViewModel, GetAllCourseListModel } from '../models/course-manager.model';
import { ResponseStudentReportCardGradesModel, StudentReportCardGradesModel } from '../models/report-card.model';
import { GetAllStaffModel } from '../models/staff.model';
import { AddUpdateStudentFinalGradeModel, GetAllStudentListForFinalGradeModel, GetGradebookGradeinFinalGradeModel } from '../models/student-final-grade.model';
import { AddUpdateStudentAttendanceModel, GetAllStudentAttendanceListModel, SearchCourseSectionForStudentAttendance, StaffDetailsModel } from '../models/take-attendance-list.model';

@Injectable({
    providedIn: 'root'
})
export class FinalGradeService {
    apiUrl: string = environment.apiURL;
    userName = this.defaultValuesService.getUserName();
    staffDetails: StaffDetailsModel = new StaffDetailsModel();
    httpOptions: { headers: any; };

    constructor(
        private http: HttpClient,
        private defaultValuesService: DefaultValuesService
    ) { this.httpOptions = {
        headers: new HttpHeaders({
          'Cache-Control': 'no-cache',
          'Pragma': 'no-cache',
        })
      }}

    setStaffDetails(staffDetails) {
        this.staffDetails = staffDetails;
    }

    getStaffDetails() {
        return this.staffDetails;
    }

    addUpdateStudentFinalGrade(obj: AddUpdateStudentFinalGradeModel) {
        obj = this.defaultValuesService.getAllMandatoryVariable(obj);
        obj.createdOrUpdatedBy= this.defaultValuesService.getUserGuidId();
        let apiurl = this.apiUrl + obj._tenantName + "/InputFinalGrade/addUpdateStudentFinalGrade";
        return this.http.post<AddUpdateStudentFinalGradeModel>(apiurl, obj,this.httpOptions)
    }

    getAllStudentFinalGradeList(obj: AddUpdateStudentFinalGradeModel) {
        obj = this.defaultValuesService.getAllMandatoryVariable(obj);
        obj.academicYear = this.defaultValuesService.getAcademicYear();
        let apiurl = this.apiUrl + obj._tenantName + "/InputFinalGrade/getAllStudentFinalGradeList";
        return this.http.post<AddUpdateStudentFinalGradeModel>(apiurl, obj,this.httpOptions)         
    }

    getAllStudentListForFinalGrade(obj: GetAllStudentListForFinalGradeModel) {
        obj = this.defaultValuesService.getAllMandatoryVariable(obj);
        obj.academicYear = this.defaultValuesService.getAcademicYear();
        let apiurl = this.apiUrl + obj._tenantName + "/InputFinalGrade/getAllStudentListForFinalGrade";
        return this.http.post<GetAllStudentListForFinalGradeModel>(apiurl, obj, this.httpOptions)
    }

    getGradebookGradeinFinalGrade(obj: GetGradebookGradeinFinalGradeModel) {
        obj = this.defaultValuesService.getAllMandatoryVariable(obj);
        obj.academicYear = this.defaultValuesService.getAcademicYear();
        let apiurl = this.apiUrl + obj._tenantName + "/InputFinalGrade/getGradebookGradeinFinalGrade";
        return this.http.post<GetGradebookGradeinFinalGradeModel>(apiurl, obj, this.httpOptions)
    }

    getStudentReportCardGrades(obj: StudentReportCardGradesModel) {
        obj = this.defaultValuesService.getAllMandatoryVariable(obj);
        obj.academicYear = this.defaultValuesService.getAcademicYear();
        let apiurl = this.apiUrl + obj._tenantName + "/InputFinalGrade/getStudentReportCardGrades";
        return this.http.post<ResponseStudentReportCardGradesModel>(apiurl, obj, this.httpOptions)
    }

    updateStudentReportCardGrades(obj: ResponseStudentReportCardGradesModel) {
        obj = this.defaultValuesService.getAllMandatoryVariable(obj);
        obj.updatedBy = this.defaultValuesService.getUserGuidId();
        let apiurl = this.apiUrl + obj._tenantName + "/InputFinalGrade/updateStudentReportCardGrades";
        return this.http.put<ResponseStudentReportCardGradesModel>(apiurl, obj, this.httpOptions)
    }
}