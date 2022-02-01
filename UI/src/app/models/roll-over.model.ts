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