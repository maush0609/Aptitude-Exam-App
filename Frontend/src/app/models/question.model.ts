export interface Question {
  id: number;
  text: string;
  optionA: string;
  optionB: string;
  optionC: string;
  optionD: string;
  correctAnswer: string;
  category?: string;
  difficultyLevel?: number;
  createdAt?: Date;
  createdBy?: {
    id: string;
    userName: string;
  };
}