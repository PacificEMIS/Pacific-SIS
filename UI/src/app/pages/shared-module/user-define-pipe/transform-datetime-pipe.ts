import { Pipe, PipeTransform } from '@angular/core';
import * as _moment from 'moment';
import { default as _rollupMoment } from 'moment';
import {SharedFunction} from '../../shared/shared-function'
const moment =  _rollupMoment || _moment;
@Pipe({
  name: 'transformDateTimePipe'
})
export class TransformDateTimePipe implements PipeTransform {
 constructor(public commonfunction:SharedFunction ){}
  transform(value: string): string {
    if(value!==null && value!==undefined){
      if(value.trim()!==""){       
        let getDateTime = this.commonfunction.serverToLocalDateAndTime(value);
        let formattedDateTime = moment(getDateTime, ["YYYY-MM-DD hh:mm:ss"]).format("DD-MM-YYYY"+" | "+"h:mm A");
        return formattedDateTime
      }else{
        return "-";
      }
    }
    else{
      return "-"
    }
  }

}
