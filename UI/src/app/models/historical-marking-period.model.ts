
import { CommonField } from "./common-field.model";


export class HistoricalMarkingPeriodListModel extends CommonField {
    historicalMarkingPeriodList: HistoricalMarkingPeriod[];
    tenantId: string;
    schoolId: number;
    totalCount: number;
    filterParams: filterParams[];
    pageNumber: number;
    _pageSize: number;
    pageSize: number;
    academicYear: number;
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

export class HistoricalMarkingPeriod {
    tenantId: string;
    schoolId: number;
    histMarkingPeriodId: number;
    academicYear: number;
    title: string;
    gradePostDate: string;
    doesGrades: boolean;
    doesExam: boolean;
    doesComments: boolean;
    rolloverId: number;
    createdBy: string;
    createdOn: string;
    updatedBy: string;
    updatedOn: string;
    constructor() {
        this.doesGrades = false;
        this.doesExam = false;
        this.doesComments = false;
    }
}

export class HistoricalMarkingPeriodAddViewModel extends CommonField {
    historicalMarkingPeriod: HistoricalMarkingPeriod;
    constructor() {
        super();
        this.historicalMarkingPeriod = new HistoricalMarkingPeriod();
    }
}


export class HistoricalGrade {
    historicalCreditTransfer: HistoricalCreditTransfer[]
    tenantId: string;
    schoolId: number;
    histGradeId: number;
    histMarkingPeriodId: number;
    schoolName: string;
    equivalencyId: number;
    createdBy: string;
    gradeAddMode: boolean; // for angular view
    gradeViewMode: boolean; // for angular view
    createdOn: string;
    updatedBy: string;
    updatedOn: string;
    constructor() {
        this.historicalCreditTransfer= [new HistoricalCreditTransfer()];
        this.histMarkingPeriodId=0;
        this.gradeAddMode= true;
    }
}

export class HistoricalCreditTransfer {
    tenantId: string;
    schoolId: number;
    studentId: number;
    histGradeId: number;
    histMarkingPeriodId: number;
    courseCode: string;
    courseName: string;
    percentage: number;
    letterGrade: number;
    gpValue: number;
    calculateGpa: boolean;
    weightedGp: boolean;
    gradeScale: number;
    creditAttempted: number;
    creditEarned: number;
    creditAddMode: boolean; // for angular view
    creditViewMode: boolean; // for angular view
    createdBy: string;
    createdOn: string;
    updatedBy: string;
    updatedOn: string;
    constructor(){
        this.creditAddMode= true;
    }

}

export class HistoricalGradeAddViewModel extends CommonField {
    historicalGradeList: HistoricalGrade[];
    tenantId: string;
    schoolId: number;
    studentId: number;
    CreatedBy: string;
    constructor(){
        super();
        this.historicalGradeList= [new HistoricalGrade()]
    }
}
