import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { WidgetQuickValueCenterComponent } from './widget-quick-value-center.component';
import { FlexLayoutModule } from '@angular/flex-layout';
import { MatIconModule } from '@angular/material/icon';
import { IconModule } from '@visurel/iconify-angular';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatButtonModule } from '@angular/material/button';


@NgModule({
  declarations: [WidgetQuickValueCenterComponent],
  imports: [
    CommonModule,
    FlexLayoutModule,
    MatIconModule,
    IconModule,
    MatTooltipModule,
    MatButtonModule
  ],
  exports: [WidgetQuickValueCenterComponent]
})
export class WidgetQuickValueCenterModule {
}

