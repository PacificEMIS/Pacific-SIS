import { Directive } from '@angular/core';
import { FormControl, NG_VALIDATORS, Validator, ValidatorFn } from '@angular/forms';

@Directive({
  selector: '[appWebsiteLinkValidator]',
  providers: [
    {
      provide: NG_VALIDATORS,
      useClass: WebsiteLinkValidatorDirective,
      multi: true
    }
  ]
})
export class WebsiteLinkValidatorDirective implements Validator {

  validator: ValidatorFn;
  constructor() {
    this.validator = this.websiteValidator();
  }

  validate(c: FormControl) {
    return this.validator(c);
  }

  websiteValidator(): ValidatorFn {
    return (control: FormControl) => {
      if (control.value) {
        let isValid = /^(http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)?[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$/.test(control.value);
        if (isValid) {

          return null;
        } else {

          return {
            websiteValidator: { valid: false }
          };
        }
      } else {

        return null;
      }
    };
  }

}
