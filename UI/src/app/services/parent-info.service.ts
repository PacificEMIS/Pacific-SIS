import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { ViewParentInfoModel, GetAllParentModel, AddParentInfoModel, ParentInfoList, GetAllParentInfoModel, RemoveAssociateParent, GetAllParentResponseModel, ParentAdvanceSearchModel } from '../models/parent-info.model';
import { CryptoService } from './Crypto.service';
import { BehaviorSubject, Subject } from 'rxjs';
import { DefaultValuesService } from '../common/default-values.service';
import { SchoolCreate } from '../enums/school-create.enum';
@Injectable({
  providedIn: 'root'
})
export class ParentInfoService {
  parentCreate = SchoolCreate;
  apiUrl: string = environment.apiURL;
  private parentId;
  private parentDetails;
  private advanceSearchParams=null;
  userName = this.defaultValuesService.getUserName();

  private parentCreateMode = new BehaviorSubject(this.parentCreate.VIEW);
  parentCreatedMode = this.parentCreateMode.asObservable();

  private parentDetailsForViewAndEdit = new BehaviorSubject(null);
  parentDetailsForViewedAndEdited = this.parentDetailsForViewAndEdit.asObservable();
  httpOptions: { headers: any; };

  constructor(
    private http: HttpClient,
    private cryptoService: CryptoService,
    private defaultValuesService: DefaultValuesService
    ) { 
      this.httpOptions = {
        headers: new HttpHeaders({
          'Cache-Control': 'no-cache',
          'Pragma': 'no-cache',
        })
      }
    }


  setParentId(id: number) {
    sessionStorage.setItem('ParentID', JSON.stringify(id));
  }
  getParentId() {
    return JSON.parse(sessionStorage.getItem('ParentID'));
  }
  setParentDetails(data) {
    this.parentDetails = data
  }
  getParentDetails() {
    return this.parentDetails;
  }

  // Update Mode in Parent
  private pageMode = new Subject;
  modeToUpdate = this.pageMode.asObservable();

  changePageMode(mode: number) {
    this.pageMode.next(mode);
  }

  ViewParentListForStudent(parentInfo: ViewParentInfoModel) {
    parentInfo = this.defaultValuesService.getAllMandatoryVariable(parentInfo);
    let apiurl = this.apiUrl + parentInfo._tenantName + "/ParentInfo/ViewParentListForStudent";
    return this.http.post<ViewParentInfoModel>(apiurl, parentInfo,this.httpOptions);
  }
  viewParentInfo(parentInfo: AddParentInfoModel) {
    parentInfo = this.defaultValuesService.getAllMandatoryVariable(parentInfo);
    parentInfo.parentInfo.schoolId = this.defaultValuesService.getSchoolID();
    parentInfo.parentInfo.tenantId = this.defaultValuesService.getTenantID();
    let apiurl = this.apiUrl + parentInfo._tenantName + "/ParentInfo/viewParentInfo";
    return this.http.post<AddParentInfoModel>(apiurl, parentInfo,this.httpOptions);
  }

  updateParentInfo(parentInfo: AddParentInfoModel) {
    parentInfo = this.defaultValuesService.getAllMandatoryVariable(parentInfo);
    parentInfo.passwordHash = this.cryptoService.encrypt(parentInfo.passwordHash);
    parentInfo.parentInfo.schoolId = this.defaultValuesService.getSchoolID();
    parentInfo.parentInfo.tenantId = this.defaultValuesService.getTenantID();
    parentInfo.parentInfo.updatedBy = this.defaultValuesService.getUserGuidId();
    parentInfo.parentInfo.parentPhoto = this.parentImage;
    parentInfo.parentInfo.parentAddress[0].parentId= parentInfo.parentInfo.parentId;
    parentInfo.parentInfo.parentAddress[0].tenantId = this.defaultValuesService.getTenantID();
    parentInfo.parentInfo.parentAddress[0].schoolId = this.defaultValuesService.getSchoolID();
    let apiurl = this.apiUrl + parentInfo._tenantName + "/ParentInfo/updateParentInfo";
    return this.http.put<AddParentInfoModel>(apiurl, parentInfo,this.httpOptions);
  }
  getAllParentInfo(Obj: GetAllParentModel) {
    Obj = this.defaultValuesService.getAllMandatoryVariable(Obj);
    let apiurl = this.apiUrl + Obj._tenantName + "/ParentInfo/getAllParentInfo";
    return this.http.post<GetAllParentResponseModel>(apiurl, Obj,this.httpOptions);
  }
  addParentForStudent(obj: AddParentInfoModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.parentInfo.schoolId = this.defaultValuesService.getSchoolID();
    obj.parentInfo.tenantId = this.defaultValuesService.getTenantID();
    obj.parentInfo.createdBy = this.defaultValuesService.getUserGuidId();
    obj.parentAssociationship.tenantId= this.defaultValuesService.getTenantID();
    obj.passwordHash = this.cryptoService.encrypt(obj.passwordHash);
    obj.parentInfo.parentPhoto = this.parentImage;
    let apiurl = this.apiUrl + obj._tenantName + "/ParentInfo/addParentForStudent";
    return this.http.post<AddParentInfoModel>(apiurl, obj,this.httpOptions);
  }
  deleteParentInfo(obj: AddParentInfoModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.parentInfo.schoolId = this.defaultValuesService.getSchoolID();
    obj.parentInfo.tenantId = this.defaultValuesService.getTenantID();
    let apiurl = this.apiUrl + obj._tenantName + "/ParentInfo/deleteParentInfo";
    return this.http.post<AddParentInfoModel>(apiurl, obj,this.httpOptions);
  }
  searchParentInfoForStudent(obj: ParentInfoList) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName + "/ParentInfo/searchParentInfoForStudent";
    return this.http.post<ParentInfoList>(apiurl, obj,this.httpOptions);
  }

  viewParentListForStudent(obj: GetAllParentInfoModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName + "/ParentInfo/viewParentListForStudent";
    return this.http.post<GetAllParentInfoModel>(apiurl, obj,this.httpOptions);
  }

  removeAssociatedParent(obj: RemoveAssociateParent) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.parentInfo.schoolId = this.defaultValuesService.getSchoolID();
    obj.parentInfo.parentAddress[0].tenantId = this.defaultValuesService.getTenantID();
    obj.parentInfo.tenantId = this.defaultValuesService.getTenantID();
    let apiurl = this.apiUrl + obj._tenantName + "/ParentInfo/removeAssociatedParent";
    return this.http.post<RemoveAssociateParent>(apiurl, obj,this.httpOptions);
  }

  addUpdateParentPhoto(obj: AddParentInfoModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.parentInfo.schoolId = this.defaultValuesService.getSchoolID();
    obj.parentInfo.tenantId = this.defaultValuesService.getTenantID();
    obj.parentInfo.updatedBy = this.defaultValuesService.getUserGuidId();
    obj.parentInfo.parentId = this.getParentId();
    obj.parentInfo.parentPhoto = this.parentImage;
    let apiurl = this.apiUrl + obj._tenantName + "/ParentInfo/addUpdateParentPhoto";
    return this.http.put<AddParentInfoModel>(apiurl, obj,this.httpOptions);
  }


  // to Update staff details in General for first time.
  private parentDetailsForGeneralInfo = new Subject;
  getParentDetailsForGeneral = this.parentDetailsForGeneralInfo.asObservable();
  sendDetails(parentDetailsForGeneralInfo) {
    this.parentDetailsForGeneralInfo.next(parentDetailsForGeneralInfo);
  }

  private parentImage;
  setParentImage(imageInBase64) {
    this.parentImage = imageInBase64;
  }

  setParentCreateMode(data) {
    this.parentCreateMode.next(data);
  }

  setParentDetailsForViewAndEdit(data) {
    this.parentDetailsForViewAndEdit.next(data);
  }

  setAdvanceSearchParams(params){
    this.advanceSearchParams=params
  }

  getAdvanceSearchParams(){
    if(this.advanceSearchParams){
      let parentSearchModel: ParentAdvanceSearchModel = new ParentAdvanceSearchModel();
      for(let param of this.advanceSearchParams){
        parentSearchModel[param.columnName] = param.filterValue;
      }
    return parentSearchModel;
    }else{
      return null
    }
  }
}
