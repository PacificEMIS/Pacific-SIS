import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { PermissionCategory } from '../../models/roll-based-access.model';
import { NavigationService } from '@vex/services/navigation.service';
import { PageRolesPermission } from '../../common/page-roles-permissions.service';
import { DefaultValuesService } from '../../common/default-values.service';
import { Router } from '@angular/router';

@Component({
    selector: 'vex-reports',
    templateUrl: './reports.component.html',
    styleUrls: ['./reports.component.scss'],
    standalone: false
})
export class ReportsComponent implements OnInit {

  icSchool = 'account_balance';
  icStudents = 'school';
  icUsers = 'people';
  icSchedule = 'date_range';
  icGrade = 'leaderboard';
  icAttendance = 'access_alarm';
  icEventAvailable = 'event_available';
  icAdmionPanelSettings = 'admin_panel_settings';
  
  reportSubmenu: PermissionCategory[]=[];
  constructor(public translateService: TranslateService,
    private navigationService: NavigationService,
    private pageRolePermission: PageRolesPermission,
    private defaultValuesService: DefaultValuesService,
    private router: Router,) { 
      this.navigationService.menuItems.subscribe((res)=>{
        if(res){
           this.renderReportsMenu();
        }
      })
  }

  ngOnInit(): void {
  }

  renderReportsMenu(){
    this.reportSubmenu = this.pageRolePermission.getPermittedCategories('/school/reports');
    for(let item of this.reportSubmenu){
      item.icon=this.pickIcon(item.permissionCategoryName)
    }
    for(let item of this.reportSubmenu[0]?.permissionSubcategory){
      if(item.rolePermission[0].canView){
        this.setPageId(item.title);
        break;
      }
    }
   
  }

  pickIcon(categoryName) {
    switch (categoryName) {
      case 'School':{
        return 'account_balance';
      }
      case 'Student':{
        return 'school';
      }
      case 'Staff':{
        return 'people';
      }
      case 'Scheduling':{
        return 'date_range';
      }
      case 'Attendance':{
        return 'access_alarm';
      }
      case 'Administration':{
        return 'admin_panel_settings';
      }
      case 'Grades':{
        return 'leaderboard';
      }
      
    }
  }

  setPageId(title) {
    
  }
 
}
