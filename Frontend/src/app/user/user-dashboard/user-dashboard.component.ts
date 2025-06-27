import { Component, OnInit } from '@angular/core';
import { UserExamService } from '../../services/user-exam.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-user-dashboard',
  templateUrl: './user-dashboard.component.html',
  styleUrls: ['./user-dashboard.component.scss']
})
export class UserDashboardComponent implements OnInit {
  availableExams: any[] = [];

  constructor(
    private userExamService: UserExamService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.userExamService.getActiveExams().subscribe({
      next: exams => {
        this.availableExams = exams;
      },
      error: err => {
        console.error('Error loading exams:', err);
      }
    });
  }

  startExam(examId: number) {
    this.router.navigate(['/user/take-exam', examId]); 
  }

  logout(): void {
    localStorage.clear(); 
    this.router.navigate(['/login']);

    console.log('User logged out.');
  }
}
