import { Component, OnInit } from '@angular/core';
import icEdit from '@iconify/icons-ic/twotone-edit';
import icDeleteForever from '@iconify/icons-ic/twotone-delete-forever';
import icContentCopy from '@iconify/icons-ic/twotone-content-copy';
import { TranslateService } from '@ngx-translate/core';
import { Router } from '@angular/router';
import { ApiKeyService } from 'src/app/services/api-key.service';
import { AddApiAccessmodel, ApiAccessmodel, UpdateApiKeyModel } from 'src/app/models/api-key.model';
import { CommonService } from 'src/app/services/common.service';
import { Clipboard } from '@angular/cdk/clipboard';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { GenerateApiKeyComponent } from '../generate-api-key/generate-api-key.component';
import { ConfirmDialogComponent } from 'src/app/pages/shared-module/confirm-dialog/confirm-dialog.component';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { LoaderService } from 'src/app/services/loader.service';

@Component({
  selector: 'vex-api-module-datails',
  templateUrl: './api-module-datails.component.html',
  styleUrls: ['./api-module-datails.component.scss']
})
export class ApiModuleDatailsComponent implements OnInit {

  icEdit = icEdit;
  icDeleteForever = icDeleteForever;
  icContentCopy = icContentCopy;
  apiDetails: any;
  apiAccessmodel: ApiAccessmodel = new ApiAccessmodel();
  apiDetailsList: any;
  addApiAccessmodel: AddApiAccessmodel = new AddApiAccessmodel();
  loading: boolean;

  constructor(
    public translateService: TranslateService,
    private router: Router,
    private apiKeyService: ApiKeyService,
    private commonService: CommonService,
    private clipboard: Clipboard,
    private snackbar: MatSnackBar,
    private dialog: MatDialog,
    private defaultValuesService: DefaultValuesService,
    private loaderService: LoaderService,
  ) {
    // translateService.use('en');
    this.loaderService.isLoading.subscribe((currentState) => {
      this.loading = currentState;
    });
    if (this.router.getCurrentNavigation().extras.state) {
      this.apiDetails = this.router.getCurrentNavigation().extras.state.data;
      this.getAPIAccess();
    } else {
      this.router.navigate(['school', 'tools', 'api']);
    }
  }

  ngOnInit(): void {
  }

  getAPIAccess() {
    this.apiAccessmodel.keyId = this.apiDetails.keyId;
    this.apiKeyService.getAPIAccess(this.apiAccessmodel).subscribe((res: any) => {
      if (res._failure) {
        this.commonService.checkTokenValidOrNot(res._message);
      } else {
        this.apiDetailsList = res;
        this.apiDetailsList.apiViewAccessData.map((item) => {
          item.isActive = item.apiViewAccessData.every(x => x.isActive);
        });
      }
    });
  }

  copyToClipboard(apiKey: string) {
    this.clipboard.copy(apiKey);
    this.snackbar.open('API key copied to clipboard.', '', {
      duration: 20000
    });
  }

  regenerateApiKey(event) {
    let dataSet = new UpdateApiKeyModel();
    dataSet.apiKeysMaster.apiTitle = event.apiTitle;
    dataSet.apiKeysMaster.keyId = event.keyId;
    this.apiKeyService.refreshAPIKey(dataSet).subscribe((res: any) => {
      if (res._failure) {
        this.commonService.checkTokenValidOrNot(res._message);
      } else {
        this.snackbar.open(res._message, '', {
          duration: 10000
        });
        // this.getApi();
        event.apiKey = res.apiKeysMaster.apiKey;
      }
    });
  }

  changeMasterModule(event, dataSet) {
    dataSet.apiViewAccessData.map((item) => {
      item.isActive = event.checked;
      item.updatedBy = this.defaultValuesService.getUserGuidId();
    });
  }

  changeChildModule(dataSet, apiViewAccess) {
    dataSet.isActive = dataSet.apiViewAccessData.every(x => x.isActive);
    apiViewAccess.updatedBy = this.defaultValuesService.getUserGuidId();
  }

  submitAccess() {
    this.apiDetailsList.apiViewAccessData.map((item) => {
      this.addApiAccessmodel.apiControllerKeyMapping = [...this.addApiAccessmodel.apiControllerKeyMapping, ...item.apiViewAccessData];
    })

    this.apiKeyService.addAPIAccess(this.addApiAccessmodel).subscribe((res: any) => {
      if (res._failure) {
        this.commonService.checkTokenValidOrNot(res._message);
      } else {
        this.snackbar.open(res._message, '', {
          duration: 10000
        });
        this.router.navigate(['school', 'tools', 'api']);
      }
    });
  }

  upddateApi(event) {
    let data = new UpdateApiKeyModel();
    data.apiKeysMaster.apiTitle = event.apiTitle;
    data.apiKeysMaster.keyId = event.keyId;

    this.dialog.open(GenerateApiKeyComponent, {
      width: '500px',
      data
    }).afterClosed().subscribe((res) => {
      if (res) {
        if(res.apiTitle) {
          event.apiTitle = res.apiTitle;
        } else {
        }
        // this.router.navigate(['school', 'tools', 'api']);
      }
    })
  }

  deleteApiKey(event) {
    this.dialog.open(ConfirmDialogComponent, {
      maxWidth: '400px',
      data: {
        title: 'Are you sure?',
        message: 'You are about to delete ' + event.apiTitle + '.'
      }
    }).afterClosed().subscribe((res) => {
      if (res) {
        let dataSet = new UpdateApiKeyModel();
        dataSet.apiKeysMaster.apiTitle = event.apiTitle;
        dataSet.apiKeysMaster.keyId = event.keyId;

        this.apiKeyService.deleteAPIKey(dataSet).subscribe((res: any) => {
          if (res._failure) {
            this.commonService.checkTokenValidOrNot(res._message);
          } else {
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
            // this.getApi();
            this.router.navigate(['school', 'tools', 'api']);

          }
        });
      }
    })
  }

}
