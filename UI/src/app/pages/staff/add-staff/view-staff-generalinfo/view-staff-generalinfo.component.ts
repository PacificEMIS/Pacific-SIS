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

import { Component, Input, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { PageRolesPermission } from '../../../../common/page-roles-permissions.service';
import { ResetPasswordComponent } from '../../../../pages/shared-module/reset-password/reset-password.component';
import { stagger60ms } from '../../../../../@vex/animations/stagger.animation';
import { StaffAddModel } from '../../../../models/staff.model';
import { SharedFunction } from '../../../shared/shared-function';
import { Permissions, RolePermissionListViewModel, RolePermissionViewModel } from '../../../../models/roll-based-access.model';
import icCheckbox from '@iconify/icons-ic/baseline-check-box';
import icCheckboxOutline from '@iconify/icons-ic/baseline-check-box-outline-blank';
import { StaffService } from 'src/app/services/staff.service';
@Component({
  selector: 'vex-view-staff-generalinfo',
  templateUrl: './view-staff-generalinfo.component.html',
  styleUrls: ['./view-staff-generalinfo.component.scss'],
  animations: [
    stagger60ms
  ]
})
export class ViewStaffGeneralinfoComponent implements OnInit {
  @Input() staffCreateMode;
  @Input() categoryId;
  @Input() staffViewDetails: StaffAddModel =  new StaffAddModel();
  @Input() nameOfMiscValues;
  module = 'Staff';
  staffPortalAccess: string;
  permissions: Permissions;
  icCheckbox = icCheckbox;
  icCheckboxOutline = icCheckboxOutline;
  constructor(public translateService: TranslateService,
              private commonFunction: SharedFunction,
              private dialog: MatDialog,
              private pageRolePermissions: PageRolesPermission,
              private staffService: StaffService,
              ) {
    //translateService.use('en');
  }

  ngOnInit(): void {
    this.viewPortalAccess();
    this.permissions = this.pageRolePermissions.checkPageRolePermission();
  }

  viewPortalAccess() {
    if (this.staffViewDetails?.staffMaster?.portalAccess) {
      this.staffPortalAccess = 'Yes';
    }
    else {
      this.staffPortalAccess = 'No';
    }
  }

  // This openResetPassword method is used for open Reset Password dialog.
  openResetPassword() {
    if (this.staffViewDetails.staffMaster.profile !== 'Super Administrator') {
      this.staffService.checkExternalSchoolId(this.staffViewDetails, this.categoryId).then((res: any) => {
        this.dialog.open(ResetPasswordComponent, {
          width: '500px',
          data: { userId: this.staffViewDetails.staffMaster.staffId, emailAddress: this.staffViewDetails.staffMaster.loginEmailAddress }
        });
      });
    }
    
  }

  getAge(birthDate) {
    return this.commonFunction.getAge(birthDate);
  }

}
