import { TestBed } from '@angular/core/testing';

import { StudentReportService } from './student-report.service';

describe('StudentReportService', () => {
  let service: StudentReportService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StudentReportService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
