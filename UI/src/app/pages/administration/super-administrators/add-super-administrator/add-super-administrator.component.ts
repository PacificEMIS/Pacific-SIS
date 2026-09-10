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

import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import icClose from '@iconify/icons-ic/twotone-close';
import icVisibility from '@iconify/icons-ic/twotone-visibility';
import icVisibilityOff from '@iconify/icons-ic/twotone-visibility-off';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { SuperAdministratorAddModel } from 'src/app/models/super-administrator.model';
import { CommonService } from 'src/app/services/common.service';
import { SuperAdministratorService } from 'src/app/services/super-administrator.service';
import { SharedFunction } from '../../../shared/shared-function';

/**
 * Creates a brand new Super Administrator: a staff record plus a login,
 * with no school attachment. The home school recorded on the staff row
 * is the school currently selected in the top bar.
 */
@Component({
  selector: 'vex-add-super-administrator',
  templateUrl: './add-super-administrator.component.html'
})
export class AddSuperAdministratorComponent implements OnInit {
  icClose = icClose;
  icVisibility = icVisibility;
  icVisibilityOff = icVisibilityOff;
  form: FormGroup;
  inputType = 'password';
  visible = false;
  saving = false;

  constructor(
    private dialogRef: MatDialogRef<AddSuperAdministratorComponent>,
    private fb: FormBuilder,
    private superAdministratorService: SuperAdministratorService,
    private defaultValuesService: DefaultValuesService,
    private commonService: CommonService,
    private commonFunction: SharedFunction,
    private snackbar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      firstGivenName: ['', Validators.required],
      middleName: [''],
      lastFamilyName: ['', Validators.required],
      loginEmailAddress: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  toggleVisibility() {
    this.visible = !this.visible;
    this.inputType = this.visible ? 'text' : 'password';
  }

  generate() {
    this.form.controls.password.setValue(this.commonFunction.autoGeneratePassword());
    this.visible = true;
    this.inputType = 'text';
  }

  submit() {
    this.form.markAllAsTouched();
    if (this.form.invalid || this.saving) {
      return;
    }
    const model = new SuperAdministratorAddModel();
    model.firstGivenName = this.form.value.firstGivenName;
    model.middleName = this.form.value.middleName;
    model.lastFamilyName = this.form.value.lastFamilyName;
    model.loginEmailAddress = this.form.value.loginEmailAddress;
    model.passwordHash = this.form.value.password;
    this.saving = true;
    this.superAdministratorService.add(model).subscribe((res) => {
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
