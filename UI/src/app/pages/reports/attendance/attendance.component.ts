import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'vex-attendance',
  templateUrl: './attendance.component.html',
  styleUrls: ['./attendance.component.scss']
})
export class AttendanceComponent implements OnInit {
  pages=[];
  studentSettings=true;
  pageTitle:string = 'Attendance Report';
  pageId: string = 'Attendance Report';

  constructor(private router: Router, public translateService: TranslateService) { }

  ngOnInit(): void {
  }

  getSelectedPage(pageId){
    this.pageId = pageId;
  }

}
