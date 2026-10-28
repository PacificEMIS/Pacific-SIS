import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LayoutModule } from '@vex/layout/layout.module';
import { CustomLayoutComponent } from './custom-layout.component';
import { SidenavModule } from '@vex/layout/sidenav/sidenav.module';
import { ToolbarModule } from '@vex/layout/toolbar/toolbar.module';
import { FooterModule } from '@vex/layout/footer/footer.module';


@NgModule({
  declarations: [CustomLayoutComponent],
  imports: [
    CommonModule,
    LayoutModule,
    SidenavModule,
    ToolbarModule,
    FooterModule
  ]
})
export class CustomLayoutModule {
}
