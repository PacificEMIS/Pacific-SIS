import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TeacherViewScheduleRoutingModule } from './teacher-view-schedule-routing.module';
import { TeacherViewScheduleComponent } from './teacher-view-schedule.component';
import { MatIconModule } from '@angular/material/icon';
import { MatLegacyButtonModule as MatButtonModule } from '@angular/material/legacy-button';
import { DateAdapter } from '@angular/material/core';
import { MatLegacyCardModule as MatCardModule } from '@angular/material/legacy-card';
import { SecondaryToolbarModule } from '@vex/components/secondary-toolbar/secondary-toolbar.module';
import { BreadcrumbsModule } from '@vex/components/breadcrumbs/breadcrumbs.module';
import { PageLayoutModule } from '@vex/components/page-layout/page-layout.module';
import { ContainerModule } from '@vex/directives/container/container.module';
import { TranslateModule } from '@ngx-translate/core';
import { SharedModuleModule } from '../../shared-module/shared-module.module';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatLegacyMenuModule as MatMenuModule } from '@angular/material/legacy-menu';
import { MatLegacyTableModule as MatTableModule } from '@angular/material/legacy-table';
import { MatLegacyTooltipModule as MatTooltipModule } from '@angular/material/legacy-tooltip';
import { CalendarModule as AngularCalendarModule } from 'angular-calendar';
import { adapterFactory } from 'angular-calendar/date-adapters/date-fns';
import { MatDividerModule } from '@angular/material/divider';
import { FormsModule } from '@angular/forms';


@NgModule({
  declarations: [TeacherViewScheduleComponent],
  imports: [
    CommonModule,
    AngularCalendarModule.forRoot({
      provide: DateAdapter,
      useFactory: adapterFactory
    }),
    TeacherViewScheduleRoutingModule,
    MatIconModule,
    MatButtonModule,
    MatCardModule,
    SecondaryToolbarModule,
    BreadcrumbsModule,
    PageLayoutModule,
    ContainerModule,
    TranslateModule,
    SharedModuleModule,
    MatExpansionModule,
    MatMenuModule,
    MatTableModule,
    MatTooltipModule,
    MatDividerModule,
    FormsModule
  ]
})
export class TeacherViewScheduleModule { }
