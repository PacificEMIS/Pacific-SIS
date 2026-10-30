import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { EditLanguageComponent } from './edit-language.component';

describe('EditLanguageComponent', () => {
  let component: EditLanguageComponent;
  let fixture: ComponentFixture<EditLanguageComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ EditLanguageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditLanguageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
