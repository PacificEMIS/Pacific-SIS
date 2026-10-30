import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { AddCalendarComponent } from './add-calendar.component';

describe('AddCalendarComponent', () => {
  let component: AddCalendarComponent;
  let fixture: ComponentFixture<AddCalendarComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ AddCalendarComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddCalendarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
