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
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import icClose from '@iconify/icons-ic/twotone-close';
import { CommonService } from 'src/app/services/common.service';
import { DefaultValuesService } from '../../../../common/default-values.service';
import { AddAssignmentModel } from '../../../../models/staff-portal-assignment.model';
import { DasboardService } from '../../../../services/dasboard.service';
import { StaffPortalAssignmentService } from '../../../../services/staff-portal-assignment.service';
import { SharedFunction } from '../../../shared/shared-function';

@Component({
  selector: 'vex-create-assignment',
  templateUrl: './create-assignment.component.html',
  styleUrls: ['./create-assignment.component.scss']
})
export class CreateAssignmentComponent implements OnInit {

  icClose = icClose;
  form: FormGroup;
  assignmentTypes = [];
  editMode: boolean = false;
  editDetails;
  generalConfiguration;
  courseSectionId: number;
  courseSectionStartDate: string;
  courseSectionEndDate: string;
  constructor(private dialogRef: MatDialogRef<CreateAssignmentComponent>,
    private fb: FormBuilder,
    private snackbar: MatSnackBar,
    private assignmentService: StaffPortalAssignmentService,
    private defaultService: DefaultValuesService,
    private dashboardService: DasboardService,
    private commonFunction: SharedFunction,
    private commonService: CommonService,
    @Optional() @Inject(MAT_DIALOG_DATA) public data: any) {
    this.dashboardService.selectedCourseSectionDetails.subscribe((res) => {
      if (res) {
        this.courseSectionId = +res.courseSectionId;
        this.courseSectionStartDate = res.durationStartDate;
        this.courseSectionEndDate = res.durationEndDate;
      }
    });
    this.generalConfiguration = this.data.generalConfiguration;
  }

  ngOnInit(): void {
    this.form = this.fb.group({
      assignmentTitle: ['', Validators.required],
      points: ['', Validators.required],
      assignmentTypeId: [this.data.currentAssignmentType, Validators.required],
      assignmentDate: ['', Validators.required],
      dueDate: ['', Validators.required],
      assignmentDescription: ['']
    });
    if (this.data.editMode) {
      this.editMode = this.data.editMode;
      this.editDetails = this.data.editDetails;
      this.patchFormValue();
    }
    else {
      if (this.generalConfiguration?.includes('dueDateDefaultsToToday')) {
        this.form.controls.dueDate.patchValue(new Date());
      }
      if (this.generalConfiguration?.includes('assignedDateDefaultsToToday')) {
        this.form.controls.assignmentDate.patchValue(new Date());
      }
    }
    this.assignmentTypes = this.data.assignmentTypes;
  }

  patchFormValue() {
    this.form.patchValue(this.editDetails);
  }

  checkFormValidation() {
    this.form.markAllAsTouched();
    if (this.form.invalid) return;

    if (this.editMode) {
      this.updateAssignment();
    } else {
      this.addAssignment();
    }
  }

  addAssignment() {
    this.assignmentService.addAssignment(this.fillFormValues()).subscribe((res) => {
      if (res) {
        this.snackbar.open(res._message, "", {
          duration: 3000,
        })
        if (!res._failure) {
          this.dialogRef.close(res.assignment.assignmentTypeId);
        } else {
          this.commonService.checkTokenValidOrNot(res._message);
        }
      } else {
        this.snackbar.open("Failed to add assignment", "", {
          duration: 3000,
        });
      }
    })
  }

  updateAssignment() {
    this.assignmentService.updateAssignment(this.fillFormValues()).subscribe((res) => {
      if (res) {
        this.snackbar.open(res._message, "", {
          duration: 3000,
        })
        if (!res._failure) {
          this.dialogRef.close(res.assignment.assignmentTypeId);
        } else {
          this.commonService.checkTokenValidOrNot(res._message);
        }
      } else {
        this.snackbar.open("Failed to update assignment", "", {
          duration: 3000,
        });
      }
    })
  }

  fillFormValues() {
    let assignmentModel: AddAssignmentModel = new AddAssignmentModel();
    assignmentModel.assignment = this.form.value;
    assignmentModel.assignment.assignmentDate = this.commonFunction.formatDateSaveWithoutTime(assignmentModel.assignment.assignmentDate);
    assignmentModel.assignment.dueDate = this.commonFunction.formatDateSaveWithoutTime(assignmentModel.assignment.dueDate);
    assignmentModel.assignment.schoolId = assignmentModel.schoolId;
    assignmentModel.assignment.courseSectionId = this.courseSectionId;
    assignmentModel.assignment.tenantId = assignmentModel.tenantId;
    assignmentModel.assignment.staffId = this.defaultService.getUserId();
    if (this.editMode) {
      assignmentModel.assignment.assignmentId = this.editDetails.assignmentId;
      assignmentModel.assignment.updatedBy = this.defaultService.getUserGuidId();
    } else {
      assignmentModel.assignment.createdBy = this.defaultService.getUserGuidId();

    }
    return assignmentModel;
  }

}
