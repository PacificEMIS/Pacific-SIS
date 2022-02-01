import { schoolDetailsModel } from "./school-details.model";
import { CommonField } from "./common-field.model";
import { CustomFieldModel } from './custom-field.model';
import { FieldsCategoryModel } from './fields-category.model';

export class SchoolMasterModel {

  public tenantId: string;
  public schoolId: number;
  public schoolInternalId: string;
  public schoolAltId: string;
  public schoolStateId: string
  public schoolDistrictId: string;
  public checked: boolean;
  public schoolLevel: string;
  public schoolClassification: string;
  public schoolName: string;
  public alternateName: string;
  public streetAddress1: string;
  public streetAddress2: string;
  public city: any;
  public county: string;
  public division: string;
  public state?: any;
  public district: string;
  public zip: string;
  public latitude?: number;
  public longitude?: number;
  public country?: any;
  public currentPeriodEnds?: number;
  public maxApiChecks?: number;
  public features: string;
  public planId?: number;
  public createdBy: string;
  public dateCreated: string;
  public modifiedBy: string;
  public dateModifed: string;
  public schoolDetail: [schoolDetailsModel];
  public fieldsCategory: FieldsCategoryModel[];
 
  public createdOn: string;
  public updatedBy: string;
  public updatedOn: string;
  constructor() {
    this.schoolDetail = [new schoolDetailsModel];

  }
}

export class SchoolAddViewModel extends CommonField {
  public schoolMaster: SchoolMasterModel;
  public isMarkingPeriod: boolean;
  public selectedCategoryId: number;
  public schoolId: number;
  public EmailAddress: string;
  public lastUsedSchoolId:number;
  public EndDate: string;
  public StartDate: string;

  constructor() {
    super();
    this.schoolMaster = new SchoolMasterModel();
    this.schoolMaster.latitude = null;
    this.schoolMaster.longitude = null;
    this.selectedCategoryId = 0;

  }
}

export class CopySchoolModel extends CommonField {
  public schoolMaster: SchoolMasterModel;
  public fromSchoolId: number;
  public periods: boolean;
  public markingPeriods: boolean;
  public calendar: boolean;
  public sections: boolean;
  public rooms: boolean;
  public gradeLevels: boolean;
  public schoolFields: boolean;
  public studentFields: boolean;
  public enrollmentCodes: boolean;
  public staffFields: boolean;
  public subjets: boolean;
  public programs: boolean;
  public course: boolean;
  public attendanceCode: boolean;
  public reportCardGrades: boolean;
  public reportCardComments: boolean;
  public standardGrades: boolean;
  public honorRollSetup: boolean;
  public effortGrades: boolean;
  public profilePermission: boolean;
  public schoolLevel: boolean;
  public schoolClassification: boolean;
  public femaleToiletType: boolean;
  public femaleToiletAccessibility: boolean;
  public maleToiletType: boolean;
  public maleToiletAccessibility: boolean;
  public commonToiletType: boolean;
  public commonToiletAccessibility: boolean;
  public race: boolean;
  public ethnicity: boolean;

  constructor() {
    super();
    this.schoolMaster = new SchoolMasterModel();
    this.periods = true;
    this.markingPeriods = true;
    this.calendar = true;
    this.sections = true;
    this.rooms = true;
    this.gradeLevels = true;
    this.schoolFields = true;
    this.studentFields = true;
    this.enrollmentCodes = true;
    this.staffFields = true;
    this.subjets = true;
    this.programs = true;
    this.course = true;
    this.attendanceCode = true;
    this.reportCardGrades = true;
    this.reportCardComments = true;
    this.standardGrades = true;
    this.honorRollSetup = true;
    this.effortGrades = true;
    this.profilePermission = true;
    this.schoolLevel = true;
    this.schoolClassification = true;
    this.femaleToiletType = true;
    this.femaleToiletAccessibility = true;
    this.maleToiletType = true;
    this.maleToiletAccessibility = true;
    this.commonToiletType = true;
    this.commonToiletAccessibility = true;
    this.race = true;
    this.ethnicity = true;
  }
}

export class CheckSchoolInternalIdViewModel extends CommonField {
  public tenantId: string;
  public schoolInternalId: string;
  public isValidInternalId: boolean;
}

