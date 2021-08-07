export class CertificateModel{
    id: number;
    tenantId: string;
    schoolId: number;
    staffId: number;
    certificationName: string;
    shortName: string;
    certificationCode: string;
    primaryCertification: boolean;
    certificationDate: string;
    certificationExpiryDate: string;
    certificationDescription: string;
    createdBy: string;
    createdOn: string;
    updatedOn: string;
    updatedBy: string;
    constructor(){
        this.id = 0;
        this.staffId = 0;
        
    }
}
