import { CommonField } from "./common-field.model";

export class AnomalousGradeViewModel extends CommonField {
    studentAnomalsGrades: StudentAnomalsGrade[] = [];
    staffId: number;
    studentId?: number;
    courseSectionId?: number;
    assignmentTypeId?: number;
    assignmentId?: number;
    academicYear?: number;
    yrMarkingPeriodId?: number;
    smstrMarkingPeriodId?: number;
    qtrMarkingPeriodId?: number;
    prgrsprdMarkingPeriodId?: number;
    searchValue?: string;
    includeInactive?: boolean;
    totalCount?: number;
    pageNumber: number = 1;
    pageSize: number = 10;
    constructor() {
        super();
    }
}

export class StudentAnomalsGrade {
    firstGivenName?: string;
    middleName?: string;
    lastFamilyName?: string;
    studentId?: number;
    studentInternalId?: string;
    courseSectionId?: number;
    assignmentTypeId?: number;
    assignmentId?: number;
    assignmentTypeTitle?: string;
    assignmentTitle?: string;
    points?: number;
    allowedMarks?: string;
    comment?: string;
}