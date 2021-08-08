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

import { StaffCertificateListModel, StaffCertificateModel } from '../../../../models/staff.model';
import { StaffService } from '../../../../services/staff.service';
import { Component, OnInit, Input, ViewChild } from '@angular/core';
import icMoreVert from '@iconify/icons-ic/twotone-more-vert';
import icAdd from '@iconify/icons-ic/baseline-add';
import icEdit from '@iconify/icons-ic/twotone-edit';
import icDelete from '@iconify/icons-ic/twotone-delete';
import icSearch from '@iconify/icons-ic/search';
import icFilterList from '@iconify/icons-ic/filter-list';
import icImpersonate from '@iconify/icons-ic/twotone-account-circle';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { fadeInUp400ms } from '../../../../../@vex/animations/fade-in-up.animation';
import { stagger40ms } from '../../../../../@vex/animations/stagger.animation';
import { TranslateService } from '@ngx-translate/core';
import { AddCertificateComponent } from './add-certificate/add-certificate.component';
import { CertificateDetailsComponent } from './certificate-details/certificate-details.component';
import { MatTableDataSource } from '@angular/material/table';
import { ConfirmDialogComponent } from '../../../shared-module/confirm-dialog/confirm-dialog.component';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { RolePermissionListViewModel, RolePermissionViewModel } from 'src/app/models/roll-based-access.model';
import { RollBasedAccessService } from 'src/app/services/roll-based-access.service';
import { PageRolesPermission } from '../../../../common/page-roles-permissions.service';
import { Permissions } from '../../../../models/roll-based-access.model';
import { CommonService } from 'src/app/services/common.service';

@Component({
  selector: 'vex-staff-certificationinfo',
  templateUrl: './staff-certificationinfo.component.html',
  styleUrls: ['./staff-certificationinfo.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})
export class StaffCertificationinfoComponent implements OnInit {
  columns = [
    { label: 'Certification Name', property: 'certificationName', type: 'text', visible: true },
    { label: 'Short Name', property: 'shortName', type: 'text', visible: true },
    { label: 'Primary Certification Indicator', property: 'primaryCertification', type: 'text', visible: true },
    { label: 'Certification Date', property: 'certificationDate', type: 'text', visible: true },
    { label: 'Certification Expiry Date', property: 'certificationExpiryDate', type: 'text', visible: true },
    { label: 'Actions', property: 'actions', type: 'text', visible: true }
  ];


  icMoreVert = icMoreVert;
  icAdd = icAdd;
  icEdit = icEdit;
  icDelete = icDelete;
  icSearch = icSearch;
  icImpersonate = icImpersonate;
  icFilterList = icFilterList;
  loading: boolean;
  staffCertificateListModel: StaffCertificateListModel = new StaffCertificateListModel();
  staffCertificateModel: StaffCertificateModel = new StaffCertificateModel();
  staffid;
  staffCertificateList: MatTableDataSource<any>;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  searchKey: string;
  permissions: Permissions;
  constructor(
    private router: Router,
    private dialog: MatDialog,
    public translateService: TranslateService,
    private snackbar: MatSnackBar,
    public rollBasedAccessService: RollBasedAccessService,
    private staffService: StaffService,
    private pageRolePermissions: PageRolesPermission,
    private commonService: CommonService,
  ) {
    //translateService.use('en');
  }

  ngOnInit(): void {
    this.permissions = this.pageRolePermissions.checkPageRolePermission();
    this.staffid = this.staffService.getStaffId();
    this.getAllStaffCertificateInfo();
  }

  getPageEvent(event) {
    // this.getAllSchool.pageNumber=event.pageIndex+1;
    // this.getAllSchool.pageSize=event.pageSize;
    // this.callAllSchool(this.getAllSchool);
  }
  onSearchClear() {
    this.searchKey = "";
    this.applyFilter();
  }
  applyFilter() {
    this.staffCertificateList.filter = this.searchKey.trim().toLowerCase()
  }
  openAddNew() {
    this.dialog.open(AddCertificateComponent, {
      data: null,
      width: '600px'
    }).afterClosed().subscribe((data) => {
      if (data === 'submited') {
        this.getAllStaffCertificateInfo();
      }
    });
  }
  openEditdata(element) {

    this.dialog.open(AddCertificateComponent, {
      data: element,
      width: '600px'
    }).afterClosed().subscribe((data) => {
      if (data === 'submited') {
        this.getAllStaffCertificateInfo();
      }
    })
  }
  openViewDetails(element) {
    this.dialog.open(CertificateDetailsComponent, {
      data: { info: element },
      width: '600px'
    })
  }

  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }
  getAllStaffCertificateInfo() {
    this.staffCertificateListModel.staffId = this.staffService.getStaffId();
    this.staffService.getAllStaffCertificateInfo(this.staffCertificateListModel).subscribe(
      (res: StaffCertificateListModel) => {
        if (typeof (res) == 'undefined') {
          this.snackbar.open('Staff Certificate List failed. ' + sessionStorage.getItem("httpError"), '', {
            duration: 10000
          });
        }
        else {
          if (res._failure) {
            this.commonService.checkTokenValidOrNot(res._message);
            this.staffCertificateList = new MatTableDataSource([]);
            this.staffCertificateList.sort = this.sort;
            if (!res.staffCertificateInfoList) {
              this.snackbar.open(res._message, '', {
                duration: 10000
              });
            }
          }
          else {
            this.staffCertificateList = new MatTableDataSource(res.staffCertificateInfoList);
            this.staffCertificateList.sort = this.sort;
          }
        }
      }
    );
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }
  deleteStaffCertificatedata(element) {
    this.staffCertificateModel.staffCertificateInfo = element
    this.staffService.deleteStaffCertificateInfo(this.staffCertificateModel).subscribe(
      (res: StaffCertificateModel) => {
        if (typeof (res) == 'undefined') {
          this.snackbar.open('Staff Certificate Delete failed. ' + sessionStorage.getItem("httpError"), '', {
            duration: 10000
          });
        }
        else {
          if (res._failure) {
            this.commonService.checkTokenValidOrNot(res._message);
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
          else {
            this.getAllStaffCertificateInfo()
          }
        }
      }
    )
  }
  confirmDelete(element) {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: '400px',
      data: {
        title: 'Are you sure?',
        message: 'You are about to delete ' + element.certificationName + '.'
      }
    });
    dialogRef.afterClosed().subscribe(dialogResult => {
      if (dialogResult) {
        this.deleteStaffCertificatedata(element);
      }
    });
  }

}
