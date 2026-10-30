import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PeriodAttendanceComponent } from './period-attendance.component';
import { RouterModule } from '@angular/router';
import { SecondaryToolbarModule } from '@vex/components/secondary-toolbar/secondary-toolbar.module';
import { BreadcrumbsModule } from '@vex/components/breadcrumbs/breadcrumbs.module';
import { MatIconModule } from '@angular/material/icon';
import { MatLegacyButtonModule as MatButtonModule } from '@angular/material/legacy-button';
import { PageLayoutModule } from '@vex/components/page-layout/page-layout.module';
import { ContainerModule } from '@vex/directives/container/container.module';
import { TranslateModule } from '@ngx-translate/core';
import { MatDividerModule } from '@angular/material/divider';
import { MatLegacyInputModule as MatInputModule } from '@angular/material/legacy-input';
import {MatLegacyCheckboxModule as MatCheckboxModule} from '@angular/material/legacy-checkbox';
import { MatLegacyTableModule as MatTableModule } from '@angular/material/legacy-table';
import { MatLegacyTooltipModule as MatTooltipModule } from '@angular/material/legacy-tooltip';
import { MatLegacySlideToggleModule as MatSlideToggleModule } from '@angular/material/legacy-slide-toggle';
import { FormsModule,ReactiveFormsModule } from '@angular/forms';
import { SharedModuleModule } from '../../../../shared-module/shared-module.module'
@NgModule({
  declarations: [PeriodAttendanceComponent],
  imports: [
    CommonModule,
    RouterModule,
    SecondaryToolbarModule,
    BreadcrumbsModule,
    MatIconModule,
    MatButtonModule,
    PageLayoutModule,
    ContainerModule,
    TranslateModule,
    MatDividerModule,
    MatInputModule,
    RouterModule,
    MatCheckboxModule,
    MatTableModule,
    MatTooltipModule,
    MatSlideToggleModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModuleModule
  ]
})
export class PeriodAttendanceModule { }
