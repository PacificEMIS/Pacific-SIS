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
import icAdd from '@iconify/icons-ic/baseline-add';
import icEdit from '@iconify/icons-ic/twotone-edit';
import icDelete from '@iconify/icons-ic/twotone-delete';
import icSearch from '@iconify/icons-ic/search';
import icFilterList from '@iconify/icons-ic/filter-list';
import { Router } from '@angular/router';
import { fadeInUp400ms } from '../../../../@vex/animations/fade-in-up.animation';
import { stagger40ms } from '../../../../@vex/animations/stagger.animation';
import { TranslateService } from '@ngx-translate/core';
import { MatDialog } from '@angular/material/dialog';
import { EditGradeScaleComponent } from './edit-grade-scale/edit-grade-scale.component';
import { EditReportCardGradeComponent } from './edit-report-card-grade/edit-report-card-grade.component';
import { GradeScaleListView, GradeScaleAddViewModel, GradeAddViewModel, GradeDragDropModel } from '../../../models/grades.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableDataSource } from '@angular/material/table';
import { ConfirmDialogComponent } from '../../shared-module/confirm-dialog/confirm-dialog.component';
import { MatSort } from '@angular/material/sort';
import { CdkDragDrop } from '@angular/cdk/drag-drop';
import { GradesService } from '../../../services/grades.service';
import { ExcelService } from '../../../services/excel.service';
import { LoaderService } from '../../../services/loader.service';
import { CryptoService } from '../../../services/Crypto.service';
import { Permissions, RolePermissionListViewModel, RolePermissionViewModel } from '../../../models/roll-based-access.model';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { PageRolesPermission } from '../../../common/page-roles-permissions.service';
import { CommonService } from 'src/app/services/common.service';

@Component({
  selector: 'vex-report-card-grades',
  templateUrl: './report-card-grades.component.html',
  styleUrls: ['./report-card-grades.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})
export class ReportCardGradesComponent implements OnInit, OnDestroy {

  @Input()
  columns = [
    { label: 'Order', property: 'id', type: 'number', visible: true },
    { label: 'Title', property: 'title', type: 'text', visible: true },
    { label: 'Breakoff', property: 'breakoff', type: 'text', visible: true },
    { label: 'Weighted GP Value', property: 'weightedGpValue', type: 'text', visible: true },
    { label: 'unweighted GP Value', property: 'unweightedGpValue', type: 'text', visible: true },
    { label: 'Comment', property: 'comment', type: 'text', visible: false },
    { label: 'action', property: 'action', type: 'text', visible: true }
  ];

  ReportCardGradesModelList;

  icAdd = icAdd;
  icEdit = icEdit;
  icDelete = icDelete;
  icSearch = icSearch;
  icFilterList = icFilterList;
  loading: Boolean;
  @ViewChild(MatSort) sort: MatSort;
  gradeScaleList = [];
  searchKey: string;
  currentGradeScaleId = null;
  gradeScaleAddViewModel: GradeScaleAddViewModel = new GradeScaleAddViewModel();
  gradeAddViewModel: GradeAddViewModel = new GradeAddViewModel();
  gradeDragDropModel: GradeDragDropModel = new GradeDragDropModel()
  gradeScaleListView: GradeScaleListView = new GradeScaleListView();
  gradeList: MatTableDataSource<any>;
  destroySubject$: Subject<void> = new Subject();
  editPermission = false;
  deletePermission = false;
  addPermission = false;
  permissionListViewModel: RolePermissionListViewModel = new RolePermissionListViewModel();
  permissionGroup: RolePermissionViewModel = new RolePermissionViewModel()
  gradeScaleListForExcel = [];
  gradeListValue = [];
  totalCount;
  permissions: Permissions;
  constructor(
    private router: Router,
    private dialog: MatDialog,
    public translateService: TranslateService,
    private snackbar: MatSnackBar,
    private gradesService: GradesService,
    private loaderService: LoaderService,
    private excelService: ExcelService,
    private pageRolePermissions: PageRolesPermission,
    private commonService: CommonService,
  ) {
    //translateService.use('en');
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });
  }

  ngOnInit(): void {
    this.getAllGradeScale(0);
    this.permissions = this.pageRolePermissions.checkPageRolePermission('/school/settings/grade-settings/report-card-grades')
  }

  getPageEvent(event) {
    // this.getAllSchool.pageNumber=event.pageIndex+1;
    // this.getAllSchool.pageSize=event.pageSize;
    // this.callAllSchool(this.getAllSchool);
  }
  applyFilter() {
    this.gradeList.filter = this.searchKey.trim().toLowerCase();
  }
  onSearchClear() {
    this.searchKey = '';
    this.applyFilter();
  }

  goToAddGrade() {
    this.dialog.open(EditReportCardGradeComponent, {
      data: { gradeScaleId: this.currentGradeScaleId },
      width: '500px'
    }).afterClosed().subscribe((data) => {
      if (data === 'submited') {
        this.getAllGradeScale(this.currentGradeScaleId);
      }
    });
  }
  editGrade(element) {
    this.dialog.open(EditReportCardGradeComponent, {
      data: { information: element },
      width: '500px'
    }).afterClosed().subscribe((data) => {
      if (data === 'submited') {
        this.getAllGradeScale(this.currentGradeScaleId);
      }
    })
  }

  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  goToAddBlock() {
    this.dialog.open(EditGradeScaleComponent, {
      width: '500px'
    }).afterClosed().subscribe((data) => {
      if (data === 'submited') {
        this.getAllGradeScale(this.currentGradeScaleId);
      }
    });
  }
  editGradeScale(element) {
    this.dialog.open(EditGradeScaleComponent, {
      data: element,
      width: '500px'
    }).afterClosed().subscribe((data) => {
      if (data === 'submited') {
        this.getAllGradeScale(element.gradeScaleId);
      }
    })
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }
  selectGradeScale(element) {
    this.currentGradeScaleId = element.gradeScaleId;
    this.gradeList = new MatTableDataSource(element.grade);
    this.totalCount = element.grade.length;
  }
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
            this.getAllGradeScale(element.gradeScaleId);
          }
        }
      }
    );
  }
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
            this.getAllGradeScale(element.gradeScaleId);
          }
        }
      }
    );
  }
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

  getAllGradeScale(selectedGradeScaleId: number) {
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
            this.gradeScaleList = []
            if (!res.gradeScaleList) {
              this.snackbar.open(res._message, '', {
                duration: 10000
              });
            }

          }
          else {
            this.gradeScaleList = res.gradeScaleList.filter(x => !x.useAsStandardGradeScale);
            let index = 0;
            if (this.gradeScaleList.length > 0) {
              if (this.currentGradeScaleId === null) {
                this.currentGradeScaleId = this.gradeScaleList[index]?.gradeScaleId;
              }
              else {
                index = this.gradeScaleList.findIndex((x) => {
                  return x.gradeScaleId === this.currentGradeScaleId;
                });
              }
              this.gradeListValue = this.gradeScaleList[index]?.grade.map((item) => {
                return ({
                  gradeScaleId: item.gradeScaleId,
                  gradeId: item.gradeId,
                  title: item.title,
                  breakoff: item.breakoff,
                  weightedGpValue: item.weightedGpValue,
                  unweightedGpValue: item.unweightedGpValue,
                  comment: item.comment
                })
              })
            }
            this.totalCount = this.gradeListValue.length
            this.gradeList = new MatTableDataSource(this.gradeListValue);
            this.gradeScaleListForExcel = this.gradeScaleList[index]?.grade;
          }
        }
      }
    );
  }

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
            this.getAllGradeScale(this.currentGradeScaleId)
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

  exportToExcel() {
    this.gradeScaleList.map((data) => {
      if (data.gradeScaleId === this.currentGradeScaleId) {
        const reportCardGradeList = data.grade;
        if (reportCardGradeList.length > 0) {
          const reportList = reportCardGradeList.map((x) => {
            return {
              [this.translateKey('title')]: x.title,
              [this.translateKey('breakoff')]: x.breakoff,
              [this.translateKey('weightedGPValue')]: x.weightedGpValue ? (x.weightedGpValue).toFixed(2) :(0).toFixed(2),
              [this.translateKey('unweightedGPValue')]: x.unweightedGpValue ? (x.unweightedGpValue).toFixed(2) :(0).toFixed(2),
              [this.translateKey('comment')]: x.comment ? x.comment : '-'
            };
          });
          
          this.excelService.exportAsExcelFile(reportList, 'Report_Card_Grade_List_');
        } else {
          this.snackbar.open('No records found. failed to export report card grade list', '', {
            duration: 5000
          });
        }
      }
    });
  }

  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }
}
