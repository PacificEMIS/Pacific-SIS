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

import { Component, OnInit, EventEmitter, Output, Input, ViewChild, OnDestroy } from '@angular/core';
import { fadeInUp400ms } from '../../../../../@vex/animations/fade-in-up.animation';
import { stagger60ms } from '../../../../../@vex/animations/stagger.animation';
import { Router } from '@angular/router';
import { FormGroup, FormBuilder, Validators, NgForm } from '@angular/forms';
import { WashInfoEnum } from '../../../../enums/wash-info.enum';
import { SchoolAddViewModel } from '../../../../models/school-master.model';
import { fadeInRight400ms } from '../../../../../@vex/animations/fade-in-right.animation';
import { TranslateService } from '@ngx-translate/core';
import { SchoolService } from '../../../../../app/services/school.service'
import { MatSnackBar } from '@angular/material/snack-bar';
import { LoaderService } from '../../../../../app/services/loader.service';
import { SharedFunction } from '../../../shared/shared-function';
import { ImageCropperService } from '../../../../services/image-cropper.service';
import icEdit from '@iconify/icons-ic/twotone-edit';
import { SchoolCreate } from '../../../../enums/school-create.enum';
import { LovList } from '../../../../models/lov.model';
import { CommonService } from '../../../../services/common.service';
import * as cloneDeep from 'lodash/cloneDeep';
import { CommonLOV } from '../../../shared-module/lov/common-lov';
import { takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { ModuleIdentifier } from '../../../../enums/module-identifier.enum';
import { Permissions, RolePermissionListViewModel, RolePermissionViewModel } from '../../../../models/roll-based-access.model';
import { RollBasedAccessService } from '../../../../services/roll-based-access.service';
import { PageRolesPermission } from '../../../../common/page-roles-permissions.service';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { Module } from 'src/app/enums/module.enum';
@Component({
  selector: 'vex-wash-info',
  templateUrl: './wash-info.component.html',
  styleUrls: ['./wash-info.component.scss'],
  animations: [
    stagger60ms,
    fadeInUp400ms,
    fadeInRight400ms
  ]
})
export class WashInfoComponent implements OnInit,OnDestroy {
  schoolCreate = SchoolCreate;
  moduleIdentifier = ModuleIdentifier;
  schoolCreateMode: SchoolCreate;
  icEdit = icEdit;
  // schoolDetailsForViewAndEdit;
  categoryId;
  form: FormGroup;
  washinfo = WashInfoEnum;
  @ViewChild('f') currentForm: NgForm;
  f: NgForm;
  module = Module.SCHOOL;
  schoolAddViewModel: SchoolAddViewModel = new SchoolAddViewModel();
  loading: boolean;
  formActionButtonTitle = "submit";
  femaleToiletTypeList;
  maleToiletTypeList;
  commonToiletTypeList;
  femaleToiletAccessibilityList;
  maleToiletAccessibilityList;
  commonToiletAccessibilityList;
  lovList: LovList = new LovList();
  cloneSchool;
  destroySubject$: Subject<void> = new Subject();
  permissions: Permissions
  customValid=false;
  constructor(
    private schoolService: SchoolService,
    private snackbar: MatSnackBar,
    public translateService: TranslateService,
    private imageCropperService: ImageCropperService,
    private commonService: CommonService,
    public rollBasedAccessService: RollBasedAccessService,
    private commonLOV:CommonLOV,
    private pageRolePermission: PageRolesPermission,
    private router: Router,
    private defaultValuesService: DefaultValuesService
    ) {
    //translateService.use('en');
    // this.schoolCreateMode = this.router.getCurrentNavigation().extras?.state ? this.router.getCurrentNavigation().extras.state.type : SchoolCreate.ADD;

  }
  ngOnInit(): void {
    this.permissions=this.pageRolePermission.checkPageRolePermission();

    
    this.schoolService.schoolCreatedMode.subscribe((res)=>{
      if(res>=0){
        this.schoolCreateMode = res;
      }
    });

    this.schoolService.schoolDetailsForViewedAndEdited.subscribe((res)=>{
      if(res){
        // this.schoolDetailsForViewAndEdit = res;
        this.schoolAddViewModel = res;
      }

    });

    // this.schoolService.categoryIdSelected.subscribe((res)=>{
    //   if(res>=0){
    //     this.categoryId = res;
    //   }
    // });

    // if (this.schoolCreateMode === this.schoolCreate.VIEW) {
      this.imageCropperService.enableUpload({module: this.moduleIdentifier.SCHOOL, upload: true, mode: this.schoolCreateMode});
      // this.schoolAddViewModel = this.schoolDetailsForViewAndEdit;
      this.cloneSchool = JSON.stringify(this.schoolAddViewModel);
    // } else {
      this.schoolCreateMode === this.schoolCreate.EDIT ? this.initializeDropdownValues() : '';
      // this.imageCropperService.enableUpload({module:this.moduleIdentifier.SCHOOL,upload:true,mode:this.schoolCreate.EDIT});
      // this.schoolService.setSchoolCreateMode(this.schoolCreateMode);
      // this.schoolAddViewModel = this.schoolService.getSchoolDetails();
      // this.cloneSchool = JSON.stringify(this.schoolAddViewModel);
    // }

    if(this.schoolCreateMode === SchoolCreate.ADD) {
      this.router.navigate(['/school', 'schoolinfo', 'generalinfo', ]);
    }

    // if(this.schoolCreateMode === SchoolCreate.ADD) {
    //   // this.router.navigate(['/school', 'schoolinfo', 'generalinfo'], {state: {type: this.schoolCreateMode}});
    // }

  }

  initializeDropdownValues() {
    this.commonLOV.getLovByName('Female Toilet Type').pipe(takeUntil(this.destroySubject$)).subscribe((res)=>{
      this.femaleToiletTypeList=res;
    });
    this.commonLOV.getLovByName('Male Toilet Type').pipe(takeUntil(this.destroySubject$)).subscribe((res)=>{
      this.maleToiletTypeList=res;
    });
    this.commonLOV.getLovByName('Common Toilet Type').pipe(takeUntil(this.destroySubject$)).subscribe((res)=>{
      this.commonToiletTypeList=res;
    });
    this.commonLOV.getLovByName('Female Toilet Accessibility').pipe(takeUntil(this.destroySubject$)).subscribe((res)=>{
      this.femaleToiletAccessibilityList=res;
    });
    this.commonLOV.getLovByName('Male Toilet Accessibility').pipe(takeUntil(this.destroySubject$)).subscribe((res)=>{
      this.maleToiletAccessibilityList=res;
    });
    this.commonLOV.getLovByName('Common Toilet Accessibility').pipe(takeUntil(this.destroySubject$)).subscribe((res)=>{
      this.commonToiletAccessibilityList=res;
    });
  }

  editWashInfo() {
    this.formActionButtonTitle = "update";
    this.initializeDropdownValues();
    this.imageCropperService.enableUpload({module:this.moduleIdentifier.SCHOOL,upload:true,mode:this.schoolCreate.EDIT});
    this.schoolCreateMode = this.schoolCreate.EDIT;
    // this.schoolService.setSchoolCreateMode(this.schoolCreateMode);
  }
  cancelEdit() {
    this.imageCropperService.enableUpload({module:this.moduleIdentifier.SCHOOL,upload:true,mode:this.schoolCreate.VIEW});
    this.imageCropperService.cancelImage("school");
    if(JSON.stringify(this.schoolAddViewModel) !== this.cloneSchool){
      this.schoolAddViewModel = JSON.parse(this.cloneSchool);
      // this.schoolDetailsForViewAndEdit=this.schoolAddViewModel;
      // this.schoolService.sendDetails(JSON.parse(this.cloneSchool));
    }
    this.schoolCreateMode = this.schoolCreate.VIEW;
    // this.schoolService.setSchoolCreateMode(this.schoolCreateMode);

  }

  submit() {
    this.customValid=false;
    this.currentForm.form.markAllAsTouched();
    this.defaultValuesService.customFieldsCheckParentComp.next(true);
    if (this.currentForm.form.valid) {
      if (this.schoolAddViewModel.schoolMaster.fieldsCategory !== null) {
        this.modifyCustomFields();
      }
      if(this.customValid){
      this.schoolService.UpdateSchool(this.schoolAddViewModel).pipe(takeUntil(this.destroySubject$)).subscribe(
        data => {
          if (data){
           if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
              this.snackbar.open( data._message, '', {
                duration: 10000
              });
            } else {
              this.snackbar.open(data._message, '', {
                duration: 10000
              });
              this.schoolCreateMode = this.schoolCreate.VIEW;
              this.schoolService.setSchoolCloneImage(data.schoolMaster.schoolDetail[0].schoolLogo);
              data.schoolMaster.schoolDetail[0].schoolLogo = null;
              this.schoolAddViewModel = data;
              // this.cloneSchool = JSON.stringify(this.schoolDetailsForViewAndEdit);
              // this.schoolService.setSchoolCreateMode(this.schoolCreateMode); 
              this.imageCropperService.enableUpload({module:this.moduleIdentifier.SCHOOL,upload:true,mode:this.schoolCreate.VIEW});
            }
          }else{
            this.snackbar.open(`Wash Info Updation failed` + this.defaultValuesService.getHttpError(), '', {
              duration: 10000
            });
          }
        }
      );
    }
    }
  }

  modifyCustomFields(){
    this.schoolAddViewModel.selectedCategoryId = this.schoolAddViewModel.schoolMaster.fieldsCategory[1].categoryId;
    for (const schoolCustomField of this.schoolAddViewModel.schoolMaster.fieldsCategory[1].customFields) {
          if (schoolCustomField.type === 'Multiple SelectBox' && this.schoolService.getSchoolMultiselectValue() !== undefined) {
            schoolCustomField.customFieldsValue[0].customFieldValue = this.schoolService.getSchoolMultiselectValue().toString().replaceAll(',', '|');
          }
        }
  }

  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }

}
