import { Component, Input, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { trackByValue } from '../../utils/track-by';
import { TranslateService } from '@ngx-translate/core';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { ProfilesTypes } from 'src/app/enums/profiles.enum';

@Component({
    selector: 'vex-breadcrumbs',
    template: `
    <div class="flex items-center">
      <vex-breadcrumb>
        @if (membershipType === profiles.SuperAdmin || membershipType === profiles.SchoolAdmin || membershipType === profiles.AdminAssitant) {
          <a [routerLink]="['/school/dashboards']">
            <mat-icon style="font-size: 20px">home</mat-icon>
          </a>
        }
        @if (membershipType === profiles.Teacher || membershipType === profiles.HomeroomTeacher) {
          <a [routerLink]="['/school/teacher/dashboards']">
            <mat-icon style="font-size: 20px">home</mat-icon>
          </a>
        }
      </vex-breadcrumb>
      @for (crumb of crumbs; track trackByValue($index, crumb); let last = $last) {
        <div class="w-1 h-1 bg-gray rounded-full ltr:mr-2 rtl:ml-2"></div>
        <vex-breadcrumb>
          @if (last) {
            <div><a>{{ crumb |translate }}</a></div>
          }
          @if (!last) {
            <div><a [routerLink]="crumb === 'schoolInfo' ? ['/school/schoolinfo/generalinfo'] : path" [state]="{type: 2}">{{ crumb |translate }}</a></div>
          }
        </vex-breadcrumb>
      }
    
    </div>
    `,
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class BreadcrumbsComponent implements OnInit {

  @Input() crumbs: string[] = [];
  trackByValue = trackByValue;
  icHome = 'home';
  membershipType;
  profiles = ProfilesTypes;
  schoolCount;
  path;

  constructor(private translate:TranslateService,
    private defaultValuesService: DefaultValuesService) {
  }

  ngOnInit() {
    this.membershipType = this.defaultValuesService.getUserMembershipType();
    this.schoolCount = parseInt(JSON.parse(localStorage.getItem("schoolCount")))
    // this.path = this.schoolCount > 0 ? ['/school/schoolinfo'] : ['/school/schoolinfo/generalinfo']
    if (this.crumbs.includes('staff')) {
      if (this.membershipType === this.profiles.SuperAdmin || this.membershipType === this.profiles.SchoolAdmin || this.membershipType === this.profiles.AdminAssitant)
        this.path = ['/school/staff']
      else
        this.path = ['/school/staff/staff-generalinfo']
    }
    else if (this.crumbs.includes('school')) {
      this.path = this.schoolCount > 1 ? ['/school/schoolinfo'] : ['/school/schoolinfo/generalinfo'];
    }
    else if (this.crumbs.includes('schoolInfo')) {
      this.path = ['/school/schoolinfo/generalinfo'];
    }
    else if (this.crumbs.includes('students')) {
      if (this.membershipType === this.profiles.Student)
        this.path = ['/school/students/student-generalinfo'];
      else
        this.path = ['/school/students'];
    }
    else if (this.crumbs.includes('reports')) {
      this.path = ['/school/reports'];
    }
    else if (this.crumbs.includes('settings')) {
      this.path = ['/school/settings'];
    }
  }
}
