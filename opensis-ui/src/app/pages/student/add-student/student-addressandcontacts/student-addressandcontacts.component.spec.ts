import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentAddressandcontactsComponent } from './student-addressandcontacts.component';

describe('StudentAddressandcontactsComponent', () => {
  let component: StudentAddressandcontactsComponent;
  let fixture: ComponentFixture<StudentAddressandcontactsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StudentAddressandcontactsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StudentAddressandcontactsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
