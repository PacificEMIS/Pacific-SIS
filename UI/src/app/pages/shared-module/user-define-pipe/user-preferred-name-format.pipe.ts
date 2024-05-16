import { Pipe, PipeTransform } from '@angular/core';
@Pipe({
    name: 'UserPreferredNameFormatPipe'
})
export class UserPreferredNameFormatPipe implements PipeTransform {
    transform(user: any): string {
        let userName: string = '-';
        if (!!(user?.lastFamilyName && user?.firstGivenName)) {
            if (!!user?.preferredName || !!user?.prefferedName) {
                userName = (user?.lastFamilyName||'-') + ', ' +(user?.preferredName || user?.prefferedName || '-');
            }
            else {
                if (!!user?.middleName) {
                    userName = (user?.lastFamilyName||'-') + ' ' + (user?.middleName||'-') + ', ' + (user?.firstGivenName||'-');
                }
                else {
                    userName = (user?.lastFamilyName||'-') + ', ' + (user?.firstGivenName||'-');
                }
            }
        }
        else if (!!user?.lastFamilyName || !!user?.firstGivenName) {
            if (!!user?.preferredName || !!user?.prefferedName) {
                userName = (user?.lastFamilyName||'-') + ', ' +(user?.preferredName || user?.prefferedName ||'-');
            }
            else {
                if (!!user?.middleName) {
                    userName = (user?.lastFamilyName||'-') + ' ' + (user?.middleName||'-') + ', ' + (user?.firstGivenName||'-');
                }
                else {
                    userName = (user?.lastFamilyName||'-') + ', ' + (user?.firstGivenName||'-');
                }
            }
        }
        else {
            let splitName = user?.studentName.split(" ");
            let lastName = splitName[splitName.length - 1];
            let firstName = splitName[0];
            let middleName = "";
            if (splitName.length > 2) {
                middleName = splitName[splitName.length - 2];
                userName = (lastName||'-') + ' ' + (middleName||'-') + ', ' + (firstName||'-');
            }
            if (!!user?.studentPreferredName) {
                userName = (lastName||'-') + ', ' + (user?.studentPreferredName||'-');
            }
            else {
                userName = (lastName||'-') + ', ' + (firstName||'-');
            }
        }
        return userName;
    }
}