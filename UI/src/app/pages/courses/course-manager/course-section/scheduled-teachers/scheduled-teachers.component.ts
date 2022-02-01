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
import { Component, Input, OnInit, SimpleChanges } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import icEmail from '@iconify/icons-ic/twotone-email';
import { CommonService } from 'src/app/services/common.service';
import { ScheduledStaffForCourseSection } from '../../../../../models/course-section.model';
import { CourseSectionService } from '../../../../../services/course-section.service';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { ImageCropperService } from 'src/app/services/image-cropper.service';
import { ModuleIdentifier } from 'src/app/enums/module-identifier.enum';
import { SchoolCreate } from 'src/app/enums/school-create.enum';
import { StaffService } from 'src/app/services/staff.service';
import { PageRolesPermission } from 'src/app/common/page-roles-permissions.service';
import { Router } from '@angular/router';
@Component({
  selector: 'vex-scheduled-teachers',
  templateUrl: './scheduled-teachers.component.html',
  styleUrls: ['./scheduled-teachers.component.scss']
})
export class ScheduledTeachersComponent implements OnInit {
  icEmail = icEmail;
  @Input() courseSectionDetails;
  moduleIdentifier=ModuleIdentifier;
  createMode=SchoolCreate;
  constructor(
    private courseSectionService:CourseSectionService,
    private snackbar:MatSnackBar,
    private commonService: CommonService,
    private defaultValuesService: DefaultValuesService,
    private imageCropperService:ImageCropperService,
    private staffService: StaffService,
    private pageRolePermission: PageRolesPermission,
    private router: Router,
    ) { } 
  scheduledTeacher:ScheduledStaffForCourseSection = new ScheduledStaffForCourseSection();
  
  ngOnChanges(changes:SimpleChanges){
        this.getScheduledTeachers();
  }
  ngOnInit(): void {
  }

  // Scheduled Teacher list
  getScheduledTeachers(){
    this.scheduledTeacher.courseId=this.courseSectionDetails.courseSection.courseId;
    this.scheduledTeacher.courseSectionId=this.courseSectionDetails.courseSection.courseSectionId;

    this.courseSectionService.getAllStaffScheduleInCourseSection(this.scheduledTeacher).subscribe((res)=>{
      if (typeof (res) == 'undefined') {       
        this.snackbar.open('Teacher Schedule Failed ' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
      else {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message); 
          this.scheduledTeacher.courseSectionsList=[]
            if(!res.courseSectionsList){
              this.snackbar.open(res._message, '', {
                duration: 10000
              });
            }
        } else {
          this.scheduledTeacher.courseSectionsList=res.courseSectionsList;
          this.scheduledTeacher.courseSectionsList[0].staffCoursesectionSchedule.forEach((item)=>{
            item.staffMaster.staffEmail=item.staffMaster.loginEmailAddress?item.staffMaster.loginEmailAddress
                            :item.staffMaster.personalEmail?item.staffMaster.personalEmail
                            :item.staffMaster.schoolEmail?item.staffMaster.schoolEmail:null
          })
        }
      }
    })
  }

  viewStaffDetails(staff) {
    this.imageCropperService.enableUpload({module:this.moduleIdentifier.STAFF,upload:true,mode:this.createMode.VIEW});
    this.staffService.setStaffId(staff.staffId);
    // this.defaultValuesService.setSchoolID(data.schoolId, true);
    this.getPermissionForStaff();
  }

  getPermissionForStaff() {
        let permittedDetails= this.pageRolePermission.getPermittedSubCategories('/school/staff');
    if(permittedDetails.length){
      this.staffService.setCategoryId(0);
      this.staffService.setCategoryTitle(permittedDetails[0].title);      
      this.router.navigateByUrl(permittedDetails[0].path, {state: { type: SchoolCreate.VIEW}});
    } else {
          this.defaultValuesService.setSchoolID(undefined);
          this.snackbar.open('Saff didnot have permission to view details.', '', {
            duration: 10000
          });
        }
  }

}
