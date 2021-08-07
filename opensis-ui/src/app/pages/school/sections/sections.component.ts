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

import { Component, OnInit,Input,ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { fadeInUp400ms } from '../../../../@vex/animations/fade-in-up.animation';
import { stagger60ms } from '../../../../@vex/animations/stagger.animation';
import icEdit from '@iconify/icons-ic/twotone-edit';
import icDelete from '@iconify/icons-ic/twotone-delete';
import icSearch from '@iconify/icons-ic/twotone-search';
import icAdd from '@iconify/icons-ic/twotone-add';
import icFilterList from '@iconify/icons-ic/twotone-filter-list';
import { EditSectionComponent } from '../sections/edit-section/edit-section.component';
import { TranslateService } from '@ngx-translate/core';
import { SelectionModel } from '@angular/cdk/collections';
import { MatTableDataSource } from '@angular/material/table';
import { GetAllSectionModel,SectionAddModel} from 'src/app/models/section.model';
import { LoaderService } from '../../../services/loader.service';
import { SectionService } from '../../../services/section.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { ConfirmDialogComponent } from '../../shared-module/confirm-dialog/confirm-dialog.component';
import {LayoutService} from 'src/@vex/services/layout.service';
import { CryptoService } from 'src/app/services/Crypto.service';
import { RolePermissionListViewModel, RolePermissionViewModel } from 'src/app/models/roll-based-access.model';
import { ExcelService } from '../../../services/excel.service';
import { PageRolesPermission } from '../../../common/page-roles-permissions.service';
import { Permissions } from '../../../models/roll-based-access.model';
import { CommonService } from 'src/app/services/common.service';
@Component({
  selector: 'vex-sections',
  templateUrl: './sections.component.html',
  styleUrls: ['./sections.component.scss'],
  animations: [
    stagger60ms,
    fadeInUp400ms
  ]
})
export class SectionsComponent implements OnInit {
  columns = [
   
    { label: 'Title', property: 'name', type: 'text', visible: true },
    { label: 'Sort Order', property: 'sortOrder', type: 'text', visible: true, cssClasses: ['font-medium'] },
    { label: 'Action', property: 'action', type: 'text', visible: true }
    
  ];
  icEdit = icEdit;
  icDelete = icDelete;
  icSearch = icSearch;
  icAdd = icAdd;
  icFilterList = icFilterList;
  loading;
  selection = new SelectionModel<any>(true, []);
  totalCount:Number;pageNumber:Number;pageSize:Number;
  getAllSection: GetAllSectionModel = new GetAllSectionModel();  
  sectionAddModel:SectionAddModel= new SectionAddModel();
  SectionModelList: MatTableDataSource<any>;
  searchKey:string;
  @ViewChild(MatSort) sort: MatSort;
  permissions: Permissions
  constructor(
    private dialog: MatDialog,
    public translateService:TranslateService, 
    private loaderService:LoaderService,
    private sectionService:SectionService,
    private snackbar: MatSnackBar,
    private layoutService:LayoutService,
    private cryptoService: CryptoService,
    private excelService: ExcelService,
    private pageRolePermissions: PageRolesPermission,
    private commonService: CommonService,

    ) 
  { 
    
    //translateService.use('en');   
    if(localStorage.getItem("collapseValue") !== null){
      if( localStorage.getItem("collapseValue") === "false"){
        this.layoutService.expandSidenav();
      }else{
        this.layoutService.collapseSidenav();
      } 
    }else{
      this.layoutService.expandSidenav();
    }
    this.loaderService.isLoading.subscribe((val) => {
       this.loading = val;
     });

   
    this.getSectiondetails();
}

  ngOnInit(): void {
    this.permissions = this.pageRolePermissions.checkPageRolePermission('/school/settings/school-settings/sections')
  }
  getSectiondetails()
  {    
    this.callAllSection(this.getAllSection);
  }
  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }
  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }
  openAddNew() {
    this.dialog.open(EditSectionComponent, {
      data: null,
      width: '600px'
    }).afterClosed().subscribe(data => {
    if(data){
      this.getSectiondetails();
    }
     
    });
  }

  callAllSection(getAllSection){
    this.sectionService.GetAllSection(this.getAllSection).subscribe(data => {
     if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
        this.SectionModelList = new MatTableDataSource([]);
        this.SectionModelList.sort=this.sort; 
          if(!data.tableSectionsList){
            this.snackbar.open( data._message, '', {
              duration: 10000
            });
          }
      }else{     
        this.SectionModelList = new MatTableDataSource(data.tableSectionsList);
        this.SectionModelList.sort=this.sort;      
      }
    });
  }
  editSection(editDetails){ 
        
    this.dialog.open(EditSectionComponent, {
      width: '600px',
      data: {
        editDetails:editDetails
      }   
    }).afterClosed().subscribe((data) => {
      if(data){
        this.getSectiondetails();
      }
    });

  }
  confirmDelete(deleteDetails){
      // call our modal window
      const dialogRef = this.dialog.open(ConfirmDialogComponent, {
        maxWidth: "400px",
        data: {
            title: "Are you sure?",
            message: "You are about to delete "+deleteDetails.name+"."}
      });
      // listen to response
      dialogRef.afterClosed().subscribe(dialogResult => {
        // if user pressed yes dialogResult will be true, 
        // if user pressed no - it will be false
        if(dialogResult){
          this.deleteSection(deleteDetails);
        }
     });
    }
  deleteSection(deleteDetails){
    this.sectionAddModel.tableSections.schoolId=+sessionStorage.getItem("selectedSchoolId");
    this.sectionAddModel.tableSections.sectionId = deleteDetails.sectionId;
          
    this.sectionService.deleteSection(this.sectionAddModel).subscribe(data => {
      if (typeof (data) == 'undefined') {
        this.snackbar.open('Section Deletion failed. ' + sessionStorage.getItem("httpError"), '', {
          duration: 10000
        });
      }
      else {
       if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
          this.snackbar.open( data._message, '', {
            duration: 10000
          });
        } else {
       
          this.snackbar.open(data._message, '', {
            duration: 10000
          }).afterOpened().subscribe(data => {
            this.getSectiondetails();
          });
          
        }
      }

    })
  }
  onSearchClear(){
    this.searchKey="";
    this.applyFilter();
  }

  applyFilter(){
    this.SectionModelList.filter = this.searchKey.trim().toLowerCase()
  }

  translateKey(key) {
    let trnaslateKey;
    this.translateService.get(key).subscribe((res: string) => {
       trnaslateKey = res;
    });
    return trnaslateKey;
  }

  exportToExcel(){
    if (this.SectionModelList.data?.length > 0) {
      const sectionList = this.SectionModelList.data?.map((x) => {
        return {
          [this.translateKey('title')]: x.name,
          [this.translateKey('sortOrder')]: x.sortOrder

        };
      });
      this.excelService.exportAsExcelFile(sectionList, 'Section_List_');
    } else {
      this.snackbar.open('No records found. failed to export Section List', '', {
        duration: 5000
      });
    }
  }
  
}
