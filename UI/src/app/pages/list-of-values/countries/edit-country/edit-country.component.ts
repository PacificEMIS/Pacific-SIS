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

import { Component, OnInit,Inject } from '@angular/core';
import { MatDialogRef,MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder,FormGroup, Validators } from '@angular/forms';
import icClose from '@iconify/icons-ic/twotone-close';
import { fadeInUp400ms } from '../../../../../@vex/animations/fade-in-up.animation';
import { stagger60ms } from '../../../../../@vex/animations/stagger.animation';
import {CountryAddModel} from '../../../../models/country.model';
import {CommonService} from '../../../../services/common.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'vex-edit-country',
  templateUrl: './edit-country.component.html',
  styleUrls: ['./edit-country.component.scss'],
  animations: [
    stagger60ms,
    fadeInUp400ms
  ]
})

export class EditCountryComponent implements OnInit {
  icClose = icClose;
  countryAddModel = new CountryAddModel();
  form: FormGroup;
  countryModalTitle:string;
  countryModalActionTitle:string;
  constructor(
    private dialogRef: MatDialogRef<EditCountryComponent>,
    @Inject(MAT_DIALOG_DATA) public data:any,
    private snackbar:MatSnackBar,
    private commonService:CommonService,
    fb:FormBuilder
    ) {
      this.form=fb.group({
        id:[0],
        name: ['', Validators.required],
        countryCode: ['']  
      })
      if(data==null){
        this.countryModalTitle="addCountry";
        this.countryModalActionTitle="submit";
      }  
      else{        
        this.countryModalTitle="editCountry";
        this.countryModalActionTitle="update";
        this.form.controls.id.patchValue(data.id)
        this.form.controls.name.patchValue(data.name)
        this.form.controls.countryCode.patchValue(data.countryCode)
      }
     }


  ngOnInit(): void {}
  
  submit() {    
    if (this.form.valid) {   
      if(this.form.controls.id.value==0){
        this.countryAddModel.country.name=this.form.controls.name.value;
        this.countryAddModel.country.countryCode=this.form.controls.countryCode.value;
        this.commonService.AddCountry(this.countryAddModel).subscribe(data => {
          if (typeof (data) == 'undefined') {
            this.snackbar.open('Country Submission failed. ' + sessionStorage.getItem("httpError"), '', {
              duration: 10000
            });
          }
          else {
           if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
              this.snackbar.open('' + data._message, '', {
                duration: 10000
              });
            } else {
              this.snackbar.open(''+ data._message, '', {
                duration: 10000
              });              
              this.dialogRef.close(true);
            }
          }
  
        })     
      }else{
        this.countryAddModel.country.id=this.form.controls.id.value;
        this.countryAddModel.country.name=this.form.controls.name.value;
        this.countryAddModel.country.countryCode=this.form.controls.countryCode.value;
        this.commonService.UpdateCountry(this.countryAddModel).subscribe(data => {
          if (typeof (data) == 'undefined') {
            this.snackbar.open('Country Updation failed. ' + sessionStorage.getItem("httpError"), '', {
              duration: 10000
            });
          }
          else {
           if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
              this.snackbar.open('' + data._message, '', {
                duration: 10000
              });
            } else {
              this.snackbar.open('' + data._message, '', {
                duration: 10000
              });              
              this.dialogRef.close(true);
            }
          }
  
        })
        
      }  
      
    }
      
  }

}
