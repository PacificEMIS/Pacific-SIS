import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { DefaultValuesService } from 'src/app/common/default-values.service';

@Component({
  selector: 'vex-student-report',
  templateUrl: './student-report.component.html',
  styleUrls: ['./student-report.component.scss']
})
export class StudentReportComponent implements OnInit {

  pages=[];
  studentSettings=true;
  pageTitle:any = 'Add/Drop Report';
  pageId: string = 'Add Drop Report';

  constructor(private router: Router,
    private defaultValuesService:DefaultValuesService) { 
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
    //this.defaultValuesService.setPageId(pageId);
    
  }

}
