import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewstudentLogininfoComponent } from './viewstudent-logininfo.component';

describe('ViewstudentLogininfoComponent', () => {
  let component: ViewstudentLogininfoComponent;
  let fixture: ComponentFixture<ViewstudentLogininfoComponent>;

  beforeEach(async(() => {
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
