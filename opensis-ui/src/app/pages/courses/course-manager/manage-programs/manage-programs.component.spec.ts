import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageProgramsComponent } from './manage-programs.component';

describe('ManageProgramsComponent', () => {
  let component: ManageProgramsComponent;
  let fixture: ComponentFixture<ManageProgramsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ManageProgramsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ManageProgramsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
