/* eslint-disable @typescript-eslint/no-unused-vars */

import { TestBed, inject, waitForAsync } from '@angular/core/testing';
import { SchoolService } from './school.service';

describe('Service: School', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [SchoolService]
    });
  });

  it('should ...', inject([SchoolService], (service: SchoolService) => {
    expect(service).toBeTruthy();
  }));
});
