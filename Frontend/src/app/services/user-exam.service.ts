import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Question } from '../models/question.model';


@Injectable({
  providedIn: 'root'
})
export class UserExamService {
  private apiUrl = 'http://localhost:5069/api/user-exam';

  constructor(private http: HttpClient) {}

  getActiveExams(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/active`);
  }

  getQuestionsByExamId(examId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/questions/${examId}`);
  }
   
  submitExam(data: any): Observable<any> {
  const stored = localStorage.getItem('currentUser');
  const token = stored ? JSON.parse(stored).token : null;

  const options = token
    ? { headers: { Authorization: `Bearer ${token}` } }
    : {};

  return this.http.post<any>(`${this.apiUrl}/submit`, data, options);
}

getExamWithQuestions(examId: number): Observable<{ durationMinutes: number, questions: Question[] }> {
  return this.http.get<{ durationMinutes: number, questions: Question[] }>(
    `${this.apiUrl}/exam/${examId}`
  );
}


  getReview(userExamId: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/review/${userExamId}`);
  }
}
