import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { DefaultValuesService } from 'src/app/common/default-values.service';

@Component({
  selector: 'vex-attendance-chart',
  templateUrl: './attendance-chart.component.html',
  styleUrls: ['./attendance-chart.component.scss']
})
export class AttendanceChartComponent implements OnInit {

  constructor(
    private router: Router,
    private defaultValuesService:DefaultValuesService
    ) { 
      this.defaultValuesService.setReportCompoentTitle.next("Attendance Chart");
    }

  ngOnInit(): void {
  }

  viewDetailsAttendanceChart(){
    this.router.navigate(['/school', 'reports', 'attendance', 'attendance-chart', 'attendance-chart-details']);
  }

}
