import { Directive } from '@angular/core';
import { Validator, NG_VALIDATORS, ValidatorFn, FormControl } from '@angular/forms';

@Directive({
  selector: '[appPhonevalidator]',
  providers: [
    {
      provide: NG_VALIDATORS,
      useClass: PhonevalidatorDirective,
      multi: true
    }
  ]
})
export class PhonevalidatorDirective implements Validator {

  validator: ValidatorFn;
  constructor() {
    this.validator = this.phoneValidator();
  }

  validate(c: FormControl) {
    return this.validator(c);
  }

  phoneValidator(): ValidatorFn {
    return (control: FormControl) => {
      if (control.value) {
        let isValid = /^(\+\d{1,3}[- ]?)?\d{10}$/.test(control.value);
        if (isValid) {
          return null;
        } else {
          return {
            phonevalidator: { valid: false }
          };
        }
      } else {
        return null;
      }
    };
  }
}