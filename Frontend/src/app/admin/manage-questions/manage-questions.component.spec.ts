import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageQuestionsComponent } from './manage-questions.component';

describe('ManageQuestionsComponent', () => {
  let component: ManageQuestionsComponent;
  let fixture: ComponentFixture<ManageQuestionsComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ManageQuestionsComponent]
    });
    fixture = TestBed.createComponent(ManageQuestionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
