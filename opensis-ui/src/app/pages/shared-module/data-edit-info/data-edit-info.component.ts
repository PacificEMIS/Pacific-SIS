import { Component, Inject, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import icClose from '@iconify/icons-ic/twotone-close';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'vex-data-edit-info',
  templateUrl: './data-edit-info.component.html',
  styleUrls: ['./data-edit-info.component.scss']
})
export class DataEditInfoComponent implements OnInit {
  icClose = icClose;
  createdBy = this.data?.createdBy;
  createdOn = this.data?.createdOn;
  modifiedBy = this.data?.modifiedBy;
  modifiedOn = this.data?.modifiedOn;

  constructor(public translateService: TranslateService,
    @Inject(MAT_DIALOG_DATA) public data: any,
  ) {
    // translateService.use("en");
  }

  ngOnInit(): void {
  }

}
