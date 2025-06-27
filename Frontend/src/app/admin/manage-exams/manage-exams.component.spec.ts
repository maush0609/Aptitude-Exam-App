import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageExamsComponent } from './manage-exams.component';

describe('ManageExamsComponent', () => {
  let component: ManageExamsComponent;
  let fixture: ComponentFixture<ManageExamsComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ManageExamsComponent]
    });
    fixture = TestBed.createComponent(ManageExamsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
