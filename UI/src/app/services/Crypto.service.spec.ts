/* eslint-disable @typescript-eslint/no-unused-vars */

import { TestBed, inject, waitForAsync } from '@angular/core/testing';
import { CryptoService } from './Crypto.service';

describe('Service: Crypto', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [CryptoService]
    });
  });

  it('should ...', inject([CryptoService], (service: CryptoService) => {
    expect(service).toBeTruthy();
  }));
});
