import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SelectBarComponent } from './select-bar.component';

describe('SelectBarComponent', () => {
  let component: SelectBarComponent;
  let fixture: ComponentFixture<SelectBarComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SelectBarComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SelectBarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
