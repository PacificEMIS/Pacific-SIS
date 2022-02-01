import { Component, Inject, OnInit, Optional, ViewChild } from '@angular/core';
import { MatCheckbox } from '@angular/material/checkbox';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import icClose from '@iconify/icons-ic/twotone-close'
import { map } from 'rxjs/operators';
import { StaffPortalAssignmentService } from '../../../../services/staff-portal-assignment.service';
import { AllScheduledCourseSectionForStaffModel } from '../../../../models/teacher-schedule.model';
import { TeacherScheduleService } from '../../../../services/teacher-schedule.service';
import { AddAssignmentModel } from '../../../../models/staff-portal-assignment.model';
import { CommonService } from '../../../../services/common.service';
import { DefaultValuesService } from '../../../../common/default-values.service';
import { DasboardService } from '../../../../services/dasboard.service';

@Component({
  selector: 'vex-copy-assignment',
  templateUrl: './copy-assignment.component.html',
  styleUrls: ['./copy-assignment.component.scss']
})
export class CopyAssignmentComponent implements OnInit {

  @ViewChild('checkBox') checkBox: MatCheckbox;
  checkAll: boolean;
  icClose = icClose;
  showCopyAssignment = true;
  showSuccessAssignment = false;
  coursecount: number=0;
  courseId: number;
  courseSectionArray: number[] = [];
  addAssignmentModel: AddAssignmentModel = new AddAssignmentModel();
  allScheduledCourseSectionBasedOnTeacher: AllScheduledCourseSectionForStaffModel = new AllScheduledCourseSectionForStaffModel();

  constructor(private dialogRef: MatDialogRef<CopyAssignmentComponent>,
    @Optional() @Inject(MAT_DIALOG_DATA) public data: any,
    private teacherReassignmentService: TeacherScheduleService,
    private staffPortalAssignmentService: StaffPortalAssignmentService,
    private commonService: CommonService,
    private snackbar: MatSnackBar,
    private dashboardService: DasboardService,
    private defaultValuesService: DefaultValuesService
    ) {
      this.dashboardService.selectedCourseSectionDetails.subscribe((res) => {
        if (res) {
          this.courseId = +res.courseId;
        }
      });
      
     }

  ngOnInit(): void {
    this.getAllScheduledCourseSectionBasedOnTeacher();
  }


  getAllScheduledCourseSectionBasedOnTeacher() {
    this.allScheduledCourseSectionBasedOnTeacher.staffId = this.defaultValuesService.getUserId();
    //this.allScheduledCourseSectionBasedOnTeacher.courseSectionViewList = null;
    this.teacherReassignmentService.getAllScheduledCourseSectionForStaff(this.allScheduledCourseSectionBasedOnTeacher).pipe(
      map((res) => {
        res._userName = this.defaultValuesService.getHttpError();
        return res;
      })
    ).subscribe((res) => {
      if (res) {
        if (res._failure) {
          this.commonService.checkTokenValidOrNot(res._message);
          this.allScheduledCourseSectionBasedOnTeacher.courseSectionViewList = [];
          if (!res.courseSectionViewList) {
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
        } else {
          this.allScheduledCourseSectionBasedOnTeacher = res;
          this.allScheduledCourseSectionBasedOnTeacher.courseSectionViewList = this.allScheduledCourseSectionBasedOnTeacher.courseSectionViewList.filter(x => x.courseId === this.courseId && x.courseSectionId !== this.data?.assignmentDetails?.courseSectionId);
          this.coursecount= this.allScheduledCourseSectionBasedOnTeacher.courseSectionViewList.length;
        }
      }
      else {
        this.snackbar.open('' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });

      }
    })
  }

  updateCheck(event) {
    if (this.courseSectionArray.length === this.allScheduledCourseSectionBasedOnTeacher.courseSectionViewList.length) {
      for (let i = 0; i < this.allScheduledCourseSectionBasedOnTeacher.courseSectionViewList.length; i++) {
        let index = this.courseSectionArray.indexOf(this.allScheduledCourseSectionBasedOnTeacher.courseSectionViewList[i].courseSectionId);
        if (index > -1) {
          this.courseSectionArray.splice(index, 1);
        }
        else {
          this.courseSectionArray.push(this.allScheduledCourseSectionBasedOnTeacher.courseSectionViewList[i].courseSectionId);
        }
      }
    }
    else if (this.courseSectionArray.length === 0) {
      for (let i = 0; i < this.allScheduledCourseSectionBasedOnTeacher.courseSectionViewList.length; i++) {
        let index = this.courseSectionArray.indexOf(this.allScheduledCourseSectionBasedOnTeacher.courseSectionViewList[i].courseSectionId);
        if (index > -1) {
          this.courseSectionArray.splice(index, 1);
        }
        else {
          this.courseSectionArray.push(this.allScheduledCourseSectionBasedOnTeacher.courseSectionViewList[i].courseSectionId);
        }
      }
    }
    else {
      for (let i = 0; i < this.allScheduledCourseSectionBasedOnTeacher.courseSectionViewList.length; i++) {
        let index = this.courseSectionArray.indexOf(this.allScheduledCourseSectionBasedOnTeacher.courseSectionViewList[i].courseSectionId);
        if (index > -1) {
          continue;
        }
        else {
          this.courseSectionArray.push(this.allScheduledCourseSectionBasedOnTeacher.courseSectionViewList[i].courseSectionId);
        }
      }
    }
  }

  selectChildren(event, id) {
    event.preventDefault();
    let index = this.courseSectionArray.indexOf(id);
    if (index > -1) {
      this.courseSectionArray.splice(index, 1);
    }
    else {
      this.courseSectionArray.push(id);
    }
    if (this.courseSectionArray.length == this.allScheduledCourseSectionBasedOnTeacher.courseSectionViewList.length) {
      this.checkAll = true;
      this.checkBox.checked = true;
    } else {
      this.checkAll = false;
      this.checkBox.checked = false;
    }
  }

  showSuccessMsg() {
    this.addAssignmentModel.assignment=this.data.assignmentDetails;
    this.addAssignmentModel.courseSectionIds = this.courseSectionArray;
    this.staffPortalAssignmentService.copyAssignmentForCourseSection(this.addAssignmentModel).subscribe((res) => {
      if (res) {
        if (res._failure) {
          this.commonService.checkTokenValidOrNot(res._message);
          this.snackbar.open(res._message, '', {
            duration: 10000
          });
        }
        else {
          this.showCopyAssignment = false;
          this.showSuccessAssignment = true;
        }
      }
      else {
        this.snackbar.open('' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });

      }
    })
  }
}
