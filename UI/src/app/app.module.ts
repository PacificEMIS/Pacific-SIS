import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { VexModule } from '@vex/vex.module';
import { HttpClientModule,HTTP_INTERCEPTORS} from '@angular/common/http';
import { CustomLayoutModule } from './custom-layout/custom-layout.module';
import { ErrorIntercept } from './services/error.interceptor';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { HttpClient } from '@angular/common/http';

import {MatNativeDateModule} from '@angular/material/core';
import {MatLegacyDialogModule as MatDialogModule} from '@angular/material/legacy-dialog';
import { MatLegacyInputModule as MatInputModule } from '@angular/material/legacy-input';
import { MatLegacySelectModule as MatSelectModule } from '@angular/material/legacy-select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatLegacyCheckboxModule as MatCheckboxModule } from '@angular/material/legacy-checkbox';
import { MatLegacySnackBarModule as MatSnackBarModule } from '@angular/material/legacy-snack-bar';
import { QuillModule } from 'ngx-quill';
import { JwtHelperService, JWT_OPTIONS  } from '@auth0/angular-jwt';
import { LoaderInterceptor } from './services/loader.interceptor';
import { LoaderService } from './services/loader.service';
import { ResponseMessageService } from './services/response-message.service';
import { ConfirmDialogComponent } from './pages/shared-module/confirm-dialog/confirm-dialog.component';
import { OwlDateTimeModule, OwlNativeDateTimeModule } from '@danielmoncada/angular-datetime-picker';
import { CalendarModule as AngularCalendarModule, DateAdapter } from 'angular-calendar';
import { adapterFactory } from 'angular-calendar/date-adapters/date-fns';
import { UserIdleModule } from 'angular-user-idle';
import { NgxMaskDirective, NgxMaskPipe, provideNgxMask } from 'ngx-mask';
import {enableProdMode} from '@angular/core';
import { DatePipe } from '@angular/common';
export function createTranslateLoader(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/', '.json');
}
 enableProdMode();
@NgModule({
    declarations: [AppComponent],
    imports: [
        BrowserModule,
        NgxMaskDirective, NgxMaskPipe,
        AngularCalendarModule.forRoot({
            provide: DateAdapter,
            useFactory: adapterFactory
        }),
        AppRoutingModule,
        BrowserAnimationsModule,
        HttpClientModule,
        MatInputModule,
        MatSelectModule,
        MatDatepickerModule,
        MatNativeDateModule,
        MatCheckboxModule,
        MatSnackBarModule,
        OwlDateTimeModule,
        OwlNativeDateTimeModule,
        UserIdleModule,
        // Vex
        VexModule,
        CustomLayoutModule,
        HttpClientModule,
        QuillModule.forRoot({
            modules: {
                toolbar: [
                    ['bold', 'italic', 'underline', 'strike'],
                    ['blockquote', 'code-block'],
                    [{ header: 1 }, { header: 2 }],
                    [{ list: 'ordered' }, { list: 'bullet' }],
                    [{ script: 'sub' }, { script: 'super' }],
                    [{ indent: '-1' }, { indent: '+1' }],
                    [{ direction: 'rtl' }],
                    [{ size: ['small', false, 'large', 'huge'] }],
                    [{ header: [1, 2, 3, 4, 5, 6, false] }],
                    [{ color: [] }, { background: [] }],
                    [{ align: [] }],
                    ['clean'],
                    ['link', 'image', 'video'] // link and image, video
                ]
            }
        }),
        TranslateModule.forRoot({
            loader: {
                provide: TranslateLoader,
                useFactory: (createTranslateLoader),
                deps: [HttpClient]
            }
        })
    ],
    providers: [
        provideNgxMask(),
        {
            provide: HTTP_INTERCEPTORS,
            useClass: ErrorIntercept,
            multi: true
        },
        { provide: JWT_OPTIONS, useValue: JWT_OPTIONS },
        { provide: HTTP_INTERCEPTORS, useClass: LoaderInterceptor, multi: true },
        LoaderService,
        JwtHelperService,
        ResponseMessageService, DatePipe
    ],
    bootstrap: [AppComponent],
    exports: []
})
export class AppModule { }