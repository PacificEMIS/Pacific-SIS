import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { CheckSchoolInternalIdViewModel, CopySchoolModel, SchoolAddViewModel } from '../models/school-master.model';
import { AllSchoolListModel, GetAllSchoolModel, OnlySchoolListModel } from '../models/get-all-school.model';
import { BehaviorSubject, Subject } from 'rxjs';
import { DataAvailablity } from '../models/user.model';
import { DefaultValuesService } from '../common/default-values.service';
import { SchoolCreate } from '../enums/school-create.enum';

@Injectable({
  providedIn: 'root'
})
export class SchoolService {
  schoolCreate = SchoolCreate;
  private schoolId;
  private navigationState=false;
  private schoolDetails;
  private categoryTitle = new BehaviorSubject(null);
  selectedCategoryTitle = this.categoryTitle.asObservable();
  private messageSource = new BehaviorSubject(false);
  currentMessage = this.messageSource.asObservable();
  apiUrl: string = environment.apiURL;

  private schoolCreateMode = new BehaviorSubject(this.schoolCreate.ADD);
  schoolCreatedMode = this.schoolCreateMode.asObservable();

  private schoolDetailsForViewAndEdit = new BehaviorSubject(null);
  schoolDetailsForViewedAndEdited = this.schoolDetailsForViewAndEdit.asObservable();
  
  private categoryDetails = new BehaviorSubject(0);
  categoryDetailsSelected = this.categoryDetails.asObservable();

  private currentSchoolName = new Subject();
  updatedSchoolName = this.currentSchoolName.asObservable();
  httpOptions: { headers: HttpHeaders; };

  constructor(private http: HttpClient,
    private defaultValuesService: DefaultValuesService) {
      this.httpOptions = {
        headers: new HttpHeaders({
          'Cache-Control': 'no-cache',
          'Pragma': 'no-cache',
        })
      }
  }

  GetAllSchoolList(obj: GetAllSchoolModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName + "/School/getAllSchoolList";
    return this.http.post<AllSchoolListModel>(apiurl, obj, { headers: this.httpOptions.headers});
  }

  GetAllSchools(obj: OnlySchoolListModel) {
   obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName + "/School/getAllSchools";
    return this.http.post<AllSchoolListModel>(apiurl, obj, { headers: this.httpOptions.headers});
  }

  ViewSchool(obj: SchoolAddViewModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.schoolMaster.schoolId = this.defaultValuesService.getSchoolID();
    obj.schoolMaster.schoolDetail[0].schoolId = this.defaultValuesService.getSchoolID();
    obj.schoolMaster.tenantId = this.defaultValuesService.getTenantID();
    let apiurl = this.apiUrl + obj._tenantName + "/School/viewSchool";
    return this.http.post<SchoolAddViewModel>(apiurl, obj, { headers: this.httpOptions.headers});
  }

  AddSchool(obj: SchoolAddViewModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.EmailAddress = this.defaultValuesService.getEmailId();
    obj.schoolMaster.tenantId = this.defaultValuesService.getTenantID();
    obj.schoolMaster.createdBy = this.defaultValuesService.getUserGuidId();
    obj.schoolMaster.schoolDetail[0].schoolLogo = this.schoolImage;
    obj.schoolMaster.schoolDetail[0].createdBy = this.defaultValuesService.getUserGuidId();
    let apiurl = this.apiUrl + obj._tenantName + "/School/addSchool";
    return this.http.post<SchoolAddViewModel>(apiurl, obj, { headers: this.httpOptions.headers})
  }
  UpdateSchool(obj: SchoolAddViewModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.EmailAddress = this.defaultValuesService.getEmailId();
    obj.schoolMaster.tenantId = this.defaultValuesService.getTenantID();
    obj.schoolMaster.updatedBy = this.defaultValuesService.getUserGuidId();
    obj.schoolMaster.createdBy = null;
    obj.schoolMaster.schoolDetail[0].schoolLogo = this.schoolImage;
    obj.schoolMaster.schoolDetail[0].createdBy = null;
    obj.schoolMaster.schoolDetail[0].updatedBy = this.defaultValuesService.getUserGuidId();
    let apiurl = this.apiUrl + obj._tenantName + "/School/updateSchool";
    return this.http.put<SchoolAddViewModel>(apiurl, obj, { headers: this.httpOptions.headers})
  }
  copySchool(obj: CopySchoolModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.fromSchoolId = this.defaultValuesService.getSchoolID();
    obj.schoolMaster.tenantId = this.defaultValuesService.getTenantID();
    let apiurl = this.apiUrl + obj._tenantName + "/School/copySchool";
    return this.http.post<CopySchoolModel>(apiurl, obj, { headers: this.httpOptions.headers})
  }
  checkSchoolInternalId(obj: CheckSchoolInternalIdViewModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    let apiurl = this.apiUrl + obj._tenantName + "/School/checkSchoolInternalId";
    return this.http.post<CheckSchoolInternalIdViewModel>(apiurl, obj, { headers: this.httpOptions.headers})
  }

  private schoolImage;
  setSchoolImage(imageInBase64) {
    this.schoolImage = imageInBase64;
  }

  public cloneSchoolImage
  setSchoolCloneImage(image){
    this.cloneSchoolImage = image;
  }
  getSchoolCloneImage(){
    return this.cloneSchoolImage;
  }

  setnavigationState(state: boolean) {
    this.navigationState = state
  }
  getnavigationState() {
    return this.navigationState;
  }

  setSchoolId(id: number) {
    this.schoolId = id
  }
  getSchoolId() {
    return this.schoolId;
  }

  private schoolMultiselectValue: any;
  setSchoolMultiselectValue(value: any) {
    this.schoolMultiselectValue = value;
  }
  getSchoolMultiselectValue() {
    return this.schoolMultiselectValue;
  }

  setSchoolDetails(data) {
    this.schoolDetails = data;
  }
  getSchoolDetails() {
    return this.schoolDetails;
  }

  changeMessage(message: boolean) {
    this.messageSource.next(message)
  }

  // Change Category in School
  private category = new Subject;
  categoryToSend = this.category.asObservable();

  changeCategory(category: number) {
    this.category.next(category);
  }

  // Update Mode in School
  private pageMode = new Subject;
  modeToUpdate = this.pageMode.asObservable();

  changePageMode(mode: number) {
    this.pageMode.next(mode);
  }

  // to Update school details in General Info in first view mode.
  private schoolDetailsForGeneral = new Subject();
  getSchoolDetailsForGeneral = this.schoolDetailsForGeneral.asObservable();

  sendDetails(schoolDetailsForGeneral) {
    this.schoolDetailsForGeneral.next(schoolDetailsForGeneral);
  }

  addUpdateSchoolLogo(obj: SchoolAddViewModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.schoolMaster.schoolId = this.defaultValuesService.getSchoolID();
    obj.schoolMaster.tenantId = this.defaultValuesService.getTenantID();
    obj.schoolMaster.updatedBy = this.defaultValuesService.getUserGuidId();
    // obj.schoolMaster.schoolDetail[0].id = this.getSchoolId();
    // obj.schoolMaster.schoolDetail[0].schoolLogo = this.schoolImage;
    let apiurl = this.apiUrl + obj._tenantName + "/School/addUpdateSchoolLogo";
    return this.http.put<SchoolAddViewModel>(apiurl, obj,  { headers: this.httpOptions.headers})
  }

  private changeStatusTo = new BehaviorSubject<DataAvailablity>({schoolChanged:false,schoolLoaded:false,dataFromUserLogin:false,academicYearChanged:false,academicYearLoaded:false});
  schoolListCalled = this.changeStatusTo.asObservable();  

  changeSchoolListStatus(message: DataAvailablity) {
    this.changeStatusTo.next(message)
  }

  setSchoolCreateMode(data) {
    this.schoolCreateMode.next(data);
  }

  setSchoolDetailsForViewAndEdit(data) {
    this.schoolDetailsForViewAndEdit.next(data);
  }

  setCategoryDetails(data) {
    this.categoryDetails.next(data);
  }

  setCategoryTitle(title:string){
    this.categoryTitle.next(title);
  }

  updateSchoolName(schoolName: string){
    this.currentSchoolName.next(schoolName);
  }

  updateLastUsedSchoolId(obj: SchoolAddViewModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.EmailAddress = this.defaultValuesService.getEmailId();
    obj.updatedBy = this.defaultValuesService.getEmailId();
    let apiurl = this.apiUrl + obj._tenantName + "/School/updateLastUsedSchoolId";
    return this.http.put<SchoolAddViewModel>(apiurl, obj,  { headers: this.httpOptions.headers})
  }

}
