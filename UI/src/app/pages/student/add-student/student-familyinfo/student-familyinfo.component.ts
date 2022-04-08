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

import { Component, OnInit , Input, AfterViewInit} from '@angular/core';
import { fadeInUp400ms } from '../../../../../@vex/animations/fade-in-up.animation';
import { stagger60ms } from '../../../../../@vex/animations/stagger.animation';
import { fadeInRight400ms } from '../../../../../@vex/animations/fade-in-right.animation';
import { TranslateService } from '@ngx-translate/core';
import icEdit from '@iconify/icons-ic/twotone-edit';
import icDelete from '@iconify/icons-ic/twotone-delete';
import icAdd from '@iconify/icons-ic/baseline-add';
import { SchoolCreate } from '../../../../enums/school-create.enum';
import { StudentService } from '../../../../services/student.service';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { CommonService } from 'src/app/services/common.service';
import { CountryModel } from 'src/app/models/country.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { StudentInfoModel } from 'src/app/models/student-info.model';
import { CommonLOV } from 'src/app/pages/shared-module/lov/common-lov';
import { takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { StudentViewSibling } from 'src/app/models/student.model';
import { ParentInfoService } from 'src/app/services/parent-info.service';
import { GetAllParentInfoModel } from 'src/app/models/parent-info.model';
import { GradeLevelService } from 'src/app/services/grade-level.service';
import { GetAllGradeLevelsModel } from 'src/app/models/grade-level.model';
@Component({
  selector: 'vex-student-familyinfo',
  templateUrl: './student-familyinfo.component.html',
  styleUrls: ['./student-familyinfo.component.scss'],
  animations: [
    stagger60ms,
    fadeInUp400ms,
    fadeInRight400ms
  ]
})
export class StudentFamilyinfoComponent implements OnInit {
  @Input() studentCreateMode: SchoolCreate;
  @Input() studentDetailsForViewAndEdit;
  icEdit = icEdit;
  icDelete = icDelete;
  icAdd = icAdd;
  currentTab: string;
  countryList;
  studentDetailsForViewAndEditData;
  countryModel: CountryModel = new CountryModel();
  multipleData: StudentInfoModel = new StudentInfoModel()
  destroySubject$: Subject<void> = new Subject();
  studentViewSibling: StudentViewSibling = new StudentViewSibling();
  getAllParentInfoModel: GetAllParentInfoModel =  new GetAllParentInfoModel();
  parentListArray: any;
  getAllGradeLevelsModel: GetAllGradeLevelsModel = new GetAllGradeLevelsModel();
  gradeLevelArr: any;

  constructor(
    public translateService: TranslateService,
    private studentService: StudentService,
    private defaultValuesService: DefaultValuesService,
    private commonService: CommonService,
    private snackbar: MatSnackBar,
    private commonLOV: CommonLOV,
    private parentInfoService: ParentInfoService,
    private gradeLevelService: GradeLevelService,
  ) {
      this.defaultValuesService.checkAcademicYear() && !this.studentService.getStudentId() ? this.studentService.redirectToGeneralInfo() : !this.defaultValuesService.checkAcademicYear() && !this.studentService.getStudentId() ? this.studentService.redirectToStudentList() : '';
  }

  ngOnInit(): void {
    this.studentService.studentCreatedMode.subscribe((res)=>{
      this.studentCreateMode = res;
    });
    this.studentService.studentDetailsForViewedAndEdited.subscribe((res)=>{
      this.studentDetailsForViewAndEdit = res;
    });
    this.studentDetailsForViewAndEditData = this.studentDetailsForViewAndEdit;
    this.currentTab = 'contacts';
    this.viewParentListForStudent();
    this.getAllCountry();
    this.callLOVs();
    this.getGradeLevel();
  }

  getAllCountry() {
    this.commonService.GetAllCountry(this.countryModel).subscribe(data => {
      if (data) {
        if (data._failure) {
          
          this.countryList = [];
          if (!data.tableCountry) {
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
          }
        } else {
          this.countryList = data.tableCountry?.sort((a, b) => a.name < b.name ? -1 : 1);
          this.multipleData.countryList = this.countryList;
        }
      }
      else {
        this.countryList = [];
      }
    });
  }

  getGradeLevel() {
    this.gradeLevelService.getAllGradeLevels(this.getAllGradeLevelsModel).subscribe((res) => {
      if (res) {
        if (res._failure) {
          
          this.snackbar.open(res._message, '', {
            duration: 10000
          });
        } else {
          this.gradeLevelArr = res.tableGradelevelList;
        }
      } else {
        this.snackbar.open(this.defaultValuesService.translateKey('gradeLevelInformationfailed')
          + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    });
  }

  callLOVs() {
    this.commonLOV.getLovByName('Relationship').subscribe((res:any) => {
      this.multipleData.RelationshipLOV = res;
    });
    this.commonLOV.getLovByName('Salutation').subscribe((res:any) => {
      this.multipleData.SalutationLOV = res;
    });
    this.commonLOV.getLovByName('Suffix').subscribe((res:any) => {
      this.multipleData.SuffixLOV = res;
    });
  }

  changeTab(status){
    if(status === 'siblingInfo') {
      if(!this.studentViewSibling.studentMaster) {
        this.getAllSiblings();
      }
    } else {
      if(!this.parentListArray) {
        this.viewParentListForStudent();
      }
    }
    this.currentTab = status;
  }

  getAllSiblings() {
    this.studentViewSibling.studentId = this.studentService.getStudentId();
    this.studentService.viewSibling(this.studentViewSibling).subscribe((res) => {
      if (res) {
        if (res._failure) {
          
          this.studentViewSibling.studentMaster = [];
          if (!res.studentMaster) {
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
        }
        else {
          this.studentViewSibling.studentMaster = res.studentMaster;
        }
      } else {
        this.snackbar.open(this.defaultValuesService.translateKey('siblingsFailedToFetch') + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    });
  }

  viewParentListForStudent() {
    this.getAllParentInfoModel.studentId = this.studentDetailsForViewAndEditData.studentMaster.studentId;
    this.parentInfoService.viewParentListForStudent(this.getAllParentInfoModel).subscribe(
      data => {
        if (data) {
          if (data._failure) {
            
            if (!data.parentInfoListForView) {
              this.snackbar.open(data._message, '', {
                duration: 10000
              });
            }
          }
          else {
            this.parentListArray = data.parentInfoListForView;
          }
        }
        else {
          this.snackbar.open(this.defaultValuesService.translateKey('parentInformationFailed') + this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
      });
  }

  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }
}
