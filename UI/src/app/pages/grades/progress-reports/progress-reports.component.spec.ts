import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProgressReportsComponent } from './progress-reports.component';

describe('ProgressReportsComponent', () => {
  let component: ProgressReportsComponent;
  let fixture: ComponentFixture<ProgressReportsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ProgressReportsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProgressReportsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
