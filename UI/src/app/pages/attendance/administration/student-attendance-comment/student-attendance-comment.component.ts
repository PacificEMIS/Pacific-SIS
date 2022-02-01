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

import { Component, Inject, OnInit } from "@angular/core";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import icClose from "@iconify/icons-ic/twotone-close";
import { TranslateService } from "@ngx-translate/core";
import { StudentAttendanceService } from "../../../../services/student-attendance.service";
import { DefaultValuesService } from "../../../../common/default-values.service";
import { GetAllAttendanceCodeModel } from "../../../../models/attendance-code.model";
import { AddUpdateStudentAttendanceModelFor360, StudentAttendanceHistoryViewModel, StudentAttendanceModelFor360, StudentUpdateCommentsModel } from "../../../../models/take-attendance-list.model";
import { AttendanceCodeService } from "../../../../services/attendance-code.service";
import { CommonService } from "../../../../services/common.service";
import { MatSnackBar } from "@angular/material/snack-bar";
import { takeUntil } from "rxjs/operators";
import { Subject } from "rxjs";
import { ProfilesTypes } from "src/app/enums/profiles.enum";
import * as moment from "moment";
@Component({
  selector: "vex-student-attendance-comment",
  templateUrl: "./student-attendance-comment.component.html",
  styleUrls: ["./student-attendance-comment.component.scss"],
})
export class StudentAttendanceCommentComponent implements OnInit {
  icClose = icClose;
  category: number;
  rowIndex: number;
  commentBox = false;
  destroySubject$: Subject<void> = new Subject();
  administratorComment = true;
  getAllAttendanceCodeModel: GetAllAttendanceCodeModel = new GetAllAttendanceCodeModel();
  addUpdateStudentAttendanceModel: AddUpdateStudentAttendanceModelFor360 = new AddUpdateStudentAttendanceModelFor360();
  studentAttendanceModelFor360: StudentAttendanceModelFor360 = new StudentAttendanceModelFor360();
  studentAttendanceHistoryViewModel: StudentAttendanceHistoryViewModel = new StudentAttendanceHistoryViewModel();
  studentAttendance = [new StudentAttendanceModelFor360()];
  attendanceCodeList = [];
  attendanceHistoryList = [];
  profiles = ProfilesTypes;

  constructor(public translateService: TranslateService,
    public dialogRef: MatDialogRef<StudentAttendanceCommentComponent>,
    private commonService: CommonService,
    public defaultValuesService: DefaultValuesService,
    private attendanceCodeService: AttendanceCodeService,
    private snackbar: MatSnackBar,
    private studentAttendanceService: StudentAttendanceService,
    @Inject(MAT_DIALOG_DATA) public data) {
  }

  ngOnInit(): void {
    this.studentAttendance = this.data?.studentAttendanceList;
    this.getAllAttendanceCode();
  }

  // Get All Attendance Codes
  getAllAttendanceCode() {
    this.getAllAttendanceCodeModel.attendanceCategoryId = 1;
    this.attendanceCodeService.getAllAttendanceCode(this.getAllAttendanceCodeModel).subscribe((res: any) => {
      if (res._failure) {
        this.commonService.checkTokenValidOrNot(res._message);
        if (res.attendanceCodeList === null) {
          this.attendanceCodeList = [];

        } else {
          this.attendanceCodeList = [];
        }
      } else {
        this.attendanceCodeList = res.attendanceCodeList;

      }
    });
  }

  attendanceCodeSelected(index) {
    this.studentAttendance[index].attendanceCode = +this.studentAttendance[index].attendanceCode;
  }

  // date convert to local
  formatDate(date) {
    return moment.utc(date).local().format("YYYY-MM-DD HH:mm:ss");
  }

  //get history
  getStudentAttendanceHistory() {
    this.studentAttendanceService.getStudentAttendanceHistory(this.studentAttendanceHistoryViewModel).subscribe((res: any) => {
      if (res._failure) {
        this.commonService.checkTokenValidOrNot(res._message);
        this.attendanceHistoryList = [];

      } else {
        res.attendanceHistoryViewModels.map((item)=> {
          item.modificationTimestamp= this.formatDate(item.modificationTimestamp); 
        });
        this.attendanceHistoryList = res.attendanceHistoryViewModels;
      }
    });
  }

  openExpandable(step: number, index: number) {
    this.category = step;
    this.rowIndex = index;
    if (this.category === 1) {
      this.studentAttendance.map(item => {

        if (item.studentAttendanceComments.length > 0) {
          let adminIndex = item.studentAttendanceComments.findIndex(item => item.membershipId === 1);
          if (adminIndex == -1) {
            this.commentBox = true;
            item.studentAttendanceComments[1] = {
              studentAttendanceId: 0,
              CommentId: 0,
              comment: null,
              membershipId: +this.defaultValuesService.getuserMembershipID(),
              studentId: item.studentId,
              tenantId: this.defaultValuesService.getTenantID(),
              schoolId: this.defaultValuesService.getSchoolID()
            }
          }
        }
        else {
          this.commentBox = true;
          item.studentAttendanceComments[0] = {
            studentAttendanceId: 0,
            CommentId: 0,
            comment: null,
            membershipId: +this.defaultValuesService.getuserMembershipID(),
            studentId: item.studentId,
            tenantId: this.defaultValuesService.getTenantID(),
            schoolId: this.defaultValuesService.getSchoolID()
          }
        }
      })
    }
    else {
      this.studentAttendanceHistoryViewModel.tenantId = this.defaultValuesService.getTenantID();
      this.studentAttendanceHistoryViewModel.schoolId = this.studentAttendance[index].schoolId;
      this.studentAttendanceHistoryViewModel.studentId = this.studentAttendance[index].studentId;
      this.studentAttendanceHistoryViewModel.courseId = this.studentAttendance[index].courseId;
      this.studentAttendanceHistoryViewModel.courseSectionId = this.studentAttendance[index].courseSectionId;
      this.studentAttendanceHistoryViewModel.periodId = this.studentAttendance[index].periodId;
      this.studentAttendanceHistoryViewModel.blockId = this.studentAttendance[index].blockId;
      this.studentAttendanceHistoryViewModel.attendanceDate = this.studentAttendance[index].attendanceDate;
      this.getStudentAttendanceHistory();
    }
  }
  openCommentBox() {
    this.administratorComment = false;
    this.commentBox = true;
  }

  close() {
    this.dialogRef.close(true);
  }

  submitStudent360() {
    if (this.defaultValuesService.getUserMembershipType() === 'Super Administrator' || this.defaultValuesService.getUserMembershipType() === 'School Administrator') {
      this.addUpdateStudentAttendanceModel.userId = this.defaultValuesService.getUserId();
    }
    else{
      this.addUpdateStudentAttendanceModel.userId= null;
    }
    this.addUpdateStudentAttendanceModel.studentAttendance = this.studentAttendance;
    this.addUpdateStudentAttendanceModel.studentId = this.studentAttendance[0].studentId;
    if (
      this.addUpdateStudentAttendanceModel.studentAttendance?.length === 0
    ) {
      this.snackbar.open("Nothing to Update", "Ok", {
        duration: 3000,
      });
      return;
    }
    this.studentAttendanceService.addUpdateStudentAttendanceForStudent360(this.addUpdateStudentAttendanceModel)
      .pipe(takeUntil(this.destroySubject$))
      .subscribe((res) => {
        if (res) {
          if (res._failure) {
            this.commonService.checkTokenValidOrNot(res._message);
            this.snackbar.open(res._message, "", {
              duration: 10000,
            });
          } else {
            this.snackbar.open(res._message, "", {
              duration: 10000,
            });
            this.addUpdateStudentAttendanceModel.studentAttendance = [];
            this.openExpandable(this.category,this.rowIndex);
          }
        } else {
          this.snackbar.open(this.defaultValuesService.getHttpError(), "", {
            duration: 10000,
          });
        }
      });
  }




}
