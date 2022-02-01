import { CommonField } from './common-field.model';
import { StudentAttendanceModel } from './take-attendance-list.model';
import { CourseSectionList } from './teacher-schedule.model';

export class StudentAttendanceListViewModel extends CommonField {
    studendAttendanceAdministrationList: StudendAttendanceAdministrationViewModel[];
    tenantId: string;
    schoolId: number;
    attendanceDate: string;
    attendanceCode: number;
    pageNumber: number;
    pageSize: number;
    _pageSize: number;
    totalCount: number;
    searchAllSchool: boolean;
    includeInactive: boolean;
    sortingModel: sorting;
    filterParams: filterParams[];
    constructor() {
        super()
        this.pageNumber = 1;
        this.pageSize = 10;
        this.sortingModel = null;
        this.filterParams = [];
    }
}

class sorting {
    sortColumn: string;
    sortDirection: string;
    constructor() {
        this.sortColumn = "";
        this.sortDirection = "";
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

export class StudendAttendanceAdministrationViewModel {
    tenantId: string;
    schoolId: number;
    studentId: number;
    studentGuid: string;
    firstGivenName: string;
    middleName: string;
    lastFamilyName: string;
    gradeLevelTitle: string;
    attendanceComment: string;
    present: string;
    gradeId: number;
    sectionId: number;
    studentAttendanceList: [];

}

export class StudentDailyAttendanceListViewModel extends CommonField {
    studentDailyAttendanceList: StudentDailyAttendance[];
    tenantId: string;
    schoolId: number;
    attendanceDate: string;
    createdBy: string;
    updatedBy: string;
    constructor() {
        super();
        this.studentDailyAttendanceList = [new StudentDailyAttendance()]
    }
}


export class StudentDailyAttendance {
    tenantId: string;
    schoolId: number;
    studentId: number;
    gradeId: number;
    gradeScaleId: number;
    sectionId: number;
    attendanceDate: string;
    attendanceCode: string;
    attendanceComment: string;
    attendanceMinutes: number;
    createdBy: string;
    createdOn: string;
    updatedOn: string;
    updatedBy: string;
}
export class CourseSectionForAttendanceViewModel extends CommonField {
    courseSectionViewList: CourseSectionList[];
    tenantId: string;
    schoolId: number;
    academicYear: number;
}

export class StudentAttendanceAddViewModel extends CommonField {
    studentAttendance: attendance[];  // for send items
    tenantId: string;
    schoolId: number;
    membershipId: number;
    studentId: number;
    courseSectionId: number;
    attendanceDate: string;
    periodId: number;
    staffId: number;
    createdBy: string;
    updatedBy: string;
    courseId: number;
    attendanceCategoryId: number;
    attendanceCode: number;
    absencesReason: string;


}

export class attendance{
    studentId: number; 
    attendanceDate: string;
}