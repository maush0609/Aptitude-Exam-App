import { Question } from './question.model';
export interface Exam {
  id: number;
  title: string;
  description?: string;
  startTime: Date;
  endTime: Date;
  durationMinutes: number;
  isPublished: boolean;
  createdAt?: Date;
  createdBy?: {
    id: string;
    userName: string;
  };
  questions?: Question[];
}

export interface ExamQuestion {
  examId: number;
  questionId: number;
  order: number;
  question?: Question;
}