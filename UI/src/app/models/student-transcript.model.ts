import { CommonField } from "./common-field.model";

export class StudentTranscript extends CommonField{
    schoolLogo: boolean;
    studentPhoto: boolean;
    gradeLagend: boolean;
    gradeLavels: string;
    createdBy: string;
    studentListForTranscript: TranscriptStudentList[];
    transcriptPdf: string;

    constructor(){
      super();
      this.schoolLogo=true;
      this.studentPhoto=true;
      this.gradeLagend=true;
      this.studentListForTranscript=[];
    }
  }

  export class TranscriptStudentList{
    firstGivenName: string;
    middleName: string;
    lastFamilyName: string;
    studentId: number;
    studentGuid: string;
  }

export class GetStudentTranscriptModel extends CommonField {
  schoolLogo: boolean;
  studentPhoto: boolean;
  gradeLagend: boolean;
  gradeLavels: string;
  academicYear: number;
  HistoricalGradeLavels : string;
  studentsDetailsForTranscripts: studentsDetailsForTranscriptsModel[];

  constructor() {
    super();
    this.schoolLogo = true;
    this.studentPhoto = true;
    this.gradeLagend = true;
    this.studentsDetailsForTranscripts = [];
  }
}

export class studentsDetailsForTranscriptsModel {
  studentId: number;
}