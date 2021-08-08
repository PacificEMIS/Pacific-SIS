import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NoticesComponent } from './notices.component';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';

import { IconModule } from '@visurel/iconify-angular';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { SecondaryToolbarModule } from '../../../../@vex/components/secondary-toolbar/secondary-toolbar.module';
import { BreadcrumbsModule } from '../../../../@vex/components/breadcrumbs/breadcrumbs.module';
import { PageLayoutModule } from '../../../../@vex/components/page-layout/page-layout.module';
import { ContainerModule } from '../../../../@vex/directives/container/container.module';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatMenuModule } from '@angular/material/menu';
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
    IconModule,
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
