import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ViewstudentLogininfoComponent } from './viewstudent-logininfo.component';

describe('ViewstudentLogininfoComponent', () => {
  let component: ViewstudentLogininfoComponent;
  let fixture: ComponentFixture<ViewstudentLogininfoComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ViewstudentLogininfoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ViewstudentLogininfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
