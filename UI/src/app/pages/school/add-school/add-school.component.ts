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

import { Component, OnInit, OnDestroy } from '@angular/core';
import { fadeInRight400ms } from '../../../../@vex/animations/fade-in-right.animation';
import { stagger60ms } from '../../../../@vex/animations/stagger.animation';
import { fadeInUp400ms } from '../../../../@vex/animations/fade-in-up.animation';
import { LoaderService } from 'src/app/services/loader.service';
import { filter, takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { PageRolesPermission } from 'src/app/common/page-roles-permissions.service';
import { MatDialog } from '@angular/material/dialog';
import { AddCopySchoolComponent } from './add-copy-school/add-copy-school.component';
import { SchoolAddViewModel } from 'src/app/models/school-master.model';
import { SuccessCopySchoolComponent } from './success-copy-school/success-copy-school.component';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { SchoolService } from 'src/app/services/school.service';
import { NavigationEnd, Router, RouterEvent } from '@angular/router';
import { FieldsCategoryListView, FieldsCategoryModel } from 'src/app/models/fields-category.model';
import { CustomFieldService } from 'src/app/services/custom-field.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CommonService } from 'src/app/services/common.service';
import icAccountBalance from '@iconify/icons-ic/twotone-account-balance';
import icCleanHands from '@iconify/icons-ic/outline-clean-hands';
import { Module } from 'src/app/enums/module.enum';
import { SchoolCreate } from 'src/app/enums/school-create.enum';

@Component({
  // changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'vex-add-school',
  templateUrl: './add-school.component.html',
  styleUrls: ['./add-school.component.scss'],
  animations: [
    fadeInRight400ms,
    stagger60ms,
    fadeInUp400ms
  ]
})

export class AddSchoolComponent implements OnInit, OnDestroy {
  responseImage: string;
  secondarySidebar: number;
  destroySubject$: Subject<void> = new Subject();
  loading: boolean;
  schoolTitle: string;
  isCopySchoolPossible: boolean;
  schoolId: string;
  schoolAddViewModel: SchoolAddViewModel =  new SchoolAddViewModel();
  schoolCount: any;
  fieldsCategoryListView: FieldsCategoryListView = new FieldsCategoryListView();
  fieldsCategory;
  currentCategory: number;
  icAccountBalance = icAccountBalance;
  icCleanHands = icCleanHands;
  pageStatus = 'Add School';
  schoolCreateMode: SchoolCreate = SchoolCreate.ADD;
  schoolCreate = SchoolCreate;
  permittedDetails;


  constructor(
    private loaderService: LoaderService,
    private pageRolePermission: PageRolesPermission,
    private dialog: MatDialog,
    private defaultValueService: DefaultValuesService,
    private schoolService: SchoolService,
    private router: Router,
    private customFieldservice: CustomFieldService,
    private snackbar: MatSnackBar,
    private commonService: CommonService
  ) {
    this.schoolCount = this.defaultValueService.getSchoolCount();
    this.commonService.setModuleName(Module.SCHOOL);
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });

    this.schoolCreateMode = this.router.getCurrentNavigation().extras.state ? this.router.getCurrentNavigation().extras.state.type: this.schoolCreateMode;

    this.permittedDetails= this.pageRolePermission.getPermittedSubCategories('/school/schoolinfo/generalinfo');
    if(this.permittedDetails.length){
    this.checkInitialState(this.permittedDetails[0].path);
    }
   
    this.schoolService.schoolCreatedMode.pipe(takeUntil(this.destroySubject$)).subscribe((res) => {
      this.schoolCreateMode = res;
    });

    this.schoolService.schoolDetailsForViewedAndEdited.subscribe((res)=>{
      if(res){
        this.schoolAddViewModel = res;
      }
    });
  }

  ngOnInit() {
    this.router.onSameUrlNavigation = 'reload';

    this.router.events.pipe(takeUntil(this.destroySubject$)).pipe(
      filter((event: RouterEvent) => event instanceof NavigationEnd)
    ).subscribe((res) => {
     this.checkInitialState(res.url);
    });
    
  }

  checkInitialState(url) {
    this.schoolCreateMode = this.router.getCurrentNavigation().extras.state ? this.router.getCurrentNavigation().extras.state.type: this.schoolCreateMode;
    this.schoolService.setSchoolCreateMode(this.schoolCreateMode);
    if (url === '/school/schoolinfo/generalinfo') {
      this.currentCategory = 1;
      this.checkCreatemodeAndExecute();
    }
    else if(url === '/school/schoolinfo/washinfo') {
      this.currentCategory = 2;
      this.checkCreatemodeAndExecute();
    } else {
      this.checkCreatemodeAndExecute();
    }    
  }

  toggleSecondarySidebar() {
    if (this.secondarySidebar === 0) {
      this.secondarySidebar = 1;
    } else {
      this.secondarySidebar = 0;
    }
  }

  checkIfCopySchoolPossible() {
    let permissions = this.pageRolePermission.checkPageRolePermission("/school/schoolinfo/generalinfo", null, true);
    this.isCopySchoolPossible = permissions?.add;
  }

  checkCreatemodeAndExecute() {
    if (this.schoolCreateMode === SchoolCreate.EDIT) {
      if(!this.schoolAddViewModel.schoolMaster.schoolDetail[0].schoolId) {
          this.getSchoolGeneralandWashInfoDetails();
      } else {
        if(this.schoolAddViewModel.schoolMaster.schoolDetail[0].schoolId !== this.defaultValueService.getSchoolID()){
          this.getSchoolGeneralandWashInfoDetails();
        }
      }
    }
    if(this.schoolCreateMode === SchoolCreate.VIEW) {
      if(this.schoolService.getSchoolId()) this.defaultValueService.setSchoolID(this.schoolService.getSchoolId(), true);
      if(!this.schoolAddViewModel.schoolMaster.schoolDetail[0].schoolId) {
        this.getSchoolGeneralandWashInfoDetails();
    } else {
      if(this.schoolAddViewModel.schoolMaster.schoolDetail[0].schoolId !== this.defaultValueService.getSchoolID()){
        this.getSchoolGeneralandWashInfoDetails();
      }
    }
    }
    if (this.schoolCreateMode === SchoolCreate.ADD) {
      let permission = this.pageRolePermission.checkPageRolePermission('/school/schoolinfo/generalinfo', null, true)
      if (!permission.add) {
        this.router.navigate(['/school', 'schoolinfo']);
        this.snackbar.open('School didnot have permission to add details.', '', {
          duration: 10000
        });
      }
      this.isCopySchoolPossible = false;
      this.schoolAddViewModel = new SchoolAddViewModel();
      this.responseImage = undefined;
      this.schoolService.setSchoolImage(null);
      this.schoolService.setSchoolDetailsForViewAndEdit(this.schoolAddViewModel);
      this.getAllFieldsCategory();
    }
  }

/* This addCopySchool method is used for open Copy School Modal and after close the modal it calls
  successCopySchool method and pass the new school name and new school id. */
  addCopySchool() {
    this.dialog.open(AddCopySchoolComponent, {
      width: '800px',
      data: { fromSchoolId: this.schoolId, fromSchoolName: this.schoolAddViewModel.schoolMaster.schoolName }
    }).afterClosed().subscribe((data) => {
      if (data) {
        this.successCopySchool(data);
      }
    });
  }

  /* This successCopySchool method is used for Open Success Copy School Modal and after close the modal
   it render the new copied school details. */
   successCopySchool(schoolData) {
    this.dialog.open(SuccessCopySchoolComponent, {
      width: '500px',
      data: { newSchoolData: schoolData }
    }).afterClosed().subscribe((data) => {
      if (data) {
        if(this.schoolService.getSchoolId()) this.defaultValueService.setSchoolID(this.schoolService.getSchoolId(), true);
        this.getSchoolGeneralandWashInfoDetails();
      }
    });
  }

  getSchoolGeneralandWashInfoDetails() {
    this.schoolService.ViewSchool(this.schoolAddViewModel).subscribe(data => {
      if (data._failure) {
        this.commonService.checkTokenValidOrNot(data._message);
      } else {
      this.schoolAddViewModel = data;
      if (data.schoolMaster.schoolId === +this.defaultValueService.getSchoolID()) {
        this.fieldsCategory = this.checkViewPermission(data.schoolMaster.fieldsCategory);
        this.checkIfCopySchoolPossible();
      } else {
        this.isCopySchoolPossible = true;
        this.fieldsCategory = this.schoolAddViewModel.schoolMaster.fieldsCategory;
        this.currentCategory = this.fieldsCategory[0].categoryId;
      }

      const index = this.schoolAddViewModel.schoolMaster.fieldsCategory.findIndex(x=> x.categoryId === this.fieldsCategory[0].categoryId)
      this.changeCategory({categoryId: this.currentCategory, title: this.fieldsCategory[0].title}, index);
      this.schoolService.setSchoolImage(this.schoolAddViewModel.schoolMaster.schoolDetail[0].schoolLogo);
      this.schoolService.setSchoolCloneImage(this.schoolAddViewModel.schoolMaster.schoolDetail[0].schoolLogo);
      this.schoolService.setSchoolDetailsForViewAndEdit(this.schoolAddViewModel);
    }
    });
  }

  showAllSchools() {
    this.router.navigate(['/school', 'schoolinfo']).then(()=>{
      this.schoolAddViewModel = new SchoolAddViewModel();
    this.schoolService.setSchoolDetailsForViewAndEdit(this.schoolAddViewModel);
    });
  }

  getAllFieldsCategory() {
    this.fieldsCategoryListView.module = Module.SCHOOL;
    this.customFieldservice.getAllFieldsCategory(this.fieldsCategoryListView).subscribe((res) => {
      if (res) {
        if (res._failure) {
          this.commonService.checkTokenValidOrNot(res._message);
          if (!res.fieldsCategoryList) {
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
        }
        else {
          this.fieldsCategory = res.fieldsCategoryList.filter(x => x.isSystemCategory === true);
          this.fieldsCategory = this.checkViewPermission(res.fieldsCategoryList);
          this.currentCategory = this.fieldsCategory[0].categoryId;
        }
      } else {
        this.snackbar.open('Custom Field list failed. ' + this.defaultValueService.getHttpError(), '', {
          duration: 10000
        });
      }
    }
    );
  }

  checkViewPermission(category) {
    let filteredCategory = [];
    let permittedTabs = this.permittedDetails;
    for (let item of category) {
      for (let permission of permittedTabs) {
        if (item.title.toLowerCase() === permission.title.toLowerCase()) {
          filteredCategory.push(item);
          break;
        }
      }
    };
    this.currentCategory = filteredCategory[0]?.categoryId;
    return filteredCategory;
  }

  changeCategory(categoryDetails, index) {
    if(this.schoolCreateMode === this.schoolCreate.ADD) return;    
    this.currentCategory = categoryDetails.categoryId;
    this.schoolService.setSchoolCreateMode(this.schoolCreateMode);
    const categoryIndex = this.schoolAddViewModel.schoolMaster.fieldsCategory.findIndex(x=> x.title === categoryDetails.title)
    this.secondarySidebar = 0; // Close secondary sidenav in mobile view
    this.checkCurrentCategoryAndRoute(categoryDetails.title, categoryIndex);
  }

  checkCurrentCategoryAndRoute(categoryTitle, index) {
    if (this.currentCategory === 1 && this.router.url !== '/school/schoolinfo/generalinfo') {
      this.router.navigate(['/school', 'schoolinfo', 'generalinfo']);
    }
    else if (this.currentCategory === 2 && this.router.url !== '/school/schoolinfo/washinfo') {
      this.router.navigate(['/school', 'schoolinfo', 'washinfo']);
    } else if (this.currentCategory > 2 && this.router.url !== '/school/schoolinfo/custom/'+categoryTitle.trim().toLowerCase().split(' ').join('-')) {
      this.router.navigate(['/school', 'schoolinfo', 'custom', categoryTitle.trim().toLowerCase().split(' ').join('-')], {state: {type: this.schoolCreateMode, categoryId: index, categoryTitle}}).then(()=>{

      });
    }
  }

  ngOnDestroy() {
    this.defaultValueService.setSchoolID(undefined);
    this.schoolService.setSchoolId(undefined);
    this.schoolService.setSchoolDetailsForViewAndEdit(new SchoolAddViewModel());
    this.schoolService.setSchoolImage(null);
    this.destroySubject$.next();
    this.destroySubject$.unsubscribe();
  }
}