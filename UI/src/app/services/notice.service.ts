import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';
import { NoticeAddViewModel, NoticeListViewModel } from '../models/notice.model';
import { NoticeDeleteModel } from '../models/notice-delete.model';
import { DefaultValuesService } from '../common/default-values.service';

@Injectable({
  providedIn: 'root'
})
export class NoticeService {
  private noticeSource = new BehaviorSubject(Object);
  currentNotice = this.noticeSource.asObservable();

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

  addNotice(notice: NoticeAddViewModel) {
    notice = this.defaultValuesService.getAllMandatoryVariable(notice);
    notice.notice.schoolId = this.defaultValuesService.getSchoolID();
    notice.notice.tenantId = this.defaultValuesService.getTenantID();
    notice.notice.createdBy = this.defaultValuesService.getUserGuidId();
    let apiurl = this.apiUrl + notice._tenantName + '/Notice/addNotice';
    return this.http.post<NoticeAddViewModel>(apiurl, notice,this.httpOptions);
  }
  updateNotice(notice: NoticeAddViewModel) {
    notice = this.defaultValuesService.getAllMandatoryVariable(notice);
    notice.notice.tenantId = this.defaultValuesService.getTenantID();
    notice.notice.updatedBy = this.defaultValuesService.getUserGuidId();
    notice.profileType = this.defaultValuesService.getUserMembershipType();
    let apiurl = this.apiUrl + notice._tenantName + '/Notice/updateNotice';
    return this.http.post<NoticeAddViewModel>(apiurl, notice,this.httpOptions);
  }
  getAllNotice(notice: NoticeListViewModel) {
    notice = this.defaultValuesService.getAllMandatoryVariable(notice);
    let apiurl = this.apiUrl + notice._tenantName + '/Notice/getAllNotice';
    return this.http.post<NoticeListViewModel>(apiurl, notice,this.httpOptions);
  }
  deleteNotice(notice: NoticeDeleteModel) {
    notice._tenantName = this.defaultValuesService.getTenent();
    notice.tenantId = this.defaultValuesService.getTenantID();
    notice._token = this.defaultValuesService.getToken();
    notice._userName = this.defaultValuesService.getUserName();
    let apiurl = this.apiUrl + notice._tenantName + '/Notice/deleteNotice';
    return this.http.post<NoticeDeleteModel>(apiurl, notice,this.httpOptions);
  }
  viewNotice(notice: NoticeAddViewModel) {
    notice = this.defaultValuesService.getAllMandatoryVariable(notice);
    let apiurl = this.apiUrl + notice._tenantName + '/Notice/viewNotice';
    return this.http.post<NoticeAddViewModel>(apiurl, notice,this.httpOptions);
  }
  changeNotice(obj) {
    this.noticeSource.next(obj);
  }
}
