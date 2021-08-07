import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditCourseSectionComponent } from './edit-course-section.component';

describe('EditCourseSectionComponent', () => {
  let component: EditCourseSectionComponent;
  let fixture: ComponentFixture<EditCourseSectionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EditCourseSectionComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EditCourseSectionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
