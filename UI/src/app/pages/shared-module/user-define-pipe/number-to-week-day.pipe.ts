import { Pipe, PipeTransform } from '@angular/core';
@Pipe({
    name: 'weekDay'
})
export class WeekDayPipe implements PipeTransform {
    weeks = [
        { name: 'SUN',altName:'S',alias:'SU',fullName:'Sunday', id: 0 },
        { name: 'MON',altName:'M',alias:'MO',fullName:'Monday', id: 1 },
        { name: 'TUE',altName:'T',alias:'TU',fullName:'Tuesday', id: 2 },
        { name: 'WED',altName:'W',alias:'WE',fullName:'Wednesday', id: 3 },
        { name: 'THU',altName:'H',alias:'TH',fullName:'Thursday', id: 4 },
        { name: 'FRI',altName:'F',alias:'FR',fullName:'Friday', id: 5 },
        { name: 'SAT',altName:'S',alias:'SA',fullName:'Saturday', id: 6 }
    ];

    transform(value,scheduling?:boolean,toolTip?:boolean,pdf?:boolean): number | string {
        if (value) {
            if(scheduling){
                let altName;
                value = value.split('|').join('')
                for (let day of this.weeks) {
                    if(day.id==+value){
                        altName=day.altName;
                        break;
                    }
                }
                return altName;
            }else if(toolTip){
                let fullName;
                value = value.split('|').join('')
                for (let day of this.weeks) {
                    if(day.id==+value){
                        fullName=day.fullName;
                        break;
                    }
                }
                return fullName;
            } else if (pdf) {
                let finalString = [];
                value = value.split('|');
                for (let j = 0; j < this.weeks.length; j++) {
                    for (let i = 0; i < value.length; i++) {
                        if (value[i].toLowerCase() == this.weeks[j].fullName.toLowerCase()) {
                            finalString.push(this.weeks[j].alias);
                        }
                    }
                }
                return finalString.join(' - ');
            } else{
                let finalString = [];
                value = value.split('|');
                for (let j = 0; j < this.weeks.length; j++) {
                    for (let i = 0; i < value.length; i++) {
                        if (value[i].toLowerCase() == this.weeks[j].fullName.toLowerCase()) {
                            finalString.push(this.weeks[j].name);
                        }
                    }
                }
                return finalString.join(' - ');
            }
        
        } else {
            return value;
        }

    }

}