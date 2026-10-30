import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DatabaseBackupRoutingModule } from './database-backup-routing.module';
import { DatabaseBackupComponent } from './database-backup.component';
import { SecondaryToolbarModule } from '@vex/components/secondary-toolbar/secondary-toolbar.module';
import { BreadcrumbsModule } from '@vex/components/breadcrumbs/breadcrumbs.module';
import { MatIconModule } from '@angular/material/icon';
import { MatLegacyButtonModule as MatButtonModule } from '@angular/material/legacy-button';
import { PageLayoutModule } from '@vex/components/page-layout/page-layout.module';
import { ContainerModule } from '@vex/directives/container/container.module';
import { MatLegacyCardModule as MatCardModule } from '@angular/material/legacy-card';
import { TranslateModule } from '@ngx-translate/core';
import { MatDividerModule } from '@angular/material/divider';
import { SharedModuleModule } from '../../shared-module/shared-module.module';


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
    MatCardModule,
    TranslateModule,
    MatDividerModule,
    SharedModuleModule
  ]
})
export class DatabaseBackupModule { }
