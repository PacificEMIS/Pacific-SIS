import { CommonField } from './common-field.model';


export class AttendanceCodeCategoryModel extends CommonField {
    attendanceCodeCategories: AttendanceCodeCategories;
    constructor() {
        super();
        this.attendanceCodeCategories = new AttendanceCodeCategories();
    }
}

export class GetAllAttendanceCategoriesListModel extends CommonField {
    public attendanceCodeCategoriesList: AttendanceCodeCategories[];
    public tenantId: string;
    public schoolId: number;
    public academicYear: number;
    constructor() {
        super();
        this.attendanceCodeCategoriesList = [new AttendanceCodeCategories()];
    }
}

export class AttendanceCodeModel extends CommonField {
    public attendanceCode: AttendanceCode;
    constructor() {
        super()
        this.attendanceCode = new AttendanceCode();
    }
}

export class GetAllAttendanceCodeModel extends CommonField {
    public attendanceCodeList: AttendanceCode[];
    public tenantId: string;
    public schoolId: number;
    public attendanceCategoryId: number;
    public isListView:boolean;
    public academicYear: number;
    constructor() {
        super();
        this.attendanceCodeList = [new AttendanceCode];
    }
}

export class AttendanceCodeDragDropModel extends CommonField{
    tenantId:string;
    schoolId:number;
    previousSortOrder: number;
    currentSortOrder:number;
    attendanceCategoryId:number;
    constructor(){
        super();
        this.previousSortOrder=0;
        this.currentSortOrder=0;
        this.attendanceCategoryId=0;
    }


}

export class GetStudentAttendanceReport {
    schoolId:number;
    pageSize: number;
    pageNumber: number;
    markingPeriodStartDate:string;
    markingPeriodEndDate:string;
    gradeLevel: string;
    academicYear: number;
    _tenantName: string;
    _userName: string;
    _token: string;
    filterParams: filterParams[];
    constructor() {
        this.pageNumber = 1;
        this.pageSize = 10;
    }
}
export class GetStudentAttendanceReportExport {
    schoolId:number;
    pageSize: number;
    pageNumber: number;
    markingPeriodStartDate:string;
    markingPeriodEndDate:string;
    gradeLevel: string;
    academicYear: number;
    _tenantName: string;
    _userName: string;
    _token: string;
    filterParams: filterParams[];
    constructor() {
        this.pageNumber = 1;
        this.pageSize = 10;
        this.gradeLevel = null;
    }
}
export class AverageDailyAttendanceReportModel extends CommonField{
    tenantId:string;
    schoolId:number;
    markingPeriodStartDate:string;
    markingPeriodEndDate:string;
    academicYear: number;
    filterParams: filterParams[];
}

export class AverageAttendanceByDayReportModel extends CommonField{
    tenantId:string;
    schoolId:number;
    pageSize: number;
    pageNumber: number;
    markingPeriodStartDate:string;
    markingPeriodEndDate:string;
    academicYear: number;
    filterParams: filterParams[];
    constructor() {
        super();
        this.pageNumber = 1;
        this.pageSize = 10;
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

export class AttendanceCodeCategories {
    public tenantId: string;
    public schoolId: number;
    public attendanceCategoryId: number;
    public academicYear: number;
    public title: string;
    public createdBy: string;
    public createdOn: string;
    public updatedOn: string;
    public updatedBy: string;

    constructor() {
       
    }
}

export class AttendanceCode {
    tenantId: string;
    schoolId: number;
    attendanceCategoryId: number;
    attendanceCode1: number;
    academicYear: number;
    title: string;
    shortName: string;
    type: string;
    stateCode: string;
    defaultCode: boolean;
    allowEntryBy: string;
    sortOrder: number;
    createdBy: string;
    createdOn: string;
    updatedOn: string;
    updatedBy: string;

    constructor() {
       
    }
}

export class CourseSectionClass{
    courseId:number;
    courseSectionId:number;
    attendanceDate:string;
    periodId:number;
    blockId: number;
    takeAttendance: boolean;
    attendanceCategoryId: number;
}