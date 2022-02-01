import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import icMenuBook from '@iconify/icons-ic/twotone-menu-book';
import icPrint from '@iconify/icons-ic/twotone-print';

@Component({
  selector: 'vex-schedule-report',
  templateUrl: './schedule-report.component.html',
  styleUrls: ['./schedule-report.component.scss']
})
export class ScheduleReportComponent implements OnInit {

  icMenuBook = icMenuBook;
  icPrint = icPrint;

  constructor(public translateService: TranslateService) { }

  ngOnInit(): void {
  }

}
