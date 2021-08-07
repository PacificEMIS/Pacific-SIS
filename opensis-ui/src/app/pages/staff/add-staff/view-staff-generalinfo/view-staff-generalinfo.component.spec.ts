import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewStaffGeneralinfoComponent } from './view-staff-generalinfo.component';

describe('ViewStaffGeneralinfoComponent', () => {
  let component: ViewStaffGeneralinfoComponent;
  let fixture: ComponentFixture<ViewStaffGeneralinfoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ViewStaffGeneralinfoComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ViewStaffGeneralinfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
