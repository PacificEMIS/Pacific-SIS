import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PrintStudentInfoComponent } from './print-student-info.component';

describe('PrintStudentInfoComponent', () => {
  let component: PrintStudentInfoComponent;
  let fixture: ComponentFixture<PrintStudentInfoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PrintStudentInfoComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PrintStudentInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
