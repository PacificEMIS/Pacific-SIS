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

import { Component, Inject, OnInit, Optional } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import icClose from '@iconify/icons-ic/twotone-close';
import { fadeInUp400ms } from '../../../../../../@vex/animations/fade-in-up.animation';
import { stagger60ms } from '../../../../../../@vex/animations/stagger.animation';
import { ParentInfoService } from '../../../../../services/parent-info.service';
import { AddParentInfoModel } from '../../../../../models/parent-info.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CountryModel } from '../../../../../models/country.model';
import { CommonService } from '../../../../../services/common.service';
import { takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { SchoolAddViewModel } from '../../../../../models/school-master.model';
import { DefaultValuesService } from '../../../../../common/default-values.service';
@Component({
  selector: 'vex-view-sibling',
  templateUrl: './view-sibling.component.html',
  styleUrls: ['./view-sibling.component.scss'],
  animations: [
    stagger60ms,
    fadeInUp400ms
  ]
})
export class ViewSiblingComponent implements OnInit {
  icClose = icClose;
  address: string;
  schoolName;
  getStudentForView = [];
  gradeLevelTitle;
  countryName;
  mapUrl: string;
  destroySubject$: Subject<void> = new Subject();
  getCountryModel: CountryModel = new CountryModel();
  addParentInfoModel: AddParentInfoModel = new AddParentInfoModel();
  schoolAddViewModel: SchoolAddViewModel = new SchoolAddViewModel();
  constructor(private dialogRef: MatDialogRef<ViewSiblingComponent>,
              private snackbar: MatSnackBar,
              private parentInfoService: ParentInfoService,
              private defaultValuesService: DefaultValuesService,
              private commonService: CommonService,
              @Optional() @Inject(MAT_DIALOG_DATA) public data: any) {}


  ngOnInit(): void {
    this.getAllCountry();
    if (this.data.flag === 'Parent'){
      this.data.siblingDetails = this.data.studentDetails;
      this.gradeLevelTitle = this.data.studentDetails.gradeLevelTitle;
      this.schoolName = this.data.studentDetails.schoolName;
      if (this.data.studentDetails.address !== ''){
          this.address = this.data.studentDetails.address;
        }else{
          this.address = null;
        }
    }else{
      this.schoolName = this.data.siblingDetails.schoolMaster.schoolName;
      this.gradeLevelTitle = this.data.siblingDetails.studentEnrollment[0].gradeLevelTitle;
      if (this.data.siblingDetails.homeAddressLineOne !== null){
        this.address = this.data.siblingDetails.homeAddressLineOne;
        if (this.data.siblingDetails.homeAddressLineTwo !== null){
          this.address = this.address + ' ' + this.data.siblingDetails.homeAddressLineTwo;
          if (this.data.siblingDetails.homeAddressCity !== null){
            this.address = this.address + ' ' + this.data.siblingDetails.homeAddressCity;
            if (this.data.siblingDetails.homeAddressState !== null) {
              this.address = this.address + ' ' + this.data.siblingDetails.homeAddressState;
              if (this.data.siblingDetails.homeAddressZip !== null) {
                this.address = this.address + ' ' + this.data.siblingDetails.homeAddressZip;
              }
            }
          }
        }
      }
      else {
        this.address = null;
      }
    }
  }
  getAllCountry() {
    this.commonService.GetAllCountry(this.getCountryModel).pipe(takeUntil(this.destroySubject$)).subscribe(data => {
      if (data){
       if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
          // this.countryListArr = [];
          if(!data.tableCountry){
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
          }
        } else {
          this.findCountryNameByIdOnViewMode(data.tableCountry);
        }
      }
      else{
      }
    });
  }

  findCountryNameByIdOnViewMode(countryListArr) {
    const index = countryListArr.findIndex((x) => {
      return x.id === +this.data.siblingDetails.homeAddressCountry ;
    });
    if (countryListArr[index]?.name !== undefined){
      this.countryName = countryListArr[index]?.name;
    }
    else{
      this.countryName = '';
    }
  }

  showOnGoogleMap(){
    if (this.address !== null){
      const address = this.address;
      this.mapUrl = `https://maps.google.com/?q=${address}`;
      window.open(this.mapUrl, '_blank');
    }else{
      this.snackbar.open(this.defaultValuesService.translateKey('invalidAddress'), this.defaultValuesService.translateKey('ok'), {
        duration: 5000
      });
    }
  }

}
