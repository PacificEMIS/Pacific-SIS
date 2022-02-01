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

import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
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
import { ParentInfoService } from '../../../services/parent-info.service';
import { filterParams, GetAllParentModel, GetAllParentResponseModel, ParentAdvanceSearchModel, ParentList } from "../../../models/parent-info.model";
import { LoaderService } from '../../../services/loader.service';
import { MatTableDataSource } from '@angular/material/table';
import { StudentService } from '../../../services/student.service';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { ExcelService } from '../../../services/excel.service';
import { fadeInRight400ms } from '../../../../@vex/animations/fade-in-right.animation';
import { Permissions, RolePermissionListViewModel } from '../../../models/roll-based-access.model';
import { CryptoService } from '../../../services/Crypto.service';
import { CommonService } from '../../../services/common.service';
import { FormControl } from '@angular/forms';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { PageRolesPermission } from '../../../common/page-roles-permissions.service';
import { ModuleIdentifier } from '../../../enums/module-identifier.enum';
import { ImageCropperService } from '../../../services/image-cropper.service';
import { SchoolCreate } from '../../../enums/school-create.enum';
import icRestore from '@iconify/icons-ic/twotone-restore';
import { DataEditInfoComponent } from '../../shared-module/data-edit-info/data-edit-info.component';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'vex-parentinfo',
  templateUrl: './parentinfo.component.html',
  styleUrls: ['./parentinfo.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms,
    fadeInRight400ms
  ]
})
export class ParentinfoComponent implements OnInit, OnDestroy {
  columns = [
    { label: 'Parent Name', property: 'firstname', type: 'text', visible: true },
    { label: 'Profile', property: 'userProfile', type: 'text', visible: true },
    { label: 'Email Address', property: 'workEmail', type: 'text', visible: true },
    { label: 'Mobile Phone', property: 'mobile', type: 'number', visible: true },
    { label: 'Associated Students', property: 'students', type: 'text', visible: true },
    { label: 'Action', property: 'action', type: 'text', visible: true }
  ];
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort

  icMoreVert = icMoreVert;
  totalCount: number =0;
  icAdd = icAdd;
  icEdit = icEdit;
  icDelete = icDelete;
  icSearch = icSearch;
  icFilterList = icFilterList;
  icImpersonate = icImpersonate;
  icRestore = icRestore;
  loading: boolean;
  searchCount: number;
  getAllParentModel: GetAllParentModel = new GetAllParentModel();
  parentFieldsModelList: MatTableDataSource<ParentList>;
  allParentList = [];
  parentListForExcel = [];
  showAdvanceSearchPanel: boolean = false;
  permissionListViewModel:RolePermissionListViewModel = new RolePermissionListViewModel();
  pageNumber:number;
  pageSize:number;
  searchCtrl: FormControl;
  // categories=[
  //   {
  //     categoryId:3,
  //     title:'General Info'
  //   },
  //   {
  //     categoryId:4,
  //     title:'Enrollment Info'
  //   },
  //   {
  //     categoryId:5,
  //     title:'Address & Contact'
  //   },
  //   {
  //     categoryId:6,
  //     title:'Family Info'
  //   },
  //   {
  //     categoryId:7,
  //     title:'Medical Info'
  //   },
  //   {
  //     categoryId:8,
  //     title:'Documents'
  //   }
  // ];
  filterParamsFromAdvanceSearch: filterParams[]=[];
  filterParams: filterParams[]=[];
  moduleIdentifier=ModuleIdentifier;
  createMode=SchoolCreate;
  constructor(
    private router: Router,
    private parentInfoService: ParentInfoService,
    private snackbar: MatSnackBar,
    private loaderService: LoaderService,
    public translateService: TranslateService,
    private studentService: StudentService,
    private excelService: ExcelService,
    private imageCropperService: ImageCropperService,
    private dialog: MatDialog,
    private commonService:CommonService,
    private pageRolePermissions: PageRolesPermission,
    private defaultValuesService: DefaultValuesService
  ) {
    this.getAllParentModel.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
    
    this.getAllParentModel.filterParams=null;
    
    this.loaderService.isLoading.subscribe((val) => {
      this.loading = val;
    });
    this.getAllparentList();


  }

  ngOnInit(): void {
    this.searchCtrl = new FormControl();
  }

  ngAfterViewInit() {
    //  Sorting
    this.getAllParentModel = new GetAllParentModel();
    this.sort.sortChange.subscribe((res) => {
      if(this.filterParamsFromAdvanceSearch?.length>0){
        Object.assign(this.getAllParentModel,{filterParams: this.filterParamsFromAdvanceSearch});
      }
      this.getAllParentModel.pageNumber = this.pageNumber
      this.getAllParentModel.pageSize = this.pageSize;
      this.getAllParentModel.sortingModel.sortColumn = res.active;
      if (this.searchCtrl.value) {
        let filterParams = [
          {
            columnName: null,
            filterValue: this.searchCtrl.value,
            filterOption: 4
          }
        ]
        Object.assign(this.getAllParentModel, { filterParams: filterParams });
        this.filterParams=filterParams;
      }
      if (res.direction == "") {
        this.getAllParentModel.sortingModel = null;
        this.getAllparentList();
        this.getAllParentModel = new GetAllParentModel();
        this.getAllParentModel.sortingModel = null;
      } else {
        this.getAllParentModel.sortingModel.sortDirection = res.direction;
        this.getAllparentList();
      }
    });

    //  Searching
    this.searchCtrl.valueChanges.pipe(debounceTime(500), distinctUntilChanged()).subscribe((term) => {
      this.filterParamsFromAdvanceSearch=null;
      this.parentInfoService.setAdvanceSearchParams(null);
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
    if (this.sort.active) {
      this.getAllParentModel.sortingModel.sortColumn = this.sort.active;
      this.getAllParentModel.sortingModel.sortDirection = this.sort.direction;
    }
    Object.assign(this.getAllParentModel, { filterParams: filterParams });
    this.filterParams=filterParams;
    
    this.getAllParentModel.pageNumber = 1;
    this.paginator.pageIndex = 0;
    this.getAllParentModel.pageSize = this.pageSize;
    this.getAllparentList();
  }

  callWithoutFilterValue() {
      Object.assign(this.getAllParentModel, { filterParams: null });
      this.filterParams=null;
    this.getAllParentModel.pageNumber = this.paginator.pageIndex + 1;
    this.getAllParentModel.pageSize = this.pageSize;
    if (this.sort.active != undefined && this.sort.direction != "") {
      this.getAllParentModel.sortingModel.sortColumn = this.sort.active;
      this.getAllParentModel.sortingModel.sortDirection = this.sort.direction;
    }
    this.getAllparentList();
  }

  goToStudentInformation(studentId) {
    this.imageCropperService.enableUpload({module:this.moduleIdentifier.STUDENT,upload:true,mode:this.createMode.VIEW});
    this.studentService.setStudentId(studentId);
    let permittedDetails= this.pageRolePermissions.getPermittedSubCategories('/school/students');
    if(permittedDetails.length){  
      this.studentService.setCategoryTitle(permittedDetails[0].title);
      this.router.navigateByUrl(permittedDetails[0].path);
    }
  }

  getPageEvent(event){
    if(this.filterParamsFromAdvanceSearch?.length>0){
      Object.assign(this.getAllParentModel,{filterParams: this.filterParamsFromAdvanceSearch});
    }
    if(this.sort.active!=undefined && this.sort.direction!=""){
      this.getAllParentModel.sortingModel.sortColumn=this.sort.active;
      this.getAllParentModel.sortingModel.sortDirection=this.sort.direction;
    }
    if(this.searchCtrl.value){
      let filterParams=[
        {
         columnName:null,
         filterValue:this.searchCtrl.value,
         filterOption:3
        }
      ]
     Object.assign(this.getAllParentModel,{filterParams: filterParams});
    this.filterParams=filterParams;
    }
    
    this.getAllParentModel.pageNumber=event.pageIndex+1;
    this.getAllParentModel.pageSize=event.pageSize;
    this.defaultValuesService.setPageSize(event.pageSize);
    this.getAllparentList();
  }

  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  // For open the data chage history dialog
  openDataEdit(element) {
    this.dialog.open(DataEditInfoComponent, {
      width: '500px',
      data: { createdBy: element.createdBy, createdOn: element.createdOn, modifiedBy: element.updatedBy, modifiedOn: element.updatedOn }
    })
  }

  viewGeneralInfo(parentInfo) {
    this.parentInfoService.setParentId(parentInfo.parentId);
    this.parentInfoService.setParentDetails(parentInfo);
    this.checkViewPermission();
  }

  checkViewPermission(){
    let permittedTabs = this.pageRolePermissions.getPermittedCategories('/school/parents');
    if(permittedTabs[0]?.path){
      this.router.navigateByUrl(permittedTabs[0]?.path);
     this.defaultValuesService.setPageId(permittedTabs[0].title?.toLowerCase()); 
    }
  }

  // checkCurrentCategoryAndRoute(pageId) {
  //   if(pageId === 'General Info') {
  //     this.router.navigate(['/school', 'parents', 'parent-generalinfo']);
  //   } else if(pageId === 'Address Info') {
  //     this.router.navigate(['/school', 'parents', 'parent-addressinfo']);
  //   }
  // }


  getAllparentList() {
    if(this.getAllParentModel.sortingModel?.sortColumn==""){
      this.getAllParentModel.sortingModel=null
    }
    this.parentInfoService.getAllParentInfo(this.getAllParentModel).subscribe(
      (res: GetAllParentResponseModel) => {
        if (typeof (res) == 'undefined') {
          this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
        else {
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
            this.parentFieldsModelList = new MatTableDataSource([]);
            this.totalCount=0;
              if (!res.parentInfoForView){
                this.snackbar.open( res._message, '', {
                  duration: 10000
                });
              }
          }
          else {
            this.totalCount=res.totalCount;
            this.pageNumber = res.pageNumber;
            this.pageSize = res._pageSize;
            this.parentListForExcel = res.parentInfoForView;
            this.parentFieldsModelList = new MatTableDataSource(res.parentInfoForView);
            this.getAllParentModel = new GetAllParentModel();

          }
        }
      }
    );
  }

  exportParentListToExcel() {
    if(!this.totalCount){
      this.snackbar.open('No Record Found. Failed to Export Parent List','', {
        duration: 5000
      });
      return
    }
    let getAllParent: GetAllParentModel = new GetAllParentModel();
    getAllParent.pageNumber=0;
    getAllParent.pageSize=0;
    if(this.sort.active && this.sort.direction){
      getAllParent.sortingModel.sortColumn=this.sort.active;
      getAllParent.sortingModel.sortDirection=this.sort.direction;
    }else{
      getAllParent.sortingModel=null;
    }
    if(this.filterParamsFromAdvanceSearch?.length>0){
        getAllParent.filterParams=[...this.filterParamsFromAdvanceSearch]
    }else if(this.filterParams){
      getAllParent.filterParams=this.filterParams
    }
      this.parentInfoService.getAllParentInfo(getAllParent).subscribe(res => {
       if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          if(!res.parentInfoForView){
            this.snackbar.open(res._message,'', {
              duration: 5000
            });
          }
        }else{
          if(res.parentInfoForView.length>0){
            let parentList = res.parentInfoForView?.map((item:ParentList)=>{
              let associatedStudents=[];
              associatedStudents = item.students?.map((student:any)=>{
                return student.split('|')[0]+' '+student.split('|')[2];
              })
              return {
               'Parent\'s Name': item.firstname+' '+item.lastname,
               'Profile' : item.userProfile,
               'Email Address' : item.workEmail,
               'Mobile Phone' : item.mobile,
               'Associated Students' : associatedStudents?associatedStudents?.join(', '):'-'
             }
            });
            this.excelService.exportAsExcelFile(parentList,'Parent_List_')
          }else{
            this.snackbar.open('No Record Found. Failed to Export Parent List','', {
              duration: 5000
            });
          }
        }
      });
      
  }

  getSearchResult(res: GetAllParentResponseModel) {
    this.searchCount=res.parentInfoForView?.length | 0;
    this.pageNumber = res.pageNumber;
    this.pageSize = res._pageSize;
    this.totalCount=res.totalCount;
    this.parentFieldsModelList = new MatTableDataSource(res.parentInfoForView);
    this.getAllParentModel = new GetAllParentModel();
  }

  getSearchInput(event){
    this.filterParamsFromAdvanceSearch = event;
    this.parentInfoService.setAdvanceSearchParams(this.filterParamsFromAdvanceSearch);
  }

  resetParentList(){
    this.searchCount = null;
    this.getAllParentModel.filterParams=null;
    this.parentInfoService.setAdvanceSearchParams(null);
    this.getAllparentList();
  }


  showAdvanceSearch() {
    this.showAdvanceSearchPanel = true;
  }

  hideAdvanceSearch(event){
    this.showAdvanceSearchPanel = false;
  }

  ngOnDestroy(){
    this.parentInfoService.setAdvanceSearchParams(null)
  }

}
