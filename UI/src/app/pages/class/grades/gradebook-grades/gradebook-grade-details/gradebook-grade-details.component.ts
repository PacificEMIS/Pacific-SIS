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

import { Component, Input, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import icEdiit from '@iconify/icons-ic/twotone-edit';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { AddGradebookGradeByStudentModel, ViewGradebookGradeByStudentModel } from 'src/app/models/gradebook-grades.model';
import { CommonService } from 'src/app/services/common.service';
import { GradeBookConfigurationService } from 'src/app/services/gradebook-configuration.service';

@Component({
  selector: 'vex-gradebook-grade-details',
  templateUrl: './gradebook-grade-details.component.html',
  styleUrls: ['./gradebook-grade-details.component.scss']
})
export class GradebookGradeDetailsComponent implements OnInit {

  icEdiit = icEdiit;
  @Input() studentId;
  @Input() isWeightedSection;
  viewGradebookGradeByStudentModel: ViewGradebookGradeByStudentModel = new ViewGradebookGradeByStudentModel();
  gradesByStudentId;
  addGradebookGradeByStudentModel: AddGradebookGradeByStudentModel =  new AddGradebookGradeByStudentModel();
  selectedCourseSection;
  markingPeriodId;

  constructor(
    private gradeBookConfigurationService: GradeBookConfigurationService,
    public defaultValuesService: DefaultValuesService,
    private commonService: CommonService,
    private snackbar: MatSnackBar,
  ) { 
  }

  ngOnInit(): void {
    this.getGradebookGradeByStudent();
    this.selectedCourseSection = this.defaultValuesService.getSelectedCourseSection();
    this.markingPeriodId = this.findMarkingPeriodTitleById(this.selectedCourseSection);
  }

  findMarkingPeriodTitleById(coursesectionDetails) {
    let markingPeriodId;
    if (coursesectionDetails.yrMarkingPeriodId) {
      markingPeriodId = '0_' + coursesectionDetails.yrMarkingPeriodId;
    } else if (coursesectionDetails.smstrMarkingPeriodId) {
      markingPeriodId = '1_' + coursesectionDetails.smstrMarkingPeriodId;
    } else if (coursesectionDetails.qtrMarkingPeriodId) {
      markingPeriodId = '2_' + coursesectionDetails.qtrMarkingPeriodId;
    } else if (coursesectionDetails.prgrsprdMarkingPeriodId) {
      markingPeriodId = '3_' + coursesectionDetails.prgrsprdMarkingPeriodId;
    } else {
      markingPeriodId = null;
    }
    return markingPeriodId;
  }

  getGradebookGradeByStudent() {
    this.viewGradebookGradeByStudentModel.courseSectionId = this.defaultValuesService.getSelectedCourseSection().courseSectionId;
    this.viewGradebookGradeByStudentModel.studentId = this.studentId;

    this.gradeBookConfigurationService.viewGradebookGradeByStudent(this.viewGradebookGradeByStudentModel).subscribe((res: any)=>{
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
      } else{
        this.addGradebookGradeByStudentModel = res;
        this.gradesByStudentId = res.assignmentTypeViewModelList;
      }
    })
  }

  submitGradebookGradeByStudent() {
    delete this.addGradebookGradeByStudentModel.academicYear;
    this.addGradebookGradeByStudentModel.markingPeriodId = this.markingPeriodId;
    this.gradeBookConfigurationService.addGradebookGradeByStudent(this.addGradebookGradeByStudentModel).subscribe((res: any)=>{
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
      } else{
        // this.gradesByStudentId = res.assignmentTypeViewModelList;
        this.getGradebookGradeByStudent();
        this.snackbar.open(res._message, '', {
          duration: 10000
        });
      }
    })
  }

}
