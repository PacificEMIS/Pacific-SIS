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

import { Component, OnInit } from "@angular/core";
import icEdit from "@iconify/icons-ic/twotone-edit";
import icDeleteForever from "@iconify/icons-ic/twotone-delete-forever";
import icDelete from "@iconify/icons-ic/twotone-delete";
import icFileCopy from "@iconify/icons-ic/twotone-file-copy";
import { MatDialog } from "@angular/material/dialog";
import { AddAssignmentComponent } from "./add-assignment/add-assignment.component";
import { CreateAssignmentComponent } from "./create-assignment/create-assignment.component";
import { DeleteAssignmentsComponent } from "./delete-assignments/delete-assignments.component";
import { StaffPortalService } from "../../../services/staff-portal.service";
import {
  AddAssignmentModel,
  AddAssignmentTypeModel,
  AssignmentList,
  GetAllAssignmentsModel,
} from "../../../models/staff-portal-assignment.model";
import { DefaultValuesService } from "../../../common/default-values.service";
import { DasboardService } from "../../../services/dasboard.service";
import { MatSnackBar } from "@angular/material/snack-bar";
import { ConfirmDialogComponent } from "../../shared-module/confirm-dialog/confirm-dialog.component";
import { ViewAssignmentDetailsComponent } from "./view-assignment-details/view-assignment-details.component";
import { StaffPortalAssignmentService } from "../../../services/staff-portal-assignment.service";
import { CopyAssignmentComponent } from "./copy-assignment/copy-assignment.component";
import { CommonService } from "src/app/services/common.service";

@Component({
  selector: "vex-assignments",
  templateUrl: "./assignments.component.html",
  styleUrls: ["./assignments.component.scss"],
})
export class AssignmentsComponent implements OnInit {
  icEdit = icEdit;
  icDeleteForever = icDeleteForever;
  icDelete = icDelete;
  icFileCopy = icFileCopy;

  assignmentModel: GetAllAssignmentsModel = new GetAllAssignmentsModel();
  courseSectionId: number;
  selectedAssignmentType: AssignmentList;

  constructor(
    private dialog: MatDialog,
    private dashboardService: DasboardService,
    private defaultValueService: DefaultValuesService,
    private snackbar: MatSnackBar,
    private assignmentService: StaffPortalAssignmentService,
    private commonService: CommonService,
  ) {
    this.dashboardService.selectedCourseSectionDetails.subscribe((res) => {
      if (res) {
        this.courseSectionId = +res.courseSectionId;
      }
    });
  }

  ngOnInit(): void {
    this.getAllAssignments();
  }

  changeAssignmentType(assignmentType) {
    this.selectedAssignmentType = assignmentType;
  }

  addEditAssignmentType(assignmentTypeDetails?) {
    this.dialog
      .open(AddAssignmentComponent, {
        data: {
          editMode: assignmentTypeDetails ? true : false,
          editDetails: assignmentTypeDetails,
        },
        width: "500px",
      })
      .afterClosed()
      .subscribe((res) => {
        if (res) {
          // Call getAll
          this.getAllAssignments();
        }
      });
  }

  confirmAssignmentTypeDelete(assignmentTypeDetails) {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: "400px",
      data: {
        title: "Are you sure?",
        message: `You are about to delete ${assignmentTypeDetails.title}.`,
      },
    });
    dialogRef.afterClosed().subscribe((dialogResult) => {
      if (dialogResult) {
        this.deleteAssignmentType(assignmentTypeDetails);
      }
    });
  }

  deleteAssignmentType(assignmentTypeDetails) {
    const assignmentType: AddAssignmentTypeModel = new AddAssignmentTypeModel();
    assignmentType.assignmentType.assignmentTypeId =
      assignmentTypeDetails.assignmentTypeId;
    assignmentType.assignmentType.courseSectionId = this.courseSectionId;

    this.assignmentService
      .deleteAssignmentType(assignmentType)
      .subscribe((res) => {
        if (res) {
          this.snackbar.open(res._message, "", {
            duration: 3000,
          });
          if (!res._failure) {
            this.getAllAssignments();
          } else {
        this.commonService.checkTokenValidOrNot(res._message);

          }
        } else {

          this.snackbar.open("Failed to delete assignment details", "", {
            duration: 3000,
          });
        }
      });
  }

  getAllAssignments(assignmentTypeId?) {
    this.assignmentModel.courseSectionId = this.courseSectionId;
    this.assignmentService
      .getAllAssignmentType(this.assignmentModel)
      .subscribe((res) => {
        if (res) {
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
            this.assignmentModel.assignmentTypeList = [];
            this.selectedAssignmentType = null;
            if (!res.assignmentTypeList) {
              this.snackbar.open(res._message, "", {
                duration: 3000,
              });
            }
          } else {
            this.assignmentModel = res;
            if (assignmentTypeId) {
              let addedTypeIdIndex = res.assignmentTypeList.findIndex(
                (item) => item.assignmentTypeId === assignmentTypeId
              );
              if (addedTypeIdIndex !== -1) {
                this.selectedAssignmentType =
                  res.assignmentTypeList[addedTypeIdIndex];
              } else {
                this.selectedAssignmentType = res.assignmentTypeList[0];
              }
            } else {
              let index = res.assignmentTypeList.findIndex(
                (item) =>
                  item.assignmentTypeId ===
                  this.selectedAssignmentType?.assignmentTypeId
              );
              if (index !== -1) {
                this.selectedAssignmentType = res.assignmentTypeList[index];
              } else {
                this.selectedAssignmentType = res.assignmentTypeList[0];
              }
            }
          }
        } else {
          this.snackbar.open("Failed to fetch assignment details", "", {
            duration: 3000,
          });
        }
      });
  }

  addEditAssignment(assignmentDetails?) {
    this.dialog
      .open(CreateAssignmentComponent, {
        data: {
          editMode: assignmentDetails ? true : false,
          editDetails: assignmentDetails,
          assignmentTypes: this.assignmentModel.assignmentTypeList,
          currentAssignmentType: this.selectedAssignmentType.assignmentTypeId,
        },
        width: "800px",
      })
      .afterClosed()
      .subscribe((res) => {
        if (res) {
          // Call getAll
          this.getAllAssignments(res);
        }
      });
  }

  confirmDeleteAssignment(assignmentDetails) {
    this.dialog
      .open(DeleteAssignmentsComponent, {
        data: {
          isGradeAssociated: false,
          assignmentTitle: assignmentDetails.title,
          message: `You are about to delete ${assignmentDetails.assignmentTitle}.`,
        },
        width: "500px",
      })
      .afterClosed()
      .subscribe((res) => {
        if (res) {
          this.deleteAssignment(assignmentDetails);
        }
      });
  }

  deleteAssignment(assignmentDetails) {
    let assignmentModel: AddAssignmentModel = new AddAssignmentModel();
    assignmentModel.assignment.courseSectionId = this.courseSectionId;
    assignmentModel.assignment.assignmentId = assignmentDetails.assignmentId;
    assignmentModel.assignment.assignmentTypeId =
      assignmentDetails.assignmentTypeId;

    this.assignmentService
      .deleteAssignment(assignmentModel)
      .subscribe((res) => {
        if (res) {
          this.snackbar.open(res._message, "", {
            duration: 3000,
          });
          if (!res._failure) {
            this.getAllAssignments();
          } else {
        this.commonService.checkTokenValidOrNot(res._message);
          }
        } else {
          this.snackbar.open("Failed to delete assignment details", "", {
            duration: 3000,
          });
        }
      });
  }

  viewDetails(assignmentDetails){
    this.dialog
      .open(ViewAssignmentDetailsComponent, {
        data: {
          assignmentDetails
        },
        width: "500px",
      })
  }

  copyAssignment(assignmentDetails){
    this.dialog.open(CopyAssignmentComponent, {
      data: {
        assignmentDetails
      },
      width: '500px'
    });
  }
}
