import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import {CustomFieldAddView, CustomFieldDragDropModel, CustomFieldListViewModel} from '../models/custom-field.model';
import {FieldsCategoryAddView, FieldsCategoryListView} from '../models/fields-category.model'
import { BlockAddViewModel, BlockListViewModel, BlockPeriodAddViewModel, BlockPeriodForHalfDayFullDayModel, BlockPeriodSortOrderViewModel } from '../models/school-period.model';
import { DefaultValuesService } from '../common/default-values.service';
@Injectable({
  providedIn: 'root'
})
export class SchoolPeriodService {
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

  deleteBlockPeriod(obj: BlockPeriodAddViewModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.blockPeriod.tenantId = this.defaultValuesService.getTenantID();
    obj.blockPeriod.schoolId = this.defaultValuesService.getSchoolID();
    let apiurl = this.apiUrl + obj._tenantName + "/Period/deleteBlockPeriod";
    return this.http.post<BlockPeriodAddViewModel>(apiurl, obj,this.httpOptions);
  }
  updateBlockPeriod(obj: BlockPeriodAddViewModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.blockPeriod.schoolId = this.defaultValuesService.getSchoolID();
    obj.blockPeriod.updatedBy = this.defaultValuesService.getUserGuidId();
    obj.blockPeriod.tenantId = this.defaultValuesService.getTenantID();
    let apiurl = this.apiUrl + obj._tenantName + "/Period/updateBlockPeriod";
    return this.http.put<BlockPeriodAddViewModel>(apiurl, obj,this.httpOptions)
  }
  addBlockPeriod(obj: BlockPeriodAddViewModel) {
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.blockPeriod.schoolId = this.defaultValuesService.getSchoolID();
    obj.blockPeriod.tenantId = this.defaultValuesService.getTenantID();
    obj.blockPeriod.createdBy = this.defaultValuesService.getUserGuidId();
    let apiurl = this.apiUrl + obj._tenantName + '/Period/addBlockPeriod';
    return this.http.post<BlockPeriodAddViewModel>(apiurl, obj,this.httpOptions);
  }

  addBlock(obj: BlockAddViewModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.block.tenantId = this.defaultValuesService.getTenantID();
    obj.block.schoolId = this.defaultValuesService.getSchoolID();
    obj.block.createdBy = this.defaultValuesService.getUserGuidId();
    let apiurl = this.apiUrl + obj._tenantName + '/Period/addBlock';
    return this.http.post<BlockAddViewModel>(apiurl, obj,this.httpOptions);
  }
  updateBlock(obj: BlockAddViewModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.block.tenantId = this.defaultValuesService.getTenantID();
    obj.block.schoolId = this.defaultValuesService.getSchoolID();
    obj.block.updatedBy = this.defaultValuesService.getUserGuidId();
    let apiurl = this.apiUrl + obj._tenantName + '/Period/updateBlock' ;
    return this.http.put<BlockAddViewModel>(apiurl, obj,this.httpOptions);
  }
  deleteBlock(obj: BlockAddViewModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.block.tenantId = this.defaultValuesService.getTenantID();
    obj.block.schoolId = this.defaultValuesService.getSchoolID();
    let apiurl = this.apiUrl + obj._tenantName + '/Period/deleteBlock' ;
    return this.http.post<BlockAddViewModel>(apiurl, obj,this.httpOptions);
  }

  updateFullDayHalfDayMinutes(obj: BlockPeriodForHalfDayFullDayModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.block.tenantId = this.defaultValuesService.getTenantID();
    obj.block.schoolId = this.defaultValuesService.getSchoolID();
    obj.block.updatedBy = this.defaultValuesService.getUserGuidId();
    let apiurl = this.apiUrl + obj._tenantName + '/Period/updateHalfDayFullDayMinutesForBlock';
    return this.http.put<BlockPeriodForHalfDayFullDayModel>(apiurl, obj,this.httpOptions);
  }
  getAllBlockList(obj: BlockListViewModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.academicYear = this.defaultValuesService.getAcademicYear()
    let apiurl = this.apiUrl + obj._tenantName + '/Period/getAllBlockList' ;
    return this.http.post<BlockListViewModel>(apiurl,obj);
  }
  updateBlockPeriodSortOrder(obj: BlockPeriodSortOrderViewModel){
    obj = this.defaultValuesService.getAllMandatoryVariable(obj);
    obj.updatedBy = this.defaultValuesService.getUserGuidId();
    let apiurl = this.apiUrl + obj._tenantName + '/Period/updateBlockPeriodSortOrder' ;
    return this.http.put<BlockPeriodSortOrderViewModel>(apiurl, obj,this.httpOptions);
  }

  private blockPeriodList: any;
  setBlockPeriodList(value: any) {
    this.blockPeriodList = value;
  }
  getBlockPeriodList() {
    return this.blockPeriodList;
  }

}
