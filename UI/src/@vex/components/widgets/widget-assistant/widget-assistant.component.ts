import { Component, OnInit } from '@angular/core';
import icCheckCircle from '@iconify/icons-ic/twotone-check-circle';
import icError from '@iconify/icons-ic/twotone-error';
import { DefaultValuesService } from '../../../../app/common/default-values.service';
@Component({
  selector: 'vex-widget-assistant',
  templateUrl: './widget-assistant.component.html',
  styleUrls: ['./widget-assistant.component.scss']
})
export class WidgetAssistantComponent implements OnInit {

  icCheckCircle = icCheckCircle;
  icError = icError;
  today : Date;
  userName: string;
  constructor(private defaultValuesService: DefaultValuesService) { }

  ngOnInit() {
    this.today = new Date();
    this.userName = this.defaultValuesService.getUserName();
  }

}
