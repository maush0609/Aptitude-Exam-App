import { Component, OnInit } from '@angular/core';
import { QuestionService } from '../../services/question.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Question } from '../../models/question.model';
import { jwtDecode } from 'jwt-decode';

@Component({
  selector: 'app-manage-questions',
  templateUrl: './manage-questions.component.html',
  styleUrls: ['./manage-questions.component.scss']
})
export class ManageQuestionsComponent implements OnInit {
  questions: Question[] = [];
  questionForm!: FormGroup;
  editingQuestion: Question | null = null;
  createdById: string = '';

  constructor(private questionService: QuestionService, private fb: FormBuilder) {}

  ngOnInit(): void {
    this.extractUserIdFromStoredToken();
    this.initForm();
    this.loadQuestions();
  }

  extractUserIdFromStoredToken(): void {
    const userJson = localStorage.getItem('currentUser');
    if (!userJson) {
      console.warn('currentUser not found in localStorage.');
      return;
    }

    try {
      const user = JSON.parse(userJson);
      const token = user.token;
      if (!token) {
        console.warn('Token missing in currentUser object.');
        return;
      }

      const decoded: any = jwtDecode(token);
      this.createdById = decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];

      if (!this.createdById) {
        console.warn('NameIdentifier claim not found in token.');
      }
    } catch (err) {
      console.error('Failed to decode currentUser token:', err);
    }
  }

  initForm(): void {
    this.questionForm = this.fb.group({
      id: [0],
      text: ['', Validators.required],
      optionA: ['', Validators.required],
      optionB: ['', Validators.required],
      optionC: ['', Validators.required],
      optionD: ['', Validators.required],
      correctAnswer: ['', [Validators.required, Validators.pattern(/^[A-D]$/)]],
      category: [''],
      difficultyLevel: [null, Validators.required]
    });
  }

  loadQuestions(): void {
    this.questionService.getQuestions().subscribe({
      next: data => this.questions = data,
      error: err => console.error('Failed to load questions:', err)
    });
  }

  edit(question: Question): void {
    this.editingQuestion = question;
    this.questionForm.patchValue(question);
  }

  delete(id: number): void {
    if (confirm('Are you sure you want to delete this question?')) {
      this.questionService.deleteQuestion(id).subscribe({
        next: () => this.loadQuestions(),
        error: err => console.error('Delete failed:', err)
      });
    }
  }

  onSubmit(): void {
    if (!this.createdById || this.createdById.trim() === '') {
      alert('User identity not found. Please log in again.');
      return;
    }

    const formValue = this.questionForm.value;
    const questionData = { ...formValue, createdById: this.createdById };

    if (formValue.id && formValue.id > 0) {
      this.questionService.updateQuestion(formValue.id, questionData).subscribe({
        next: () => {
          this.loadQuestions();
          this.questionForm.reset();
          this.editingQuestion = null;
        },
        error: err => console.error('Update failed:', err)
      });
    } else {
      this.questionService.createQuestion(questionData).subscribe({
        next: () => {
          this.loadQuestions();
          this.questionForm.reset();
        },
        error: err => console.error('Creation failed:', err)
      });
    }
  }

  cancelEdit(): void {
    this.editingQuestion = null;
    this.questionForm.reset();
  }
}
