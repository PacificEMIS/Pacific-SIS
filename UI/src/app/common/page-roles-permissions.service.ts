import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { zip } from "rxjs";
import { Profiles } from "../enums/profiles.enum";
import {
  PermissionCategory,
  Permissions,
  PermittedTabs,
  RolePermissionListViewModel,
} from "../models/roll-based-access.model";
import { DefaultValuesService } from "./default-values.service";

@Injectable({
  providedIn: "root",
})
export class PageRolesPermission {
  PROFILES = Profiles;
  invalidPermission = new Permissions();
  constructor(
    private defaultValueService: DefaultValuesService,
    private router: Router
  ) {}

  getPermissionList() {
    return this.defaultValueService.getPermissionList();
  }

  checkPageRolePermission(path?: string, customPermission?: RolePermissionListViewModel, skipCategoryPath?: boolean): Permissions {
    let membershipId = +this.defaultValueService.getuserMembershipID();
    if (!membershipId) {
      return this.invalidPermission;
    }
    let pathToFind = path ? path : this.router.url;
    let permission: RolePermissionListViewModel = customPermission ? customPermission : this.getPermissionList();
    for (let item of permission.permissionList) {
      if (item.permissionGroup.path === pathToFind) {
        return this.generateRolePermission(item.permissionGroup);
      } else {
        if (item.permissionGroup.permissionCategory.length > 0) {
          for (let cat of item.permissionGroup.permissionCategory) {
            if (cat.path === pathToFind && !skipCategoryPath) {
              return this.generateRolePermission(cat);
            } else {
              if (cat.permissionSubcategory.length > 0) {
                for (let subCat of cat.permissionSubcategory) {
                  if (subCat.path === pathToFind) {
                    return this.generateRolePermission(subCat);
                  }
                }
              }
            }
          }
        }
      }
    }
    return this.invalidPermission;
  }

  getPermittedCategories(path: string):PermissionCategory[]{

    let permission: RolePermissionListViewModel = this.getPermissionList();
    let pathToFind = path;
    for (let item of permission.permissionList) {
      if(item.permissionGroup.path===pathToFind){
        item.permissionGroup.permissionCategory = item.permissionGroup.permissionCategory.filter((item)=>{
          return item.rolePermission[0].canView
        })
        return item.permissionGroup.permissionCategory
      }
    }
    return [];
  }



  getPermittedSubCategories(path: string, customPermission?: RolePermissionListViewModel):PermittedTabs[] {
    let permittedTabDetails:PermittedTabs[] = [];
    let permission: RolePermissionListViewModel = customPermission ? customPermission : this.getPermissionList();
    let pathToFind = path;
    loop1:for (let item of permission.permissionList) {
        if(item.permissionGroup.type==='link' && item.permissionGroup.path===pathToFind){
          item.permissionGroup?.permissionCategory?.map((cat)=>{
            if(cat.rolePermission[0].canView){
              permittedTabDetails.push({
                title: cat.permissionCategoryName,
                path:cat.path
              })
            }
          })
          return permittedTabDetails;
        }
        else if (item.permissionGroup.permissionCategory.length > 0) {
          for (let cat of item.permissionGroup.permissionCategory) {
            if(cat.path.includes(pathToFind) && ( this.defaultValueService.getUserMembershipType() === "Teacher" ||this.defaultValueService.getUserMembershipType() === "Homeroom Teacher" )){
              if(cat.rolePermission[0].canView){
                permittedTabDetails.push({
                  title: cat.permissionCategoryName,
                  path:cat.path
                })
              }
            }
           else if (cat.path === pathToFind) {
                if(cat.permissionSubcategory.length>0){
                  for(let subCat of cat.permissionSubcategory){
                    if(subCat.rolePermission[0].canView){
                      permittedTabDetails.push({
                        title: subCat.permissionSubcategoryName,
                        path:subCat.path
                      })
                    }
                  }
                }
                return permittedTabDetails
            }
          }
        }
    }

    return permittedTabDetails;
    
  }


  generateRolePermission(item) {
    let rolePermission: Permissions = {
      add: item.rolePermission[0].canAdd ? item.rolePermission[0].canAdd : false,
      view: item.rolePermission[0].canView ? item.rolePermission[0].canView : false,
      edit: item.rolePermission[0].canEdit ? item.rolePermission[0].canEdit : false,
      delete: item.rolePermission[0].canDelete ? item.rolePermission[0].canDelete : false,
    };
    return rolePermission;
  }
}
