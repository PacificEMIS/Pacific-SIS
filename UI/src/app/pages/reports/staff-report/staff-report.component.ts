import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { DefaultValuesService } from 'src/app/common/default-values.service';

@Component({
  selector: 'vex-staff-report',
  templateUrl: './staff-report.component.html',
  styleUrls: ['./staff-report.component.scss']
})
export class StaffReportComponent implements OnInit {
  pages=[];
  studentSettings=true;
  pageTitle:any = 'Advance Report';
  pageId: string = 'Advance Report';

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
