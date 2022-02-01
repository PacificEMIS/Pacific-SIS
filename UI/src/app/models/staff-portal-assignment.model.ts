import { CommonField } from "./common-field.model";

export class AddAssignmentTypeModel extends CommonField{
    assignmentType: AssignmentType;
    constructor(){
        super();
        this.assignmentType = new AssignmentType();
    }
}

export class AssignmentType extends CommonField{
    assignmentTypeId: number;
    academicYear: number;
    markingPeriodId: number;
    courseSectionId: number;
    title: string;
    weightage: number;
    createdBy: string;
    createdOn: string;
    updatedBy: string;
    updatedOn: string;
    constructor(){
        super();
    }
}

export class GetAllAssignmentsModel extends CommonField{
    assignmentTypeList:AssignmentList[]
    courseSectionId: number;
    totalWeightage: number;
    academicYear: number;
    constructor(){
        super();
    }
}

export class AssignmentList extends CommonField{
      assignmentTypeId: number;
      academicYear: number;
      markingPeriodId: number;
      courseSectionId: number;
      title: string;
      weightage: number;
      assignment:AssignmentModel[]
      createdBy: string;
      createdOn: string;
      updatedBy: string;
      updatedOn: string;
      constructor(){
        super();
    }
}

export class AddAssignmentModel extends CommonField{
    assignment:AssignmentModel;
    courseSectionIds:number[];
    constructor(){
        super();
        this.assignment = new AssignmentModel()
    }
}

export class AssignmentModel extends CommonField{
    assignmentTypeId: number;
    assignmentId: number;
    courseSectionId: number;
    assignmentTitle: string;
    points: number;
    assignmentDate: string;
    dueDate: string;
    assignmentDescription: string;
    staffId: number;
    createdBy: string;
    createdOn: string;
    updatedBy: string;
    updatedOn: string;
    constructor(){
        super();
    }
}