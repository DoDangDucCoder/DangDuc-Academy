import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { TransferHttp } from '../transfer-http/transfer-http';
import { RepositoryModel, PageResult } from 'src/app/models/models/repository_base';
import {
  AuditLogModel,
  InstructorDashboardModel,
  LiveClassModel,
  QuizModel
} from 'src/app/models/models/phase4-admin.models';

@Injectable({ providedIn: 'root' })
export class Phase4AdminService {
  constructor(private readonly transferHttp: TransferHttp) {}

  getAuditLogs(pageIndex: number, pageSize: number, action = '', entityName = '', userId?: number, fromDate = '', toDate = '') {
    const query = new URLSearchParams();
    if (action) query.set('action', action);
    if (entityName) query.set('entityName', entityName);
    if (userId) query.set('userId', String(userId));
    if (fromDate) query.set('fromDate', fromDate);
    if (toDate) query.set('toDate', toDate);
    const suffix = query.toString() ? `?${query.toString()}` : '';
    return this.transferHttp
      .get(`audit-log/${pageIndex}/${pageSize}${suffix}`)
      .pipe(map((res: RepositoryModel<PageResult<AuditLogModel[]>>) => res));
  }

  getInstructorDashboard(instructorId: number) {
    return this.transferHttp
      .get(`instructor-dashboard/${instructorId}`)
      .pipe(map((res: RepositoryModel<InstructorDashboardModel>) => res));
  }

  getQuizzesByCourse(idCourse: number) {
    return this.transferHttp
      .get(`quiz/course/${idCourse}`)
      .pipe(map((res: RepositoryModel<QuizModel[]>) => res));
  }

  createQuiz(model: QuizModel) {
    return this.transferHttp
      .post('quiz', model)
      .pipe(map((res: RepositoryModel<QuizModel>) => res));
  }

  updateQuiz(id: number, model: QuizModel) {
    return this.transferHttp
      .put(`quiz/${id}`, model)
      .pipe(map((res: RepositoryModel<boolean>) => res));
  }

  getLiveClasses(idCourse?: number, idInstructor?: number) {
    const query = new URLSearchParams();
    if (idCourse) query.set('idCourse', String(idCourse));
    if (idInstructor) query.set('idInstructor', String(idInstructor));
    const suffix = query.toString() ? `?${query.toString()}` : '';
    return this.transferHttp
      .get(`live-class${suffix}`)
      .pipe(map((res: RepositoryModel<LiveClassModel[]>) => res));
  }

  createLiveClass(model: LiveClassModel) {
    return this.transferHttp
      .post('live-class', model)
      .pipe(map((res: RepositoryModel<LiveClassModel>) => res));
  }

  updateLiveClass(id: number, model: LiveClassModel) {
    return this.transferHttp
      .put(`live-class/${id}`, model)
      .pipe(map((res: RepositoryModel<boolean>) => res));
  }
}
