import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { EditSchoolLevelComponent } from './edit-school-level.component';

describe('EditSchoolLevelComponent', () => {
  let component: EditSchoolLevelComponent;
  let fixture: ComponentFixture<EditSchoolLevelComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ EditSchoolLevelComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditSchoolLevelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
