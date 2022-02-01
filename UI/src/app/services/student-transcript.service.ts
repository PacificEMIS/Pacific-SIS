import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { DefaultValuesService } from '../common/default-values.service';
import { GetStudentTranscriptModel, StudentTranscript } from '../models/student-transcript.model';

@Injectable({
  providedIn: 'root'
})

export class StudentTranscriptService {
    apiUrl: string = environment.apiURL;
  httpOptions: { headers: any; };

    constructor(
      private http: HttpClient,
      private defaultValuesService: DefaultValuesService) {
        this.httpOptions = {
          headers: new HttpHeaders({
            'Cache-Control': 'no-cache',
            'Pragma': 'no-cache',
          })
        }
       }
    
      addTranscriptForStudent(obj: StudentTranscript) {
        obj = this.defaultValuesService.getAllMandatoryVariable(obj);
        obj.createdBy= this.defaultValuesService.getUserGuidId();
        const apiurl = this.apiUrl + obj._tenantName + '/Student/addTranscriptForStudent';
        return this.http.post<StudentTranscript>(apiurl, obj,this.httpOptions);
      }

      getTranscriptForStudents(reportCard: GetStudentTranscriptModel) {
        reportCard = this.defaultValuesService.getAllMandatoryVariable(reportCard);
        reportCard.academicYear = this.defaultValuesService.getAcademicYear();
        let apiurl = this.apiUrl + reportCard._tenantName + "/Student/getTranscriptForStudents";
        return this.http.post<any>(apiurl, reportCard, this.httpOptions)
      }

      generateTranscriptForStudent(obj: StudentTranscript) {
        obj = this.defaultValuesService.getAllMandatoryVariable(obj);
        const apiurl = this.apiUrl + obj._tenantName + '/Student/generateTranscriptForStudent';
        return this.http.post<StudentTranscript>(apiurl, obj,this.httpOptions);
      }
}