import { CommonField } from "./common-field.model";

export class TableCountryModel {
    public id: number;
    public name: string;
    public countryCode: string;
    public createdBy: string;
    public createdOn: string;
    public updatedBy: string;
    public updatedOn: string;
    public state: []
    constructor(){
        this.state = null;
        this.id= 0;
        this.name=null;
        this.countryCode=null;  
    }   
}



export class CountryModel extends CommonField {    
    public tableCountry : TableCountryModel[];
    public stateCount: number;   
    public isCountryAvailable:boolean;
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