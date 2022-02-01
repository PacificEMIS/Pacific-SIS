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
import { EditSchoolClassificationComponent } from './edit-school-classification/edit-school-classification.component';
import {LovList,LovAddView} from '../../../models/lov.model';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { LoaderService } from '../../../services/loader.service';
import { CommonService } from '../../../services/common.service';
import { ConfirmDialogComponent } from '../../shared-module/confirm-dialog/confirm-dialog.component';
import { ExcelService } from '../../../services/excel.service';
import { SharedFunction } from '../../shared/shared-function';
import { RolePermissionListViewModel, RolePermissionViewModel } from 'src/app/models/roll-based-access.model';
import { CryptoService } from '../../../services/Crypto.service';
import { PageRolesPermission } from '../../../common/page-roles-permissions.service';
import { Permissions } from '../../../models/roll-based-access.model';
import { DefaultValuesService } from 'src/app/common/default-values.service';

@Component({
  selector: 'vex-school-classification',
  templateUrl: './school-classification.component.html',
  styleUrls: ['./school-classification.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})
export class SchoolClassificationComponent implements OnInit {
  columns = [
    { label: 'Title', property: 'lovColumnValue', type: 'text', visible: true },
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
  listCount; 
  totalCount:Number;pageNumber:Number;pageSize:Number;
  getAllClassification: LovList = new LovList(); 
  saveClassification: LovAddView = new LovAddView(); 
  ClassificationModelList: MatTableDataSource<any>;
  searchKey:string;
  schoolClassificationListForExcel=[];
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
    private cryptoService: CryptoService,
    private pageRolePermissions: PageRolesPermission,
    private defaultValuesService: DefaultValuesService
    ) {
    //translateService.use('en');
    this.loaderService.isLoading.subscribe((val) => {
      this.loading = val;
    });
    this.getSchoolClassificationList();
  }

  ngOnInit(): void {
    this.permissions = this.pageRolePermissions.checkPageRolePermission('/school/settings/lov-settings/school-classification')
    
  }

  getSchoolClassificationList(){
    this.getAllClassification.lovName="School Classification";
    this.getAllClassification.isListView = true;
    this.commonService.getAllDropdownValues(this.getAllClassification).subscribe(data => {
      if(typeof(data)=='undefined'){
        this.snackbar.open('No Record Found For School Classification. ' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }else{
       if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
          if (data.dropdownList == null) {
            this.ClassificationModelList= new MatTableDataSource(null);
            this.listCount=this.ClassificationModelList.data;
            this.snackbar.open( data._message, '', {
              duration: 10000
            });
          } else {
            this.ClassificationModelList= new MatTableDataSource(null);
            this.listCount=this.ClassificationModelList.data;
          }
        }
        else {     
          this.ClassificationModelList=new MatTableDataSource(data.dropdownList) ;
          this.schoolClassificationListForExcel = data.dropdownList;
          this.ClassificationModelList.sort=this.sort;           
          this.listCount=this.ClassificationModelList.data.length;
        }
      }
      
    });
  }

  goToAdd(){
    this.dialog.open(EditSchoolClassificationComponent, {     
      width: '500px'
    }).afterClosed().subscribe((data) => {
      if(data){
        this.getSchoolClassificationList();
      }            
    });   
  }
  confirmDelete(deleteDetails){
    
    // call our modal window
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: "400px",
      data: {
          title: "Are you sure?",
          message: "You are about to delete "+deleteDetails.lovColumnValue+"."}
    });
    // listen to response
    dialogRef.afterClosed().subscribe(dialogResult => {
      // if user pressed yes dialogResult will be true, 
      // if user pressed no - it will be false
      if(dialogResult){
        this.deleteClassification(deleteDetails);
      }
   });
  }
  deleteClassification(deleteDetails){
    this.saveClassification.dropdownValue.id=deleteDetails.id;          
    this.commonService.deleteDropdownValue(this.saveClassification).subscribe(data => {
      if (typeof (data) == 'undefined') {
        this.snackbar.open('School Classification Deletion failed. ' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
      else {
       if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
          this.snackbar.open('' + data._message, '', {
            duration: 10000
          });
        } else {
       
          this.snackbar.open(''+ data._message, '', {
            duration: 10000
          }).afterOpened().subscribe(data => {
            this.getSchoolClassificationList();
          });
          
        }
      }

    })
  }
  goToEdit(editDetails){
    this.dialog.open(EditSchoolClassificationComponent, {
      data: editDetails,
      width: '500px'
    }).afterClosed().subscribe((data) => {
      if(data){
        this.getSchoolClassificationList();
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
    this.ClassificationModelList.filter = this.searchKey.trim().toLowerCase()
  }

  translateKey(key) {
    let trnaslateKey;
   this.translateService.get(key).subscribe((res: string) => {
       trnaslateKey = res;
    });
    return trnaslateKey;
  }

  exportSchoolClassificationListToExcel(){
    if(this.schoolClassificationListForExcel.length!=0){
      let schoolClassificationList=this.schoolClassificationListForExcel?.map((item)=>{
        return{
          [this.translateKey('title')]: item.lovColumnValue,
          [this.translateKey('createdBy')]: item.createdBy ? item.createdBy: '-',
          [this.translateKey('createDate')]: this.commonfunction.transformDateWithTime(item.createdOn),
          [this.translateKey('updatedBy')]: item.updatedBy ? item.updatedBy: '-',
          [this.translateKey('updateDate')]:  this.commonfunction.transformDateWithTime(item.updatedOn)
        }
      });
      this.excelService.exportAsExcelFile(schoolClassificationList,'School_Classification_List_')
     }
     else{
    this.snackbar.open('No Records Found. Failed to Export School Classification List','', {
      duration: 5000
    });
  }
}

}
