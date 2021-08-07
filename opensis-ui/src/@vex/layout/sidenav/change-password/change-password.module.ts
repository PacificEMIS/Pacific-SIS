import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatToolbarModule } from '@angular/material/toolbar';
import { FlexLayoutModule } from '@angular/flex-layout';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { IconModule } from '@visurel/iconify-angular';
import { MatDialogModule } from '@angular/material/dialog';
import { MatInputModule } from '@angular/material/input';
import { TranslateModule } from '@ngx-translate/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDividerModule } from '@angular/material/divider';
import { ChangePasswordComponent } from './change-password.component';
import { MatPasswordStrengthModule } from '@angular-material-extensions/password-strength';
import { FormsModule,ReactiveFormsModule } from '@angular/forms';

@NgModule({
  declarations: [ChangePasswordComponent],
  imports: [
    CommonModule,
    MatToolbarModule,
    FlexLayoutModule,
    MatButtonModule,
    MatIconModule,
    IconModule,
    MatDialogModule,
    MatInputModule,
    TranslateModule,
    MatFormFieldModule,
    MatDividerModule,
    MatPasswordStrengthModule,
    FormsModule,
    ReactiveFormsModule
  ],
})
export class ChangePasswordModule {
}
