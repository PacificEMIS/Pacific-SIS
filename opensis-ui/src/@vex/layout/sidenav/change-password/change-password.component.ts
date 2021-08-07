import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit, ViewEncapsulation } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import icClose from '@iconify/icons-ic/twotone-close';
import { MatPasswordStrengthComponent } from "@angular-material-extensions/password-strength";
import { ChangePasswordViewModel } from '../../../../app/models/common.model';
import { CommonService } from '../../../../app/services/common.service';
import { FormBuilder, FormGroup } from '@angular/forms';
import icVisibility from '@iconify/icons-ic/twotone-visibility';
import icVisibilityOff from '@iconify/icons-ic/twotone-visibility-off';
import { MatSnackBar } from '@angular/material/snack-bar';
import { SharedFunction } from '../../../../app/pages/shared/shared-function';
import { DefaultValuesService } from '../../../../app/common/default-values.service';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'vex-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.scss'],
  encapsulation: ViewEncapsulation.None,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ChangePasswordComponent implements OnInit {
  icClose = icClose;
  icVisibility = icVisibility;
  icVisibilityOff = icVisibilityOff;
  changePasswordViewModel: ChangePasswordViewModel = new ChangePasswordViewModel();
  inputType = 'password';
  form: FormGroup;

  constructor(public translateService: TranslateService,
    private commonService: CommonService,
    private snackbar: MatSnackBar,
    private commonFunction: SharedFunction,
    private dialog: MatDialog,
    private cd: ChangeDetectorRef,
    private defaultValuesService: DefaultValuesService,
    private fb: FormBuilder) {
    translateService.use("en");
    this.form = fb.group({
      currentPasswordHash: [''],
      newPasswordHash: [''],
      confirmPasswordHash: ['']

    })
  }

  ngOnInit(): void {
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

  generate() {
    this.inputType = 'text';
    this.form.controls.newPasswordHash.setValue(this.commonFunction.autoGeneratePassword());
  }

  submit() {
    this.form.markAllAsTouched();
    if (this.form.valid) {
      if (this.form.controls.confirmPasswordHash.value === this.form.controls.newPasswordHash.value) {
        this.changePasswordViewModel.currentPasswordHash = this.form.controls.currentPasswordHash.value;
        this.changePasswordViewModel.newPasswordHash = this.form.controls.newPasswordHash.value;
        this.commonService.changePassword(this.changePasswordViewModel).subscribe(
          (res) => {
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
                this.dialog.closeAll();
              }
            }
            else {
              this.snackbar.open(res._message, '', {
                duration: 10000
              });
            }
          }
        );
      }
      else {
        this.snackbar.open(this.defaultValuesService.translateKey('confirmNewPasswordNotMathedWithNewPassword'), '', {
          duration: 10000
        });
      }

    }
  }

}
