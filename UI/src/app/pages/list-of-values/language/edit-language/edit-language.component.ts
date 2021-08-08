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
import {LanguageAddModel} from '../../../../models/language.model';
import {CommonService} from '../../../../services/common.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ValidationService } from '../../../shared/validation.service';
import { DefaultValuesService } from '../../../../common/default-values.service';

@Component({
  selector: 'vex-edit-language',
  templateUrl: './edit-language.component.html',
  styleUrls: ['./edit-language.component.scss'],
  animations: [
    stagger60ms,
    fadeInUp400ms
  ]
})

export class EditLanguageComponent implements OnInit {
  icClose = icClose;
  languageModel = new LanguageAddModel();
  form: FormGroup;
  languageModalTitle="addLanguage";
  languageModalActionTitle="submit";
  constructor(
    private dialogRef: MatDialogRef<EditLanguageComponent>,
    @Inject(MAT_DIALOG_DATA) public data:any,
    private snackbar:MatSnackBar,
    private commonService:CommonService,
    private defaultValueService: DefaultValuesService,
    fb:FormBuilder
    ) {
      this.form=fb.group({
        langId:[0],
        locale: ['', ValidationService.noWhitespaceValidator],
        languageCode: ['',[ValidationService.noWhitespaceValidator,ValidationService.lowerCaseValidator]]     
      })
      if(data==null){
        this.languageModalTitle="addLanguage";
        this.languageModalActionTitle="submit";
      }  
      else{        
        this.languageModalTitle="editLanguage";
        this.languageModalActionTitle="update";
        this.form.controls.langId.patchValue(data.langId)
        this.form.controls.locale.patchValue(data.locale)
        this.form.controls.languageCode.patchValue(data.languageCode)
      }
     }

  ngOnInit(): void { }

  submit() {    
    this.form.markAllAsTouched()
    if (this.form.valid) {   
      if(this.form.controls.langId.value==0){
        this.languageModel.language.locale=this.form.controls.locale.value;
        this.languageModel.language.languageCode=this.form.controls.languageCode.value;
        this.commonService.AddLanguage(this.languageModel).subscribe(data => {
          if (typeof (data) == 'undefined') {
            this.snackbar.open('Language Submission failed. ' + sessionStorage.getItem("httpError"), '', {
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
        this.languageModel.language.langId=this.form.controls.langId.value;
        this.languageModel.language.locale=this.form.controls.locale.value;
        this.languageModel.language.languageCode=this.form.controls.languageCode.value;
        this.commonService.UpdateLanguage(this.languageModel).subscribe(data => {
          if (typeof (data) == 'undefined') {
            this.snackbar.open('Language Updation failed. ' + sessionStorage.getItem("httpError"), '', {
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
