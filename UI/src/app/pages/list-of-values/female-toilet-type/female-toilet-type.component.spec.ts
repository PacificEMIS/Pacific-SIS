import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FemaleToiletTypeComponent } from './female-toilet-type.component';

describe('FemaleToiletTypeComponent', () => {
  let component: FemaleToiletTypeComponent;
  let fixture: ComponentFixture<FemaleToiletTypeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FemaleToiletTypeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FemaleToiletTypeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
