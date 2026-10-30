import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { EditFemaleToiletTypeComponent } from './edit-female-toilet-type.component';

describe('EditFemaleToiletTypeComponent', () => {
  let component: EditFemaleToiletTypeComponent;
  let fixture: ComponentFixture<EditFemaleToiletTypeComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ EditFemaleToiletTypeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditFemaleToiletTypeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
