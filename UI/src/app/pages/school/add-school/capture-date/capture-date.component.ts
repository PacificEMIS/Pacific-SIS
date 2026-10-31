import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { SharedFunction } from 'src/app/pages/shared/shared-function';
import * as moment from 'moment';
@Component({
  selector: 'vex-capture-date',
  templateUrl: './capture-date.component.html',
  styleUrls: ['./capture-date.component.scss']
})
export class CaptureDateComponent implements OnInit {

  constructor(
    private dialogRef: MatDialogRef<CaptureDateComponent>,
    public translateService:TranslateService,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private commonFunction: SharedFunction,
    private snackbar: MatSnackBar,
   ) { }

  ngOnInit(): void {    
  }

  submit() {
    if(this.data.schoolAddViewModel.StartDate && this.data.schoolAddViewModel.EndDate) {
      this.data.schoolAddViewModel.StartDate = this.commonFunction.formatDateSaveWithoutTime(this.data.schoolAddViewModel.StartDate);
      this.data.schoolAddViewModel.EndDate = null;
      this.dialogRef.close(this.data.schoolAddViewModel);
    } else {
        this.snackbar.open('Please enter start date and end date.', '', {
            duration: 10000
          });
        }
  }

  generateMax(date) {
   return moment(date).add(365, 'days').toDate();
  }

}
