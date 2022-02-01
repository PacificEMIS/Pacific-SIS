import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'vex-staff-report',
  templateUrl: './staff-report.component.html',
  styleUrls: ['./staff-report.component.scss']
})
export class StaffReportComponent implements OnInit {
  pages=[];
  studentSettings=true;
  pageTitle:string = 'Advance Report';
  pageId: string = 'Advance Report';

  constructor(private router: Router) { }

  ngOnInit(): void {
  }
  getSelectedPage(pageId){
    this.pageId = pageId;
  }
}
