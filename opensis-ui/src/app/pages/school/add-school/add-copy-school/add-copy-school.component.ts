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

import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import icClose from '@iconify/icons-ic/twotone-close';
import { TranslateService } from '@ngx-translate/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { SchoolService } from 'src/app/services/school.service';
import { CopySchoolModel } from 'src/app/models/school-master.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { LoaderService } from 'src/app/services/loader.service';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { CommonService } from 'src/app/services/common.service';

@Component({
  selector: 'vex-add-copy-school',
  templateUrl: './add-copy-school.component.html',
  styleUrls: ['./add-copy-school.component.scss']
})
export class AddCopySchoolComponent implements OnInit, OnDestroy {

  icClose = icClose;
  schoolFieldList = [{ name: "periods", value: "periods" }, { name: "markingPeriod", value: "markingPeriods" }, { name: "calendar", value: "calendar" }, { name: "sections", value: "sections" }, { name: "rooms", value: "rooms" }, { name: "gradeLevels", value: "gradeLevels" }, { name: "schoolFields", value: "schoolFields" }];
  studentFieldList = [{ name: "studentFields", value: "studentFields" }, { name: "enrollmentCodes", value: "enrollmentCodes" }];
  staffFieldList = [{ name: "staffFields", value: "staffFields" }];
  courseFieldList = [{ name: "subjects", value: "subjets" }, { name: "programs", value: "programs" }, { name: "course", value: "course" }];
  attendanceFieldList = [{ name: "attendanceCodes", value: "attendanceCode" }];
  gradeFieldList = [{ name: "reportCardGrades", value: "reportCardGrades" }, { name: "reportCardComments", value: "reportCardComments" }, { name: "standardGrades", value: "standardGrades" }, { name: "effortGrades", value: "effortGrades" }, { name: "honorRollSetup", value: "honorRollSetup" }];
  adminFieldList = [{ name: "profilesPermissions", value: "profilePermission" }];
  listOfValuesFieldList = [{ name: "schoolLevel", value: "schoolLevel" }, { name: "schoolClassification", value: "schoolClassification" }, { name: "femaleToiletType", value: "femaleToiletType" }, { name: "femaleToiletAccessibility", value: "femaleToiletAccessibility" }, { name: "maleToiletType", value: "maleToiletType" }, { name: "maleToiletAccessibility", value: "maleToiletAccessibility" }, { name: "commonToiletType", value: "commonToiletType" }, { name: "commonToiletAccessibility", value: "commonToiletAccessibility" }, { name: "race", value: "race" }, { name: "ethnicity", value: "ethnicity" }];
  copySchoolModel: CopySchoolModel = new CopySchoolModel();
  fromSchoolName = this.data.fromSchoolName;
  destroySubject$: Subject<void> = new Subject();
  loading: boolean;
  allToggleValidationCheck = true;

  constructor(
    public translateService: TranslateService,
    private loaderService: LoaderService,
    private schoolService: SchoolService,
    private defaultValuesService: DefaultValuesService,
    private snackbar: MatSnackBar,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private commonService: CommonService,
    private dialogRef: MatDialogRef<AddCopySchoolComponent>
  ) {
    //translateService.use('en');
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });
  }

  ngOnInit(): void {
  }

  // This onChange method is used for subjects and programs toggle on/off logic based on course.
  onChange(fieldName, event) {
    if (event.checked && fieldName === 'course') {
      this.copySchoolModel.subjets = true;
      this.copySchoolModel.programs = true;
    } else if (!event.checked && (fieldName === 'programs' || fieldName === 'subjets')) {
      this.copySchoolModel.course = false;
    }
  }

  // This checkValidation method is used to validate at least one toggle switch is turned on.
  checkValidation() {
    let booleanValues = [];
    Object.values(this.copySchoolModel).map((element) => {
      if (element === true || element === false) {
        booleanValues.push(element);
      }
    });
    for (let item of booleanValues) {
      if (item) {
        this.allToggleValidationCheck = false;
        break;
      } else {
        this.allToggleValidationCheck = true;
      }
    }
  }

  /* This submit method is used for call copySchool API and after successful API call
  the copy school modal will be close and pass the new school name and new school id to the source. */
  submit() {
    this.checkValidation();
    if (!this.allToggleValidationCheck) {
      if (this.copySchoolModel.schoolMaster.schoolName?.trim().length > 0) {
        this.copySchoolModel.fromSchoolId = this.data.fromSchoolId;
        this.schoolService.copySchool(this.copySchoolModel).subscribe(
          (res: CopySchoolModel) => {
            if (res) {
            if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
                this.snackbar.open(res._message, '', {
                  duration: 10000
                });
              }
              else {
                this.snackbar.open(res._message, '', {
                  duration: 10000
                });
                this.schoolService.changeMessage(true);
                let newSchoolName = res.schoolMaster.schoolName;
                let newSchoolId = res.schoolMaster.schoolId;
                this.dialogRef.close({ schoolName: newSchoolName, schoolId: newSchoolId });
              }
            }
            else {
              this.snackbar.open(sessionStorage.getItem("httpError"), '', {
                duration: 10000
              });
            }
          });
      }
    } else {
      this.snackbar.open(this.defaultValuesService.translateKey('mustHaveAtLeastOneItemSelectedToProceedWithCopySchool'), '', {
        duration: 10000
      });
    }
  }

  // For destroy the isLoading subject.
  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }
}

