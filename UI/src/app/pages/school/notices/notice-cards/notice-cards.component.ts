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

import { Component, Input, OnInit } from '@angular/core';
import icPreview from '@iconify/icons-ic/round-preview';
import icPeople from '@iconify/icons-ic/twotone-people';
import icMoreVert from '@iconify/icons-ic/more-vert';
import { NoticeDeleteModel } from '../../../../models/notice-delete.model';
import { NoticeService } from '../../../../services/notice.service';
import { ActivatedRoute } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { NoticeAddViewModel, NoticeListViewModel } from '../../../../models/notice.model';
import { MatDialog } from '@angular/material/dialog';
import { EditNoticeComponent } from '../edit-notice/edit-notice.component';
import { TranslateService } from '@ngx-translate/core';
import { ConfirmDialogComponent } from '../../../../../app/pages/shared-module/confirm-dialog/confirm-dialog.component';
import { Permissions, RolePermissionListViewModel, RolePermissionViewModel } from '../../../../models/roll-based-access.model';
import { RollBasedAccessService } from '../../../../services/roll-based-access.service';
import { CryptoService } from '../../../../services/Crypto.service';
import { DefaultValuesService } from '../../../../common/default-values.service';
import { PageRolesPermission } from '../../../../common/page-roles-permissions.service';
import { CommonService } from 'src/app/services/common.service';
@Component({
  selector: 'vex-notice-cards',
  templateUrl: './notice-cards.component.html',
  styleUrls: ['./notice-cards.component.scss']
})
export class NoticeCardsComponent implements OnInit {
  noticeListViewModel: NoticeListViewModel = new NoticeListViewModel();
  noticeaddViewModel: NoticeAddViewModel = new NoticeAddViewModel();
  icPreview = icPreview;
  icPeople = icPeople;
  icMoreVert = icMoreVert;
  showMember = true;
  editPermission = false;
  deletePermission = false;
  addPermission = false;
  permissionListViewModel: RolePermissionListViewModel = new RolePermissionListViewModel();
  permissionGroup: RolePermissionViewModel = new RolePermissionViewModel();

  noticeDeleteModel = new NoticeDeleteModel();
  @Input() title: string;
  @Input() notice;
  @Input() noticeId: number;
  @Input() imageUrl: string;
  @Input() visibleFrom: string;
  @Input() visibleTo: number;
  @Input() getAllMembersList;
  @Input() recordFor;
  permissions: Permissions;
  constructor(
    private dialog: MatDialog,
    private noticeService: NoticeService,
    public translateService: TranslateService,
    public rollBasedAccessService: RollBasedAccessService,
    private Activeroute: ActivatedRoute,
    private snackbar: MatSnackBar,
    private pageRolePermissions: PageRolesPermission,
    private defaultValuesService: DefaultValuesService,
    private commonService: CommonService,
  ) {
    //translateService.use('en');
  }

  ngOnInit(): void {
    this.permissions = this.pageRolePermissions.checkPageRolePermission()

    if (this.visibleTo.toString() === '') {
      this.showMember = false;
    }
  }


  deleteNoticeConfirm(id) {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: "400px",
      data: {
        title: this.defaultValuesService.translateKey('areYouSure'),
        message: this.defaultValuesService.translateKey('youAreAboutToDelete') + this.title
      }
    });
    // listen to response
    dialogRef.afterClosed().subscribe(dialogResult => {
      // if user pressed yes dialogResult will be true, 
      // if user pressed no - it will be false
      if (dialogResult) {
        this.deleteNotice(id);
      }
    });
  }

  deleteNotice(id) {
    this.noticeDeleteModel.NoticeId = +id;
    this.noticeDeleteModel.schoolId = this.notice.schoolId;
    this.noticeService.deleteNotice(this.noticeDeleteModel).subscribe(
      (res) => {
        if (res._failure) {
          this.commonService.checkTokenValidOrNot(res._message);
          this.snackbar.open(res._message, '', {
            duration: 10000
          });
        } else {
          //  this.noticeService.getAllNotice(this.noticeListViewModel).subscribe((res) => {
          //    this.noticeListViewModel = res;
          //  });
          this.snackbar.open(res._message, '', {
            duration: 10000
          });
          this.noticeService.changeNotice({ status: true, recordFor: this.recordFor });
        }
      });
  }


  editNotice(noticeId: number) {
    this.dialog.open(EditNoticeComponent, {
      data: { allMembers: this.getAllMembersList, notice: this.notice, membercount: this.getAllMembersList.getAllMemberList.length },
      width: '800px'
    }).afterClosed().subscribe(
      res => {
        if (res) {
          this.noticeService.changeNotice({ status: true, recordFor: this.recordFor });
        }
      }
    );
  }
}
