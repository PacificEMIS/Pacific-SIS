import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Subject } from 'rxjs';
import { CryptoService } from './Crypto.service';
import {
  CheckStaffInternalIdViewModel,
  GetAllStaffModel, StaffAddModel,
  StaffCertificateModel,
  StaffCertificateListModel,
  StaffSchoolInfoModel,
  StaffImportModel, 
  ScheduledCourseSectionsForStaffModel} from '../models/staff.model';
import { DefaultValuesService } from '../common/default-values.service';
import { SchoolCreate } from '../enums/school-create.enum';
@Injectable({
  providedIn: 'root'
})
export class StaffService {
  staffCreate = SchoolCreate;


  apiUrl: string = environment.apiURL;
  private currentYear = new BehaviorSubject(false);
  currentY = this.currentYear.asObservable();
  userName = sessionStorage.getItem('user');

  private staffImage;
  private isFirstView:boolean= true;

  private staffId: number;

  private staffMultiselectValue: any;


  private category = new Subject();
  categoryToSend = this.category.asObservable();

  private staffDetails;

     // Update Mode in Staff
 public pageMode = new Subject();
 modeToUpdate = this.pageMode.asObservable();

  // to Update staff details in General for first time.
  private staffDetailsForGeneralInfo = new Subject();
  getStaffDetailsForGeneral = this.staffDetailsForGeneralInfo.asObservable();

  private staffCreateMode = new BehaviorSubject(this.staffCreate.ADD);
  staffCreatedMode = this.staffCreateMode.asObservable();

  private staffDetailsForViewAndEdit = new BehaviorSubject(null);
  staffDetailsForViewedAndEdited = this.staffDetailsForViewAndEdit.asObservable();
  
  private categoryId = new BehaviorSubject(null);
  categoryIdSelected = this.categoryId.asObservable();

  private dataAfterSavingGeneralInfo = new Subject();
  dataAfterSavedGeneralInfo = this.dataAfterSavingGeneralInfo.asObservable();

  private checkUpdatedProfileName = new BehaviorSubject(null);
  checkedUpdatedProfileName = this.checkUpdatedProfileName.asObservable();
  
  private categoryTitle = new BehaviorSubject(null);
  selectedCategoryTitle = this.categoryTitle.asObservable();

  // for cancel after staff photo added
  public cloneStaffImage;
  httpOptions: { headers: any; };

  constructor(private http: HttpClient,
    private defaultValuesService: DefaultValuesService,
    private cryptoService: CryptoService) {
      this.httpOptions = {
        headers: new HttpHeaders({
          'Cache-Control': 'no-cache',
          'Pragma': 'no-cache',
        })
      }
    }

  setStaffImage(imageInBase64) {
    this.staffImage = imageInBase64;
  }
  setStaffId(id: number) {
    this.staffId = id;
  }
  getStaffId() {
    return this.staffId;
  }
  setStaffMultiselectValue(value: any) {
    this.staffMultiselectValue = value;
  }
  getStaffMultiselectValue() {
    return this.staffMultiselectValue;
  }
  changeCategory(category: number) {
    this.category.next(category);
  }
  setStaffDetails(data) {
    this.staffDetails = data;
  }
  getStaffDetails() {
    return this.staffDetails;
  }
  checkStaffInternalId(obj: CheckStaffInternalIdViewModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    const apiurl = this.apiUrl + obj._tenantName + '/Staff/checkStaffInternalId';
    return this.http.post<CheckStaffInternalIdViewModel>(apiurl, obj,this.httpOptions);
  }

 changePageMode(mode: number){
     this.pageMode.next(mode);
 }
  sendDetails(staffDetailsForGeneralInfo) {
    this.staffDetailsForGeneralInfo.next(staffDetailsForGeneralInfo);
  }
  setStaffCloneImage(image){
    this.cloneStaffImage = image;
  }
  getStaffCloneImage(){
    return this.cloneStaffImage;
  }


  addStaff(obj: StaffAddModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.passwordHash = this.cryptoService.encrypt(obj.passwordHash);
    obj.staffMaster.staffPhoto = this.staffImage;
    obj.staffMaster.schoolId = this.defaultValuesService.getSchoolID();
    obj.staffMaster.tenantId = this.defaultValuesService.getTenantID();
    obj.staffMaster.createdBy = this.defaultValuesService.getEmailId();
    const apiurl = this.apiUrl + obj._tenantName + '/Staff/addStaff';
    return this.http.post<StaffAddModel>(apiurl, obj,this.httpOptions);
  }

  updateStaff(obj: StaffAddModel) {
    obj.passwordHash = this.cryptoService.encrypt(obj.passwordHash);
    obj.staffMaster.staffPhoto = this.staffImage;
    obj.staffMaster.schoolId = this.defaultValuesService.getSchoolID();
    obj.staffMaster.tenantId = this.defaultValuesService.getTenantID();
    obj.staffMaster.updatedBy = this.defaultValuesService.getEmailId();
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    const apiurl = this.apiUrl + obj._tenantName + '/Staff/updateStaff';
    return this.http.put<StaffAddModel>(apiurl, obj,this.httpOptions);
  }

  viewStaff(obj: StaffAddModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.staffMaster.schoolId = this.defaultValuesService.getSchoolID();
    obj.staffMaster.tenantId = this.defaultValuesService.getTenantID();
    const apiurl = this.apiUrl + obj._tenantName + '/Staff/viewStaff';
    return this.http.post<StaffAddModel>(apiurl, obj,this.httpOptions);
  }

  getAllStaffList(obj: GetAllStaffModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    const apiurl = this.apiUrl + obj._tenantName + '/Staff/getAllStaffList';
    return this.http.post<GetAllStaffModel>(apiurl, obj,this.httpOptions);
  }

  // Staff Certificate Services
  getAllStaffCertificateInfo(obj: StaffCertificateListModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    const apiurl = this.apiUrl + obj._tenantName + '/Staff/getAllStaffCertificateInfo';
    return this.http.post<StaffCertificateListModel>(apiurl, obj,this.httpOptions);
  }

  addStaffCertificateInfo(obj: StaffCertificateModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.staffCertificateInfo.schoolId = this.defaultValuesService.getSchoolID();
    obj.staffCertificateInfo.tenantId = this.defaultValuesService.getTenantID();
    obj.staffCertificateInfo.createdBy = this.defaultValuesService.getEmailId();
    const apiurl = this.apiUrl + obj._tenantName + '/Staff/addStaffCertificateInfo';
    return this.http.post<StaffCertificateModel>(apiurl, obj,this.httpOptions);
  }
  deleteStaffCertificateInfo(obj: StaffCertificateModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.staffCertificateInfo.schoolId = this.defaultValuesService.getSchoolID();
    obj.staffCertificateInfo.tenantId = this.defaultValuesService.getTenantID();
    const apiurl = this.apiUrl + obj._tenantName + '/Staff/deleteStaffCertificateInfo';
    return this.http.post<StaffCertificateModel>(apiurl, obj,this.httpOptions);
  }
  updateStaffCertificateInfo(obj: StaffCertificateModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.staffCertificateInfo.schoolId = this.defaultValuesService.getSchoolID();
    obj.staffCertificateInfo.tenantId = this.defaultValuesService.getTenantID();
    obj.staffCertificateInfo.updatedBy = this.defaultValuesService.getEmailId();
    const apiurl = this.apiUrl + obj._tenantName + '/Staff/updateStaffCertificateInfo';
    return this.http.put<StaffCertificateModel>(apiurl, obj,this.httpOptions);
  }

  addStaffSchoolInfo(obj: StaffSchoolInfoModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    const apiurl = this.apiUrl + obj._tenantName + '/Staff/addStaffSchoolInfo';
    return this.http.post<StaffSchoolInfoModel>(apiurl, obj,this.httpOptions);
  }

  viewStaffSchoolInfo(obj: StaffSchoolInfoModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    const apiurl = this.apiUrl + obj._tenantName + '/Staff/viewStaffSchoolInfo';
    return this.http.post<StaffSchoolInfoModel>(apiurl, obj,this.httpOptions);
  }

  updateStaffSchoolInfo(obj: StaffSchoolInfoModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    const apiurl = this.apiUrl + obj._tenantName + '/Staff/updateStaffSchoolInfo';
    return this.http.put<StaffSchoolInfoModel>(apiurl, obj,this.httpOptions);
  }

  addUpdateStaffPhoto(obj: StaffAddModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.staffMaster.staffId = this.getStaffId();
    obj.staffMaster.staffPhoto = this.staffImage;
    obj.staffMaster.tenantId = this.defaultValuesService.getTenantID();
    obj.staffMaster.updatedBy = this.defaultValuesService.getEmailId();
    const apiurl = this.apiUrl + obj._tenantName + '/Staff/addUpdateStaffPhoto';
    return this.http.put<StaffAddModel>(apiurl, obj,this.httpOptions);
  }

  addStaffList(obj: StaffImportModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.createdBy = this.defaultValuesService.getEmailId();
    const apiurl = this.apiUrl + obj._tenantName + '/Staff/addStaffList';
    return this.http.post<StaffImportModel>(apiurl, obj,this.httpOptions);
  }

  setStaffFirstView(status){
    this.isFirstView=status;
  }
  getStaffFirstView(){
    return this.isFirstView;
  }

  setStaffCreateMode(data) {
    this.staffCreateMode.next(data);
  }

  setStaffDetailsForViewAndEdit(data) {
    this.staffDetailsForViewAndEdit.next(data);
  }

  setCategoryId(data) {
    this.categoryId.next(data);
  }

  setDataAfterSavingGeneralInfo(data) {
    this.dataAfterSavingGeneralInfo.next(data);
  }

  setCheckUpdatedProfileName(data) {
    this.checkUpdatedProfileName.next(data);
  }

  setCategoryTitle(title: string){
    this.categoryTitle.next(title);
  }

  getScheduledCourseSectionsForStaff(obj: ScheduledCourseSectionsForStaffModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    const apiurl = this.apiUrl + obj._tenantName + '/Staff/getScheduledCourseSectionsForStaff';
    return this.http.post<ScheduledCourseSectionsForStaffModel>(apiurl, obj,this.httpOptions);
  }

}
