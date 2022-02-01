import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'ssnMask'
})
export class SsnMaskPipe implements PipeTransform {

  transform(value: string): string {
    let mask = value.replace(/\w(?=\w{4})/g, "*");
    return mask;
  }

}
