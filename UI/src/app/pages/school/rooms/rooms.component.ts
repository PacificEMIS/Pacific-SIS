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
import { MatDialog } from '@angular/material/dialog';
import { fadeInUp400ms } from '../../../../@vex/animations/fade-in-up.animation';
import { stagger60ms } from '../../../../@vex/animations/stagger.animation';
import icEdit from '@iconify/icons-ic/twotone-edit';
import icDelete from '@iconify/icons-ic/twotone-delete';
import icSearch from '@iconify/icons-ic/twotone-search';
import icAdd from '@iconify/icons-ic/twotone-add';
import icFilterList from '@iconify/icons-ic/twotone-filter-list';
import { EditRoomComponent } from '../rooms/edit-room/edit-room.component';
import { RoomDetailsComponent } from '../rooms/room-details/room-details.component';

import { RoomAddView, RoomListViewModel } from '../../../models/room.model';
import { RoomService } from '../../../services/room.service';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { TranslateService } from '@ngx-translate/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ConfirmDialogComponent } from '../../shared-module/confirm-dialog/confirm-dialog.component';
import { LoaderService } from '../../../services/loader.service';
import { ExcelService } from '../../../services/excel.service';
import { CryptoService } from '../../../services/Crypto.service';
import { Permissions, RolePermissionListViewModel, RolePermissionViewModel } from '../../../models/roll-based-access.model';
import { PageRolesPermission } from '../../../common/page-roles-permissions.service';
import { CommonService } from 'src/app/services/common.service';
import { DefaultValuesService } from 'src/app/common/default-values.service';
@Component({
  selector: 'vex-rooms',
  templateUrl: './rooms.component.html',
  styleUrls: ['./rooms.component.scss'],
  animations: [
    stagger60ms,
    fadeInUp400ms
  ]
})
export class RoomsComponent implements OnInit {

  icEdit = icEdit;
  icDelete = icDelete;
  icSearch = icSearch;
  icAdd = icAdd;
  icFilterList = icFilterList;
  roomaddviewmodel: RoomAddView = new RoomAddView();
  roomListViewModel: RoomListViewModel = new RoomListViewModel();
  roomDetails: any;
  loading: boolean;
  searchKey: string;
  permissions: Permissions;
  constructor(
    private dialog: MatDialog,
    private roomService: RoomService,
    private snackbar: MatSnackBar,
    private excelService: ExcelService,
    private translateService: TranslateService,
    private loaderService: LoaderService,
    private cryptoService: CryptoService,
    private pageRolePermissions: PageRolesPermission,
    private commonService: CommonService,
    public defaultValuesService: DefaultValuesService
  ) {
    this.loaderService.isLoading.subscribe((val) => {
      this.loading = val;
    });
  }
  roomModelList: MatTableDataSource<any>;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  columns = [
    { label: 'Title', property: 'title', type: 'text', visible: true },
    { label: 'Capacity', property: 'capacity', type: 'text', visible: true, cssClasses: ['font-medium'] },
    { label: 'Sort Order', property: 'sortOrder', type: 'text', visible: true, cssClasses: ['text-secondary', 'font-medium'] },
    { label: 'isActive', property: 'isActive', type: 'text', visible: true, cssClasses: ['text-secondary', 'font-medium'] },
    { label: 'Action', property: 'action', type: 'text', visible: true },
  ];

  ngOnInit(): void {
    this.permissions = this.pageRolePermissions.checkPageRolePermission('/school/settings/school-settings/rooms')
    this.roomListViewModel.schoolId = this.defaultValuesService.getSchoolID();
    this.getAllRooms();
  }
  getAllRooms() {
    this.roomListViewModel.includeInactive = true;
    this.roomListViewModel.isListView=true;
    this.roomService.getAllRoom(this.roomListViewModel).subscribe(
      (res: RoomListViewModel) => {
        if (typeof(res) === 'undefined'){
          this.snackbar.open('' + this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
        else {
          if (res._failure) {
            this.commonService.checkTokenValidOrNot(res._message);
            this.roomModelList = new MatTableDataSource([]);
            this.roomModelList.sort = this.sort;
            if (!res.tableroomList) {
              this.snackbar.open(res._message, '', {
                duration: 10000
              });
            }
          }
          else {
            this.roomModelList = new MatTableDataSource(res.tableroomList);
            this.roomModelList.sort = this.sort;
          }
        }
      });
  }

  openAddNew() {
    this.dialog.open(EditRoomComponent, {
      data: null,
      width: '600px'
    }).afterClosed().subscribe(data => {
      if (data === 'submited') {
        this.getAllRooms();
      }
    });
  }

  openViewDetails(element) {
    this.dialog.open(RoomDetailsComponent, {
      data: { info: element },
      width: '600px'
    });
  }

  onSearchClear() {
    this.searchKey = '';
    this.applyFilter();
  }

  applyFilter() {
    this.roomModelList.filter = this.searchKey.trim().toLowerCase();
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  openEditdata(element) {
    this.dialog.open(EditRoomComponent, {
      data: element,
      width: '600px'
    }).afterClosed().subscribe((data) => {
      if (data === 'submited') {
        this.getAllRooms();
      }
    });
  }
  deleteRoomdata(element) {
    this.roomaddviewmodel.tableRoom = element;
    this.roomService.deleteRoom(this.roomaddviewmodel).subscribe(
      (res: RoomAddView) => {
        if (res) {
          if (res._failure) {
            this.commonService.checkTokenValidOrNot(res._message);
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
          else {
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
            this.getAllRooms();
          }
        }
        else{
          this.snackbar.open( this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
      }
    );
  }
  confirmDelete(element) {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: '400px',
      data: {
        title: 'Are you sure?',
        message: 'You are about to delete ' + element.title + '.'
      }
    });
    dialogRef.afterClosed().subscribe(dialogResult => {
      if (dialogResult) {
        this.deleteRoomdata(element);
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

  exportToExcel() {
    if (this.roomModelList.data?.length > 0) {
      const reportList = this.roomModelList.data?.map((x) => {
        return {
          [this.translateKey('title')]: x.title,
          [this.translateKey('capacity')]: x.capacity,
          [this.translateKey('description')]: x.description,
          [this.translateKey('sortOrder')]: x.sortOrder,
          [this.translateKey('active')]: x.isActive ? this.translateKey('yes') : this.translateKey('no')
        };
      });
      this.excelService.exportAsExcelFile(reportList, 'Rooms_List_');
    } else {
      this.snackbar.open('No records found. failed to export Rooms List', '', {
        duration: 5000
      });
    }
  }
}
