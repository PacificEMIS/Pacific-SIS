import { CommonField } from "./common-field.model";
import { PermissionGroup } from "./roll-based-access.model";



export class UserViewModel extends CommonField {
    
    public password : string;
    public email: string;
    public name: string;
    public membershipName : string;
    public membershipType: string;
    public membershipId:number;
    public userId?:  number;
    public userGuid:  string;
    public tenantId: string;
    public userPhoto: string;
    public schoolId:number;
    public permissionList: PermissionGroup[] | any;
    public createdBy: string;
    public createdOn: string;
    public updatedOn: string;
    public updatedBy: string;
    public firstGivenName:string;
    public lastUsedSchoolId:number;
    public userAccessLog: UserAccessLogModel;
    constructor() {
        super();
        this.tenantId ="1E93C7BF-0FAE-42BB-9E09-A1CEDC8C0355";
        this.userId = 0;
        this.userAccessLog=new UserAccessLogModel();
    }
}
class UserAccessLogModel{
    ipaddress:string;
    Emailaddress: string;
}
export class CheckUserEmailAddressViewModel extends CommonField {
    public tenantId: string;
    public emailAddress: string;
    public isValidEmailAddress: boolean;
}

export class DataAvailablity{
    schoolLoaded:boolean;
    schoolChanged:boolean;
    dataFromUserLogin:boolean;
    academicYearChanged: boolean;
    academicYearLoaded: boolean;
}

export class UserLogoutModel extends CommonField {
    email: string;
    constructor() {
        super();
    }
}
