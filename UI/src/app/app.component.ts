/***********************************************************************************
openSIS is a free student information system for public and non-public
schools from Open Solutions for Education, Inc.Website: www.os4ed.com.

Visit the openSIS product website at https://opensis.com to learn more.
If you have question regarding this software or the license, please contact
via the website.

The software is released under the terms of the GNU Affero General Public License as
published by the Free Software Foundation, version 3 of the License.
See https://www.gnu.org/licenses/agpl-3.0.en.html.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

Copyright (c) Open Solutions for Education, Inc.

All rights reserved.
***********************************************************************************/

import { Component, Inject, LOCALE_ID, NgZone, OnDestroy, OnInit, Renderer2 } from '@angular/core';
import { ConfigService } from '@vex/services/config.service';
import { Settings } from 'luxon';
import { DOCUMENT } from '@angular/common';
import { Platform } from '@angular/cdk/platform';
import { NavigationService } from '@vex/services/navigation.service';
import { LayoutService } from '@vex/services/layout.service';
import { ActivatedRoute, Router } from '@angular/router';
import { filter, map, takeUntil, tap } from 'rxjs/operators';
import { coerceBooleanProperty } from '@angular/cdk/coercion';
import { SplashScreenService } from '@vex/services/splash-screen.service';
import { Style, StyleService } from '@vex/services/style.service';
import { ConfigName } from '@vex/interfaces/config-name.model';
import { SchoolService } from './services/school.service';
import { Subject, Subscription } from 'rxjs';
import { PermissionGroupListViewModel, RolePermissionListViewModel } from './models/roll-based-access.model';
import { RollBasedAccessService } from './services/roll-based-access.service';
import { MenuModel } from './models/menu.model';
import { CryptoService } from './services/Crypto.service';
import { TranslateService } from '@ngx-translate/core';
import { CommonService } from './services/common.service';
import { UserIdleService } from 'angular-user-idle';
import { SessionExpireAlertComponent } from '@vex/layout/session-expire/session-expire-alert/session-expire-alert.component';
import { MatLegacyDialog as MatDialog } from '@angular/material/legacy-dialog';
import { DefaultValuesService } from './common/default-values.service';
import { UserViewModel } from './models/user.model';
import { SessionService } from '@vex/services/session.service';
import { LoginService } from './services/login.service';
import * as jwt_decode from 'jwt-decode';

@Component({
  selector: 'vex-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit, OnDestroy {
  title = 'vex';
  protected destroySubject$ = new Subject<void>();
  menuList: MenuModel[] = [];
  private timerStartSubscription: Subscription;
  private timeoutSubscription: Subscription;
  private pingSubscription: Subscription;
  count = 0;
  refreshTokenTimer = 0;
  minutes: number;
  tokenEndTime;
  tokenExpired: boolean;
  timer: boolean;
  private patchedLangs = new Set<string>();
  
  constructor(private configService: ConfigService,
    private styleService: StyleService,
    private renderer: Renderer2,
    private platform: Platform,
    @Inject(DOCUMENT) private document: Document,
    @Inject(LOCALE_ID) private localeId: string,
    private layoutService: LayoutService,
    private route: ActivatedRoute,
    private navigationService: NavigationService,
    private schoolService: SchoolService,
    private splashScreenService: SplashScreenService,
    private rollBasedAccessService: RollBasedAccessService,
    private cryptoService: CryptoService,
    public translateService: TranslateService,
    private defaultValuesService: DefaultValuesService,
    private commonService: CommonService,
    private userIdle: UserIdleService,
    private router: Router,
    private dialog: MatDialog,
    private defaultValueService: DefaultValuesService,
    private sessionService: SessionService,
    private loginService: LoginService,
    private zone: NgZone,
    @Inject(DOCUMENT) private _document: HTMLDocument
    ) {
      // this.getIpAdressFromExternal();

      this._document.getElementById('appFavicon').setAttribute('href',  this.defaultValueService.getPhotoAndFooter() ? 'data:image/jpeg;base64,'+ this.defaultValueService.getPhotoAndFooter().tenantFavIcon : '');

      const tenantName = this.defaultValueService.getTenantName();
      if (tenantName) {
        this._document.title = tenantName.toUpperCase();
      }


      Settings.defaultLocale = this.localeId;

    if (this.platform.BLINK) {
      this.renderer.addClass(this.document.body, 'is-blink');
    }

    /**
     * Customize the template to your needs with the ConfigService
     * Example:
     *  this.configService.updateConfig({
     *    sidenav: {
     *      title: 'Custom App',
     *      imageUrl: '//placehold.it/100x100',
     *      showCollapsePin: false
     *    },
     *    showConfigButton: false,
     *    footer: {
     *      visible: false
     *    }
     *  });
     */

    /**
     * Config Related Subscriptions
     * You can remove this if you don't need the functionality of being able to enable specific configs with queryParams
     * Example: example.com/?layout=apollo&style=default
     */
    this.route.queryParamMap.pipe(
      map(queryParamMap => queryParamMap.has('rtl') && coerceBooleanProperty(queryParamMap.get('rtl'))),
    ).subscribe(isRtl => {
      this.document.body.dir = isRtl ? 'rtl' : 'ltr';
      this.configService.updateConfig({
        rtl: isRtl
      });
    });

    this.route.queryParamMap.pipe(
      filter(queryParamMap => queryParamMap.has('layout'))
    ).subscribe(queryParamMap => this.configService.setConfig(queryParamMap.get('layout') as ConfigName));

    this.route.queryParamMap.pipe(
      filter(queryParamMap => queryParamMap.has('style'))
    ).subscribe(queryParamMap => this.styleService.setStyle(queryParamMap.get('style') as Style));

    this.schoolService.schoolListCalled.pipe(takeUntil(this.destroySubject$)).subscribe((res) => {
      if (res.dataFromUserLogin) {
        this.renderMenuFromLocalStorage();
      }else if(res.schoolChanged){
        this.callRolePermissions()
      }else if(res.schoolLoaded){
        if(this.defaultValuesService.getPermissionList()){
          this.renderMenuFromLocalStorage();
        }else{
          this.callRolePermissions();
        }
      }
    });
    // this.navigationService.items = [
    //   {
    //     type: 'link',
    //     label: 'Dashboard',
    //     route: '/school/dashboards',
    //     icon: 'layers'
    //   },
    //   {
    //     type: 'dropdown',
    //     label: 'Schools',
    //     icon: 'account_balance',
    //     children: [
    //       {
    //         type: 'link',
    //         label: 'School Information',
    //         route: '/school/schoolinfo',
    //         icon: 'info'
    //       },
    //       {
    //         type: 'link',
    //         label: 'Marking Periods',
    //         route: '/school/marking-periods',
    //         icon: 'info'
    //       },
    //       {
    //         type: 'link',
    //         label: 'Calendars',
    //         route: '/school/schoolcalendars',
    //         icon: 'info'
    //       },
    //       {
    //         type: 'link',
    //         label: 'Notices',
    //         route: '/school/notices',
    //         icon: 'info'
    //       },
    //     ]
    //   },
    //   { type: 'dropdown',
    //     label: 'Students',
    //     icon: 'school',
    //     children: [
    //       {
    //         type: 'link',
    //         label: 'Student Information',
    //         route: '/school/students',
    //         icon: 'info'
    //       },
    //       {
    //         type: 'link',
    //         label: 'Group Assign Student Info',
    //         route: '/school/students',
    //         icon: 'info'
    //       },
    //       {
    //         type: 'link',
    //         label: 'Student Re Enroll',
    //         route: '/school/students',
    //         icon: 'info'
    //       }
    //     ]
    //   },
    //   { type: 'link',
    //     label: 'Parents',
    //     icon: 'escalator_warning',
    //     route: '/school/parents'
    //   },
    //   { type: 'dropdown',
    //     label: 'Staff',
    //     icon: 'people',
    //     children: [
    //       {
    //         type: 'link',
    //         label: 'Staff Info',
    //         route: '/school/staff',
    //         icon: 'info'
    //       },
    //       {
    //         type: 'link',
    //         label: 'Teacher Functions',
    //         route: '/school/teacherfunctions',
    //         icon: 'info'
    //       }
    //     ]
    //   },
    //   { type: 'dropdown',
    //     label: 'Courses',
    //     icon: 'book',
    //     children: [
    //       {
    //         type: 'link',
    //         label: 'Course Manager',
    //         route: '/school/course-manager',
    //         icon: 'info'
    //       },
    //       {
    //         type: 'link',
    //         label: 'Course Catalog',
    //         route: '/school/coursemanager',
    //         icon: 'info'
    //       },
    //       {
    //         type: 'link',
    //         label: 'Teacher Reassignment',
    //         route: '/school/coursemanager',
    //         icon: 'info'
    //       }
    //     ]
    //   },
    //   { type: 'dropdown',
    //     label: 'Scheduling',
    //     icon: 'date_range',
    //     children: [
    //       {
    //         type: 'link',
    //         label: 'Schedule Students',
    //         route: '/school/schedule-student',
    //         icon: 'info'
    //       },
    //       {
    //         type: 'link',
    //         label: 'Schedule Teacher',
    //         route: '/school/schedule-teacher',
    //         icon: 'info'
    //       },
    //       {
    //         type: 'link',
    //         label: 'Run Scheduler',
    //         route: '/school/runscheduler',
    //         icon: 'info'
    //       },
    //       {
    //         type: 'link',
    //         label: 'Group Drop',
    //         route: '/school/group-drop',
    //         icon: 'info'
    //       },
    //     ]
    //   },
    //   { type: 'dropdown',
    //     label: 'Grades',
    //     icon: 'leaderboard',
    //     children: [
    //       {
    //         type: 'link',
    //         label: 'Progress Reports',
    //         route: '/school/progressreport',
    //         icon: 'info'
    //       }
    //     ]
    //   },
    //   { type: 'dropdown',
    //     label: 'Attendance',
    //     icon: 'access_alarm',
    //     children: [
    //       {
    //         type: 'link',
    //         label: 'Administration',
    //         route: '/school/administration',
    //         icon: 'info'
    //       }
    //     ]
    //   },
    //   { type: 'dropdown',
    //     label: 'Messaging',
    //     icon: 'mark_email_unread',
    //     children: [
    //       {
    //         type: 'link',
    //         label: 'Inbox',
    //         route: '/school/inbox',
    //         icon: 'info'
    //       }
    //     ]
    //   },
    //   { type: 'link',
    //     label: 'Reports',
    //     icon: 'list_alt',
    //     route: '/school/progressreport'
    //   },
    //   { type: 'link',
    //     label: 'Settings',
    //     icon: 'settings',
    //     route: '/school/settings'
    //   },
    //   { type: 'dropdown',
    //     label: 'Tools',
    //     icon: 'construction',
    //     children: [
    //       {
    //         type: 'link',
    //         label: 'Access Log',
    //         route: '/school/acesslog',
    //         icon: 'info'
    //       }
    //     ]
    //   },

    // this.navigationService.items = [
    //   {
    //     type: 'link',
    //     label: 'Dashboard',
    //     route: '/school/dashboards',
    //     icon: 'layers'
    //   },
    //   {
    //     type: 'dropdown',
    //     label: 'Schools',
    //     icon: 'account_balance',
    //     children: [
    //       {
    //         type: 'link',
    //         label: 'School Information',
    //         route: '/school/schoolinfo',
    //         icon: 'info'
    //       },
    //       {
    //         type: 'link',
    //         label: 'Marking Periods',
    //         route: '/school/marking-periods',
    //         icon: 'info'
    //       },
    //       {
    //         type: 'link',
    //         label: 'Calendars',
    //         route: '/school/schoolcalendars',
    //         icon: 'info'
    //       },
    //       {
    //         type: 'link',
    //         label: 'Notices',
    //         route: '/school/notices',
    //         icon: 'info'
    //       },
    //     ]
    //   },
    //   { type: 'dropdown',
    //     label: 'Students',
    //     icon: 'school',
    //     children: [
    //       {
    //         type: 'link',
    //         label: 'Student Information',
    //         route: '/school/students',
    //         icon: 'info'
    //       },
    //       {
    //         type: 'link',
    //         label: 'Group Assign Student Info',
    //         route: '/school/students',
    //         icon: 'info'
    //       },
    //       {
    //         type: 'link',
    //         label: 'Student Re Enroll',
    //         route: '/school/students',
    //         icon: 'info'
    //       },
    //       {
    //         type: 'link',
    //         label: 'Student Data Import',
    //         route: '/school/studentdataimport',
    //         icon: 'info'
    //       }
    //     ]
    //   },
    //   { type: 'link',
    //     label: 'Parents',
    //     icon: 'escalator_warning',
    //     route: '/school/parents'
    //   },
    //   { type: 'dropdown',
    //     label: 'Staff',
    //     icon: 'people',
    //     children: [
    //       {
    //         type: 'link',
    //         label: 'Staff Info',
    //         route: '/school/staff',
    //         icon: 'info'
    //       },
    //       {
    //         type: 'link',
    //         label: 'Teacher Functions',
    //         route: '/school/teacherfunctions',
    //         icon: 'info'
    //       }
    //     ]
    //   },
    //   { type: 'dropdown',
    //     label: 'Courses',
    //     icon: 'book',
    //     children: [
    //       {
    //         type: 'link',
    //         label: 'Course Manager',
    //         route: '/school/course-manager',
    //         icon: 'info'
    //       },
    //       {
    //         type: 'link',
    //         label: 'Course Catalog',
    //         route: '/school/coursemanager',
    //         icon: 'info'
    //       },
    //       {
    //         type: 'link',
    //         label: 'Teacher Reassignment',
    //         route: '/school/coursemanager',
    //         icon: 'info'
    //       }
    //     ]
    //   },
    //   { type: 'dropdown',
    //     label: 'Scheduling',
    //     icon: 'date_range',
    //     children: [
    //       {
    //         type: 'link',
    //         label: 'Schedule Students',
    //         route: '/school/schedule-student',
    //         icon: 'info'
    //       },
    //       {
    //         type: 'link',
    //         label: 'Schedule Teacher',
    //         route: '/school/schedule-teacher',
    //         icon: 'info'
    //       },
    //       {
    //         type: 'link',
    //         label: 'Run Scheduler',
    //         route: '/school/runscheduler',
    //         icon: 'info'
    //       },
    //       {
    //         type: 'link',
    //         label: 'Group Drop',
    //         route: '/school/group-drop',
    //         icon: 'info'
    //       },
    //       {
    //         type: 'link',
    //         label: 'Teacher Reassignment',
    //         route: '/school/teacher-reassignment',
    //         icon: 'info'
    //       },
    //     ]
    //   },
    //   { type: 'dropdown',
    //     label: 'Grades',
    //     icon: 'leaderboard',
    //     children: [
    //       {
    //         type: 'link',
    //         label: 'Progress Reports',
    //         route: '/school/progressreport',
    //         icon: 'info'
    //       }
    //     ]
    //   },
    //   { type: 'dropdown',
    //     label: 'Attendance',
    //     icon: 'access_alarm',
    //     children: [
    //       {
    //         type: 'link',
    //         label: 'Administration',
    //         route: '/school/administration',
    //         icon: 'info'
    //       }
    //     ]
    //   },
    //   { type: 'dropdown',
    //     label: 'Messaging',
    //     icon: 'mark_email_unread',
    //     children: [
    //       {
    //         type: 'link',
    //         label: 'Inbox',
    //         route: '/school/inbox',
    //         icon: 'info'
    //       }
    //     ]
    //   },
    //   { type: 'link',
    //     label: 'Reports',
    //     icon: 'list_alt',
    //     route: '/school/progressreport'
    //   },
    //   { type: 'link',
    //     label: 'Settings',
    //     icon: 'settings',
    //     route: '/school/settings'
    //   },
    //   { type: 'dropdown',
    //     label: 'Tools',
    //     icon: 'construction',
    //     children: [
    //       {
    //         type: 'link',
    //         label: 'Access Log',
    //         route: '/school/acesslog',
    //         icon: 'info'
    //       }
    //     ]
    //   },

    // ];
    // ];


  }

  // getIpAdressFromExternal() {
  //   this.commonService.getIpAddress().subscribe((res)=>{
  //     if(res) {
  //     }
  //   })
  // }

  renderMenuFromLocalStorage(){
    let permissions:RolePermissionListViewModel = this.defaultValuesService.getPermissionList();
          this.generateMenuBasedOnSchoolId(permissions);
  }
  callRolePermissions(){
    let rolePermissionListView: RolePermissionListViewModel = new RolePermissionListViewModel();
        this.rollBasedAccessService.getAllRolePermission(rolePermissionListView).subscribe((res: RolePermissionListViewModel) => {
          if(res._failure){
            this.commonService.checkTokenValidOrNot(res._message);
          }
          this.generateMenuBasedOnSchoolId(res);
        });
  }

  ngOnInit() {
    this.commonService.triggeredUserActivity.subscribe((res)=>{
      this.tokenDetails();
      if(this.loginService.isAuthenticated()) {
        this.onStartWatching();
      } else {
        this.onStopWatching();
      }
    })

    this.commonService.changedLanguage.subscribe((res)=>{
      this.defaultValueService.getLanguage() ? this.translateService.use(this.defaultValueService.getLanguage()) : this.translateService.use('en');
    })

    this.translateService.onLangChange.subscribe((event) => {
      this.patchTranslationsWithAppName(event.lang);
    });

    this.rollBasedAccessService.permissionsChanged.subscribe((res)=>{
      if(res){
        let permissions = this.defaultValuesService.getPermissionList();
        this.generateMenuBasedOnSchoolId(permissions)
      }
    })
  }

  private patchTranslationsWithAppName(lang: string) {
    if (this.patchedLangs.has(lang)) return;
    const appName = this.defaultValueService.getAppName();
    const translations = this.translateService.translations[lang];
    if (!translations || !appName) return;

    const patched: any = {};
    for (const key of Object.keys(translations)) {
      if (typeof translations[key] === 'string' && /opensis/i.test(translations[key])) {
        patched[key] = translations[key].replace(/openSIS|OpenSIS|opensis/gi, appName);
      }
    }

    if (Object.keys(patched).length > 0) {
      this.patchedLangs.add(lang);
      this.translateService.setTranslation(lang, patched, true);
    }
  }

  onStartWatching() {
   this.setCOnfigAndStartTimer();

    this.zone.runOutsideAngular(() => {
    this.timerStartSubscription = this.userIdle.onTimerStart()
    .pipe(tap(() => this.timer = true))
    .subscribe(count => {
      if(count) {
        this.count += 1;
        if(this.count === 1) {
          this.openDialog();
        }
      }
    } );
    });

  this.pingSubscription =  this.userIdle.ping$.subscribe(() => {
    this.tokenDetails();
    if(this.defaultValueService.getToken()) {
      this.getRefreshToken();
    } else {
      this.commonService.clearStorage();
    }
  });
  }

  setCOnfigAndStartTimer() {
    this.userIdle.setConfigValues({
      idle: 23 * 20 * 60,
      timeout: 0,
      ping: this.tokenEndTime
    });
    this.userIdle.startWatching();
  }

  tokenDetails() {
    if(this.defaultValueService.getToken()) {
    let decoded = JSON.parse(JSON.stringify(jwt_decode.default(this.defaultValueService.getToken())));

    let date1: any = new Date(decoded.exp * 1000)
    let date2: any = new Date();
    let res = Math.abs(date1 - date2) / 1000;
    this.minutes = Math.floor(res / 60);
    this.tokenEndTime = Math.max((this.minutes - 2) * 60, 60);
    this.tokenExpired = Date.now() > (decoded.exp * 1000 - 120000);
    if(this.tokenExpired) {
      this.logout();
    } else {
    }
  }
  }

  getRefreshToken() {
    const loginViewModel: UserViewModel = new UserViewModel();
  
    this.sessionService.RefreshToken(loginViewModel).subscribe(res => {
      if (res) {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          this.openDialog();
        }
        else {
          if (this.defaultValueService.getToken()) {
            // this.onStartWatching();
            this.defaultValueService.setToken(res._token).then(()=>{
              this.count = 0;
              this.tokenDetails();
              this.onStopWatching();
              this.onStartWatching();
            });
          }
        }
      }
    });
  }

  onStopWatching() {
    this.userIdle.resetTimer();
    this.userIdle.stopWatching();
    if(this.timerStartSubscription && this.pingSubscription) {
    this.timerStartSubscription.unsubscribe();
    this.pingSubscription.unsubscribe();
    }
    this.dialog.closeAll();
  }

  logout() {
    this.onStopWatching();
    this.dialog.closeAll();
    this.commonService.logoutUser();
  }

  openDialog() {
    if (this.router.url != '/' && this.defaultValueService.getToken()) {
      this.dialog.open(SessionExpireAlertComponent, {
        maxWidth: '600px',
        disableClose: true
      }).afterClosed().subscribe(token => {
        this.userIdle.resetTimer();
        this.userIdle.stopWatching();
        this.count = 0;
        if (token) {
          this.defaultValueService.setToken(token).then(()=>{
            this.tokenDetails();
            this.setCOnfigAndStartTimer();
          });
          return;
        }
        this.logout();
        this.router.navigateByUrl('/');
      });
    }
  }

  clearStorage() {
    localStorage.clear();
    let schoolId = this.defaultValueService.getSchoolID()
    sessionStorage.clear();
    if (schoolId) {
      this.defaultValueService.setSchoolID(JSON.stringify(schoolId))
    }
    this.defaultValueService.setTenantIdVal(this.defaultValueService.getDefaultTenant());
    let a = this.defaultValueService.setTenantIdVal(this.defaultValueService.getDefaultTenant());
  }

  generateMenuBasedOnSchoolId(permissions: RolePermissionListViewModel) {
    this.menuList = [];

    permissions.permissionList?.map((item) => {
      // debugger;
      // item.permissionGroup.permissionCategory = item.permissionGroup.permissionCategory.filter((item)=>item.rolePermission?.length>0);
      // item.permissionGroup.permissionCategory = item.permissionGroup.permissionCategory.map((cat)=>{
      //   cat.permissionSubcategory=cat.permissionSubcategory?.filter((subcat)=>subcat.rolePermission?.length>0);
      //   return cat;
      // })
      if (item.permissionGroup.permissionGroupName === 'Add New') {
                 
      } else {
        if (item.permissionGroup.type === 'link' && item.permissionGroup?.rolePermission[0]?.canView) {
          this.menuList.push({
            type: 'link',
            label: item.permissionGroup.title,
            icon: this.convertStringToIcon(item.permissionGroup.icon),
            route: item.permissionGroup.path
          });
        } else if (item.permissionGroup.type === 'sub') {
          const children: MenuModel[] = [];
          let membershipType = this.defaultValueService.getUserMembershipType();
          if (membershipType !== 'Teacher' && membershipType !== 'Homeroom Teacher') {
            item.permissionGroup.permissionCategory?.map((child) => {
              if (child.rolePermission[0]?.canView && child.type !== '') {
                children.push({
                  type: 'link',
                  label: child.title,
                  route: child.path
                });
              }
            })
            if (children.length > 0) {
              this.menuList.push({
                type: 'dropdown',
                label: item.permissionGroup.title,
                icon: this.convertStringToIcon(item.permissionGroup.icon),
                children
              });
            }
          } else {
            if (item.permissionGroup.title !== 'Staff') {
              item.permissionGroup.permissionCategory?.map((child) => {
                if (child.rolePermission[0]?.canView && child.type !== '') {
                  children.push({
                    type: 'link',
                    label: child.title,
                    route: child.path
                  });
                }
              })
              if (children.length > 0) {
                this.menuList.push({
                  type: 'dropdown',
                  label: item.permissionGroup.title,
                  icon: this.convertStringToIcon(item.permissionGroup.icon),
                  children
                });
              }
            }
          }
        }
      }
    });
    this.navigationService.items = this.menuList;
    this.defaultValuesService.setPermissionList(permissions);
    // this.rollBasedAccessService.setPermission();
    this.navigationService.changeMenuItemsStatus(true);

  }

  convertStringToIcon(icon: string) {
    switch (icon) {
      case 'icLayers': {
        return 'layers';
      }
      case 'icschool': {
        return 'account_balance';
      }
      case 'icstudents': {
        return 'school';
      }
      case 'icparents': {
        return 'escalator_warning';
      }
      case 'icusers': {
        return 'people';
      }
      case 'icbook': {
        return 'book';
      }
      case 'icschedule': {
        return 'date_range';
      }
      case 'icgrade': {
        return 'leaderboard';
      }
      case 'icattendance': {
        return 'access_alarm';
      }
      case 'icmessage': {
        return 'mark_email_unread';
      }
      case 'icreports': {
        return 'list_alt';
      }
      case 'icsettings': {
        return 'settings';
      }
      case 'ictools': {
        return 'construction';
      }
      default: {
        return 'info';
      }
    }
  }

  ngOnDestroy() {
    this.schoolService.changeSchoolListStatus({schoolChanged:false,schoolLoaded:false,dataFromUserLogin:false,academicYearChanged:false,academicYearLoaded:false});
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }
}
