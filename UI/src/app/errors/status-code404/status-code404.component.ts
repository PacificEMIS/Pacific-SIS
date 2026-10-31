import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonService } from 'src/app/services/common.service';
import { LoginService } from 'src/app/services/login.service';

@Component({
    selector: 'vex-status-code404',
    templateUrl: './status-code404.component.html',
    styleUrls: ['./status-code404.component.scss'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class StatusCode404Component implements OnInit {

  constructor(
    private commonService: CommonService,
    public loginService: LoginService,
  ) { }

  ngOnInit(): void {
  }

  homeClicked() {
    this.commonService.checkAndRoute();
  }

  numSequence(n: number): Array<number> {
    return Array(n);
  }

}
