import { ConstantPool, isNgContent } from '@angular/compiler';
import { Component, Input, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CommonService } from 'src/app/services/common.service';
import { FinalGradingMarkingPeriodList, GradebookConfigurationAddViewModel, GradebookConfigurationProgressPeriods, GradebookConfigurationQuarter, GradebookConfigurationSemester, GradebookConfigurationYear } from '../../../models/gradebook-configuration.model';
import { GradeScaleListView } from '../../../models/grades.model';
import { GradeBookConfigurationService } from '../../../services/gradebook-configuration.service';
import { GradesService } from '../../../services/grades.service';
import { DefaultValuesService } from 'src/app/common/default-values.service';
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
  ptotalNot100: boolean = false;
  qtotalNot100: boolean = false;
  stotalNot100: boolean = false;
  ytotalNot100: boolean = false;
  isNotGraded: boolean;
  @Input() courseWeightedFlag: boolean;

  constructor(private gradeBookConfigurationService: GradeBookConfigurationService,
    private gradesService: GradesService,
    private snackbar: MatSnackBar,
    public defaultValuesService: DefaultValuesService,
    private commonService: CommonService
  ) {
    this.gradebookConfigurationAddViewModel.gradebookConfiguration.general = '';
    this.selectedCourseSection = this.defaultValuesService.getSelectedCourseSection();
  }

  ngOnInit(): void {
    if (this.selectedCourseSection?.gradeScaleType !== 'Ungraded') {
      this.isNotGraded = false;
      this.getAllGradeScale(0);
    } else {
      this.isNotGraded = true;
    }
  }


  populateFinalGrading() {
    this.finalGradingMarkingPeriodList.courseSectionId=+this.selectedCourseSection.courseSectionId;
    this.finalGradingMarkingPeriodList.isConfiguration=true;
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
            // For progressPeriods
          if (this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationProgressPeriods.length > 0) {
            this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationProgressPeriods.map((item) => {
              this.finalGradingMarkingPeriodList.progressPeriods.map((subItem, index) => {
                if (item.prgrsprdMarkingPeriodId === subItem.markingPeriodId) {
                  item.title = subItem.title;
                  item.doesExam = subItem.doesExam;
                  item.doesGrades = subItem.doesGrades;
                }
              })
            });
          }
          else {
            this.finalGradingMarkingPeriodList.progressPeriods = this.finalGradingMarkingPeriodList.progressPeriods.map((item, index) => {
              this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationProgressPeriods.push(new GradebookConfigurationProgressPeriods());
              this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationProgressPeriods[index].prgrsprdMarkingPeriodId = item.markingPeriodId;
              this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationProgressPeriods[index].title = item.title;
              this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationProgressPeriods[index].doesExam = item.doesExam;
              this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationProgressPeriods[index].doesGrades = item.doesGrades;
              this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationProgressPeriods[index].gradingPercentage = 0;
              this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationProgressPeriods[index].examPercentage = 0;
              return item;
            })
            this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationProgressPeriods.pop();
          }
          
          //For quarters
            if (this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationQuarter.length > 0) {
              this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationQuarter.map((item)=>{
                this.finalGradingMarkingPeriodList.quarters.map((subItem, index) => {
                  if(!item.prgrsprdMarkingPeriodId && item.qtrMarkingPeriodId === subItem.markingPeriodId) {
                    item.title = subItem.title;
                    item.doesExam = subItem.doesExam;
                    item.doesGrades = subItem.doesGrades;
                    item.isProgressPeriodExists = subItem.progressPeriods.length > 0;
                  }

                  subItem.progressPeriods.map((progressPeriod) => {
                    if (item.prgrsprdMarkingPeriodId && item.prgrsprdMarkingPeriodId === progressPeriod.markingPeriodId) {
                      item.title = progressPeriod.title;
                      item.doesExam = progressPeriod.doesExam;
                      item.doesGrades = progressPeriod.doesGrades;
                      item.isProgressPeriodExists = subItem.progressPeriods.length > 0;
                    }
                  });

                })
              });
            }
            else {
                this.finalGradingMarkingPeriodList.quarters.map((item, index) => {
                this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationQuarter.push(new GradebookConfigurationQuarter());
                this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationQuarter[index].qtrMarkingPeriodId = item.markingPeriodId;
                this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationQuarter[index].title = item.title;
                this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationQuarter[index].doesExam = item.doesExam;
                this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationQuarter[index].doesGrades = item.doesGrades;
                this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationQuarter[index].gradingPercentage = 0;
                this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationQuarter[index].examPercentage = 0;
                this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationQuarter[index].isProgressPeriodExists = item.progressPeriods.length > 0;

                item.progressPeriods.map((progressPeriod, index) => {
                  this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationQuarter.push(new GradebookConfigurationQuarter());
                  const lastIndex = this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationQuarter.length - 1;
                  this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationQuarter[lastIndex].qtrMarkingPeriodId = item.markingPeriodId;
                  this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationQuarter[lastIndex].prgrsprdMarkingPeriodId = progressPeriod.markingPeriodId;
                  this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationQuarter[lastIndex].title = progressPeriod.title;
                  this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationQuarter[lastIndex].doesExam = progressPeriod.doesExam;
                  this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationQuarter[lastIndex].doesGrades = progressPeriod.doesGrades;
                  this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationQuarter[lastIndex].gradingPercentage = 0;
                  this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationQuarter[lastIndex].examPercentage = 0;
                  this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationQuarter[lastIndex].isProgressPeriodExists = item.progressPeriods.length > 0;
                })
              })
              // this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationQuarter.pop();
            }

            // For semesters
            if (this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationSemester.length > 0) {

              this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationSemester.map((item: any, i) => {
                this.finalGradingMarkingPeriodList.semesters.map((subitem: any, index) => {
                  if (!item.qtrMarkingPeriodId && item.smstrMarkingPeriodId === subitem.markingPeriodId) {
                    item.title = subitem.title;
                    item.doesExam = subitem.doesExam;
                    item.doesGrades = subitem.doesGrades;
                    item.isQuarterExists = subitem.quarters.length > 0;
                  }

                  subitem.quarters.map((quater) => {
                    if (item.qtrMarkingPeriodId && item.qtrMarkingPeriodId === quater.markingPeriodId) {
                      item.title = quater.title;
                      item.doesExam = quater.doesExam;
                      item.doesGrades = quater.doesGrades;
                      item.isQuarterExists = subitem.quarters.length > 0;
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
                this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationSemester[i].isQuarterExists = item.quarters.length > 0;

                
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
                  this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationSemester[lastIndex].isQuarterExists = item.quarters.length > 0;

                })

              })
            }

            // For schoolYears
            if (this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationYear.length > 0) {
              if (this.finalGradingMarkingPeriodList.schoolYears) {
                this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationYear.map((item: any, i) => {
                  if (item.yrMarkingPeriodId === this.finalGradingMarkingPeriodList.schoolYears.markingPeriodId && !item.smstrMarkingPeriodId) {
                    item.title = this.finalGradingMarkingPeriodList.schoolYears.title;
                    item.doesExam = this.finalGradingMarkingPeriodList.schoolYears.doesExam;
                    item.doesGrades = this.finalGradingMarkingPeriodList.schoolYears.doesGrades;
                    item.isSemesterExists = this.finalGradingMarkingPeriodList.schoolYears.semesters.length > 0;

                  } else if (item.smstrMarkingPeriodId) {
                    this.finalGradingMarkingPeriodList.schoolYears.semesters.map((semester, i) => {
                      if (item.smstrMarkingPeriodId === semester.markingPeriodId) {
                        item.title = semester.title;
                        item.doesExam = semester.doesExam;
                        item.doesGrades = semester.doesGrades;
                        item.isSemesterExists = this.finalGradingMarkingPeriodList.schoolYears.semesters.length > 0;
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
                this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationYear[0].isSemesterExists = this.finalGradingMarkingPeriodList.schoolYears.semesters.length > 0;


                this.finalGradingMarkingPeriodList.schoolYears.semesters.map((semester, i) => {
                  this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationYear.push(new GradebookConfigurationYear());
                  this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationYear[i + 1].yrMarkingPeriodId = this.finalGradingMarkingPeriodList.schoolYears.markingPeriodId;
                  this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationYear[i + 1].smstrMarkingPeriodId = semester.markingPeriodId;
                  this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationYear[i + 1].title = semester.title;
                  this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationYear[i + 1].doesExam = semester.doesExam;
                  this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationYear[i + 1].doesGrades = semester.doesGrades;
                  this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationYear[i + 1].gradingPercentage = 0;
                  this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationYear[i + 1].examPercentage = 0;
                  this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationYear[i + 1].isSemesterExists = this.finalGradingMarkingPeriodList.schoolYears.semesters.length > 0;

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
          this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
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
      this.generalChecbox = this.generalChecbox.toString().replace(',', '|');
    }
    else {
      this.generalChecbox = this.generalEvent.toString();
    }

  }

  getAllGradeScale(selectedGradeScaleId: number) {
    this.gradesService.getAllGradeScaleList(this.gradeScaleListView).subscribe(
      (res: GradeScaleListView) => {

        if (typeof (res) === 'undefined') {
          this.snackbar.open('' + this.defaultValuesService.getHttpError(), '', {
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
            this.viewGradebookConfiguration();
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
            this.viewGradebookConfiguration();
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
            if(this.generalChecbox){
              this.generalEvent= this.gradebookConfigurationAddViewModel.gradebookConfiguration.general.split('|');
            }
            this.populateFinalGrading();
          }
        }
        else {
          this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
          this.populateFinalGrading();
        }
      }
    );
  }


  addUpdateGradebookConfiguration() {
    if (this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationProgressPeriods.length > 0) {

      for (let progressPeriod of this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationProgressPeriods) {

        if (progressPeriod.examPercentage + progressPeriod.gradingPercentage > 100) {
          this.snackbar.open(progressPeriod.title + " total marks schould be less than or equal to 100", '', {
            duration: 10000
          });
          return
        }
        else {
          if (progressPeriod.examPercentage + progressPeriod.gradingPercentage === 100) {
            this.ptotalNot100 = false;
          }
          else {
            this.ptotalNot100 = true;
          }
        }
      }
    }

    if (this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationQuarter.length > 0) {

      for (let quater of this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationQuarter) {
        for (let progressPeriodForQuater of this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationQuarter) {
          if (progressPeriodForQuater?.prgrsprdMarkingPeriodId && progressPeriodForQuater?.doesGrades && progressPeriodForQuater?.qtrMarkingPeriodId === quater?.qtrMarkingPeriodId) {
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
    delete this.gradebookConfigurationAddViewModel.gradebookConfiguration.academicYear;
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
          this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
      }
    );
  }

  checkConfiguration(type, index) {

    if (this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationProgressPeriods.length > 0 && type === 'progressPeriods') {
      const progressPeriods = this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationProgressPeriods[index];
      if (progressPeriods.examPercentage + progressPeriods.gradingPercentage === 100) {
        return false;
      }
      else {
        return true;
      }
    }

    if (this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationQuarter.length > 0 && type === 'quater') {
          const quater =  this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationQuarter[index];
          let totalQuaterGrade = 0;
          for (let progressPeriodForQuater of this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationQuarter) {
            if (progressPeriodForQuater?.prgrsprdMarkingPeriodId && progressPeriodForQuater?.doesGrades && progressPeriodForQuater?.qtrMarkingPeriodId === quater?.qtrMarkingPeriodId) {
              totalQuaterGrade += progressPeriodForQuater.gradingPercentage;
            }
          }
        totalQuaterGrade = quater.isProgressPeriodExists ? totalQuaterGrade : quater.gradingPercentage;
        
        if (quater.examPercentage + totalQuaterGrade === 100) {
          return false;
          }
          else {
            return true;
          }
    }
    if (this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationSemester.length > 0 && type === 'semester') {
      const semester = this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationSemester[index];
      let totalSemesterGrade = 0;
      for (let quaterForSemester of this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationSemester) {
          if (quaterForSemester?.qtrMarkingPeriodId && quaterForSemester?.doesGrades && quaterForSemester?.smstrMarkingPeriodId === semester?.smstrMarkingPeriodId) {
            totalSemesterGrade += quaterForSemester.gradingPercentage;
          }
        }
        totalSemesterGrade = semester.isQuarterExists ? totalSemesterGrade : semester.gradingPercentage;
        if (semester.examPercentage + totalSemesterGrade === 100) {
          return false;
        }
        else {
          return true;
        }
    }
    if (this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationYear.length > 0 && type === 'year') {
      const year = this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationYear[index];
      let totalYearGrade = 0;
      for (let semesterForYear of this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationYear) {
          if (semesterForYear?.smstrMarkingPeriodId && semesterForYear?.doesGrades && semesterForYear?.yrMarkingPeriodId === year?.yrMarkingPeriodId) {
            totalYearGrade += semesterForYear.gradingPercentage;
          }
        }
        totalYearGrade = year.isSemesterExists ? totalYearGrade : year.gradingPercentage;

        if (year.examPercentage + totalYearGrade === 100) {
          return false;
        }
        else {
          return true;
        }
    }
  }

}
