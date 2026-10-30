import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { EditSectionComponent } from './edit-section.component';

describe('EditSectionComponent', () => {
  let component: EditSectionComponent;
  let fixture: ComponentFixture<EditSectionComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ EditSectionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditSectionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
