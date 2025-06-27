import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UserExamService } from '../../services/user-exam.service';
import { Question } from '../../models/question.model';

@Component({
  selector: 'app-take-exam',
  templateUrl: './take-exam.component.html',
  styleUrls: ['./take-exam.component.scss']
})
export class TakeExamComponent implements OnInit {
  examId!: number;
  questions: Question[] = [];
  userAnswers: { [questionId: number]: string } = {};
  timeLeft: number = 0;
  timer!: any;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private userExamService: UserExamService
  ) {}

 ngOnInit(): void {
  this.examId = +this.route.snapshot.paramMap.get('id')!;
  this.userExamService.getExamWithQuestions(this.examId).subscribe((data) => {
    this.questions = data.questions;
    this.timeLeft = data.durationMinutes * 60; 
    this.startTimer();
  });
}

  startTimer() {
    this.timer = setInterval(() => {
      this.timeLeft--;
      if (this.timeLeft <= 0) {
        this.submit();
      }
    }, 1000);
  }

  getFormattedTime(): string {
    const minutes = Math.floor(this.timeLeft / 60);
    const seconds = this.timeLeft % 60;
    return `${minutes.toString().padStart(2, '0')}:${seconds
      .toString()
      .padStart(2, '0')}`;
  }

  submit() {
    clearInterval(this.timer);

    const answersArray = Object.entries(this.userAnswers).map(([questionId, answer]) => ({
      questionId: +questionId,
      userAnswer: answer 
    }));

    const payload = {
      examId: this.examId,
      answers: answersArray
    };

    this.userExamService.submitExam(payload).subscribe((result) => {
      const userExamId = result.userExamId; 
      this.router.navigate(['/user/review', userExamId]); 
    });
  }

  trackAnswer(qid: number, answer: string) {
    this.userAnswers[qid] = answer;
  }
}
