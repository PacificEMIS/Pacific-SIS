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

import { Component, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import icEdit from '@iconify/icons-ic/twotone-edit';
import icDelete from '@iconify/icons-ic/twotone-delete';
import icAdd from '@iconify/icons-ic/baseline-add';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import icSearch from '@iconify/icons-ic/search';
import icFilterList from '@iconify/icons-ic/filter-list';
import { MatDialog } from '@angular/material/dialog';
import { AddGradeComponent } from './add-grade/add-grade.component';
import { ValidationService } from '../../shared/validation.service';
import { GradeScaleAddViewModel, GradeScaleListView, GradeDragDropModel, GradeAddViewModel } from 'src/app/models/grades.model';
import { GradesService } from 'src/app/services/grades.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ConfirmDialogComponent } from '../../shared-module/confirm-dialog/confirm-dialog.component';
import { MatTableDataSource } from '@angular/material/table';
import { LoaderService } from 'src/app/services/loader.service';
import { CdkDragDrop } from '@angular/cdk/drag-drop';
import { fadeInUp400ms } from '../../../../@vex/animations/fade-in-up.animation';
import { stagger40ms } from '../../../../@vex/animations/stagger.animation';
import { MatSort } from '@angular/material/sort';
import { Permissions, RolePermissionListViewModel, RolePermissionViewModel } from '../../../models/roll-based-access.model';
import { CryptoService } from '../../../services/Crypto.service';
import { ExcelService } from '../../../services/excel.service';
import { takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { PageRolesPermission } from '../../../common/page-roles-permissions.service';
import { CommonService } from 'src/app/services/common.service';

@Component({
  selector: 'vex-standard-grades',
  templateUrl: './standard-grades.component.html',
  styleUrls: ['./standard-grades.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})
export class StandardGradesComponent implements OnInit, OnDestroy {

  @Input()
  columns = [
    { label: 'Order', property: 'id', type: 'number', visible: true },
    { label: 'Title', property: 'title', type: 'text', visible: true },
    { label: 'Description', property: 'comment', type: 'text', visible: true },
    { label: 'action', property: 'action', type: 'text', visible: true }
  ];

  icEdit = icEdit;
  icDelete = icDelete;
  icAdd = icAdd;
  effortCategoryTitle: string;
  addCategory = false;
  form: FormGroup;
  buttonType: string;
  icSearch = icSearch;
  icFilterList = icFilterList;
  gradeScaleAddViewModel: GradeScaleAddViewModel = new GradeScaleAddViewModel();
  gradeScaleListView: GradeScaleListView = new GradeScaleListView();
  gradeScaleList = [];
  currentGradeScaleId = null;
  gradeListValue = [];
  gradeList: MatTableDataSource<any>;
  @ViewChild(MatSort) sort: MatSort;
  totalCount;
  loading: Boolean;
  gradeDragDropModel: GradeDragDropModel = new GradeDragDropModel();
  gradeAddViewModel: GradeAddViewModel = new GradeAddViewModel();
  gradeScaleListForExcel = [];
  destroySubject$: Subject<void> = new Subject();
  editPermission = false;
  deletePermission = false;
  addPermission = false;
  permissionListViewModel: RolePermissionListViewModel = new RolePermissionListViewModel();
  permissionGroup: RolePermissionViewModel = new RolePermissionViewModel();
  searchKey: string;
  editMode: boolean;
  permissions: Permissions
  gradeName = null;

  constructor(public translateService: TranslateService,
    private dialog: MatDialog,
    private fb: FormBuilder,
    private gradesService: GradesService,
    private snackbar: MatSnackBar,
    private loaderService: LoaderService,
    private pageRolePermissions: PageRolesPermission,
    private excelService: ExcelService,
    private commonService: CommonService,

  ) {
    //translateService.use('en');
    this.formInit();
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });

  }

  ngOnInit(): void {
    this.getAllGradeScale();
    this.permissions = this.pageRolePermissions.checkPageRolePermission('/school/settings/grade-settings/standard-grades-setup')
  }

  // This formInit method is used for initialize the reactive form group.
  formInit() {
    this.form = this.fb.group({
      gradeScaleId: [0],
      gradeScaleName: ['', [ValidationService.noWhitespaceValidator]],
    })
  }

  // This applyFilter method is used for search grade.
  applyFilter() {
    this.gradeList.filter = this.searchKey.trim().toLowerCase();
  }

  // This onSearchClear method is used for clear search input.
  onSearchClear() {
    this.searchKey = '';
    this.applyFilter();
  }

  // This selectGradeScale method is used for select the grade scale.
  selectGradeScale(element) {
    this.currentGradeScaleId = element.gradeScaleId;
    this.gradeList = new MatTableDataSource(element.grade);
    this.totalCount = element.grade.length;
  }

  // This submit method is used for call addGradeScale and updateGradeScale method.
  submit() {
    this.form.markAllAsTouched();
    if (this.form.invalid) { return; }

    if (this.editMode) {
      this.updateGradeScale();
    } else {
      this.addGradeScale();
    }

  }

  // This addGradeScale method is used for call addGradeScale API.
  addGradeScale() {
    this.gradeScaleAddViewModel.gradeScale.gradeScaleId = this.form.controls.gradeScaleId.value;
    this.gradeScaleAddViewModel.gradeScale.gradeScaleName = this.form.controls.gradeScaleName.value;
    this.gradeScaleAddViewModel.gradeScale.useAsStandardGradeScale = true;
    this.gradesService.addGradeScale(this.gradeScaleAddViewModel).subscribe(
      (res: GradeScaleAddViewModel) => {
        if (typeof (res) == 'undefined') {
          this.snackbar.open('' + sessionStorage.getItem("httpError"), '', {
            duration: 10000
          });
        }
        else {
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
            this.getAllGradeScale();
            this.closeAddCategory(this.gradeName);
          }
        }
      }
    );
  }

  // This updateGradeScale method is used for call updateGradeScale API.
  updateGradeScale() {
    this.gradeScaleAddViewModel.gradeScale.gradeScaleId = this.form.controls.gradeScaleId.value;
    this.gradeScaleAddViewModel.gradeScale.gradeScaleName = this.form.controls.gradeScaleName.value;
    this.gradeScaleAddViewModel.gradeScale.useAsStandardGradeScale = true;

    this.gradesService.updateGradeScale(this.gradeScaleAddViewModel).subscribe(
      (res: GradeScaleAddViewModel) => {
        if (typeof (res) == 'undefined') {
          this.snackbar.open('' + sessionStorage.getItem("httpError"), '', {
            duration: 10000
          });
        }
        else {
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
            this.getAllGradeScale();
            this.closeAddCategory(this.gradeName);
          }
        }
      }
    );
  }

  // This getAllGradeScale method is used for call getAllGradeScaleList API.
  getAllGradeScale() {
    this.gradesService.getAllGradeScaleList(this.gradeScaleListView).subscribe(
      (res: GradeScaleListView) => {
        if (typeof (res) === 'undefined') {
          this.snackbar.open('' + sessionStorage.getItem('httpError'), '', {
            duration: 10000
          });
        }
        else {
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
            this.gradeScaleList = [];
            if (!res.gradeScaleList) {
              this.snackbar.open(res._message, '', {
                duration: 10000
              });
            }
          }
          else {
            this.gradeScaleList = res.gradeScaleList.filter(x => x.useAsStandardGradeScale);
            let index = 0;
            if (this.gradeScaleList.length > 0) {
              if (!this.currentGradeScaleId) {
                this.currentGradeScaleId = this.gradeScaleList[index]?.gradeScaleId;
              }
              else {
                index = this.gradeScaleList.findIndex((x) => {
                  return x.gradeScaleId === this.currentGradeScaleId;
                });
              }
              this.gradeListValue = this.gradeScaleList[index].grade.map((item) => {
                return ({
                  gradeScaleId: item.gradeScaleId,
                  gradeId: item.gradeId,
                  title: item.title,
                  comment: item.comment
                })
              })
            }
            this.totalCount = this.gradeListValue.length
            this.gradeList = new MatTableDataSource(this.gradeListValue);
            this.gradeList.filterPredicate = this.createFilter();
            this.gradeScaleListForExcel = this.gradeScaleList[index]?.grade;
            this.closeAddCategory(this.gradeName)
          }
        }

      }
    );
  }

  // This createFilter method is used for filter the search key.
  createFilter(): (data: any, filter: string) => boolean {
    let filterFunction = (data, filter): boolean => {
      return (
        data.title.toLowerCase().includes(filter) || data.comment.toLowerCase().includes(filter)
      );
    };
    return filterFunction;
  }

  // This drop method is used for drag and drop of grade.
  drop(event: CdkDragDrop<string[]>) {
    this.gradeDragDropModel.gradeScaleId = this.currentGradeScaleId;
    this.gradeDragDropModel.currentSortOrder = this.gradeScaleListForExcel[event.currentIndex].sortOrder;
    this.gradeDragDropModel.previousSortOrder = this.gradeScaleListForExcel[event.previousIndex].sortOrder;

    this.gradesService.updateGradeSortOrder(this.gradeDragDropModel).subscribe(
      (res: GradeDragDropModel) => {
        if (typeof (res) === 'undefined') {
          this.snackbar.open('' + sessionStorage.getItem('httpError'), '', {
            duration: 10000
          });
        } else {
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
            this.snackbar.open('' + res._message, '', {
              duration: 10000
            });
          }
          else {
            this.getAllGradeScale()
          }
        }
      }
    );
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  // This editGradeScale method is used for open update grade scale input field.
  editGradeScale(element) {
    this.gradeScaleList.map((item) => {
      if (item.gradeScaleName !== element.gradeScaleName) {
        item.hideDeleteButton = false;
      }
      else {
        item.hideDeleteButton = true;
      }
    })
    this.gradeName = element;

    this.effortCategoryTitle = "updateGradeScale";
    this.form.patchValue(element)
    this.addCategory = true;
    this.buttonType = "update";
    this.editMode = true;
  }

  // This goToAddCategory method is used for open add grade scale input field.
  goToAddCategory() {
    this.editMode = false;
    this.effortCategoryTitle = "addNewGradeScale";
    this.addCategory = true;
    this.buttonType = "submit";
  }

  // This deleteGradeScale method is used for call deleteGradeScale API.
  deleteGradeScale(element) {
    this.gradeScaleAddViewModel.gradeScale.gradeScaleId = element.gradeScaleId
    this.gradesService.deleteGradeScale(this.gradeScaleAddViewModel).subscribe(
      (res: GradeScaleAddViewModel) => {
        if (typeof (res) === 'undefined') {
          this.snackbar.open('' + sessionStorage.getItem('httpError'), '', {
            duration: 10000
          });
        }
        else {
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
            if (element.gradeScaleId === this.currentGradeScaleId) {
              this.currentGradeScaleId = null;
            }
            this.getAllGradeScale();
          }
        }
      }
    );
  }

  // This confirmDeleteGradeScale method is used for open deleteGradeScale confirmation dialog.
  confirmDeleteGradeScale(element) {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: '400px',
      data: {
        title: 'Are you sure?',
        message: 'You are about to delete ' + element.gradeScaleName + '.'
      }
    });
    dialogRef.afterClosed().subscribe(dialogResult => {
      if (dialogResult) {
        this.deleteGradeScale(element);
      }
    });
  }

  // This deleteGrade method is used for call deleteGrade API.
  deleteGrade(element) {
    this.gradeAddViewModel.grade.gradeId = element.gradeId
    this.gradesService.deleteGrade(this.gradeAddViewModel).subscribe(
      (res: GradeAddViewModel) => {
        if (typeof (res) === 'undefined') {
          this.snackbar.open('' + sessionStorage.getItem('httpError'), '', {
            duration: 10000
          });
        }
        else {
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
            this.getAllGradeScale();
          }
        }
      }
    );
  }

  // This confirmDeleteGrade method is used for open deleteGrade confirmation dialog.
  confirmDeleteGrade(element) {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: '400px',
      data: {
        title: 'Are you sure?',
        message: 'You are about to delete report card grade ' + element.title + '.'
      }
    });
    dialogRef.afterClosed().subscribe(dialogResult => {
      if (dialogResult) {
        this.deleteGrade(element);
      }
    });
  }

  // This closeAddCategory method is used for cancel add/edit grade scale.
  closeAddCategory(element) {
    if(element){
      element.hideDeleteButton = false;
    }
    this.addCategory = false;
    this.formInit();
  }

  // This addGrade method is used for open add grade dialog.
  addGrade() {
    this.dialog.open(AddGradeComponent, {
      data: { gradeScaleId: this.currentGradeScaleId },
      width: '500px'
    }).afterClosed().subscribe((data) => {
      if (data) {
        this.getAllGradeScale();
      }
    });
  }

  // This editGrade method is used for open edit grade dialog.
  editGrade(element) {
    this.dialog.open(AddGradeComponent, {
      data: { information: element },
      width: '500px'
    }).afterClosed().subscribe((data) => {
      if (data) {
        this.getAllGradeScale();
      }
    })
  }

  // This toggleColumnVisibility method is used for filter the column from Mat table.
  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  // This translateKey method is used for translate the excel sheet heading.
  translateKey(key) {
    let trnaslateKey;
    this.translateService.get(key).subscribe((res: string) => {
      trnaslateKey = res;
    });
    return trnaslateKey;
  }

  // This exportToExcel method is used for export data to excel sheet.
  exportToExcel() {
    this.gradeScaleList.map((data) => {
      if (data.gradeScaleId === this.currentGradeScaleId) {
        const standardGradeList = data.grade;
        if (standardGradeList.length > 0) {
          const reportList = standardGradeList.map((x) => {
            return {
              [this.translateKey('title')]: x.title,
              [this.translateKey('description')]: x.comment
            };
          });
          this.excelService.exportAsExcelFile(reportList, 'Standard_Grade_List_');
        } else {
          this.snackbar.open('No records found. failed to export standard grade list', '', {
            duration: 5000
          });
        }
      }
    });
  }

  // For destroy the isLoading subject.
  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }

}