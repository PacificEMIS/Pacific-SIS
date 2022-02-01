import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'vex-grades-report',
  templateUrl: './grades-report.component.html',
  styleUrls: ['./grades-report.component.scss']
})
export class GradesReportComponent implements OnInit {
  pages=[];
  studentSettings=true;
  pageTitle:string = 'Grade Breakdown';
  pageId: string = 'Grade Breakdown';

  constructor(private router: Router) { }

  ngOnInit(): void {
  }

  getSelectedPage(pageId){
    this.pageId = pageId;
  }

}
