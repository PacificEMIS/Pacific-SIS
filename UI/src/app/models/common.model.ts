import { DefaultValuesService } from "../common/default-values.service";
import { CommonField } from "./common-field.model";
import { CustomFieldModel } from "./custom-field.model";
import { filterParams } from "./student.model";

export class AgeRangeList extends CommonField{
    gradeAgeRangeList:[GradeRange]
    constructor(){
        super();
        this._tenantName=JSON.parse(sessionStorage.getItem('tenant'));
        this._userName = JSON.parse(sessionStorage.getItem("user"));
    }
}

class GradeRange{
    ageRangeId:number;
    ageRange:string;
}

export class EducationalStage extends CommonField{
    gradeEducationalStageList:[EducationalStageList]
    constructor(){
        super();
        this._tenantName=JSON.parse(sessionStorage.getItem('tenant'));
        this._userName = JSON.parse(sessionStorage.getItem("user"));
    }
}

export class UserMasterModel {
    public emailAddress: string;
    public passwordHash: string;
    public tenantId: string;
    public schoolId: number;
    public userId: number;
    public createdBy: string;
    public createdOn: string;
    public updatedBy: string;
    public updatedOn: string;
    constructor() {
    }
}

export class ResetPasswordModel extends CommonField {
    public userMaster: UserMasterModel;
    constructor() {
        super();
        this.userMaster = new UserMasterModel();
    }
}

export class ChangePasswordViewModel extends CommonField{
    public tenantId: string;
    public schoolId: number;
    public userId: number;
    public emailAddress: string;
    public currentPasswordHash: string;
    public newPasswordHash: string;
    public confirmPasswordHash: string;
    
}

class EducationalStageList{
    iscedCode:number;
    educationalStage:string;
}

export class FilterParamsForAdvancedSearch {
    columnName: string;
    filterValue: string;
    filterOption: number;
    constructor() {
        this.filterOption = 1;
    }
}

export class FilterParamsForAdvancedSearchModel {
    public filterParams: filterParams[];
    constructor() {
        this.filterParams = [];
    }
}

export class AdvancedSearchExpansionModel {
    identificationInformation: boolean;
    accessInformation: boolean;
    searchBirthdays: boolean;
    demographicInformation: boolean;
    enrollmentInformation: boolean;
    reEnrollmentInformation: boolean;
    addressInformation: boolean;
    personalContactInformation: boolean;
    alertInformation: boolean;
    medicalNotes: boolean;
    immunizationRecord: boolean;
    nurseVisitRecord: boolean;
    searchAllSchools: boolean;
    includeInactiveStudents: boolean;
    course: boolean;
    constructor() {
        this.identificationInformation = true;
        this.accessInformation = true;
        this.searchBirthdays = true;
        this.demographicInformation = true;
        this.enrollmentInformation = true;
        this.reEnrollmentInformation = false;
        this.addressInformation = true;
        this.personalContactInformation = true;
        this.alertInformation = true;
        this.medicalNotes = true;
        this.immunizationRecord = true;
        this.nurseVisitRecord = true;
        this.searchAllSchools = true;
        this.includeInactiveStudents = true;
        this.course = false;
    }
}

export class ActiveDeactiveUserModel extends CommonField {
    public userId: number;
    public isActive: boolean;
    public module: string;
    public loginEmail: string;
}

export class BulkDataImportExcelHeader extends CommonField{
    customfieldTitle:CustomFieldModel[];
    module: string;
    constructor(){
        super();
    }
}

export class DatabaseBackupModel extends CommonField {
    constructor(){
        super();
    }
}