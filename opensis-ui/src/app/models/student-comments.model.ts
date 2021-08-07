import{CommonField} from './common-field.model'
export class StudentCommentsModel{
    tenantId: string;
    schoolId: number;
    studentId: number;
    commentId: number;
    comment: string;
    createdBy: string;
    createdOn: string;
    updatedBy: string;
    updatedOn: string;
    constructor(){
      this.studentId = 0;
      this.commentId = 0;
      this.comment = null;
    }
  }
export class StudentCommentsAddView extends CommonField{
      studentComments: StudentCommentsModel;
      constructor(){
          super();
      }
  }

  export class StudentCommentsAddForGroupAssign extends CommonField{
    studentComments: StudentCommentsModel;
    public studentIds: any;
    constructor(){
        super();
        this.studentIds = [];
        this.studentComments =  new StudentCommentsModel();
    }
}
export class StudentCommentsListViewModel extends CommonField {
    public studentCommentsList: StudentCommentsModel[];
    public schoolId: number;
    public studentId: number;
    constructor() {
        super();
        this.studentId = 0;
    }
}