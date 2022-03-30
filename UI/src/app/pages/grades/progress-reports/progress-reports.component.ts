import { Component, OnInit } from '@angular/core';
import { MatPaginatorIntl } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import icPrint from '@iconify/icons-ic/twotone-print';
import { TranslateService } from '@ngx-translate/core';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { GetStudentProgressReportModel } from 'src/app/models/report.model';
import { CommonService } from 'src/app/services/common.service';
import { ReportService } from 'src/app/services/report.service';
import moment from 'moment';
import { LoaderService } from 'src/app/services/loader.service';

@Component({
  selector: 'vex-progress-reports',
  templateUrl: './progress-reports.component.html',
  styleUrls: ['./progress-reports.component.scss']
})
export class ProgressReportsComponent implements OnInit {

  constructor(
  ) { }

  ngOnInit(): void {
  }

}
