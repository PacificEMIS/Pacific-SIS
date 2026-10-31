/***********************************************************************************
openSIS is a free student information system for public and non-public
schools from Open Solutions for Education, Inc.Website: www.os4ed.com.

Visit the openSIS product website at https://opensis.com to learn more.
If you have question regarding this software or the license, please contact
via the website.

The software is released under the terms of the GNU Affero General Public License as
published by the Free Software Foundation, version 3 of the License.
See https://www.gnu.org/licenses/agpl-3.0.en.html.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

Copyright (c) Open Solutions for Education, Inc.

All rights reserved.
***********************************************************************************/

import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { NavigationService } from '@vex/services/navigation.service';
import { CryptoService } from 'src/app/services/Crypto.service';
import { PermissionCategory, PermissionGroup, RolePermissionListViewModel } from '../../models/roll-based-access.model';
import { Router } from '@angular/router';
import { RollBasedAccessService } from '../../services/roll-based-access.service';
import { DefaultValuesService } from '../../common/default-values.service';
import { PageRolesPermission } from '../../common/page-roles-permissions.service';

@Component({
    selector: 'vex-settings',
    templateUrl: './settings.component.html',
    styleUrls: ['./settings.component.scss'],
    standalone: false
})
export class SettingsComponent implements OnInit {

  icSchool = 'account_balance';
  icStudents = 'school';
  icUsers = 'people';
  icSchedule = 'date_range';
  icGrade = 'leaderboard';
  icAdministration = 'admin_panel_settings';
  icAttendance = 'access_alarm';
  icListOfValues = 'list_alt';

  settingSubmenu: PermissionCategory[]=[];
  constructor(public translateService: TranslateService,
    private navigationService: NavigationService,
    private pageRolePermission: PageRolesPermission,
    private defaultValuesService: DefaultValuesService,
    private roleBaseAccessService:RollBasedAccessService,
    private router: Router,) {
    this.navigationService.menuItems.subscribe((res)=>{
      if(res){
         this.renderSettingsMenu();
      }
    })
  }

  ngOnInit(): void {
  //  this.renderSettingsMenu();
  }

  renderSettingsMenu(){
    this.settingSubmenu = this.pageRolePermission.getPermittedCategories('/school/settings');
    for(let item of this.settingSubmenu){
      item.icon=this.pickIcon(item.permissionCategoryName)
    }
    for(let item of this.settingSubmenu[0]?.permissionSubcategory){
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
      case 'Attendance':{
        return 'access_alarm';
      }
      case 'Administration':{
        return 'admin_panel_settings';
      }
      case 'Grades':{
        return 'leaderboard';
      }
      case 'List of Values':{
        return 'list_alt';
      }
    }
  }

  setPageId(pageId?,path?) {
    this.defaultValuesService.setPageId(pageId);
    if(pageId==="Marking Periods"){
      this.router.navigate([path]);
    }
    if(pageId==="Calendars"){
      this.router.navigate([path]);
    }
  }

  setParentId(index){
    for(let item of this.settingSubmenu[index].permissionSubcategory){
      if(item.rolePermission[0].canView){
        this.defaultValuesService.setPageId(item.title);
        break;
      }
    }
  }
}
