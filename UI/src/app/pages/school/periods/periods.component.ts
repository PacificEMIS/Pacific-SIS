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

import { Component, OnInit, ViewChild } from '@angular/core';
import icAdd from '@iconify/icons-ic/baseline-add';
import icEdit from '@iconify/icons-ic/twotone-edit';
import icDelete from '@iconify/icons-ic/twotone-delete';
import icSearch from '@iconify/icons-ic/search';
import icFilterList from '@iconify/icons-ic/filter-list';
import { fadeInUp400ms } from '../../../../@vex/animations/fade-in-up.animation';
import { stagger40ms } from '../../../../@vex/animations/stagger.animation';
import { TranslateService } from '@ngx-translate/core';
import { MatDialog } from '@angular/material/dialog';
import { EditBlockComponent } from './edit-block/edit-block.component';
import { SchoolPeriodService } from '../../../services/school-period.service';
import { BlockAddViewModel, BlockListViewModel, BlockPeriodAddViewModel, BlockPeriodForHalfDayFullDayModel, BlockPeriodSortOrderViewModel } from '../../../models/school-period.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableDataSource } from '@angular/material/table';
import { ConfirmDialogComponent } from '../../shared-module/confirm-dialog/confirm-dialog.component';
import { MatSort } from '@angular/material/sort';
import { EditPeriodComponent } from './edit-period/edit-period.component';
import { LoaderService } from '../../../services/loader.service';
import { CdkDragDrop } from '@angular/cdk/drag-drop';
import { ExcelService } from '../../../services/excel.service';
import * as moment from 'moment';
import { Permissions, RolePermissionListViewModel, RolePermissionViewModel } from '../../../models/roll-based-access.model';
import { RollBasedAccessService } from '../../../services/roll-based-access.service';
import { CryptoService } from '../../../services/Crypto.service';
import { PageRolesPermission} from '../../../common/page-roles-permissions.service'
import { CommonService } from 'src/app/services/common.service';
import { DefaultValuesService } from 'src/app/common/default-values.service';
@Component({
  selector: 'vex-periods',
  templateUrl: './periods.component.html',
  styleUrls: ['./periods.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})
export class PeriodsComponent implements OnInit {

  icAdd = icAdd;
  icEdit = icEdit;
  icDelete = icDelete;
  icSearch = icSearch;
  icFilterList = icFilterList;
  blockPeriodList: MatTableDataSource<any>;
  @ViewChild(MatSort) sort: MatSort;
  blockCount:number;
  blockListViewModel: BlockListViewModel = new BlockListViewModel();
  blockPeriodSortOrderViewModel: BlockPeriodSortOrderViewModel = new BlockPeriodSortOrderViewModel();
  blockPeriodAddViewModel: BlockPeriodAddViewModel = new BlockPeriodAddViewModel();
  blockAddViewModel: BlockAddViewModel = new BlockAddViewModel();
  currentBlockId: number = null;
  loading: boolean;
  closeFullDayMinutes:any;
  closeHalfDayMinutes:any;
  blockPeriodForHalfDayFullDayModel: BlockPeriodForHalfDayFullDayModel = new BlockPeriodForHalfDayFullDayModel();

  columns = [
    { label: 'ID', property: 'periodId', type: 'number', visible: true },
    { label: 'Title', property: 'periodTitle', type: 'text', visible: true },
    { label: 'Short Name', property: 'periodShortName', type: 'text', visible: true },
    { label: 'Start Time', property: 'periodStartTime', type: 'text', visible: true },
    { label: 'End Time', property: 'periodEndTime', type: 'text', visible: true },
    { label: 'Length', property: 'length', type: 'number', visible: true },
    { label: 'Calculate Attendance', property: 'calculateAttendance', type: 'text', visible: true },
    { label: 'Action', property: 'action', type: 'text', visible: true }
  ];
  searchKey: string;
  permissions: Permissions;
  viewSetMinutes = true;
  editSetminutes = false;

  constructor(
    public translateService: TranslateService,
    private dialog: MatDialog,
    private snackbar: MatSnackBar,
    private schoolPeriodService: SchoolPeriodService,
    private excelService : ExcelService,
    private loaderService: LoaderService,
    public rollBasedAccessService: RollBasedAccessService,
    private pageRolePermissions: PageRolesPermission,
    private cryptoService: CryptoService,
    private commonService: CommonService,
    public defaultValuesService: DefaultValuesService,
    ) {
    this.loaderService.isLoading.subscribe((val) => {
      this.loading = val;
    });
  }

  ngOnInit(): void {
    this.permissions = this.pageRolePermissions.checkPageRolePermission('/school/settings/school-settings/periods')
    this.getAllBlockList();
  }

  openSetMinutes(){
    this.viewSetMinutes = false;
    this.editSetminutes = true;
    this.closeFullDayMinutes=this.blockPeriodForHalfDayFullDayModel.block.fullDayMinutes;
    this.closeHalfDayMinutes=this.blockPeriodForHalfDayFullDayModel.block.halfDayMinutes;
  }

  closeSetMinutes(){
    this.viewSetMinutes = true;
    this.editSetminutes = false;
    this.blockPeriodForHalfDayFullDayModel.block.fullDayMinutes=this.closeFullDayMinutes;
    this.blockPeriodForHalfDayFullDayModel.block.halfDayMinutes=this.closeHalfDayMinutes;
  }

  selectBlock(element) {
    this.currentBlockId = element.blockId;
    this.setFullDayHalDayMinutes(element);
    this.viewSetMinutes = true;
    this.editSetminutes = false;
    let periodList = element.blockPeriod?.map(function (item) {
      let length = Math.round(((new Date("1900-01-01T" + item.periodEndTime).getTime() - new Date("1900-01-01T" + item.periodStartTime).getTime())) / 60000);
      if (length < 0) {
        length += 1440;
      }
      return {
        blockId: item.blockId,
        periodId: item.periodId,
        periodTitle: item.periodTitle,
        periodShortName: item.periodShortName,
        periodStartTime: new Date("1900-01-01T" + item.periodStartTime),
        periodEndTime:  new Date("1900-01-01T" + item.periodEndTime),
        calculateAttendance: item.calculateAttendance,
        sortOrder:item.periodSortOrder,
        length
      };
    });
    this.blockPeriodList = new MatTableDataSource(periodList);
    this.blockPeriodList.sort = this.sort;
  }
  getAllBlockList() {
    this.blockListViewModel.isListView=true;
    this.schoolPeriodService.getAllBlockList(this.blockListViewModel).subscribe(
      (res: BlockListViewModel) => {
        if (typeof (res) == 'undefined') {
          this.snackbar.open('' + this.defaultValuesService.getHttpError(), '', {
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
            this.blockListViewModel = res;
            this.blockCount= res.getBlockListForView.length;
            if (this.currentBlockId == null) {
              this.currentBlockId = res.getBlockListForView[0].blockId;
              this.setFullDayHalDayMinutes(res.getBlockListForView[0]);
              
              let periodList= this.itemInPeriodList(0 , res);
              this.blockPeriodList = new MatTableDataSource(periodList);
              this.blockPeriodList.sort = this.sort;
            }
            else {
              let index = this.blockListViewModel.getBlockListForView.findIndex((x) => {
                return x.blockId === this.currentBlockId
              });
              let periodList= this.itemInPeriodList(index, res);
              this.blockPeriodList = new MatTableDataSource(periodList);
              this.blockPeriodList.sort = this.sort;

              this.setFullDayHalDayMinutes(res.getBlockListForView[index]);

            }
          }
        }
      }
    );
  }

  itemInPeriodList(index=0,response){
    let periodList = response.getBlockListForView[index].blockPeriod?.map(function (item) {
      let length = Math.round(((new Date("1900-01-01T" + item.periodEndTime).getTime() - new Date("1900-01-01T" + item.periodStartTime).getTime())) / 60000);
      if (length < 0) {
        length += 1440;
      }
      return {
        blockId: item.blockId,
        periodId: item.periodId,
        periodTitle: item.periodTitle,
        periodShortName: item.periodShortName,
        periodStartTime: new Date("1900-01-01T" + item.periodStartTime),
        periodEndTime:  new Date("1900-01-01T" + item.periodEndTime),
        calculateAttendance: item.calculateAttendance,
        sortOrder:item.periodSortOrder,
        length
      };
    });
    return periodList;
  }

  translateKey(key) {
    let trnaslateKey;
   this.translateService.get(key).subscribe((res: string) => {
       trnaslateKey = res;
    });
    return trnaslateKey;
  }

  excelPeriodList(index=0,response){
    
    let periodList = response.getBlockListForView[index].blockPeriod?.map((item)=> {
      let length = Math.round(((new Date("1900-01-01T" + item.periodEndTime).getTime() - new Date("1900-01-01T" + item.periodStartTime).getTime())) / 60000);
      if (length < 0) {
        length += 1440;
      }
      return {
        [this.translateKey('title') ]: item.periodTitle,
        [this.translateKey('shortName')]: item.periodShortName,
        [this.translateKey('startTime')]: moment(new Date("1900-01-01T" + item.periodStartTime), ["YYYY-MM-DD hh:mm:ss"]).format("hh:mm A"),
        [this.translateKey('endTime')]:  moment(new Date("1900-01-01T" + item.periodEndTime), ["YYYY-MM-DD hh:mm:ss"]).format("hh:mm A"),
        [this.translateKey('lengthInMinutes')]: length,
        [this.translateKey('calculateAttendance')]: item.calculateAttendance? this.translateKey('yes'): this.translateKey('no')
      };
    });
    return periodList;
  }


  editBlock(element) {
    this.dialog.open(EditBlockComponent, {
      data: element,
      width: '500px'
    }).afterClosed().subscribe((data) => {
      if (data === 'submited') {
        this.getAllBlockList();
      }
    })
  }
  deleteBlock(element) {
    this.blockAddViewModel.block = element
    this.schoolPeriodService.deleteBlock(this.blockAddViewModel).subscribe(
      (res: BlockAddViewModel) => {
        if (typeof (res) == 'undefined') {
          this.snackbar.open('' + this.defaultValuesService.getHttpError(), '', {
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
            this.currentBlockId= null;
            this.setFullDayHalDayMinutes(null);
            this.getAllBlockList();
          }
        }
      }
    )
  }
  confirmDeleteBlock(element) {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: "400px",
      data: {
        title: "Are you sure?",
        message: "You are about to delete " + element.blockTitle + "."
      }
    });
    dialogRef.afterClosed().subscribe(dialogResult => {
      if (dialogResult) {
        this.deleteBlock(element);
      }
    });
  }


  goToAddBlock() {
    this.dialog.open(EditBlockComponent, {
      width: '500px'
    }).afterClosed().subscribe((data) => {
      if (data.mode === 'submited') {
        this.currentBlockId = data.currentBlockId;
        this.setFullDayHalDayMinutes(data);
        this.getAllBlockList();
      }
    });
  }

  setFullDayHalDayMinutes(data) {
    if(data) {
      this.blockPeriodForHalfDayFullDayModel.block.blockId = data.blockId;
      this.blockPeriodForHalfDayFullDayModel.block.fullDayMinutes = data.fullDayMinutes ? data.fullDayMinutes : 0;
      this.blockPeriodForHalfDayFullDayModel.block.halfDayMinutes = data.halfDayMinutes ? data.halfDayMinutes : 0;
    } else {
      this.blockPeriodForHalfDayFullDayModel.block.blockId = null;
      this.blockPeriodForHalfDayFullDayModel.block.fullDayMinutes = null;
      this.blockPeriodForHalfDayFullDayModel.block.halfDayMinutes = null;
    }   
  }

  updateFullDayHalfDayMinutes() {
    if((this.blockPeriodForHalfDayFullDayModel.block.fullDayMinutes>0 && this.blockPeriodForHalfDayFullDayModel.block.halfDayMinutes>0)){
        if(this.blockPeriodForHalfDayFullDayModel.block.fullDayMinutes > this.blockPeriodForHalfDayFullDayModel.block.halfDayMinutes) {
        this.schoolPeriodService.updateFullDayHalfDayMinutes(this.blockPeriodForHalfDayFullDayModel).subscribe(
          (res: BlockPeriodForHalfDayFullDayModel) => {
        if(res) {
          if(res._failure){
            this.commonService.checkTokenValidOrNot(res._message);
                    this.snackbar.open( res._message, '', {
                      duration: 10000
                    });
                  }
                  else {
                    this.viewSetMinutes = true;
                    this.editSetminutes = false;
                    this.getAllBlockList();
                    this.snackbar.open( res._message, '', {
                      duration: 10000
                    });
                  }
        } else {
          this.snackbar.open('Update failed. ' + this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
      }
    )
     }  else if((this.blockPeriodForHalfDayFullDayModel.block.fullDayMinutes < this.blockPeriodForHalfDayFullDayModel.block.halfDayMinutes)){
      this.snackbar.open( this.defaultValuesService.translateKey('halfDayMinutesShouldBeLessThanFullDayMinutes'), '', {
        duration: 10000
      });
    }
     else {
      this.snackbar.open( this.defaultValuesService.translateKey('halfDayMinutesShouldBeLessThanFullDayMinutes'), '', {
        duration: 10000
      });
     }
    }
    else{
      this.snackbar.open( this.defaultValuesService.translateKey('fullDayAndHalfDayMinutesCannotBeBlank'), '', {
        duration: 10000
      });
    }
  }


  exportPeriodListToExcel(){
    this.schoolPeriodService.getAllBlockList(this.blockListViewModel).subscribe(
      (res: BlockListViewModel) => {
        if (typeof (res) == 'undefined') {
          this.snackbar.open('' + this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
        else {
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
        if (!res.getBlockListForView) {
          this.snackbar.open(res._message, '', {
            duration: 10000
          });
        }
          }
          else {
            this.blockListViewModel = res;
            if (this.currentBlockId == null) {
              this.currentBlockId = res.getBlockListForView[0].blockId;
              this.setFullDayHalDayMinutes(res.getBlockListForView[0]);

              let periodList= this.excelPeriodList(0, res);
              if(periodList.length!=0){
                this.excelService.exportAsExcelFile(periodList,'Period_List_')
              }
              else{
                this.snackbar.open('No Records Found. Failed to Export Period List','', {
                  duration: 5000
                });
              }
              
            }
            else {
              let index = this.blockListViewModel.getBlockListForView.findIndex((x) => {
                return x.blockId === this.currentBlockId
              });
              let periodList= this.excelPeriodList(index, res);
              if(periodList.length!=0){
                this.excelService.exportAsExcelFile(periodList,'Period_List_')
              }
              else{
                this.snackbar.open('No Records Found. Failed to Export Period List','', {
                  duration: 5000
                });
              }
              
            }
          }
        }
      }
    );

  }

  getPageEvent(event) {
    // this.getAllSchool.pageNumber=event.pageIndex+1;
    // this.getAllSchool.pageSize=event.pageSize;
    // this.callAllSchool(this.getAllSchool);
  }

  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  onSearchClear() {
    this.searchKey = "";
    this.applyFilter();
  }
  applyFilter() {
    this.blockPeriodList.filter = this.searchKey.trim().toLowerCase();
  }

  dropPeriodList(event: CdkDragDrop<string[]>) {
    this.blockPeriodSortOrderViewModel.blockId = this.currentBlockId;
    this.blockPeriodSortOrderViewModel.currentSortOrder = this.blockPeriodList.data[event.currentIndex].sortOrder
    this.blockPeriodSortOrderViewModel.previousSortOrder = this.blockPeriodList.data[event.previousIndex].sortOrder
    this.schoolPeriodService.updateBlockPeriodSortOrder(this.blockPeriodSortOrderViewModel).subscribe(
      (res: BlockPeriodSortOrderViewModel) => {
        if (typeof (res) == 'undefined') {
          this.snackbar.open('Period Drag failed. ' + this.defaultValuesService.getHttpError(), '', {
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
            this.getAllBlockList();
          }
        }
      }
    );
  }


  goToAddPeriod() {
    this.dialog.open(EditPeriodComponent, {
      data: { blockId: this.currentBlockId },
      width: '500px'
    }).afterClosed().subscribe((data) => {
      if (data === 'submited') {
        this.getAllBlockList();
      }
    });
  }

  editPeriod(element) {
    this.dialog.open(EditPeriodComponent, {
      data: { periodData: element },
      width: '500px'
    }).afterClosed().subscribe((data) => {
      if (data === 'submited') {
        this.getAllBlockList();
      }
    });
  }

  confirmDeletePeriod(element) {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: "400px",
      data: {
        title: "Are you sure?",
        message: "You are about to delete " + element.periodTitle + "."
      }
    });
    dialogRef.afterClosed().subscribe(dialogResult => {
      if (dialogResult) {
        this.deletePeriod(element);
      }
    });
  }

  deletePeriod(element) {
    this.blockPeriodAddViewModel.blockPeriod = element;
    this.schoolPeriodService.deleteBlockPeriod(this.blockPeriodAddViewModel).subscribe(
      (res: BlockPeriodAddViewModel) => {
        if (typeof (res) == 'undefined') {
          this.snackbar.open('Period deletion failed. ' + this.defaultValuesService.getHttpError(), '', {
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
            this.getAllBlockList()
          }
        }
      }
    )
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

}
