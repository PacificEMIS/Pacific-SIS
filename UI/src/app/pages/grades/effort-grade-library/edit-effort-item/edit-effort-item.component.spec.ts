import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditEffortItemComponent } from './edit-effort-item.component';

describe('EditEffortItemComponent', () => {
  let component: EditEffortItemComponent;
  let fixture: ComponentFixture<EditEffortItemComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EditEffortItemComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditEffortItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
