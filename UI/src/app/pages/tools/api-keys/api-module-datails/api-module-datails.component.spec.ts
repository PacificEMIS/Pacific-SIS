import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ApiModuleDatailsComponent } from './api-module-datails.component';

describe('ApiModuleDatailsComponent', () => {
  let component: ApiModuleDatailsComponent;
  let fixture: ComponentFixture<ApiModuleDatailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ApiModuleDatailsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ApiModuleDatailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
