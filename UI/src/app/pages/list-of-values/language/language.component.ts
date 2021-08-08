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

import { Component, OnInit, Input ,ViewChild} from '@angular/core';
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
import {LanguageModel} from '../../../models/language.model';
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

@Component({
  selector: 'vex-language',
  templateUrl: './language.component.html',
  styleUrls: ['./language.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})
export class LanguageComponent implements OnInit {
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
  totalCount:Number;pageNumber:Number;pageSize:Number;
  languageModel: LanguageModel = new LanguageModel();  
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
  constructor(private router: Router,
    private dialog: MatDialog,
    public translateService:TranslateService,
    public loaderService:LoaderService,
    public commonService:CommonService,
    public snackbar:MatSnackBar,
    private excelService:ExcelService,
    public commonfunction:SharedFunction,
    private pageRolePermissions: PageRolesPermission
    ) {
    //translateService.use('en');
    this.loaderService.isLoading.subscribe((val) => {
      this.loading = val;
    });
    this.getLanguageList();
  }

  ngOnInit(): void { 
    this.permissions = this.pageRolePermissions.checkPageRolePermission('/school/settings/lov-settings/language')
   }

  deleteLanguageData(element){
    this.languageAddModel._tenantName = sessionStorage.getItem("tenant");
    this.languageAddModel._userName = sessionStorage.getItem("user");
    this.languageAddModel._token = sessionStorage.getItem("token");
    this.languageAddModel.language.langId = element;
    this.commonService.DeleteLanguage(this.languageAddModel).subscribe(
      (res)=>{
        if(typeof(res)=='undefined'){
          this.snackbar.open('Language Deletion failed. ' + sessionStorage.getItem("httpError"), '', {
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
    this.languageModel._tenantName = sessionStorage.getItem("tenant");  
    this.languageModel._userName = sessionStorage.getItem("user");
    this.commonService.GetAllLanguage(this.languageModel).subscribe(data => {
      if (typeof (data) == 'undefined') {
        this.snackbar.open('Language list failed. ' + sessionStorage.getItem("httpError"), '', {
          duration: 10000
        });
      }else{
       if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
          if(data.tableLanguage==null){
            this.languageModelList = new MatTableDataSource([]);
            this.languageModelList.sort=this.sort;
            this.languageModelList.paginator = this.paginator;  
            this.snackbar.open( data._message, '', {
              duration: 10000
            });
          }
          else{
            this.languageModelList = new MatTableDataSource([]);
            this.languageModelList.sort=this.sort;
            this.languageModelList.paginator = this.paginator;  
          }
        }else{ 
          this.languageModelList = new MatTableDataSource(data.tableLanguage);
          this.languageListForExcel= data.tableLanguage;
          this.languageModelList.sort=this.sort; 
          this.languageModelList.paginator = this.paginator;         
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

  exportLanguageListToExcel(){
    if(this.languageListForExcel.length!=0){
      let languageList=this.languageListForExcel?.map((item)=>{
        return{
          [this.translateKey('title')]: item.locale,
          [this.translateKey('shortCode')]: item.languageCode,
          [this.translateKey('createdBy')]: item.createdBy ? item.createdBy: '-',
          [this.translateKey('createDate')]: this.commonfunction.transformDateWithTime(item.createdOn),
          [this.translateKey('updatedBy')]: item.updatedBy ? item.updatedBy: '-',
          [this.translateKey('updateDate')]:  this.commonfunction.transformDateWithTime(item.updatedOn)
        }
      });
      this.excelService.exportAsExcelFile(languageList,'Language_List_')
     }
     else{
    this.snackbar.open('No Records Found. Failed to Export Language List','', {
      duration: 5000
    });
  }
}

  goToAdd(){
    this.dialog.open(EditLanguageComponent, {
      
      width: '500px'
    }).afterClosed().subscribe((data) => {
      if(data){
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
