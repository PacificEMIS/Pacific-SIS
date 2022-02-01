import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { DefaultValuesService } from '../common/default-values.service';
import { AddApiAccessmodel, ApiAccessmodel, GenerateApiKeyModel, GetApiKeyModel, UpdateApiKeyModel } from '../models/api-key.model';

@Injectable({
  providedIn: 'root'
})
export class ApiKeyService {
  apiUrl: string = environment.apiURL;
  httpOptions: { headers: any; };

  constructor(
    private http: HttpClient,
    private defaultValuesService: DefaultValuesService,
    ) {
      this.httpOptions = {
          headers: new HttpHeaders({
            'Cache-Control': 'no-cache',
            'Pragma': 'no-cache',
          })
        }
   }

   generateAPIKey(apiKey: GenerateApiKeyModel) {
    apiKey = this.defaultValuesService.getAllMandatoryVariable(apiKey);
    apiKey.apiKeysMaster.tenantId =  this.defaultValuesService.getTenantID();
    apiKey.apiKeysMaster.schoolId =  this.defaultValuesService.getSchoolID();
    apiKey.apiKeysMaster.createdBy =  this.defaultValuesService.getUserGuidId();

      let apiurl = this.apiUrl + apiKey._tenantName + "/ApiKey/generateAPIKey";
      return this.http.post<GenerateApiKeyModel>(apiurl, apiKey, this.httpOptions)
  }

  getAPIKey(apiKey: GetApiKeyModel) {
    apiKey = this.defaultValuesService.getAllMandatoryVariable(apiKey);
      let apiurl = this.apiUrl + apiKey._tenantName + "/ApiKey/getAPIKey";
      return this.http.post<GetApiKeyModel>(apiurl, apiKey, this.httpOptions)
  }

  deleteAPIKey(apiKey: UpdateApiKeyModel) {
    apiKey = this.defaultValuesService.getAllMandatoryVariable(apiKey);
    apiKey.apiKeysMaster.tenantId =  this.defaultValuesService.getTenantID();
    apiKey.apiKeysMaster.schoolId =  this.defaultValuesService.getSchoolID();
    apiKey.apiKeysMaster.updatedBy =  this.defaultValuesService.getUserGuidId();
      let apiurl = this.apiUrl + apiKey._tenantName + "/ApiKey/deleteAPIKey";
      return this.http.put<UpdateApiKeyModel>(apiurl, apiKey, this.httpOptions)
  }

  updateAPIKeyTitle(apiKey: UpdateApiKeyModel) {
    apiKey = this.defaultValuesService.getAllMandatoryVariable(apiKey);
    apiKey.apiKeysMaster.tenantId =  this.defaultValuesService.getTenantID();
    apiKey.apiKeysMaster.schoolId =  this.defaultValuesService.getSchoolID();
    apiKey.apiKeysMaster.updatedBy =  this.defaultValuesService.getUserGuidId();
      let apiurl = this.apiUrl + apiKey._tenantName + "/ApiKey/updateAPIKeyTitle";
      return this.http.put<UpdateApiKeyModel>(apiurl, apiKey, this.httpOptions)
  }

  refreshAPIKey(apiKey: UpdateApiKeyModel) {
    apiKey = this.defaultValuesService.getAllMandatoryVariable(apiKey);
    apiKey.apiKeysMaster.tenantId =  this.defaultValuesService.getTenantID();
    apiKey.apiKeysMaster.schoolId =  this.defaultValuesService.getSchoolID();
    apiKey.apiKeysMaster.updatedBy =  this.defaultValuesService.getUserGuidId();
      let apiurl = this.apiUrl + apiKey._tenantName + "/ApiKey/refreshAPIKey";
      return this.http.put<UpdateApiKeyModel>(apiurl, apiKey, this.httpOptions)
  }

  getAPIAccess(apiKey: ApiAccessmodel) {
    apiKey = this.defaultValuesService.getAllMandatoryVariable(apiKey);
      let apiurl = this.apiUrl + apiKey._tenantName + "/ApiKey/getAPIAccess";
      return this.http.post<AddApiAccessmodel>(apiurl, apiKey, this.httpOptions);
  }

  addAPIAccess(apiKey: AddApiAccessmodel) {
    apiKey = this.defaultValuesService.getAllMandatoryVariable(apiKey);
      let apiurl = this.apiUrl + apiKey._tenantName + "/ApiKey/addAPIAccess";
      return this.http.post<AddApiAccessmodel>(apiurl, apiKey, this.httpOptions);
  }

}