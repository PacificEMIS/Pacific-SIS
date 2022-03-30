import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { GetUnassociatedStudentListByCourseSectionModel, ScheduleCoursesForStudent360Model, ScheduledCourseSectionListForStudent360Model, ScheduledStudentDeleteModel, ScheduledStudentDropModel, ScheduleStudentListViewModel, StudentCourseSectionScheduleAddViewModel, StudentScheduleReportViewModel } from '../models/student-schedule.model';
import { DefaultValuesService } from '../common/default-values.service';
import { StudentListModel } from '../models/student.model';
import { ScheduleReportFilterModel } from '../models/schedule-report-filter.model';

@Injectable({
  providedIn: 'root'
})
export class StudentScheduleService {
  apiUrl:string = environment.apiURL;
  httpOptions: { headers: any; };
  constructor(private http: HttpClient, private defaultValuesService: DefaultValuesService) {
    this.httpOptions = {
      headers: new HttpHeaders({
        'Cache-Control': 'no-cache',
        'Pragma': 'no-cache',
      })
    }
   }

  addStudentCourseSectionSchedule(Obj: StudentCourseSectionScheduleAddViewModel){
    Obj = this.defaultValuesService.getAllMandatoryVariable(Obj);
    Obj.createdBy= this.defaultValuesService.getUserGuidId();
    let apiurl = this.apiUrl + Obj._tenantName + '/StudentSchedule/addStudentCourseSectionSchedule';
    return this.http.post<StudentCourseSectionScheduleAddViewModel>(apiurl, Obj,this.httpOptions);
  }

  searchScheduledStudentForGroupDrop(Obj: ScheduleStudentListViewModel){
    Obj = this.defaultValuesService.getAllMandatoryVariable(Obj);
    let apiurl = this.apiUrl + Obj._tenantName + '/StudentSchedule/getStudentListByCourseSection';  
    return this.http.post<ScheduleStudentListViewModel>(apiurl, Obj,this.httpOptions);
  }

  getUnassociatedStudentListByCourseSection(Obj: GetUnassociatedStudentListByCourseSectionModel) {
    Obj = this.defaultValuesService.getAllMandatoryVariable(Obj);
    let apiurl = this.apiUrl + Obj._tenantName + '/StudentSchedule/getUnassociatedStudentListByCourseSection';
    return this.http.post<GetUnassociatedStudentListByCourseSectionModel>(apiurl, Obj, this.httpOptions);
  }

  groupDropForScheduledStudent(Obj: ScheduledStudentDropModel){
    Obj = this.defaultValuesService.getAllMandatoryVariable(Obj);
    Obj.updatedBy
    let apiurl = this.apiUrl + Obj._tenantName + '/StudentSchedule/groupDropForScheduledStudent';
    return this.http.put<ScheduledStudentDropModel>(apiurl, Obj,this.httpOptions);
  }

  groupDeleteForScheduledStudent(Obj: ScheduledStudentDeleteModel) {
    Obj = this.defaultValuesService.getAllMandatoryVariable(Obj);
    let apiurl = this.apiUrl + Obj._tenantName + '/StudentSchedule/groupDeleteForScheduledStudent';
    return this.http.post<ScheduledStudentDeleteModel>(apiurl, Obj, this.httpOptions);
  }

  studentScheduleReport(Obj: StudentScheduleReportViewModel){
    Obj = this.defaultValuesService.getAllMandatoryVariable(Obj);
    let apiurl = this.apiUrl + Obj._tenantName + '/StudentSchedule/studentScheduleReport';
    return this.http.post<StudentScheduleReportViewModel>(apiurl, Obj,this.httpOptions);
  }

  scheduleCoursesForStudent360(Obj: ScheduleCoursesForStudent360Model){
    Obj = this.defaultValuesService.getAllMandatoryVariable(Obj);
    let apiurl = this.apiUrl + Obj._tenantName + '/StudentSchedule/scheduleCoursesForStudent360';
    return this.http.post<ScheduleCoursesForStudent360Model>(apiurl, Obj,this.httpOptions);
  }

  dropScheduledCourseSectionForStudent360(Obj: ScheduledStudentDropModel){
    Obj = this.defaultValuesService.getAllMandatoryVariable(Obj);
    let apiurl = this.apiUrl + Obj._tenantName + '/StudentSchedule/dropScheduledCourseSectionForStudent360';
    return this.http.put<ScheduledStudentDropModel>(apiurl, Obj,this.httpOptions);
  }
  
  scheduleCourseSectionListForStudent360(Obj: ScheduledCourseSectionListForStudent360Model){
    Obj = this.defaultValuesService.getAllMandatoryVariable(Obj);
    let apiurl = this.apiUrl + Obj._tenantName + '/StudentSchedule/scheduleCourseSectionListForStudent360';
    return this.http.post<ScheduledCourseSectionListForStudent360Model>(apiurl, Obj,this.httpOptions);
  }

  scheduledCourseSectionListForReport(Obj: ScheduleReportFilterModel){
    Obj = this.defaultValuesService.getAllMandatoryVariable(Obj);
    let apiurl = this.apiUrl + Obj._tenantName + '/Report/scheduledCourseSectionList';
    return this.http.post<ScheduleReportFilterModel>(apiurl, Obj,this.httpOptions);
  }

}
