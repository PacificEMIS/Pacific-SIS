import { CommonField } from "./common-field.model";

export class TableCityModel {    

    public id: number;
    public name: string;
    public stateId: number;
    public createdBy: string;
    public createdOn: string;
    public updatedOn: string;
    public updatedBy: string;
    constructor(){
        this.id= null;
        this.name=null;
        this.stateId=null;
        
    }
    
}


export class CityModel extends CommonField {
    
    public tableCity : [TableCityModel];
    public stateId:number;
}