import { CommonField } from './common-field.model';

export class FillAttendanceViewModel extends CommonField {
    courseSectionId: number;
    membershipId: number;
    createdBy: string;
    totalRecordsCreated: number;
    courseSectionsProcessed: number;
}
