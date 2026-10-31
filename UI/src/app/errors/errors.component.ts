import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonService } from '../services/common.service';

@Component({
    selector: 'vex-errors',
    templateUrl: './errors.component.html',
    styleUrls: ['./errors.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class ErrorsComponent implements OnInit {

  constructor(
    private commonService: CommonService
  ) { }

  ngOnInit(): void {
    this.commonService.checkAndRouteTo404();
  }

}
