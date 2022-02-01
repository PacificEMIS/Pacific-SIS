import { Component, Inject, OnInit } from '@angular/core';
import icClose from '@iconify/icons-ic/twotone-close';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MomentDateAdapter, MAT_MOMENT_DATE_ADAPTER_OPTIONS } from '@angular/material-moment-adapter';
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material/core';
import { MatDatepicker } from '@angular/material/datepicker';
import * as _moment from 'moment';
import { default as _rollupMoment, Moment } from 'moment';
import { HistoricalMarkingPeriodAddViewModel } from 'src/app/models/historical-marking-period.model';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CommonService } from 'src/app/services/common.service';
import { HistoricalMarkingPeriodService } from 'src/app/services/historical-marking-period.service';
import { SharedFunction } from '../../../../pages/shared/shared-function'
const moment = _rollupMoment || _moment;

export const MY_FORMATS = {
  parse: {
    dateInput: 'MMMM YYYY',
  },
  display: {
    dateInput: 'MMMM YYYY',
    monthYearLabel: 'MMM YYYY',
    dateA11yLabel: 'LL',
    monthYearA11yLabel: 'MMMM YYYY',
  },
};

@Component({
  selector: 'vex-add-historical-marking-periods',
  templateUrl: './add-historical-marking-periods.component.html',
  styleUrls: ['./add-historical-marking-periods.component.scss'],
  providers: [
    {
      provide: DateAdapter,
      useClass: MomentDateAdapter,
    },

    { provide: MAT_DATE_FORMATS, useValue: MY_FORMATS },
  ],
})
export class AddHistoricalMarkingPeriodsComponent implements OnInit {
  historyTitle: string;
  icClose = icClose;
  date = new FormControl(moment());
  historicalMarkingPeriodAddViewModel: HistoricalMarkingPeriodAddViewModel = new HistoricalMarkingPeriodAddViewModel();
  form: FormGroup;
  editMode: boolean;
  constructor(@Inject(MAT_DIALOG_DATA) public data: any,
    private historicalMarkingPeriodService: HistoricalMarkingPeriodService,
    private defaultValuesService: DefaultValuesService,
    private commonFunction: SharedFunction,
    private dialogRef: MatDialogRef<AddHistoricalMarkingPeriodsComponent>,
    private commonService: CommonService,
    private snackbar: MatSnackBar,
    private fb: FormBuilder) {
    this.form = fb.group({
      histMarkingPeriodId: [0],
      title: ['', [Validators.required]],
      academicYear: [, [Validators.required]],
      gradePostDate: [, [Validators.required]]

    })
    if (data == null) {
      this.historyTitle = "addHistoricalMarkingPeriod";
      this.form.controls.gradePostDate.patchValue(moment());
    }
    else {
      this.editMode = true;
      this.historyTitle = "editHistoricalMarkingPeriod";
      this.date = new FormControl(moment(data.gradePostDate));
      this.historicalMarkingPeriodAddViewModel.historicalMarkingPeriod = data;
      this.form.controls.histMarkingPeriodId.patchValue(data.histMarkingPeriodId);
      this.form.controls.gradePostDate.patchValue(data.gradePostDate);
      this.form.controls.title.patchValue(data.title);
      this.form.controls.academicYear.patchValue(data.academicYear);
    }
  }

  ngOnInit(): void {
  }

  chosenYearHandler(normalizedYear: Moment) {
    const ctrlValue = this.date.value;
    ctrlValue.year(normalizedYear.year());
    this.date.setValue(ctrlValue);
    this.form.controls.gradePostDate.setValue(ctrlValue);
  }

  chosenMonthHandler(normalizedMonth: Moment, datepicker: MatDatepicker<Moment>) {
    const ctrlValue = this.date.value;
    ctrlValue.month(normalizedMonth.month());
    this.date.setValue(ctrlValue);
    this.form.controls.gradePostDate.setValue(ctrlValue);
    datepicker.close();
  }

  submit() {
    this.form.markAllAsTouched();
    if (this.form.valid) {
      this.historicalMarkingPeriodAddViewModel.historicalMarkingPeriod.title = this.form.controls.title.value;
      this.historicalMarkingPeriodAddViewModel.historicalMarkingPeriod.academicYear = this.form.controls.academicYear.value;
      this.historicalMarkingPeriodAddViewModel.historicalMarkingPeriod.gradePostDate = moment(this.form.controls.gradePostDate.value).format('YYYY-MM-DD');
      if (this.form.controls.histMarkingPeriodId.value === 0) {
        this.historicalMarkingPeriodService.addHistoricalMarkingPeriod(this.historicalMarkingPeriodAddViewModel).subscribe(
          (res) => {
            if (typeof (res) === 'undefined') {
              this.snackbar.open('Room list failed. ' + this.defaultValuesService.getHttpError(), '', {
                duration: 10000
              });
            }
            else {
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
                this.dialogRef.close('submited');
              }
            }
          }
        );

      }
      else {
        this.historicalMarkingPeriodService.updateHistoricalMarkingPeriod(this.historicalMarkingPeriodAddViewModel).subscribe(
          (res) => {
            if (typeof (res) === 'undefined') {
              this.snackbar.open('Room list failed. ' + this.defaultValuesService.getHttpError(), '', {
                duration: 10000
              });
            }
            else {
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
                this.dialogRef.close('submited');
              }
            }
          }
        );
      }
    }
  }

}
