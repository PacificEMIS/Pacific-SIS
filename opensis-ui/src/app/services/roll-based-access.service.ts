import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { PermissionGroupListViewModel, RolePermissionListViewModel } from '../models/roll-based-access.model';
import { BehaviorSubject } from 'rxjs';
import { CryptoService } from './Crypto.service';
import { DefaultValuesService } from '../common/default-values.service';

@Injectable({
    providedIn: 'root'
})
export class RollBasedAccessService {
    private permissionList = new BehaviorSubject([]);
    private permission;
    currentpermissionList = this.permissionList.asObservable();
    private obj = { "memberId": 0, "memberTitle": "", "memberDescription": "" }
    private member = new BehaviorSubject(this.obj);
    selectedMember = this.member.asObservable();

    apiUrl: string = environment.apiURL;
    httpOptions: { headers: any; };
    constructor(private http: HttpClient, private cryptoService: CryptoService, private defaultValuesService: DefaultValuesService) { 
        this.httpOptions = {
            headers: new HttpHeaders({
              'Cache-Control': 'no-cache',
              'Pragma': 'no-cache',
            })
          }
    }
    getAllPermissionGroup(permission: PermissionGroupListViewModel) {
        permission = this.defaultValuesService.getAllMandatoryVariable(permission);
        let apiurl = this.apiUrl + permission._tenantName + "/RoleBasedAccess/getAllPermissionGroup";
        return this.http.post<PermissionGroupListViewModel>(apiurl, permission,this.httpOptions)
    }

    getAllRolePermission(permission: RolePermissionListViewModel) {
        permission = this.defaultValuesService.getAllMandatoryVariable(permission);
        let apiurl = this.apiUrl + permission._tenantName + "/RoleBasedAccess/getAllRolePermission";
        return this.http.post<RolePermissionListViewModel>(apiurl, permission,this.httpOptions)
    }

    getPermission() {
        return this.defaultValuesService.getPermissionList();
    }

    // setPermission() {
    //     this.permission = JSON.parse(this.cryptoService.dataDecrypt(localStorage.getItem('permissions')))
    // }

    updateRolePermission(permission: PermissionGroupListViewModel) {
        permission = this.defaultValuesService.getAllMandatoryVariable(permission);
        let apiurl = this.apiUrl + permission._tenantName + "/RoleBasedAccess/updateRolePermission";
        return this.http.put<PermissionGroupListViewModel>(apiurl, permission,this.httpOptions)
    }
    send(message) {
        this.permissionList.next(message)
    }
    sendSelectedMember(member) {
        this.member.next(member)
    }

    private accessControlStatus = new BehaviorSubject<boolean>(false);
    permissionsChanged = this.accessControlStatus.asObservable();

    changeAccessControl(message: boolean) {
        this.accessControlStatus.next(message)
    }

}