import { ConstantPool, isNgContent } from '@angular/compiler';
import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CommonService } from 'src/app/services/common.service';
import { FinalGradingMarkingPeriodList, GradebookConfigurationAddViewModel, GradebookConfigurationQuarter, GradebookConfigurationSemester, GradebookConfigurationYear } from '../../../models/gradebook-configuration.model';
import { GradeScaleListView } from '../../../models/grades.model';
import { GradeBookConfigurationService } from '../../../services/gradebook-configuration.service';
import { GradesService } from '../../../services/grades.service';

@Component({
  selector: 'vex-gradebook-configuration',
  templateUrl: './gradebook-configuration.component.html',
  styleUrls: ['./gradebook-configuration.component.scss']
})
export class GradebookConfigurationComponent implements OnInit {
  selectedCourseSection;
  finalGradingMarkingPeriodList: FinalGradingMarkingPeriodList = new FinalGradingMarkingPeriodList();
  gradeScaleListView: GradeScaleListView = new GradeScaleListView();
  gradebookConfigurationAddViewModel: GradebookConfigurationAddViewModel = new GradebookConfigurationAddViewModel();
  generalEvent = [];
  gradeScaleList = [];
  gradeList = [];
  generalChecbox: string;
  qtotalNot100: boolean = false;
  stotalNot100: boolean = false;
  ytotalNot100: boolean = false;

  constructor(private gradeBookConfigurationService: GradeBookConfigurationService,
    private gradesService: GradesService,
    private snackbar: MatSnackBar,
    private commonService: CommonService,
  ) {
    this.gradebookConfigurationAddViewModel.gradebookConfiguration.general = '';
    this.selectedCourseSection = JSON.parse(localStorage.getItem('selectedCourseSection'));
  }

  ngOnInit(): void {
    //if(this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationGradescale.length===0){
    this.getAllGradeScale(0);
    //}

    this.viewGradebookConfiguration();
  }


  populateFinalGrading() {
    this.gradeBookConfigurationService.populateFinalGrading(this.finalGradingMarkingPeriodList).subscribe(
      (res: FinalGradingMarkingPeriodList) => {
        if (res) {
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
          else {
            this.finalGradingMarkingPeriodList = res;
            if (this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationQuarter.length > 1) {
              
              this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationQuarter.map((item)=>{
                this.finalGradingMarkingPeriodList.quarters.map((subItem, index) => {
                  if(item.qtrMarkingPeriodId === subItem.markingPeriodId) {
                    item.title = subItem.title;
                    item.doesExam = subItem.doesExam;
                    item.doesGrades = subItem.doesGrades;
                  }
                })
              });
             

            }
            else {
              this.finalGradingMarkingPeriodList.quarters = this.finalGradingMarkingPeriodList.quarters.map((item, index) => {
                this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationQuarter.push(new GradebookConfigurationQuarter());
                this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationQuarter[index].qtrMarkingPeriodId = item.markingPeriodId;
                this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationQuarter[index].title = item.title;
                this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationQuarter[index].doesExam = item.doesExam;
                this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationQuarter[index].doesGrades = item.doesGrades;
                this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationQuarter[index].gradingPercentage = 0;
                this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationQuarter[index].examPercentage = 0;


                return item;
              })
              this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationQuarter.pop();
            }

            if (this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationSemester.length > 1) {

              this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationSemester.map((item: any, i) => {
                this.finalGradingMarkingPeriodList.semesters.map((subitem: any, index) => {
                  if (!item.qtrMarkingPeriodId && item.smstrMarkingPeriodId === subitem.markingPeriodId) {
                    item.title = subitem.title;
                    item.doesExam = subitem.doesExam;
                    item.doesGrades = subitem.doesGrades;
                  }

                  subitem.quarters.map((quater) => {
                    if (item.qtrMarkingPeriodId && item.qtrMarkingPeriodId === quater.markingPeriodId) {
                      item.title = quater.title;
                      item.doesExam = quater.doesExam;
                      item.doesGrades = quater.doesGrades;
                    }
                  });
                });
              });
            }
            else {
              this.finalGradingMarkingPeriodList.semesters.map((item, i) => {
                this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationSemester.push(new GradebookConfigurationSemester());
                this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationSemester[i].smstrMarkingPeriodId = item.markingPeriodId;
                this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationSemester[i].title = item.title;
                this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationSemester[i].doesExam = item.doesExam;
                this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationSemester[i].doesGrades = item.doesGrades;
                this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationSemester[i].gradingPercentage = 0;
                this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationSemester[i].examPercentage = 0;

                item.quarters.map((quater, index) => {
                  this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationSemester.push(new GradebookConfigurationSemester());
                  const lastIndex = this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationSemester.length - 1;
                  this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationSemester[lastIndex].smstrMarkingPeriodId = item.markingPeriodId;
                  this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationSemester[lastIndex].qtrMarkingPeriodId = quater.markingPeriodId;
                  this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationSemester[lastIndex].title = quater.title;
                  this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationSemester[lastIndex].doesExam = quater.doesExam;
                  this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationSemester[lastIndex].doesGrades = quater.doesGrades;
                  this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationSemester[lastIndex].gradingPercentage = 0;
                  this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationSemester[lastIndex].examPercentage = 0;
                })

              })
            }
            if (this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationYear.length > 1) {
              if (this.finalGradingMarkingPeriodList.schoolYears) {
                this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationYear.map((item: any, i) => {
                  if (item.yrMarkingPeriodId === this.finalGradingMarkingPeriodList.schoolYears.markingPeriodId && !item.smstrMarkingPeriodId) {
                    item.title = this.finalGradingMarkingPeriodList.schoolYears.title;
                    item.doesExam = this.finalGradingMarkingPeriodList.schoolYears.doesExam;
                    item.doesGrades = this.finalGradingMarkingPeriodList.schoolYears.doesGrades;
                  } else if (item.smstrMarkingPeriodId) {
                    this.finalGradingMarkingPeriodList.schoolYears.semesters.map((semester, i) => {
                      if (item.smstrMarkingPeriodId === semester.markingPeriodId) {
                        item.title = semester.title;
                        item.doesExam = semester.doesExam;
                        item.doesGrades = semester.doesGrades;
                      }
                    });
                  }
                });
              }
            }
            else {
              if (this.finalGradingMarkingPeriodList.schoolYears) {
                this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationYear.push(new GradebookConfigurationYear());
                this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationYear[0].yrMarkingPeriodId = this.finalGradingMarkingPeriodList.schoolYears.markingPeriodId;
                this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationYear[0].title = this.finalGradingMarkingPeriodList.schoolYears.title;
                this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationYear[0].doesExam = this.finalGradingMarkingPeriodList.schoolYears.doesExam;
                this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationYear[0].doesGrades = this.finalGradingMarkingPeriodList.schoolYears.doesGrades;
                this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationYear[0].gradingPercentage = 0;
                this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationYear[0].examPercentage = 0;

                this.finalGradingMarkingPeriodList.schoolYears.semesters.map((semester, i) => {
                  this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationYear.push(new GradebookConfigurationYear());
                  this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationYear[i + 1].yrMarkingPeriodId = this.finalGradingMarkingPeriodList.schoolYears.markingPeriodId;
                  this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationYear[i + 1].smstrMarkingPeriodId = semester.markingPeriodId;
                  this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationYear[i + 1].title = semester.title;
                  this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationYear[i + 1].doesExam = semester.doesExam;
                  this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationYear[i + 1].doesGrades = semester.doesGrades;
                  this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationYear[i + 1].gradingPercentage = 0;
                  this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationYear[i + 1].examPercentage = 0;

                  // return semester;
                })
                this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationYear.pop();
              }
            }

            if(this.selectedCourseSection?.gradeScaleType==='Teacher_Scale') {
              this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationGradescale.map((data, i) => {
                this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationGradescale[i].title = this.gradeScaleList[0]?.grade[i].title;
              })
            }
          }
        }
        else {
          this.snackbar.open(sessionStorage.getItem('httpError'), '', {
            duration: 10000
          });
        }
      }
    );
  }


  generalValue(event, checked) {
    if (checked) {
      this.generalEvent.push(event);
    }
    else {
      var index = this.generalEvent.indexOf(event);
      if (index !== -1) {
        this.generalEvent.splice(index, 1);
      }
    }
    if (this.generalEvent.length > 0) {
      this.generalChecbox = this.generalEvent.toString().replace(',', '|');
    }
    else {
      this.generalChecbox = this.generalEvent.toString();
    }

  }

  getAllGradeScale(selectedGradeScaleId: number) {
    this.gradesService.getAllGradeScaleList(this.gradeScaleListView).subscribe(
      (res: GradeScaleListView) => {

        if (typeof (res) === 'undefined') {
          this.snackbar.open('' + sessionStorage.getItem('httpError'), '', {
            duration: 10000
          });
        }
        else {
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
            this.gradeScaleList = []
            if (!res.gradeScaleList) {
              this.snackbar.open(res._message, '', {
                duration: 10000
              });
            }

          }
          else {
            this.gradeScaleList = res.gradeScaleList.filter(x => !x.useAsStandardGradeScale);
            let index = 0;
            if (this.gradeScaleList.length > 0) {
              this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationGradescale = this.gradeScaleList[index]?.grade.map((data) => {
                return ({
                  gradeScaleId: data.gradeScaleId,
                  gradeId: data.gradeId,
                  title: data.title
                })
              })
            }
          }
        }
      }
    );
  }

  viewGradebookConfiguration() {
    this.gradebookConfigurationAddViewModel.gradebookConfiguration.courseId = this.selectedCourseSection.courseId;
    this.gradebookConfigurationAddViewModel.gradebookConfiguration.courseSectionId = +this.selectedCourseSection.courseSectionId;
    this.gradeBookConfigurationService.viewGradebookConfiguration(this.gradebookConfigurationAddViewModel).subscribe(
      (res: GradebookConfigurationAddViewModel) => {
        if (res) {
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);

            this.populateFinalGrading();
          }
          else {
            this.gradebookConfigurationAddViewModel = res;
            this.generalChecbox = this.gradebookConfigurationAddViewModel.gradebookConfiguration.general;
            this.populateFinalGrading();
          }
        }
        else {
          this.snackbar.open(sessionStorage.getItem('httpError'), '', {
            duration: 10000
          });
          this.populateFinalGrading();
        }
      }
    );
  }


  addUpdateGradebookConfiguration() {

    if (this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationQuarter.length > 0) {

      for (let quater of this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationQuarter) {

        if (quater.examPercentage + quater.gradingPercentage > 100) {
          this.snackbar.open(quater.title + " total marks schould be less than or equal to 100", '', {
            duration: 10000
          });
          return
        }
        else {
          if (quater.examPercentage + quater.gradingPercentage === 100) {
            this.qtotalNot100 = false;
          }
          else {
            this.qtotalNot100 = true;
          }

        }
      }
    }
    if (this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationSemester.length > 0) {
      for (let semester of this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationSemester) {
        for (let quaterForSemester of this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationSemester) {
          if (quaterForSemester?.qtrMarkingPeriodId && quaterForSemester?.doesGrades && quaterForSemester?.smstrMarkingPeriodId === semester?.smstrMarkingPeriodId) {
            if (semester.examPercentage + quaterForSemester.gradingPercentage > 100) {
              this.snackbar.open(semester.title + " total marks schould be less than or equal to 100", '', {
                duration: 10000
              });
              return
            }
            else {
              if (semester.examPercentage + quaterForSemester.gradingPercentage === 100) {
                this.stotalNot100 = false;
              }
              else {
                this.stotalNot100 = true;
              }

            }
          }
        }

      }
    }
    if (this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationYear.length > 0) {
      for (let year of this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationYear) {
        for (let semesterForYear of this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationYear) {
          if (semesterForYear?.smstrMarkingPeriodId && semesterForYear?.doesGrades && semesterForYear?.yrMarkingPeriodId === year?.yrMarkingPeriodId) {
            if (year.examPercentage + semesterForYear.gradingPercentage > 100) {
              this.snackbar.open(year.title + " total marks schould be less than or equal to 100", '', {
                duration: 10000
              });
              return
            }
          }
        }
      }
    }



    this.gradebookConfigurationAddViewModel.gradebookConfiguration.general = this.generalChecbox;
    this.gradebookConfigurationAddViewModel.gradebookConfiguration.courseId = this.selectedCourseSection.courseId;
    this.gradebookConfigurationAddViewModel.gradebookConfiguration.courseSectionId = +this.selectedCourseSection.courseSectionId;
    this.gradeBookConfigurationService.addUpdateGradebookConfiguration(this.gradebookConfigurationAddViewModel).subscribe(
      (res: GradebookConfigurationAddViewModel) => {
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
            this.gradebookConfigurationAddViewModel = res;
            this.populateFinalGrading();
          }
        }
        else {
          this.snackbar.open(sessionStorage.getItem('httpError'), '', {
            duration: 10000
          });
        }
      }
    );
  }

  checkConfiguration(type, index) {

    if (this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationQuarter.length > 0 && type === 'quater') {
          const quater =  this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationQuarter[index];
        if (quater.examPercentage + quater.gradingPercentage === 100) {
           return false;
          }
          else {
            return true;
          }
    }
    if (this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationSemester.length > 0 && type === 'semester') {
      const semester = this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationSemester[index];
      let totalQuaterGrade = 0;
      for (let quaterForSemester of this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationSemester) {
          if (quaterForSemester?.qtrMarkingPeriodId && quaterForSemester?.doesGrades && quaterForSemester?.smstrMarkingPeriodId === semester?.smstrMarkingPeriodId) {
            totalQuaterGrade += quaterForSemester.gradingPercentage;
          }
        }
        if (semester.examPercentage + totalQuaterGrade === 100) {
          return false;
        }
        else {
          return true;
        }
    }
    if (this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationYear.length > 0 && type === 'year') {
      const year = this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationYear[index];
      let totalSemesterGrade = 0;
      for (let semesterForYear of this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationYear) {
          if (semesterForYear?.smstrMarkingPeriodId && semesterForYear?.doesGrades && semesterForYear?.yrMarkingPeriodId === year?.yrMarkingPeriodId) {
            totalSemesterGrade += semesterForYear.gradingPercentage;
          }
        }
        if (year.examPercentage + totalSemesterGrade === 100) {
          return false;
        }
        else {
          return true;
        }
    }
  }

}
