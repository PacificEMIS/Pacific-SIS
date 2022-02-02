import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import icPrint from '@iconify/icons-ic/twotone-print';

@Component({
  selector: 'vex-schedule-report-details',
  templateUrl: './schedule-report-details.component.html',
  styleUrls: ['./schedule-report-details.component.scss']
})
export class ScheduleReportDetailsComponent implements OnInit {
  icPrint = icPrint;

  constructor(public translateService: TranslateService) { }
  
  ngOnInit(): void {
  }

}
