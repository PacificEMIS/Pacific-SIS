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

import { ChangeDetectionStrategy, AfterViewChecked, ChangeDetectorRef, Component, EventEmitter, Input, OnDestroy, OnInit } from '@angular/core';
import icArrowDropDown from '@iconify/icons-ic/arrow-drop-down';
import icAdd from '@iconify/icons-ic/add';
import { MatDialog } from '@angular/material/dialog';
import { fadeInUp400ms } from '../../../../@vex/animations/fade-in-up.animation';
import { stagger60ms } from '../../../../@vex/animations/stagger.animation';
import { EditNoticeComponent } from '../notices/edit-notice/edit-notice.component';
import { NoticeService } from '../../../services/notice.service';
import { NoticeListViewModel } from '../../../models/notice.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { MembershipService } from '../../../services/membership.service';
import { GetAllMembersList } from '../../../models/membership.model';
import moment from 'moment';
import { LoaderService } from '../../../services/loader.service';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { Permissions, RolePermissionListViewModel, RolePermissionViewModel } from '../../../models/roll-based-access.model';
import { RollBasedAccessService } from '../../../services/roll-based-access.service';
import { CryptoService } from '../../../services/Crypto.service';
import { DefaultValuesService } from '../../../common/default-values.service';
import { PageRolesPermission } from '../../../common/page-roles-permissions.service';
import { CommonService } from 'src/app/services/common.service';
@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'vex-notices',
  templateUrl: './notices.component.html',
  styleUrls: ['./notices.component.scss'],
  animations: [
    stagger60ms,
    fadeInUp400ms
  ]
})
export class NoticesComponent implements OnInit, AfterViewChecked, OnDestroy {

  @Input() afterClosed = new EventEmitter<boolean>();
  noticeListViewModel: NoticeListViewModel = new NoticeListViewModel();
  noticeList = [];
  cloneNoticeList = [];
  icPreview = icArrowDropDown;
  icAdd = icAdd;
  activateOpenAddNew = true;
  recordFor: string = 'current';
  getAllMembersList: GetAllMembersList = new GetAllMembersList();
  loading: boolean;
  destroySubject$: Subject<void> = new Subject();
  editPermission = false;
  deletePermission = false;
  addPermission = false;
  permissionListViewModel: RolePermissionListViewModel = new RolePermissionListViewModel();
  permissionGroup: RolePermissionViewModel = new RolePermissionViewModel();
  permissions: Permissions;
  constructor(
    private dialog: MatDialog,
    public translateService: TranslateService,
    private noticeService: NoticeService,
    private membershipService: MembershipService,
    private snackbar: MatSnackBar,
    private loaderService: LoaderService,
    public defaultValuesService: DefaultValuesService,
    private cdr: ChangeDetectorRef,
    public rollBasedAccessService: RollBasedAccessService,
    private pageRolePermissions: PageRolesPermission,
    private commonService: CommonService,
  ) {
    this.loaderService.isLoading.subscribe((v) => {
      this.loading = v;
    });
    this.noticeService.currentNotice.pipe(takeUntil(this.destroySubject$)).subscribe(
      (res: any) => {
        if (res.status) {
          this.recordFor = res.recordFor;
          this.getAllNotice();
        }
      }
    );

  }

  ngOnInit(): void {
    this.permissions = this.pageRolePermissions.checkPageRolePermission('/school/notices');

    this.getAllNotice();
    this.getMemberList();
  }
  ngAfterViewChecked() {
    this.cdr.detectChanges();
  }
  getMemberList() {
    this.membershipService.getAllMembers(this.getAllMembersList).subscribe(
      (res) => {
        if (res) {
          if (res._failure) {
            this.commonService.checkTokenValidOrNot(res._message);
            if (!res.getAllMemberList) {
              this.snackbar.open(res._message, '', {
                duration: 10000
              });
            }
          }
          else {
            this.getAllMembersList = res;
          }

        }
        else{
          this.snackbar.open('No Member Found. ' + this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
      });
  }

  getAllNotice() {
    this.noticeListViewModel.membershipId = + this.defaultValuesService.getuserMembershipID();
    this.noticeService.getAllNotice(this.noticeListViewModel).subscribe((res) => {
      if (res) {
        if (res._failure) {
          this.commonService.checkTokenValidOrNot(res._message);
          this.noticeList = [];
          this.cloneNoticeList = [];
          if (!res.noticeList) {
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
        } else {
          this.noticeList = res.noticeList;
          this.cloneNoticeList = JSON.parse(JSON.stringify(res.noticeList));
          this.showRecords(this.recordFor);
        }
      } else {
        this.snackbar.open('No Notice Found. ' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    });
  }

  showRecords(event) {
    this.recordFor = event;

    if (event !== 'current') {
      if (this.recordFor.toLowerCase() === 'upcoming') {
        this.noticeList = this.cloneNoticeList.filter(
          m => {

            const validFrom = moment(m.validFrom).format('YYYY-MM-DD').toString();
            const today = moment().format('YYYY-MM-DD').toString();

            // For upcoming
            if (moment(validFrom).isAfter(today)) {
              return m;
            }
          }
        );
      } else if (this.recordFor.toLowerCase() === 'past') {
        this.noticeList = this.cloneNoticeList.filter(
          m => {

            const validTo = moment(m.validTo).format('YYYY-MM-DD').toString();
            const today = moment().format('YYYY-MM-DD').toString();

            // For past
            if (moment(validTo).isBefore(today)) {
              return m;
            }
          }
        );
      }
    } else {
      this.noticeList = this.cloneNoticeList.filter(
        m => {

          const validFrom = moment(m.validFrom).format('YYYY-MM-DD').toString();
          const validTo = moment(m.validTo).format('YYYY-MM-DD').toString();
          const today = moment().format('YYYY-MM-DD').toString();

          // For current
          if (moment(today).isBetween(validFrom, validTo, undefined, '[]')) {
            return m;
          }
        }
      );
    }
  }

  openAddNew() {
    this.dialog.open(EditNoticeComponent, {
      data: { allMembers: this.getAllMembersList, membercount: this.getAllMembersList.getAllMemberList.length },
      width: '800px'
    }).afterClosed().subscribe(
      res => {
        if (res) {
          this.getAllNotice();
        }
      }
    );
  }

  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
    this.noticeService.changeNotice(false);
  }
}
