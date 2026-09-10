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

import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import icClose from '@iconify/icons-ic/twotone-close';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import {
  SuperAdministratorActionModel,
  SuperAdministratorCandidateListModel,
  SuperAdministratorCandidateModel
} from 'src/app/models/super-administrator.model';
import { CommonService } from 'src/app/services/common.service';
import { SuperAdministratorService } from 'src/app/services/super-administrator.service';

/**
 * Promotes an existing staff member who already has a portal login.
 * They keep their staff record and school attachments; only the login
 * membership and the staff profile change.
 */
@Component({
  selector: 'vex-promote-staff',
  templateUrl: './promote-staff.component.html'
})
export class PromoteStaffComponent implements OnInit, OnDestroy {
  icClose = icClose;
  searchCtrl = new FormControl('');
  candidates: SuperAdministratorCandidateModel[] = [];
  selected: SuperAdministratorCandidateModel = null;
  searched = false;
  saving = false;
  destroySubject$: Subject<void> = new Subject();

  constructor(
    private dialogRef: MatDialogRef<PromoteStaffComponent>,
    private superAdministratorService: SuperAdministratorService,
    private defaultValuesService: DefaultValuesService,
    private commonService: CommonService,
    private snackbar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.searchCtrl.valueChanges.pipe(debounceTime(400), distinctUntilChanged(), takeUntil(this.destroySubject$))
      .subscribe((text) => this.search(text));
    this.search('');
  }

  search(text: string) {
    const model = new SuperAdministratorCandidateListModel();
    model.searchText = text;
    this.superAdministratorService.getPromotionCandidates(model).subscribe((res) => {
      this.searched = true;
      if (typeof (res) === 'undefined') {
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', { duration: 10000 });
        return;
      }
      if (res._failure) {
        this.commonService.checkTokenValidOrNot(res._message);
        this.candidates = [];
        this.snackbar.open(res._message, '', { duration: 10000 });
        return;
      }
      this.candidates = res.candidates;
      if (this.selected && !this.candidates.some(c => c.staffId === this.selected.staffId)) {
        this.selected = null;
      }
    });
  }

  fullName(row: SuperAdministratorCandidateModel) {
    return [row.firstGivenName, row.middleName, row.lastFamilyName].filter(x => x).join(' ');
  }

  select(row: SuperAdministratorCandidateModel) {
    this.selected = row;
  }

  submit() {
    if (!this.selected || this.saving) {
      return;
    }
    const model = new SuperAdministratorActionModel();
    model.staffId = this.selected.staffId;
    this.saving = true;
    this.superAdministratorService.promote(model).subscribe((res) => {
      this.saving = false;
      if (typeof (res) === 'undefined') {
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', { duration: 10000 });
        return;
      }
      if (res._failure) {
        this.commonService.checkTokenValidOrNot(res._message);
        this.snackbar.open(res._message, '', { duration: 10000 });
        return;
      }
      this.snackbar.open(res._message, '', { duration: 10000 });
      this.dialogRef.close(true);
    });
  }

  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }
}
