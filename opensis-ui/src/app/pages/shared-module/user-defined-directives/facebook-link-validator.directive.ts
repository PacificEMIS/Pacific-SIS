import { Directive } from '@angular/core';
import { FormControl, NG_VALIDATORS, Validator, ValidatorFn } from '@angular/forms';

@Directive({
  selector: '[appFacebookLinkValidator]',
  providers: [
    {
      provide: NG_VALIDATORS,
      useClass: FacebookLinkValidatorDirective,
      multi: true
    }
  ]
})
export class FacebookLinkValidatorDirective implements Validator {

  validator: ValidatorFn;
  constructor() {
    this.validator = this.facebookValidator();
  }

  validate(c: FormControl) {
    return this.validator(c);
  }

  facebookValidator(): ValidatorFn {
    return (control: FormControl) => {
      if (control.value) {
        let isValid = /(?:(?:http|https):\/\/)?(?:www.)?facebook.com\/(?:(?:\w)*#!\/)?(?:pages\/)?(?:[?\w\-]*\/)?(?:profile.php\?id=(?=\d.*))?([\w\-]*)?/.test(control.value);
        if (isValid) {

          return null;
        } else {

          return {
            facebookValidator: { valid: false }
          };
        }
      } else {

        return null;
      }
    };
  }

}
