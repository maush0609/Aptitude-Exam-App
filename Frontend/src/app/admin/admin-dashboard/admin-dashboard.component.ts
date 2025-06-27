import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { ExamService } from '../../services/exam.service';
import { QuestionService } from '../../services/question.service';

@Component({
  selector: 'app-admin-dashboard',
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.scss']
})
export class AdminDashboardComponent implements OnInit {
  exams: any[] = [];
  questions: any[] = [];
  stats = {
    totalExams: 0,
    totalQuestions: 0,
    activeExams: 0
  };

  constructor(
    public authService: AuthService,
    private examService: ExamService,
    private questionService: QuestionService,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadExams();
    this.loadQuestions();
  }

  loadExams() {
    this.examService.getExams().subscribe({
      next: (exams) => {
        this.exams = exams;
        this.stats.totalExams = exams.length;
        this.stats.activeExams = exams.filter((e: any) => e.isPublished).length;
      },
      error: (err) => console.error('Failed to load exams:', err)
    });
  }

  loadQuestions() {
    this.questionService.getQuestions().subscribe({
      next: (questions) => {
        this.questions = questions;
        this.stats.totalQuestions = questions.length;
      },
      error: (err) => console.error('Failed to load questions:', err)
    });
  }

  logout() {
    this.authService.logout();
  }

  navigateTo(path: string) {
    this.router.navigate([`/admin/${path}`]);
  }
}