import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'vex-attendance-chart-details',
  templateUrl: './attendance-chart-details.component.html',
  styleUrls: ['./attendance-chart-details.component.scss']
})
export class AttendanceChartDetailsComponent implements OnInit {

  constructor(public translateService: TranslateService) { }

  ngOnInit(): void {
  }

}
