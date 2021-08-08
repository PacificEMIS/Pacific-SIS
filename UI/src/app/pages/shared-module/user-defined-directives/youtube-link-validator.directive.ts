import { Directive } from '@angular/core';
import { Validator, NG_VALIDATORS, ValidatorFn, FormControl } from '@angular/forms';

@Directive({
  selector: '[appYoutubeLinkValidator]',
  providers: [
    {
      provide: NG_VALIDATORS,
      useClass: youtubeLinkValidatorDirective,
      multi: true
    }
  ]
})
export class youtubeLinkValidatorDirective implements Validator {

  validator: ValidatorFn;
  constructor() {
    this.validator = this.youtubeValidator();
  }

  validate(c: FormControl) {
    return this.validator(c);
  }

  youtubeValidator(): ValidatorFn {
    return (control: FormControl) => {
      if (control.value) {
        let isValid = /^((?:https?:)?\/\/)?((?:www|m)\.)?((?:youtube\.com|youtu.be))(\/(?:[\w\-]+\?v=|embed\/|v\/)?)([\w\-]+)(\S+)?$/.test(control.value);
        if (isValid) {

          return null;
        } else {

          return {
            youtubeLinkvalidator: { valid: false }
          };
        }
      } else {

        return null;
      }
    };
  }
}