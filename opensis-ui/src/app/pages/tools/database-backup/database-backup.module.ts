import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DatabaseBackupRoutingModule } from './database-backup-routing.module';
import { DatabaseBackupComponent } from './database-backup.component';
import { SecondaryToolbarModule } from '../../../../@vex/components/secondary-toolbar/secondary-toolbar.module';
import { BreadcrumbsModule } from '../../../../@vex/components/breadcrumbs/breadcrumbs.module';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { PageLayoutModule } from '../../../../@vex/components/page-layout/page-layout.module';
import { ContainerModule } from '../../../../@vex/directives/container/container.module';
import { IconModule } from '@visurel/iconify-angular';
import { MatCardModule } from '@angular/material/card';
import { TranslateModule } from '@ngx-translate/core';


@NgModule({
  declarations: [DatabaseBackupComponent],
  imports: [
    CommonModule,
    DatabaseBackupRoutingModule,
    SecondaryToolbarModule,
    BreadcrumbsModule,
    MatIconModule,
    MatButtonModule,
    PageLayoutModule,
    ContainerModule,
    IconModule,
    MatCardModule,
    TranslateModule
  ]
})
export class DatabaseBackupModule { }
