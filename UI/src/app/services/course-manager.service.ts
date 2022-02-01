import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { DefaultValuesService } from '../common/default-values.service';
import { CalendarBellScheduleModel, CalendarBellScheduleViewModel } from '../models/calendar.model';
import { GetAllSubjectModel, AddSubjectModel, MassUpdateSubjectModel, MassUpdateProgramModel, AddProgramModel, DeleteSubjectModel, DeleteProgramModel, GetAllProgramModel, SearchCourseForScheduleModel, CourseStandardForCourseViewModel, CourseWithCourseSectionDetailsViewModel, CourseCatelogViewModel } from '../models/course-manager.model';
import { GetAllCourseListModel, AddCourseModel } from '../models/course-manager.model';
import { CryptoService } from './Crypto.service';

@Injectable({
    providedIn: 'root'
})
export class CourseManagerService {
    apiUrl: string = environment.apiURL;
    httpOptions: { headers: any; };
    constructor(private http: HttpClient, private cryptoService: CryptoService,
        private defaultValuesService: DefaultValuesService) {
        this.httpOptions = {
            headers: new HttpHeaders({
                'Cache-Control': 'no-cache',
                'Pragma': 'no-cache',
            })
        }
    }

    GetAllSubjectList(courseManager: GetAllSubjectModel) {
        courseManager = this.defaultValuesService.getAllMandatoryVariable(courseManager);
        courseManager.academicYear = this.defaultValuesService.getAcademicYear();
        let apiurl = this.apiUrl + courseManager._tenantName + "/CourseManager/getAllSubjectList";
        return this.http.post<GetAllSubjectModel>(apiurl, courseManager, this.httpOptions)
    }

    AddEditSubject(courseManager: MassUpdateSubjectModel) {
        courseManager = this.defaultValuesService.getAllMandatoryVariable(courseManager);
        let apiurl = this.apiUrl + courseManager._tenantName + "/CourseManager/addEditSubject";
        return this.http.put<AddSubjectModel>(apiurl, courseManager, this.httpOptions)
    }

    DeleteSubject(courseManager: DeleteSubjectModel) {
        courseManager = this.defaultValuesService.getAllMandatoryVariable(courseManager);
        courseManager.subject.tenantId = this.defaultValuesService.getTenantID();
        courseManager.subject.schoolId = this.defaultValuesService.getSchoolID();
        let apiurl = this.apiUrl + courseManager._tenantName + "/CourseManager/deleteSubject";
        return this.http.post<DeleteSubjectModel>(apiurl, courseManager, this.httpOptions)
    }

    GetAllProgramsList(courseManager: GetAllProgramModel) {
        courseManager = this.defaultValuesService.getAllMandatoryVariable(courseManager);
        let apiurl = this.apiUrl + courseManager._tenantName + "/CourseManager/getAllProgram";
        return this.http.post<GetAllProgramModel>(apiurl, courseManager, this.httpOptions)
    }
    AddEditPrograms(courseManager: MassUpdateProgramModel) {
        courseManager = this.defaultValuesService.getAllMandatoryVariable(courseManager);
        let apiurl = this.apiUrl + courseManager._tenantName + "/CourseManager/addEditProgram";
        return this.http.put<AddProgramModel>(apiurl, courseManager, this.httpOptions);
    }

    DeletePrograms(courseManager: DeleteProgramModel) {
        courseManager = this.defaultValuesService.getAllMandatoryVariable(courseManager);
        courseManager.programs.schoolId = this.defaultValuesService.getSchoolID();
        courseManager.programs.tenantId = this.defaultValuesService.getTenantID();
        let apiurl = this.apiUrl + courseManager._tenantName + "/CourseManager/deleteProgram";
        return this.http.post<DeleteProgramModel>(apiurl, courseManager, this.httpOptions)
    }



    GetAllCourseList(courseManager: GetAllCourseListModel) {
        courseManager = this.defaultValuesService.getAllMandatoryVariable(courseManager);
        courseManager.academicYear = this.defaultValuesService.getAcademicYear();
        let apiurl = this.apiUrl + courseManager._tenantName + "/CourseManager/getAllCourseList";
        return this.http.post<GetAllCourseListModel>(apiurl, courseManager, this.httpOptions)
    }
    AddCourse(courseManager: AddCourseModel) {
        courseManager = this.defaultValuesService.getAllMandatoryVariable(courseManager);
        courseManager.course.schoolId = this.defaultValuesService.getSchoolID();
        courseManager.course.tenantId = this.defaultValuesService.getTenantID();
        courseManager.course.createdBy = this.defaultValuesService.getUserGuidId();
        let apiurl = this.apiUrl + courseManager._tenantName + "/CourseManager/addCourse";
        return this.http.post<AddCourseModel>(apiurl, courseManager, this.httpOptions)
    }
    UpdateCourse(courseManager: AddCourseModel) {
        courseManager = this.defaultValuesService.getAllMandatoryVariable(courseManager);
        courseManager.course.schoolId = this.defaultValuesService.getSchoolID();
        courseManager.course.tenantId = this.defaultValuesService.getTenantID();
        courseManager.course.updatedBy = this.defaultValuesService.getUserGuidId();
        let apiurl = this.apiUrl + courseManager._tenantName + "/CourseManager/updateCourse";
        return this.http.put<AddCourseModel>(apiurl, courseManager, this.httpOptions)
    }

    DeleteCourse(courseManager: AddCourseModel) {
        courseManager = this.defaultValuesService.getAllMandatoryVariable(courseManager);
        courseManager.course.schoolId = this.defaultValuesService.getSchoolID();
        courseManager.course.tenantId = this.defaultValuesService.getTenantID();
        let apiurl = this.apiUrl + courseManager._tenantName + "/CourseManager/deleteCourse";
        return this.http.post<AddCourseModel>(apiurl, courseManager, this.httpOptions)
    }
    searchCourseForSchedule(searchParams: SearchCourseForScheduleModel) {
        searchParams = this.defaultValuesService.getAllMandatoryVariable(searchParams);
        searchParams.academicYear = this.defaultValuesService.getAcademicYear();
        let apiurl = this.apiUrl + searchParams._tenantName + "/CourseManager/searchCourseForSchedule";
        return this.http.post<SearchCourseForScheduleModel>(apiurl, searchParams, this.httpOptions)
    }

    getAllCourseStandardForCourse(obj: CourseStandardForCourseViewModel) {
        obj = this.defaultValuesService.getAllMandatoryVariable(obj);
        let apiurl = this.apiUrl + obj._tenantName + "/CourseManager/getAllCourseStandardForCourse";
        return this.http.post<CourseStandardForCourseViewModel>(apiurl, obj, this.httpOptions)
    }

    addEditBellSchedule(obj: CalendarBellScheduleModel) {
        obj = this.defaultValuesService.getAllMandatoryVariable(obj);
        // obj.bellSchedule.academicYear = this.defaultValuesService.getAcademicYear();
        obj.bellSchedule.createdBy = this.defaultValuesService.getUserGuidId();
        obj.bellSchedule.tenantId = this.defaultValuesService.getTenantID();
        obj.bellSchedule.schoolId = this.defaultValuesService.getSchoolID();

        let apiurl = this.apiUrl + obj._tenantName + "/CourseManager/addEditBellSchedule";
        return this.http.put<CalendarBellScheduleModel>(apiurl, obj, this.httpOptions)
    }

    getAllBellSchedule(obj: CalendarBellScheduleViewModel) {
        obj = this.defaultValuesService.getAllMandatoryVariable(obj);
        obj.academicYear = this.defaultValuesService.getAcademicYear();
        let apiurl = this.apiUrl + obj._tenantName + "/CourseManager/getAllBellSchedule";
        return this.http.post<CalendarBellScheduleViewModel>(apiurl, obj, this.httpOptions)
    }

    getCourseCatelog(obj: CourseCatelogViewModel) {
        obj = this.defaultValuesService.getAllMandatoryVariable(obj);
        obj.academicYear = this.defaultValuesService.getAcademicYear();
        let apiurl = this.apiUrl + obj._tenantName + "/CourseManager/getCourseCatelog";
        return this.http.post<CourseCatelogViewModel>(apiurl, obj, this.httpOptions)
    }
}
