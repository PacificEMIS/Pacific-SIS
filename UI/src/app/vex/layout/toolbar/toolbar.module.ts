import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToolbarComponent } from './toolbar.component';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatRippleModule } from '@angular/material/core';
import { ToolbarNotificationsModule } from './toolbar-notifications/toolbar-notifications.module';
import { RouterModule } from '@angular/router';
import { MegaMenuModule } from '../../components/mega-menu/mega-menu.module';
import { ContainerModule } from '../../directives/container/container.module';
import { SelectBarModule } from './select-bar/select-bar.module';
import { TranslateModule } from '@ngx-translate/core';
import { MatTooltipModule } from '@angular/material/tooltip';

@NgModule({
  declarations: [ToolbarComponent],
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatRippleModule,
    ToolbarNotificationsModule,
    RouterModule,
    MegaMenuModule,
    ContainerModule,
    SelectBarModule,
    TranslateModule,
    MatTooltipModule
      ],
  exports: [ToolbarComponent]
})
export class ToolbarModule {
}
