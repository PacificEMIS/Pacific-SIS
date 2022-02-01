import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { DefaultValuesService } from '../common/default-values.service';
import { CourseSectionForAttendanceViewModel, StudentAttendanceAddViewModel, StudentAttendanceListViewModel, StudentDailyAttendanceListViewModel } from '../models/attendance-administrative.model';
import { GetAllCourseListModel } from '../models/course-manager.model';
import { ScheduledCourseSectionViewModel } from '../models/dashboard.model';
import { GetAllStaffModel } from '../models/staff.model';
import {
  AddUpdateStudentAttendanceModel,
  AddUpdateStudentAttendanceModelFor360,
  GetAllStudentAttendanceListModel,
  SearchCourseSectionForStudentAttendance,
  StaffDetailsModel, 
  StudentAttendanceHistoryViewModel, 
  StudentUpdateAttendanceCommentsModel} from '../models/take-attendance-list.model';

  import {StudentRecalculateDailyAttendance} from '../models/student-recalculate-attendance.model'

@Injectable({
  providedIn: 'root'
})
export class StudentAttendanceService {
  apiUrl: string = environment.apiURL;
  userName = this.defaultValuesService.getUserName();
  staffDetails: StaffDetailsModel = new StaffDetailsModel();
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

  setStaffDetails(staffDetails) {
    this.staffDetails = staffDetails;
  }

  getStaffDetails() {
    return this.staffDetails;
  }

  // getAllStaffList(obj: GetAllStaffModel){
  //   obj = this.defaultValuesService.getAllMandatoryVariable(obj);
  //   const apiurl = this.apiUrl + obj._tenantName + '/Staff/getAllStaffList';
  //   return this.http.post<GetAllStaffModel>(apiurl, obj,this.httpOptions);
  // }

  getAllCourcesForStudentAttendance(obj: SearchCourseSectionForStudentAttendance) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.academicYear = this.defaultValuesService.getAcademicYear()
    const apiurl = this.apiUrl + obj._tenantName + '/StudentAttendance/searchCourseSectionForStudentAttendance';
    return this.http.post<SearchCourseSectionForStudentAttendance>(apiurl, obj,this.httpOptions);
  }

  getAllCourseSectionList(courseManager: GetAllCourseListModel) {
    courseManager = this.defaultValuesService.getAllMandatoryVariable(courseManager);
    const apiurl = this.apiUrl + courseManager._tenantName + '/CourseManager/getAllCourseList';
    return this.http.post<GetAllCourseListModel>(apiurl, courseManager,this.httpOptions);
  }

  getAllStudentAttendanceList(studentAttendance: GetAllStudentAttendanceListModel) {
    studentAttendance = this.defaultValuesService.getAllMandatoryVariable(studentAttendance);
    const apiurl = this.apiUrl + studentAttendance._tenantName + '/StudentAttendance/getAllStudentAttendanceList';
    return this.http.post<GetAllStudentAttendanceListModel>(apiurl, studentAttendance,this.httpOptions);
  }

  addUpdateStudentAttendance(studentAttendance: AddUpdateStudentAttendanceModel) {
    studentAttendance = this.defaultValuesService.getAllMandatoryVariable(studentAttendance);
    const apiurl = this.apiUrl + studentAttendance._tenantName + '/StudentAttendance/addUpdateStudentAttendance';
    return this.http.post<AddUpdateStudentAttendanceModel>(apiurl, studentAttendance,this.httpOptions);
  }

  addUpdateStudentAttendanceForStudent360(studentAttendance: AddUpdateStudentAttendanceModelFor360) {
    studentAttendance = this.defaultValuesService.getAllMandatoryVariable(studentAttendance);
    studentAttendance.updatedBy = this.defaultValuesService.getUserGuidId();
    studentAttendance.memberShipId = +this.defaultValuesService.getuserMembershipID();
    const apiurl = this.apiUrl + studentAttendance._tenantName + '/StudentAttendance/addUpdateStudentAttendanceForStudent360';
    return this.http.post<AddUpdateStudentAttendanceModelFor360>(apiurl, studentAttendance,this.httpOptions);
  }

  staffListForMissingAttendance(obj: GetAllStaffModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.academicYear = this.defaultValuesService.getAcademicYear()
    const apiurl = this.apiUrl + obj._tenantName + '/StudentAttendance/staffListForMissingAttendance';
    return this.http.post<GetAllStaffModel>(apiurl, obj,this.httpOptions);
  }

  missingAttendanceList(obj: GetAllStaffModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.academicYear = this.defaultValuesService.getAcademicYear()
    const apiurl = this.apiUrl + obj._tenantName + '/StudentAttendance/missingAttendanceList';
    return this.http.post<ScheduledCourseSectionViewModel>(apiurl, obj,this.httpOptions);
  }

  addUpdateStudentAttendanceComments(obj: StudentUpdateAttendanceCommentsModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.studentAttendanceComments.tenantId = this.defaultValuesService.getTenantID();
    obj.studentAttendanceComments.schoolId = this.defaultValuesService.getSchoolID();
    const apiurl = this.apiUrl + obj._tenantName + '/StudentAttendance/addUpdateStudentAttendanceComments';
    return this.http.post<StudentUpdateAttendanceCommentsModel>(apiurl, obj,this.httpOptions);
  }
  recalculateDailyAttendence(obj: StudentRecalculateDailyAttendance){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    const apiurl = this.apiUrl + obj._tenantName + '/StudentAttendance/reCalculateDailyAttendance';
    return this.http.post<StudentRecalculateDailyAttendance>(apiurl, obj,this.httpOptions);
  }

  getAllStudentAttendanceListForAdministration(obj: StudentAttendanceListViewModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName + "/StudentAttendance/getAllStudentAttendanceListForAdministration";
    return this.http.post<StudentAttendanceListViewModel>(apiurl, obj, this.httpOptions);
   }

   updateStudentDailyAttendance(obj: StudentDailyAttendanceListViewModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName + "/StudentAttendance/updateStudentDailyAttendance";
    return this.http.post<StudentDailyAttendanceListViewModel>(apiurl, obj, this.httpOptions);
   }
   
   courseSectionListForAttendanceAdministration(obj: CourseSectionForAttendanceViewModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.academicYear= this.defaultValuesService.getAcademicYear();
    let apiurl = this.apiUrl + obj._tenantName + "/StudentAttendance/courseSectionListForAttendanceAdministration";
    return this.http.post<CourseSectionForAttendanceViewModel>(apiurl, obj, this.httpOptions);
   }

   addAbsences(obj: StudentAttendanceAddViewModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.createdBy= this.defaultValuesService.getUserGuidId();
    obj.membershipId= +this.defaultValuesService.getuserMembershipID();
    let apiurl = this.apiUrl + obj._tenantName + "/StudentAttendance/addAbsences";
    return this.http.post<StudentAttendanceAddViewModel>(apiurl, obj, this.httpOptions);
   }

   getStudentAttendanceHistory(obj: StudentAttendanceHistoryViewModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName + "/StudentAttendance/getStudentAttendanceHistory";
    return this.http.post<StudentAttendanceHistoryViewModel>(apiurl, obj, this.httpOptions);
   }
  
}
