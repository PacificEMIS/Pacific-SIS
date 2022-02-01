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
import { Router} from '@angular/router';
import { fadeInUp400ms } from '../../../../@vex/animations/fade-in-up.animation';
import { stagger40ms } from '../../../../@vex/animations/stagger.animation';
import { TranslateService } from '@ngx-translate/core';
import { EditStudentFieldsComponent } from './edit-student-fields/edit-student-fields.component';
import { StudentFieldsCategoryComponent } from './student-fields-category/student-fields-category.component';
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
import { DefaultValuesService } from '../../../common/default-values.service';
import { SchoolService } from '../../../services/school.service';
import { PageRolesPermission } from '../../../common/page-roles-permissions.service';
import { CommonService } from 'src/app/services/common.service';
import { Module } from 'src/app/enums/module.enum';


@Component({
  selector: 'vex-student-fields',
  templateUrl: './student-fields.component.html',
  styleUrls: ['./student-fields.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})
export class StudentFieldsComponent implements OnInit {
  columns = [
    /* { label: '', property: 'type', type: 'text', visible: true }, */
    { label: 'id', property: 'fieldId', type: 'text', visible: true },
    { label: 'fieldName', property: 'title', type: 'text', visible: true },
    { label: 'fieldType', property: 'type', type: 'text', visible: true },
    { label: 'inUsed', property: 'hide', type: 'checkbox', visible: true },
    { label: 'action', property: 'action', type: 'text', visible: true }
   ];

  StudentFieldsModelList;
  fieldsCategoryList;
  currentCategoryId = null;
  fieldCategoryModuleEnum = FieldCategoryModuleEnum;
  restrictedCategoryid = [5, 6, 8, 9, 10]; // All the catagory where Custom field cannot insert
  icMoreVert = icMoreVert;
  icAdd = icAdd;
  icEdit = icEdit;
  icDelete = icDelete;
  icSearch = icSearch;
  icFilterList = icFilterList;
  loading: boolean;
  searchKey: string;
  customFieldListViewModel: CustomFieldListViewModel = new CustomFieldListViewModel();
  customFieldAddView: CustomFieldAddView = new CustomFieldAddView();
  fieldsCategoryListView: FieldsCategoryListView = new FieldsCategoryListView();
  fieldsCategoryAddView: FieldsCategoryAddView = new FieldsCategoryAddView();
  customFieldDragDropModel: CustomFieldDragDropModel = new CustomFieldDragDropModel();
  noSortIndexContainer=[0,1,2,3,4]
  permissions: Permissions;
  constructor(
    private router: Router,
    private snackbar: MatSnackBar,
    private dialog: MatDialog,
    public translateService: TranslateService,
    private customFieldservice: CustomFieldService,
    private loaderService: LoaderService,
    private defaultValuesService: DefaultValuesService,
    private pageRolePermissions: PageRolesPermission,
    private schoolService:SchoolService,
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
    this.searchKey = '';
    this.applyFilter();
  }
  applyFilter(){
    this.customFieldList.filter = this.searchKey.trim().toLowerCase();
  }

  ngOnInit(): void {
    this.permissions = this.pageRolePermissions.checkPageRolePermission('/school/settings/student-settings/student-fields')
    this.getAllCustomFieldCategory();
  }
  selectCategory(element){
    this.currentCategoryId = element.categoryId;
    this.customFieldList = new MatTableDataSource(element.customFields) ;
    this.customFieldList.sort = this.sort;
  }

   goToAdd(){
    this.dialog.open(EditStudentFieldsComponent, {
      data: {categoryID: this.currentCategoryId},
      width: '500px'
    }).afterClosed().subscribe((data) => {
      if (data === 'submited'){
        this.schoolService.changeSchoolListStatus({schoolLoaded:false,schoolChanged:true,dataFromUserLogin:false,academicYearChanged:false,academicYearLoaded:false});
        this.getAllCustomFieldCategory();
      }
    });
   }
   goToAddCategory(){
    this.dialog.open(StudentFieldsCategoryComponent, {
      width: '500px'
    }).afterClosed().subscribe((data) => {
      if (data === 'submited'){
        this.schoolService.changeSchoolListStatus({schoolLoaded:false,schoolChanged:true,dataFromUserLogin:false,academicYearChanged:false,academicYearLoaded:false});
        this.getAllCustomFieldCategory();
      }
    });
   }


  getPageEvent(event){
    // this.getAllSchool.pageNumber=event.pageIndex+1;
    // this.getAllSchool.pageSize=event.pageSize;
    // this.callAllSchool(this.getAllSchool);
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
    this.customFieldAddView.customFields = element;
    this.customFieldservice.deleteCustomField(this.customFieldAddView).subscribe(
      (res: CustomFieldAddView) => {
        if (res){
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
          else {
            this.getAllCustomFieldCategory();
          }
        }
        else{
          this.snackbar.open(this.defaultValuesService.translateKey('customFieldFailed') + this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
      }
    );
  }
  confirmDelete(element){
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: '400px',
      data: {
          title: this.defaultValuesService.translateKey('areYouSure'),
          message: this.defaultValuesService.translateKey('youAreAboutToDelete') + element.title + '.'}
    });
    dialogRef.afterClosed().subscribe(dialogResult => {
      if (dialogResult){
        this.schoolService.changeSchoolListStatus({schoolLoaded:false,schoolChanged:true,dataFromUserLogin:false,academicYearChanged:false,academicYearLoaded:false});
        this.deleteCustomFieldata(element);
      }
   });
  }
  openEditdata(element){
    this.dialog.open(EditStudentFieldsComponent, {
      data: {information: element},
        width: '500px'
    }).afterClosed().subscribe((data) => {
      if (data === 'submited'){
        this.schoolService.changeSchoolListStatus({schoolLoaded:false,schoolChanged:true,dataFromUserLogin:false,academicYearChanged:false,academicYearLoaded:false});
        this.getAllCustomFieldCategory();
      }
    });
  }
  getAllCustomFieldCategory(){
    this.fieldsCategoryListView.module = Module.STUDENT;
    this.customFieldservice.getAllFieldsCategory(this.fieldsCategoryListView).subscribe(
      (res: FieldsCategoryListView) => {
        if (res){
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
            this.fieldsCategoryList = res.fieldsCategoryList;
            if (this.currentCategoryId == null){
              this.currentCategoryId = res.fieldsCategoryList[0].categoryId;
              this.customFieldList = new MatTableDataSource(res.fieldsCategoryList[0].customFields) ;
              this.customFieldList.sort = this.sort;
            }
            else{
              const index = this.fieldsCategoryList.findIndex((x) => {
                return x.categoryId === this.currentCategoryId;
              });
              this.customFieldList = new MatTableDataSource(res.fieldsCategoryList[index].customFields) ;
              this.customFieldList.sort = this.sort;
            }
          }
        }
        else{
          this.snackbar.open(this.defaultValuesService.translateKey('fieldCategoryListFailed') + this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
      }
    );
  }
  editFieldCategory(element){
    this.dialog.open(StudentFieldsCategoryComponent, {
      data: element,
      width: '500px'
    }).afterClosed().subscribe((data) => {
      if (data === 'submited'){
      this.schoolService.changeSchoolListStatus({schoolLoaded:false,schoolChanged:true,dataFromUserLogin:false,academicYearChanged:false,academicYearLoaded:false});
        this.getAllCustomFieldCategory();
      }
    });
  }
  deleteFieldCategory(element){
    this.fieldsCategoryAddView.fieldsCategory = element;
    this.customFieldservice.deleteFieldsCategory(this.fieldsCategoryAddView).subscribe(
      (res: FieldsCategoryAddView) => {
        if (res){
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
            this.snackbar.open( res._message, '', {
              duration: 10000
            });
          }
          else{
            this.snackbar.open( res._message, '', {
              duration: 10000
            });
            if (element.categoryId === this.currentCategoryId){
              this.currentCategoryId = null;
            }
            this.getAllCustomFieldCategory();
          }
        }
        else{
          this.snackbar.open(this.defaultValuesService.translateKey('fieldCategoryDeleteFailed') 
          + this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
      }
    );
  }
  confirmDeleteFieldCategory(element){
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: '400px',
      data: {
          title: this.defaultValuesService.translateKey('areYouSure'),
          message: this.defaultValuesService.translateKey('youAreAboutToDelete') + element.title + '.'}
    });
    dialogRef.afterClosed().subscribe(dialogResult => {
      if (dialogResult){
        this.deleteFieldCategory(element);
      }
   });
  }

  drop(event: CdkDragDrop<string[]>) {
    if(this.noSortIndexContainer.includes(event.currentIndex) && this.currentCategoryId===3){
      this.snackbar.open('Could not sort', 'Ok', {
        duration: 2000
      });
      return;
    }
    this.customFieldDragDropModel.categoryId = this.currentCategoryId;
    this.customFieldDragDropModel.currentSortOrder = this.customFieldList.data[event.currentIndex].sortOrder;
    this.customFieldDragDropModel.previousSortOrder = this.customFieldList.data[event.previousIndex].sortOrder;
    this.customFieldservice.updateCustomFieldSortOrder(this.customFieldDragDropModel).subscribe(
      (res: CustomFieldDragDropModel) => {
        if (res){
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
        else{
          this.snackbar.open(this.defaultValuesService.translateKey('customFieldDragShortFailed')
          + this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
      }
    );
  }

}
