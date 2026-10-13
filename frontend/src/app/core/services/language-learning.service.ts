import { Injectable } from '@angular/core';
import { TransferHttp } from '../transfer-http/transfer-http';
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { RepositoryModel } from 'src/app/models/models/repository_base';

export interface LanguageExam {
  id: number;
  language: string;
  examType: string;
  level?: string;
  title: string;
  description?: string;
  durationMinutes: number;
  totalQuestions: number;
  passScore: number;
  thumbnailUrl?: string;
  viewCount: number;
  attemptCount: number;
}

export interface ExamQuestion {
  id: number;
  orderNumber: number;
  questionText: string;
  questionImage?: string;
  audioUrl?: string;
  questionType: number;
  options: { id: number; label: string; answerText: string }[];
}

export interface ExamPart {
  id: number;
  partNumber: number;
  title: string;
  description?: string;
  audioUrl?: string;
  questions: ExamQuestion[];
}

export interface LanguageExamDetail extends LanguageExam {
  parts: ExamPart[];
}

export interface StartExamResponse {
  attemptId: number;
  examId: number;
  startTime: string;
  deadline: string;
  durationSeconds: number;
  exam: LanguageExamDetail;
}

export interface UserAnswer { questionId: number; selectedAnswerId: number; }

export interface QuestionResult {
  questionId: number;
  questionText: string;
  selectedAnswerId: number;
  correctAnswerId: number;
  isCorrect: boolean;
  explanation?: string;
  options: { id: number; label: string; answerText: string; isCorrect: boolean }[];
}

export interface ExamResult {
  attemptId: number;
  examId: number;
  examTitle: string;
  totalScore: number;
  maxScore: number;
  percentage: number;
  correctCount: number;
  totalQuestions: number;
  isPassed: boolean;
  timeTakenSeconds: number;
  questionResults: QuestionResult[];
}

export interface LeaderboardEntry {
  rank: number;
  userName: string;
  avatarUrl?: string;
  percentage: number;
  timeTakenSeconds: number;
  submitTime: string;
}

export interface VocabularyItem {
  id: number;
  language: string;
  word: string;
  pronunciation?: string;
  meaningVi: string;
  meaningEn?: string;
  exampleSentence?: string;
  exampleTranslation?: string;
  audioUrl?: string;
  imageUrl?: string;
  level?: string;
  tags?: string;
  nextReview?: string;
  isMastered: boolean;
  repetition: number;
}

export interface VocabStats {
  language: string;
  totalLearned: number;
  masteredCount: number;
  dueForReview: number;
  newToday: number;
  progressPercent: number;
}

@Injectable({ providedIn: 'root' })
export class LanguageLearningService {
  private readonly base = 'api/v1/language';

  constructor(private http: TransferHttp) {}

  // ── Exams ──────────────────────────────────────────────────────────────
  getExamList(language: string, examType?: string, level?: string, page = 1, pageSize = 12): Observable<RepositoryModel<LanguageExam[]>> {
    let url = `${this.base}/exams?language=${language}&page=${page}&pageSize=${pageSize}`;
    if (examType) url += `&examType=${examType}`;
    if (level) url += `&level=${level}`;
    return this.http.get<RepositoryModel<LanguageExam[]>>(url).pipe(map(r => r));
  }

  getExamDetail(examId: number): Observable<RepositoryModel<LanguageExamDetail>> {
    return this.http.get<RepositoryModel<LanguageExamDetail>>(`${this.base}/exams/${examId}`).pipe(map(r => r));
  }

  startExam(examId: number): Observable<RepositoryModel<StartExamResponse>> {
    return this.http.post(`${this.base}/exams/${examId}/start`, {}).pipe(map(r => r as RepositoryModel<StartExamResponse>));
  }

  submitExam(attemptId: number, answers: UserAnswer[]): Observable<RepositoryModel<ExamResult>> {
    return this.http.post(`${this.base}/exams/attempts/${attemptId}/submit`, answers).pipe(map(r => r as RepositoryModel<ExamResult>));
  }

  getResult(attemptId: number): Observable<RepositoryModel<ExamResult>> {
    return this.http.get<RepositoryModel<ExamResult>>(`${this.base}/exams/attempts/${attemptId}/result`).pipe(map(r => r));
  }

  getLeaderboard(examId: number, top = 10): Observable<RepositoryModel<LeaderboardEntry[]>> {
    return this.http.get<RepositoryModel<LeaderboardEntry[]>>(`${this.base}/exams/${examId}/leaderboard?top=${top}`).pipe(map(r => r));
  }

  // ── Vocabulary ─────────────────────────────────────────────────────────
  getVocabulary(language: string, level?: string, page = 1, pageSize = 20, tags?: string): Observable<RepositoryModel<VocabularyItem[]>> {
    let url = `${this.base}/vocabulary?language=${language}&page=${page}&pageSize=${pageSize}`;
    if (level) url += `&level=${level}`;
    if (tags)  url += `&tags=${encodeURIComponent(tags)}`;
    return this.http.get<RepositoryModel<VocabularyItem[]>>(url).pipe(map(r => r));
  }

  getReviewItems(language: string, limit = 20): Observable<RepositoryModel<VocabularyItem[]>> {
    return this.http.get<RepositoryModel<VocabularyItem[]>>(`${this.base}/vocabulary/review?language=${language}&limit=${limit}`).pipe(map(r => r));
  }

  updateProgress(vocabId: number, quality: number): Observable<RepositoryModel<boolean>> {
    return this.http.post(`${this.base}/vocabulary/${vocabId}/progress?quality=${quality}`, {}).pipe(map(r => r as RepositoryModel<boolean>));
  }

  getVocabStats(language: string): Observable<RepositoryModel<VocabStats>> {
    return this.http.get<RepositoryModel<VocabStats>>(`${this.base}/vocabulary/stats?language=${language}`).pipe(map(r => r));
  }
}
