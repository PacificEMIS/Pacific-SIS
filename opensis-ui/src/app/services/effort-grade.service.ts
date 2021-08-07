import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { DefaultValuesService } from '../common/default-values.service';
import { StudentEffortGradeListModel } from '../models/student-effort-grade.model';

@Injectable({
    providedIn: 'root'
})
export class EffotrGradeService {
    apiUrl: string = environment.apiURL;
    userName = sessionStorage.getItem('user');
    httpOptions: { headers: any; };
    
    constructor(
        private http: HttpClient,
        private defaultValuesService: DefaultValuesService
    ) {this.httpOptions = {
        headers: new HttpHeaders({
          'Cache-Control': 'no-cache',
          'Pragma': 'no-cache',
        })
      } }

    addUpdateStudentEffortGrade(obj: StudentEffortGradeListModel) {
        obj = this.defaultValuesService.getAllMandatoryVariable(obj);
        let apiurl = this.apiUrl + obj._tenantName + "/StudentEffortGrade/addUpdateStudentEffortGrade";
        return this.http.post<StudentEffortGradeListModel>(apiurl, obj,this.httpOptions)
    }

    getAllStudentEffortGradeList(obj: StudentEffortGradeListModel) {
        obj = this.defaultValuesService.getAllMandatoryVariable(obj);
        let apiurl = this.apiUrl + obj._tenantName + "/StudentEffortGrade/getAllStudentEffortGradeList";
        return this.http.post<StudentEffortGradeListModel>(apiurl, obj,this.httpOptions)
    }
}
