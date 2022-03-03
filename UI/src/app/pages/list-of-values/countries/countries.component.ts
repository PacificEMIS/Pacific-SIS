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

import { Component, OnInit, Input,ViewChild, AfterViewInit } from '@angular/core';
import icMoreVert from '@iconify/icons-ic/twotone-more-vert';
import icAdd from '@iconify/icons-ic/baseline-add';
import icEdit from '@iconify/icons-ic/twotone-edit';
import icDelete from '@iconify/icons-ic/twotone-delete';
import icSearch from '@iconify/icons-ic/search';
import icFilterList from '@iconify/icons-ic/filter-list';
import icImpersonate from '@iconify/icons-ic/twotone-account-circle';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router} from '@angular/router';
import { fadeInUp400ms } from '../../../../@vex/animations/fade-in-up.animation';
import { stagger40ms } from '../../../../@vex/animations/stagger.animation';
import { TranslateService } from '@ngx-translate/core';
import { LOVCountryModel } from '../../../models/country.model';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { LoaderService } from '../../../services/loader.service';
import { CommonService } from '../../../services/common.service';
import { EditCountryComponent } from './edit-country/edit-country.component';
import { MatPaginator, MatPaginatorIntl } from '@angular/material/paginator';
import { CountryAddModel } from '../../../models/country.model';
import { ConfirmDialogComponent } from '../../shared-module/confirm-dialog/confirm-dialog.component';
import { ExcelService } from '../../../services/excel.service';
import { SharedFunction } from '../../shared/shared-function';
import { RolePermissionListViewModel, RolePermissionViewModel } from 'src/app/models/roll-based-access.model';
import { CryptoService } from '../../../services/Crypto.service';
import { PageRolesPermission } from '../../../common/page-roles-permissions.service';
import { Permissions } from '../../../models/roll-based-access.model';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { FormControl } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'vex-countries',
  templateUrl: './countries.component.html',
  styleUrls: ['./countries.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})
export class CountriesComponent implements OnInit, AfterViewInit {
  columns = [
    { label: 'title', property: 'name', type: 'text', visible: true },
    { label: 'shortName', property: 'countryCode', type: 'text', visible: true },
    { label: 'createdBy', property: 'createdBy', type: 'text', visible: true },
    { label: 'createDate', property: 'createdOn', type: 'text', visible: true },
    { label: 'updatedBy', property: 'updatedBy', type: 'text', visible: true },
    { label: 'updateDate', property: 'updatedOn', type: 'text', visible: true },
    { label: 'actions', property: 'actions', type: 'text', visible: true }
  ];  
  icMoreVert = icMoreVert;
  icAdd = icAdd;
  icEdit = icEdit;
  icDelete = icDelete;
  icSearch = icSearch;
  icImpersonate = icImpersonate;
  icFilterList = icFilterList; 
  loading;  
  countryAddModel = new CountryAddModel();
  searchCtrl: FormControl;
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  getCountryModel: LOVCountryModel = new LOVCountryModel();
  CountryModelList: MatTableDataSource<any>;
  searchKey:string;
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  editPermission = false;
  deletePermission = false;
  addPermission = false;
  permissionListViewModel: RolePermissionListViewModel = new RolePermissionListViewModel();
  permissionGroup: RolePermissionViewModel = new RolePermissionViewModel();
  permissions: Permissions;
  filterParams: { columnName: any; filterValue: any; filterOption: number; }[];
  constructor(private router: Router,
    private dialog: MatDialog,
    public translateService:TranslateService,
    public loaderService:LoaderService,
    public commonService:CommonService,
    public snackbar:MatSnackBar,
    private excelService:ExcelService,
    public commonfunction:SharedFunction,
    private cryptoService: CryptoService,
    private pageRolePermissions: PageRolesPermission,
    private defaultValuesService: DefaultValuesService,
    private paginatorObj: MatPaginatorIntl,
    ) {
      paginatorObj.itemsPerPageLabel = translateService.instant('itemsPerPage');
    //translateService.use('en');
    this.loaderService.isLoading.subscribe((val) => {
      this.loading = val;
    });
    this.getCountryList();
  }

  ngOnInit(): void {
    this.searchCtrl = new FormControl();
    this.permissions = this.pageRolePermissions.checkPageRolePermission('/school/settings/lov-settings/countries')
  }

  getCountryList(){    
    this.getCountryModel.isListView = true;
    if (this.getCountryModel.sortingModel?.sortColumn === "") {
      this.getCountryModel.sortingModel = null;
    }
    this.commonService.LOVGetAllCountry(this.getCountryModel).subscribe(data => {
      if (typeof (data) == 'undefined') {
        this.snackbar.open('Country list failed. ' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }else{
       if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
         if (data.tableCountry === null) {
           this.CountryModelList = new MatTableDataSource([]);
           this.totalCount = null;
           this.snackbar.open('' + data._message, '', {
             duration: 10000
           });
         } else {
           this.CountryModelList = new MatTableDataSource([]);
           this.totalCount = null;
         }
       } else {
         this.totalCount = data.totalCount;
         this.pageNumber = data.pageNumber;
         this.pageSize = data._pageSize;
         this.CountryModelList = new MatTableDataSource(data.tableCountry);
         this.getCountryModel = new LOVCountryModel();
       }
      }
    });
  }

  translateKey(key) {
    let trnaslateKey;
   this.translateService.get(key).subscribe((res: string) => {
       trnaslateKey = res;
    });
    return trnaslateKey;
  }

  exportCountryListToExcel() {
    const getAllCountry: LOVCountryModel = new LOVCountryModel();
    getAllCountry.pageNumber = 0;
    getAllCountry.pageSize = 0;
    getAllCountry.sortingModel = null;
    this.commonService.LOVGetAllCountry(getAllCountry).subscribe(res => {
      if (res._failure) {
        this.commonService.checkTokenValidOrNot(res._message);
        this.snackbar.open('Failed to Export Country List.' + res._message, '', {
          duration: 10000
        });
      } else {
        if (res.tableCountry.length > 0) {
          let countryList;
          countryList = res.tableCountry?.map((item) => {
            return {
              [this.translateKey('title')]: item.name,
              [this.translateKey('shortName')]: item.countryCode,
              [this.translateKey('createdBy')]: item.createdBy ? item.createdBy : '-',
              [this.translateKey('createDate')]: this.commonfunction.transformDateWithTime(item.createdOn),
              [this.translateKey('updatedBy')]: item.updatedBy ? item.updatedBy : '-',
              [this.translateKey('updateDate')]: this.commonfunction.transformDateWithTime(item.updatedOn)
            }
          });
          this.excelService.exportAsExcelFile(countryList, 'Country_List_')
        } else {
          this.snackbar.open('No Records Found. Failed to Export Country List', '', {
            duration: 5000
          });
        }
      }
    });
  }

  getPageEvent(event) {
    if (this.sort.active != undefined && this.sort.direction != "") {
      this.getCountryModel.sortingModel.sortColumn = this.sort.active;
      this.getCountryModel.sortingModel.sortDirection = this.sort.direction;
    }
    if (this.searchCtrl.value != null && this.searchCtrl.value != "") {
      this.filterParams = [
        {
          columnName: null,
          filterValue: this.searchCtrl.value,
          filterOption: 3
        }
      ]
      Object.assign(this.getCountryModel, { filterParams: this.filterParams });
    }
    this.pageNumber = event.pageIndex + 1;
    this.getCountryModel.pageNumber = this.pageNumber;
    this.pageSize = event.pageSize;
    this.getCountryModel.pageSize = this.pageSize;
    this.defaultValuesService.setPageSize(event.pageSize);
    this.getCountryList();
  }

  ngAfterViewInit() {
    //  Sorting
    this.getCountryModel = new LOVCountryModel();
    this.sort.sortChange.subscribe((res) => {
      this.getCountryModel.pageNumber = this.pageNumber
      this.getCountryModel.pageSize = this.pageSize;
      this.getCountryModel.sortingModel.sortColumn = res.active;
      if (this.searchCtrl.value != null && this.searchCtrl.value != "") {
        this.filterParams = [
          {
            columnName: null,
            filterValue: this.searchCtrl.value,
            filterOption: 3
          }
        ]
        Object.assign(this.getCountryModel, { filterParams: this.filterParams });
      }
      if (res.direction == "") {
        this.getCountryModel.sortingModel = null;
        this.getCountryList();
        this.getCountryModel = new LOVCountryModel();
        this.getCountryModel.sortingModel = null;
      } else {
        this.getCountryModel.sortingModel.sortDirection = res.direction;
        this.getCountryList();
      }
    });

    //  Searching
    this.searchCtrl.valueChanges.pipe(debounceTime(500), distinctUntilChanged()).subscribe((term) => {
      if (term.trim().length > 0) {
         this.filterParams = [
          {
            columnName: null,
            filterValue: term,
            filterOption: 3
          }
        ]
        if (this.sort.active != undefined && this.sort.direction != "") {
          this.getCountryModel.sortingModel.sortColumn = this.sort.active;
          this.getCountryModel.sortingModel.sortDirection = this.sort.direction;
        }
        Object.assign(this.getCountryModel, { filterParams: this.filterParams });
        this.pageNumber = 1;
        this.getCountryModel.pageNumber = this.pageNumber;
        this.paginator.pageIndex = 0;
        this.getCountryModel.pageSize = this.pageSize;
        this.getCountryList();
      }
      else {
        this.filterParams = null;
        Object.assign(this.getCountryModel, { filterParams: this.filterParams });
        this.pageNumber = this.paginator.pageIndex + 1;
        this.getCountryModel.pageNumber = this.pageNumber;
        this.getCountryModel.pageSize = this.pageSize;
        if (this.sort.active != undefined && this.sort.direction != "") {
          this.getCountryModel.sortingModel.sortColumn = this.sort.active;
          this.getCountryModel.sortingModel.sortDirection = this.sort.direction;
        }
        this.getCountryList();
      }
    })
  }

  goToAdd(){
    this.dialog.open(EditCountryComponent, {      
      width: '500px'
    }).afterClosed().subscribe((data) => {
      if(data){
        this.getCountryModel.pageSize = this.pageSize;
        this.getCountryList();
      }            
    });   
  }

  deleteCountryData(element){
    this.countryAddModel.country.id = element;
    this.commonService.DeleteCountry(this.countryAddModel).subscribe(
      (res)=>{
        if(typeof(res)=='undefined'){
          this.snackbar.open('Country Deletion failed. ' + this.defaultValuesService.getHttpError(), '', {
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
            this.snackbar.open('' + res._message, '', {
              duration: 10000
            });
            this.getCountryModel.pageSize = this.pageSize;
            this.getCountryModel.pageNumber = this.pageNumber;
            Object.assign(this.getCountryModel, { filterParams: this.filterParams });
            this.getCountryList()
          }
        }
      }
    )
  }
  confirmDelete(element){
   
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: "400px",
      data: {
          title: "Are you sure?",
          message: "You are about to delete "+element.name+"."}
    });
    dialogRef.afterClosed().subscribe(dialogResult => {
      if(dialogResult){
        this.deleteCountryData(element.id);
      }
   });
  }
  goToEdit(editDetails){
    this.dialog.open(EditCountryComponent, {
      data: editDetails,  
      width: '500px'
    }).afterClosed().subscribe((data) => {
      if(data){
        this.getCountryModel.pageSize = this.pageSize;
        this.getCountryList();
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

  onSearchClear(){
    this.searchKey="";
    this.applyFilter();
  }

  applyFilter(){
    this.CountryModelList.filter = this.searchKey.trim().toLowerCase()
  }

}
