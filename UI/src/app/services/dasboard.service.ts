import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { DashboardViewModel, ScheduledCourseSectionViewModel } from '../models/dashboard.model';
import { Observable,BehaviorSubject } from 'rxjs';
import { DefaultValuesService } from '../common/default-values.service';

@Injectable({
  providedIn: 'root'
})
export class DasboardService {

  apiUrl:string = environment.apiURL;
  private dashboardSubject = new BehaviorSubject(false);

  private courseSectionDetails = new BehaviorSubject(null);
  selectedCourseSectionDetails = this.courseSectionDetails.asObservable();
  httpOptions: { headers: any; };

  constructor(private http: HttpClient , private defaultValuesService: DefaultValuesService) { 
    this.httpOptions = {
      headers: new HttpHeaders({
        'Cache-Control': 'no-cache',
        'Pragma': 'no-cache',
      })
    }
  }

  getDashboardView(obj: DashboardViewModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.academicYear= this.defaultValuesService.getAcademicYear();
    obj.membershipId= +this.defaultValuesService.getuserMembershipID();
    let apiurl = this.apiUrl + obj._tenantName + "/Common/getDashboardView";
    return this.http.post<DashboardViewModel>(apiurl, obj,this.httpOptions)
  }

  getDashboardViewForCalendarView(obj: DashboardViewModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.academicYear= this.defaultValuesService.getAcademicYear();
    let apiurl = this.apiUrl + obj._tenantName + "/Common/getDashboardViewForCalendarView";
    return this.http.post<DashboardViewModel>(apiurl, obj,this.httpOptions)
  }
  getDashboardViewForStaff(obj: ScheduledCourseSectionViewModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.markingPeriodStartDate = this.defaultValuesService.getMarkingPeriodStartDate();
    obj.markingPeriodEndDate = this.defaultValuesService.getMarkingPeriodEndDate();
    let apiurl = this.apiUrl + obj._tenantName + "/Common/getDashboardViewForStaff";
    return this.http.post<ScheduledCourseSectionViewModel>(apiurl, obj,this.httpOptions)
  }

  sendPageLoadEvent(event) {
    this.dashboardSubject.next(event);
  }
  getPageLoadEvent(): Observable<any> {
    return this.dashboardSubject.asObservable();
  }

  getMissingAttendanceCountForDashboardView(obj: ScheduledCourseSectionViewModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName + "/Common/getMissingAttendanceCountForDashboardView";
    return this.http.post<ScheduledCourseSectionViewModel>(apiurl, obj,this.httpOptions)
  }

  selectedCourseSection(details) {
    this.courseSectionDetails.next(details);
  }
  



}