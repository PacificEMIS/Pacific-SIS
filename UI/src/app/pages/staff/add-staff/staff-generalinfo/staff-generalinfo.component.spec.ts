import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { StaffGeneralinfoComponent } from './staff-generalinfo.component';

describe('StaffGeneralinfoComponent', () => {
  let component: StaffGeneralinfoComponent;
  let fixture: ComponentFixture<StaffGeneralinfoComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ StaffGeneralinfoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StaffGeneralinfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
