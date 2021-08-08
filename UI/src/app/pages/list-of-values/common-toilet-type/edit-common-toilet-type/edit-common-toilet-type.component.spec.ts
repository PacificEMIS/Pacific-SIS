import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditCommonToiletTypeComponent } from './edit-common-toilet-type.component';

describe('EditCommonToiletTypeComponent', () => {
  let component: EditCommonToiletTypeComponent;
  let fixture: ComponentFixture<EditCommonToiletTypeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EditCommonToiletTypeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditCommonToiletTypeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
