import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import icDelete from '@iconify/icons-ic/twotone-delete-forever';
import { CommonService } from 'src/app/services/common.service';
import { DatabaseBackupModel } from 'src/app/models/common.model';
import { HttpEventType, HttpResponse } from '@angular/common/http';
import { formatDate } from '@angular/common';
import { LoaderService } from 'src/app/services/loader.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'vex-database-backup',
  templateUrl: './database-backup.component.html',
  styleUrls: ['./database-backup.component.scss']
})
export class DatabaseBackupComponent implements OnInit {

  icDelete = icDelete;
  databaseBackupModel: DatabaseBackupModel = new DatabaseBackupModel()
  loading: boolean;
  
  constructor(
    public translateService: TranslateService,
    private commonService: CommonService,
    private loaderService: LoaderService,
    private snackbar: MatSnackBar
    ) { 
    // translateService.use("en");
    this.loaderService.isLoading
    // .pipe(takeUntil(this.destroySubject$))
    .subscribe((currentState) => {
      this.loading = currentState;
    });
  }

  ngOnInit(): void {
  }

  crateDatabaseBackup() {
    this.commonService.createDatabaseBackup(this.databaseBackupModel).subscribe((event:any)=>{
      if(event._failure){
        this.commonService.checkTokenValidOrNot(event._message);
          if(!event.gradeUsStandardList){
            this.snackbar.open(event._message, '', {
              duration: 10000
            });
          }

          
      } else {  
        this.downloadFile(event);
    }
    });
  }
 
  private downloadFile(data: HttpResponse<Blob>) {
    const downloadedFile = new Blob([data.body], { type: data.body.type });
    const a = document.createElement('a');
    a.setAttribute('style', 'display:none;');
    document.body.appendChild(a);
    a.download = 'DBBackup_' + formatDate(Date(), 'yyyy_MM_dd_hh_mm_ss', 'en-US')+'.sql';
    a.href = URL.createObjectURL(downloadedFile);
    a.target = '_blank';
    a.click();
    document.body.removeChild(a);
}
}
