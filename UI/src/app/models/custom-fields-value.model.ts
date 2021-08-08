import { CommonField } from "./common-field.model";
export class CustomFieldsValueModel {
    tenantId: string;
    schoolId: number;
    categoryId: number;
    fieldId: number;
    targetId: number;
    module:string;
    customFieldType: string;
    customFieldTitle: string;
    customFieldValue: string;
    createdBy: string;
    createdOn: string;
    updatedOn: string;
    updatedBy: string;
    constructor(){
        this.fieldId=0;
        this.customFieldType="";
        this.customFieldTitle="";
        this.module="";
        this.customFieldValue="";
        this.categoryId=0;
        this.updatedBy="";
    }
  }