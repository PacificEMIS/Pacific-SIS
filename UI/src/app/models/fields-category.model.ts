
import { Module } from "../enums/module.enum";
import { CommonField } from "./common-field.model";
import { CustomFieldModel } from "./custom-field.model";
export class FieldsCategoryModel {
    tenantId: string;
    schoolId: number;
    categoryId: number;
    isSystemCategory: boolean;
    search: boolean;
    title: string;
    module: string;
    sortOrder: number;
    required: boolean;
    hide: boolean;
    createdBy: string;
    createdOn: string;
    updatedOn: string;
    updatedBy: string;
    customFields: CustomFieldModel[]
    constructor() {
        this.categoryId = 0;
        this.isSystemCategory = false;
        this.search = false;
        this.title = "";
        this.module = "";
        this.sortOrder = 0;
        this.required = false;
        this.hide = false;
        this.updatedOn = null;
        this.updatedBy = null;
        this.customFields = [];
    }
}

export class FieldsCategoryAddView extends CommonField {
    fieldsCategory: FieldsCategoryModel
    constructor() {
        super();
        this.fieldsCategory = new FieldsCategoryModel()
    }
}
export class FieldsCategoryListView extends CommonField {
    public fieldsCategoryList: [FieldsCategoryModel];
    public tenantId: string;
    public schoolId: number;
    public module: Module;
    constructor() {
        super();
        // this.module = ModuleIdentifier;
    }
}