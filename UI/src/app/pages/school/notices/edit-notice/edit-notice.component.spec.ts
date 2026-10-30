import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { EditNoticeComponent } from './edit-notice.component';

describe('EditNoticeComponent', () => {
  let component: EditNoticeComponent;
  let fixture: ComponentFixture<EditNoticeComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ EditNoticeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditNoticeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
