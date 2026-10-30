import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { RolloverRoutingModule } from './rollover-routing.module';
import { RolloverComponent } from './rollover.component';
import { RolloverSummaryDialogComponent } from './rollover-summary-dialog/rollover-summary-dialog.component';
import { MatLegacyDialogModule as MatDialogModule } from '@angular/material/legacy-dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatLegacyButtonModule as MatButtonModule } from '@angular/material/legacy-button';
import { MatLegacyFormFieldModule as MatFormFieldModule } from '@angular/material/legacy-form-field';
import { MatLegacyCardModule as MatCardModule } from '@angular/material/legacy-card';
import { SecondaryToolbarModule } from '@vex/components/secondary-toolbar/secondary-toolbar.module';
import { BreadcrumbsModule } from '@vex/components/breadcrumbs/breadcrumbs.module';
import { PageLayoutModule } from '@vex/components/page-layout/page-layout.module';
import { ContainerModule } from '@vex/directives/container/container.module';
import { MatLegacyMenuModule as MatMenuModule } from '@angular/material/legacy-menu';
import { TranslateModule } from '@ngx-translate/core';
import { MatLegacyInputModule as MatInputModule } from '@angular/material/legacy-input';
import { FormsModule } from '@angular/forms';
import { MatDividerModule } from '@angular/material/divider';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { SharedModuleModule } from '../../shared-module/shared-module.module';


@NgModule({
    declarations: [RolloverComponent, RolloverSummaryDialogComponent],
    imports: [
        CommonModule,
        RolloverRoutingModule,
        MatIconModule,
        MatButtonModule,
        MatFormFieldModule,
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
        MatDialogModule,
        SharedModuleModule
    ]
})
export class RolloverModule { }
