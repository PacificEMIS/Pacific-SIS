import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { EditparentSchoolaccessComponent } from './editparent-schoolaccess.component';

describe('EditparentSchoolaccessComponent', () => {
  let component: EditparentSchoolaccessComponent;
  let fixture: ComponentFixture<EditparentSchoolaccessComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ EditparentSchoolaccessComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditparentSchoolaccessComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
