import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import icVisibility from '@iconify/icons-ic/twotone-visibility';
import icVisibilityOff from '@iconify/icons-ic/twotone-visibility-off';
import { fadeInUp400ms } from '../../../../@vex/animations/fade-in-up.animation';
import { ResetPasswordByTokenModel } from '../../../models/user.model';
import { LoginService } from '../../../services/login.service';
import { DefaultValuesService } from '../../../common/default-values.service';
import { CatalogDbService } from 'src/app/services/catalog-db.service';
import { AvailableTenantViewModel } from '../../../models/available-tenant';

@Component({
  selector: 'vex-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['../login/login.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  animations: [
    fadeInUp400ms
  ]
})
export class ResetPasswordComponent implements OnInit {

  form: FormGroup;
  loading = false;
  resetToken: string;
  success = false;
  error = false;
  errorMessage = '';
  inputTypeNew = 'password';
  inputTypeConfirm = 'password';
  visibleNew = false;
  visibleConfirm = false;
  icVisibility = icVisibility;
  icVisibilityOff = icVisibilityOff;
  tenantPhoto: string;
  tenantName: string;
  tenantFooter: string;

  constructor(
    private fb: FormBuilder,
    private cd: ChangeDetectorRef,
    private route: ActivatedRoute,
    private snackbar: MatSnackBar,
    private loginService: LoginService,
    private defaultValuesService: DefaultValuesService,
    private catalogDB: CatalogDbService,
  ) {
    let tenant = this.defaultValuesService.getDefaultTenant();
    this.defaultValuesService.setTenant(tenant);
    this.checkValidTenant();

    this.form = this.fb.group({
      newPassword: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]]
    });
  }

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.resetToken = params['token'] || '';
      if (!this.resetToken) {
        this.error = true;
        this.errorMessage = 'Invalid or expired reset link.';
        this.cd.markForCheck();
      }
    });
  }

  checkValidTenant() {
    let tenant = this.defaultValuesService.getDefaultTenant();
    let model: AvailableTenantViewModel = new AvailableTenantViewModel();
    model.tenant.tenantName = tenant;
    this.catalogDB.validTenant(model).subscribe(
      (data) => {
        if (!data.failure) {
          this.tenantPhoto = data.tenant.tenantLogo;
          this.tenantName = data.tenant.tenantName;
          this.tenantFooter = data.tenant.tenantFooter;
          this.cd.markForCheck();
        }
      }
    );
  }

  submit() {
    this.form.markAllAsTouched();
    if (this.form.value.newPassword !== this.form.value.confirmPassword) {
      this.snackbar.open('Passwords do not match', '', { duration: 5000 });
      return;
    }
    if (this.form.valid) {
      this.loading = true;
      let model = new ResetPasswordByTokenModel();
      model.resetToken = this.resetToken;
      model.newPasswordHash = this.form.value.confirmPassword;

      this.loginService.resetPasswordByToken(model).subscribe(
        (res) => {
          this.loading = false;
          if (res._failure) {
            this.error = true;
            this.errorMessage = res._message;
          } else {
            this.success = true;
          }
          this.cd.markForCheck();
        },
        (err) => {
          this.loading = false;
          this.error = true;
          this.errorMessage = 'An error occurred. Please try again.';
          this.cd.markForCheck();
        }
      );
    }
  }

  toggleVisibilityNew() {
    this.visibleNew = !this.visibleNew;
    this.inputTypeNew = this.visibleNew ? 'text' : 'password';
    this.cd.markForCheck();
  }

  toggleVisibilityConfirm() {
    this.visibleConfirm = !this.visibleConfirm;
    this.inputTypeConfirm = this.visibleConfirm ? 'text' : 'password';
    this.cd.markForCheck();
  }
}
