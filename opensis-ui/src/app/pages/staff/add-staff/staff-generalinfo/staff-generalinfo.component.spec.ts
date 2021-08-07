import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StaffGeneralinfoComponent } from './staff-generalinfo.component';

describe('StaffGeneralinfoComponent', () => {
  let component: StaffGeneralinfoComponent;
  let fixture: ComponentFixture<StaffGeneralinfoComponent>;

  beforeEach(async(() => {
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
