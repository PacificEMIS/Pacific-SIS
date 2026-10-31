import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'ssnMask',
    standalone: false
})
export class SsnMaskPipe implements PipeTransform {

  transform(value: string): string {
    let mask = value.replace(/\w(?=\w{4})/g, "*");
    return mask;
  }

}
