import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'vex-schedule',
  templateUrl: './schedule.component.html',
  styleUrls: ['./schedule.component.scss']
})
export class ScheduleComponent implements OnInit {

  pages=[];
  studentSettings=true;
  pageTitle:string = 'Add/Drop Report';
  pageId: string = 'Add Drop Report';

  constructor(private router: Router, public translateService: TranslateService,) { }


  ngOnInit(): void {
  }

  getSelectedPage(pageId){
    this.pageId = pageId;
    
  }

}
