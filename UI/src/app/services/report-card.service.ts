import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { DefaultValuesService } from '../common/default-values.service';
import { AddCourseCommentCategoryModel, AddReportCardPdf, DeleteCourseCommentCategoryModel, GetAllCourseCommentCategoryModel, UpdateSortOrderForCourseCommentCategoryModel } from '../models/report-card.model';

@Injectable({
  providedIn: 'root'
})
export class ReportCardService {
  httpOptions: { headers: any; };

  constructor(private defaultValuesService:DefaultValuesService,
    private http:HttpClient) {
      this.httpOptions = {
        headers: new HttpHeaders({
          'Cache-Control': 'no-cache',
          'Pragma': 'no-cache',
        })
      }
     }
    apiUrl: string = environment.apiURL;


  addCourseCommentCategory(reportCardComment:AddCourseCommentCategoryModel){
    reportCardComment = this.defaultValuesService.getAllMandatoryVariable(reportCardComment);
    let apiurl = this.apiUrl + reportCardComment._tenantName+"/ReportCard/addCourseCommentCategory";
    return this.http.post<AddCourseCommentCategoryModel>(apiurl,reportCardComment,this.httpOptions)
  }

  deleteCourseCommentCategory(reportCardComment:DeleteCourseCommentCategoryModel){
    reportCardComment = this.defaultValuesService.getAllMandatoryVariable(reportCardComment);
    let apiurl = this.apiUrl + reportCardComment._tenantName+"/ReportCard/deleteCourseCommentCategory";
    return this.http.post<DeleteCourseCommentCategoryModel>(apiurl,reportCardComment,this.httpOptions)
  }

  updateSortOrderForCourseCommentCategory(reportCardComment:UpdateSortOrderForCourseCommentCategoryModel){
    reportCardComment = this.defaultValuesService.getAllMandatoryVariable(reportCardComment);
    reportCardComment.updatedBy = this.defaultValuesService.getUserGuidId();
    let apiurl = this.apiUrl + reportCardComment._tenantName+"/ReportCard/updateSortOrderForCourseCommentCategory";
    return this.http.post<UpdateSortOrderForCourseCommentCategoryModel>(apiurl,reportCardComment,this.httpOptions)
  }

  getAllCourseCommentCategory(reportCardComment:GetAllCourseCommentCategoryModel){
    reportCardComment = this.defaultValuesService.getAllMandatoryVariable(reportCardComment);
    reportCardComment.academicYear = this.defaultValuesService.getAcademicYear()
    let apiurl = this.apiUrl + reportCardComment._tenantName+"/ReportCard/getAllCourseCommentCategory";
    return this.http.post<GetAllCourseCommentCategoryModel>(apiurl,reportCardComment,this.httpOptions)
  }

  addReportCard(reportCard:AddReportCardPdf){
    reportCard = this.defaultValuesService.getAllMandatoryVariable(reportCard);
    reportCard.createdBy= this.defaultValuesService.getUserGuidId();
    let apiurl = this.apiUrl + reportCard._tenantName+"/ReportCard/addReportCard";
    return this.http.post<GetAllCourseCommentCategoryModel>(apiurl,reportCard,this.httpOptions)
  }

  getReportCardForStudents(reportCard:AddReportCardPdf){
    reportCard = this.defaultValuesService.getAllMandatoryVariable(reportCard);
    reportCard.createdBy= this.defaultValuesService.getUserGuidId();
    let apiurl = this.apiUrl + reportCard._tenantName+"/ReportCard/getReportCardForStudents";
    return this.http.post<GetAllCourseCommentCategoryModel>(apiurl,reportCard,this.httpOptions)
  }

  generateReportCard(reportCard:AddReportCardPdf){
    reportCard = this.defaultValuesService.getAllMandatoryVariable(reportCard);
    let apiurl = this.apiUrl + reportCard._tenantName+"/ReportCard/generateReportCard";
    return this.http.post<GetAllCourseCommentCategoryModel>(apiurl,reportCard,this.httpOptions)
  }

}
