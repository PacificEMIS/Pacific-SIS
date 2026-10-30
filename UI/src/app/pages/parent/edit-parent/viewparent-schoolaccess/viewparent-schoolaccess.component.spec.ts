import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ViewparentSchoolaccessComponent } from './viewparent-schoolaccess.component';

describe('ViewparentSchoolaccessComponent', () => {
  let component: ViewparentSchoolaccessComponent;
  let fixture: ComponentFixture<ViewparentSchoolaccessComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ViewparentSchoolaccessComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ViewparentSchoolaccessComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
