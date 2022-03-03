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

import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { fadeInRight400ms } from '../../../../../@vex/animations/fade-in-right.animation';
import { stagger60ms } from '../../../../../@vex/animations/stagger.animation';
import { fadeInUp400ms } from '../../../../../@vex/animations/fade-in-up.animation';
import { TranslateService } from '@ngx-translate/core';
import { TakeAttendanceList } from '../../../../models/take-attendance-list.model';
import { Router } from '@angular/router';
import { TakeAttendanceComponent } from '../take-attendance/take-attendance.component';
import { RolePermissionListViewModel } from 'src/app/models/roll-based-access.model';
import { CryptoService } from 'src/app/services/Crypto.service';
import { MatPaginator, MatPaginatorIntl } from '@angular/material/paginator';
import { GetAllStaffModel, StaffMasterModel } from 'src/app/models/staff.model';
import { MatTableDataSource } from '@angular/material/table';
import { FormControl } from '@angular/forms';
import { MatSort } from '@angular/material/sort';
import { MatSnackBar } from '@angular/material/snack-bar';
import { LoaderService } from 'src/app/services/loader.service';
import { StaffService } from 'src/app/services/staff.service';
import { FinalGradeService } from 'src/app/services/final-grade.service';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { Permissions } from '../../../../models/roll-based-access.model';
import { PageRolesPermission } from '../../../../common/page-roles-permissions.service';
import { CommonService } from 'src/app/services/common.service';
import { DefaultValuesService } from 'src/app/common/default-values.service';

@Component({
  selector: 'vex-input-effort-grades',
  templateUrl: './input-effort-grades.component.html',
  styleUrls: ['./input-effort-grades.component.scss'],
  animations: [
    fadeInRight400ms,
    stagger60ms,
    fadeInUp400ms
  ]
})
export class InputEffortGradesComponent implements OnInit, AfterViewInit {

  pageStatus = "Teacher Function";
  totalCount: number = 0;
  pageSize: number;
  pageInit = 1;
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort
  getAllStaff: GetAllStaffModel = new GetAllStaffModel();
  staffList: MatTableDataSource<StaffMasterModel>;
  loading: boolean;
  pageNumber: number;
  searchCtrl: FormControl;
  displayedColumns: string[] = ['lastFamilyName', 'staffInternalId', 'profile', 'jobTitle', 'schoolEmail', 'mobilePhone'];

  permissionListViewModel: RolePermissionListViewModel = new RolePermissionListViewModel();
  permissionGroup;
  permissionCategoryForTeacherFunctions;
  permissions: Permissions;
  constructor(
    public translateService: TranslateService,
    private router: Router,
    private snackbar: MatSnackBar,
    private cryptoService: CryptoService,
    private loaderService: LoaderService,
    private staffService: StaffService,
    private finalGradeService: FinalGradeService,
    private pageRolePermissions: PageRolesPermission,
    private commonService: CommonService,
    private defaultValuesService: DefaultValuesService,
    private paginatorObj: MatPaginatorIntl,
  ) {
    paginatorObj.itemsPerPageLabel = translateService.instant('itemsPerPage');
    //translateService.use('en');
    this.getAllStaff.filterParams = null;
    this.loaderService.isLoading.subscribe((val) => {
      this.loading = val;
    });
  }

  viewEffortGradeDetails(element) { 
    if(!this.permissions?.edit) return;
    const staffFullName = `${element.firstGivenName} ${element.middleName ? element.middleName + ' ' : ''}${element.lastFamilyName}`;
    this.finalGradeService.setStaffDetails({ staffId: element.staffId, staffFullName });
    this.router.navigate(['/school', 'staff', 'teacher-functions', 'input-effort-grade', 'effort-grade-details']);
    this.pageInit = 2;
  }

  ngOnInit(): void {
    this.callStaffList();
    this.searchCtrl = new FormControl();
    this.permissions = this.pageRolePermissions.checkPageRolePermission();
    // if(!this.inputEffortGradePermissions.canView) {
    //   this.router.navigate(['/school', 'staff', 'teacher-functions']);
    // }
  }

  ngAfterViewInit() {
    //  Sorting
    this.getAllStaff = new GetAllStaffModel();
    this.sort.sortChange.subscribe((res) => {
      this.getAllStaff.pageNumber = this.pageNumber
      this.getAllStaff.pageSize = this.pageSize;
      this.getAllStaff.sortingModel.sortColumn = res.active;
      if (this.searchCtrl.value != null && this.searchCtrl.value != "") {
        let filterParams = [
          {
            columnName: null,
            filterValue: this.searchCtrl.value,
            filterOption: 4
          }
        ]
        Object.assign(this.getAllStaff, { filterParams: filterParams });
      }
      if (res.direction == "") {
        this.getAllStaff.sortingModel = null;
        this.callStaffList();
        this.getAllStaff = new GetAllStaffModel();
        this.getAllStaff.sortingModel = null;
      } else {
        this.getAllStaff.sortingModel.sortDirection = res.direction;
        this.callStaffList();
      }
    });

    //  Searching
    this.searchCtrl.valueChanges.pipe(debounceTime(500), distinctUntilChanged()).subscribe((term) => {
      if (term != '') {
        this.callWithFilterValue(term);
      } else {
        this.callWithoutFilterValue()
      }
    });
  }

  callWithFilterValue(term) {
    let filterParams = [
      {
        columnName: null,
        filterValue: term,
        filterOption: 4
      }
    ]
    if (this.sort.active != undefined && this.sort.direction != "") {
      this.getAllStaff.sortingModel.sortColumn = this.sort.active;
      this.getAllStaff.sortingModel.sortDirection = this.sort.direction;
    }
    Object.assign(this.getAllStaff, { filterParams: filterParams });
    this.getAllStaff.pageNumber = 1;
    this.paginator.pageIndex = 0;
    this.getAllStaff.pageSize = this.pageSize;
    this.callStaffList();
  }

  callWithoutFilterValue() {
    Object.assign(this.getAllStaff, { filterParams: null });
    this.getAllStaff.pageNumber = this.paginator.pageIndex + 1;
    this.getAllStaff.pageSize = this.pageSize;
    if (this.sort.active != undefined && this.sort.direction != "") {
      this.getAllStaff.sortingModel.sortColumn = this.sort.active;
      this.getAllStaff.sortingModel.sortDirection = this.sort.direction;
    }
    this.callStaffList();
  }

  getPageEvent(event) {
    if (this.sort.active != undefined && this.sort.direction != "") {
      this.getAllStaff.sortingModel.sortColumn = this.sort.active;
      this.getAllStaff.sortingModel.sortDirection = this.sort.direction;
    }
    if (this.searchCtrl.value != null && this.searchCtrl.value != "") {
      let filterParams = [
        {
          columnName: null,
          filterValue: this.searchCtrl.value,
          filterOption: 3
        }
      ]
      Object.assign(this.getAllStaff, { filterParams: filterParams });
    }
    this.getAllStaff.pageNumber = event.pageIndex + 1;
    this.getAllStaff.pageSize = event.pageSize;
    this.callStaffList();
  }

  callStaffList() {
    if (this.getAllStaff.sortingModel?.sortColumn == "") {
      this.getAllStaff.sortingModel = null
    }
    this.getAllStaff.isHomeRoomTeacher = true ;
    this.staffService.getAllStaffList(this.getAllStaff).subscribe(res => {
    if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
        this.staffList = new MatTableDataSource([]);
        this.totalCount=0;
        if (!res.staffMaster) {
          this.snackbar.open(res._message, '', {
            duration: 10000
          });
        }

      } else {
        this.totalCount = res.totalCount;
        this.pageNumber = res.pageNumber;
        this.pageSize = res._pageSize;
        this.staffList = new MatTableDataSource(res.staffMaster);
        this.getAllStaff = new GetAllStaffModel();
      }
    });
  }

}
