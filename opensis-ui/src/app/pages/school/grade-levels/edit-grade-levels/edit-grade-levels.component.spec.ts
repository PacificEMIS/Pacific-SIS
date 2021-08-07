import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditGradeLevelsComponent } from './edit-grade-levels.component';

describe('EditGradeLevelsComponent', () => {
  let component: EditGradeLevelsComponent;
  let fixture: ComponentFixture<EditGradeLevelsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EditGradeLevelsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditGradeLevelsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
