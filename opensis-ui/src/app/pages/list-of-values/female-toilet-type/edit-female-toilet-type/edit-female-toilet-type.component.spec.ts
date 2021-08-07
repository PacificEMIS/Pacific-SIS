import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditFemaleToiletTypeComponent } from './edit-female-toilet-type.component';

describe('EditFemaleToiletTypeComponent', () => {
  let component: EditFemaleToiletTypeComponent;
  let fixture: ComponentFixture<EditFemaleToiletTypeComponent>;

  beforeEach(async(() => {
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
