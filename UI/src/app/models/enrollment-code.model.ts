import { CommonField } from "./common-field.model";

export class EnrollmentCodeModel{
      tenantId: string;
      schoolId: number;
      enrollmentCode: number;
      academicYear: number;
      title: string;
      shortName: string;
      sortOrder: number;
      type: string;
      createdBy: string;
      createdOn: string;
      updatedOn: string;
      updatedBy: string;
      constructor(){
        this.enrollmentCode= 0;
        this.title= null;
        this.shortName= null;
        this.sortOrder= null;
        this.type= null;
      }
  }

export class EnrollmentCodeAddView extends CommonField{
  studentEnrollmentCode:EnrollmentCodeModel;
    constructor(){
        super();
        this.studentEnrollmentCode=new EnrollmentCodeModel()
    }
}

export class EnrollmentCodeListView extends CommonField{
  public studentEnrollmentCodeList:[EnrollmentCodeModel];
  public schoolId:number;
  public tenantId:string;
  public isListView:boolean;
  public academicYear: number;
  constructor() {
    super();
    this.studentEnrollmentCodeList = [new EnrollmentCodeModel]
}
  //"_failure": true,
//  "_message": "string"
    

}