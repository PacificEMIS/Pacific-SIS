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

import { Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { SchoolPreferenceAddViewModel } from 'src/app/models/school-preference.model';
import { CommonService } from 'src/app/services/common.service';
import { SchoolPreferenceService } from 'src/app/services/school-preference.service';
import { PageRolesPermission } from '../../../common/page-roles-permissions.service';
import { Permissions } from '../../../models/roll-based-access.model';

@Component({
  selector: 'vex-preference',
  templateUrl: './preference.component.html',
  styleUrls: ['./preference.component.scss']
})
export class PreferenceComponent implements OnInit {
  schoolPreferenceAddViewModel: SchoolPreferenceAddViewModel = new SchoolPreferenceAddViewModel(); 
  @ViewChild('f') currentForm: NgForm;
  f: NgForm;
  submitTitle='submit';
  permissions: Permissions
  constructor(public translateService: TranslateService,
    private schoolPreferenceService: SchoolPreferenceService,
    private snackbar: MatSnackBar,
    private pageRolePermissions: PageRolesPermission,
    private defaultValuesService: DefaultValuesService,
    private commonService: CommonService,
  ) {
    translateService.use("en");

  }

  ngOnInit(): void {
    this.permissions = this.pageRolePermissions.checkPageRolePermission('/school/settings/school-settings/preference')
    this.viewSchoolPreference();
  }

  viewSchoolPreference() {
    this.schoolPreferenceService.viewPreference(this.schoolPreferenceAddViewModel).subscribe(data => {
      if(data){
       if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
          this.snackbar.open(data._message, '', {
            duration: 10000
          });
        } else {
          this.schoolPreferenceAddViewModel= data;
          this.submitTitle='update';
        }
      }
      else{
        this.snackbar.open(data._message, '', {
          duration: 10000
        });
      }
      
    });
  }

  submit(){
    this.currentForm.form.markAllAsTouched();
    if(this.currentForm.form.valid){
      // if(this.schoolPreferenceAddViewModel.schoolPreference.fullDayMinutes>this.schoolPreferenceAddViewModel.schoolPreference.halfDayMinutes ){
        this.schoolPreferenceService.addUpdateSchoolPreference(this.schoolPreferenceAddViewModel).subscribe(data => {
          if(data){
           if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
              this.snackbar.open(data._message, '', {
                duration: 10000
              });
            } else {
              this.schoolPreferenceAddViewModel= data;
              this.snackbar.open(data._message, '', {
                duration: 10000
              });
            }
          }
          else{
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
          }
          
        });
      }
      // else{
      //   this.snackbar.open( this.defaultValuesService.translateKey('fullDayMinuitesnotlessthanHalfDayMinutes'), '', {
      //     duration: 10000
      //   });
      // }
     
    // }
    else{
      this.snackbar.open("Plese Enter valid data", '', {
        duration: 10000
      });
    }
    
  }

}
