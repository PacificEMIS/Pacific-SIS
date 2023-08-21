import { CommonField } from './common-field.model';
import { SchoolMasterModel } from './school-master.model';

export class CalendarModel {
    public tenantId: string;
    public schoolId: number;
    public calenderId: number;
    public title: string;
    public academicYear: number;
    public defaultCalender: boolean;
    public sessionCalendar: boolean;
    public days: string;
    public rolloverId: number;
    public createdBy: string;
    public createdOn: string;
    public updatedOn: string;
    public updatedBy: string;
    public visibleToMembershipId :string;
    public startDate :string;
    public endDate : string;
    public calendarEventList:string
    constructor() {
        
        
    }
}
export interface Weeks {
    name: string;
    id: number;
  }

export class CalendarAddViewModel extends CommonField {
    public schoolCalendar: CalendarModel;
    constructor() {
        super();
        this.schoolCalendar = new CalendarModel();
    }
}

export class BellScheduleModel {
    tenantId: string;
    schoolId: number;
    academicYear: number;
    bellScheduleDate: string;
    blockId: number;
    createdBy: string;
    constructor() {
    }
}

export class CalendarBellScheduleModel extends CommonField {
    public bellSchedule: BellScheduleModel;
        
    constructor() {
        super();
        this.bellSchedule = new BellScheduleModel();
    }
}

export class CalendarBellScheduleViewModel extends CommonField {
    public academicYear: number;
        
    constructor() {
        super();
    }
}


export class CalendarListModel extends CommonField {
    public calendarList: CalendarModel[];
    public tenantId: string;
    public schoolId: number;
    public membershipId: number;
    public academicYear :number;
    constructor() {
        super();
        this.calendarList = [];
    }
}

export class GetCalendarAndHolidayListModel {
    public _tenantName: string;
    public _userName: string;
    public _token: string;
    public schoolCalendar: SchoolCalendar;
    constructor() {
        this.schoolCalendar = new SchoolCalendar();
    }
}

class SchoolCalendar {
    schoolId: number;
    tenantId: string;
    academicYear: number
}