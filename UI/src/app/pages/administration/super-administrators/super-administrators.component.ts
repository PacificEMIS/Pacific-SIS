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

import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableDataSource } from '@angular/material/table';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import icAdd from '@iconify/icons-ic/twotone-add';
import icMoreVert from '@iconify/icons-ic/twotone-more-vert';
import icArrowUpward from '@iconify/icons-ic/twotone-arrow-upward';
import icInfo from '@iconify/icons-ic/twotone-info';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { PageRolesPermission } from 'src/app/common/page-roles-permissions.service';
import { ModuleIdentifier } from 'src/app/enums/module-identifier.enum';
import { ProfilesTypes } from 'src/app/enums/profiles.enum';
import { SchoolCreate } from 'src/app/enums/school-create.enum';
import { SuperAdministratorActionModel, SuperAdministratorListModel, SuperAdministratorModel } from 'src/app/models/super-administrator.model';
import { CommonService } from 'src/app/services/common.service';
import { ImageCropperService } from 'src/app/services/image-cropper.service';
import { LoaderService } from 'src/app/services/loader.service';
import { StaffService } from 'src/app/services/staff.service';
import { SuperAdministratorService } from 'src/app/services/super-administrator.service';
import { ConfirmDialogComponent } from '../../shared-module/confirm-dialog/confirm-dialog.component';
import { AddSuperAdministratorComponent } from './add-super-administrator/add-super-administrator.component';
import { DemoteSuperAdministratorComponent } from './demote-super-administrator/demote-super-administrator.component';
import { PromoteStaffComponent } from './promote-staff/promote-staff.component';

/**
 * Settings > Administration > Super Administrators.
 *
 * Lists every tenant-wide Super Administrator and lets another Super
 * Administrator add, promote, demote, activate, deactivate or delete
 * one. The API enforces the same rules; this page only reflects them
 * (for example the caller's own row has no actions).
 */
@Component({
  selector: 'vex-super-administrators',
  templateUrl: './super-administrators.component.html',
  styleUrls: ['./super-administrators.component.scss']
})
export class SuperAdministratorsComponent implements OnInit, OnDestroy {
  icAdd = icAdd;
  icMoreVert = icMoreVert;
  icArrowUpward = icArrowUpward;
  icInfo = icInfo;

  displayedColumns: string[] = ['name', 'loginEmail', 'homeSchool', 'status', 'lastLogin', 'actions'];
  dataSource: MatTableDataSource<SuperAdministratorModel> = new MatTableDataSource<SuperAdministratorModel>([]);
  loading = false;
  loaded = false;
  isSuperAdministrator = false;
  destroySubject$: Subject<void> = new Subject();

  constructor(
    private superAdministratorService: SuperAdministratorService,
    private defaultValuesService: DefaultValuesService,
    private commonService: CommonService,
    private snackbar: MatSnackBar,
    private dialog: MatDialog,
    private translateService: TranslateService,
    private loaderService: LoaderService,
    private staffService: StaffService,
    private imageCropperService: ImageCropperService,
    private pageRolePermissions: PageRolesPermission,
    private router: Router
  ) {
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });
  }

  ngOnInit(): void {
    this.isSuperAdministrator = this.defaultValuesService.getUserMembershipType() === ProfilesTypes.SuperAdmin;
    if (this.isSuperAdministrator) {
      this.getAll();
    }
  }

  getAll() {
    this.superAdministratorService.getAll(new SuperAdministratorListModel()).subscribe((res) => {
      this.loaded = true;
      if (typeof (res) === 'undefined') {
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', { duration: 10000 });
        return;
      }
      if (res._failure) {
        this.commonService.checkTokenValidOrNot(res._message);
        this.dataSource = new MatTableDataSource<SuperAdministratorModel>([]);
        this.snackbar.open(res._message, '', { duration: 10000 });
        return;
      }
      this.dataSource = new MatTableDataSource<SuperAdministratorModel>(res.superAdministrators);
    });
  }

  fullName(row: SuperAdministratorModel) {
    return [row.firstGivenName, row.middleName, row.lastFamilyName].filter(x => x).join(' ');
  }

  openAdd() {
    this.dialog.open(AddSuperAdministratorComponent, { width: '560px', disableClose: true })
      .afterClosed().subscribe((saved) => { if (saved) { this.getAll(); } });
  }

  openPromote() {
    this.dialog.open(PromoteStaffComponent, { width: '640px', disableClose: true })
      .afterClosed().subscribe((saved) => { if (saved) { this.getAll(); } });
  }

  openDemote(row: SuperAdministratorModel) {
    this.dialog.open(DemoteSuperAdministratorComponent, { width: '560px', disableClose: true, data: row })
      .afterClosed().subscribe((saved) => { if (saved) { this.getAll(); } });
  }

  toggleStatus(row: SuperAdministratorModel) {
    const activate = !row.isActive;
    const key = activate ? 'confirmActivateSuperAdministrator' : 'confirmDeactivateSuperAdministrator';
    this.confirm(this.translateService.instant(key, { name: this.fullName(row) })).subscribe((ok) => {
      if (!ok) { return; }
      const model = new SuperAdministratorActionModel();
      model.staffId = row.staffId;
      model.isActive = activate;
      this.superAdministratorService.setActiveStatus(model).subscribe((res) => this.afterAction(res));
    });
  }

  delete(row: SuperAdministratorModel) {
    this.confirm(this.translateService.instant('confirmDeleteSuperAdministrator', { name: this.fullName(row) })).subscribe((ok) => {
      if (!ok) { return; }
      const model = new SuperAdministratorActionModel();
      model.staffId = row.staffId;
      this.superAdministratorService.delete(model).subscribe((res) => this.afterAction(res));
    });
  }

  viewStaffRecord(row: SuperAdministratorModel) {
    // Same hand-off the staff list uses to open a staff record in view mode.
    this.imageCropperService.enableUpload({ module: ModuleIdentifier.STAFF, upload: true, mode: SchoolCreate.VIEW });
    this.staffService.setStaffId(row.staffId);
    const permittedDetails = this.pageRolePermissions.getPermittedSubCategories('/school/staff');
    if (permittedDetails.length) {
      this.staffService.setCategoryId(0);
      this.staffService.setCategoryTitle(permittedDetails[0].title);
      this.router.navigateByUrl(permittedDetails[0].path, { state: { type: SchoolCreate.VIEW } });
    }
  }

  private confirm(message: string) {
    return this.dialog.open(ConfirmDialogComponent, {
      maxWidth: '420px',
      data: { title: this.translateService.instant('areYouSure'), message }
    }).afterClosed();
  }

  private afterAction(res: SuperAdministratorActionModel) {
    if (typeof (res) === 'undefined') {
      this.snackbar.open(this.defaultValuesService.getHttpError(), '', { duration: 10000 });
      return;
    }
    if (res._failure) {
      this.commonService.checkTokenValidOrNot(res._message);
    }
    this.snackbar.open(res._message, '', { duration: 10000 });
    if (!res._failure) {
      this.getAll();
    }
  }

  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }
}
