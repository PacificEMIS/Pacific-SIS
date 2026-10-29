import { NgModule } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';
import { WidgetAssistantComponent } from './widget-assistant.component';


@NgModule({
  declarations: [WidgetAssistantComponent],
  imports: [
    MatIconModule,
    CommonModule
  ],
  exports: [WidgetAssistantComponent]
})
export class WidgetAssistantModule {
}
