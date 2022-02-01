import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GradebookGradeListComponent } from './gradebook-grade-list.component';

describe('GradebookGradeListComponent', () => {
  let component: GradebookGradeListComponent;
  let fixture: ComponentFixture<GradebookGradeListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GradebookGradeListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GradebookGradeListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
