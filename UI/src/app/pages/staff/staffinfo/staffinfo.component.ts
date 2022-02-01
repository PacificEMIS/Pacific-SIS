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

import { Component, OnInit, ViewChild, AfterViewInit} from '@angular/core';
import icMoreVert from '@iconify/icons-ic/twotone-more-vert';
import icAdd from '@iconify/icons-ic/baseline-add';
import icEdit from '@iconify/icons-ic/twotone-edit';
import icDelete from '@iconify/icons-ic/twotone-delete';
import icSearch from '@iconify/icons-ic/search';
import icFilterList from '@iconify/icons-ic/filter-list';
import icImpersonate from '@iconify/icons-ic/twotone-account-circle';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { fadeInUp400ms } from '../../../../@vex/animations/fade-in-up.animation';
import { stagger40ms } from '../../../../@vex/animations/stagger.animation';
import { TranslateService } from '@ngx-translate/core';
import { StaffService } from '../../../services/staff.service';
import { LoaderService } from '../../../services/loader.service';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { FormControl } from '@angular/forms';
import { debounceTime, distinctUntilChanged, takeUntil, filter } from 'rxjs/operators';
import { GetAllStaffModel, StaffListModel, StaffMasterModel, StaffSchoolInfoModel } from '../../../models/staff.model';
import { ImageCropperService } from '../../../services/image-cropper.service';
import { ExcelService } from '../../../services/excel.service';
import { Subject } from 'rxjs';
import { ModuleIdentifier } from '../../../enums/module-identifier.enum';
import { SchoolCreate } from '../../../enums/school-create.enum';
import { fadeInRight400ms } from 'src/@vex/animations/fade-in-right.animation';
import { Permissions, RolePermissionListViewModel, RolePermissionViewModel } from '../../../models/roll-based-access.model';
import { RollBasedAccessService } from '../../../services/roll-based-access.service';
import { SaveFilterComponent } from './save-filter/save-filter.component';
import { MatDialog } from '@angular/material/dialog';
import { SearchFilter, SearchFilterAddViewModel, SearchFilterListViewModel } from '../../../models/search-filter.model';
import { CommonService } from '../../../services/common.service';
import { ConfirmDialogComponent } from '../../shared-module/confirm-dialog/confirm-dialog.component';
import { CryptoService } from '../../../services/Crypto.service';
import moment from 'moment';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { PageRolesPermission } from '../../../common/page-roles-permissions.service';
import { DataEditInfoComponent } from '../../shared-module/data-edit-info/data-edit-info.component';
import icRestore from '@iconify/icons-ic/twotone-restore';


@Component({
  selector: 'vex-staffinfo',
  templateUrl: './staffinfo.component.html',
  styleUrls: ['./staffinfo.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms,
    fadeInRight400ms
  ]
})
export class StaffinfoComponent implements OnInit, AfterViewInit{
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort

  getAllStaff: GetAllStaffModel = new GetAllStaffModel();
  staffList: MatTableDataSource<any>;
  showAdvanceSearchPanel: boolean = false;
  columns = [
    { label: 'Name', property: 'lastFamilyName', type: 'text', visible: true },
    { label: 'Staff ID', property: 'staffInternalId', type: 'text', visible: true },
    { label: 'Profile', property: 'profile', type: 'text', visible: true },
    { label: 'Job Title', property: 'jobTitle', type: 'text', visible: true },
    { label: 'School Email', property: 'schoolEmail', type: 'text', visible: true },
    { label: 'Mobile Phone', property: 'mobilePhone', type: 'number', visible: true },
    { label: 'Status', property: 'status', type: 'text', visible: false },
    { label: 'Actions', property: 'actions', type: 'text', visible: true }
  ];

  icMoreVert = icMoreVert;
  icAdd = icAdd;
  icEdit = icEdit;
  icDelete = icDelete;
  icSearch = icSearch;
  icImpersonate = icImpersonate;
  icRestore = icRestore;
  icFilterList = icFilterList;

  loading: boolean;
  totalCount: number = 0;
  pageNumber: number;
  pageSize: number;
  searchCtrl: FormControl;
  destroySubject$: Subject<void> = new Subject();
  moduleIdentifier=ModuleIdentifier;
  createMode=SchoolCreate;
  searchValue: any = null;
  searchCount: number = null;

  showSaveFilter:boolean=false;
  searchFilterAddViewModel: SearchFilterAddViewModel = new SearchFilterAddViewModel();
  searchFilterListViewModel: SearchFilterListViewModel= new SearchFilterListViewModel();
  searchFilter: SearchFilter= new SearchFilter();
  filterJsonParams;
  showLoadFilter=true;
  toggleValues: any = null;
  categories=[
    {
      categoryId:12,
      title:'General Info'
    },
    {
      categoryId:13,
      title:'School Info'
    },
    {
      categoryId:14,
      title:'Address & Contact'
    },
    {
      categoryId:15,
      title:'Certification Info'
    }
  ]
  permissions: Permissions;
  constructor(private snackbar: MatSnackBar,
              private router: Router,
              private loaderService: LoaderService,
              public translateService: TranslateService,
              private staffService: StaffService,
              private imageCropperService:ImageCropperService,
              public rollBasedAccessService: RollBasedAccessService,
              private excelService:ExcelService,
              private pageRolePermission: PageRolesPermission,
              private dialog: MatDialog,
              private commonService:CommonService,
              private cryptoService: CryptoService,
              public defaultValuesService: DefaultValuesService
              ) {
                this.defaultValuesService.sendAllSchoolFlag(false);
                this.defaultValuesService.sendIncludeInactiveFlag(false);
    this.getAllStaff.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
    
    this.getAllStaff.filterParams = null;
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });
  }

  ngOnInit(): void {
    this.callStaffList();
    this.getAllSearchFilter();
    this.searchCtrl = new FormControl();

    this.permissions = this.pageRolePermission.checkPageRolePermission();
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
    this.defaultValuesService.setPageSize(event.pageSize);
    this.callStaffList();
  }

  viewStaffDetails(data) {
    this.imageCropperService.enableUpload({module:this.moduleIdentifier.STAFF,upload:true,mode:this.createMode.VIEW});
    this.staffService.setStaffId(data.staffId);
    // this.defaultValuesService.setSchoolID(data.schoolId, true);
    this.getPermissionForStaff();
  }

  getPermissionForStaff() {
    // let rolePermissionListView: RolePermissionListViewModel = new RolePermissionListViewModel();
    // this.rollBasedAccessService.getAllRolePermission(rolePermissionListView).subscribe((res: RolePermissionListViewModel) => {
    //   if(res._failure){
    //     this.commonService.checkTokenValidOrNot(res._message);
    //   } else{
        let permittedDetails= this.pageRolePermission.getPermittedSubCategories('/school/staff');
    if(permittedDetails.length){
      this.staffService.setCategoryId(0);
      this.staffService.setCategoryTitle(permittedDetails[0].title);      
      this.router.navigateByUrl(permittedDetails[0].path, {state: { type: SchoolCreate.VIEW}});
    } else {
          this.defaultValuesService.setSchoolID(undefined);
          this.snackbar.open('Saff didnot have permission to view details.', '', {
            duration: 10000
          });
        }
    //   }
    // });
  }
  

  goToAdd() {
    this.staffService.setStaffId(null);
    this.router.navigate(['/school', 'staff', 'staff-generalinfo']); 
    this.imageCropperService.enableUpload({module:this.moduleIdentifier.STAFF,upload:true,mode:this.createMode.ADD});

  }

  navigateToSetting(){
    this.defaultValuesService.setPageId('Staff Bulk Data Import');
    this.router.navigate(["school/tools/staff-bulk-data-import"]);
  }

  callStaffList() {
    if (this.getAllStaff.sortingModel?.sortColumn == "") {
      this.getAllStaff.sortingModel = null
    }
    this.staffService.getAllStaffList(this.getAllStaff).subscribe(res => {
    if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
        this.staffList = new MatTableDataSource([]);
        this.totalCount = null;
        if (!res.staffMaster){
          this.snackbar.open(res._message, '', {
            duration: 10000
          });
        }
      } else {
        this.totalCount = res.totalCount;
        this.pageNumber = res.pageNumber;
        this.pageSize = res._pageSize;
        this.staffList = new MatTableDataSource(res.staffMaster);
        for (let staff of this.staffList.filteredData){
          if (staff.isActive === true || staff.isActive === null) {
          if (staff.staffSchoolInfo[0].endDate){
            let today = moment().format('DD-MM-YYYY').toString();
            let todayarr = today.split('-');
            let todayDate = +todayarr[0];
            let todayMonth = +todayarr[1];
            let todayYear = +todayarr[2];
            let endDate = moment(staff.staffSchoolInfo[0].endDate).format('DD-MM-YYYY').toString();
            let endDateArr = endDate.split('-');
            let endDateDate = +endDateArr[0];
            let endDateMonth = +endDateArr[1];
            let endDateYear = +endDateArr[2];
            if ( endDateYear === todayYear){
              if (endDateMonth === todayMonth){
                if (endDateDate >= todayDate){
                  staff.status = 'active';
                }
                else {
                  staff.status = 'inactive';
                }
              }
              else if (endDateMonth < todayMonth){
                staff.status = 'inactive';
              }
              else{
                staff.status = 'active';
              }
            }
            else if (endDateYear < todayYear){
              staff.status = 'inactive';
            }
            else{
              staff.status = 'active';
            }
          }
          else{
            staff.status = 'active';
          }
          } else {
            staff.status = 'inactive';
          }
        }
        this.getAllStaff = new GetAllStaffModel();
      }
    });
  }

  exportStaffListToExcel(){
    let getAllStaff: GetAllStaffModel = new GetAllStaffModel();
    getAllStaff.pageNumber=0;
    getAllStaff.pageSize=0;
    getAllStaff.sortingModel=null;
    this.staffService.getAllStaffList(getAllStaff).subscribe(res => {
       if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          this.snackbar.open('Failed to Export Staff List.'+ res._message, '', {
          duration: 10000
          });
        }else{
          if(res.staffMaster.length>0){
            let staffList = res.staffMaster?.map((x:StaffMasterModel)=>{
              let middleName=x.middleName==null?' ':' '+x.middleName+' ';
              return {
               Name: x.firstGivenName+middleName+x.lastFamilyName,
               'Staff ID': x.staffInternalId,
               Profile: x.profile,
               'Job Title': x.jobTitle,
               'School Email':x.schoolEmail,
               'Mobile Phone':x.mobilePhone
             }
            });

            this.excelService.exportAsExcelFile(staffList,'Staffs_List_')
          }else{
            this.snackbar.open('No Records Found. Failed to Export Staff List','', {
              duration: 5000
            });
          }
        }
      });
    
  }

  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  showAdvanceSearch() {
    this.showAdvanceSearchPanel = true;
    this.filterJsonParams = null;
  }

  hideAdvanceSearch(event){
    this.showSaveFilter = event.showSaveFilter;
    this.showAdvanceSearchPanel = false;
    if(event.showSaveFilter == false){
      this.getAllSearchFilter();
    }
  }

  // For open the data chage history dialog
  openDataEdit(element) {
    this.dialog.open(DataEditInfoComponent, {
      width: '500px',
      data: { createdBy: element.createdBy, createdOn: element.createdOn, modifiedBy: element.updatedBy, modifiedOn: element.updatedOn }
    })
  }

  getSearchResult(res){
    if (res.totalCount){
      this.searchCount = res.totalCount;
      this.totalCount = res.totalCount;
    }
    else{
      this.searchCount = 0;
      this.totalCount = 0;
    }
    this.showSaveFilter = true;
    this.pageNumber = res.pageNumber;
    this.pageSize = res._pageSize;
    this.staffList = new MatTableDataSource(res.staffMaster);
    this.getAllStaff = new GetAllStaffModel();
  }
  getSearchInput(event){
    this.searchValue = event;
    this.columns[6].visible = event.inactiveStaff;
  }
  resetStaffList(){
    this.searchCount = null;
    this.searchValue = null;
    this.callStaffList();
  }
  getToggleValues(event){
    this.toggleValues = event;
    if (event.inactiveStaff === true){
      this.columns[6].visible = true;
    }
    else if (event.inactiveStaff === false){
      this.columns[6].visible = false;
    }
  }

  openSaveFilter(){
    this.dialog.open(SaveFilterComponent, {
      width: '500px',
      data: null
    }).afterClosed().subscribe(res => {
      if(res){
       this.showSaveFilter=false;
       this.showLoadFilter=false;
       this.getAllSearchFilter();
      }
    });
  }

  getAllSearchFilter(){
    this.searchFilterListViewModel.module='Staff';
    this.commonService.getAllSearchFilter(this.searchFilterListViewModel).subscribe((res) => {
      if (typeof (res) === 'undefined') {
        this.snackbar.open('Filter list failed. ' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
      else {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          this.searchFilterListViewModel.searchFilterList=[]
          if(!res.searchFilterList){
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
        }
        else {
          this.searchFilterListViewModel= res;
          let filterData=this.searchFilterListViewModel.searchFilterList.filter(x=> x.filterId == this.searchFilter.filterId);
          if(filterData.length >0){
            this.searchFilter.jsonList= filterData[0].jsonList;
          }
          if(this.filterJsonParams == null){
            this.searchFilter = this.searchFilterListViewModel.searchFilterList[this.searchFilterListViewModel.searchFilterList.length-1];
          }
        }
      }
    }
    );
  }

  editFilter(){
    this.showAdvanceSearchPanel = true;
    this.filterJsonParams = this.searchFilter;
    this.showSaveFilter = false;
    this.showLoadFilter=false;
  }

  deleteFilter(){
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: '400px',
      data: {
          title: 'Are you sure?',
          message: 'You are about to delete ' + this.searchFilter.filterName + '.'}
    });
    dialogRef.afterClosed().subscribe(dialogResult => {
      if (dialogResult){
        this.deleteFilterdata(this.searchFilter);
      }
   });
  }

  deleteFilterdata(filterData){
    this.searchFilterAddViewModel.searchFilter = filterData;
    this.commonService.deleteSearchFilter(this.searchFilterAddViewModel).subscribe(
      (res: SearchFilterAddViewModel) => {
        if (typeof(res) === 'undefined'){
          this.snackbar.open('' + this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
        else{
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
            this.snackbar.open('' + res._message, '', {
              duration: 10000
            });
          }
          else {
            this.searchFilter = new SearchFilter();
            this.showLoadFilter=true;
            this.getAllStaff.filterParams= null;
            this.getAllSearchFilter();
            this.callStaffList();
          }
        }
      }
    );
  }

  searchByFilterName(filter){
    this.searchFilter= filter;
    this.showLoadFilter=false;
    this.showSaveFilter=false;
    this.getAllStaff.filterParams = JSON.parse(filter.jsonList);
    this.getAllStaff.sortingModel = null;
    this.staffService.getAllStaffList(this.getAllStaff).subscribe(data => {
     if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
        if(data.staffMaster===null){
          this.staffList = new MatTableDataSource([]);   
          this.snackbar.open( data._message, '', {
            duration: 10000
          });
        } else{
          this.staffList = new MatTableDataSource([]);
        }
      }else{
        this.totalCount= data.totalCount;
        this.pageNumber = data.pageNumber;
        this.pageSize = data._pageSize;
        this.staffList = new MatTableDataSource(data.staffMaster);
        this.getAllStaff = new GetAllStaffModel();
      }
    });
  }

  ngOnDestroy(){
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }

}
