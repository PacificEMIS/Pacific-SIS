import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DatabaseBackupComponent } from './database-backup.component';

describe('DatabaseBackupComponent', () => {
  let component: DatabaseBackupComponent;
  let fixture: ComponentFixture<DatabaseBackupComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DatabaseBackupComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DatabaseBackupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
