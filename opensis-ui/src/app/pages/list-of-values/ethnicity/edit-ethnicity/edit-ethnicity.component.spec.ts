import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditEthnicityComponent } from './edit-ethnicity.component';

describe('EditEthnicityComponent', () => {
  let component: EditEthnicityComponent;
  let fixture: ComponentFixture<EditEthnicityComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EditEthnicityComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditEthnicityComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
