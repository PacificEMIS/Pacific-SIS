import { Component, Input, OnInit } from '@angular/core';
import { trackByRoute } from '../../utils/track-by';
import { NavigationService } from '../../services/navigation.service';
// import icRadioButtonChecked from '@iconify/icons-ic/twotone-radio-button-checked';
// import icRadioButtonUnchecked from '@iconify/icons-ic/twotone-radio-button-unchecked';
import icCollapseSidebar from '@iconify/icons-ic/twotone-switch-left';
import icExpandSidebar from '@iconify/icons-ic/twotone-switch-right';
import icArrowDropDown from '@iconify/icons-ic/arrow-drop-down';
import { LayoutService } from '../../services/layout.service';
import { ConfigService } from '../../services/config.service';
import { map, takeUntil } from 'rxjs/operators';
import { Router, ActivatedRoute } from '@angular/router';
import { Subject } from 'rxjs';
import { ChangePasswordComponent } from './change-password/change-password.component';
import { MatDialog } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { CommonService } from 'src/app/services/common.service';
import { StaffService } from 'src/app/services/staff.service';
import { PageRolesPermission } from 'src/app/common/page-roles-permissions.service';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { SchoolCreate } from 'src/app/enums/school-create.enum';
import { ImpersonateServices } from 'src/app/services/impersonate.service';
import { ProfilesTypes } from 'src/app/enums/profiles.enum';

@Component({
  selector: 'vex-sidenav',
  templateUrl: './sidenav.component.html',
  styleUrls: ['./sidenav.component.scss']
})
export class SidenavComponent implements OnInit {

  @Input() collapsed: boolean;
  globalCollapseValue;
  collapsedOpen$ = this.layoutService.sidenavCollapsedOpen$;
  title$ = this.configService.config$.pipe(map(config => config.sidenav.title));
  imageUrl$ = this.configService.config$.pipe(map(config => config.sidenav.imageUrl));
  showCollapsePin$ = this.configService.config$.pipe(map(config => config.sidenav.showCollapsePin));

  items = this.navigationService.items;
  trackByRoute = trackByRoute;
  // icRadioButtonChecked = icRadioButtonChecked;
  // icRadioButtonUnchecked = icRadioButtonUnchecked;
  icCollapseSidebar = icCollapseSidebar;
  icExpandSidebar = icExpandSidebar;
  icArrowDropDown = icArrowDropDown;
  userName: any;
  membershipName: string;
  userPhoto;
  destroySubject$ = new Subject<void>();
  tenantLogoIcon: any;
  tenantName: any;
  tenantSidenavLogo: any;
  pageId: string;
  permittedSubmenuList:any;
  impersonateSubjectForsideNav:boolean=false;
  profile = ProfilesTypes;
  constructor(private navigationService: NavigationService,
    private layoutService: LayoutService,
    private configService: ConfigService,
    private router: Router,
    private staffService: StaffService,
    private pageRolePermission: PageRolesPermission,
    public translateService: TranslateService,
    private dialog: MatDialog,
    private commonService: CommonService,
    public defaultValuesService:DefaultValuesService,
    private impersonateServices:ImpersonateServices
    ) {
      
      this.tenantSidenavLogo = this.defaultValuesService.getPhotoAndFooter().tenantSidenavLogo;
      this.tenantLogoIcon = this.defaultValuesService.getPhotoAndFooter().tenantLogoIcon;
      this.tenantName = this.defaultValuesService.getTenantName();

      this.navigationService.menuItems.pipe(takeUntil(this.destroySubject$)).subscribe((res) => {
        if (res) {
          this.items = this.navigationService.items;
        }
      });
      // translateService.use("en");
      this.impersonateServices.impersonateSubjectForsideNav.subscribe(x=>{
        if(x) {
          this.impersonateSubjectForsideNav=true;
          this.impersonateSideNavOnInit();
        }
      })
  }

  ngOnInit() {
    this.impersonateSideNavOnInit()
  }
  impersonateSideNavOnInit() {
    this.userName = this.defaultValuesService.getFullUserName();

    this.defaultValuesService.newSubject.subscribe(data => {
      this.userName = data;
    })

    this.membershipName = this.defaultValuesService.getuserMembershipName();
    this.getUserPhoto();
  }

  expandSidebar(){
    this.layoutService.expandSidenav();
  }

  getUserPhoto() {
    let photo = this.defaultValuesService.getuserPhoto();

    if (photo) {
      this.userPhoto = 'data:image/png;base64,' + photo;
    } else {
      this.userPhoto = '../../../assets/img/profilePic.jpg';
    }
    this.defaultValuesService.photoChanged.subscribe(data => {
      this.userPhoto = 'data:image/png;base64,' + data;
    });
  }

  onMouseEnter() {
    this.layoutService.collapseOpenSidenav();
  }

  onMouseLeave() {
    this.layoutService.collapseCloseSidenav();
  }

  toggleCollapse() {
    if (this.collapsed) {
      this.defaultValuesService.setCollapseValue("false");
      this.layoutService.expandSidenav()
    } else {
      this.layoutService.collapseSidenav();
      this.defaultValuesService.setCollapseValue("true");
    }

  }
  logOut() {
    this.commonService.logoutUser();
  }

  showMyAccount() {
    let userId = this.defaultValuesService.getUserId();
    this.staffService.setStaffId(+userId);
    let permittedDetails = this.pageRolePermission.getPermittedSubCategories('/school/staff');
    if (permittedDetails.length) {
      this.staffService.setCategoryTitle(permittedDetails[0].title);
      this.router.navigateByUrl(permittedDetails[0].path, {state: {type: SchoolCreate.VIEW}});

      this.staffService.setCategoryId(0);
    }
  }

  openChangePassword() {
    this.dialog.open(ChangePasswordComponent, {
      width: "500px",
    });
  }

  showPreference() {
    this.defaultValuesService.setPageId('Preference');
    this.router.navigateByUrl('/school/settings/school-settings');
}

  ngOnDestroy() {
    this.navigationService.changeMenuItemsStatus(false);
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }
}
