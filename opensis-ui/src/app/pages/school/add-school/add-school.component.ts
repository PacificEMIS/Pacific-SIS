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

import { Component, OnInit, OnDestroy, ChangeDetectionStrategy, ChangeDetectorRef, AfterViewInit } from '@angular/core';
import { fadeInRight400ms } from '../../../../@vex/animations/fade-in-right.animation';
import { ImageCropperService } from '../../../services/image-cropper.service';

import { SchoolAddViewModel } from '../../../models/school-master.model';
import { ActivatedRoute, Router } from '@angular/router';
import { SchoolService } from '../../../services/school.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { SharedFunction } from '../../../pages/shared/shared-function';
import { CommonService } from '../../../services/common.service';
import { LayoutService } from '../../../../../src/@vex/services/layout.service';
import { CustomFieldAddView, CustomFieldListViewModel, CustomFieldModel } from '../../../models/custom-field.model';
import { CustomFieldService } from '../../../services/custom-field.service';
import { FieldsCategoryAddView, FieldsCategoryListView, FieldsCategoryModel } from '../../../models/fields-category.model';
import { LoaderService } from '../../../services/loader.service';
import { SchoolCreate } from '../../../enums/school-create.enum';
import { takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { ModuleIdentifier } from '../../../enums/module-identifier.enum';
import { RolePermissionListViewModel } from 'src/app/models/roll-based-access.model';
import { CryptoService } from 'src/app/services/Crypto.service';
import { stagger60ms } from '../../../../@vex/animations/stagger.animation';
import { fadeInUp400ms } from '../../../../@vex/animations/fade-in-up.animation';
import { MatDialog } from '@angular/material/dialog';
import { AddCopySchoolComponent } from './add-copy-school/add-copy-school.component';
import { SuccessCopySchoolComponent } from './success-copy-school/success-copy-school.component';
import icCleanHands from '@iconify/icons-ic/outline-clean-hands';
import icArticle from '@iconify/icons-ic/outline-article';
import { DefaultValuesService } from '../../../common/default-values.service';
import { PageRolesPermission } from '../../../common/page-roles-permissions.service';

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
  schoolCreate = SchoolCreate;
  schoolCreateMode: SchoolCreate = SchoolCreate.ADD;
  schoolTitle = 'Add School Information';
  pageStatus = 'Add School';
  responseImage: string;
  image = '';
  module = 'School';
  fieldsCategory: FieldsCategoryModel[];
  fieldsCategoryListView = new FieldsCategoryListView();
  schoolId: number = null;
  enableCropTool = false;
  indexOfCategory = 0;
  schoolAddViewModel: SchoolAddViewModel = new SchoolAddViewModel();
  customFieldModel: [CustomFieldModel];
  currentCategory;
  secondarySidebar = 0;

  loading: boolean;
  destroySubject$: Subject<void> = new Subject();
  moduleIdentifier = ModuleIdentifier;
  icCleanHands = icCleanHands;
  icArticle = icArticle;
  categoryTitle: string;
  isCopySchoolPossible = false;
  constructor(private imageCropperService: ImageCropperService,
              private snackbar: MatSnackBar,
              private schoolService: SchoolService,
              private router: Router,
              private commonService:CommonService,
              private layoutService: LayoutService,
              private loaderService: LoaderService,
              private customFieldservice: CustomFieldService,
              private pageRolePermission: PageRolesPermission,
              private dialog: MatDialog,
              private defaultValueService: DefaultValuesService,
              private cdr: ChangeDetectorRef) {
    this.layoutService.collapseSidenav();
    this.imageCropperService.getUncroppedEvent().pipe(takeUntil(this.destroySubject$)).subscribe((res) => {
      this.schoolService.setSchoolImage(res);
    });
    this.schoolService.modeToUpdate.pipe(takeUntil(this.destroySubject$)).subscribe((res:any) => {
      // this.schoolCreateMode = res;
      if (this.schoolCreateMode === this.schoolCreate.VIEW){
        this.pageStatus = 'View School';
      }else{
        this.pageStatus = 'Edit School';
      }
    });
    this.schoolService.categoryToSend.pipe(takeUntil(this.destroySubject$)).subscribe((res) => {
        // this.currentCategory = this.currentCategory + 1;
        if(this.fieldsCategory?.length>1){
          this.currentCategory=this.fieldsCategory[1].categoryId;
        }
        if(this.schoolService.getSchoolDetails().schoolMaster?.schoolName){
          this.schoolTitle = this.schoolService.getSchoolDetails().schoolMaster.schoolName;
        }
    this.checkCurrentCategoryAndRoute();

    });
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });
    this.schoolService.getSchoolDetailsForGeneral.pipe(takeUntil(this.destroySubject$)).subscribe((res: SchoolAddViewModel) => {
      this.schoolAddViewModel = res;
      this.schoolService.setSchoolDetailsForViewAndEdit(this.schoolAddViewModel);
    });
    this.schoolService.updatedSchoolName.pipe(takeUntil(this.destroySubject$)).subscribe((name: string)=>{
      if(name){
        this.schoolTitle=name;
      }
    })

    // this.getCurrentCategory();
  }

  ngOnInit() {
    this.commonService.setModuleName(this.module);
    this.schoolCreateMode = this.schoolCreate.ADD;
    this.schoolService.setSchoolCreateMode(this.schoolCreateMode);
    this.schoolService.sendDetails(this.schoolAddViewModel);
    this.schoolId = this.schoolService.getSchoolId();
    if (this.schoolId) {
      this.schoolCreateMode = this.schoolCreate.VIEW;
      this.schoolService.setSchoolCreateMode(this.schoolCreateMode);
      this.getSchoolGeneralandWashInfoDetails();
      this.onViewMode();
    }else if (this.schoolCreateMode === this.schoolCreate.ADD) {
    let permission = this.pageRolePermission.checkPageRolePermission('/school/schoolinfo/generalinfo')
    if(!permission.add){
      this.router.navigate(['/school', 'schoolinfo'])
    }
      // if(this.checkCanAddSchool()) {
        this.getAllFieldsCategory();
        this.imageCropperService.enableUpload({module: this.moduleIdentifier.SCHOOL, upload: true, mode: this.schoolCreate.ADD});
      // } else{
      //   this.router.navigate(['/school','schoolinfo']);
      // }
     
    }

  }
  checkIfCopySchoolPossible(){
    let permissions=this.pageRolePermission.checkPageRolePermission("/school/schoolinfo/generalinfo");
    this.isCopySchoolPossible = permissions?.add;
  }


  ngAfterViewChecked() {
    this.cdr.detectChanges();
  }

  onViewMode() {
    this.pageStatus = 'View School';
  }

  changeCategory(categoryDetails, index) {
    // if(this.schoolCreateMode === this.schoolCreate.ADD) return;
    this.categoryTitle=categoryDetails.title;
    this.schoolService.setCategoryTitle(this.categoryTitle);
    this.commonService.setModuleName(this.module);
    const schoolDetails = this.schoolService.getSchoolDetails();
    if (schoolDetails) {
      this.currentCategory = categoryDetails.categoryId;
      this.indexOfCategory = index;
      this.schoolCreateMode = this.schoolCreate.EDIT;
      this.schoolAddViewModel = schoolDetails;
      this.schoolService.setSchoolDetailsForViewAndEdit(this.schoolAddViewModel);
    }
    if (this.schoolCreateMode === this.schoolCreate.VIEW) {
      this.currentCategory = categoryDetails.categoryId;
      this.indexOfCategory = index;
      this.pageStatus = "View Student";
    }

    this.schoolService.setCategoryId(this.indexOfCategory);
    this.schoolService.setSchoolCreateMode(this.schoolCreateMode);
    this.secondarySidebar = 0; // Close secondary sidenav in mobile view
    this.checkCurrentCategoryAndRoute();
  }

  checkCurrentCategoryAndRoute() {
    if(this.currentCategory === 1) {
      this.router.navigate(['/school', 'schoolinfo', 'generalinfo']);
    }
    else if(this.currentCategory === 2) {
      this.router.navigate(['/school', 'schoolinfo', 'washinfo']);
    }else if(this.currentCategory>2){
      this.router.navigate(['/school', 'schoolinfo', 'custom', this.categoryTitle.trim().toLowerCase().split(' ').join('-') ]);
    }
  }


  getAllFieldsCategory() {
    this.fieldsCategoryListView.module = 'School';
    this.fieldsCategoryListView.schoolId = +sessionStorage.getItem('selectedSchoolId');
    this.customFieldservice.getAllFieldsCategory(this.fieldsCategoryListView).subscribe((res) => {
      if (typeof (res) === 'undefined') {
        this.snackbar.open('Custom Field list failed. ' + sessionStorage.getItem('httpError'), '', {
          duration: 10000
        });
      }
      else {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          if(!res.fieldsCategoryList){
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
      }
    }
    );
  }

  checkViewPermission(category){
    let filteredCategory=[];
    let permittedTabs =this.pageRolePermission.getPermittedSubCategories('/school/schoolinfo')
     for(let item of category){
      for(let permission of permittedTabs){
        if ( item.title.toLowerCase() === permission.title.toLowerCase()){
          filteredCategory.push(item);
          break;
        }
      }
    };
    this.currentCategory = filteredCategory[0]?.categoryId;
    return filteredCategory;
  }

  getSchoolGeneralandWashInfoDetails() {
    this.schoolAddViewModel.schoolMaster.schoolDetail[0].schoolId = this.schoolService.getSchoolId();
    this.schoolAddViewModel.schoolMaster.schoolId = this.schoolAddViewModel.schoolMaster.schoolDetail[0].schoolId;
    this.schoolService.ViewSchool(this.schoolAddViewModel).subscribe(data => {
      if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
      }
      this.schoolAddViewModel = data;
      this.responseImage = this.schoolAddViewModel.schoolMaster.schoolDetail[0].schoolLogo;
      this.schoolAddViewModel.schoolMaster.schoolDetail[0].schoolLogo = null;
      if(data.schoolMaster.schoolId===+this.defaultValueService.getSchoolID()){
        this.fieldsCategory = this.checkViewPermission(data.schoolMaster.fieldsCategory);
        this.checkIfCopySchoolPossible();
        this.schoolAddViewModel.schoolMaster.fieldsCategory = this.fieldsCategory;
      }else{
        this.isCopySchoolPossible = true;
        this.fieldsCategory=this.schoolAddViewModel.schoolMaster.fieldsCategory;
        this.currentCategory = this.fieldsCategory[0].categoryId;
      }
      this.schoolService.sendDetails(this.schoolAddViewModel);
      this.schoolTitle = this.schoolAddViewModel.schoolMaster.schoolName;
      this.schoolService.setSchoolImage(this.responseImage);
      this.schoolService.setSchoolCloneImage(this.responseImage);
    });
    this.schoolService.setSchoolDetailsForViewAndEdit(this.schoolAddViewModel);
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
        this.getSchoolGeneralandWashInfoDetails();
      }
    });
  }

  toggleSecondarySidebar() {
    if(this.secondarySidebar === 0){
      this.secondarySidebar = 1;
    } else {
      this.secondarySidebar = 0;
    }
  }

  ngOnDestroy() {
    this.schoolService.setSchoolDetails(undefined);
    this.schoolService.setSchoolImage(null);
    this.schoolService.setSchoolId(null);
    this.schoolService.setSchoolCloneImage(null);
    this.schoolService.setCategoryTitle(null);
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }

}
