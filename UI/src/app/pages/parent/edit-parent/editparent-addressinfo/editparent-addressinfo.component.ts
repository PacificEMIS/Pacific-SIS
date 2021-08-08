import { AfterViewInit } from '@angular/core';
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

import { Component, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, NgForm, Validators } from '@angular/forms';
import { fadeInUp400ms } from '../../../../../@vex/animations/fade-in-up.animation';
import { stagger60ms } from '../../../../../@vex/animations/stagger.animation';
import { fadeInRight400ms } from '../../../../../@vex/animations/fade-in-right.animation';
import { TranslateService } from '@ngx-translate/core';
import icAdd from '@iconify/icons-ic/baseline-add';
import icEdit from '@iconify/icons-ic/edit';
import icClear from '@iconify/icons-ic/baseline-clear';
import { SchoolCreate } from '../../../../enums/school-create.enum';
import { AddParentInfoModel } from '../../../../models/parent-info.model';
import { CountryModel } from '../../../../models/country.model';
import { CommonService } from '../../../../services/common.service';
import { ParentInfoService } from '../../../../services/parent-info.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ImageCropperService } from '../../../../services/image-cropper.service';
import { ModuleIdentifier } from '../../../../enums/module-identifier.enum';
import { Permissions, RolePermissionListViewModel, RolePermissionViewModel } from '../../../../models/roll-based-access.model';
import { CryptoService } from '../../../../services/Crypto.service';
import { ReplaySubject, Subject } from 'rxjs';
import { MatSelect } from '@angular/material/select';
import { take, takeUntil } from 'rxjs/operators';
import { PageRolesPermission } from '../../../../common/page-roles-permissions.service';

@Component({
  selector: 'vex-editparent-addressinfo',
  templateUrl: './editparent-addressinfo.component.html',
  styleUrls: ['./editparent-addressinfo.component.scss'],
  animations: [
    stagger60ms,
    fadeInUp400ms,
    fadeInRight400ms
  ]
})
export class EditparentAddressinfoComponent implements OnInit, AfterViewInit, OnDestroy {

  icAdd = icAdd;
  icClear = icClear;
  icEdit = icEdit;
  parentDetailsForViewAndEdit;
  @ViewChild('f') currentForm: NgForm;
  f: NgForm;
  parentCreate = SchoolCreate;
  parentCreateMode: SchoolCreate;
  addParentInfoModel: AddParentInfoModel = new AddParentInfoModel();
  duplicateAddParentInfoModel: AddParentInfoModel = new AddParentInfoModel();
  countryModel: CountryModel = new CountryModel();
  countryListArr = [];
  countryName = '-';
  country = '-';
  data;
  parentInfo;
  moduleIdentifier = ModuleIdentifier;
  mapUrl: string;
  countryCtrl: FormControl = new FormControl();
  countryFilterCtrl: FormControl = new FormControl();
  public filteredCountry: ReplaySubject<any> = new ReplaySubject<any>(1);
  @ViewChild('singleSelect') singleSelect: MatSelect;
  protected _onDestroy = new Subject<void>();
  permissions: Permissions;
  constructor(private fb: FormBuilder,
              private snackbar: MatSnackBar,
              public translateService: TranslateService,
              private commonService: CommonService,
              private parentInfoService: ParentInfoService,
              private imageCropperService: ImageCropperService,
              private pageRolePermissions: PageRolesPermission,
              private cryptoService: CryptoService) {
    //translateService.use('en');
  }
  protected setInitialValue() {
    this.filteredCountry
    .pipe(take(1), takeUntil(this._onDestroy))
    .subscribe(() => {
      this.singleSelect.compareWith = (a, b) => a && b && a.name === b.name;
    });
  }

  ngOnInit(): void {
    this.parentInfoService.parentDetailsForViewedAndEdited.subscribe((res)=>{
      if(res){
        this.parentDetailsForViewAndEdit = res;
        this.addParentInfoModel = this.parentDetailsForViewAndEdit;
        this.addParentInfoModel.parentInfo.parentAddress[0].country = +this.parentDetailsForViewAndEdit?.parentInfo?.parentAddress[0].country; 
        this.duplicateAddParentInfoModel = JSON.parse(JSON.stringify(this.parentDetailsForViewAndEdit));
      }
    });

    this.parentInfoService.parentCreatedMode.subscribe((res)=>{
      this.parentCreateMode = res;
    });
    this.permissions = this.pageRolePermissions.checkPageRolePermission()
    this.imageCropperService.enableUpload({module: this.moduleIdentifier.PARENT, upload: true, mode: this.parentCreate.VIEW});
    this.parentCreateMode = this.parentCreate.VIEW;
    this.parentInfo = {};

    this.getAllCountry();
  }
  ngAfterViewInit(): void {
    this.countryValueChange();
  }
  countryValueChange(){
    this.countryFilterCtrl.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe((res) => {
        this.filterNationalitis();
      });
  }

  cancelEdit() {
    this.addParentInfoModel = JSON.parse(JSON.stringify(this.duplicateAddParentInfoModel));
    this.parentCreateMode = this.parentCreate.VIEW;
  }


  editAddressContactInfo() {
    this.parentCreateMode = this.parentCreate.EDIT;
    this.imageCropperService.enableUpload({ module: this.moduleIdentifier.PARENT, upload: true, mode: this.parentCreate.EDIT });
    this.countryListArr.map((val) => {
      if (val.name === this.countryName) {
        this.addParentInfoModel.parentInfo.parentAddress[0].country = val.id;
      }
      let countryInNumber = +this.addParentInfoModel.parentInfo.parentAddress[0].country;
      if (val.id === countryInNumber) {
        this.countryName = val.name;
      }
    });
  }

  getAllCountry() {
    this.commonService.GetAllCountry(this.countryModel).subscribe(data => {
      if (typeof (data) === 'undefined') {
        this.countryListArr = [];
      }
      else {
       if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
          if (data.tableCountry) {
            this.countryListArr = [];
          } else {
            this.countryListArr = [];
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
          }
        } else {
          this.countryCtrl.setValue(data.tableCountry?.sort((a, b) => a.name < b.name ? -1 : 1));
          this.filteredCountry.next(data.tableCountry?.slice());
          this.countryListArr = data.tableCountry?.sort((a, b) => a.name < b.name ? -1 : 1 )
          if (this.parentCreateMode === this.parentCreate.VIEW) {
            this.viewCountryName();
          }

          this.countryListArr.map((val) => {
            let country = + this.addParentInfoModel.parentInfo.parentAddress[0].country;
            if (val.id === country) {

              this.country = val.name;
            }

          })


        }
      }
    })
  }


  viewCountryName() {
    this.countryListArr.map((val) => {
      let countryInNumber = + this.addParentInfoModel.parentInfo.parentAddress[0].country;      
      if (val.id === countryInNumber) {
        this.countryName = val.name;

      }

    })
  }

  submit() {
    this.parentInfoService.updateParentInfo(this.addParentInfoModel).subscribe(data => {
      if (typeof (data) == 'undefined') {
        this.snackbar.open('Address Updation failed. ' + sessionStorage.getItem("httpError"), '', {
          duration: 10000
        });
      }
      else {
       if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
          this.snackbar.open( data._message, '', {
            duration: 10000
          });
        } else {
          this.snackbar.open(data._message, '', {
            duration: 10000
          });
          this.addParentInfoModel.parentInfo.parentAddress[0].country = +data.parentInfo.parentAddress[0].country;
          this.viewCountryName();
          this.parentCreateMode = this.parentCreate.VIEW;
          this.parentInfoService.changePageMode(this.parentCreateMode);
          this.imageCropperService.enableUpload({module:this.moduleIdentifier.PARENT,upload:true,mode:this.parentCreate.VIEW});
          this.duplicateAddParentInfoModel.parentInfo.parentAddress[0] = data.parentInfo.parentAddress[0];
          this.duplicateAddParentInfoModel.parentInfo.parentAddress[0].country = +data.parentInfo.parentAddress[0].country;
        }
      }

    });
  }
  showOnGoogleMap(){
    const stAdd1 = this.addParentInfoModel.parentInfo.parentAddress[0].addressLineOne;
    const stAdd2 = this.addParentInfoModel.parentInfo.parentAddress[0].addressLineTwo;
    const city = this.addParentInfoModel.parentInfo.parentAddress[0].city;
    const country = this.countryName;
    const state = this.addParentInfoModel.parentInfo.parentAddress[0].state;
    const zip = this.addParentInfoModel.parentInfo.parentAddress[0].zip;
    if (stAdd1 && country && city && zip){
      this.mapUrl = `https://maps.google.com/?q=${stAdd1},${stAdd2},${city},${state},${zip},${country}`;
      window.open(this.mapUrl, '_blank');
    }else{
      this.snackbar.open('Invalid parent address', 'Ok', {
        duration: 5000
      });
    }
  }
  filterNationalitis(){
    if (!this.countryListArr) {
      return;
    }
    let search = this.countryFilterCtrl.value;
    if (!search) {
      this.filteredCountry.next(this.countryListArr.slice());
      return;
    }
    else {
      search = search.toLowerCase();
    }
    this.filteredCountry.next(
      this.countryListArr.filter(country => country.name.toLowerCase().indexOf(search) > -1)
    );
  }

  ngOnDestroy(){
  }
}
function ngAfterViewInit() {
  throw new Error('Function not implemented.');
}

