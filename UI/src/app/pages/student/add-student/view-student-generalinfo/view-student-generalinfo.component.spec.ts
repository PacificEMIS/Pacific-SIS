import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewStudentGeneralinfoComponent } from './view-student-generalinfo.component';

describe('ViewStudentGeneralinfoComponent', () => {
  let component: ViewStudentGeneralinfoComponent;
  let fixture: ComponentFixture<ViewStudentGeneralinfoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ViewStudentGeneralinfoComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ViewStudentGeneralinfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
