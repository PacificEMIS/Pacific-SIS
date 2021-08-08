import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AttendanceCategoryComponent } from './attendance-category.component';

describe('AttendanceCategoryComponent', () => {
  let component: AttendanceCategoryComponent;
  let fixture: ComponentFixture<AttendanceCategoryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AttendanceCategoryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AttendanceCategoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
