import { CommonField } from "./common-field.model";


export class SearchFilter {
    public tenantId: string;
    public schoolId: number;
    public module: string;
    public filterId: number;
    public filterName: string;
    public emailaddress: string;
    public jsonList: string;
    public createdBy: string;
    public createdOn: string;
    public updatedBy: string;
    public updatedOn: string;
    constructor() {
        this.tenantId = sessionStorage.getItem("tenantId");
        this.schoolId = +sessionStorage.getItem("selectedSchoolId");
        this.module = null;
        this.filterName = null;
    }

}

export class SearchFilterAddViewModel extends CommonField {
    public searchFilter: SearchFilter;
    constructor() {
        super();
        this._tenantName = sessionStorage.getItem("tenant");
        this._userName = sessionStorage.getItem("user");
        this._token = sessionStorage.getItem("token");
        this.searchFilter= new SearchFilter();
    }
}

export class SearchFilterListViewModel extends CommonField {
    public searchFilterList: SearchFilter[];
    public tenantId: string;
    public schoolId: number;
    public module: string;
    constructor() {
        super();
        this.schoolId = + sessionStorage.getItem('selectedSchoolId');
        this.tenantId = sessionStorage.getItem("tenantId");
        this._tenantName = sessionStorage.getItem("tenant");
        this._userName = sessionStorage.getItem("user");
        this._token = sessionStorage.getItem("token");
    }
}