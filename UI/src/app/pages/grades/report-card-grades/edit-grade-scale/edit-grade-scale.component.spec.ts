import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditGradeScaleComponent } from './edit-grade-scale.component';

describe('EditBlockComponent', () => {
  let component: EditGradeScaleComponent;
  let fixture: ComponentFixture<EditGradeScaleComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EditGradeScaleComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditGradeScaleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
