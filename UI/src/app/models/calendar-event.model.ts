import { CommonField } from './common-field.model';


export class CalendarEventModel {
    public tenantId: string;
    public schoolId: number;
    public calendarId: number;
    public eventId :number;
    public title: string;
    public academicYear: number;
    public schoolDate: string;
    public description: string;
    public createdBy: string;
    public createdOn: string;
    public updatedOn: string;
    public updatedBy: string;
    public visibleToMembershipId :string;
    public eventColor :string;
    public systemWideEvent :boolean;
    public startDate :string;
    public endDate : string;
   
    constructor() {
      
    }
}

export class CalendarEventAddViewModel extends CommonField {
    public schoolCalendarEvent: CalendarEventModel;
    constructor() {
        super();
        this.schoolCalendarEvent = new CalendarEventModel();
    }
}


export class CalendarEventListViewModel extends CommonField {
    public calendarEventList: CalendarEventModel[];
    public tenantId: string;
    public schoolId: number;
    public calendarId: number;
    public membershipId: number;
    public academicYear: number;
    constructor() {
        super();
        this.calendarEventList = [];
    }
}

export interface colors {
    name: string;
    value: string;
    
  }