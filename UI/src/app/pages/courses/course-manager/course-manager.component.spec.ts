import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { CourseManagerComponent } from './course-manager.component';

describe('CourseManagerComponent', () => {
  let component: CourseManagerComponent;
  let fixture: ComponentFixture<CourseManagerComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ CourseManagerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CourseManagerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
