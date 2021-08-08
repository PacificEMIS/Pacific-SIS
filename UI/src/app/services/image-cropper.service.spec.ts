import { TestBed } from '@angular/core/testing';

import { ImageCropperService } from './image-cropper.service';

describe('ImageCropperService', () => {
  let service: ImageCropperService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ImageCropperService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
