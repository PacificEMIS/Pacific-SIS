import { Component, OnInit, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { PageRolesPermission } from 'src/app/common/page-roles-permissions.service';
import { RolePermissionListViewModel } from 'src/app/models/roll-based-access.model';
import { GetAllStaffModel, StaffMasterModel } from 'src/app/models/staff.model';
import { CommonService } from 'src/app/services/common.service';
import { CryptoService } from 'src/app/services/Crypto.service';
import { FinalGradeService } from 'src/app/services/final-grade.service';
import { LoaderService } from 'src/app/services/loader.service';
import { StaffService } from 'src/app/services/staff.service';
@Component({
  selector: 'vex-gradebook-grades',
  templateUrl: './gradebook-grades.component.html',
  styleUrls: ['./gradebook-grades.component.scss']
})



export class GradebookGradesComponent implements OnInit {

  pageStatus = "Teacher Function";
  totalCount: number = 0;
  pageSize: number;
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
  permissions;

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
    private defaultValuesService: DefaultValuesService
  ) {
    //translateService.use('en');
    this.getAllStaff.filterParams = null;
    this.loaderService.isLoading.subscribe((val) => {
      this.loading = val;
    });    
  }

  viewGradeDetails(element) {     
    if(!this.permissions?.edit) return;
    const staffFullName = `${element.firstGivenName} ${element.middleName ? element.middleName + ' ' : ''}${element.lastFamilyName}`;
    this.finalGradeService.setStaffDetails({ staffId: element.staffId, staffFullName });
    this.router.navigate(['/school', 'staff', 'teacher-functions', 'gradebook-grades', 'gradebook-grade-list']);
  }

  ngOnInit(): void {
    this.callStaffList();
    this.searchCtrl = new FormControl();
    this.permissions = this.pageRolePermissions.checkPageRolePermission();
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
