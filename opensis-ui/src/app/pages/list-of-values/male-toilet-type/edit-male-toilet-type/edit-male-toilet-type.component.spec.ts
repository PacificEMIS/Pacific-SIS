import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditMaleToiletTypeComponent } from './edit-male-toilet-type.component';

describe('EditMaleToiletTypeComponent', () => {
  let component: EditMaleToiletTypeComponent;
  let fixture: ComponentFixture<EditMaleToiletTypeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EditMaleToiletTypeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditMaleToiletTypeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
