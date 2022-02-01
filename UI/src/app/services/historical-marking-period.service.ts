import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { BehaviorSubject } from 'rxjs';
import { DefaultValuesService } from '../common/default-values.service';
import { HistoricalGradeAddViewModel, HistoricalMarkingPeriodAddViewModel, HistoricalMarkingPeriodListModel } from '../models/historical-marking-period.model';

@Injectable({
    providedIn: 'root'
})
export class HistoricalMarkingPeriodService {
    private studentDetails;
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

    setHistStudentDetails(data) {
        this.studentDetails = data;
      }
      getHistStudentDetails() {
        return this.studentDetails;
      }

    getAllhistoricalMarkingPeriodList(obj: HistoricalMarkingPeriodListModel) {
        obj = this.defaultValuesService.getAllMandatoryVariable(obj);
        obj.academicYear = this.defaultValuesService.getAcademicYear();
        let apiurl = this.apiUrl + obj._tenantName + "/HistoricalMarkingPeriod/getAllhistoricalMarkingPeriodList";
        return this.http.post<HistoricalMarkingPeriodListModel>(apiurl, obj, this.httpOptions)
    }

    addHistoricalMarkingPeriod(obj: HistoricalMarkingPeriodAddViewModel) {
        obj = this.defaultValuesService.getAllMandatoryVariable(obj);
        obj.historicalMarkingPeriod.schoolId = this.defaultValuesService.getSchoolID();
        obj.historicalMarkingPeriod.tenantId = this.defaultValuesService.getTenantID();
        obj.historicalMarkingPeriod.createdBy = this.defaultValuesService.getUserGuidId();
        let apiurl = this.apiUrl + obj._tenantName + "/HistoricalMarkingPeriod/addHistoricalMarkingPeriod";
        return this.http.post<HistoricalMarkingPeriodAddViewModel>(apiurl, obj, this.httpOptions)
    }

    updateHistoricalMarkingPeriod(obj: HistoricalMarkingPeriodAddViewModel) {
        obj = this.defaultValuesService.getAllMandatoryVariable(obj);
        obj.historicalMarkingPeriod.schoolId = this.defaultValuesService.getSchoolID();
        obj.historicalMarkingPeriod.tenantId = this.defaultValuesService.getTenantID();
        obj.historicalMarkingPeriod.updatedBy = this.defaultValuesService.getUserGuidId();
        let apiurl = this.apiUrl + obj._tenantName + "/HistoricalMarkingPeriod/updateHistoricalMarkingPeriod";
        return this.http.put<HistoricalMarkingPeriodAddViewModel>(apiurl, obj, this.httpOptions)
    }

    deleteHistoricalMarkingPeriod(obj: HistoricalMarkingPeriodAddViewModel) {
        obj = this.defaultValuesService.getAllMandatoryVariable(obj);
        obj.historicalMarkingPeriod.schoolId = this.defaultValuesService.getSchoolID();
        obj.historicalMarkingPeriod.tenantId = this.defaultValuesService.getTenantID()
        let apiurl = this.apiUrl + obj._tenantName + "/HistoricalMarkingPeriod/deleteHistoricalMarkingPeriod";
        return this.http.post<HistoricalMarkingPeriodAddViewModel>(apiurl, obj, this.httpOptions)
    }


    addUpdateHistoricalGrade(obj: HistoricalGradeAddViewModel) {
        obj = this.defaultValuesService.getAllMandatoryVariable(obj);
        obj.schoolId = this.defaultValuesService.getSchoolID();
        obj.tenantId = this.defaultValuesService.getTenantID();
        obj.CreatedBy = this.defaultValuesService.getUserGuidId();
        let apiurl = this.apiUrl + obj._tenantName + "/HistoricalMarkingPeriod/addUpdateHistoricalGrade";
        return this.http.post<HistoricalGradeAddViewModel>(apiurl, obj, this.httpOptions)
    }

    getAllHistoricalGradeList(obj: HistoricalGradeAddViewModel) {
        obj = this.defaultValuesService.getAllMandatoryVariable(obj);
        obj.schoolId = this.defaultValuesService.getSchoolID();
        obj.tenantId = this.defaultValuesService.getTenantID();
        let apiurl = this.apiUrl + obj._tenantName + "/HistoricalMarkingPeriod/getAllHistoricalGradeList";
        return this.http.post<HistoricalGradeAddViewModel>(apiurl, obj, this.httpOptions)
    }

}