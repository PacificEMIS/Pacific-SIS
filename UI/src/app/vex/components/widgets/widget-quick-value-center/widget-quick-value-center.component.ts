import { ChangeDetectionStrategy, Component, Input, OnInit } from '@angular/core';
import { scaleInOutAnimation } from '../../../animations/scale-in-out.animation';

@Component({
  selector: 'vex-widget-quick-value-center',
  templateUrl: './widget-quick-value-center.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  animations: [scaleInOutAnimation]
})
export class WidgetQuickValueCenterComponent implements OnInit {

  @Input() icon: string;
  @Input() value: string;
  @Input() label: string;
  @Input() subtext: string;
  @Input() change: number;
  @Input() helpText: string;
  @Input() iconClass: string;
  @Input() cardClass: string;

  faCaretUp = 'arrow_drop_up';
  faCaretDown = 'arrow_drop_down';
  icHelp = 'help_outline';
  icShare = 'share';

  showButton: boolean;

  constructor() { }

  ngOnInit() {
  }
}
