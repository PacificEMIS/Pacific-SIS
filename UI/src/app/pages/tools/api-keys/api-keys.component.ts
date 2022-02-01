import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { GenerateApiKeyComponent } from './generate-api-key/generate-api-key.component';
import icContentCopy from '@iconify/icons-ic/twotone-content-copy';
import icSettings from '@iconify/icons-ic/twotone-settings';
import icEdit from '@iconify/icons-ic/twotone-edit';
import icDeleteForever from '@iconify/icons-ic/twotone-delete-forever';
import { Router } from '@angular/router';
import { KeysGenerateService } from './service/keys-generate.service';
import { ApiKeyService } from 'src/app/services/api-key.service';
import { GenerateApiKeyModel, GetApiKeyModel, UpdateApiKeyModel } from 'src/app/models/api-key.model';
import { CommonService } from 'src/app/services/common.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Clipboard } from '@angular/cdk/clipboard';
import { ConfirmDialogComponent } from '../../shared-module/confirm-dialog/confirm-dialog.component';
import { LoaderService } from 'src/app/services/loader.service';


@Component({
  selector: 'vex-api-keys',
  templateUrl: './api-keys.component.html',
  styleUrls: ['./api-keys.component.scss']
})
export class ApiKeysComponent implements OnInit {

  icContentCopy = icContentCopy;
  icSettings = icSettings;
  icEdit = icEdit;
  icDeleteForever = icDeleteForever;
  generatedKeysStatus: number;
  getApiKeyModel: GetApiKeyModel = new GetApiKeyModel();
  apiKeyList: any;
  loading: boolean;

  constructor(
    public translateService: TranslateService,
    private dialog: MatDialog,
    private router: Router,
    private keyService: KeysGenerateService,
    private apiKeyService: ApiKeyService,
    private commonService: CommonService,
    private snackbar: MatSnackBar,
    private clipboard: Clipboard,
    private loaderService: LoaderService
  ) {
    // translateService.use('en');
    this.loaderService.isLoading.subscribe((currentState) => {
      this.loading = currentState;
    });
  }

  ngOnInit(): void {
    this.generatedKeysStatus = 1;
    this.getApi();

    this.keyService.apiKeyGenerated.subscribe(data => {
      if (data == 2) {
        this.generatedKeysStatus = 2;
      } else {
        this.generatedKeysStatus = 1;
      }
    });
  }

  getApi() {
    this.apiKeyService.getAPIKey(this.getApiKeyModel).subscribe((res: any) => {
      if (res._failure) {
        this.commonService.checkTokenValidOrNot(res._message);
        this.apiKeyList = res.apiKeysMasterList.length > 0 ? res.apiKeysMasterLis : null;
      } else {
        this.apiKeyList = res.apiKeysMasterList;        
        this.generatedKeysStatus = this.apiKeyList.length > 0 ? 2 : 1;
      }
    });
  }

  generateApi() {
    this.dialog.open(GenerateApiKeyComponent, {
      width: '500px'
    }).afterClosed().subscribe((res) => {
      if (res) {
        this.getApi();
      }
    })
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
          this.getApi();
        }
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
            this.getApi();
          }
        });
      }
    })
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
    event.apiKey = res.apiKeysMaster.apiKey;
        // this.getApi();
      }
    });
  }

  copyToClipboard(apiKey: string) {
    this.clipboard.copy(apiKey);
    this.snackbar.open('API key copied to clipboard.', '', {
      duration: 20000
    });
  }

  goToDetails(data) {
    this.router.navigate(['/school', 'tools', 'api', 'api-module-details'], {state: {data}});
  }

}
