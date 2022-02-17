import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { DefaultValuesService } from '../common/default-values.service';
import { GetScheduledAddDropReportModel, GetStudentAddDropReportModel , GetStudentAdvancedReportModel, GetStudentEnrollmentReportModel , GetStaffAdvancedReportModel, GetSchoolReportModel, GetStudentListByCourseSectionModel} from '../models/report.model';

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
}
