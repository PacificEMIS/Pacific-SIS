import { Component, OnInit } from '@angular/core';
import { Notification } from '../interfaces/notification.interface';
import { DateTime } from 'luxon';
import { trackById } from '../../../../utils/track-by';

@Component({
    selector: 'vex-toolbar-notifications-dropdown',
    templateUrl: './toolbar-notifications-dropdown.component.html',
    styleUrls: ['./toolbar-notifications-dropdown.component.scss'],
    standalone: false
})
export class ToolbarNotificationsDropdownComponent implements OnInit {

  notifications: Notification[] = [
    {
      id: '1',
      label: 'New Order Received',
      icon: 'shopping_basket',
      colorClass: 'text-primary',
      datetime: DateTime.local().minus({ hour: 1 })
    },
    {
      id: '2',
      label: 'New customer has registered',
      icon: 'account_circle',
      colorClass: 'text-orange',
      datetime: DateTime.local().minus({ hour: 2 })
    },
    {
      id: '3',
      label: 'Campaign statistics are available',
      icon: 'insert_chart',
      colorClass: 'text-purple',
      datetime: DateTime.local().minus({ hour: 5 })
    },
    {
      id: '4',
      label: 'Project has been approved',
      icon: 'check_circle',
      colorClass: 'text-green',
      datetime: DateTime.local().minus({ hour: 9 })
    },
    {
      id: '5',
      label: 'Client reports are available',
      icon: 'description',
      colorClass: 'text-primary',
      datetime: DateTime.local().minus({ hour: 30 })
    },
    {
      id: '6',
      label: 'New review received',
      icon: 'feedback',
      colorClass: 'text-orange',
      datetime: DateTime.local().minus({ hour: 40 }),
      read: true
    },
    {
      id: '7',
      label: '22 verified registrations',
      icon: 'verified_user',
      colorClass: 'text-green',
      datetime: DateTime.local().minus({ hour: 60 })
    },
    {
      id: '8',
      label: 'New files available',
      icon: 'file_copy',
      colorClass: 'text-amber',
      datetime: DateTime.local().minus({ hour: 90 })
    }
  ];

  icSettings = 'settings';
  icChevronRight = 'chevron_right';
  icClearAll = 'clear_all';
  icNotificationsOff = 'notifications_off';
  trackById = trackById;

  constructor() { }

  ngOnInit() {
  }

}
