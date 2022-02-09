import { CommonField } from "./common-field.model";


export class ViewGradebookGradeModel extends CommonField{
    academicYear: number;
    assignmentsListViewModels;
    markingPeriodId: number;
    courseSectionId: number;
    includeInactive: boolean
    showUngraded;
    SearchValue;
    MarkingPeriodStartDate: string;
    MarkingPeriodEndDate: string;

    constructor() {
        super();
    }
}
export class AddGradebookGradeModel extends CommonField{
    academicYear: number;
    assignmentsListViewModels;
    markingPeriodId: number | string;
    courseSectionId: number;
    includeInactive: boolean
    showUngraded;
    SearchValue;
    createdBy: string;

    constructor() {
        super();
    }
}



export class ViewGradebookGradeByStudentModel extends CommonField{
    academicYear: number;
    markingPeriodId: number;
    courseSectionId: number;
    includeInactive: boolean;
    studentId: number;
    showUngraded;
    SearchValue;
    markingPeriodStartDate: string;
    markingPeriodEndDate: string;

    constructor() {
        super();
    }
}

export class AddGradebookGradeByStudentModel extends CommonField{
    academicYear: number;
    assignmentTypeViewModelList;
    markingPeriodId: number | string;
    courseSectionId: number;
    includeInactive: boolean;
    studentId: number;
    showUngraded;
    SearchValue;
    createdBy: string;

    constructor() {
        super();
    }
}

export class ViewGradebookGradeByAssignmentTypeModel extends CommonField{
    academicYear: number;
    markingPeriodId: number;
    courseSectionId: number;
    includeInactive: boolean;
    assignmentTpyeId: number;
    showUngraded;
    SearchValue;

    constructor() {
        super();
    }
}

export class AddGradebookGradeByAssignmentTypeModel extends CommonField{
    academicYear: number;
    assignmentsListViewModels;
    markingPeriodId: number | string;
    courseSectionId: number;
    includeInactive: boolean;
    assignmentTpyeId: number;
    showUngraded;
    SearchValue;
    createdBy: string;

    constructor() {
        super();
    }
}