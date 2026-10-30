import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { PeriodsComponent } from './periods.component';

describe('PeriodsComponent', () => {
  let component: PeriodsComponent;
  let fixture: ComponentFixture<PeriodsComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ PeriodsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PeriodsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
