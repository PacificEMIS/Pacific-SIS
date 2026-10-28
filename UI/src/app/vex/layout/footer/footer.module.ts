import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { FooterComponent } from './footer.component';
import { FlexLayoutModule } from '@angular/flex-layout';
import { MatButtonModule } from '@angular/material/button';
import { ContainerModule } from '../../directives/container/container.module';

@NgModule({
  imports: [
    MatIconModule,
    CommonModule,
    FlexLayoutModule,
    MatButtonModule,
    ContainerModule
  ],
  declarations: [FooterComponent],
  exports: [FooterComponent]
})
export class FooterModule {
}
