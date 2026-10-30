import { Directive } from '@angular/core';
import { UntypedFormControl, NG_VALIDATORS, Validator, ValidatorFn } from '@angular/forms';

@Directive({
  selector: '[appTwitterLinkValidator]',
  providers: [
    {
      provide: NG_VALIDATORS,
      useClass: TwitterLinkValidatorDirective,
      multi: true
    }
  ]
})
export class TwitterLinkValidatorDirective implements Validator {

  validator: ValidatorFn;
  constructor() {
    this.validator = this.twitterValidator();
  }

  validate(c: UntypedFormControl) {
    return this.validator(c);
  }

  twitterValidator(): ValidatorFn {
    return (control: UntypedFormControl) => {
      if (control.value) {
        let isValid = /(https:\/\/twitter.com\/(?![a-zA-Z0-9_]+\/)([a-zA-Z0-9_]+))/.test(control.value);
        if (isValid) {

          return null;
        } else {

          return {
            twitterValidator: { valid: false }
          };
        }
      } else {

        return null;
      }
    };
  }

}
