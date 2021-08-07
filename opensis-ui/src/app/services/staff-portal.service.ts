import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { environment } from '../../environments/environment';
import { DefaultValuesService } from '../common/default-values.service';
import { AllCourseSectionView } from '../models/course-manager.model';
import { ScheduledCourseSectionViewModel } from '../models/dashboard.model';
import { AddAssignmentModel, AddAssignmentTypeModel, GetAllAssignmentsModel } from '../models/staff-portal-assignment.model';
import { GetAllStaffModel } from '../models/staff.model';


@Injectable({
    providedIn: 'root'
})
export class StaffPortalService {
    apiUrl: string = environment.apiURL;
    courseDetails: AllCourseSectionView;
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

    setCourseSectionDetails(courseSectionDetails) {
        this.courseDetails = courseSectionDetails;
    }

    getCourseSectionDetails() {
        return this.courseDetails;
    }

    //teacher take atendance
    private changeStatusTo = new BehaviorSubject(false);
    attendanceCalled = this.changeStatusTo.asObservable();

    changeattendanceStatus(status) {
        this.changeStatusTo.next(status);
    }

    missingAttendanceListForCourseSection(obj: GetAllStaffModel) {
        obj = this.defaultValuesService.getAllMandatoryVariable(obj);
        const apiurl = this.apiUrl + obj._tenantName + '/StaffPortal/missingAttendanceListForCourseSection';
        return this.http.post<ScheduledCourseSectionViewModel>(apiurl, obj,this.httpOptions);
    }
}