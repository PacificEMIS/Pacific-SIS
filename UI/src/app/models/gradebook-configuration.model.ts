import { CommonField } from "./common-field.model";
import { TableProgressPeriod, TableQuarter, TableSchoolSemester, TableSchoolYear } from "./marking-period.model";

export class GradebookConfigurationProgressPeriods {
    id: number;
    tenantId: string;
    schoolId: number;
    courseId: number;
    courseSectionId: number;
    academicYear: number;
    gradebookConfigurationId: number;
    prgrsprdMarkingPeriodId: number;
    gradingPercentage: number;
    examPercentage: number;
    title: string;  // for view
    doesGrades: boolean;  // for view
    doesExam: boolean;  // for view
    createdBy: string;
    createdOn: string;
    updatedBy: string;
    updatedOn: string;
}

export class GradebookConfigurationQuarter{
    id: number;
    tenantId: string;
    schoolId: number;
    courseId: number;
    courseSectionId: number;
    academicYear: number;
    gradebookConfigurationId: number;
    qtrMarkingPeriodId: number;
    prgrsprdMarkingPeriodId: number;
    gradingPercentage: number;
    examPercentage: number;
    title: string;  // for view
    doesGrades:boolean;  // for view
    doesExam:boolean;  // for view
    createdBy: string;
    createdOn: string;
    updatedBy: string;
    updatedOn: string;
    isProgressPeriodExists?: boolean;
}

export class GradebookConfigurationSemester{
    id: number;
    tenantId: string;
    schoolId: number;
    courseId: number;
    courseSectionId: number;
    academicYear: number;
    gradebookConfigurationId: number;
    smstrMarkingPeriodId: number;
    qtrMarkingPeriodId: number;
    gradingPercentage: number;
    examPercentage: number;
    title: string;  // for view
    doesGrades:boolean;  // for view
    doesExam:boolean;  // for view
    createdBy: string;
    createdOn: string;
    updatedBy: string;
    updatedOn: string;
    isQuarterExists?: boolean;
}


export class GradebookConfigurationYear{
    id: number;
    tenantId: string;
    schoolId: number;
    courseId: number;
    courseSectionId: number;
    academicYear: number;
    gradebookConfigurationId: number;
    smstrMarkingPeriodId: number;
    yrMarkingPeriodId: number;
    gradingPercentage: number;
    examPercentage: number;
    title: string;  // for view
    doesGrades:boolean;  // for view
    doesExam:boolean;  // for view
    createdBy: string;
    createdOn: string;
    updatedBy: string;
    updatedOn: string;
    isSemesterExists?: boolean;
}

export class GradebookConfigurationGradescale{
    id: number;
    tenantId: string;
    schoolId: number;
    courseId: number;
    courseSectionId: number;
    academicYear: number;
    gradebookConfigurationId: number;
    title:string; // for view
    gradeScaleId: number;
    gradeId: number;
    breakoffPoints: number;
    createdBy: string;
    createdOn: string;
    updatedBy: string;
    updatedOn: string;
}

export class GradebookConfiguration {
    tenantId: string;
    schoolId: number;
    courseId: number;
    courseSectionId: number;
    academicYear: number;
    gradebookConfigurationId: number;
    general: string;
    scoreRounding: string;
    assignmentSorting: string;
    maxAnomalousGrade: number;
    upgradedAssignmentGradeDays: number;
    gradebookConfigurationGradescale: GradebookConfigurationGradescale[];
    gradebookConfigurationProgressPeriods: GradebookConfigurationProgressPeriods[];
    gradebookConfigurationQuarter: GradebookConfigurationQuarter[];
    gradebookConfigurationSemester:GradebookConfigurationSemester[];
    gradebookConfigurationYear: GradebookConfigurationYear[];
    createdBy: string;
    createdOn: string;
    updatedBy: string;
    updatedOn: string;
    constructor(){
        this.scoreRounding = 'up';
        this.assignmentSorting = 'newestFirst';
        this.gradebookConfigurationProgressPeriods = [];
        this.gradebookConfigurationQuarter= [];
        this.gradebookConfigurationSemester= [];
        this.gradebookConfigurationYear= [];
    }
}

export class GradebookConfigurationAddViewModel extends CommonField{
    gradebookConfiguration: GradebookConfiguration;
    constructor() {
        super();
        this.gradebookConfiguration = new GradebookConfiguration();
    }
}

export class FinalGradingMarkingPeriodList extends CommonField{
    tenantId: string;
    schoolId: number;
    academicYear: number;
    progressPeriods: TableProgressPeriod[];
    quarters: TableQuarter[];
    semesters: TableSchoolSemester[];
    schoolYears: TableSchoolYear;
    courseSectionId: number;
    isConfiguration: boolean;
}