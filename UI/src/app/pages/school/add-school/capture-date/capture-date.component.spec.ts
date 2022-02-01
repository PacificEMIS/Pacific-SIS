import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CaptureDateComponent } from './capture-date.component';

describe('CaptureDateComponent', () => {
  let component: CaptureDateComponent;
  let fixture: ComponentFixture<CaptureDateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CaptureDateComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CaptureDateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
