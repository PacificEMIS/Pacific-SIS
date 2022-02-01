import { CommonField } from "./common-field.model";

export class AddCourseCommentCategoryModel extends CommonField{
    courseCommentCategory:ReportCardCommentModel[];
    constructor(){
        super();
        this.courseCommentCategory=[]
    }
    
}

export class DeleteCourseCommentCategoryModel extends CommonField{
    courseId: number;
    courseCommentId: number;
    constructor(){
        super();
    }
}

export class UpdateSortOrderForCourseCommentCategoryModel extends CommonField{
    courseId: number;
    previousSortOrder: number;
    currentSortOrder: number;
    constructor(){
        super();
    }
}

export class GetAllCourseCommentCategoryModel extends CommonField{
    courseCommentCategories:ReportCardCommentModel[];
    academicYear: number;
    isListView:boolean;
    constructor(){
        super();
        this.courseCommentCategories=[]
    }
}

export class ReportCardCommentModel extends CommonField{
    courseCommentId: number;
    courseId: number;
    courseName: string;
    applicableAllCourses: boolean;
    comments: string;
    sortOrder: number;
    createdBy: string;
    createdOn: string;
    updatedBy: string;
    updatedOn: string;
    
    constructor(){
        super();
    }
}

export class DistinctGetAllReportCardModel extends CommonField{
    courseCommentCategories:DistinctReportCardModel[];
    constructor(){
        super();
        this.courseCommentCategories=[]
    }
}
export class DistinctReportCardModel{
    static: boolean; //This is only for Front End Uses
    courseCommentId: number;
    courseId: number;
    courseName: string;
    applicableAllCourses: boolean;
    comments: CommentModel[];
    sortOrder: number;
    createdBy?: string;
    createdOn?: string;
    updatedBy?: string;
    updatedOn?: string;
}

export class CommentModel{
    takeInput:boolean; //This is only for Front End Uses
    courseId: number;
    courseName: string;
    applicableAllCourses:boolean;
    courseCommentId:number;
    comment: string;
    sortOrder: number;
}

export class StudentDetails{
    studentId: number
}

export class AddReportCardPdf extends CommonField{
    academicYear: number;
    markingPeriods: string;
    teacherName: boolean;
    teacherComments: boolean;
    parcentage: boolean;
    gpa: boolean;
    yearToDateDailyAbsences: boolean;
    dailyAbsencesThisMarkingPeriod: boolean;
    otherAttendanceCodeYearToDate: boolean;
    studentsReportCardViewModelList: StudentDetails[];
    templateType: string;
    createdBy: string;
    createdOn: string;
    updatedBy: string;
    updatedOn: string;
    
    constructor() {
        super();
        this.studentsReportCardViewModelList = [];
        this.teacherName = true;
        this.teacherComments = true;
        this.parcentage = true;
        this.gpa = true;
        this.yearToDateDailyAbsences = true;
        this.dailyAbsencesThisMarkingPeriod = true;
        this.otherAttendanceCodeYearToDate = true;
    }
}

export class StudentReportCardGradesModel extends CommonField {
    courseSectionWithGradesViewModelList: courseSectionWithGradesViewModelList[];
    studentId: number;
    markingPeriodId: string;
    academicYear: number;
    constructor() {
        super();
        this.courseSectionWithGradesViewModelList = [];
    }
}

export class ResponseStudentReportCardGradesModel extends CommonField {
    courseSectionWithGradesViewModelList: courseSectionWithGradesViewModelList[];
    studentId: number;
    markingPeriodId: string;
    academicYear: number;
    updatedBy: string;
    studentInternalId: string;
    firstGivenName: string;
    middleName: string;
    lastFamilyName: string;
    studentPhoto: string;
    weightedGPA: number;
    unWeightedGPA: number;
    gredeLavel: string;
    constructor() {
        super();
        this.courseSectionWithGradesViewModelList = [];
    }
}

export class courseSectionWithGradesViewModelList {
    courseId: number;
    courseSectionId: number;
    studentFinalGradeSrlno: number;
    courseSectionName: string;
    percentMarks: number;
    gradeObtained: string;
    gpValue: number;
    weightedGP: string;
    gradeId: number;
    gradeScaleId: number;
    gradeScaleName: string;
    gradeScaleValue: number;
    creditEarned: number;
    creditAttempted: number;
    createdBy: string;
    createdOn: string;
    updatedBy: string;
    updatedOn: string;
}