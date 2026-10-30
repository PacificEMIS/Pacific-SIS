import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { EditSchoolClassificationComponent } from './edit-school-classification.component';

describe('EditSchoolClassificationComponent', () => {
  let component: EditSchoolClassificationComponent;
  let fixture: ComponentFixture<EditSchoolClassificationComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ EditSchoolClassificationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditSchoolClassificationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
