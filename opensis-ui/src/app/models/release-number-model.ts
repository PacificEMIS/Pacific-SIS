import { CommonField } from "./common-field.model";

export class ReleaseNumber {
    tenantId: string;
    schoolId: number;
    releaseNumber1: string;
    releaseDate: string;
    createdBy: string;
    createdOn: string;
    updatedBy: string;
    updatedOn: string;
}

export class ReleaseNumberAddViewModel extends CommonField {

    public releaseNumber : ReleaseNumber;
    constructor() {
        super();
        this.releaseNumber = new ReleaseNumber();
    }

}