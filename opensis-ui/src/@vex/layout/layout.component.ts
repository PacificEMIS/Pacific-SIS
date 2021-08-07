import { AfterViewInit, ChangeDetectorRef, Component, HostListener, Inject, Input, OnDestroy, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { BreakpointObserver } from '@angular/cdk/layout';
import { LayoutService } from '../services/layout.service';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { MatSidenav, MatSidenavContainer } from '@angular/material/sidenav';
import { Event, NavigationEnd, Router, Scroll } from '@angular/router';
import { filter, map, startWith, withLatestFrom } from 'rxjs/operators';
import { checkRouterChildsData } from '../utils/check-router-childs-data';
import { DOCUMENT } from '@angular/common';
import { ConfigService } from '../services/config.service';
import * as jwt_decode from 'jwt-decode';
import { MatDialog } from '@angular/material/dialog';
import { SessionExpireAlertComponent } from './session-expire/session-expire-alert/session-expire-alert.component';
import { DefaultValuesService } from '../../app/common/default-values.service';
import { interval, Subscription, timer } from 'rxjs';
import { UserViewModel } from 'src/app/models/user.model';
import { SessionService } from '../services/session.service';
import { UserIdleService } from 'angular-user-idle';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CommonService } from '../../app/services/common.service';

@UntilDestroy()
@Component({
  selector: 'vex-layout',
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.scss']
})
export class LayoutComponent implements OnInit, AfterViewInit, OnDestroy {
  userActivity;

  // @HostListener('window:mousemove', ['$event'])
  // @HostListener('window:keydown', ['$event'])
  // keyPress(event): void {
  //   clearTimeout(this.userActivity);
  //   this.setTimeout();
  // }
  // setTimeout(): void {
  //   this.userActivity = setTimeout(() => {
  //     this.openDialog();
  //   }, 1000 * 60 * 20);
  // }

  minutes: number;
  count: number;
  tokenEndTime;
  tokenExpired: boolean = false;
  // searchTimer: Subscription;
  @Input() sidenavRef: TemplateRef<any>;
  @Input() toolbarRef: TemplateRef<any>;
  @Input() footerRef: TemplateRef<any>;
  @Input() quickpanelRef: TemplateRef<any>;

  isLayoutVertical$ = this.configService.config$.pipe(map(config => config.layout === 'vertical'));
  isBoxed$ = this.configService.config$.pipe(map(config => config.boxed));
  isToolbarFixed$ = this.configService.config$.pipe(map(config => config.toolbar.fixed));
  isFooterFixed$ = this.configService.config$.pipe(map(config => config.footer.fixed));
  isFooterVisible$ = this.configService.config$.pipe(map(config => config.footer.visible));
  sidenavCollapsed$ = this.layoutService.sidenavCollapsed$;
  isDesktop$ = this.layoutService.isDesktop$;

  scrollDisabled$ = this.router.events.pipe(
    filter(event => event instanceof NavigationEnd),
    startWith(null),
    map(() => checkRouterChildsData(this.router.routerState.root.snapshot, data => data.scrollDisabled))
  );

  containerEnabled$ = this.router.events.pipe(
    filter(event => event instanceof NavigationEnd),
    startWith(null),
    map(() => checkRouterChildsData(this.router.routerState.root.snapshot, data => data.containerEnabled))
  );

  searchOpen$ = this.layoutService.searchOpen$;
  @ViewChild('quickpanel', { static: true }) quickpanel: MatSidenav;
  @ViewChild('sidenav', { static: true }) sidenav: MatSidenav;
  @ViewChild(MatSidenavContainer, { static: true }) sidenavContainer: MatSidenavContainer;

  constructor(
    private cd: ChangeDetectorRef,
    private breakpointObserver: BreakpointObserver,
    private layoutService: LayoutService,
    private configService: ConfigService,
    private router: Router,
    private sessionService: SessionService,
    private snackbar: MatSnackBar,
    private commonService: CommonService,
    private defaultValueService: DefaultValuesService,
    @Inject(DOCUMENT) private document: Document,
    private dialog: MatDialog,
    private userIdle: UserIdleService
    ) { }

  ngOnInit() {
    /**
     * Expand Sidenav when we switch from mobile to desktop view
     */
    this.isDesktop$.pipe(
      filter(matches => !matches),
      untilDestroyed(this)
    ).subscribe(() => this.layoutService.expandSidenav());

    /**
     * Open/Close Quickpanel through LayoutService
     */
    this.layoutService.quickpanelOpen$.pipe(
      untilDestroyed(this)
    ).subscribe(open => open ? this.quickpanel.open() : this.quickpanel.close());

    /**
     * Open/Close Sidenav through LayoutService
     */
    this.layoutService.sidenavOpen$.pipe(
      untilDestroyed(this)
    ).subscribe(open => open ? this.sidenav.open() : this.sidenav.close());

    /**
     * Mobile only:
     * Close Sidenav after Navigating somewhere (e.g. when a user clicks a link in the Sidenav)
     */
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd),
      withLatestFrom(this.isDesktop$),
      filter(([event, matches]) => !matches),
      untilDestroyed(this)
    ).subscribe(() => this.sidenav.close());

    //this.checkToken();
    this.commonService.isOnline().subscribe((res)=>{
      if(!res){
        this.commonService.clearStorage();
        this.snackbar.open(this.defaultValueService.translateKey('networkDisconnected'), '', {
          duration: 5000
        });
        this.commonService.setUserActivity(true);
        this.router.navigateByUrl('/');
      }
    })
  }

  ngAfterViewInit(): void {
    /**
     * Enable Scrolling to specific parts of the page using the Router
     */
    this.router.events.pipe(
      filter<Event, Scroll>((e: Event): e is Scroll => e instanceof Scroll),
      untilDestroyed(this)
    ).subscribe(e => {
      if (e.position) {
        // backward navigation
        this.sidenavContainer.scrollable.scrollTo({
          start: e.position[0],
          top: e.position[1]
        });
      } else if (e.anchor) {
        // anchor navigation

        const scroll = (anchor: HTMLElement) => this.sidenavContainer.scrollable.scrollTo({
          behavior: 'smooth',
          top: anchor.offsetTop,
          left: anchor.offsetLeft
        });

        let anchorElem = this.document.getElementById(e.anchor);

        if (anchorElem) {
          scroll(anchorElem);
        } else {
          setTimeout(() => {
            anchorElem = this.document.getElementById(e.anchor);
            scroll(anchorElem);
          }, 100);
        }
      } else {
        // forward navigation
        this.sidenavContainer.scrollable.scrollTo({
          top: 0,
          start: 0
        });
      }
    });
  }

  ngOnDestroy() {
    // this.searchTimer.unsubscribe()
  }
}
