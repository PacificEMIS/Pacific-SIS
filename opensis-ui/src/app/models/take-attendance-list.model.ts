import { CommonField } from "../models/common-field.model";

export interface TakeAttendanceList {
    name: string;
    staffID: number;
    openSisProfile: string;
    jobTitle: string;
    schoolEmail: string;
    mobilePhone: number;
}

export class SearchCourseSectionForStudentAttendance {
    tenantId: string;
    schoolId: number;
    staffId: number;
    _tenantName: string;
    _userName: string;
    _token: string;
    _failure: boolean;
    _message: string;
    
    constructor() {
        this.tenantId = sessionStorage.getItem("tenantId");
        this.schoolId = +sessionStorage.getItem("selectedSchoolId");
        this._tenantName = sessionStorage.getItem("tenant");
        this._userName = sessionStorage.getItem("user");
        this._token = sessionStorage.getItem("token");
        this._failure = false;
        this._message = "";
    }
}

export class StaffDetailsModel {
    private staffId;
    private staffFullName;

}

export class GetAllStudentAttendanceListModel extends CommonField{
    studentAttendance: StudentAttendanceModel[];
    courseSectionId: number;
    attendanceDate: string;
    periodId: number;
    staffId: number;
    createdBy: string;
    updatedBy: string;
    courseId: number;
    constructor(){
        super();
        this.tenantId = sessionStorage.getItem("tenantId");
        this.schoolId = +sessionStorage.getItem("selectedSchoolId");
        this._tenantName = sessionStorage.getItem("tenant");
        this._userName = sessionStorage.getItem("user");
        this._token = sessionStorage.getItem("token");
    }
}
export class StudentAttendanceModel extends CommonField{
    studentId: number;
    staffId: number;
    courseId: number;
    courseSectionId: number;
    attendanceCategoryId: number;
    attendanceCode: number | string;
    attendanceDate: string;
    blockId: number;
    periodId: number;
    comments: string;
    studentAttendanceComments: StudentAttendanceCommentsModel[];
    createdBy: string;
    createdOn: string;
    updatedBy: string;
    updatedOn: string;
    constructor(){
        super();
        this.studentAttendanceComments = [new StudentAttendanceCommentsModel];
    }
}
export class StudentAttendanceCommentsModel {
    comment: string;
    membershipId: number;
    constructor(){

    }
}

export class StudentUpdateAttendanceCommentsModel extends CommonField {
        studentAttendanceComments: StudentUpdateCommentsModel =  new StudentUpdateCommentsModel();
    constructor(){
        super();
    }
}

export class StudentUpdateCommentsModel {
        studentAttendanceId: number
        CommentId: number;
        comment: string;
        membershipId: number;
        studentId: string;
        tenantId: string;
        schoolId: number;
    constructor(){
    }
}


export class AddUpdateStudentAttendanceModel extends CommonField{
    studentAttendance:StudentAttendanceModel[];
    courseSectionId: number;
    attendanceDate: string;
    membershipId: number;
    periodId: number;
    staffId: number;
    createdBy: string;
    updatedBy: string;
    courseId: number;
    constructor(){
        super();
        this.studentAttendance = [new StudentAttendanceModel]
    }
}


export class StudentAttendanceModelFor360{
    studentId: number;
    schoolId: number;
    staffId: number;
    courseId: number;
    courseSectionId: number;
    attendanceCategoryId: number;
    attendanceCode: number | string;
    attendanceDate: string;
    studentAttendanceComments:[];
    blockId: number;
    periodId: number;
    updatedBy: string;
}

export class AddUpdateStudentAttendanceModelFor360 extends CommonField{
    studentAttendance: StudentAttendanceModelFor360[];
    courseSectionId: number;
    attendanceDate: string;
    studentId: number;
    schoolId: number;
    periodId: number;
    staffId: number;
    createdBy: string;
    updatedBy: string;
    courseId: number;
    constructor(){
        super();
        this.studentAttendance = []
    }
}