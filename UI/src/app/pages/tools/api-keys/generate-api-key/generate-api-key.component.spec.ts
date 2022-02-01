import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GenerateApiKeyComponent } from './generate-api-key.component';

describe('GenerateApiKeyComponent', () => {
  let component: GenerateApiKeyComponent;
  let fixture: ComponentFixture<GenerateApiKeyComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GenerateApiKeyComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GenerateApiKeyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
