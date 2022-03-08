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

import { Component, OnInit, Input, ViewChild, OnDestroy } from '@angular/core';
import icMoreVert from '@iconify/icons-ic/twotone-more-vert';
import icAdd from '@iconify/icons-ic/baseline-add';
import icEdit from '@iconify/icons-ic/twotone-edit';
import icDelete from '@iconify/icons-ic/twotone-delete';
import icSearch from '@iconify/icons-ic/search';
import icFilterList from '@iconify/icons-ic/filter-list';
import icImport from '@iconify/icons-ic/twotone-unarchive';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { fadeInUp400ms } from '../../../../@vex/animations/fade-in-up.animation';
import { stagger40ms } from '../../../../@vex/animations/stagger.animation';
import { TranslateService } from '@ngx-translate/core';
import { EditEffortGradeScaleComponent } from './edit-effort-grade-scale/edit-effort-grade-scale.component';
import { FormControl } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { EffortGradeScaleModel, GetAllEffortGradeScaleListModel, UpdateEffortGradeScaleSortOrderModel } from '../../../models/grades.model';
import { GradesService } from '../../../services/grades.service';
import { MatTableDataSource } from '@angular/material/table';
import { ConfirmDialogComponent } from '../../shared-module/confirm-dialog/confirm-dialog.component';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { LoaderService } from '../../../services/loader.service';
import { ExcelService } from '../../../services/excel.service';
import { CdkDragDrop } from '@angular/cdk/drag-drop';
import { Subject } from 'rxjs';
import { Permissions, RolePermissionListViewModel, RolePermissionViewModel } from '../../../models/roll-based-access.model';
import { CryptoService } from '../../../services/Crypto.service';
import { PageRolesPermission } from '../../../common/page-roles-permissions.service';
import { CommonService } from 'src/app/services/common.service';
import { DefaultValuesService } from 'src/app/common/default-values.service';

@Component({
  selector: 'vex-effort-grade-scale',
  templateUrl: './effort-grade-scale.component.html',
  styleUrls: ['./effort-grade-scale.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})
export class EffortGradeScaleComponent implements OnInit,OnDestroy {
  icMoreVert = icMoreVert;
  icAdd = icAdd;
  icEdit = icEdit;
  icDelete = icDelete;
  icSearch = icSearch;
  icImport = icImport;
  icFilterList = icFilterList;

  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort
  columns = [
    { label: 'order', property: 'order', type: 'number', visible: true },
    { label: 'value', property: 'gradeScaleValue', type: 'text', visible: true },
    { label: 'comment', property: 'gradeScaleComment', type: 'text', visible: true },
    { label: 'actions', property: 'actions', type: 'text', visible: true }
  ];
  effortGradeScaleModelList: MatTableDataSource<any>;
  loading: boolean;
  searchCtrl: FormControl;
  getEffortGradeScaleList: GetAllEffortGradeScaleListModel = new GetAllEffortGradeScaleListModel();
  totalCount: number = 0;
  pageNumber: number;
  pageSize: number;
  destroySubject$: Subject<void> = new Subject();
  editPermission = false;
  deletePermission = false;
  addPermission = false;
  permissionListViewModel: RolePermissionListViewModel = new RolePermissionListViewModel();
  permissionGroup: RolePermissionViewModel = new RolePermissionViewModel();
  effortGradeScaleDragAndDrop: UpdateEffortGradeScaleSortOrderModel = new UpdateEffortGradeScaleSortOrderModel();
  permissions: Permissions;
  constructor(private router: Router,
    private dialog: MatDialog,
    public translateService: TranslateService,
    private gradesService: GradesService,
    private loaderService: LoaderService,
    private snackbar: MatSnackBar,
    private excelService: ExcelService,
    private pageRolePermissions: PageRolesPermission,
    private commonService: CommonService,
    public defaultValuesService: DefaultValuesService
    ) {
    //translateService.use('en');
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });
    this.getEffortGradeScaleList.filterParams = null;
    this.getAllEffortGradeScale();
    if(!this.defaultValuesService.checkAcademicYear()){
      this.columns.pop();
      this.columns.shift();
    }
  }

  ngOnInit(): void {
    this.permissions = this.pageRolePermissions.checkPageRolePermission('/school/settings/grade-settings/effort-grade-setup')
    this.searchCtrl = new FormControl();
  }

  ngAfterViewInit() {
    //  Sorting
    this.getEffortGradeScaleList = new GetAllEffortGradeScaleListModel();
    this.sort.sortChange.subscribe((res) => {
      this.getEffortGradeScaleList.pageNumber = this.pageNumber
      this.getEffortGradeScaleList.pageSize = this.pageSize;
      this.getEffortGradeScaleList.sortingModel.sortColumn = res.active;
      if (this.searchCtrl.value != null && this.searchCtrl.value != "") {
        let filterParams = [
          {
            columnName: null,
            filterValue: this.searchCtrl.value,
            filterOption: 3
          }
        ]
        Object.assign(this.getEffortGradeScaleList, { filterParams: filterParams });
      }
      if (res.direction == "") {
        this.getEffortGradeScaleList.sortingModel = null;
        this.getAllEffortGradeScale();
        this.getEffortGradeScaleList = new GetAllEffortGradeScaleListModel();
        this.getEffortGradeScaleList.sortingModel = null;
      } else {
        this.getEffortGradeScaleList.sortingModel.sortDirection = res.direction;
        this.getAllEffortGradeScale();
      }
    });

    //  Searching
    this.searchCtrl.valueChanges.pipe(debounceTime(500), distinctUntilChanged()).subscribe((term) => {
      if (term != '') {
        this.searchWithTerm(term)
      } else {
        this.searchWithoutTerm()
      }
    })
  }

  searchWithTerm(term) {
    let filterParams = [
      {
        columnName: null,
        filterValue: term,
        filterOption: 3
      }
    ]
    if (this.sort.active != undefined && this.sort.direction != "") {
      this.getEffortGradeScaleList.sortingModel.sortColumn = this.sort.active;
      this.getEffortGradeScaleList.sortingModel.sortDirection = this.sort.direction;
    }
    Object.assign(this.getEffortGradeScaleList, { filterParams: filterParams });
    this.getEffortGradeScaleList.pageNumber = 1;
    this.paginator.pageIndex = 0;
    this.getEffortGradeScaleList.pageSize = this.pageSize;
    this.getAllEffortGradeScale();
  }

  searchWithoutTerm() {
    Object.assign(this.getEffortGradeScaleList, { filterParams: null });
    this.getEffortGradeScaleList.pageNumber = this.paginator.pageIndex + 1;
    this.getEffortGradeScaleList.pageSize = this.pageSize;
    if (this.sort.active != undefined && this.sort.direction != "") {
      this.getEffortGradeScaleList.sortingModel.sortColumn = this.sort.active;
      this.getEffortGradeScaleList.sortingModel.sortDirection = this.sort.direction;
    }
    this.getAllEffortGradeScale();
  }

  getPageEvent(event) {
    if (this.sort.active != undefined && this.sort.direction != "") {
      this.getEffortGradeScaleList.sortingModel.sortColumn = this.sort.active;
      this.getEffortGradeScaleList.sortingModel.sortDirection = this.sort.direction;
    }
    if (this.searchCtrl.value != null && this.searchCtrl.value != "") {
      let filterParams = [
        {
          columnName: null,
          filterValue: this.searchCtrl.value,
          filterOption: 3
        }
      ]
      Object.assign(this.getEffortGradeScaleList, { filterParams: filterParams });
    }
    this.getEffortGradeScaleList.pageNumber = event.pageIndex + 1;
    this.getEffortGradeScaleList.pageSize = event.pageSize;
    this.getAllEffortGradeScale();
  }

  openAdd() {
    this.dialog.open(EditEffortGradeScaleComponent, {
      data: {
        editMode: false,
      },
      width: '500px'
    }).afterClosed().subscribe((res) => {
      if (res) {
        this.getAllEffortGradeScale();
      }
    });
  }

  openEdit(gradeScaleDetails) {
    this.dialog.open(EditEffortGradeScaleComponent, {
      data: {
        editMode: true,
        gradeScaleDetails: gradeScaleDetails
      },
      width: '500px'
    }).afterClosed().subscribe((res) => {
      if (res) {
        this.getAllEffortGradeScale();
      }
    });
  }

  confirmDelete(deleteDetails) {
    // call our modal window
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: "400px",
      data: {
        title: "Are you sure?",
        message: "You are about to delete a grade scale"
      }
    });
    // listen to response
    dialogRef.afterClosed().subscribe(dialogResult => {
      // if user pressed yes dialogResult will be true, 
      // if user pressed no - it will be false
      if (dialogResult) {
        this.deleteEffortGradeScale(deleteDetails);
      }
    });
  }

  deleteEffortGradeScale(deleteDetails) {
    let effortGradeScale = new EffortGradeScaleModel();
    effortGradeScale.effortGradeScale.effortGradeScaleId = deleteDetails.effortGradeScaleId;
    this.gradesService.deleteEffortGradeScale(effortGradeScale).subscribe((res) => {
      if (typeof (res) == 'undefined') {
        this.snackbar.open('Effort Grade Scale Deletion failed. ' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      } else if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
        this.snackbar.open(res._message, '', {
          duration: 10000
        });
      } else {
        this.snackbar.open('Effort Grade Scale Deleted Successfully.', '', {
          duration: 10000
        });
        this.getAllEffortGradeScale();
      }
    })
  }

  getAllEffortGradeScale() {
    if (this.getEffortGradeScaleList.sortingModel?.sortColumn == "") {
      this.getEffortGradeScaleList.sortingModel.sortColumn = "sortOrder"
      this.getEffortGradeScaleList.sortingModel.sortDirection = "asc"
    }
    this.getEffortGradeScaleList.isListView=true;
    this.gradesService.getAllEffortGradeScaleList(this.getEffortGradeScaleList).subscribe(data => {
     if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
        if(data.effortGradeScaleList==null){
          this.effortGradeScaleModelList = new MatTableDataSource([]);
          this.snackbar.open( data._message, '', {
            duration: 10000
          });
        }else{
          this.effortGradeScaleModelList = new MatTableDataSource([]);
        }
      } else {
        this.totalCount = data.totalCount;
        this.pageNumber = data.pageNumber;
        this.pageSize = data._pageSize;
        this.effortGradeScaleModelList = new MatTableDataSource(data.effortGradeScaleList);
        this.getEffortGradeScaleList = new GetAllEffortGradeScaleListModel();
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

  exportEffortGradeScaleListToExcel() {
    let getEffortGradeScaleList: GetAllEffortGradeScaleListModel = new GetAllEffortGradeScaleListModel();
    getEffortGradeScaleList.pageNumber = 0;
    getEffortGradeScaleList.pageSize = 0;
    getEffortGradeScaleList.sortingModel.sortColumn = "sortOrder"
    getEffortGradeScaleList.sortingModel.sortDirection = "asc"
    this.gradesService.getAllEffortGradeScaleList(getEffortGradeScaleList).subscribe(res => {
    if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
        if(!res.effortGradeScaleList){
          this.snackbar.open(res._message, '', {
            duration: 10000
          });
        }
      } else {
        if (res.effortGradeScaleList?.length > 0) {
          let gradeScale = res.effortGradeScaleList?.map((x) => {
            return {
              [this.translateKey('value')]: x.gradeScaleValue,
              [this.translateKey('comment')]: x.gradeScaleComment
            }
          });
          this.excelService.exportAsExcelFile(gradeScale, 'Effort_Grade_Scale_List_')
        } else {
          this.snackbar.open('No Records Found. Failed to Export Effort Grade Scale List', '', {
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

  drop(event: CdkDragDrop<string[]>) {
    this.effortGradeScaleDragAndDrop.currentSortOrder = this.effortGradeScaleModelList.data[event.currentIndex].sortOrder
    this.effortGradeScaleDragAndDrop.previousSortOrder = this.effortGradeScaleModelList.data[event.previousIndex].sortOrder
    this.gradesService.updateEffortGradeScaleSortOrder(this.effortGradeScaleDragAndDrop).subscribe(
      (res: UpdateEffortGradeScaleSortOrderModel) => {
        if (typeof (res) == 'undefined') {
          this.snackbar.open('Effort Grade Scale Drag Sort Failed. ' + this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        } else {
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
            this.snackbar.open('Effort Grade Scale Drag Sort Failed. ' + res._message, '', {
              duration: 10000
            });
          }
          else {
            this.getAllEffortGradeScale()
          }
        }
      }
    );
  }

  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }

}
