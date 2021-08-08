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
import { ConfigService } from '../@vex/services/config.service';
import { Settings } from 'luxon';
import { DOCUMENT } from '@angular/common';
import { Platform } from '@angular/cdk/platform';
import { NavigationService } from '../@vex/services/navigation.service';
import icLayers from '@iconify/icons-ic/twotone-layers';
import icschool from '@iconify/icons-ic/baseline-account-balance';
import icinfo from '@iconify/icons-ic/info';
import icsettings from '@iconify/icons-ic/settings';
import icstudents from '@iconify/icons-ic/school';
import icusers from '@iconify/icons-ic/people';
import icschedule from '@iconify/icons-ic/date-range';
import icgrade from '@iconify/icons-ic/baseline-leaderboard';
import icattendance from '@iconify/icons-ic/baseline-access-alarm';
import icmessage from '@iconify/icons-ic/baseline-mark-email-unread';
import icactivity from '@iconify/icons-ic/accessibility';
import icreports from '@iconify/icons-ic/baseline-list-alt';
import ictools from '@iconify/icons-ic/baseline-construction';
import icparents from '@iconify/icons-ic/baseline-escalator-warning';
import icbook from '@iconify/icons-ic/baseline-book';
import { LayoutService } from '../@vex/services/layout.service';
import { ActivatedRoute, Router } from '@angular/router';
import { filter, map, takeUntil, tap } from 'rxjs/operators';
import { coerceBooleanProperty } from '@angular/cdk/coercion';
import { SplashScreenService } from '../@vex/services/splash-screen.service';
import { Style, StyleService } from '../@vex/services/style.service';
import { ConfigName } from '../@vex/interfaces/config-name.model';
import { SchoolService } from './services/school.service';
import { Subject, Subscription } from 'rxjs';
import { PermissionGroupListViewModel, RolePermissionListViewModel } from './models/roll-based-access.model';
import { RollBasedAccessService } from './services/roll-based-access.service';
import { MenuModel } from './models/menu.model';
import { CryptoService } from './services/Crypto.service';
import { TranslateService } from '@ngx-translate/core';
import { CommonService } from './services/common.service';
import { UserIdleService } from 'angular-user-idle';
import { SessionExpireAlertComponent } from 'src/@vex/layout/session-expire/session-expire-alert/session-expire-alert.component';
import { MatDialog } from '@angular/material/dialog';
import { DefaultValuesService } from './common/default-values.service';
import { UserViewModel } from './models/user.model';
import { SessionService } from 'src/@vex/services/session.service';
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

    ) {

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
    //     icon: icLayers
    //   },
    //   {
    //     type: 'dropdown',
    //     label: 'Schools',
    //     icon: icschool,
    //     children: [
    //       {
    //         type: 'link',
    //         label: 'School Information',
    //         route: '/school/schoolinfo',
    //         icon: icinfo
    //       },
    //       {
    //         type: 'link',
    //         label: 'Marking Periods',
    //         route: '/school/marking-periods',
    //         icon: icinfo
    //       },
    //       {
    //         type: 'link',
    //         label: 'Calendars',
    //         route: '/school/schoolcalendars',
    //         icon: icinfo
    //       },
    //       {
    //         type: 'link',
    //         label: 'Notices',
    //         route: '/school/notices',
    //         icon: icinfo
    //       },
    //     ]
    //   },
    //   { type: 'dropdown',
    //     label: 'Students',
    //     icon: icstudents,
    //     children: [
    //       {
    //         type: 'link',
    //         label: 'Student Information',
    //         route: '/school/students',
    //         icon: icinfo
    //       },
    //       {
    //         type: 'link',
    //         label: 'Group Assign Student Info',
    //         route: '/school/students',
    //         icon: icinfo
    //       },
    //       {
    //         type: 'link',
    //         label: 'Student Re Enroll',
    //         route: '/school/students',
    //         icon: icinfo
    //       }
    //     ]
    //   },
    //   { type: 'link',
    //     label: 'Parents',
    //     icon: icparents,
    //     route: '/school/parents'
    //   },
    //   { type: 'dropdown',
    //     label: 'Staff',
    //     icon: icusers,
    //     children: [
    //       {
    //         type: 'link',
    //         label: 'Staff Info',
    //         route: '/school/staff',
    //         icon: icinfo
    //       },
    //       {
    //         type: 'link',
    //         label: 'Teacher Functions',
    //         route: '/school/teacherfunctions',
    //         icon: icinfo
    //       }
    //     ]
    //   },
    //   { type: 'dropdown',
    //     label: 'Courses',
    //     icon: icbook,
    //     children: [
    //       {
    //         type: 'link',
    //         label: 'Course Manager',
    //         route: '/school/course-manager',
    //         icon: icinfo
    //       },
    //       {
    //         type: 'link',
    //         label: 'Course Catalog',
    //         route: '/school/coursemanager',
    //         icon: icinfo
    //       },
    //       {
    //         type: 'link',
    //         label: 'Teacher Reassignment',
    //         route: '/school/coursemanager',
    //         icon: icinfo
    //       }
    //     ]
    //   },
    //   { type: 'dropdown',
    //     label: 'Scheduling',
    //     icon: icschedule,
    //     children: [
    //       {
    //         type: 'link',
    //         label: 'Schedule Students',
    //         route: '/school/schedule-student',
    //         icon: icinfo
    //       },
    //       {
    //         type: 'link',
    //         label: 'Schedule Teacher',
    //         route: '/school/schedule-teacher',
    //         icon: icinfo
    //       },
    //       {
    //         type: 'link',
    //         label: 'Run Scheduler',
    //         route: '/school/runscheduler',
    //         icon: icinfo
    //       },
    //       {
    //         type: 'link',
    //         label: 'Group Drop',
    //         route: '/school/group-drop',
    //         icon: icinfo
    //       },
    //     ]
    //   },
    //   { type: 'dropdown',
    //     label: 'Grades',
    //     icon: icgrade,
    //     children: [
    //       {
    //         type: 'link',
    //         label: 'Progress Reports',
    //         route: '/school/progressreport',
    //         icon: icinfo
    //       }
    //     ]
    //   },
    //   { type: 'dropdown',
    //     label: 'Attendance',
    //     icon: icattendance,
    //     children: [
    //       {
    //         type: 'link',
    //         label: 'Administration',
    //         route: '/school/administration',
    //         icon: icinfo
    //       }
    //     ]
    //   },
    //   { type: 'dropdown',
    //     label: 'Messaging',
    //     icon: icmessage,
    //     children: [
    //       {
    //         type: 'link',
    //         label: 'Inbox',
    //         route: '/school/inbox',
    //         icon: icinfo
    //       }
    //     ]
    //   },
    //   { type: 'link',
    //     label: 'Reports',
    //     icon: icreports,
    //     route: '/school/progressreport'
    //   },
    //   { type: 'link',
    //     label: 'Settings',
    //     icon: icsettings,
    //     route: '/school/settings'
    //   },
    //   { type: 'dropdown',
    //     label: 'Tools',
    //     icon: ictools,
    //     children: [
    //       {
    //         type: 'link',
    //         label: 'Access Log',
    //         route: '/school/acesslog',
    //         icon: icinfo
    //       }
    //     ]
    //   },

    // this.navigationService.items = [
    //   {
    //     type: 'link',
    //     label: 'Dashboard',
    //     route: '/school/dashboards',
    //     icon: icLayers
    //   },
    //   {
    //     type: 'dropdown',
    //     label: 'Schools',
    //     icon: icschool,
    //     children: [
    //       {
    //         type: 'link',
    //         label: 'School Information',
    //         route: '/school/schoolinfo',
    //         icon: icinfo
    //       },
    //       {
    //         type: 'link',
    //         label: 'Marking Periods',
    //         route: '/school/marking-periods',
    //         icon: icinfo
    //       },
    //       {
    //         type: 'link',
    //         label: 'Calendars',
    //         route: '/school/schoolcalendars',
    //         icon: icinfo
    //       },
    //       {
    //         type: 'link',
    //         label: 'Notices',
    //         route: '/school/notices',
    //         icon: icinfo
    //       },
    //     ]
    //   },
    //   { type: 'dropdown',
    //     label: 'Students',
    //     icon: icstudents,
    //     children: [
    //       {
    //         type: 'link',
    //         label: 'Student Information',
    //         route: '/school/students',
    //         icon: icinfo
    //       },
    //       {
    //         type: 'link',
    //         label: 'Group Assign Student Info',
    //         route: '/school/students',
    //         icon: icinfo
    //       },
    //       {
    //         type: 'link',
    //         label: 'Student Re Enroll',
    //         route: '/school/students',
    //         icon: icinfo
    //       },
    //       {
    //         type: 'link',
    //         label: 'Student Data Import',
    //         route: '/school/studentdataimport',
    //         icon: icinfo
    //       }
    //     ]
    //   },
    //   { type: 'link',
    //     label: 'Parents',
    //     icon: icparents,
    //     route: '/school/parents'
    //   },
    //   { type: 'dropdown',
    //     label: 'Staff',
    //     icon: icusers,
    //     children: [
    //       {
    //         type: 'link',
    //         label: 'Staff Info',
    //         route: '/school/staff',
    //         icon: icinfo
    //       },
    //       {
    //         type: 'link',
    //         label: 'Teacher Functions',
    //         route: '/school/teacherfunctions',
    //         icon: icinfo
    //       }
    //     ]
    //   },
    //   { type: 'dropdown',
    //     label: 'Courses',
    //     icon: icbook,
    //     children: [
    //       {
    //         type: 'link',
    //         label: 'Course Manager',
    //         route: '/school/course-manager',
    //         icon: icinfo
    //       },
    //       {
    //         type: 'link',
    //         label: 'Course Catalog',
    //         route: '/school/coursemanager',
    //         icon: icinfo
    //       },
    //       {
    //         type: 'link',
    //         label: 'Teacher Reassignment',
    //         route: '/school/coursemanager',
    //         icon: icinfo
    //       }
    //     ]
    //   },
    //   { type: 'dropdown',
    //     label: 'Scheduling',
    //     icon: icschedule,
    //     children: [
    //       {
    //         type: 'link',
    //         label: 'Schedule Students',
    //         route: '/school/schedule-student',
    //         icon: icinfo
    //       },
    //       {
    //         type: 'link',
    //         label: 'Schedule Teacher',
    //         route: '/school/schedule-teacher',
    //         icon: icinfo
    //       },
    //       {
    //         type: 'link',
    //         label: 'Run Scheduler',
    //         route: '/school/runscheduler',
    //         icon: icinfo
    //       },
    //       {
    //         type: 'link',
    //         label: 'Group Drop',
    //         route: '/school/group-drop',
    //         icon: icinfo
    //       },
    //       {
    //         type: 'link',
    //         label: 'Teacher Reassignment',
    //         route: '/school/teacher-reassignment',
    //         icon: icinfo
    //       },
    //     ]
    //   },
    //   { type: 'dropdown',
    //     label: 'Grades',
    //     icon: icgrade,
    //     children: [
    //       {
    //         type: 'link',
    //         label: 'Progress Reports',
    //         route: '/school/progressreport',
    //         icon: icinfo
    //       }
    //     ]
    //   },
    //   { type: 'dropdown',
    //     label: 'Attendance',
    //     icon: icattendance,
    //     children: [
    //       {
    //         type: 'link',
    //         label: 'Administration',
    //         route: '/school/administration',
    //         icon: icinfo
    //       }
    //     ]
    //   },
    //   { type: 'dropdown',
    //     label: 'Messaging',
    //     icon: icmessage,
    //     children: [
    //       {
    //         type: 'link',
    //         label: 'Inbox',
    //         route: '/school/inbox',
    //         icon: icinfo
    //       }
    //     ]
    //   },
    //   { type: 'link',
    //     label: 'Reports',
    //     icon: icreports,
    //     route: '/school/progressreport'
    //   },
    //   { type: 'link',
    //     label: 'Settings',
    //     icon: icsettings,
    //     route: '/school/settings'
    //   },
    //   { type: 'dropdown',
    //     label: 'Tools',
    //     icon: ictools,
    //     children: [
    //       {
    //         type: 'link',
    //         label: 'Access Log',
    //         route: '/school/acesslog',
    //         icon: icinfo
    //       }
    //     ]
    //   },

    // ];
    // ];


  }

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
      sessionStorage.getItem("language") ? this.translateService.use(sessionStorage.getItem("language")) : this.translateService.use('en');
    })

    this.rollBasedAccessService.permissionsChanged.subscribe((res)=>{
      if(res){
        let permissions = this.defaultValuesService.getPermissionList();
        this.generateMenuBasedOnSchoolId(permissions)
      }
    })
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
      idle: 20 * 60,
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
    this.minutes = Math.floor(res / 60) % 60;
    this.tokenEndTime = (this.minutes - 2) * 60;
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
    sessionStorage.setItem('tenant', this.defaultValueService.getDefaultTenant());
    let a = sessionStorage.setItem('tenant', this.defaultValueService.getDefaultTenant());
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
          let membershipName = sessionStorage.getItem('membershipName');
          if (membershipName !== 'Teacher') {
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
        return icLayers;
      }
      case 'icschool': {
        return icschool;
      }
      case 'icstudents': {
        return icstudents;
      }
      case 'icparents': {
        return icparents;
      }
      case 'icusers': {
        return icusers;
      }
      case 'icbook': {
        return icbook;
      }
      case 'icschedule': {
        return icschedule;
      }
      case 'icgrade': {
        return icgrade;
      }
      case 'icattendance': {
        return icattendance;
      }
      case 'icmessage': {
        return icmessage;
      }
      case 'icreports': {
        return icreports;
      }
      case 'icsettings': {
        return icsettings;
      }
      case 'ictools': {
        return ictools;
      }
      default: {
        return icinfo;
      }
    }
  }

  ngOnDestroy() {
    this.schoolService.changeSchoolListStatus({schoolChanged:false,schoolLoaded:false,dataFromUserLogin:false,academicYearChanged:false,academicYearLoaded:false});
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }
}
