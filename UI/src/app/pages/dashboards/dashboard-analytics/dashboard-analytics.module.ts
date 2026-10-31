import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DashboardAnalyticsRoutingModule } from './dashboard-analytics-routing.module';
import { DashboardAnalyticsComponent } from './dashboard-analytics.component';
import { MatIconModule } from '@angular/material/icon';
import { WidgetTableModule } from '@vex/components/widgets/widget-table/widget-table.module';
import { DashboardTileComponent } from 'src/app/shared/dashboard/dashboard-tile.component';
import { StatRowComponent, StatTileComponent } from 'src/app/shared/dashboard/stat-tile.component';
import { ChartComponent } from 'src/app/shared/dashboard/chart.component';
import { SecondaryToolbarModule } from '@vex/components/secondary-toolbar/secondary-toolbar.module';
import { BreadcrumbsModule } from '@vex/components/breadcrumbs/breadcrumbs.module';
import { MatButtonModule } from '@angular/material/button';
import { PageLayoutModule } from '@vex/components/page-layout/page-layout.module';
import { ContainerModule } from '@vex/directives/container/container.module';
import {SharedModuleModule} from '../../shared-module/shared-module.module';
import { CalendarModule as AngularCalendarModule, DateAdapter } from 'angular-calendar';
import { adapterFactory } from 'angular-calendar/date-adapters/date-fns';
import {MatTooltipModule} from '@angular/material/tooltip';
import { TranslateModule } from '@ngx-translate/core';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatDividerModule } from '@angular/material/divider';

@NgModule({
  declarations: [DashboardAnalyticsComponent],
  imports: [
    CommonModule,
    AngularCalendarModule.forRoot({
      provide: DateAdapter,
      useFactory: adapterFactory
    }),
    DashboardAnalyticsRoutingModule,
    MatIconModule,
    MatTooltipModule,
    WidgetTableModule,
    DashboardTileComponent,
    StatRowComponent,
    StatTileComponent,
    ChartComponent,
    SecondaryToolbarModule,
    BreadcrumbsModule,
    MatButtonModule,
    PageLayoutModule,
    ContainerModule,
    SharedModuleModule,
    MatExpansionModule,
    MatDividerModule,
    TranslateModule
  ]
})
export class DashboardAnalyticsModule {
}
