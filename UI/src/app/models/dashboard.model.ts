import { CalendarEventModel } from "./calendar-event.model";
import { CalendarModel } from "./calendar.model";
import { CommonField } from "./common-field.model";
import { AllCourseSectionView } from "./course-manager.model";
import { NoticeAddViewModel, NoticeModel } from "./notice.model";


export class DashboardViewModel extends CommonField{
    tenantId: string;
    public schoolId: number;
    public superAdministratorName: string;
    public academicYear: number;
    public schoolName: string;
    public totalStudent: number;
    public totalStaff: number;
    public totalParent: number;
    public noticeTitle: string;
    public noticeBody :string;
    public lastUsedSchoolId:number;
    public schoolCalendar : CalendarModel;
    public calendarEventList : CalendarEventModel[];
    public membershipId : number;
}

export class ScheduledCourseSectionViewModel extends CommonField{
    courseSectionViewList:AllCourseSectionView[];
    missingAttendanceCount: number;
    tenantId: string;
    schoolId: number;
    totalCount:number;
    pageNumber:number;
    _pageSize:number;
    staffId: number;
    allCourse: boolean;
    public membershipId : number;
    noticeList:NoticeModel[];
    notificationList:any;
    academicYear:any;
}
