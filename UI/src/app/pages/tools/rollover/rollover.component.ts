import { Component, OnInit, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import icWarning from '@iconify/icons-ic/warning';
import { FinalGradingMarkingPeriodList } from '../../../models/gradebook-configuration.model';
import { GradeBookConfigurationService } from '../../../services/gradebook-configuration.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DefaultValuesService } from '../../../common/default-values.service';
import { CommonService } from '../../../services/common.service';
import { RolloverReadinessViewModel, RolloverViewModel } from '../../../models/roll-over.model';
import { GetAcademicYearListModel, TableProgressPeriod, TableQuarter, TableSchoolSemester } from '../../../models/marking-period.model';
import { RollOverService } from '../../../services/roll-over.service';
import { NgForm } from '@angular/forms';
import { SharedFunction } from '../../shared/shared-function';
import { LoaderService } from '../../../services/loader.service';
import * as moment from 'moment';
import { MarkingPeriodService } from 'src/app/services/marking-period.service';
import { MatDialog } from '@angular/material/dialog';
import { RolloverSummaryDialogComponent } from './rollover-summary-dialog/rollover-summary-dialog.component';

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
  maxSchoolBeginDateVal: Date;
  minSchoolEndDateVal1: Date;
  minSchoolEndDateVal2: Date;
  maxSchoolEndDateVal: Date;
  currentSchoolEndDateVal: Date;
  getAcademicYears: GetAcademicYearListModel = new GetAcademicYearListModel();
  showRollOver: boolean = false;
  showDevAutofill: boolean = false;
  generatingPreflight: boolean = false;
  rolloverNotYetAvailable: boolean = false;
  maxYearEndDate: Date;
  finalGradingMarkingPeriodList: FinalGradingMarkingPeriodList = new FinalGradingMarkingPeriodList();
  rolloverViewModel: RolloverViewModel = new RolloverViewModel();
  constructor(public translateService: TranslateService,
    private gradeBookConfigurationService: GradeBookConfigurationService,
    private snackbar: MatSnackBar,
    private commonFunction: SharedFunction,
    public defaultValuesService: DefaultValuesService,
    private rollOverService: RollOverService,
    private markingPeriodService: MarkingPeriodService,
    private loaderService: LoaderService,
    private dialog: MatDialog,
    private commonService: CommonService) {
    this.loaderService.isLoading.subscribe((v) => {
      this.loading = v;
    });
  }

  ngOnInit(): void {
    // Testing helper is only offered on local development hosts — deployed
    // tenants run on real domains behind the reverse proxy and never match.
    const host = window.location.hostname;
    this.showDevAutofill = host === 'localhost' || host === '127.0.0.1'
      || host === 'lvh.me' || host.endsWith('.lvh.me');

    this.checkCurrentAcademicYearIsMaxOrNot(this.defaultValuesService.getAcademicYear())
    // this.minSchoolBeginDateVal = moment(this.defaultValuesService.getFullYearEndDate()).add(1, 'days').toDate();
    // this.maxSchoolEndDateVal = moment(this.defaultValuesService.getFullYearEndDate()).add(366, 'days').toDate(); 
    //366 days because in begin date calculation starting from after 1 day

    this.currentSchoolEndDateVal = moment(this.defaultValuesService.getFullYearEndDate()).toDate();
    const startDate = moment(this.defaultValuesService.getFullYearStartDate()).add(365, 'days').toDate();
    const fullYearEndDate = moment(this.defaultValuesService.getFullYearEndDate()).toDate();
    const diffDays = Math.abs(moment(fullYearEndDate).diff(moment(startDate), 'days'));
    this.minSchoolBeginDateVal = moment(moment(startDate).subtract(diffDays > 60 ? diffDays : 60, 'days').toDate()).startOf('month').toDate();
    this.maxSchoolBeginDateVal = moment(moment(startDate).add(1, 'month').toDate()).endOf('month').toDate();
    this.minSchoolEndDateVal1 = moment(this.minSchoolBeginDateVal).add(364, 'days').toDate();
    this.minSchoolEndDateVal2 = moment(this.maxSchoolBeginDateVal).add(364, 'days').toDate();
  }

  schoolBeginDateChange() {
    this.maxSchoolEndDateVal = moment(this.rolloverViewModel.schoolRollover.schoolBeginDate).add(364, 'days').toDate();
  }

  checkCurrentAcademicYearIsMaxOrNot(selectedYear: any) {
    let maxArr = []
    this.getAcademicYears.schoolId = this.defaultValuesService.getSchoolID();
    this.markingPeriodService.getAcademicYearList(this.getAcademicYears).subscribe((res: any) => {
      if (res._failure) { }
      else
        res.academicYears.forEach(element => {
          maxArr.push(element.academyYear)
        });
      let maxYear = Math.max(...maxArr)
      if (selectedYear == maxYear || selectedYear < maxYear)
        res.academicYears.forEach(value => {
          if (maxYear == value.academyYear) {
            const todayInsideMaxYear = moment(new Date()).isBetween(value.startDate, value.endDate);
            this.showRollOver = !todayInsideMaxYear;
            if (todayInsideMaxYear) {
              // Today falls inside the newest year's range. Two distinct
              // states: if the newest year is still the selected year, the
              // year is simply in progress and rollover opens after its end
              // date; if a newer year exists, a rollover already created it.
              this.rolloverNotYetAvailable = selectedYear == maxYear;
              this.maxYearEndDate = moment(value.endDate).toDate();
            }
          }
        })
      if (this.showRollOver)
        this.populateFinalGrading();
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
            else {
              this.rolloverViewModel.semesters = [];
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
      this.openPreflightSummary();
    }


  }

  // Fetches the read-only completeness summary and shows it in a confirmation
  // modal. A failed fetch never blocks the rollover: the modal still opens with
  // an error banner and the operator decides. In review-only mode (available
  // during the administrative period before the calendar year ends) the dialog
  // offers no confirm button — it is purely informational.
  private openPreflightSummary(reviewOnly: boolean = false) {
    this.generatingPreflight = true;
    this.rollOverService.getPreflightSummary(new RolloverReadinessViewModel()).subscribe(
      (res: RolloverReadinessViewModel) => {
        this.generatingPreflight = false;
        if (res && res._failure) {
          this.commonService.checkTokenValidOrNot(res._message);
        }
        this.openSummaryDialog(res, !res || res._failure === true, reviewOnly);
      },
      () => {
        this.generatingPreflight = false;
        this.openSummaryDialog(null, true, reviewOnly);
      });
  }

  openPreflightReview() {
    this.openPreflightSummary(true);
  }

  private openSummaryDialog(summary: RolloverReadinessViewModel, loadFailed: boolean, reviewOnly: boolean = false) {
    const dialogRef = this.dialog.open(RolloverSummaryDialogComponent, {
      width: '900px',
      maxWidth: '95vw',
      maxHeight: '90vh',
      data: {
        summary,
        academicYear: this.defaultValuesService.getFullAcademicYear(),
        loadFailed,
        reviewOnly
      }
    });
    dialogRef.afterClosed().subscribe(confirmed => {
      if (confirmed && !reviewOnly) {
        this.doRollover();
      }
    });
  }

  private doRollover() {
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
            // Reload so the toolbar re-fetches the academic year list and the
            // new school year appears in the dropdown without re-login. The
            // session keeps the current year selected; the operator switches
            // to the new year when ready.
            setTimeout(() => window.location.reload(), 3000);
          }
        }
        else {
          this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
      })
  }

  cancel() {
    this.currentForm.resetForm();
  }

  // Testing helper (local dev hosts only): fills every date with the current
  // year's marking period dates shifted one year forward. The dates still run
  // through the same pickers/validators — the tester must verify or replace
  // them before actually rolling over.
  autofillDatesForTesting() {
    const source = this.finalGradingMarkingPeriodList.schoolYears;
    if (!source) {
      return;
    }
    const plusOneYear = (date: any): any => date ? moment(date).add(1, 'year').toDate() : null;

    this.rolloverViewModel.schoolRollover.schoolBeginDate = plusOneYear(this.defaultValuesService.getFullYearStartDate());
    this.schoolBeginDateChange();
    const proposedEnd = moment(this.defaultValuesService.getFullYearEndDate()).add(1, 'year');
    const maxEnd = moment(this.maxSchoolEndDateVal);
    this.rolloverViewModel.schoolRollover.schoolEndDate = (proposedEnd.isAfter(maxEnd) ? maxEnd : proposedEnd).toDate() as any;
    this.rolloverViewModel.schoolRollover.reenrollmentDate = this.rolloverViewModel.schoolRollover.schoolBeginDate;

    source.semesters?.forEach((semester, i) => {
      const targetSemester = this.rolloverViewModel.semesters[i];
      if (!targetSemester) {
        return;
      }
      targetSemester.startDate = plusOneYear(semester.startDate);
      targetSemester.endDate = plusOneYear(semester.endDate);
      semester.quarters?.forEach((quarter, j) => {
        const targetQuarter = targetSemester.quarters[j];
        if (!targetQuarter) {
          return;
        }
        targetQuarter.startDate = plusOneYear(quarter.startDate);
        targetQuarter.endDate = plusOneYear(quarter.endDate);
        quarter.progressPeriods?.forEach((period, k) => {
          const targetPeriod = targetQuarter.progressPeriods[k];
          if (!targetPeriod) {
            return;
          }
          targetPeriod.startDate = plusOneYear(period.startDate);
          targetPeriod.endDate = plusOneYear(period.endDate);
        });
      });
    });
  }
}
