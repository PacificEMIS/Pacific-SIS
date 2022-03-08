import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { DefaultValuesService } from 'src/app/common/default-values.service';

@Component({
  selector: 'vex-attendance',
  templateUrl: './attendance.component.html',
  styleUrls: ['./attendance.component.scss']
})
export class AttendanceComponent implements OnInit {
  pages=[];
  studentSettings=true;
  pageTitle:any = 'Attendance Report';
  pageId: string = 'Attendance Report';

  constructor(
    private router: Router, 
    public translateService: TranslateService, 
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
