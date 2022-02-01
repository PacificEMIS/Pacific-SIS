import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { MarkingPeriodListModel,MarkingPeriodAddModel,SemesterAddModel,QuarterAddModel,ProgressPeriodAddModel, GetAcademicYearListModel, GetMarkingPeriodTitleListModel,GetAllMarkingPeriodTitle, GetMarkingPeriodByCourseSectionModel} from '../models/marking-period.model';
import { BehaviorSubject } from 'rxjs';
import { DefaultValuesService } from '../common/default-values.service';
@Injectable({
  providedIn: 'root'
})
export class MarkingPeriodService {

  apiUrl: string = environment.apiURL;
  private currentYear = new BehaviorSubject(false);
  currentY = this.currentYear.asObservable();
  httpOptions: { headers: any; };

  constructor(private http: HttpClient, private defaultValuesService: DefaultValuesService) { 
    this.httpOptions = {
      headers: new HttpHeaders({
        'Cache-Control': 'no-cache',
        'Pragma': 'no-cache',
      })
    }
  }

  GetMarkingPeriod(obj: MarkingPeriodListModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName + '/MarkingPeriod/getMarkingPeriod';
    return this.http.post<MarkingPeriodListModel>(apiurl, obj,this.httpOptions);
  }

  AddSchoolYear(obj: MarkingPeriodAddModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.tableSchoolYears.schoolId = this.defaultValuesService.getSchoolID();
    obj.tableSchoolYears.tenantId = this.defaultValuesService.getTenantID();
    obj.tableSchoolYears.createdBy = this.defaultValuesService.getUserGuidId();
    let apiurl = this.apiUrl + obj._tenantName + '/MarkingPeriod/addSchoolYear';
    return this.http.post<MarkingPeriodAddModel>(apiurl, obj,this.httpOptions);
  }
  UpdateSchoolYear(obj: MarkingPeriodAddModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.tableSchoolYears.schoolId = this.defaultValuesService.getSchoolID();
    obj.tableSchoolYears.tenantId = this.defaultValuesService.getTenantID();
    obj.tableSchoolYears.updatedBy = this.defaultValuesService.getUserGuidId();
    let apiurl = this.apiUrl + obj._tenantName + '/MarkingPeriod/updateSchoolYear';
    return this.http.put<MarkingPeriodAddModel>(apiurl, obj,this.httpOptions);
  }
  DeleteSchoolYear(obj: MarkingPeriodAddModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.tableSchoolYears.schoolId = this.defaultValuesService.getSchoolID();
    obj.tableSchoolYears.tenantId = this.defaultValuesService.getTenantID();
    let apiurl = this.apiUrl + obj._tenantName + '/MarkingPeriod/deleteSchoolYear';
    return this.http.post<MarkingPeriodAddModel>(apiurl, obj,this.httpOptions);
  }
  AddSemester(obj: SemesterAddModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.tableSemesters.schoolId = this.defaultValuesService.getSchoolID();
    obj.tableSemesters.tenantId = this.defaultValuesService.getTenantID();
    obj.tableSemesters.createdBy = this.defaultValuesService.getUserGuidId();
    let apiurl = this.apiUrl + obj._tenantName + '/MarkingPeriod/addSemester';
    return this.http.post<SemesterAddModel>(apiurl, obj,this.httpOptions);
  }
  UpdateSemester(obj: SemesterAddModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.tableSemesters.schoolId = this.defaultValuesService.getSchoolID();
    obj.tableSemesters.tenantId = this.defaultValuesService.getTenantID();
    obj.tableSemesters.updatedBy = this.defaultValuesService.getUserGuidId();
    let apiurl = this.apiUrl + obj._tenantName + '/MarkingPeriod/updateSemester';
    return this.http.put<SemesterAddModel>(apiurl, obj,this.httpOptions);
  }
  DeleteSemester(obj: SemesterAddModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.tableSemesters.schoolId = this.defaultValuesService.getSchoolID();
    obj.tableSemesters.tenantId = this.defaultValuesService.getTenantID();
    let apiurl = this.apiUrl + obj._tenantName + '/MarkingPeriod/deleteSemester';
    return this.http.post<SemesterAddModel>(apiurl, obj,this.httpOptions);
  }

  AddQuarter(obj: QuarterAddModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.tableQuarter.schoolId = this.defaultValuesService.getSchoolID();
    obj.tableQuarter.tenantId = this.defaultValuesService.getTenantID();
    obj.tableQuarter.createdBy = this.defaultValuesService.getUserGuidId();
    let apiurl = this.apiUrl + obj._tenantName + '/MarkingPeriod/addQuarter';
    return this.http.post<QuarterAddModel>(apiurl, obj,this.httpOptions);
  }
  UpdateQuarter(obj: QuarterAddModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.tableQuarter.schoolId = this.defaultValuesService.getSchoolID();
    obj.tableQuarter.tenantId = this.defaultValuesService.getTenantID();
    obj.tableQuarter.updatedBy = this.defaultValuesService.getUserGuidId();
    let apiurl = this.apiUrl + obj._tenantName + '/MarkingPeriod/updateQuarter';
    return this.http.put<QuarterAddModel>(apiurl, obj,this.httpOptions);
  }
  DeleteQuarter(obj: QuarterAddModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.tableQuarter.schoolId = this.defaultValuesService.getSchoolID();
    obj.tableQuarter.tenantId = this.defaultValuesService.getTenantID();
    let apiurl = this.apiUrl + obj._tenantName + '/MarkingPeriod/deleteQuarter';
    return this.http.post<QuarterAddModel>(apiurl, obj,this.httpOptions);
  }
  AddProgressPeriod(obj: ProgressPeriodAddModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.tableProgressPeriods.schoolId = this.defaultValuesService.getSchoolID();
    obj.tableProgressPeriods.tenantId = this.defaultValuesService.getTenantID();
    obj.tableProgressPeriods.createdBy = this.defaultValuesService.getUserGuidId();
    let apiurl = this.apiUrl + obj._tenantName + '/MarkingPeriod/addProgressPeriod';
    return this.http.post<ProgressPeriodAddModel>(apiurl, obj,this.httpOptions);
  }
  UpdateProgressPeriod(obj: ProgressPeriodAddModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.tableProgressPeriods.schoolId = this.defaultValuesService.getSchoolID();
    obj.tableProgressPeriods.tenantId = this.defaultValuesService.getTenantID();
    obj.tableProgressPeriods.updatedBy = this.defaultValuesService.getUserGuidId();
    let apiurl = this.apiUrl + obj._tenantName + '/MarkingPeriod/updateProgressPeriod';
    return this.http.put<ProgressPeriodAddModel>(apiurl, obj,this.httpOptions);
  }
  DeleteProgressPeriod(obj: ProgressPeriodAddModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.tableProgressPeriods.schoolId = this.defaultValuesService.getSchoolID();
    obj.tableProgressPeriods.tenantId = this.defaultValuesService.getTenantID();
    let apiurl = this.apiUrl + obj._tenantName + '/MarkingPeriod/deleteProgressPeriod';
    return this.http.post<ProgressPeriodAddModel>(apiurl, obj,this.httpOptions);
  }

  // getAcademicYearList and getMarkingPeriodTitleList
  //  is for Select Dropdown Bar for selecting academic year and period
  // which is in right upper corner of opensisv2 site.
  
  getAcademicYearList(obj: GetAcademicYearListModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName + '/MarkingPeriod/getAcademicYearList';
    return this.http.post<GetAcademicYearListModel>(apiurl, obj,this.httpOptions);
  }

  getMarkingPeriodTitleList(obj: GetMarkingPeriodTitleListModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName + '/MarkingPeriod/getMarkingPeriodTitleList';
    return this.http.post<GetMarkingPeriodTitleListModel>(apiurl, obj,this.httpOptions);
  }
  getCurrentYear(message: boolean) {
    this.currentYear.next(message);
  }

  getAllMarkingPeriodList(obj: GetMarkingPeriodTitleListModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName + '/MarkingPeriod/getAllMarkingPeriodList';
    return this.http.post<GetAllMarkingPeriodTitle>(apiurl, obj,this.httpOptions);
  }

  getMarkingPeriodsByCourseSection(obj: GetMarkingPeriodByCourseSectionModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.schoolId = this.defaultValuesService.getSchoolID();
    obj.academicYear = this.defaultValuesService.getAcademicYear();
    obj.markingPeriodStartDate = this.defaultValuesService.getMarkingPeriodStartDate();
    obj.markingPeriodEndDate = this.defaultValuesService.getMarkingPeriodEndDate();

    let apiurl = this.apiUrl + obj._tenantName + '/MarkingPeriod/getMarkingPeriodsByCourseSection';
    return this.http.post<GetAllMarkingPeriodTitle>(apiurl, obj,this.httpOptions);
  }
}
