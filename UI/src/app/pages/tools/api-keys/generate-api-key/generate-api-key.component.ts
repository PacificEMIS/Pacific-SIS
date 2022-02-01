import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import icClose from '@iconify/icons-ic/twotone-close';
import { GenerateApiKeyModel, UpdateApiKeyModel } from 'src/app/models/api-key.model';
import { ApiKeyService } from 'src/app/services/api-key.service';
import { CommonService } from 'src/app/services/common.service';
import { LoaderService } from 'src/app/services/loader.service';
import { KeysGenerateService } from '../service/keys-generate.service';

@Component({
  selector: 'vex-generate-api-key',
  templateUrl: './generate-api-key.component.html',
  styleUrls: ['./generate-api-key.component.scss']
})
export class GenerateApiKeyComponent implements OnInit {

  icClose = icClose;
  apiKeyModel;
  @ViewChild('f') apiForm: NgForm;
  loading: boolean;

  constructor(
    public dialogRef: MatDialogRef<GenerateApiKeyComponent>,
    private apiKeyService: ApiKeyService,
    private commonService: CommonService,
    private snackbar: MatSnackBar,
    private loaderService: LoaderService,
    @Inject(MAT_DIALOG_DATA) public data,
  ) {
    this.loaderService.isLoading.subscribe((currentState) => {
      this.loading = currentState;
    });
    this.apiKeyModel = this.data ? this.data : new GenerateApiKeyModel();
  }

  ngOnInit(): void {
  }

  generate(): void {
    if (this.apiForm.valid) {
      if (this.data) {
        this.apiKeyService.updateAPIKeyTitle(this.apiKeyModel).subscribe((res) => {
          if (res._failure) {
            this.commonService.checkTokenValidOrNot(res._message);
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          } else {
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
            this.dialogRef.close({apiTitle: res.apiKeysMaster.apiTitle});
          }
        });
      } else {
        this.apiKeyService.generateAPIKey(this.apiKeyModel).subscribe((res) => {
          if (res._failure) {
            this.commonService.checkTokenValidOrNot(res._message);
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          } else {
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
            this.dialogRef.close(2);
          }
        });
      }
    } else {
      this.apiForm.form.markAllAsTouched();
    }

  }

}
