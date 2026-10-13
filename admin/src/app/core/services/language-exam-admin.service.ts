import { Injectable } from '@angular/core';
import { TransferHttp } from '../transfer-http/transfer-http';
import { map } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class LanguageExamAdminService {

  constructor(private transferHttp: TransferHttp) { }

  // ─── Exam endpoints ───────────────────────────────────────────────────────

  getExamList(language: string, page: number = 1, pageSize: number = 10) {
    const url = `api/v1/language/exams?language=${language}&page=${page}&pageSize=${pageSize}`;
    return this.transferHttp.get(url).pipe(map((res: any) => res));
  }

  createExam(model: any) {
    const url = `api/v1/language/exams/admin`;
    return this.transferHttp.post(url, model).pipe(map((res: any) => res));
  }

  updateExam(id: number | string, model: any) {
    const url = `api/v1/language/exams/admin/${id}`;
    return this.transferHttp.put(url, model).pipe(map((res: any) => res));
  }

  deleteExam(id: number | string) {
    const url = `api/v1/language/exams/admin/${id}`;
    return this.transferHttp.delete(url).pipe(map((res: any) => res));
  }

  // ─── Question endpoints ───────────────────────────────────────────────────

  getQuestions(examId: number | string) {
    const url = `api/v1/language/exams/${examId}/questions`;
    return this.transferHttp.get(url).pipe(map((res: any) => res));
  }

  createQuestion(partId: number | string, model: any) {
    const url = `api/v1/language/questions`;
    return this.transferHttp.post(url, { ...model, partId }).pipe(map((res: any) => res));
  }

  updateQuestion(id: number | string, model: any) {
    const url = `api/v1/language/questions/${id}`;
    return this.transferHttp.put(url, model).pipe(map((res: any) => res));
  }

  deleteQuestion(id: number | string) {
    const url = `api/v1/language/questions/${id}`;
    return this.transferHttp.delete(url).pipe(map((res: any) => res));
  }

  // ─── Vocabulary endpoints ─────────────────────────────────────────────────

  getVocabularyList(language: string, page: number = 1, pageSize: number = 10) {
    const url = `api/v1/language/vocabulary?language=${language}&page=${page}&pageSize=${pageSize}`;
    return this.transferHttp.get(url).pipe(map((res: any) => res));
  }

  createVocab(model: any) {
    const url = `api/v1/language/vocabulary/admin`;
    return this.transferHttp.post(url, model).pipe(map((res: any) => res));
  }

  updateVocab(id: number | string, model: any) {
    const url = `api/v1/language/vocabulary/admin/${id}`;
    return this.transferHttp.put(url, model).pipe(map((res: any) => res));
  }

  deleteVocab(id: number | string) {
    const url = `api/v1/language/vocabulary/admin/${id}`;
    return this.transferHttp.delete(url).pipe(map((res: any) => res));
  }

  bulkImportVocab(language: string, items: any[]) {
    const url = `api/v1/language/vocabulary/bulk`;
    return this.transferHttp.post(url, { language, items }).pipe(map((res: any) => res));
  }
}
