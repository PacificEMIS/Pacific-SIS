import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Subject } from 'rxjs';
import { DefaultValuesService } from '../common/default-values.service';
import { ProfilesTypes } from '../enums/profiles.enum';
import { RolePermissionListViewModel } from '../models/roll-based-access.model';
import { CommonService } from './common.service';
import { RollBasedAccessService } from './roll-based-access.service';
import { SchoolService } from './school.service';

@Injectable({
  providedIn: 'root'
})
export class ImpersonateServices {
  permissionListViewModel: RolePermissionListViewModel = new RolePermissionListViewModel();
  profiles = ProfilesTypes;
  public impersonateSubjectForSelectBar = new Subject();
  public impersonateSubjectForToolBar = new Subject();
  public impersonateSubjectForsideNav = new Subject();
  constructor(
    private defaultValuesService: DefaultValuesService,
    public rollBasedAccessService: RollBasedAccessService,
    private schoolService: SchoolService,
    private router: Router,
    private commonService: CommonService) { }

  impersonateStoreData() {
    let superAdminData = {
      userGuiId: this.defaultValuesService.getUserGuidId(),
      userPhoto: this.defaultValuesService.getuserPhoto(),
      userId: this.defaultValuesService.getUserId(),
      userMembershipId: this.defaultValuesService.getuserMembershipID(),
      userMembershipType: this.defaultValuesService.getUserMembershipType(),
      userFullUserName: this.defaultValuesService.getFullUserName(),
      userName: this.defaultValuesService.getUserName(),
      userEmail: this.defaultValuesService.getEmailId(),
      userMembershipName: this.defaultValuesService.getuserMembershipName(),
      userSchoolId: this.defaultValuesService.getSchoolID()
    }
    this.setSuperAdminCredentials(superAdminData)
    this.setImpersonateButton(true)
  }

  backToSuperAdmin() {
    sessionStorage.removeItem('impersonateButton');
    let value = this.getSuperAdminCredentials();
    this.defaultValuesService.setFullUserName(value.userFullUserName);
    this.defaultValuesService.setUserMembershipType(value.userMembershipType);
    this.defaultValuesService.setEmailId(value.userEmail);
    this.defaultValuesService.setUserGuidId(value.userGuiId);
    this.defaultValuesService.setUserName(value.userName);
    this.defaultValuesService.setUserId(value.userId);
    this.defaultValuesService.setUserPhoto(value.userPhoto);
    this.defaultValuesService.setUserMembershipID(value.userMembershipId);
    this.defaultValuesService.setuserMembershipName(value.userMembershipName);
    this.defaultValuesService.setSchoolID(value.userSchoolId)
    sessionStorage.removeItem('adminCredentials');
  }

  setImpersonateButton(impersonateButton: boolean) {
    sessionStorage.setItem("impersonateButton", JSON.stringify(impersonateButton));
  }
  getImpersonateButton() {
    return JSON.parse(sessionStorage.getItem("impersonateButton"));
  }

  setSuperAdminCredentials(adminCredentials: object) {
    sessionStorage.setItem("adminCredentials", JSON.stringify(adminCredentials));
  }
  getSuperAdminCredentials() {
    return JSON.parse(sessionStorage.getItem("adminCredentials"));
  }

  callRolePermissions(impersonate?:boolean){
    let rolePermissionListView: RolePermissionListViewModel = new RolePermissionListViewModel();
    rolePermissionListView.permissionList = [];
        this.rollBasedAccessService.getAllRolePermission(rolePermissionListView).subscribe((res: RolePermissionListViewModel) => {
          if(res){
            if(!res._failure){
              this.defaultValuesService.setPermissionList(res);
            this.schoolService.changeSchoolListStatus({schoolLoaded:false,schoolChanged:false,dataFromUserLogin:true,academicYearChanged:false,academicYearLoaded:false});
            if(this.defaultValuesService.getUserMembershipType() === this.profiles.HomeroomTeacher || this.defaultValuesService.getUserMembershipType() === this.profiles.Teacher){
              this.router.navigateByUrl("/school/teacher/dashboards").then(()=>{
                this.commonService.setUserActivity(true);
              });
            }
            else{
              this.router.navigateByUrl("/school/dashboards").then(()=>{
                this.commonService.setUserActivity(true);
              });
            }
            }
          }else{
              this.commonService.checkTokenValidOrNot(res._message);
          }
          if (impersonate) {
            this.impersonateSubjectForToolBar.next(true);
            this.impersonateSubjectForSelectBar.next(true);
            this.impersonateSubjectForsideNav.next(true);
          }

        });
  }
}
