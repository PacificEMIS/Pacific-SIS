import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditCustomProfileComponent } from './edit-custom-profile.component';

describe('EditCustomProfileComponent', () => {
  let component: EditCustomProfileComponent;
  let fixture: ComponentFixture<EditCustomProfileComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EditCustomProfileComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EditCustomProfileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
