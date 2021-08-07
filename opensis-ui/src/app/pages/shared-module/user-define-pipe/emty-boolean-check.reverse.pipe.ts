import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'EmtyBooleanCheckReversePipe'
})
export class EmtyBooleanCheckReversePipe implements PipeTransform {

  transform(value: boolean): string {
    if(value===false){
      return "Yes";
      
    }
    else if (value===true){
      return "No";
    }
    else if(value ===null){
      return "-";
    }
   
    else{
      return "-";
    }
  }

}
