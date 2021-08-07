import { CommonField } from './common-field.model';

export class NoticeModel {
    public tenantId: string;
    public schoolId: number;
    public noticeId: number;
    public targetMembershipIds: string;
    public title: string;
    public body: string;
    public validFrom: string;
    public validTo: string;
    public visibleToAllSchool: boolean;
    public sortOrder: number;
    public createdBy: string;
    public createdOn: string;
    public updatedOn: string;
    public updatedBy: string;
    constructor(){
        this.visibleToAllSchool = false;
    }
}

export class NoticeAddViewModel extends CommonField {
    public notice: NoticeModel;
    constructor() {
        super();
        this.notice = new NoticeModel();
    }
}


export class NoticeListViewModel extends CommonField {
    public noticeList: NoticeModel[];
    public tenantId: string;
    public schoolId: number;
    public membershipId: number;
    constructor() {
        super();
        this.noticeList = [];
    }
}

