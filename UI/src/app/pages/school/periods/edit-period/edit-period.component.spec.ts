import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditPeriodComponent } from './edit-period.component';

describe('EditPeriodComponent', () => {
  let component: EditPeriodComponent;
  let fixture: ComponentFixture<EditPeriodComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EditPeriodComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditPeriodComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
