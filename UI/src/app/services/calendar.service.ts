import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { CalendarAddViewModel, CalendarListModel } from '../models/calendar.model';
import { DefaultValuesService } from '../common/default-values.service';

@Injectable({
    providedIn: 'root'
})
export class CalendarService {
    private calendarId;
    apiUrl: string = environment.apiURL;
    httpOptions: { headers: any; };

    constructor(private http: HttpClient, private defaultValuesService: DefaultValuesService) {
        this.httpOptions = {
            headers: new HttpHeaders({
              'Cache-Control': 'no-cache',
              'Pragma': 'no-cache',
            })
          }
     }

    addCalendar(calendar: CalendarAddViewModel) {
        calendar = this.defaultValuesService.getAllMandatoryVariable(calendar);
        calendar.schoolCalendar.schoolId= this.defaultValuesService.getSchoolID();
        calendar.schoolCalendar.tenantId= this.defaultValuesService.getTenantID();
        calendar.schoolCalendar.createdBy= this.defaultValuesService.getEmailId();
        calendar.schoolCalendar.academicYear= this.defaultValuesService.getAcademicYear();
        let apiurl = this.apiUrl + calendar._tenantName + "/Calendar/addCalendar";
        return this.http.post<CalendarListModel>(apiurl, calendar, this.httpOptions)
    }
    viewCalendar(calendar: CalendarAddViewModel) {
        calendar = this.defaultValuesService.getAllMandatoryVariable(calendar);
        calendar.schoolCalendar.schoolId= this.defaultValuesService.getSchoolID();
        calendar.schoolCalendar.tenantId= this.defaultValuesService.getTenantID();
        calendar.schoolCalendar.academicYear= this.defaultValuesService.getAcademicYear();
        let apiurl = this.apiUrl + calendar._tenantName + "/Calendar/viewCalendar";
        return this.http.post<CalendarAddViewModel>(apiurl, calendar, this.httpOptions)
    }

    updateCalendar(calendar: CalendarAddViewModel) {
        calendar = this.defaultValuesService.getAllMandatoryVariable(calendar);
        calendar.schoolCalendar.schoolId= this.defaultValuesService.getSchoolID();
        calendar.schoolCalendar.tenantId= this.defaultValuesService.getTenantID();
        calendar.schoolCalendar.updatedBy= this.defaultValuesService.getEmailId();
        calendar.schoolCalendar.academicYear= this.defaultValuesService.getAcademicYear();
        let apiurl = this.apiUrl + calendar._tenantName + "/Calendar/updateCalendar";
        return this.http.put<CalendarListModel>(apiurl, calendar, this.httpOptions)
    }

    getAllCalendar(calendar: CalendarListModel) {
        calendar = this.defaultValuesService.getAllMandatoryVariable(calendar);
        calendar.academicYear= this.defaultValuesService.getAcademicYear();
        let apiurl = this.apiUrl + calendar._tenantName + "/Calendar/getAllCalendar";
        return this.http.post<CalendarListModel>(apiurl, calendar, this.httpOptions)
    }
    deleteCalendar(calendar: CalendarAddViewModel) {
        calendar = this.defaultValuesService.getAllMandatoryVariable(calendar);
        let apiurl = this.apiUrl + calendar._tenantName + "/Calendar/deleteCalendar";
        return this.http.post<CalendarAddViewModel>(apiurl, calendar, this.httpOptions)
    }
    setCalendarId(id: number) {
        this.calendarId = id
    }
    getCalendarId() {
        return this.calendarId;
    }

}
