import { Component, OnInit } from '@angular/core';
import { ExamService } from '../../services/exam.service';
import { QuestionService } from '../../services/question.service';
import { Exam } from '../../models/exam.model';
import { Question } from '../../models/question.model';

@Component({
  selector: 'app-manage-exams',
  templateUrl: './manage-exams.component.html',
  styleUrls: ['./manage-exams.component.scss']
})
export class ManageExamsComponent implements OnInit {
  exams: Exam[] = [];
  questions: Question[] = [];
  selectedQuestions: number[] = [];
  isEditing: boolean = false;
  selectedExam: Exam | null = null;

  newExam: Partial<Exam> = {
    title: '',
    description: '',
    startTime: new Date(),
    endTime: new Date(),
    durationMinutes: 30,
    isPublished: false,
  };

  editExam: any = {};

  constructor(
    private examService: ExamService,
    private questionService: QuestionService
  ) {}

  ngOnInit(): void {
    this.loadExams();
    this.loadQuestions();
  }

  loadExams() {
    this.examService.getExams().subscribe((exams) => (this.exams = exams));
  }

  loadQuestions() {
    this.questionService.getQuestions().subscribe((qs) => (this.questions = qs));
  }

  toggleQuestionSelection(id: number) {
    const index = this.selectedQuestions.indexOf(id);
    if (index > -1) {
      this.selectedQuestions.splice(index, 1);
    } else {
      this.selectedQuestions.push(id);
    }
  }

  createExam() {
    this.examService.createExam(this.newExam).subscribe({
      next: (exam) => {
        const tasks = this.selectedQuestions.map((qid, index) =>
          this.examService.addQuestionToExam(exam.id, qid, index + 1)
        );
        Promise.all(tasks.map(obs => obs.toPromise()))
          .then(() => this.loadExams())
          .catch(err => console.error('Error adding questions:', err));

        this.resetForm();
      },
      error: (err) => console.error('Failed to create exam:', err)
    });
  }

  startEdit(exam: Exam) {
    this.isEditing = true;
    this.selectedExam = exam;
    this.editExam = {
      ...exam,
      startTime: this.formatDateTimeForInput(exam.startTime),
      endTime: this.formatDateTimeForInput(exam.endTime),
    };
  }

  cancelEdit() {
    this.isEditing = false;
    this.selectedExam = null;
    this.editExam = {};
  }

  updateExam() {
    const updated = {
      ...this.editExam,
      startTime: new Date(this.editExam.startTime),
      endTime: new Date(this.editExam.endTime),
    };
    this.examService.updateExam(updated.id, updated).subscribe({
      next: () => {
        this.loadExams();
        this.cancelEdit();
      },
      error: (err) => console.error('Failed to update exam:', err)
    });
  }

  deleteExam(id: number) {
    if (confirm('Are you sure you want to delete this exam?')) {
      this.examService.deleteExam(id).subscribe({
        next: () => this.loadExams(),
        error: (err) => console.error('Failed to delete exam:', err)
      });
    }
  }

  formatDateTimeForInput(date: Date | string): string {
    const d = new Date(date);
    const offset = d.getTimezoneOffset();
    const local = new Date(d.getTime() - offset * 60000);
    return local.toISOString().slice(0, 16);
  }

  resetForm() {
    this.newExam = {
      title: '',
      description: '',
      startTime: new Date(),
      endTime: new Date(),
      durationMinutes: 30,
      isPublished: false,
    };
    this.selectedQuestions = [];
  }
}
