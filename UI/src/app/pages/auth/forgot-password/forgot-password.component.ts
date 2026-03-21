import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { fadeInUp400ms } from '../../../../@vex/animations/fade-in-up.animation';
import { ForgotPasswordModel } from '../../../models/user.model';
import { LoginService } from '../../../services/login.service';
import { DefaultValuesService } from '../../../common/default-values.service';
import { CatalogDbService } from 'src/app/services/catalog-db.service';
import { AvailableTenantViewModel } from '../../../models/available-tenant';

@Component({
  selector: 'vex-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['../login/login.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  animations: [
    fadeInUp400ms
  ]
})
export class ForgotPasswordComponent implements OnInit {

  form: FormGroup;
  loading = false;
  submitted = false;
  tenantPhoto: string;
  tenantName: string;
  tenantFooter: string;

  constructor(
    private fb: FormBuilder,
    private cd: ChangeDetectorRef,
    private snackbar: MatSnackBar,
    private loginService: LoginService,
    private defaultValuesService: DefaultValuesService,
    private catalogDB: CatalogDbService,
  ) {
    let tenant = this.defaultValuesService.getDefaultTenant();
    this.defaultValuesService.setTenant(tenant);
    this.checkValidTenant();

    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]]
    });
  }

  ngOnInit() {}

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
    if (this.form.valid) {
      this.loading = true;
      let model = new ForgotPasswordModel();
      model.emailAddress = this.form.value.email.trim().toLowerCase();

      this.loginService.forgotPassword(model).subscribe(
        (res) => {
          this.loading = false;
          this.submitted = true;
          this.cd.markForCheck();
        },
        (error) => {
          this.loading = false;
          this.submitted = true;
          this.cd.markForCheck();
        }
      );
    }
  }
}
