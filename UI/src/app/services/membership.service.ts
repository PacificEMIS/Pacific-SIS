import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, of, Subject } from 'rxjs';
import { GetAllMembersList, Membership,AddMembershipModel } from '../models/membership.model';
import { NoticeAddViewModel, NoticeListViewModel } from '../models/notice.model';
import { NoticeDeleteModel } from '../models/notice-delete.model';
import { DefaultValuesService } from '../common/default-values.service';
@Injectable({
  providedIn: 'root'
})
export class MembershipService {

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

  getAllMembers(obj: GetAllMembersList) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName + '/Membership/getAllMembers';
    return this.http.post<GetAllMembersList>(apiurl, obj,this.httpOptions);
  }

  addMembership(obj: AddMembershipModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.membership.schoolId = this.defaultValuesService.getSchoolID();
    obj.membership.tenantId = this.defaultValuesService.getTenantID();
    obj.membership.createdBy = this.defaultValuesService.getUserGuidId();
    let apiurl = this.apiUrl + obj._tenantName + '/Membership/addMembership';
    return this.http.post<AddMembershipModel>(apiurl, obj,this.httpOptions);
  }
  updateMembership(obj: AddMembershipModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.membership.schoolId = this.defaultValuesService.getSchoolID();
    obj.membership.tenantId = this.defaultValuesService.getTenantID();
    obj.membership.updatedBy = this.defaultValuesService.getUserGuidId();
    let apiurl = this.apiUrl + obj._tenantName + '/Membership/updateMembership';
    return this.http.put<AddMembershipModel>(apiurl, obj,this.httpOptions);
  }

  deleteMembership(obj: AddMembershipModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.membership.schoolId = this.defaultValuesService.getSchoolID();
    obj.membership.tenantId = this.defaultValuesService.getTenantID();
    let apiurl = this.apiUrl + obj._tenantName + '/Membership/deleteMembership';
    return this.http.post<AddMembershipModel>(apiurl, obj,this.httpOptions);
  }

  getAllMembersBySchool(obj: GetAllMembersList) {
    obj._tenantName = this.defaultValuesService.getDefaultTenant();
    obj._userName = this.defaultValuesService.getUserName();
    obj._token = this.defaultValuesService.getToken();
    obj.tenantId = this.defaultValuesService.getTenantID();
    let apiurl = this.apiUrl + obj._tenantName + '/Membership/getAllMembers';
    return this.http.post<GetAllMembersList>(apiurl, obj,this.httpOptions);
  }

}
