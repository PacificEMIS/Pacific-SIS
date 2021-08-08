import { Component, OnInit } from '@angular/core';
import icDeleteForever from '@iconify/icons-ic/twotone-delete-forever';
import { TranslateService } from '@ngx-translate/core';

export interface LogData {
  loginTime: string;
  loginEmail: string;
  name: string;
  profile: string;
  failureCount: number;
  status: string;
  ipAddress: string;
}

export const LogData: LogData[] = [
  {loginTime: '2021-06-17 11:26:05', loginEmail: 'johndoe@example.com', name: 'John Doe', profile: 'Super Admin', failureCount: 0, status: 'Success', ipAddress: '123.123.12.123'},
  {loginTime: '2021-06-17 11:14:28', loginEmail: 'johndoe@example.com', name: 'Danny Anderson', profile: 'Student', failureCount: 0, status: 'Success', ipAddress: '123.123.12.123'},
  {loginTime: '2021-06-17 11:03:16', loginEmail: 'johndoe@example.com', name: 'Roman Loafer', profile: 'Student', failureCount: 0, status: 'Success', ipAddress: '123.123.12.123'},
  {loginTime: '2021-06-17 10:19:43', loginEmail: 'johndoe@example.com', name: 'Ella Brown', profile: 'Teacher', failureCount: 0, status: 'Success', ipAddress: '123.123.12.123'},
  {loginTime: '2021-06-17 11:26:05', loginEmail: 'johndoe@example.com', name: 'Adriana Garcia', profile: 'Teacher', failureCount: 0, status: 'Success', ipAddress: '123.123.12.123'},
  {loginTime: '2021-06-17 10:19:08', loginEmail: 'johndoe@example.com', name: 'Javier Holmes', profile: 'Student', failureCount: 0, status: 'Success', ipAddress: '123.123.12.123'},
  {loginTime: '2021-06-17 10:18:24', loginEmail: 'johndoe@example.com', name: 'Olivia Jones', profile: 'Student', failureCount: 0, status: 'Success', ipAddress: '123.123.12.123'},
  {loginTime: '2021-06-17 10:10:05', loginEmail: 'johndoe@example.com', name: 'Laura Paiva', profile: 'Student', failureCount: 0, status: 'Success', ipAddress: '123.123.12.123'},
  {loginTime: '2021-06-17 11:26:05', loginEmail: 'johndoe@example.com', name: 'John Doe', profile: 'Super Admin', failureCount: 0, status: 'Success', ipAddress: '123.123.12.123'},
  {loginTime: '2021-06-17 11:03:16', loginEmail: 'johndoe@example.com', name: 'Alyssa Kimathi', profile: 'Super Admin', failureCount: 0, status: 'Success', ipAddress: '123.123.12.123'},
];

@Component({
  selector: 'vex-access-log',
  templateUrl: './access-log.component.html',
  styleUrls: ['./access-log.component.scss']
})
export class AccessLogComponent implements OnInit {

  icDeleteForever = icDeleteForever;

  displayedColumns: string[] = ['loginTime', 'loginEmail', 'name', 'profile', 'failureCount', 'status', 'ipAddress'];
  logData = LogData;

  constructor(public translateService: TranslateService) { 
    translateService.use("en");
  }

  ngOnInit(): void {
  }

}
