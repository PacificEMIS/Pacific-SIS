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

import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import icClose from '@iconify/icons-ic/twotone-close';
import { StudentUpdateAttendanceCommentsModel } from '../../../../models/take-attendance-list.model';
import { DefaultValuesService } from '../../../../common/default-values.service';
import { StudentAttendanceService } from '../../../../services/student-attendance.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CommonService } from 'src/app/services/common.service';

@Component({
  selector: 'vex-add-comments',
  templateUrl: './add-teacher-comments.component.html',
  styleUrls: ['./add-teacher-comments.component.scss']
})
export class AddTeacherCommentsComponent implements OnInit {

  icClose = icClose;
  comments:string;
  actionButtonTitle='submit';
  headerTitle='addCommentTo';
  commentBox = false;
  administratorComment = true;
  tapForEdit: boolean = false;
  studentAttendanceList: StudentUpdateAttendanceCommentsModel= new StudentUpdateAttendanceCommentsModel();

  constructor(private dialogRef: MatDialogRef<AddTeacherCommentsComponent>,
    public translateService:TranslateService,
     @Inject(MAT_DIALOG_DATA) public data,
     public defaultValuesService: DefaultValuesService,
     private studentAttendanceService: StudentAttendanceService,
     private commonService: CommonService,
    private snackbar: MatSnackBar,) { 
    if (data.type === 'update') {
      if (data.commentData) {
        data.commentData.map((item) => {
          if (item?.membership?.profileType === 'Homeroom Teacher' || item?.membership?.profileType === 'Teacher') {
            this.comments = item.comment;
            this.actionButtonTitle = 'update'
            this.headerTitle = 'updateCommentTo'
            if (data?.type === 'update' && (item.comment?.trim() === '' || item.comment === null)) {
              this.tapForEdit = true;
            }
          }
        });
      }
    } else {
      if (data.commentData) {
        this.comments = data.commentData[0].comment;
      }
    }
  }

  ngOnInit(): void {
  }

  addOrUpdateComments() {
    if(this.data?.type === 'update') {
    this.studentAttendanceList.studentAttendanceComments.comment =  this.comments;
    this.studentAttendanceList.staffId = +this.defaultValuesService.getUserId();
    this.studentAttendanceList.studentAttendanceComments.studentAttendanceId = this.data.commentData[0].studentAttendanceId;
    this.studentAttendanceList.studentAttendanceComments.CommentId = this.data.commentData[0].commentId;
    this.studentAttendanceList.studentAttendanceComments.studentId = this.data.commentData[0].studentId;

    this.studentAttendanceService.addUpdateStudentAttendanceComments(this.studentAttendanceList).subscribe((response)=>{
  if (response._failure) { this.commonService.checkTokenValidOrNot(response._message);


      this.snackbar.open(response._message, '', {
        duration: 10000
      });   
  } else {
    this.snackbar.open(response._message, '', {
      duration: 10000
    });   
    this.dialogRef.close({ response: response.studentAttendanceComments ,submit:true, status: 'update'});
  }
});
    } else {
      this.dialogRef.close({response :{comment:this.comments, membershipId:+this.defaultValuesService.getuserMembershipID()},submit:true, status: 'submit'});
    }
    
  }

  openCommentBox(){
    this.administratorComment = false;
    this.commentBox = true;
  }


}
