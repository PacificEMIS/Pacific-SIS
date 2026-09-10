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
    public noticeList: [];
    public enrollmentByGrade: GradeCount[];
    public repeatersByGrade: GradeCount[];
    public dropoutsByGrade: GradeCount[];
    public staffByProfile: NameCount[];
    public staffByJobTitle: NameCount[];
    public studentsByStatus: NameCount[];
    constructor() {
        super();
        this.noticeList = [];
        this.enrollmentByGrade = [];
        this.repeatersByGrade = [];
        this.dropoutsByGrade = [];
        this.staffByProfile = [];
        this.staffByJobTitle = [];
        this.studentsByStatus = [];
    }
}

export class GradeCount {
    gradeId: number;
    gradeLevelTitle: string;
    count: number;
    sortOrder: number;
}

export class NameCount {
    name: string;
    key: string;
    count: number;
}

export class AttendanceTakenRecord {
    courseSectionId: number;
    periodId: number;
    attendanceDate: string;
    attendanceCount: number;
    enrolledCount: number;
}

export class ScheduledCourseSectionViewModel extends CommonField{
    courseSectionViewList:AllCourseSectionView[];
    attendanceTakenList: AttendanceTakenRecord[];
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
    markingPeriodStartDate: string;
    markingPeriodEndDate: string;
}
