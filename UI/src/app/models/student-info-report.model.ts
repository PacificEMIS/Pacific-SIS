import { CommonField } from "./common-field.model";


export class StudentInfoReportModel extends CommonField{
    isGeneralInfo: boolean;
    isEnrollmentInfo: boolean;
    isAddressInfo: boolean;
    isFamilyInfo: boolean;
    isMedicalInfo: boolean;
    isComments: boolean;
    studentGuids: any[]

    constructor() {
        super();
        this.studentGuids = [];
        this.isGeneralInfo = true;
        this.isEnrollmentInfo = true;
        this.isAddressInfo = true;
        this.isFamilyInfo = true;
        this.isMedicalInfo = false;
        this.isComments = false;
    }
}





