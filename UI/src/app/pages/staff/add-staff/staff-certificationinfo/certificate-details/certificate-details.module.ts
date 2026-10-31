import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CertificateDetailsComponent } from './certificate-details.component';
import { MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { MatButtonModule } from '@angular/material/button';
import { TranslateModule } from '@ngx-translate/core';
import { SharedModuleModule } from '../../../../shared-module/shared-module.module';



@NgModule({
  declarations: [CertificateDetailsComponent],
  imports: [
    CommonModule,
    MatIconModule,
    MatButtonModule,
    MatDialogModule,
    MatDividerModule,
    TranslateModule,
    SharedModuleModule
  ]
})
export class CertificateDetailsModule { }
