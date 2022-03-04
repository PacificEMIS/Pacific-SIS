import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { DefaultValuesService } from 'src/app/common/default-values.service';

@Component({
  selector: 'vex-schedule',
  templateUrl: './schedule.component.html',
  styleUrls: ['./schedule.component.scss']
})
export class ScheduleComponent implements OnInit {

  pages=[];
  studentSettings=true;
  pageTitle:any = 'Add/Drop Report';
  pageId: string = 'Add Drop Report';

  constructor(
    private router: Router, public translateService: TranslateService, 
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
