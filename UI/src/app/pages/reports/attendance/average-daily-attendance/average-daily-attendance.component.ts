import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';


@Component({
  selector: 'vex-average-daily-attendance',
  templateUrl: './average-daily-attendance.component.html',
  styleUrls: ['./average-daily-attendance.component.scss']
})
export class AverageDailyAttendanceComponent implements OnInit {

  constructor(public translateService: TranslateService) { 
    translateService.use("en");
  }

  ngOnInit(): void {
  }

}
