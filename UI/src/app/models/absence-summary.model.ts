import { CommonField } from "./common-field.model";


export class GetStudentAbsenceReport {
    schoolId: number;
    pageSize: number;
    pageNumber: number;
    studentId: number;
    markingPeriodStartDate: string;
    markingPeriodEndDate: string;
    periodId: number|string;
    _tenantName: string;
    _userName: string;
    _token: string;
    filterParams: filterParams[];
    constructor() {
        this.pageNumber = 1;
        this.pageSize = 10;
        this.periodId='';
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
export class StudentListForAbsenceSummary extends CommonField {
    studendAttendanceList: StudentListForAbsence[];
    tenantId: string;
    schoolId: number;
    totalCount: number;
    pageNumber: number;
    _pageSize: number;
}



export class StudentListForAbsence {
    tenantId: string;
    schoolId: number;
    studentId: number;
    studentGuid: string;
    attendanceDate: string;
    studentInternalId: string;
    studentAlternetId: string;
    firstGivenName: string;
    middleName: string;
    lastFamilyName: string;
    gradeLevelTitle: string;
    homePhone: string;
    absentCount: number;
    halfDayCount: number;
}

export class AbsenceListByStudent extends CommonField {
    studendList: AbsenceStudentModel[];
    studentInternalId: string;
    firstGivenName: string;
    middleName: string;
    lastFamilyName: string;
    gradeLevelTitle: string;
    studentPhoto: string;
    tenantId: string;
    schoolId: number;
    totalCount: number;
    pageNumber: number;
    _pageSize: number;
}

export class AbsenceStudentModel {
    tenantId: string;
    schoolId: number;
    studentId: number;
    absenceDate: string;
    attendance: string;
    adminComment: string;
    teacherComment: string;
}

export class GetStudentAbsenceReportForSearch {
    schoolId: number;
    pageSize: number;
    pageNumber: number;
    studentId: number;
    markingPeriodStartDate: string;
    markingPeriodEndDate: string;
    academicYear: string;
    periodId: number|string;
    courseSectionId: number;
    membershipType: string;
    _tenantName: string;
    _userName: string;
    _token: string;
    filterParams: filterParams[];
    customFieldFilter: customFieldFilter[];
    sortingModel: sortingModel;
    constructor() {
        this.pageNumber = 1;
        this.pageSize = 10;
        this.periodId='';
        this.sortingModel = new sortingModel();
    }
}

class sortingModel {
    sortColumn: string;
    sortDirection: string;
    constructor() {
        this.sortColumn = 'lastFamilyName';
        this.sortDirection = 'asc';
    }
}

export class customFieldFilter {
    customFieldTitle: string;
    customFieldValue: string;
    constructor() {
      this.customFieldTitle = null;
      this.customFieldValue = null;
    }
  }