
import { CommonField } from './common-field.model'
import { StudentListView } from './student.model';

export class StudentFinalGrade {
    public tenantId: string;
    public schoolId: number;
    public studentId: number;
    public commentId: number;
    public studentFinalGradeSrlno: number;
    public basedOnStandardGrade: boolean;
    public courseId: number;
    public gradeId: number;
    public gradeScaleId: number;
    public academicYear: number;
    public calendarId: number;
    public yrMarkingPeriodId: number;
    public smstrMarkingPeriodId: number;
    public qtrMarkingPeriodId: number;
    public isPercent: boolean;
    public percentMarks: number;
    public gradeObtained: string;
    public teacherComment: string;
    public createdBy: string;
    public createdOn: string;
    public updatedBy: string;
    public updatedOn: string;
    public studentFinalGradeComments: StudentFinalGradeComments[];
    public studentFinalGradeStandard: StudentFinalGradeStandard[];

    constructor() {
        this.studentFinalGradeStandard = [];
    }

}

export class StudentFinalGradeComments {
    courseCommentId: number;
}

export class StudentFinalGradeStandard {
    standardGradeScaleId: number;
    gradeObtained: number;
}

export class AddUpdateStudentFinalGradeModel extends CommonField {
    studentFinalGradeList = [];
    courseStandardList: [];
    tenantId: string;
    schoolId: number;
    calendarId: number;
    courseSectionId: number;
    standardGradeScaleId: number;
    courseId: number;
    isPercent: boolean;
    markingPeriodId: string;
    creditHours: number;
    academicYear: number;
    isExamGrade: boolean;
    isCustomMarkingPeriod: boolean;
    createdOrUpdatedBy: string;
}

export class GetAllStudentListForFinalGradeModel extends CommonField {
    public studentListViews: StudentListView[];
    public studentId: number;
    public enrollmentCode: number;
    public enrollmentDate: string;
    public gradeId: number;
    public gradeLevelTitle: string;
    public academicYear: number;
    public updatedBy: string;
    public totalCount: number;
    public pageNumber: number;
    public pageSize: number;
    public _pageSize: number;
    public filterParams: filterParams[];
    public dobStartDate: string;
    public dobEndDate: string;
    public includeInactive: boolean;
    public emailAddress: string;
    constructor() {
        super();
        this.pageNumber = 1;
        this.pageSize = 10;
        this.filterParams = [];
    }
}

export class GetGradebookGradeinFinalGradeModel extends CommonField {
    public courseSectionId: number;
    public markingPeriodId: string;
    public academicYear: number;

    constructor() {
        super();
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