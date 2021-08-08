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
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { fadeInUp400ms } from '../../../../@vex/animations/fade-in-up.animation';
import { stagger40ms } from '../../../../@vex/animations/stagger.animation';
import { TranslateService } from '@ngx-translate/core';
import { EditSchoolFieldsComponent } from './edit-school-fields/edit-school-fields.component';
import { SchoolFieldsCategoryComponent } from './school-fields-category/school-fields-category.component';
import { CustomFieldService } from '../../../services/custom-field.service';
import {CustomFieldAddView, CustomFieldDragDropModel, CustomFieldListViewModel} from '../../../models/custom-field.model';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { ConfirmDialogComponent } from '../../shared-module/confirm-dialog/confirm-dialog.component';
import { LoaderService } from '../../../services/loader.service';
import { FieldsCategoryListView, FieldsCategoryAddView } from '../../../models/fields-category.model';
import {FieldCategoryModuleEnum} from '../../../enums/field-category-module.enum';
import { CdkDragDrop} from '@angular/cdk/drag-drop';
import { CryptoService } from '../../../services/Crypto.service';
import { Permissions, RolePermissionListViewModel, RolePermissionViewModel } from '../../../models/roll-based-access.model';
import { SchoolService } from '../../../services/school.service';
import { PageRolesPermission } from '../../../common/page-roles-permissions.service';
import { CommonService } from 'src/app/services/common.service';

@Component({
  selector: 'vex-school-fields',
  templateUrl: './school-fields.component.html',
  styleUrls: ['./school-fields.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})
export class SchoolFieldsComponent implements OnInit {
  
  columns = [
    /* { label: '', property: 'type', type: 'text', visible: true }, */
    { label: 'id', property: 'fieldId', type: 'text', visible: true },
    { label: 'fieldName', property: 'title', type: 'text', visible: true },
    { label: 'fieldType', property: 'type', type: 'text', visible: true },
    { label: 'inUsed', property: 'hide', type: 'checkbox', visible: true },
    { label: 'action', property: 'action', type: 'text', visible: true }
   ];

  EnrollmentCodeModelList;
  fieldsCategoryList;
  currentCategoryId=null;
  fieldCategoryModuleEnum = FieldCategoryModuleEnum;

  icMoreVert = icMoreVert;
  icAdd = icAdd;
  icEdit = icEdit;
  icDelete = icDelete;
  icSearch = icSearch;
  icFilterList = icFilterList;
  loading: boolean;
  searchKey:string;
  customFieldListViewModel:CustomFieldListViewModel= new CustomFieldListViewModel();
  customFieldAddView:CustomFieldAddView= new CustomFieldAddView()
  fieldsCategoryListView:FieldsCategoryListView= new FieldsCategoryListView();
  fieldsCategoryAddView:FieldsCategoryAddView= new FieldsCategoryAddView();
  customFieldDragDropModel:CustomFieldDragDropModel= new CustomFieldDragDropModel();
  permissions: Permissions;
  constructor(
    private snackbar: MatSnackBar,
    private dialog: MatDialog,
    public translateService:TranslateService,
    private customFieldservice:CustomFieldService,
    private loaderService:LoaderService,
    private schoolService: SchoolService,
    private pageRolePermissions: PageRolesPermission,
    private commonService: CommonService,
    ) {
    //translateService.use('en');
    this.loaderService.isLoading.subscribe((val) => {
      this.loading = val;
    }); 
  }
  customFieldList: MatTableDataSource<any>;
  @ViewChild(MatSort) sort: MatSort;
  onSearchClear(){
    this.searchKey="";
    this.applyFilter();
  }
  applyFilter(){
    this.customFieldList.filter = this.searchKey.trim().toLowerCase()
  }
  ngOnInit(): void {
    this.permissions = this.pageRolePermissions.checkPageRolePermission('/school/settings/school-settings/school-fields')
    this.getAllCustomFieldCategory()
  }
  selectCategory(element){
    this.currentCategoryId=element.categoryId;
    this.customFieldList=new MatTableDataSource(element.customFields) ;
    this.customFieldList.sort=this.sort;
  }
   goToAdd(){   
    this.dialog.open(EditSchoolFieldsComponent, {
      data: {information: null,categoryID:this.currentCategoryId},
      width: '600px'
    }).afterClosed().subscribe((data)=>{
      if(data==='submited'){
        this.schoolService.changeSchoolListStatus({schoolLoaded:false,schoolChanged:true,dataFromUserLogin:false,academicYearChanged:false,academicYearLoaded:false});
        this.getAllCustomFieldCategory();
      }
    });
   }

   goToAddCategory(){   
    this.dialog.open(SchoolFieldsCategoryComponent, {
      width: '500px'
    }).afterClosed().subscribe((data)=>{
      if(data==='submited'){
        this.schoolService.changeSchoolListStatus({schoolLoaded:false,schoolChanged:true,dataFromUserLogin:false,academicYearChanged:false,academicYearLoaded:false});
        this.getAllCustomFieldCategory();
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

  deleteCustomFieldata(element){
    this.customFieldAddView.customFields=element
    this.customFieldservice.deleteCustomField(this.customFieldAddView).subscribe(
      (res:CustomFieldAddView)=>{
        if(typeof(res)=='undefined'){
          this.snackbar.open('Custom Field failed. ' + sessionStorage.getItem("httpError"), '', {
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
            this.getAllCustomFieldCategory()
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
          message: "You are about to delete "+element.title+"."}
    });
    dialogRef.afterClosed().subscribe(dialogResult => {
      if(dialogResult){
        this.schoolService.changeSchoolListStatus({schoolLoaded:false,schoolChanged:true,dataFromUserLogin:false,academicYearChanged:false,academicYearLoaded:false});
        this.deleteCustomFieldata(element);
      }
   });
}
openEditdata(element){
  this.dialog.open(EditSchoolFieldsComponent, {
    data: {information:element},
      width: '800px'
  }).afterClosed().subscribe((data)=>{
    if(data==='submited'){
      this.schoolService.changeSchoolListStatus({schoolLoaded:false,schoolChanged:true,dataFromUserLogin:false,academicYearChanged:false,academicYearLoaded:false});
      this.getAllCustomFieldCategory();
    }
  })
}
getAllCustomFieldCategory(){
  this.fieldsCategoryListView.module=this.fieldCategoryModuleEnum.School;
  this.customFieldservice.getAllFieldsCategory(this.fieldsCategoryListView).subscribe(
    (res:FieldsCategoryListView)=>{
      if(typeof(res)=='undefined'){
        this.snackbar.open('Field Category list failed. ' + sessionStorage.getItem("httpError"), '', {
          duration: 10000
        });
      }
      else{
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          this.fieldsCategoryList = [];
          if (!res.fieldsCategoryList) {
            this.snackbar.open( res._message, '', {
              duration: 10000
            });
          }
        } 
        else{
          this.fieldsCategoryList= res.fieldsCategoryList
          if(this.currentCategoryId==null){
            this.currentCategoryId=res.fieldsCategoryList[0].categoryId  
            this.customFieldList=new MatTableDataSource(res.fieldsCategoryList[0].customFields) ;
            this.customFieldList.sort=this.sort;
          }
          else{
            let index = this.fieldsCategoryList.findIndex((x) => {
              return x.categoryId === this.currentCategoryId
            });
            this.customFieldList=new MatTableDataSource(res.fieldsCategoryList[index].customFields) ;
            this.customFieldList.sort=this.sort;
          }
        }
      }
    }
  );
}
editFieldCategory(element){
  this.dialog.open(SchoolFieldsCategoryComponent,{ 
    data: element,
    width: '800px'
  }).afterClosed().subscribe((data)=>{
    if (data === 'submited'){
      this.schoolService.changeSchoolListStatus({schoolLoaded:false,schoolChanged:true,dataFromUserLogin:false,academicYearChanged:false,academicYearLoaded:false});
      this.getAllCustomFieldCategory();
    }
  })
}
deleteFieldCategory(element){
  this.fieldsCategoryAddView.fieldsCategory = element;
  this.customFieldservice.deleteFieldsCategory(this.fieldsCategoryAddView).subscribe(
    (res: FieldsCategoryAddView)=>{
      if(typeof(res) === 'undefined'){
        this.snackbar.open('Field Category delete failed. ' + sessionStorage.getItem("httpError"), '', {
          duration: 10000
        });
      }
      else{
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          this.snackbar.open(  res._message, '', {
            duration: 10000
          });
        }
        else{
          this.snackbar.open('' + res._message, '', {
            duration: 10000
          });
          if (element.categoryId === this.currentCategoryId){
            this.currentCategoryId = null;
          }
          this.getAllCustomFieldCategory();
        }
      }
    }
  );
}
confirmDeleteFieldCategory(element){
  const dialogRef = this.dialog.open(ConfirmDialogComponent, {
    maxWidth: '400px',
    data: {
        title: 'Are you sure?',
        message: 'You are about to delete ' + element.title + '.' }
  });
  dialogRef.afterClosed().subscribe(dialogResult => {
    if (dialogResult){
      this.deleteFieldCategory(element);
    }
 });
}

  drop(event: CdkDragDrop<string[]>) {
    this.customFieldDragDropModel.categoryId = this.currentCategoryId;
    this.customFieldDragDropModel.currentSortOrder = this.customFieldList.data[event.currentIndex].sortOrder;
    this.customFieldDragDropModel.previousSortOrder = this.customFieldList.data[event.previousIndex].sortOrder;
    this.customFieldservice.updateCustomFieldSortOrder(this.customFieldDragDropModel).subscribe(
      (res: CustomFieldDragDropModel) => {
        if (typeof(res) === 'undefined'){
          this.snackbar.open('Custom Field Drag short failed. ' + sessionStorage.getItem('httpError'), '', {
            duration: 10000
          });
        }else{
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
            this.snackbar.open( res._message, '', {
              duration: 10000
            });
          }
          else{
            this.getAllCustomFieldCategory();
          }
        }
      }
    );
  }
}
