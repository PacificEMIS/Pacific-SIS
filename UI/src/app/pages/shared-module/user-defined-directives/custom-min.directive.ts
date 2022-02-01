import { Directive, Input } from '@angular/core';
import { FormControl, NG_VALIDATORS, Validator } from '@angular/forms';

@Directive({
  selector: '[customMin]',
  providers: [{ provide: NG_VALIDATORS, useExisting: CustomMinDirective, multi: true }]
})
export class CustomMinDirective implements Validator {

  constructor() { }

  @Input()
  customMin: number;

  validate(c: FormControl): { [key: string]: any } {
    let v = c.value;
    return (v < this.customMin) ? { "customMin": true } : null;
  }
}