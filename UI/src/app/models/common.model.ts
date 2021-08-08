import { DefaultValuesService } from "../common/default-values.service";
import { CommonField } from "./common-field.model";
import { CustomFieldModel } from "./custom-field.model";

export class AgeRangeList extends CommonField{
    gradeAgeRangeList:[GradeRange]
    constructor(){
        super();
        this._tenantName=sessionStorage.getItem('tenant');
        this._userName = sessionStorage.getItem("user");
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
        this._tenantName=sessionStorage.getItem('tenant');
        this._userName = sessionStorage.getItem("user");
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

export class BulkDataImportExcelHeader extends CommonField{
    customfieldTitle:CustomFieldModel[];
    module: string;
    constructor(){
        super();
    }
}