import { Component, ElementRef, HostBinding, Input, OnDestroy, OnInit } from '@angular/core';
import { LayoutService } from '../../services/layout.service';
import { ConfigService } from '../../services/config.service';
import { map, takeUntil } from 'rxjs/operators';
import icBookmarks from '@iconify/icons-ic/twotone-bookmarks';
import emojioneUS from '@iconify/icons-emojione/flag-for-flag-united-states';
import emojioneDE from '@iconify/icons-emojione/flag-for-flag-germany';
import icMenu from '@iconify/icons-ic/twotone-menu';
import icPersonAdd from '@iconify/icons-ic/twotone-person-add';
import icAssignmentTurnedIn from '@iconify/icons-ic/twotone-assignment-turned-in';
import icBallot from '@iconify/icons-ic/twotone-ballot';
import icDescription from '@iconify/icons-ic/twotone-description';
import icAssignment from '@iconify/icons-ic/twotone-assignment';
import icReceipt from '@iconify/icons-ic/twotone-receipt';
import icDoneAll from '@iconify/icons-ic/twotone-done-all';
import icArrowDropDown from '@iconify/icons-ic/twotone-arrow-drop-down';
import icSearch from '@iconify/icons-ic/twotone-search';
import icAdd from '@iconify/icons-ic/twotone-add';
import { NavigationService } from '../../services/navigation.service';
import { PopoverService } from '../../components/popover/popover.service';
import { MegaMenuComponent } from '../../components/mega-menu/mega-menu.component';
import { fadeInUp400ms } from '../../../@vex/animations/fade-in-up.animation';
import { stagger40ms } from '../../../@vex/animations/stagger.animation';
import { RolePermissionListViewModel } from 'src/app/models/roll-based-access.model';
import { CryptoService } from 'src/app/services/Crypto.service';
import { Subject } from 'rxjs';
import { DefaultValuesService } from '../../../app/common/default-values.service';
import { TranslateService } from '@ngx-translate/core';
import { ImpersonateServices } from 'src/app/services/impersonate.service';

@Component({
  selector: 'vex-toolbar',
  templateUrl: './toolbar.component.html',
  styleUrls: ['./toolbar.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ],
})
export class ToolbarComponent implements OnInit,OnDestroy {

  @Input() mobileQuery: boolean;

  @Input()
  @HostBinding('class.shadow-b')
  hasShadow: boolean;
  tenantLogoIcon: any;
  tenantName: any;
  profileType;
  navigationItems = this.navigationService.items;

  isHorizontalLayout$ = this.configService.config$.pipe(map(config => config.layout === 'horizontal'));
  isVerticalLayout$ = this.configService.config$.pipe(map(config => config.layout === 'vertical'));
  isNavbarInToolbar$ = this.configService.config$.pipe(map(config => config.navbar.position === 'in-toolbar'));
  isNavbarBelowToolbar$ = this.configService.config$.pipe(map(config => config.navbar.position === 'below-toolbar'));

  icSearch = icSearch;
  icBookmarks = icBookmarks;
  emojioneUS = emojioneUS;
  emojioneDE = emojioneDE;
  icMenu = icMenu;
  icPersonAdd = icPersonAdd;
  icAssignmentTurnedIn = icAssignmentTurnedIn;
  icBallot = icBallot;
  icDescription = icDescription;
  icAssignment = icAssignment;
  icReceipt = icReceipt;
  icDoneAll = icDoneAll;
  icArrowDropDown = icArrowDropDown;
  icAdd = icAdd;
  // addNewMenu=[];
  // private destroySubject$ = new Subject<void>();
  impersonateButton:boolean;
  impersonateSubjectForToolBar:boolean=false;
  constructor(private layoutService: LayoutService,
    private configService: ConfigService,
    private navigationService: NavigationService,
    private popoverService: PopoverService,
    public defaultValueService: DefaultValuesService,
    private cryptoService:CryptoService,
    public translateService: TranslateService,
    private impersonateServices:ImpersonateServices
    ) {
    this.tenantLogoIcon = this.defaultValueService.getPhotoAndFooter().tenantLogoIcon;
    this.tenantName = this.defaultValueService.getTenantName();
    //  this.navigationService.menuItems.pipe(takeUntil(this.destroySubject$)).subscribe((res)=>{
    //    if(res){
    //     this.renderAddNew();
    //    }
    //  })
    this.profileType= this.defaultValueService.getUserMembershipType();
    this.impersonateServices.impersonateSubjectForToolBar.subscribe(x=>{
      if(x) {
        this.impersonateSubjectForToolBar=true;
        this.impersonateToolBar();
      }
    })
  }

  ngOnInit() {
    this.impersonateToolBar();
  }

  impersonateToolBar() {
    this.impersonateButton = this.impersonateServices.getImpersonateButton();
    // this.renderAddNew();
    if (this.impersonateSubjectForToolBar) {
      this.impersonateServices.impersonateSubjectForToolBar.next(false)
      this.impersonateSubjectForToolBar = false;
    }
  }

  // renderAddNew(){
  // let permissions = JSON.parse(this.cryptoService.dataDecrypt(localStorage.getItem('permissions')));
  // if(permissions){
  //   this.addNewMenu = permissions?.permissionList?.filter((item) => {
  //     return item.permissionGroup.permissionGroupId === 2;
  //   });
  // }
 
  // }

  openQuickpanel() {
    this.layoutService.openQuickpanel();
  }

  openSidenav() {
    this.layoutService.openSidenav();
  }

  openMegaMenu(origin: ElementRef | HTMLElement) {
    this.popoverService.open({
      content: MegaMenuComponent,
      origin,
      position: [
        {
          originX: 'start',
          originY: 'bottom',
          overlayX: 'start',
          overlayY: 'top'
        },
        {
          originX: 'end',
          originY: 'bottom',
          overlayX: 'end',
          overlayY: 'top',
        },
      ]
    });
  }
  backToSuperAdmin() {
    this.impersonateServices.backToSuperAdmin();
    this.impersonateServices.callRolePermissions(true);
  }
  /*openSearch() {
    this.layoutService.openSearch();
  }*/

  ngOnDestroy(){
    // this.destroySubject$.next();
    // this.destroySubject$.complete();
  }
}
