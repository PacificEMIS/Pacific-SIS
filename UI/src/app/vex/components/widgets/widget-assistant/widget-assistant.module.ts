import { NgModule } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';
import { WidgetAssistantComponent } from './widget-assistant.component';
import { FlexLayoutModule } from '@angular/flex-layout';


@NgModule({
  declarations: [WidgetAssistantComponent],
  imports: [
    MatIconModule,
    CommonModule,
    FlexLayoutModule
  ],
  exports: [WidgetAssistantComponent]
})
export class WidgetAssistantModule {
}
