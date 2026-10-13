import { PageResult } from './repository_base';

export interface AuditLogModel {
  id: number;
  userId?: number | null;
  action: string;
  entityName: string;
  entityId?: string | null;
  oldValue?: string | null;
  newValue?: string | null;
  ipAddress?: string | null;
  userAgent?: string | null;
  createdAt?: string;
}

export interface InstructorDashboardModel {
  instructorId: number;
  totalCourses: number;
  totalStudents: number;
  totalRevenue: number;
  completionRate: number;
  courses: InstructorCourseMetricModel[];
  recentReviews: InstructorReviewModel[];
}

export interface InstructorCourseMetricModel {
  courseId: number;
  title?: string;
  students: number;
  revenue: number;
  averageRating: number;
}

export interface InstructorReviewModel {
  courseId: number;
  studentId: number;
  rating: number;
  comment?: string;
}

export interface QuizModel {
  id?: number;
  idCourse: number;
  title: string;
  description?: string;
  passingScore: number;
  questions: QuizQuestionModel[];
}

export interface QuizQuestionModel {
  id?: number;
  idQuiz?: number;
  questionText: string;
  point: number;
  answers: QuizAnswerOptionModel[];
}

export interface QuizAnswerOptionModel {
  id?: number;
  idQuestion?: number;
  answerText: string;
  isCorrect: boolean;
}

export interface LiveClassModel {
  id?: number;
  idCourse: number;
  idInstructor: number;
  title: string;
  description?: string;
  startTime: string;
  endTime: string;
  meetingUrl: string;
  status?: number;
}

export type AuditLogPage = PageResult<AuditLogModel[]>;
