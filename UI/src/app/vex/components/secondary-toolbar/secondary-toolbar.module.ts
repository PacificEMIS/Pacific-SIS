import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SecondaryToolbarComponent } from './secondary-toolbar.component';
import { RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { BreadcrumbsModule } from '../breadcrumbs/breadcrumbs.module';
import { ContainerModule } from '../../directives/container/container.module';
import {MatMenuModule} from '@angular/material/menu';
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

