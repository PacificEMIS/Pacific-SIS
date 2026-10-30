import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { StudentCommentsComponent } from './student-comments.component';

describe('StudentCommentsComponent', () => {
  let component: StudentCommentsComponent;
  let fixture: ComponentFixture<StudentCommentsComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ StudentCommentsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StudentCommentsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
