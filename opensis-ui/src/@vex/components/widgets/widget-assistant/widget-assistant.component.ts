import { Component, OnInit } from '@angular/core';
import icCheckCircle from '@iconify/icons-ic/twotone-check-circle';
import icError from '@iconify/icons-ic/twotone-error';

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
  constructor() { }

  ngOnInit() {
    this.today = new Date();
    this.userName = sessionStorage.getItem('user');
  }

}
