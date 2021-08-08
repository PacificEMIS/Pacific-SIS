import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentFieldsCategoryComponent } from './student-fields-category.component';

describe('StudentFieldsCategoryComponent', () => {
  let component: StudentFieldsCategoryComponent;
  let fixture: ComponentFixture<StudentFieldsCategoryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StudentFieldsCategoryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StudentFieldsCategoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
