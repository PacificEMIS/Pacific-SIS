import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ViewSiblingComponent } from './view-sibling.component';
import { MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { FlexLayoutModule } from '@angular/flex-layout';
import { MatDividerModule } from '@angular/material/divider';
import { IconModule } from '@visurel/iconify-angular';
import { MatMenuModule } from '@angular/material/menu';
import { MatButtonModule } from '@angular/material/button';
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
    IconModule,
    FlexLayoutModule,
    MatDividerModule,
    MatMenuModule,
    TranslateModule,
    SharedModuleModule
  ]
})
export class ViewSiblingModule { }
