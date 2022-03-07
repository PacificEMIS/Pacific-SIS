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

import { Component, OnInit, Input, ViewChild } from '@angular/core';
import icMoreVert from '@iconify/icons-ic/twotone-more-vert';
import icAdd from '@iconify/icons-ic/baseline-add';
import icEdit from '@iconify/icons-ic/twotone-edit';
import icDelete from '@iconify/icons-ic/twotone-delete';
import icSearch from '@iconify/icons-ic/search';
import icFilterList from '@iconify/icons-ic/filter-list';
import icImport from '@iconify/icons-ic/twotone-unarchive';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router} from '@angular/router';
import { fadeInUp400ms } from '../../../../@vex/animations/fade-in-up.animation';
import { stagger40ms } from '../../../../@vex/animations/stagger.animation';
import { TranslateService } from '@ngx-translate/core';
import { EditCommonToiletAccessibilityComponent } from './edit-common-toilet-accessibility/edit-common-toilet-accessibility.component';
import { LovAddView, LovList } from '../../../models/lov.model';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { MatPaginator, MatPaginatorIntl } from '@angular/material/paginator';
import { ExcelService } from '../../../services/excel.service';
import { CommonService } from '../../../services/common.service';
import { LoaderService } from '../../../services/loader.service';
import { SharedFunction } from '../../shared/shared-function';
import { ConfirmDialogComponent } from '../../shared-module/confirm-dialog/confirm-dialog.component';
import { RolePermissionListViewModel, RolePermissionViewModel } from 'src/app/models/roll-based-access.model';
import { CryptoService } from '../../../services/Crypto.service';
import { PageRolesPermission } from '../../../common/page-roles-permissions.service';
import { Permissions } from '../../../models/roll-based-access.model';
import { DefaultValuesService } from 'src/app/common/default-values.service';

@Component({
  selector: 'vex-common-toilet-accessibility',
  templateUrl: './common-toilet-accessibility.component.html',
  styleUrls: ['./common-toilet-accessibility.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})
export class CommonToiletAccessibilityComponent implements OnInit {
  columns = [
    { label: 'accessibilityName', property: 'lovColumnValue', type: 'text', visible: true },
    { label: 'createdBy', property: 'createdBy', type: 'text', visible: true },
    { label: 'createdDate', property: 'createdOn', type: 'text', visible: true },
    { label: 'updatedBy', property: 'updatedBy', type: 'text', visible: true },
    { label: 'updatedDate', property: 'updatedOn', type: 'text', visible: true },
    { label: 'actions', property: 'actions', type: 'text', visible: true }
  ];

  EffortGradeScaleModelList;

  icMoreVert = icMoreVert;
  icAdd = icAdd;
  icEdit = icEdit;
  icDelete = icDelete;
  icSearch = icSearch;
  icImport = icImport;
  icFilterList = icFilterList;
  loading:Boolean;
  searchKey:string;
  lovList:LovList= new LovList();
  lovAddView:LovAddView=new LovAddView();
  lovName="Common Toilet Accessibility";
  commonToiletAccessibilityList: MatTableDataSource<any>;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  listCount;
  editPermission = false;
  deletePermission = false;
  addPermission = false;
  permissionListViewModel: RolePermissionListViewModel = new RolePermissionListViewModel();
  permissionGroup: RolePermissionViewModel = new RolePermissionViewModel();
  permissions: Permissions;
  constructor(
    private router: Router,
    private dialog: MatDialog,
    public translateService:TranslateService,
    private snackbar: MatSnackBar,
    private excelService:ExcelService,
    private commonService:CommonService,
    private loaderService:LoaderService,
    public commonfunction:SharedFunction,
    private pageRolePermissions: PageRolesPermission,
    private defaultValuesService: DefaultValuesService,
    private paginatorObj: MatPaginatorIntl,
    ) {
      paginatorObj.itemsPerPageLabel = translateService.instant('itemsPerPage');
    //translateService.use('en');
    this.loaderService.isLoading.subscribe((val) => {
      this.loading = val;
    }); 
  }

  ngOnInit(): void {
    this.permissions = this.pageRolePermissions.checkPageRolePermission('/school/settings/lov-settings/common-toilet-accessibility')
    
    this.getAllDropdownValues();
  }

  getPageEvent(event){    
    // this.getAllSchool.pageNumber=event.pageIndex+1;
    // this.getAllSchool.pageSize=event.pageSize;
    // this.callAllSchool(this.getAllSchool);
  }

  goToAdd(){
    this.dialog.open(EditCommonToiletAccessibilityComponent, {
      data:{mod:0},
      width: '500px'
    }).afterClosed().subscribe((data)=>{
      if(data==='submited'){
        this.getAllDropdownValues()
      }
    })
  }
  goToEdit(element){
    this.dialog.open(EditCommonToiletAccessibilityComponent, {
      data:{mod:1,element},
      width: '500px'
    }).afterClosed().subscribe((data)=>{
      if(data==='submited'){
        this.getAllDropdownValues()
      }
    })
  }

  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }
  onSearchClear(){
    this.searchKey="";
    this.applyFilter();
  }
  applyFilter(){
    this.commonToiletAccessibilityList.filter = this.searchKey.trim().toLowerCase()
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  deleteCommonToiletAccessibilitydata(element){
    this.lovAddView.dropdownValue.id=element.id
    this.commonService.deleteDropdownValue(this.lovAddView).subscribe(
      (res:LovAddView)=>{
        if(typeof(res)=='undefined'){
          this.snackbar.open( this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
        else{
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
            this.snackbar.open( res._message, '', {
              duration: 10000
            });
          } 
          else { 
            this.snackbar.open( res._message, '', {
              duration: 10000
            });
            this.getAllDropdownValues()
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
          message: "You are about to delete "+element.lovColumnValue+"."}
    });
    dialogRef.afterClosed().subscribe(dialogResult => {
      if(dialogResult){
        this.deleteCommonToiletAccessibilitydata(element);
      }
   });
  }
  getAllDropdownValues(){
    this.lovList.lovName=this.lovName;
    this.lovList.isListView = true;
    this.commonService.getAllDropdownValues(this.lovList).subscribe(
      (res:LovList)=>{
        if(typeof(res)=='undefined'){
          this.snackbar.open('' + this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
        else{
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);  
            if (res.dropdownList == null) {
              this.commonToiletAccessibilityList= new MatTableDataSource(null);
              this.listCount=this.commonToiletAccessibilityList.data;
              this.snackbar.open( res._message, '', {
                duration: 10000
              });
            } else {
              this.commonToiletAccessibilityList= new MatTableDataSource(null);
              this.listCount=this.commonToiletAccessibilityList.data;
            }
          } 
          else{
            this.commonToiletAccessibilityList=new MatTableDataSource(res.dropdownList) ;
            this.commonToiletAccessibilityList.sort=this.sort;    
            this.commonToiletAccessibilityList.paginator=this.paginator;
            this.listCount=this.commonToiletAccessibilityList.data.length;
          }
        }
      }
    );
  }

  translateKey(key) {
    let trnaslateKey;
   this.translateService.get(key).subscribe((res: string) => {
       trnaslateKey = res;
    });
    return trnaslateKey;
  }

  exportToExcel(){
    if (this.commonToiletAccessibilityList.data?.length > 0) {
      let reportList = this.commonToiletAccessibilityList.data?.map((x) => {
        return {
          [this.translateKey('accessibilityName')]: x.lovColumnValue,
          [this.translateKey('createdBy')]: x.createdBy ? x.createdBy: '-',
          [this.translateKey('createDate')]: this.commonfunction.transformDateWithTime(x.createdOn),
          [this.translateKey('updatedBy')]: x.updatedBy ? x.updatedBy: '-',
          [this.translateKey('updateDate')]:  this.commonfunction.transformDateWithTime(x.updatedOn)
        }
      });
      this.excelService.exportAsExcelFile(reportList,"Common_Toilet_Accessibility_List_")
    } else {
      this.snackbar.open('No records found. failed to export common toilet accessibility list', '', {
        duration: 5000
      });
    }
  }
}
