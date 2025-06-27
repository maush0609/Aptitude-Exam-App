import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ExamService {
  private apiUrl = 'http://localhost:5069/api/exams';

  constructor(private http: HttpClient) {}

  getExams(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  createExam(exam: any): Observable<any> {
    return this.http.post(this.apiUrl, exam);
  }
  addQuestionToExam(examId: number, questionId: number, order: number) {
  return this.http.post(
    `${this.apiUrl}/${examId}/questions/${questionId}?order=${order}`, {}
  );
}
  updateExam(id: number, updatedExam: any): Observable<any> {
  return this.http.put(`${this.apiUrl}/${id}`, updatedExam);
}

deleteExam(id: number): Observable<any> {
  return this.http.delete(`${this.apiUrl}/${id}`);
}

}