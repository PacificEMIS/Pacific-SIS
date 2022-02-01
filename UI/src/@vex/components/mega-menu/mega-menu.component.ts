import { Component, OnInit } from '@angular/core';
import { Icon } from '@visurel/iconify-angular';
import icSchool from '@iconify/icons-ic/twotone-account-balance';
import icStudents from '@iconify/icons-ic/twotone-school';
import icStaff from '@iconify/icons-ic/twotone-people';
import icEvent from '@iconify/icons-ic/twotone-event';
import icNotice from '@iconify/icons-ic/twotone-assignment';
import { PopoverRef } from '../popover/popover-ref';
import { PageRolesPermission } from 'src/app/common/page-roles-permissions.service';
import { SchoolCreate } from 'src/app/enums/school-create.enum';
import { SchoolService } from 'src/app/services/school.service';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { TranslateService } from '@ngx-translate/core';

export interface MegaMenuFeature {
  icon?: Icon;
  label: string;
  route: string;
  isActive: boolean;
}

export interface MegaMenuPage {
  label: string;
  route: string;
}

@Component({
  selector: 'vex-mega-menu',
  templateUrl: './mega-menu.component.html'
})
export class MegaMenuComponent implements OnInit {
  stateObj = {type: SchoolCreate.ADD};

  items:number;
  features: MegaMenuFeature[] = [
    {
      icon: icSchool,
      label: 'school',
      route: '/school/schoolinfo/generalinfo',
      isActive: true,
    },
    {
      icon: icStudents,
      label: 'student',
      route: '/school/students/student-generalinfo',
      isActive: true,
    },
    {
      icon: icStaff,
      label: 'staff',
      route: '/school/staff/staff-generalinfo',
      isActive: true,
    },
    {
      icon: icEvent,
      label: 'event',
      route: '/school/schoolcalendars',
      isActive: true,
    },
    {
      icon: icNotice,
      label: 'notice',
      route: '/school/notices',
      isActive: true,
    }
  ];

  pages: MegaMenuPage[] = [
    {
      label: 'All-In-One Table',
      route: '/apps/aio-table'
    },
    {
      label: 'Authentication',
      route: '/login'
    },
    {
      label: 'Components',
      route: '/ui/components/overview'
    },
    {
      label: 'Documentation',
      route: '/documentation'
    },
    {
      label: 'FAQ',
      route: '/pages/faq'
    },
    {
      label: 'Form Elements',
      route: '/ui/forms/form-elements'
    },
    {
      label: 'Form Wizard',
      route: '/ui/forms/form-wizard'
    },
    {
      label: 'Guides',
      route: '/pages/guides'
    },
    {
      label: 'Help Center',
      route: '/apps/help-center'
    },
    {
      label: 'Scrumboard',
      route: '/apps/scrumboard'
    }
  ];

  constructor(
    private popoverRef: PopoverRef<MegaMenuComponent>,
    private pageRolePermission: PageRolesPermission,
    public schoolService: SchoolService,
    public defaultValueService: DefaultValuesService,
    public translateService: TranslateService,
    ) {

      this.features.map((item: any)=>{
        if(item.route === '/school/schoolinfo/generalinfo') {
          item.isActive = this.pageRolePermission.checkPageRolePermission(item.route, null, true).edit;
        }
        else if(item.route === '/school/students/student-generalinfo') {
            item.isActive = this.defaultValueService.checkAcademicYear() && this.pageRolePermission.checkPageRolePermission(item.route).edit ? true : false;
        }
        else if (item.route === '/school/staff/staff-generalinfo') {
          item.isActive = this.defaultValueService.checkAcademicYear() && this.pageRolePermission.checkPageRolePermission(item.route).edit ? true : false;
        }
        else if (item.route === '/school/schoolcalendars') {
          item.isActive = this.defaultValueService.checkAcademicYear() && this.pageRolePermission.checkPageRolePermission(item.route).edit ? true : false;
        }
        else if (item.route === '/school/notices') {
          item.isActive = this.defaultValueService.checkAcademicYear() && this.pageRolePermission.checkPageRolePermission(item.route).edit ? true : false;
        }
      })

      this.items= this.features.filter(x=> x.isActive)?.length;
    // popoverRef.data[0]?.permissionGroup?.permissionCategory?.map((item) => {
    //   if(item.rolePermission[0].canView){
    //     this.features.push({
    //       icon: this.pickIcon(item.permissionCategoryName),
    //       label: item.title,
    //       route: item.path
    //     });
    //   }
    // });
  }

  // pickIcon(categoryName) {
  //   switch (categoryName) {
  //     case 'School':{
  //       return icSchool;
  //     }
  //     case 'Student':{
  //       return icStudents;

  //     }
  //     case 'Staff':{
  //       return icStaff;

  //     }
  //     case 'Event':{
  //       return icEvent;

  //     }
  //     case 'Notice':{
  //       return icNotice;
  //     }
  //   }
  // }
  ngOnInit() {
  }

  close() {
    this.popoverRef.close();
  }
}
