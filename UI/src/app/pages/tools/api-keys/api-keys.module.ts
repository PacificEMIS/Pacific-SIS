import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ApiKeysRoutingModule } from './api-keys-routing.module';
import { ApiKeysComponent } from './api-keys.component';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { IconModule } from '@visurel/iconify-angular';
import { MatCardModule } from '@angular/material/card';
import { SecondaryToolbarModule } from '../../../../@vex/components/secondary-toolbar/secondary-toolbar.module';
import { BreadcrumbsModule } from '../../../../@vex/components/breadcrumbs/breadcrumbs.module';
import { PageLayoutModule } from '../../../../@vex/components/page-layout/page-layout.module';
import { ContainerModule } from '../../../../@vex/directives/container/container.module';
import { MatMenuModule } from '@angular/material/menu';
import { TranslateModule } from '@ngx-translate/core';
import { GenerateApiKeyComponent } from './generate-api-key/generate-api-key.component';
import { MatDialogModule } from '@angular/material/dialog';
import { MatInputModule } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { MatDividerModule } from '@angular/material/divider';
import { MatExpansionModule } from '@angular/material/expansion';
import { ApiModuleDatailsComponent } from './api-module-datails/api-module-datails.component'
import { RouterModule } from '@angular/router';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { KeysGenerateService } from './service/keys-generate.service';
import { SharedModuleModule } from '../../shared-module/shared-module.module';


@NgModule({
  declarations: [ApiKeysComponent, GenerateApiKeyComponent, ApiModuleDatailsComponent],
  imports: [
    CommonModule,
    ApiKeysRoutingModule,
    MatIconModule,
    MatButtonModule,
    MatFormFieldModule,
    IconModule,
    MatCardModule,
    SecondaryToolbarModule,
    BreadcrumbsModule,
    PageLayoutModule,
    ContainerModule,
    MatMenuModule,
    TranslateModule,
    MatDialogModule,
    MatInputModule,
    FormsModule,
    MatDividerModule,
    MatExpansionModule,
    RouterModule,
    MatSlideToggleModule,
    SharedModuleModule
  ],
  providers: [KeysGenerateService]
})
export class ApiKeysModule { }
