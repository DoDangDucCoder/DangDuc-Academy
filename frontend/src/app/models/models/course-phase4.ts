export interface QuizModel {
  id: number;
  idCourse: number;
  title: string;
  description?: string;
  passingScore: number;
  questions: QuizQuestionModel[];
}

export interface QuizQuestionModel {
  id: number;
  idQuiz: number;
  questionText: string;
  point: number;
  answers: QuizAnswerOptionModel[];
}

export interface QuizAnswerOptionModel {
  id: number;
  idQuestion: number;
  answerText: string;
  isCorrect: boolean;
}

export interface QuizAttemptResultModel {
  attemptId: number;
  score: number;
  totalScore: number;
  passingScore: number;
  isPassed: boolean;
}

export interface LiveClassModel {
  id: number;
  idCourse: number;
  idInstructor: number;
  title: string;
  description?: string;
  startTime: string;
  endTime: string;
  meetingUrl: string;
  status: number;
}

export interface CertificateModel {
  id: number;
  idCourse: number;
  idStudent: number;
  certificateCode: string;
  issuedDate: string;
  fileUrl?: string;
  status: number;
}
