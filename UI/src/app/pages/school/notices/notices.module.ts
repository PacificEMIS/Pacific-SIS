import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NoticesComponent } from './notices.component';
import { MatIconModule } from '@angular/material/icon';
import { MatLegacyButtonModule as MatButtonModule } from '@angular/material/legacy-button';

import { MatLegacyCardModule as MatCardModule } from '@angular/material/legacy-card';
import { MatLegacySnackBarModule as MatSnackBarModule } from '@angular/material/legacy-snack-bar';
import { SecondaryToolbarModule } from '@vex/components/secondary-toolbar/secondary-toolbar.module';
import { BreadcrumbsModule } from '@vex/components/breadcrumbs/breadcrumbs.module';
import { PageLayoutModule } from '@vex/components/page-layout/page-layout.module';
import { ContainerModule } from '@vex/directives/container/container.module';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatLegacyMenuModule as MatMenuModule } from '@angular/material/legacy-menu';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { NoticeCardsComponent } from './notice-cards/notice-cards.component';
import { EditNoticeModule } from '../notices/edit-notice/edit-notice.module';
import { TranslateModule } from '@ngx-translate/core';
import {SharedModuleModule} from '../../shared-module/shared-module.module';
import {NoticeRoutingModule} from '../notices/notice-routing-module';

@NgModule({
  declarations: [NoticesComponent, NoticeCardsComponent],
  imports: [
    CommonModule,
    MatIconModule,
    MatButtonModule,
    MatCardModule,
    MatSidenavModule,
    MatSnackBarModule,
    SecondaryToolbarModule,
    BreadcrumbsModule,
    PageLayoutModule,
    ContainerModule,
    MatMenuModule,
    MatButtonToggleModule,
    EditNoticeModule,
    TranslateModule,
    SharedModuleModule,
    NoticeRoutingModule
  ]
})
export class NoticesModule { }
