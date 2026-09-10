import { CommonField } from './common-field.model';

export class SuperAdministratorModel {
    staffId: number;
    schoolId: number;
    schoolName: string;
    staffGuid: string;
    firstGivenName: string;
    middleName: string;
    lastFamilyName: string;
    loginEmailAddress: string;
    hasLogin: boolean;
    isActive: boolean;
    staffRecordDisabled: boolean;
    isSelf: boolean;
    hasSchoolAttachments: boolean;
    previousSchoolId: number;
    previousProfile: string;
    lastLoginOn: string;
    createdOn: string;
}

export class SuperAdministratorListModel extends CommonField {
    superAdministrators: SuperAdministratorModel[];
    constructor() {
        super();
        this.superAdministrators = [];
    }
}

export class SuperAdministratorAddModel extends CommonField {
    firstGivenName: string;
    middleName: string;
    lastFamilyName: string;
    loginEmailAddress: string;
    passwordHash: string;
    createdBy: string;
    staffId: number;
}

export class SuperAdministratorActionModel extends CommonField {
    staffId: number;
    isActive: boolean;
    targetSchoolId: number;
    targetProfileType: string;
}

export class SuperAdministratorCandidateModel {
    staffId: number;
    schoolId: number;
    schoolName: string;
    firstGivenName: string;
    middleName: string;
    lastFamilyName: string;
    loginEmailAddress: string;
    profile: string;
}

export class SuperAdministratorCandidateListModel extends CommonField {
    searchText: string;
    candidates: SuperAdministratorCandidateModel[];
    constructor() {
        super();
        this.candidates = [];
    }
}
