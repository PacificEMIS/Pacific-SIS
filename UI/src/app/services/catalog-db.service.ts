
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { AvailableTenantViewModel } from '../models/available-tenant';


const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};
@Injectable({
  providedIn: 'root'
})
export class CatalogDbService {

  constructor(private http: HttpClient) { }
  
  validTenant(model: AvailableTenantViewModel) {
    return this.http.post<AvailableTenantViewModel>(`${environment.apiURL}api/CatalogDB/CheckIfTenantIsAvailable`, model, httpOptions);
  }
}
