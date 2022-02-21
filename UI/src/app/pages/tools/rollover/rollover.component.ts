import { Component, OnInit, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import icWarning from '@iconify/icons-ic/warning';
import { FinalGradingMarkingPeriodList } from '../../../models/gradebook-configuration.model';
import { GradeBookConfigurationService } from '../../../services/gradebook-configuration.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DefaultValuesService } from '../../../common/default-values.service';
import { CommonService } from '../../../services/common.service';
import { RolloverViewModel } from '../../../models/roll-over.model';
import { TableProgressPeriod, TableQuarter, TableSchoolSemester } from '../../../models/marking-period.model';
import { RollOverService } from '../../../services/roll-over.service';
import { NgForm } from '@angular/forms';
import { SharedFunction } from '../../shared/shared-function';
import { LoaderService } from '../../../services/loader.service';
import * as moment from 'moment';
import { SchoolService } from 'src/app/services/school.service';
import { takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { GetAcademicYearListModel } from 'src/app/models/marking-period.model';
import { MarkingPeriodService } from 'src/app/services/marking-period.service';

@Component({
  selector: 'vex-rollover-processing',
  templateUrl: './rollover.component.html',
  styleUrls: ['./rollover.component.scss']
})
export class RolloverComponent implements OnInit {

  icWarning = icWarning;
  loading: boolean;
  @ViewChild('f') currentForm: NgForm;
  f: NgForm;
  minSchoolBeginDateVal: Date;
  finalGradingMarkingPeriodList: FinalGradingMarkingPeriodList = new FinalGradingMarkingPeriodList();
  rolloverViewModel: RolloverViewModel = new RolloverViewModel();
  showRollOver: boolean = false;
  showUploadBtn: boolean
  destroySubject$: Subject<void> = new Subject();
  getAcademicYears: GetAcademicYearListModel = new GetAcademicYearListModel();
  constructor(public translateService: TranslateService,
    private gradeBookConfigurationService: GradeBookConfigurationService,
    private snackbar: MatSnackBar,
    private commonFunction: SharedFunction,
    private defaultValuesService: DefaultValuesService,
    private rollOverService: RollOverService,
    private loaderService: LoaderService,
    private commonService: CommonService,
    private schoolService: SchoolService,
    private markingPeriodService: MarkingPeriodService,
    private defaultValueService:DefaultValuesService,
    ) {
    this.loaderService.isLoading.subscribe((v) => {
      this.loading = v;
    });
    this.schoolService.schoolListCalled.pipe(takeUntil(this.destroySubject$)).subscribe((res) => {
      if (res.academicYearChanged || res.academicYearLoaded) {
        this.checkCurrentAcademicYearIsMaxOrNot(this.defaultValueService.getAcademicYear())
      }
    })
  }

  ngOnInit(): void {
    this.populateFinalGrading();
    this.minSchoolBeginDateVal = moment(this.defaultValuesService.getFullYearEndDate()).add(1, 'days').toDate();
  }

  checkCurrentAcademicYearIsMaxOrNot(selectedYear:any) {
    let result = false
    let maxArr = []
    this.getAcademicYears.schoolId = this.defaultValueService.getSchoolID();
    this.markingPeriodService.getAcademicYearList(this.getAcademicYears).subscribe((res:any) => {
      if(res._failure) {

      } else {
        res.academicYears.forEach(element => {
          maxArr.push(element.academyYear)
        });
      }
      let maxYear = Math.max(...maxArr)
      if(selectedYear = maxYear || selectedYear < maxYear) {
        res.academicYears.forEach(value => {
          if(maxYear==value.academyYear) {
            result = moment(new Date()).isBetween(value.startDate, value.endDate)
            this.showRollOver = !result;
            this.showUploadBtn = this.showRollOver
          }
        })
      }
    })
  }

  getMinDateValue() {
    return moment(this.rolloverViewModel.schoolRollover.schoolBeginDate).add(1, 'days').toDate();
  }

  populateFinalGrading() {
    this.gradeBookConfigurationService.populateFinalGrading(this.finalGradingMarkingPeriodList).subscribe(
      (res: FinalGradingMarkingPeriodList) => {
        if (res) {
          if (res._failure) {
            this.commonService.checkTokenValidOrNot(res._message);
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
          else {
            this.finalGradingMarkingPeriodList = res;
            if (this.finalGradingMarkingPeriodList.schoolYears) {


              this.rolloverViewModel.schoolRollover.schoolBeginDate = null;
              this.rolloverViewModel.schoolRollover.schoolEndDate = null;
              this.rolloverViewModel.fullYearName = this.finalGradingMarkingPeriodList.schoolYears.title;
              this.rolloverViewModel.fullYearShortName = this.finalGradingMarkingPeriodList.schoolYears.shortName;
              this.rolloverViewModel.doesComments = this.finalGradingMarkingPeriodList.schoolYears.doesComments;
              this.rolloverViewModel.doesExam = this.finalGradingMarkingPeriodList.schoolYears.doesExam;
              this.rolloverViewModel.doesGrades = this.finalGradingMarkingPeriodList.schoolYears.doesGrades;

              this.finalGradingMarkingPeriodList.schoolYears.semesters.map((semester, i) => {
                this.rolloverViewModel.semesters.push(new TableSchoolSemester());
                this.rolloverViewModel.semesters[i + 1].yearId = semester.yearId;
                this.rolloverViewModel.semesters[i + 1].markingPeriodId = semester.markingPeriodId;
                this.rolloverViewModel.semesters[i + 1].startDate = null;
                this.rolloverViewModel.semesters[i + 1].endDate = null;
                this.rolloverViewModel.semesters[i + 1].title = semester.title;
                this.rolloverViewModel.semesters[i + 1].title = semester.title;
                this.rolloverViewModel.semesters[i + 1].shortName = semester.shortName;
                this.rolloverViewModel.semesters[i + 1].doesExam = semester.doesExam;
                this.rolloverViewModel.semesters[i + 1].doesGrades = semester.doesGrades;
                this.rolloverViewModel.semesters[i + 1].doesComments = semester.doesComments;
                this.rolloverViewModel.semesters[i + 1].quarters = [];


                semester.quarters.map((quater, index) => {
                  this.rolloverViewModel.semesters[i + 1].quarters.push(new TableQuarter());
                  let lastIndex = this.rolloverViewModel.semesters[i + 1].quarters.length - 1;
                  this.rolloverViewModel.semesters[i + 1].quarters[lastIndex].markingPeriodId = quater.markingPeriodId
                  this.rolloverViewModel.semesters[i + 1].quarters[lastIndex].startDate = null;
                  this.rolloverViewModel.semesters[i + 1].quarters[lastIndex].endDate = null;
                  this.rolloverViewModel.semesters[i + 1].quarters[lastIndex].shortName = quater.shortName;
                  this.rolloverViewModel.semesters[i + 1].quarters[lastIndex].doesComments = quater.doesComments;
                  this.rolloverViewModel.semesters[i + 1].quarters[lastIndex].doesExam = quater.doesExam;
                  this.rolloverViewModel.semesters[i + 1].quarters[lastIndex].doesGrades = quater.doesGrades;
                  this.rolloverViewModel.semesters[i + 1].quarters[lastIndex].title = quater.title;
                  this.rolloverViewModel.semesters[i + 1].quarters[lastIndex].semesterId = quater.semesterId;

                  this.rolloverViewModel.semesters[i + 1].quarters[lastIndex].progressPeriods = [];

                  quater.progressPeriods?.map((period, index) => {
                    this.rolloverViewModel.semesters[i + 1].quarters[lastIndex].progressPeriods.push(new TableProgressPeriod());
                    const lastPeriodIndex = this.rolloverViewModel.semesters[i + 1].quarters[lastIndex].progressPeriods.length - 1;
                    this.rolloverViewModel.semesters[i + 1].quarters[lastIndex].progressPeriods[lastPeriodIndex].markingPeriodId = period.markingPeriodId
                    this.rolloverViewModel.semesters[i + 1].quarters[lastIndex].progressPeriods[lastPeriodIndex].startDate = null;
                    this.rolloverViewModel.semesters[i + 1].quarters[lastIndex].progressPeriods[lastPeriodIndex].endDate = null;
                    this.rolloverViewModel.semesters[i + 1].quarters[lastIndex].progressPeriods[lastPeriodIndex].shortName = period.shortName;
                    this.rolloverViewModel.semesters[i + 1].quarters[lastIndex].progressPeriods[lastPeriodIndex].doesComments = period.doesComments;
                    this.rolloverViewModel.semesters[i + 1].quarters[lastIndex].progressPeriods[lastPeriodIndex].doesExam = period.doesExam;
                    this.rolloverViewModel.semesters[i + 1].quarters[lastIndex].progressPeriods[lastPeriodIndex].doesGrades = period.doesGrades;
                    this.rolloverViewModel.semesters[i + 1].quarters[lastIndex].progressPeriods[lastPeriodIndex].title = period.title;
                    this.rolloverViewModel.semesters[i + 1].quarters[lastIndex].progressPeriods[lastPeriodIndex].quarterId = period.quarterId;

                  })

                })
              })
              this.rolloverViewModel.semesters.shift();
            }
            else{
              this.rolloverViewModel.semesters=[];
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

  rollOver() {
    this.rolloverViewModel.schoolRollover.reenrollmentDate = this.commonFunction.formatDateSaveWithoutTime(this.rolloverViewModel.schoolRollover.reenrollmentDate);
    this.rolloverViewModel.schoolRollover.schoolBeginDate = this.commonFunction.formatDateSaveWithoutTime(this.rolloverViewModel.schoolRollover.schoolBeginDate);
    this.rolloverViewModel.schoolRollover.schoolEndDate = this.commonFunction.formatDateSaveWithoutTime(this.rolloverViewModel.schoolRollover.schoolEndDate);

    for (let semester of this.rolloverViewModel.semesters) {
      semester.startDate = this.commonFunction.formatDateSaveWithoutTime(semester.startDate);
      semester.endDate = this.commonFunction.formatDateSaveWithoutTime(semester.endDate);

      for (let quater of semester.quarters) {
        quater.startDate = this.commonFunction.formatDateSaveWithoutTime(quater.startDate);
        quater.endDate = this.commonFunction.formatDateSaveWithoutTime(quater.endDate);

        for (let period of quater.progressPeriods) {
          period.startDate = this.commonFunction.formatDateSaveWithoutTime(period.startDate);
          period.endDate = this.commonFunction.formatDateSaveWithoutTime(period.endDate);
        }
      }
    }
    this.rolloverViewModel.schoolRollover.rolloverContent = JSON.stringify(this.rolloverViewModel);
    this.currentForm.form.markAllAsTouched();
    if (this.currentForm.form.valid) {
      this.rollOverService.rollover(this.rolloverViewModel).subscribe(
        (res: RolloverViewModel) => {
          if (res) {
            if (res._failure) {
              this.commonService.checkTokenValidOrNot(res._message);
              this.snackbar.open(res._message, '', {
                duration: 10000
              });
            }
            else {
              this.snackbar.open(res._message, '', {
                duration: 10000
              });
            }
          }
          else {
            this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
              duration: 10000
            });
          }
        })
    }


  }
}
