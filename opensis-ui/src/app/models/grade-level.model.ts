import { CommonField } from "./common-field.model";

export class GetAllGradeLevelsModel extends CommonField{
    public tableGradelevelList:[];
    public tenantId: string;
    public schoolId: number;
    public _tenantName: string;
    public _token: string;
    public _failure: true;
    public _message: string;  
}

class tblGradelevel {
    public tenantId: string;
    public schoolId: number;
    public gradeId: number;
    public shortName: string;
    public title: string;
    public equivalencyId: number;
    public ageRangeId:number;
    public iscedCode:number;
    public nextGrade: string;
    public nextGradeId:number;
    public sortOrder: number;
    public createdOn: string;
    public updatedOn: string;
    public createdBy: string;
    public updatedBy: string

    constructor(){
        
    }
}
export class AddGradeLevelModel extends CommonField{
        public tblGradelevel: tblGradelevel
        constructor(){
            super();
            this.tblGradelevel = new tblGradelevel();
        }

}


class gradeEquivalencyList{
    equivalencyId: string;
    gradeLevelEquivalency: string;
    gradelevels:[]
}

export class GelAllGradeEquivalencyModel extends CommonField{
    gradeEquivalencyList:gradeEquivalencyList;
}