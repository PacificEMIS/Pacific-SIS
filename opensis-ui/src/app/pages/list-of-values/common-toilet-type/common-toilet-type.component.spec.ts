import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CommonToiletTypeComponent } from './common-toilet-type.component';

describe('CommonToiletTypeComponent', () => {
  let component: CommonToiletTypeComponent;
  let fixture: ComponentFixture<CommonToiletTypeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CommonToiletTypeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CommonToiletTypeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
