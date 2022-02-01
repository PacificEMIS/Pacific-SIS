import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { RolloverRoutingModule } from './rollover-routing.module';
import { RolloverComponent } from './rollover.component';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { IconModule } from '@visurel/iconify-angular';
import { MatCardModule } from '@angular/material/card';
import { SecondaryToolbarModule } from '../../../../@vex/components/secondary-toolbar/secondary-toolbar.module';
import { BreadcrumbsModule } from '../../../../@vex/components/breadcrumbs/breadcrumbs.module';
import { PageLayoutModule } from '../../../../@vex/components/page-layout/page-layout.module';
import { ContainerModule } from '../../../../@vex/directives/container/container.module';
import { MatMenuModule } from '@angular/material/menu';
import { TranslateModule } from '@ngx-translate/core';
import { MatInputModule } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { MatDividerModule } from '@angular/material/divider';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { SharedModuleModule } from '../../shared-module/shared-module.module';


@NgModule({
  declarations: [RolloverComponent],
  imports: [
    CommonModule,
    RolloverRoutingModule,
    MatIconModule,
    MatButtonModule,
    MatFormFieldModule,
    IconModule,
    MatCardModule,
    SecondaryToolbarModule,
    BreadcrumbsModule,
    PageLayoutModule,
    ContainerModule,
    MatMenuModule,
    TranslateModule,
    MatInputModule,
    FormsModule,
    MatDividerModule,
    MatDatepickerModule,
    SharedModuleModule
  ]
})
export class RolloverModule { }
