import { CommonField } from "./common-field.model";

export class TableSectionList {
    public tenantId: string;
    public schoolId: number;
    public sectionId: number;
    public name: string;
    public sortOrder: number;
    public lastUpdated: string;
    public updatedBy: string;

}


export class GetAllSectionModel extends CommonField{
    public tableSectionsList: TableSectionList[];
    public tenantId: string;
    public schoolId: number;
    public isSectionAvailable: boolean;
    public isListView:boolean;
    constructor() {
        super();
        this.tableSectionsList = [new TableSectionList];
        this.isSectionAvailable = false;
    }
}

export class TableSection  {
    public tenantId: string;
    public schoolId: number;
    public sectionId: number;
    public name: string;
    public sortOrder: number;
    public createdBy: string;
    public createdOn: string;
    public updatedOn: string;
    public updatedBy: string;
    constructor() {
    }

}


export class SectionAddModel extends CommonField {
   public tableSections: TableSection;
    constructor() {
        super(); 
        this.tableSections = new TableSection();
    }

}




