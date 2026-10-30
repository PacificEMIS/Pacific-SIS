import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ViewSiblingComponent } from './view-sibling.component';
import { MatLegacyDialogModule as MatDialogModule } from '@angular/material/legacy-dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { MatLegacyMenuModule as MatMenuModule } from '@angular/material/legacy-menu';
import { MatLegacyButtonModule as MatButtonModule } from '@angular/material/legacy-button';
import { MatNativeDateModule } from '@angular/material/core';
import { TranslateModule } from '@ngx-translate/core';
import {SharedModuleModule} from '../../../../shared-module/shared-module.module';


@NgModule({
  declarations: [ViewSiblingComponent],
  imports: [
    CommonModule,
    MatIconModule,
    MatButtonModule,
    MatDialogModule,
    MatNativeDateModule,
    MatDividerModule,
    MatMenuModule,
    TranslateModule,
    SharedModuleModule
  ]
})
export class ViewSiblingModule { }
