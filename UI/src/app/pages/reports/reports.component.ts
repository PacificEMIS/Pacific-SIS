import { Component, OnInit } from '@angular/core';
import icSchool from '@iconify/icons-ic/twotone-account-balance';
import icStudents from '@iconify/icons-ic/twotone-school';
import icUsers from '@iconify/icons-ic/twotone-people';
import icSchedule from '@iconify/icons-ic/twotone-date-range';
import icGrade from '@iconify/icons-ic/twotone-leaderboard';
import icAttendance from '@iconify/icons-ic/twotone-access-alarm';
import icEventAvailable from '@iconify/icons-ic/twotone-event-available';
import icAdmionPanelSettings from '@iconify/icons-ic/twotone-admin-panel-settings';
import { TranslateService } from '@ngx-translate/core';
import { PermissionCategory } from '../../models/roll-based-access.model';
import { NavigationService } from '../../../@vex/services/navigation.service';
import { PageRolesPermission } from '../../common/page-roles-permissions.service';
import { DefaultValuesService } from '../../common/default-values.service';
import { Router } from '@angular/router';

@Component({
  selector: 'vex-reports',
  templateUrl: './reports.component.html',
  styleUrls: ['./reports.component.scss']
})
export class ReportsComponent implements OnInit {

  icSchool = icSchool;
  icStudents = icStudents;
  icUsers = icUsers;
  icSchedule = icSchedule;
  icGrade = icGrade;
  icAttendance = icAttendance;
  icEventAvailable = icEventAvailable;
  icAdmionPanelSettings = icAdmionPanelSettings;
  
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
        return icSchool;
      }
      case 'Student':{
        return icStudents;
      }
      case 'Staff':{
        return icUsers;
      }
      case 'Scheduling':{
        return icSchedule;
      }
      case 'Attendance':{
        return icAttendance;
      }
      case 'Administration':{
        return icAdmionPanelSettings;
      }
      case 'Grades':{
        return icGrade;
      }
      
    }
  }

  setPageId(title) {
    
  }
 
}
