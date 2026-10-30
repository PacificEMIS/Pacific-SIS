import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SecondaryToolbarComponent } from './secondary-toolbar.component';
import { RouterModule } from '@angular/router';
import { MatLegacyButtonModule as MatButtonModule } from '@angular/material/legacy-button';
import { MatIconModule } from '@angular/material/icon';
import { BreadcrumbsModule } from '../breadcrumbs/breadcrumbs.module';
import { ContainerModule } from '../../directives/container/container.module';
import {MatLegacyMenuModule as MatMenuModule} from '@angular/material/legacy-menu';
import { TranslateModule } from '@ngx-translate/core';

@NgModule({
  declarations: [SecondaryToolbarComponent],
  imports: [
    CommonModule,
    RouterModule,
    MatButtonModule,
    MatIconModule,
    BreadcrumbsModule,
    ContainerModule,
    MatMenuModule,
    TranslateModule
  ],
  exports: [SecondaryToolbarComponent]
})
export class SecondaryToolbarModule {
}

