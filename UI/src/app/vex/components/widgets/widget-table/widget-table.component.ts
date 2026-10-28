import { AfterViewInit, Component, Input, OnChanges, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import icMoreHoriz from '@iconify/icons-ic/twotone-more-horiz';
import icCloudDownload from '@iconify/icons-ic/twotone-cloud-download';
import { TableColumn } from '../../../interfaces/table-column.interface';
import { PageRolesPermission } from 'src/app/common/page-roles-permissions.service';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'vex-widget-table',
  templateUrl: './widget-table.component.html'
})
export class WidgetTableComponent<T> implements OnInit, OnChanges, AfterViewInit {

  @Input() data: T[];
  @Input() columns: TableColumn<T>[];
  @Input() pageSize = 6;
  @Input() calendarEvents;

  visibleColumns: Array<keyof T | string>;
  dataSource = new MatTableDataSource<T>();

  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;

  icMoreHoriz = icMoreHoriz;
  icCloudDownload = icCloudDownload;

  constructor(
    private pageRolePermission: PageRolesPermission,
    private router: Router,
    private snackbar: MatSnackBar,
    public translateService: TranslateService,
  ) { }

  ngOnInit() {
    
  }

  goToCalendar() {
    if (this.pageRolePermission.checkPageRolePermission('/school/schoolcalendars').view) {
      this.router.navigate(['school/schoolcalendars']);
    } else {
      this.snackbar.open(`You don't have permission to view Calendar`, '', {
        duration: 10000
      });
    }
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.columns) {
      this.visibleColumns = this.columns.map(column => column.property);
    }
    if (changes.data) {
      this.dataSource.data = this.data;
    }
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }
}
