import { TestBed } from '@angular/core/testing';

import { ResponseMessageService } from './response-message.service';

describe('ResponseMessageService', () => {
  let service: ResponseMessageService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ResponseMessageService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
