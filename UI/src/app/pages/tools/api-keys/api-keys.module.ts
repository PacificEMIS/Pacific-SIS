import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ApiKeysRoutingModule } from './api-keys-routing.module';
import { ApiKeysComponent } from './api-keys.component';
import { MatIconModule } from '@angular/material/icon';
import { MatLegacyButtonModule as MatButtonModule } from '@angular/material/legacy-button';
import { MatLegacyFormFieldModule as MatFormFieldModule } from '@angular/material/legacy-form-field';
import { MatLegacyCardModule as MatCardModule } from '@angular/material/legacy-card';
import { SecondaryToolbarModule } from '@vex/components/secondary-toolbar/secondary-toolbar.module';
import { BreadcrumbsModule } from '@vex/components/breadcrumbs/breadcrumbs.module';
import { PageLayoutModule } from '@vex/components/page-layout/page-layout.module';
import { ContainerModule } from '@vex/directives/container/container.module';
import { MatLegacyMenuModule as MatMenuModule } from '@angular/material/legacy-menu';
import { TranslateModule } from '@ngx-translate/core';
import { GenerateApiKeyComponent } from './generate-api-key/generate-api-key.component';
import { MatLegacyDialogModule as MatDialogModule } from '@angular/material/legacy-dialog';
import { MatLegacyInputModule as MatInputModule } from '@angular/material/legacy-input';
import { FormsModule } from '@angular/forms';
import { MatDividerModule } from '@angular/material/divider';
import { MatExpansionModule } from '@angular/material/expansion';
import { ApiModuleDatailsComponent } from './api-module-datails/api-module-datails.component'
import { RouterModule } from '@angular/router';
import { MatLegacySlideToggleModule as MatSlideToggleModule } from '@angular/material/legacy-slide-toggle';
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
