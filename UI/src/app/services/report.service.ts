import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { DefaultValuesService } from '../common/default-values.service';
import { AbsenceListByStudent, GetStudentAbsenceReport, StudentListForAbsenceSummary } from '../models/absence-summary.model';
import { GetScheduledAddDropReportModel, GetStudentAddDropReportModel , GetStudentAdvancedReportModel, GetStudentEnrollmentReportModel , GetStaffAdvancedReportModel, GetSchoolReportModel, GetStudentListByCourseSectionModel, GetStudentProgressReportModel, GetHonorRollReportModel} from '../models/report.model';

@Injectable({
  providedIn: 'root'
})
export class ReportService {
  httpOptions: { headers: any; };
  apiUrl: string = environment.apiURL;

  constructor(private defaultValuesService: DefaultValuesService,
    private http: HttpClient) {
    this.httpOptions = {
      headers: new HttpHeaders({
        'Cache-Control': 'no-cache',
        'Pragma': 'no-cache',
      })
    }
  }

  getStudentAddDropReport(obj: GetStudentAddDropReportModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName + "/Report/getstudentAddDropReport";
    return this.http.post<GetStudentAddDropReportModel>(apiurl, obj, this.httpOptions);
  }

  getScheduledAddDropReport(obj: GetScheduledAddDropReportModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName + "/Report/getSheduledAddDropReport";
    return this.http.post<GetScheduledAddDropReportModel>(apiurl, obj, this.httpOptions);
  }

  // getStudentInfoReport(obj: GetStudentInfoReportModel) {
  //   obj = this.defaultValuesService.getAllMandatoryVariable(obj);
  //   let apiurl = this.apiUrl + obj._tenantName + "/Report/getStudentInfoReport";
  //   return this.http.post<GetStudentAddDropReportModel>(apiurl, obj, this.httpOptions)
  // }

  getStudentEnrollmentReport(obj: GetStudentEnrollmentReportModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName + "/Report/getStudentEnrollmentReport";
    return this.http.post<GetStudentEnrollmentReportModel>(apiurl, obj, this.httpOptions)
  }

  getStudentAdvancedReport(obj: GetStudentAdvancedReportModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName + "/Report/getStudentAdvancedReport";
    return this.http.post<GetStudentAdvancedReportModel>(apiurl, obj, this.httpOptions)
  }

  getStaffAdvancedReport(obj: GetStaffAdvancedReportModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName + "/Report/getStaffAdvancedReport";
    return this.http.post<GetStaffAdvancedReportModel>(apiurl, obj, this.httpOptions)
  }

  getSchoolReport(obj: GetSchoolReportModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName + "/Report/getSchoolReport";
    return this.http.post<GetSchoolReportModel>(apiurl, obj, this.httpOptions)
  }

  getStudentListByCourseSection(obj: GetStudentListByCourseSectionModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName + "/Report/getStudentListByCourseSection";
    return this.http.post<GetStudentListByCourseSectionModel>(apiurl, obj, this.httpOptions)
  }

  getStudentProgressReport(obj: GetStudentProgressReportModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.academicYear = this.defaultValuesService.getAcademicYear();
    obj.markingPeriodStartDate = this.defaultValuesService.getMarkingPeriodStartDate();
    obj.markingPeriodEndDate = this.defaultValuesService.getMarkingPeriodEndDate();
    obj.markingPeriodTitle = this.defaultValuesService.getMarkingPeriodTitle();

    let apiurl = this.apiUrl + obj._tenantName + "/Report/GetStudentProgressReport";
    return this.http.post<GetStudentProgressReportModel>(apiurl, obj, this.httpOptions)
  }

  getHonorRollReport(obj: GetHonorRollReportModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.academicYear = this.defaultValuesService.getAcademicYear();
    obj.markingPeriodStartDate = this.defaultValuesService.getMarkingPeriodStartDate();
    obj.markingPeriodEndDate = this.defaultValuesService.getMarkingPeriodEndDate();

    let apiurl = this.apiUrl + obj._tenantName + "/Report/getHonorRollReport";
    return this.http.post<GetHonorRollReportModel>(apiurl, obj, this.httpOptions)
  }

  getAllStudentAbsenceList(obj: GetStudentAbsenceReport) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName + "/Report/getAllStudentAbsenceList";
    return this.http.post<StudentListForAbsenceSummary>(apiurl, obj, this.httpOptions)
  }

  getAbsenceListByStudent(obj: GetStudentAbsenceReport) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName + "/Report/getAbsenceListByStudent";
    return this.http.post<AbsenceListByStudent>(apiurl, obj, this.httpOptions)
  }
}
