import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'vex-attendance-chart',
  templateUrl: './attendance-chart.component.html',
  styleUrls: ['./attendance-chart.component.scss']
})
export class AttendanceChartComponent implements OnInit {

  constructor(private router: Router,) { }

  ngOnInit(): void {
  }

  viewDetailsAttendanceChart(){
    this.router.navigate(['/school', 'reports', 'attendance', 'attendance-chart', 'attendance-chart-details']);
  }

}
