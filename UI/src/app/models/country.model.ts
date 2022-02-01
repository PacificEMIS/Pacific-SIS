import { CommonField } from "./common-field.model";

export class TableCountryModel {
    public id: number;
    public name: string;
    public countryCode: string;
    public createdBy: string;
    public createdOn: string;
    public updatedBy: string;
    public updatedOn: string;
    public state: [];
    constructor(){
        this.state = [];
        this.id= 0;
        this.name=null;
        this.countryCode=null;  
    }   
}

export class LOVCountryModel extends CommonField {
    public tableCountry: TableCountryModel[];
    public stateCount: number;
    public isCountryAvailable: boolean;
    public isListView: boolean;
    public pageNumber: number;
    public pageSize: number;
    public _pageSize: number;
    public totalCount: number;
    public sortingModel: sorting;
    public filterParams: filterParams[];
    constructor() {
        super();
        this.tableCountry = null;
        this.isCountryAvailable = false;
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

export class CountryModel extends CommonField {    
    public tableCountry : TableCountryModel[];
    public stateCount: number;   
    public isCountryAvailable:boolean;
    public isListView: boolean;
    constructor() {
        super();
        this.tableCountry= null;
        this.isCountryAvailable=false;
    }
}

export class CountryAddModel extends CommonField {
    
    public country : TableCountryModel   
    constructor() {
        super();
        this.country= new TableCountryModel();
    }
}