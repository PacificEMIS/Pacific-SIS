/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
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
