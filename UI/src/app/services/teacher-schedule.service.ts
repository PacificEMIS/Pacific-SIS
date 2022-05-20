import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { RemoveStaffCourseSectionSchedule, ScheduledStaffForCourseSection } from '../models/course-section.model';
import { AllScheduledCourseSectionForStaffModel, StaffScheduleViewModel } from '../models/teacher-schedule.model';
import { DefaultValuesService } from '../common/default-values.service';

@Injectable({
  providedIn: 'root'
})
export class TeacherScheduleService {
  apiUrl: string = environment.apiURL;
  httpOptions: { headers: HttpHeaders; };

  constructor(private http: HttpClient, private defaultValuesService: DefaultValuesService) {
    this.httpOptions = {
      headers: new HttpHeaders({
        'Cache-Control': 'no-cache',
        'Pragma': 'no-cache',
      })
    }
   }

  staffScheduleViewForCourseSection(teacherSchedule: StaffScheduleViewModel){
    teacherSchedule = this.defaultValuesService.getAllMandatoryVariable(teacherSchedule);
    let apiurl = this.apiUrl + teacherSchedule._tenantName + "/StaffSchedule/staffScheduleViewForCourseSection";
    return this.http.post<StaffScheduleViewModel>(apiurl, teacherSchedule,this.httpOptions);
  }
  
  addStaffCourseSectionSchedule(teacherSchedule: StaffScheduleViewModel){
    teacherSchedule = this.defaultValuesService.getAllMandatoryVariable(teacherSchedule);
    let apiurl = this.apiUrl + teacherSchedule._tenantName + "/StaffSchedule/addStaffCourseSectionSchedule";
    return this.http.post<StaffScheduleViewModel>(apiurl, teacherSchedule,this.httpOptions);
  }
  
  checkAvailabilityStaffCourseSectionSchedule(teacherSchedule: StaffScheduleViewModel){
    teacherSchedule = this.defaultValuesService.getAllMandatoryVariable(teacherSchedule);
    let apiurl = this.apiUrl + teacherSchedule._tenantName + "/StaffSchedule/checkAvailabilityStaffCourseSectionSchedule";
    return this.http.post<StaffScheduleViewModel>(apiurl, teacherSchedule,this.httpOptions);
  }

  getAllScheduledCourseSectionForStaff(reassignmentDetails: AllScheduledCourseSectionForStaffModel){
    reassignmentDetails = this.defaultValuesService.getAllMandatoryVariable(reassignmentDetails);
    reassignmentDetails.academicYear = this.defaultValuesService.getAcademicYear()
    let apiurl = this.apiUrl + reassignmentDetails._tenantName + "/StaffSchedule/getAllScheduledCourseSectionForStaff";
    return this.http.post<AllScheduledCourseSectionForStaffModel>(apiurl, reassignmentDetails,this.httpOptions);
  }

    addStaffCourseSectionReSchedule(reassignmentDetails: StaffScheduleViewModel) {
    reassignmentDetails = this.defaultValuesService.getAllMandatoryVariable(reassignmentDetails);
    let apiurl = this.apiUrl + reassignmentDetails._tenantName + "/StaffSchedule/AddStaffCourseSectionReSchedule";
    return this.http.post<StaffScheduleViewModel>(apiurl, reassignmentDetails,this.httpOptions);
  }

  checkAvailabilityStaffCourseSectionReSchedule(reassignmentDetails: ScheduledStaffForCourseSection){
    reassignmentDetails = this.defaultValuesService.getAllMandatoryVariable(reassignmentDetails);
    let apiurl = this.apiUrl + reassignmentDetails._tenantName + "/StaffSchedule/checkAvailabilityStaffCourseSectionReSchedule";
    return this.http.post<ScheduledStaffForCourseSection>(apiurl, reassignmentDetails,this.httpOptions)
  }
    
  addStaffCourseSectionReScheduleByCourse(reassignmentDetails : ScheduledStaffForCourseSection){
    reassignmentDetails = this.defaultValuesService.getAllMandatoryVariable(reassignmentDetails);
    let apiurl = this.apiUrl + reassignmentDetails._tenantName + "/StaffSchedule/addStaffCourseSectionReScheduleByCourse";
    return this.http.post<ScheduledStaffForCourseSection>(apiurl, reassignmentDetails,this.httpOptions)
  }

  removeStaffCourseSectionSchedule(removeStaffDetails : RemoveStaffCourseSectionSchedule){
    removeStaffDetails = this.defaultValuesService.getAllMandatoryVariable(removeStaffDetails);
    let apiurl = this.apiUrl + removeStaffDetails._tenantName + "/StaffSchedule/removeStaffCourseSectionSchedule";
    return this.http.post<RemoveStaffCourseSectionSchedule>(apiurl, removeStaffDetails,this.httpOptions)
  }

}
