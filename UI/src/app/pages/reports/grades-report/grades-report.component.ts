import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { DefaultValuesService } from 'src/app/common/default-values.service';

@Component({
  selector: 'vex-grades-report',
  templateUrl: './grades-report.component.html',
  styleUrls: ['./grades-report.component.scss']
})
export class GradesReportComponent implements OnInit {
  pages=[];
  studentSettings=true;
  pageTitle:any = 'Grade Breakdown';
  pageId: string = 'Grade Breakdown';

  constructor(
    private router: Router,
    private defaultValuesService:DefaultValuesService
    ) { 
      this.defaultValuesService.setReportCompoentTitle.subscribe(x=>{
        if(x){
          this.pageTitle = x;
        }
      })
    }

  ngOnInit(): void {
  }

  getSelectedPage(pageId){
    this.pageId = pageId;
  }

}
