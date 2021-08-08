import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DataEditInfoComponent } from './data-edit-info.component';

describe('DataEditInfoComponent', () => {
  let component: DataEditInfoComponent;
  let fixture: ComponentFixture<DataEditInfoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DataEditInfoComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DataEditInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
