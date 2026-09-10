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

import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import icClose from '@iconify/icons-ic/twotone-close';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { OnlySchoolListModel } from 'src/app/models/get-all-school.model';
import { SchoolMasterModel } from 'src/app/models/school-master.model';
import { SuperAdministratorActionModel, SuperAdministratorModel } from 'src/app/models/super-administrator.model';
import { CommonService } from 'src/app/services/common.service';
import { SchoolService } from 'src/app/services/school.service';
import { SuperAdministratorService } from 'src/app/services/super-administrator.service';

/**
 * Turns a Super Administrator back into a school-level staff member.
 * A Super Administrator is attached to no school, so the dialog asks
 * which school and which profile the person will have afterwards. For a
 * staff who was promoted earlier the previous home school and profile
 * are preselected.
 */
@Component({
  selector: 'vex-demote-super-administrator',
  templateUrl: './demote-super-administrator.component.html'
})
export class DemoteSuperAdministratorComponent implements OnInit {
  icClose = icClose;
  form: FormGroup;
  schools: SchoolMasterModel[] = [];
  profileTypes = ['School Administrator', 'Admin Assistant', 'Teacher', 'Homeroom Teacher'];
  saving = false;

  constructor(
    private dialogRef: MatDialogRef<DemoteSuperAdministratorComponent>,
    @Inject(MAT_DIALOG_DATA) public data: SuperAdministratorModel,
    private fb: FormBuilder,
    private superAdministratorService: SuperAdministratorService,
    private schoolService: SchoolService,
    private defaultValuesService: DefaultValuesService,
    private commonService: CommonService,
    private snackbar: MatSnackBar
  ) { }

  ngOnInit(): void {
    const previousProfile = this.profileTypes.includes(this.data?.previousProfile) ? this.data.previousProfile : '';
    this.form = this.fb.group({
      targetSchoolId: [this.data?.previousSchoolId ?? +this.defaultValuesService.getSchoolID(), Validators.required],
      targetProfileType: [previousProfile, Validators.required]
    });
    this.loadSchools();
  }

  fullName() {
    return [this.data?.firstGivenName, this.data?.middleName, this.data?.lastFamilyName].filter(x => x).join(' ');
  }

  loadSchools() {
    const model = new OnlySchoolListModel();
    model.emailAddress = this.defaultValuesService.getEmailId();
    this.schoolService.GetAllSchools(model).subscribe((res) => {
      if (typeof (res) === 'undefined') {
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', { duration: 10000 });
        return;
      }
      if (res._failure) {
        this.commonService.checkTokenValidOrNot(res._message);
        this.snackbar.open(res._message, '', { duration: 10000 });
        return;
      }
      this.schools = res.getSchoolForView || [];
    });
  }

  submit() {
    this.form.markAllAsTouched();
    if (this.form.invalid || this.saving) {
      return;
    }
    const model = new SuperAdministratorActionModel();
    model.staffId = this.data.staffId;
    model.targetSchoolId = +this.form.value.targetSchoolId;
    model.targetProfileType = this.form.value.targetProfileType;
    this.saving = true;
    this.superAdministratorService.demote(model).subscribe((res) => {
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
}
