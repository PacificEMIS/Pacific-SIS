import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { EditCommonToiletTypeComponent } from './edit-common-toilet-type.component';

describe('EditCommonToiletTypeComponent', () => {
  let component: EditCommonToiletTypeComponent;
  let fixture: ComponentFixture<EditCommonToiletTypeComponent>;

  beforeEach(waitForAsync(() => {
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
