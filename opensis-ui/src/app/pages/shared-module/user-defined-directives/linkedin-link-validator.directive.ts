import { Directive } from '@angular/core';
import { FormControl, NG_VALIDATORS, Validator, ValidatorFn } from '@angular/forms';

@Directive({
  selector: '[appLinkedinLinkValidator]',
  providers: [
    {
      provide: NG_VALIDATORS,
      useClass: LinkedinLinkValidatorDirective,
      multi: true
    }
  ]
})
export class LinkedinLinkValidatorDirective implements Validator {

  validator: ValidatorFn;
  constructor() {
    this.validator = this.linkedinValidator();
  }

  validate(c: FormControl) {
    return this.validator(c);
  }

  linkedinValidator(): ValidatorFn {
    return (control: FormControl) => {
      if (control.value) {
        let isValid = /^https:\/\/[a-z]{2,3}\.linkedin\.com\/.*$/.test(control.value);
        if (isValid) {

          return null;
        } else {

          return {
            linkedinValidator: { valid: false }
          };
        }
      } else {

        return null;
      }
    };
  }

}
