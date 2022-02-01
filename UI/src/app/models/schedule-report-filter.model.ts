import { CommonField } from "./common-field.model";
import { CourseSectionAddViewModel } from "./course-section.model";

export class ScheduleReportFilterModel extends CommonField {
    courseId: string;            //=======>
    courseSubject: string;
    courseProgram: string;
    blockPeriodId: string;        //======>
    staffId: string;
  courseSectionViewList: any;
    constructor() {
        super();
        this.courseId = null;
        this.courseSubject = null;
        this.courseProgram = null;
        this.blockPeriodId = null;
        this.staffId = null;
    }
}

export class ScheduleReportFilterViewModel extends CommonField {
    tenantId: string;
    public schoolId: number;
    public schoolName: string;
    public courseId: number;
    public courseSubject: string;
    public courseProgram: string;
    public blockPeriodId: number;
    public staffId: string;
    public courseSectionViewList: CourseSectionAddViewList[]
}

export class CourseSectionAddViewList {
    public studentLists: any;
    public tenantId: string;
    public schoolId: number;
    public courseId: number;
    public courseTitle: string
    public courseSubject: string
    public courseProgram: string
    public courseSectionId: number
    public courseSectionName: string
    public staffName: string
    public scheduledStudentCount: number
    public availableSeat: number
    public totalSeats: number
}

export class ScheduleReportStaddAndCourseListFilterModel {
    subject: string;
    course: string;
    courseId: string;
    staffId: string;
    periodId: string;

    
    constructor() {
        this.subject = null,
        this.course = null,
        this.courseId = null,
        this.staffId = null,
        this.periodId = null
    }
}

