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

import { Component, Inject, OnInit, Optional } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { MatSnackBar } from "@angular/material/snack-bar";
import icClose from "@iconify/icons-ic/twotone-close";
import { CommonService } from "src/app/services/common.service";
import { DefaultValuesService } from "../../../../common/default-values.service";
import { AddAssignmentTypeModel } from "../../../../models/staff-portal-assignment.model";
import { DasboardService } from "../../../../services/dasboard.service";
import { StaffPortalAssignmentService } from "../../../../services/staff-portal-assignment.service";

@Component({
  selector: "vex-add-assignment",
  templateUrl: "./add-assignment.component.html",
  styleUrls: ["./add-assignment.component.scss"],
})
export class AddAssignmentComponent implements OnInit {
  icClose = icClose;

  form: FormGroup;
  courseSectionId: number;
  editMode: boolean = false;
  editDetails;
  constructor(
    private dialogRef: MatDialogRef<AddAssignmentComponent>,
    private assignmentService: StaffPortalAssignmentService,
    private snackbar: MatSnackBar,
    private dashboardService: DasboardService,
    private defaultValueService: DefaultValuesService,
    @Optional() @Inject(MAT_DIALOG_DATA) public data: any,
    private fb: FormBuilder,
    private commonService: CommonService,
  ) {
    this.dashboardService.selectedCourseSectionDetails.subscribe((res) => {
      if (res) {
        this.courseSectionId = +res.courseSectionId;
      }
    });
  }

  ngOnInit(): void {
    this.form = this.fb.group({
      title: ["", Validators.required],
      weightage: [null],
    });
    if (this.data.editMode) {
      this.editMode = this.data.editMode;
      this.editDetails = this.data.editDetails;
      this.patchFormValue();
    }
  }

  patchFormValue() {
    this.form.patchValue({
      title: this.editDetails.title,
      weightage: this.editDetails.weightage,
    });
  }

  checkFormValidation() {
    this.form.markAllAsTouched();
    if (this.form.invalid) return;
    if (this.editMode) {
      this.updateAssignmentType();
    } else {
      this.addAssignmentType();
    }
  }

  updateAssignmentType() {
    const assignmentType: AddAssignmentTypeModel = new AddAssignmentTypeModel();
    assignmentType.assignmentType.title = this.form.value.title;
    assignmentType.assignmentType.weightage = this.form.value.weightage;
    assignmentType.assignmentType.assignmentTypeId =
      this.editDetails.assignmentTypeId;
    assignmentType.assignmentType.academicYear =
      this.defaultValueService.getAcademicYear();
    assignmentType.assignmentType.courseSectionId = this.courseSectionId;
    assignmentType.assignmentType.markingPeriodId =
      +this.defaultValueService.getMarkingPeriodId();

    this.assignmentService
      .updateAssignmentType(assignmentType)
      .subscribe((res) => {
        if (res) {
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
            this.snackbar.open(res._message, "", {
              duration: 3000,
            });
          } else {
            this.snackbar.open(res._message, "", {
              duration: 3000,
            });
            this.dialogRef.close(true);
          }
        } else {
          this.snackbar.open("Failed to update assignment type", "", {
            duration: 3000,
          });
        }
      });
  }

  addAssignmentType() {
    const assignmentType: AddAssignmentTypeModel = new AddAssignmentTypeModel();
    assignmentType.assignmentType.title = this.form.value.title;
    assignmentType.assignmentType.weightage = this.form.value.weightage;

    assignmentType.assignmentType.academicYear =
      this.defaultValueService.getAcademicYear();
    assignmentType.assignmentType.courseSectionId = this.courseSectionId;
    assignmentType.assignmentType.markingPeriodId =
      +this.defaultValueService.getMarkingPeriodId();
    this.assignmentService
      .addAssignmentType(assignmentType)
      .subscribe((res) => {
        if (res) {
          this.snackbar.open(res._message, "", {
            duration: 3000,
          });
          if (!res._failure) {
            this.dialogRef.close(true);
          }
        } else {
          if(res._failure){
            this.commonService.checkTokenValidOrNot(res._message);
          }
          this.snackbar.open("Failed to add assignment type", "", {
            duration: 3000,
          });
        }
      });
  }
}
