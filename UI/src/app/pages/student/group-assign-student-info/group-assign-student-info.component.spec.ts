import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GroupAssignStudentInfoComponent } from './group-assign-student-info.component';

describe('GroupAssignStudentInfoComponent', () => {
  let component: GroupAssignStudentInfoComponent;
  let fixture: ComponentFixture<GroupAssignStudentInfoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GroupAssignStudentInfoComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GroupAssignStudentInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
