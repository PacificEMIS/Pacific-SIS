import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { DefaultValuesService } from '../common/default-values.service';
import { StudentInfoReportModel } from '../models/student-info-report.model';

@Injectable({
  providedIn: 'root'
})
export class StudentReportService {

  apiUrl:string = environment.apiURL;
  httpOptions: { headers: any; };

  constructor(
    private defaultValuesService: DefaultValuesService,
    private http: HttpClient
  ) { 
    this.httpOptions = {
      headers: new HttpHeaders({
        'Cache-Control': 'no-cache',
        'Pragma': 'no-cache',
      })
    }
  }

  getStudentInfoReport(obj: StudentInfoReportModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    const apiurl = this.apiUrl + obj._tenantName + '/Report/getStudentInfoReport';
    return this.http.post<StudentInfoReportModel>(apiurl, obj,this.httpOptions);
  }
}
