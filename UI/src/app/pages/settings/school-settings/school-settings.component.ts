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
import { RolePermissionListViewModel } from 'src/app/models/roll-based-access.model';
import { CryptoService } from 'src/app/services/Crypto.service';
import { fadeInRight400ms } from '../../../../@vex/animations/fade-in-right.animation';
import { DefaultValuesService } from '../../../common/default-values.service';
import { PageRolesPermission } from '../../../common/page-roles-permissions.service';
import { Router } from '@angular/router';
@Component({
  selector: 'vex-school-settings',
  templateUrl: './school-settings.component.html',
  styleUrls: ['./school-settings.component.scss'],
  animations: [
    fadeInRight400ms
  ]
})
export class SchoolSettingsComponent implements OnInit {
  pages=[];
  schoolSettings=true;
  pageTitle:string;
  pageId: string;
  permittedSubmenuList:any;
  constructor(private pageRolePermissions: PageRolesPermission, private defaultValuesService: DefaultValuesService,private router: Router) { }

  ngOnInit(): void {
    this.permittedSubmenuList = this.pageRolePermissions.getPermittedSubCategories('/school/settings/school-settings');
    this.permittedSubmenuList.map((option)=>{
        this.pages.push(option.title);
    }) 
    let availablePageId=this.defaultValuesService.getPageId();
    if(!availablePageId){
          this.defaultValuesService.setPageId(this.pages[0]);
    }
    this.pageId = this.defaultValuesService.getPageId();
  }

  getSelectedPage(pageId){
    this.pageId = pageId;
    if(this.pageId==="Marking Periods"){
      this.router.navigate(['school/marking-periods']);
    }
    if(this.pageId==="Calendars"){
      this.router.navigate(['school/schoolcalendars']);
    }
    this.defaultValuesService.setPageId(pageId);
  }



}
