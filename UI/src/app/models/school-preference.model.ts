import { CommonField } from "./common-field.model";


export class SchoolPreference{
    schoolPreferenceId: number;
    tenantId: string;
    schoolId: number;
    schoolGuid: string;
    schoolInternalId: string;
    schoolAltId: string;
    fullDayMinutes: number;
    halfDayMinutes: number;
    maxLoginFailure: number;
    maxInactivityDays: number;
    createdBy: string;
    createdOn: string;
    updatedBy: string;
    updatedOn: string;
}

export class SchoolPreferenceAddViewModel extends CommonField{
    schoolPreference: SchoolPreference;
    constructor() {
        super();
        this.schoolPreference= new SchoolPreference();
    }
   
}
