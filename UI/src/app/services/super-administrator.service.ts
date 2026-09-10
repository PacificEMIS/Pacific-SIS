import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { DefaultValuesService } from '../common/default-values.service';
import { CryptoService } from './Crypto.service';
import {
  SuperAdministratorActionModel,
  SuperAdministratorAddModel,
  SuperAdministratorCandidateListModel,
  SuperAdministratorListModel
} from '../models/super-administrator.model';

/**
 * Settings > Administration > Super Administrators.
 * Every call is answered only for callers whose own login is an active
 * Super Administrator; the API derives the caller from the token.
 */
@Injectable({
  providedIn: 'root'
})
export class SuperAdministratorService {

  apiUrl: string = environment.apiURL;
  httpOptions: { headers: any; };

  constructor(private http: HttpClient, private defaultValuesService: DefaultValuesService, private cryptoService: CryptoService) {
    this.httpOptions = {
      headers: new HttpHeaders({
        'Cache-Control': 'no-cache',
        'Pragma': 'no-cache',
      })
    };
  }

  getAll(obj: SuperAdministratorListModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    const apiurl = this.apiUrl + obj._tenantName + '/SuperAdministrator/getAll';
    return this.http.post<SuperAdministratorListModel>(apiurl, obj, this.httpOptions);
  }

  add(obj: SuperAdministratorAddModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.createdBy = this.defaultValuesService.getUserGuidId();
    // Same AES wrapping as staff portal passwords; the API decrypts then hashes.
    obj.passwordHash = this.cryptoService.encrypt(obj.passwordHash);
    const apiurl = this.apiUrl + obj._tenantName + '/SuperAdministrator/add';
    return this.http.post<SuperAdministratorAddModel>(apiurl, obj, this.httpOptions);
  }

  setActiveStatus(obj: SuperAdministratorActionModel) {
    obj = this.prepareAction(obj);
    const apiurl = this.apiUrl + obj._tenantName + '/SuperAdministrator/setActiveStatus';
    return this.http.post<SuperAdministratorActionModel>(apiurl, obj, this.httpOptions);
  }

  delete(obj: SuperAdministratorActionModel) {
    obj = this.prepareAction(obj);
    const apiurl = this.apiUrl + obj._tenantName + '/SuperAdministrator/delete';
    return this.http.post<SuperAdministratorActionModel>(apiurl, obj, this.httpOptions);
  }

  getPromotionCandidates(obj: SuperAdministratorCandidateListModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    const apiurl = this.apiUrl + obj._tenantName + '/SuperAdministrator/getPromotionCandidates';
    return this.http.post<SuperAdministratorCandidateListModel>(apiurl, obj, this.httpOptions);
  }

  promote(obj: SuperAdministratorActionModel) {
    obj = this.prepareAction(obj);
    const apiurl = this.apiUrl + obj._tenantName + '/SuperAdministrator/promote';
    return this.http.post<SuperAdministratorActionModel>(apiurl, obj, this.httpOptions);
  }

  demote(obj: SuperAdministratorActionModel) {
    obj = this.prepareAction(obj);
    const apiurl = this.apiUrl + obj._tenantName + '/SuperAdministrator/demote';
    return this.http.post<SuperAdministratorActionModel>(apiurl, obj, this.httpOptions);
  }

  private prepareAction(obj: SuperAdministratorActionModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.updatedBy = this.defaultValuesService.getUserGuidId();
    return obj;
  }
}
