export class UserAccessLogModel{
      public tenantId:string;
      public schoolId:string;
      public emailaddress:string;
      public userName:string;
      public membershipId:string;
      public profile:string;
      public loginAttemptDate:string;
      public loginFailureCount:string;
      public loginStatus:string;
      public ipaddress:string;
      public createdBy:string;
      public createdOn:string;
      public updatedBy:string;
      public updatedOn:string;
}
export class GetAccessLogInfoModel {
    public tenantId: string;
    public pageNumber: number;
    public pageSize: number;
    public sortingModel: string;
    public filterParams: filterParams[];
    public dobStartDate : Date|string;
    public dobEndDate : Date|string;
    public _userName: string;
    public _token: string;
    public _tenantName: string;
    public _failure: boolean;
    public isDelete:boolean;
    public _message: string; 
    totalCount: number;
    _pageSize: number;
    userAccessLogList:[UserAccessLogModel]
    constructor(){
        this.pageNumber = 1;
        this.pageSize = 10;
        this.filterParams = null;
        this._message = "";
        this._failure=false;
        this.isDelete=false;
    }
}
export class filterParams {
    columnName: string;
    filterValue: string;
    filterOption: number;
    constructor() {
      this.columnName = null;
      this.filterOption = 1;
      this.filterValue = null;
    }
  }