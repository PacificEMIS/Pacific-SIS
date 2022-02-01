import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'filterData'
})
export class InputEffortGradesNgForDivFilterPipe implements PipeTransform {

  transform(value: any, searchTerm: any) {
    if (value?.length === 0) {
      return value;
    }
    if (searchTerm) {
      return value?.filter((term) => {
        if (term.firstGivenName?.toLowerCase().indexOf(searchTerm?.toLowerCase()) > -1
          || term.lastFamilyName?.toLowerCase().indexOf(searchTerm?.toLowerCase()) > -1
          || term.studentId?.toString().indexOf(searchTerm?.toString()) > -1
          || term.gradeLevel?.toLowerCase().indexOf(searchTerm?.toLowerCase()) > -1) {
          term.isDisplay = true;
          return term;
        } else {
          term.isDisplay = false;
          return term;
        }
      })
    } else {
      value?.map((term) => {
        term.isDisplay = true;
      });
      return value;
    }
  }

}
