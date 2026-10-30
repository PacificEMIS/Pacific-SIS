import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CalendarComponent } from './calendar.component';
import { CalendarModule as AngularCalendarModule, DateAdapter } from 'angular-calendar';
import { adapterFactory } from 'angular-calendar/date-adapters/date-fns';
import { CalendarEditComponent } from './calendar-edit/calendar-edit.component';
import { MatLegacyDialogModule as MatDialogModule } from '@angular/material/legacy-dialog';
import { MatLegacyButtonModule as MatButtonModule } from '@angular/material/legacy-button';
import { MatIconModule } from '@angular/material/icon';
import { MatLegacyCardModule as MatCardModule } from '@angular/material/legacy-card';
import { ScrollbarModule } from '@vex/components/scrollbar/scrollbar.module';
import { MatLegacySelectModule as MatSelectModule } from '@angular/material/legacy-select';
import { MatLegacySnackBarModule as MatSnackBarModule } from '@angular/material/legacy-snack-bar';
import { MatLegacyInputModule as MatInputModule } from '@angular/material/legacy-input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { ReactiveFormsModule } from '@angular/forms';
import { SecondaryToolbarModule } from '@vex/components/secondary-toolbar/secondary-toolbar.module';
import { BreadcrumbsModule } from '@vex/components/breadcrumbs/breadcrumbs.module';
import { PageLayoutModule } from '@vex/components/page-layout/page-layout.module';
import { MatNativeDateModule } from '@angular/material/core';
import { ContainerModule } from '@vex/directives/container/container.module';
import { MatLegacyMenuModule as MatMenuModule } from '@angular/material/legacy-menu';
import { TranslateModule } from '@ngx-translate/core';
import { CalendarRoutingModule } from './calendar-routing-module';
import {SharedModuleModule} from '../../shared-module/shared-module.module';
import { MatLegacyTableModule as MatTableModule } from '@angular/material/legacy-table';

@NgModule({
    declarations: [CalendarComponent, CalendarEditComponent],
    imports: [
        CommonModule,
        AngularCalendarModule.forRoot({
            provide: DateAdapter,
            useFactory: adapterFactory
        }),
        MatDialogModule,
        MatButtonModule,
        MatIconModule,
        MatCardModule,
        SecondaryToolbarModule,
        BreadcrumbsModule,
        ScrollbarModule,
        MatSnackBarModule,
        MatInputModule,
        MatSelectModule,
        MatDatepickerModule,
        ReactiveFormsModule,
        PageLayoutModule,
        MatNativeDateModule,
        ContainerModule,
        MatMenuModule,
        TranslateModule,
        CalendarRoutingModule,
        SharedModuleModule,
        MatTableModule
    ]
})
export class CalendarModule {
}
