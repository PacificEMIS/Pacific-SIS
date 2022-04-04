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
  @Input() maxAnomalousGrade;
  viewGradebookGradeByStudentModel: ViewGradebookGradeByStudentModel = new ViewGradebookGradeByStudentModel();
  gradesByStudentId;
  addGradebookGradeByStudentModel: AddGradebookGradeByStudentModel =  new AddGradebookGradeByStudentModel();
  selectedCourseSection;
  markingPeriodId;
  isStudentGradeValid;

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
        res.assignmentTypeViewModelList?.map(item => {
          item.assignmentViewModelList.map(subItem => {
            this.maxAnomalousGrade ? subItem.maxAllowedMarks = ((subItem.points * this.maxAnomalousGrade) / 100) + subItem.points : subItem.maxAllowedMarks = null;
          });
        });
        this.addGradebookGradeByStudentModel = res;
        this.gradesByStudentId = res.assignmentTypeViewModelList;
      }
    })
  }

  checkStudentGradeIsValidOrNot() {
    if (this.maxAnomalousGrade) {
      loop1:
      for (let item of this.addGradebookGradeByStudentModel.assignmentTypeViewModelList) {
        loop2:
        for (let subItem of item.assignmentViewModelList) {
          if (subItem.allowedMarks > subItem.maxAllowedMarks) {
            this.snackbar.open('Please enter a valid anomalous grade. Check "Allowed maximum % in anomalous grade".', '', {
              duration: 10000
            });
            this.isStudentGradeValid = false;
            break loop1;
          } else {
            this.isStudentGradeValid = true;
          }
        }
      }
    } else {
      this.isStudentGradeValid = true;
    }
  }

  submitGradebookGradeByStudent() {
    this.checkStudentGradeIsValidOrNot();
    if (this.isStudentGradeValid) {
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

}
