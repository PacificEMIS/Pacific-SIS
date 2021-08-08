import { Pipe, PipeTransform } from '@angular/core';
import * as _moment from 'moment';
import { default as _rollupMoment } from 'moment';
import { SharedFunction } from '../../shared/shared-function'
const moment = _rollupMoment || _moment;
@Pipe({
  name: 'transformTimePipe'
})
export class TransformTimePipe implements PipeTransform {
  constructor(public commonfunction: SharedFunction) { }
  transform(value: string): string {
    if (value !== null && value !== undefined) {
      if (value.length === 5) {
        value = new Date("1900-01-01T" + value).toString();
        let getDateTime = this.commonfunction.formatDateSaveWithTime(value);
        let formattedDateTime = moment(getDateTime, ["YYYY-MM-DD hh:mm:ss"]).format("hh:mm A");
        return formattedDateTime

      }
      else {
        let getDateTime = this.commonfunction.serverToLocalDateAndTime(value);
        let formattedDateTime = moment(getDateTime, ["YYYY-MM-DD hh:mm:ss"]).format("hh:mm A");
        return formattedDateTime
      }
    }
    else {
      return "-"
    }


  }

}