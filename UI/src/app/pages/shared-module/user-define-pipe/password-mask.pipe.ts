import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'passwordMask'
})
export class PasswordMaskPipe implements PipeTransform {

  transform(value: string): string {
    let mask = value.replace(/./g, "*");
    return mask;
  }

}
