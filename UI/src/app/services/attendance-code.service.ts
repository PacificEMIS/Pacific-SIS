import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { DefaultValuesService } from '../common/default-values.service';
import { AttendanceCodeCategoryModel, AttendanceCodeModel, AttendanceCodeDragDropModel, GetAllAttendanceCategoriesListModel, GetAllAttendanceCodeModel,AverageDailyAttendanceReportModel , GetStudentAttendanceReport } from '../models/attendance-code.model';
@Injectable({
  providedIn: 'root'
})
export class AttendanceCodeService {

  apiUrl: string = environment.apiURL;
  httpOptions;

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

  addAttendanceCodeCategories(AttendanceCategory: AttendanceCodeCategoryModel) {
    AttendanceCategory = this.defaultValuesService.getAllMandatoryVariable(AttendanceCategory);
    AttendanceCategory.attendanceCodeCategories.schoolId = this.defaultValuesService.getSchoolID();
    AttendanceCategory.attendanceCodeCategories.tenantId = this.defaultValuesService.getTenantID();
    AttendanceCategory.attendanceCodeCategories.createdBy = this.defaultValuesService.getUserGuidId();
    let apiurl = this.apiUrl + AttendanceCategory._tenantName + "/AttendanceCode/addAttendanceCodeCategories";
    return this.http.post<AttendanceCodeCategoryModel>(apiurl, AttendanceCategory, this.httpOptions);
  }

  getAllAttendanceCodeCategories(AttendanceCategoryList: GetAllAttendanceCategoriesListModel) {
    AttendanceCategoryList = this.defaultValuesService.getAllMandatoryVariable(AttendanceCategoryList);
    AttendanceCategoryList.academicYear = this.defaultValuesService.getAcademicYear();
    let apiurl = this.apiUrl + AttendanceCategoryList._tenantName + "/AttendanceCode/getAllAttendanceCodeCategories";
    return this.http.post<GetAllAttendanceCategoriesListModel>(apiurl, AttendanceCategoryList, this.httpOptions);
  }

  updateAttendanceCodeCategories(AttendanceCategory: AttendanceCodeCategoryModel) {
    AttendanceCategory = this.defaultValuesService.getAllMandatoryVariable(AttendanceCategory);
    AttendanceCategory.attendanceCodeCategories.schoolId = this.defaultValuesService.getSchoolID();
    AttendanceCategory.attendanceCodeCategories.tenantId = this.defaultValuesService.getTenantID();
    AttendanceCategory.attendanceCodeCategories.updatedBy = this.defaultValuesService.getUserGuidId();
    let apiurl = this.apiUrl + AttendanceCategory._tenantName + "/AttendanceCode/updateAttendanceCodeCategories";
    return this.http.put<AttendanceCodeCategoryModel>(apiurl, AttendanceCategory, this.httpOptions);
  }

  deleteAttendanceCodeCategories(AttendanceCategory: AttendanceCodeCategoryModel) {
    AttendanceCategory = this.defaultValuesService.getAllMandatoryVariable(AttendanceCategory);
    AttendanceCategory.attendanceCodeCategories.schoolId = this.defaultValuesService.getSchoolID();
    AttendanceCategory.attendanceCodeCategories.tenantId = this.defaultValuesService.getTenantID();
    let apiurl = this.apiUrl + AttendanceCategory._tenantName + "/AttendanceCode/deleteAttendanceCodeCategories";
    return this.http.post<AttendanceCodeCategoryModel>(apiurl, AttendanceCategory, this.httpOptions);
  }

  getAllAttendanceCode(AttendanceCode: GetAllAttendanceCodeModel) {
    AttendanceCode = this.defaultValuesService.getAllMandatoryVariable(AttendanceCode);
    AttendanceCode.academicYear=this.defaultValuesService.getAcademicYear();
    let apiurl = this.apiUrl + AttendanceCode._tenantName + "/AttendanceCode/getAllAttendanceCode";
    return this.http.post<GetAllAttendanceCodeModel>(apiurl, AttendanceCode, this.httpOptions);
  }

  addAttendanceCode(AttendanceCode: AttendanceCodeModel) {
    AttendanceCode = this.defaultValuesService.getAllMandatoryVariable(AttendanceCode);
    AttendanceCode.attendanceCode.schoolId = this.defaultValuesService.getSchoolID();
    AttendanceCode.attendanceCode.tenantId = this.defaultValuesService.getTenantID();
    AttendanceCode.attendanceCode.createdBy = this.defaultValuesService.getUserGuidId();
    let apiurl = this.apiUrl + AttendanceCode._tenantName + "/AttendanceCode/addAttendanceCode";
    return this.http.post<AttendanceCodeModel>(apiurl, AttendanceCode, this.httpOptions);
  }

  updateAttendanceCode(AttendanceCode: AttendanceCodeModel) {
    AttendanceCode = this.defaultValuesService.getAllMandatoryVariable(AttendanceCode);
    AttendanceCode.attendanceCode.schoolId = this.defaultValuesService.getSchoolID();
    AttendanceCode.attendanceCode.tenantId = this.defaultValuesService.getTenantID();
    AttendanceCode.attendanceCode.updatedBy = this.defaultValuesService.getUserGuidId();
    let apiurl = this.apiUrl + AttendanceCode._tenantName + "/AttendanceCode/updateAttendanceCode";
    return this.http.put<AttendanceCodeModel>(apiurl, AttendanceCode, this.httpOptions);
  }

  deleteAttendanceCode(AttendanceCode: AttendanceCodeModel) {
    AttendanceCode = this.defaultValuesService.getAllMandatoryVariable(AttendanceCode);
    AttendanceCode.attendanceCode.schoolId = this.defaultValuesService.getSchoolID();
    AttendanceCode.attendanceCode.tenantId = this.defaultValuesService.getTenantID();
    let apiurl = this.apiUrl + AttendanceCode._tenantName + "/AttendanceCode/deleteAttendanceCode";
    return this.http.post<AttendanceCodeModel>(apiurl, AttendanceCode, this.httpOptions);
  }

  updateAttendanceCodeSortOrder(AttendanceCode: AttendanceCodeDragDropModel) {
    AttendanceCode = this.defaultValuesService.getAllMandatoryVariable(AttendanceCode);
    AttendanceCode.updatedBy = this.defaultValuesService.getUserGuidId();
    let apiurl = this.apiUrl + AttendanceCode._tenantName + "/AttendanceCode/updateAttendanceCodeSortOrder";
    return this.http.post<AttendanceCodeDragDropModel>(apiurl, AttendanceCode, this.httpOptions);
  }

  getAverageDailyAttendanceReport(AverageDailyAttendanceReport: AverageDailyAttendanceReportModel) {
    AverageDailyAttendanceReport = this.defaultValuesService.getAllMandatoryVariable(AverageDailyAttendanceReport);
    AverageDailyAttendanceReport.updatedBy = this.defaultValuesService.getUserGuidId();
    AverageDailyAttendanceReport.schoolId=this.defaultValuesService.getSchoolID();
    AverageDailyAttendanceReport.tenantId=this.defaultValuesService.getTenantID();
    AverageDailyAttendanceReport.academicYear=this.defaultValuesService.getAcademicYear();
    let apiurl = this.apiUrl + AverageDailyAttendanceReport._tenantName + "/Report/getAverageDailyAttendanceReport";
    return this.http.post<AverageDailyAttendanceReportModel>(apiurl, AverageDailyAttendanceReport, this.httpOptions);
  }

  getStudentAttendanceReport(StudentAttendanceReport: GetStudentAttendanceReport) {
    StudentAttendanceReport = this.defaultValuesService.getAllMandatoryVariable(StudentAttendanceReport);
    let apiurl = this.apiUrl + StudentAttendanceReport._tenantName + "/Report/getStudentAttendanceReport";
    return this.http.post<GetStudentAttendanceReport>(apiurl, StudentAttendanceReport, this.httpOptions);
  }

  getStudentAttendanceExcelReport(StudentAttendanceReport: GetStudentAttendanceReport) {
    StudentAttendanceReport = this.defaultValuesService.getAllMandatoryVariable(StudentAttendanceReport);
    let apiurl = this.apiUrl + StudentAttendanceReport._tenantName + "/Report/getStudentAttendanceExcelReport";
    return this.http.post<GetStudentAttendanceReport>(apiurl, StudentAttendanceReport, this.httpOptions);
  }

}
