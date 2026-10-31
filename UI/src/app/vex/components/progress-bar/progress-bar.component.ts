import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { LoadingBarService } from '@ngx-loading-bar/core';

@Component({
    selector: 'vex-progress-bar',
    templateUrl: './progress-bar.component.html',
    styleUrls: ['./progress-bar.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class ProgressBarComponent implements OnInit {

  constructor(public loader: LoadingBarService) { }

  ngOnInit() {
  }

}
