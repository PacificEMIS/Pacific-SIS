import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditHonorRollComponent } from './edit-honor-roll.component';

describe('EditHonorRollComponent', () => {
  let component: EditHonorRollComponent;
  let fixture: ComponentFixture<EditHonorRollComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EditHonorRollComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EditHonorRollComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
