import { Component, OnInit } from '@angular/core';
import { DefaultValuesService } from '../../../../common/default-values.service';
@Component({
  selector: 'vex-widget-assistant',
  templateUrl: './widget-assistant.component.html',
  styleUrls: ['./widget-assistant.component.scss']
})
export class WidgetAssistantComponent implements OnInit {

  icCheckCircle = 'check_circle';
  icError = 'error';
  today : Date;
  userName: string;
  constructor(private defaultValuesService: DefaultValuesService) { }

  ngOnInit() {
    this.today = new Date();
    this.userName = this.defaultValuesService.getUserName();
  }

}
