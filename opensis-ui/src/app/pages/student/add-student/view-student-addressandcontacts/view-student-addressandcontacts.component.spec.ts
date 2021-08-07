import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewStudentAddressandcontactsComponent } from './view-student-addressandcontacts.component';

describe('ViewStudentAddressandcontactsComponent', () => {
  let component: ViewStudentAddressandcontactsComponent;
  let fixture: ComponentFixture<ViewStudentAddressandcontactsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ViewStudentAddressandcontactsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ViewStudentAddressandcontactsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
