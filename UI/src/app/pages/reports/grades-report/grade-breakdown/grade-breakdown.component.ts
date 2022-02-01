import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';


@Component({
  selector: 'vex-grade-breakdown',
  templateUrl: './grade-breakdown.component.html',
  styleUrls: ['./grade-breakdown.component.scss']
})
export class GradeBreakdownComponent implements OnInit {

  constructor(public translateService: TranslateService) { 
    // translateService.use("en");
  }

  ngOnInit(): void {
  }

}
