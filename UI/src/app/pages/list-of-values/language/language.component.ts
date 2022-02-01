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

import { Component, OnInit, Input ,ViewChild, AfterViewInit} from '@angular/core';
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
import { EditLanguageComponent } from './edit-language/edit-language.component';
import { LOVLanguageModel} from '../../../models/language.model';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { LoaderService } from '../../../services/loader.service';
import { CommonService } from '../../../services/common.service';
import { MatPaginator } from '@angular/material/paginator';
import {LanguageAddModel} from '../../../models/language.model';
import { ConfirmDialogComponent } from '../../shared-module/confirm-dialog/confirm-dialog.component';
import { ExcelService } from '../../../services/excel.service';
import { SharedFunction } from '../../shared/shared-function';
import { RolePermissionListViewModel, RolePermissionViewModel } from 'src/app/models/roll-based-access.model';
import { CryptoService } from '../../../services/Crypto.service';
import { Permissions } from '../../../models/roll-based-access.model';
import { PageRolesPermission } from '../../../common/page-roles-permissions.service';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { FormControl } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'vex-language',
  templateUrl: './language.component.html',
  styleUrls: ['./language.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})
export class LanguageComponent implements OnInit, AfterViewInit {
  columns = [
    { label: 'Title', property: 'locale', type: 'text', visible: true },
    { label: 'Short Code', property: 'languageCode', type: 'text', visible: true },
    { label: 'Created By', property: 'createdBy', type: 'text', visible: true },
    { label: 'Create Date', property: 'createdOn', type: 'text', visible: true },
    { label: 'Updated By', property: 'updatedBy', type: 'text', visible: true },
    { label: 'Update Date', property: 'updatedOn', type: 'text', visible: true },
    { label: 'Actions', property: 'actions', type: 'text', visible: true }
  ];

  SchoolClassificationModelList;

  icMoreVert = icMoreVert;
  icAdd = icAdd;
  icEdit = icEdit;
  icDelete = icDelete;
  icSearch = icSearch;
  icImpersonate = icImpersonate;
  icFilterList = icFilterList; 
  loading;  
  searchCtrl: FormControl
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  languageModel: LOVLanguageModel = new LOVLanguageModel();
  languageAddModel = new LanguageAddModel();
  languageModelList: MatTableDataSource<any>;
  languageListForExcel = [];
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
    private pageRolePermissions: PageRolesPermission,
    private defaultValuesService: DefaultValuesService
    ) {
    //translateService.use('en');
    this.loaderService.isLoading.subscribe((val) => {
      this.loading = val;
    });
    this.getLanguageList();
  }

  ngOnInit(): void { 
    this.searchCtrl = new FormControl();
    this.permissions = this.pageRolePermissions.checkPageRolePermission('/school/settings/lov-settings/language')
   }

  deleteLanguageData(element){
    this.languageAddModel._tenantName = this.defaultValuesService.getTenantName();
    this.languageAddModel._userName = this.defaultValuesService.getUserName();
    this.languageAddModel._token = this.defaultValuesService.getToken();
    this.languageAddModel.language.langId = element;
    this.commonService.DeleteLanguage(this.languageAddModel).subscribe(
      (res)=>{
        if(typeof(res)=='undefined'){
          this.snackbar.open('Language Deletion failed. ' + this.defaultValuesService.getHttpError(), '', {
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
            this.languageModel.pageSize = this.pageSize;
            this.languageModel.pageNumber = this.pageNumber;
            Object.assign(this.languageModel, { filterParams: this.filterParams });
            this.getLanguageList()
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
          message: "You are about to delete "+element.locale+"."}
    });
    dialogRef.afterClosed().subscribe(dialogResult => {
      if(dialogResult){
        this.deleteLanguageData(element.langId);
      }
   });
  }
  getLanguageList(){
    this.languageModel.isListView = true;
    if (this.languageModel.sortingModel?.sortColumn === "") {
      this.languageModel.sortingModel = null;
    }
    this.commonService.LOVGetAllLanguage(this.languageModel).subscribe(data => {
      if (typeof (data) == 'undefined') {
        this.snackbar.open('Language list failed. ' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }else{
       if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
         if (data.tableLanguage === null) {
           this.languageModelList = new MatTableDataSource([]);
           this.totalCount = null;
           this.snackbar.open(data._message, '', {
             duration: 10000
           });
         }
         else {
           this.languageModelList = new MatTableDataSource([]);
           this.totalCount = null;
         }
       } else {
         this.totalCount = data.totalCount;
         this.pageNumber = data.pageNumber;
         this.pageSize = data._pageSize;
         this.languageModelList = new MatTableDataSource(data.tableLanguage);
         this.languageModel = new LOVLanguageModel();
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

  exportLanguageListToExcel() {
    const getAllLanguage: LOVLanguageModel = new LOVLanguageModel();
    getAllLanguage.pageNumber = 0;
    getAllLanguage.pageSize = 0;
    getAllLanguage.sortingModel = null;
    this.commonService.LOVGetAllLanguage(getAllLanguage).subscribe(res => {
      if (res._failure) {
        this.commonService.checkTokenValidOrNot(res._message);
        this.snackbar.open('Failed to Export Language List.' + res._message, '', {
          duration: 10000
        });
      } else {
        if (res.tableLanguage.length > 0) {
          let languageList;
          languageList = res.tableLanguage?.map((item) => {
            return {
              [this.translateKey('title')]: item.locale,
              [this.translateKey('shortCode')]: item.languageCode,
              [this.translateKey('createdBy')]: item.createdBy ? item.createdBy : '-',
              [this.translateKey('createDate')]: this.commonfunction.transformDateWithTime(item.createdOn),
              [this.translateKey('updatedBy')]: item.updatedBy ? item.updatedBy : '-',
              [this.translateKey('updateDate')]: this.commonfunction.transformDateWithTime(item.updatedOn)
            }
          });
          this.excelService.exportAsExcelFile(languageList, 'Language_List_')
        } else {
          this.snackbar.open('No Records Found. Failed to Export Language List', '', {
            duration: 5000
          });
        }
      }
    });
  }

  getPageEvent(event) {
    if (this.sort.active != undefined && this.sort.direction != "") {
      this.languageModel.sortingModel.sortColumn = this.sort.active;
      this.languageModel.sortingModel.sortDirection = this.sort.direction;
    }
    if (this.searchCtrl.value != null && this.searchCtrl.value != "") {
      this.filterParams = [
        {
          columnName: null,
          filterValue: this.searchCtrl.value,
          filterOption: 3
        }
      ]
      Object.assign(this.languageModel, { filterParams: this.filterParams });
    }
    this.pageNumber = event.pageIndex + 1;

    this.languageModel.pageNumber = this.pageNumber;
    this.pageSize = event.pageSize;
    this.languageModel.pageSize = this.pageSize;
    this.defaultValuesService.setPageSize(event.pageSize);
    this.getLanguageList();
  }

  ngAfterViewInit() {
    //  Sorting
    this.languageModel = new LOVLanguageModel();
    this.sort.sortChange.subscribe((res) => {
      this.languageModel.pageNumber = this.pageNumber
      this.languageModel.pageSize = this.pageSize;
      this.languageModel.sortingModel.sortColumn = res.active;
      if (this.searchCtrl.value != null && this.searchCtrl.value != "") {
        this.filterParams = [
          {
            columnName: null,
            filterValue: this.searchCtrl.value,
            filterOption: 3
          }
        ]
        Object.assign(this.languageModel, { filterParams: this.filterParams });
      }
      if (res.direction == "") {
        this.languageModel.sortingModel = null;
        this.getLanguageList();
        this.languageModel = new LOVLanguageModel();
        this.languageModel.sortingModel = null;
      } else {
        this.languageModel.sortingModel.sortDirection = res.direction;
        this.getLanguageList();
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
          this.languageModel.sortingModel.sortColumn = this.sort.active;
          this.languageModel.sortingModel.sortDirection = this.sort.direction;
        }
        Object.assign(this.languageModel, { filterParams: this.filterParams });
        this.pageNumber = 1;
        this.languageModel.pageNumber = this.pageNumber;
        this.paginator.pageIndex = 0;
        this.languageModel.pageSize = this.pageSize;
        this.getLanguageList();
      }
      else {
      this.filterParams = null;
        Object.assign(this.languageModel, { filterParams: null });
        this.pageNumber = this.paginator.pageIndex + 1;
        this.languageModel.pageNumber = this.pageNumber;
        this.languageModel.pageSize = this.pageSize;
        if (this.sort.active != undefined && this.sort.direction != "") {
          this.languageModel.sortingModel.sortColumn = this.sort.active;
          this.languageModel.sortingModel.sortDirection = this.sort.direction;
        }
        this.getLanguageList();
      }
    })
  }

  goToAdd(){
    this.dialog.open(EditLanguageComponent, {
      
      width: '500px'
    }).afterClosed().subscribe((data) => {
      if(data){
        this.languageModel.pageSize = this.pageSize;
        this.getLanguageList();
      }            
    });   
  }

  goToEdit(editDetails){
    this.dialog.open(EditLanguageComponent, {
      data: editDetails,
      width: '500px'
    }).afterClosed().subscribe((data) => {
      if(data){
        this.languageModel.pageSize = this.pageSize;
        this.getLanguageList();
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
    this.languageModelList.filter = this.searchKey.trim().toLowerCase()
  }

}
