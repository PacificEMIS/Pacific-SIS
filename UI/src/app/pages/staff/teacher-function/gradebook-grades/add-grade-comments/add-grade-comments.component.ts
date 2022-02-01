import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import icClose from '@iconify/icons-ic/twotone-close';
import { DefaultValuesService } from 'src/app/common/default-values.service';

@Component({
  selector: 'vex-add-grade-comments',
  templateUrl: './add-grade-comments.component.html',
  styleUrls: ['./add-grade-comments.component.scss']
})
export class AddGradeCommentsComponent implements OnInit {
  icClose = icClose;
  public isThisCurrentYear:Boolean;
  constructor(
    private dialogRef: MatDialogRef<AddGradeCommentsComponent>,
     public translateService:TranslateService,
     private defaultValuesService:DefaultValuesService,
     @Inject(MAT_DIALOG_DATA) public data,
     ) { 
    // translateService.use('en');
    this.isThisCurrentYear=this.defaultValuesService.checkAcademicYear();
  }

  ngOnInit(): void {
  }

  submitComment() {
    this.dialogRef.close(this.data.comment);
  }

}
