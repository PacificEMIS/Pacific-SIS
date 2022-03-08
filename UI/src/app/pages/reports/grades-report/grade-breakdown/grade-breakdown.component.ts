import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { DefaultValuesService } from 'src/app/common/default-values.service';


@Component({
  selector: 'vex-grade-breakdown',
  templateUrl: './grade-breakdown.component.html',
  styleUrls: ['./grade-breakdown.component.scss']
})
export class GradeBreakdownComponent implements OnInit {

  constructor(public translateService: TranslateService,
    private defaultValuesService:DefaultValuesService
    ) { 
      this.defaultValuesService.setReportCompoentTitle.next("Grade BreakDown");
    // translateService.use("en");
  }

  ngOnInit(): void {
  }

}
