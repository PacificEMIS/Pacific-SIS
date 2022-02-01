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

import { ChangeDetectorRef, Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import icClose from '@iconify/icons-ic/twotone-close';
import icVisibility from '@iconify/icons-ic/twotone-visibility';
import icVisibilityOff from '@iconify/icons-ic/twotone-visibility-off';
import { CommonService } from 'src/app/services/common.service';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ValidationService } from '../../shared/validation.service';
import { ResetPasswordModel } from 'src/app/models/common.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { SharedFunction } from '../../shared/shared-function';
import { LoaderService } from 'src/app/services/loader.service';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { DefaultValuesService } from 'src/app/common/default-values.service';

@Component({
  selector: 'vex-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss']
})
export class ResetPasswordComponent implements OnInit, OnDestroy {

  icClose = icClose;
  icVisibility = icVisibility;
  icVisibilityOff = icVisibilityOff;
  form: FormGroup;
  resetPasswordModel: ResetPasswordModel = new ResetPasswordModel();
  inputType = 'password';
  cnfInputType = 'password';
  destroySubject$: Subject<void> = new Subject();
  loading: boolean;

  constructor(public translateService: TranslateService,
    private commonService: CommonService,
    private snackbar: MatSnackBar,
    private fb: FormBuilder,
    private cd: ChangeDetectorRef,
    private loaderService: LoaderService,
    private commonFunction: SharedFunction,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private defaultValuesService: DefaultValuesService,
    private dialogRef: MatDialogRef<ResetPasswordComponent>) {
    //translateService.use('en');
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });
  }

  ngOnInit(): void {
    this.form = this.fb.group({
      newPassword: ['', [ValidationService.noWhitespaceValidator]],
      confirmNewPassword: ['', [ValidationService.noWhitespaceValidator]]
    }, {
      validator: ValidationService.mustMatch('newPassword', 'confirmNewPassword')
    });
  }

  // This toggleVisibility method is used for password visibility on/off.
  toggleVisibility(key) {
    if (this[key] === "text") {
      this[key] = 'password';
      this.cd.markForCheck();
    } else {
      this[key] = 'text';
      this.cd.markForCheck();
    }
  }

  // This generatePassword method is used for Auto generate password by calling the method autoGeneratePassword from commonFunction.
  generatePassword() {
    this.inputType = 'text';
    this.cnfInputType = 'text';
    let password = this.commonFunction.autoGeneratePassword();
    this.form.controls.newPassword.setValue(password);
    this.form.controls.confirmNewPassword.setValue(password);
  }

  // This submit method is used for call resetPassword API.
  submit() {
    this.form.markAllAsTouched();
    if (this.form.valid) {
      this.resetPasswordModel.userMaster.userId = this.data.userId;
      this.resetPasswordModel.userMaster.emailAddress = this.data.emailAddress;
      this.resetPasswordModel.userMaster.passwordHash = this.form.controls.confirmNewPassword.value;

      this.commonService.resetPassword(this.resetPasswordModel).subscribe(
        (res: ResetPasswordModel) => {
          if (res) {
          if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
              this.snackbar.open(res._message, '', {
                duration: 10000
              });
            }
            else {
              this.snackbar.open(res._message, '', {
                duration: 10000
              });
              this.dialogRef.close(true);
            }
          } else {
            this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
              duration: 10000
            });
          }
        }
      )
    }
  }

  // For destroy the isLoading subject.
  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }
}
