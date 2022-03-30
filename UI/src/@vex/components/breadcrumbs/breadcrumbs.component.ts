import { Component, Input, OnInit } from '@angular/core';
import icHome from '@iconify/icons-ic/twotone-home';
import { trackByValue } from '../../utils/track-by';
import { TranslateService } from '@ngx-translate/core';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { ProfilesTypes } from 'src/app/enums/profiles.enum';

@Component({
  selector: 'vex-breadcrumbs',
  template: `
    <div class="flex items-center">
      <vex-breadcrumb>
        <a *ngIf="membershipType === profiles.SuperAdmin || membershipType === profiles.SchoolAdmin || membershipType === profiles.AdminAssitant" [routerLink]="['/school/dashboards']">
          <ic-icon [icon]="icHome" inline="true" size="20px"></ic-icon>
        </a>
        <a *ngIf="membershipType === profiles.Teacher || membershipType === profiles.HomeroomTeacher" [routerLink]="['/school/teacher/dashboards']">
          <ic-icon [icon]="icHome" inline="true" size="20px"></ic-icon>
        </a>
      </vex-breadcrumb>
      <ng-container *ngFor="let crumb of crumbs;let last=last; trackBy: trackByValue">
        <div class="w-1 h-1 bg-gray rounded-full ltr:mr-2 rtl:ml-2"></div>
        <vex-breadcrumb>
        <div *ngIf="last"><a>{{ crumb |translate }}</a></div>
        <div *ngIf="!last"><a [routerLink]="['/school/' + crumb.replace(' ', '') |lowercase ]">{{ crumb |translate }}</a></div>
        </vex-breadcrumb>
      </ng-container>
 
    </div>
  `
})
export class BreadcrumbsComponent implements OnInit {

  @Input() crumbs: string[] = [];
  trackByValue = trackByValue;
  icHome = icHome;
  membershipType;
  profiles = ProfilesTypes;

  constructor(private translate:TranslateService,
    private defaultValuesService: DefaultValuesService) {
  }

  ngOnInit() {
    this.membershipType = this.defaultValuesService.getUserMembershipType();
  }
}
