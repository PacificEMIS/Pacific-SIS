import {
  Component,
  Input,
  OnDestroy,
  OnInit,
  TemplateRef,
} from "@angular/core";
import icShoppingBasket from "@iconify/icons-ic/twotone-shopping-basket";
import { CommonService } from "../../../app/services/common.service";
import { ReleaseNumberAddViewModel } from "../../../app/models/release-number-model";
import { MatSnackBar } from "@angular/material/snack-bar";
import { SchoolService } from "../../../app/services/school.service";
import { takeUntil } from "rxjs/operators";
import { Subject } from "rxjs";
import { DefaultValuesService } from '../../../app/common/default-values.service';

@Component({
  selector: "vex-footer",
  templateUrl: "./footer.component.html",
  styleUrls: ["./footer.component.scss"],
})
export class FooterComponent implements OnInit, OnDestroy {
  @Input() customTemplate: TemplateRef<any>;
  destroySubject$: Subject<void> = new Subject();
  icShoppingBasket = icShoppingBasket;
  releaseNumberAddViewModel: ReleaseNumberAddViewModel = new ReleaseNumberAddViewModel();
  tenantFooter: any;
  
  constructor(
    private commonService: CommonService,
    private snackbar: MatSnackBar,
    private schoolService: SchoolService,
    private defaultValuesService: DefaultValuesService
  ) {
    this.tenantFooter = this.defaultValuesService.getPhotoAndFooter().tenantFooter;
  }

  ngOnInit() {
    this.schoolService.schoolListCalled
      .pipe(takeUntil(this.destroySubject$))
      .subscribe((res) => {
        if (res.academicYearChanged || res.academicYearLoaded) {
          this.getReleaseNumber();
        }
      });
  }

  getReleaseNumber() {
    this.releaseNumberAddViewModel.releaseNumber.schoolId = this.defaultValuesService.getSchoolID();
    this.releaseNumberAddViewModel.releaseNumber.tenantId =this.defaultValuesService.getTenantID();
    this.commonService
      .getReleaseNumber(this.releaseNumberAddViewModel)
      .subscribe((data) => {
        if (typeof data == "undefined") {
          this.snackbar.open(
            "Release Number failed. " + this.defaultValuesService.getHttpError(),
            "",
            {
              duration: 10000,
            }
          );
        } else {
         if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
          } else {
            this.releaseNumberAddViewModel.releaseNumber.releaseNumber1 =
              data.releaseNumber.releaseNumber1;
          }
        }
      });
  }


  ngOnDestroy(): void {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }
}
