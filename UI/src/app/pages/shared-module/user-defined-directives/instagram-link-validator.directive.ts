import { Directive } from '@angular/core';
import { FormControl, NG_VALIDATORS, Validator, ValidatorFn } from '@angular/forms';

@Directive({
  selector: '[appInstagramLinkValidator]',
  providers: [
    {
      provide: NG_VALIDATORS,
      useClass: InstagramLinkValidatorDirective,
      multi: true
    }
  ]
})
export class InstagramLinkValidatorDirective implements Validator {

  validator: ValidatorFn;
  constructor() {
    this.validator = this.instagramValidator();
  }

  validate(c: FormControl) {
    return this.validator(c);
  }

  instagramValidator(): ValidatorFn {
    return (control: FormControl) => {
      if (control.value) {
        let isValid = /(?:(?:http|https):\/\/)?(?:www\.)?(?:instagram\.com|instagr\.am)\/([A-Za-z0-9-_\.]+)/.test(control.value);
        if (isValid) {

          return null;
        } else {

          return {
            instagramValidator: { valid: false }
          };
        }
      } else {

        return null;
      }
    };
  }

}
