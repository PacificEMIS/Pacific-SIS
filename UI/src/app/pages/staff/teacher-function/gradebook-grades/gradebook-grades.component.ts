import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
@Component({
  selector: 'vex-gradebook-grades',
  templateUrl: './gradebook-grades.component.html',
  styleUrls: ['./gradebook-grades.component.scss']
})



export class GradebookGradesComponent implements OnInit {


  constructor(
    public translateService: TranslateService,
  ) {
  }


  ngOnInit(): void {
  }

}
