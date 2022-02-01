import { CommonField } from "./common-field.model";



export class tableLanguage {
    langId: number;
    lcid: string;
    locale: string;
    languageCode: string;
    createdBy: string;
    createdOn: string;
    updatedBy: string;
    updatedOn: string;
    userMaster: []
}

export class LanguageAddModel {    
    public language:tableLanguage;   
    public _failure: boolean;
    public _message:string;
    public _tenantName:string;
    public _userName: string;
    public _token:string;
    
    constructor() {       
        this.language =new tableLanguage();
        this._failure=false;
        this._message="";
        this._tenantName="";
        this._token="";
          this._userName="";
    }
}

export class LOVLanguageModel extends CommonField {
    public tableLanguage: tableLanguage[];
    public isLanguageAvailable: boolean;
    public isListView: boolean;
    public pageNumber: number;
    public pageSize: number;
    public _pageSize: number;
    public totalCount: number;
    public sortingModel: sorting;
    public filterParams: filterParams[];
    constructor() {
        super();
        this.tableLanguage = null;
        this.isLanguageAvailable = false;
        this.pageNumber = 1;
        this.pageSize = 10;
        this.sortingModel = new sorting();
        this.filterParams = null;
    }
}

class sorting {
    sortColumn: string;
    sortDirection: string;
    constructor() {
        this.sortColumn = '';
        this.sortDirection = '';
    }
}

class filterParams {
    columnName: string;
    filterValue: string;
    filterOption: number;
    constructor() {
        this.columnName = null;
        this.filterOption = 3;
        this.filterValue = null;
    }
}

export class LanguageModel  {
    public tableLanguage : tableLanguage[];
    public isLanguageAvailable:boolean;
    public isListView: boolean;
    public _failure: boolean;
    public _message:string;
    public _tenantName:string;
    public _userName: string;
    public _token:string;
    constructor() {       
        this.tableLanguage=[];   
        this.isLanguageAvailable=false;
    }
}


