import { CommonField } from "./common-field.model";
import { StudentMasterModel } from "./student.model";

export class GetStudentAddDropReportModel extends CommonField {
    studentEnrollmentList: StudentEnrollmentListModel[];
    markingPeriodStartDate: string;
    markingPeriodEndDate: string;
    gradeLevel: string;
    totalCount: number;
    pageNumber: number;
    pageSize: number;
    _pageSize: number;
    filterParams: filterParams[];
    constructor() {
        super();
        this.pageNumber = 1;
        this.pageSize = 10;
        this.filterParams = [];
        this.gradeLevel = '';
    }
}

export class filterParams {
    columnName: string;
    filterValue: string;
    filterOption: number;
    constructor() {
        this.columnName = null;
        this.filterOption = 3;
        this.filterValue = null;
    }
}

export class StudentEnrollmentListModel {
    studentMaster: StudentMasterModel;
    rollingOption: string;
    schoolName: string;
    enrollmentDate: string;
    enrollmentCode: string;
    exitDate: string;
    exitCode: string;
}

export class GetScheduledAddDropReportModel extends CommonField {
    studentCoursesectionScheduleList: StudentCoursesectionScheduleListModel[];
    markingPeriodStartDate: string;
    markingPeriodEndDate: string;
    totalCount: number;
    pageNumber: number;
    pageSize: number;
    _pageSize: number;
    filterParams: filterParams[];
    constructor() {
        super();
        this.pageNumber = 1;
        this.pageSize = 10;
        this.filterParams = [];
    }
}

export class StudentCoursesectionScheduleListModel {
    tenantId: string;
    schoolId: number;
    courseId: number;
    courseSectionId: number;
    studentId: number;
    studentGuid: string;
    checked: boolean; // This is not a backend key
    studentInternalId: string;
    studentName: string;
    staffName: string;
    courseName: string;
    courseSectionName: string;
    enrolledDate: string;
    dropDate: string;
}

export class GetStudentEnrollmentReportModel extends CommonField {
    studentListViews:GetStudentEnrollmentReportViews[];
    pageNumber: number;
    filterParams: filterParams[];
    pageSize: number;
    markingPeriodStartDate: string;
    markingPeriodEndDate : string;
    gradeLevel :string;
    tenantId: string;
    schoolId: number;
    _pageSize: number;
    includeInactive:boolean;
    totalCount: number;
    constructor() {
        super();
        this.pageNumber = 1;
        this.pageSize = 10;
        this.filterParams = [];
    }
}

export class GetStudentEnrollmentReportViews {
    tenantId: string;
    firstGivenName: string;
    middleName: string;
    lastFamilyName: string;
    alternateId: string;
    gradeLevelTitle: string;
    sectionName: string;
    mobilePhone: string;
    courseId: number;
    courseSectionId: number;
    studentId: number;
    studentGuid: string;
    checked: boolean; // This is not a backend key
    studentInternalId: string;
    studentName: string;
    staffName: string;
    courseName: string;
    courseSectionName: string;
    enrolledDate: string;
    dropDate: string;
}

export class GetStudentAdvancedReportModel extends CommonField{
    studentGuids: any[]

    constructor() {
        super();
        this.studentGuids = [];
    }
}

export class GetStaffAdvancedReportModel extends CommonField{
    staffGuids: any[]

    constructor() {
        super();
        this.staffGuids = [];
    }
}

export class GetSchoolReportModel extends CommonField {
    schoolIds: any[];
    isGeneralInfo: boolean;
    isAddressInfo: boolean;
    isContactInfo: boolean;
    isWashInfo: boolean;
    isCustomCategory: boolean;

    constructor() {
        super();
        this.schoolIds = [];
        this.isGeneralInfo = false;
        this.isAddressInfo = false;
        this.isContactInfo = false;
        this.isWashInfo = false;
        this.isCustomCategory = false;
    }
}