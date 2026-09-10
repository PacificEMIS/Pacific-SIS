import { CommonField } from "./common-field.model";
import { TableSchoolSemester } from "./marking-period.model";


export class RolloverViewModel extends CommonField {
    schoolRollover: SchoolRollover;
    fullYearName: string;
    fullYearShortName: string;
    rolloverStatus: boolean;
    doesExam: boolean;
    doesGrades: boolean;
    doesComments: boolean;
    semesters: TableSchoolSemester[];
    constructor(){
        super();
        this.schoolRollover= new SchoolRollover();
        this.semesters= [new TableSchoolSemester()];
    }
   
}





export class SchoolRollover {
    tenantId: string;
    schoolId: number;
    rolloverId: number;
    reenrollmentDate: string;
    schoolBeginDate: string;
    schoolEndDate: string;
    rolloverContent: string;
    rolloverStatus: boolean;
    CreatedBy: string;
    CreatedOn: string;
    UpdatedBy: string;
    UpdatedOn: string;
}

// Read-only pre-rollover completeness summary (mirrors RolloverReadinessViewModel).
export class RolloverReadinessViewModel extends CommonField {
    academicYear: number;
    totalStudents: number;
    studentsByGradeGender: GradeGenderCount[];
    dispositions: DispositionGradeGender[];
    terminalGradeNeedsReview: GradeGenderCount[];
    terminalGradeTitles: string[];
    exitsByCode: ExitCodeGradeGender[];
    sectionsMissingAttendance: SectionCompletenessRow[];
    sectionsMissingGrades: SectionCompletenessRow[];
    unlinkedEnrollmentCount: number;
    disabledWithOpenEnrollmentCount: number;
    constructor() {
        super();
        this.studentsByGradeGender = [];
        this.dispositions = [];
        this.terminalGradeNeedsReview = [];
        this.terminalGradeTitles = [];
        this.exitsByCode = [];
        this.sectionsMissingAttendance = [];
        this.sectionsMissingGrades = [];
    }
}

export class GradeGenderCount {
    gradeId: number;
    gradeLevelTitle: string;
    gender: string;
    count: number;
    sortOrder: number;
}

export class DispositionGradeGender {
    dispositionKey: string;
    total: number;
    rows: GradeGenderCount[];
}

export class ExitCodeGradeGender {
    exitCodeTitle: string;
    total: number;
    rows: GradeGenderCount[];
}

export class SectionCompletenessRow {
    courseSectionId: number;
    courseTitle: string;
    courseSectionName: string;
    teacherName: string;
    markingPeriodTitle: string;
    count: number;
}