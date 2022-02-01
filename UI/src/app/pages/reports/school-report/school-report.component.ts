import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'vex-school-report',
  templateUrl: './school-report.component.html',
  styleUrls: ['./school-report.component.scss']
})
export class SchoolReportComponent implements OnInit {
  pages=[];
  studentSettings=true;
  pageTitle:any = 'Institute Report';
  pageId: string = 'Institute Report';

  constructor(private router: Router) { }

  ngOnInit(): void {
  }
  getSelectedPage(pageId){
    this.pageId = pageId;
    
  }

}
