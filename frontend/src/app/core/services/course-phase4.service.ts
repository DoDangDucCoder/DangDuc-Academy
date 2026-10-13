import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { RepositoryModel } from 'src/app/models/models/repository_base';
import {
  CertificateModel,
  LiveClassModel,
  QuizAttemptResultModel,
  QuizModel
} from 'src/app/models/models/course-phase4';
import { TransferHttp } from '../transfer-http/transfer-http';

@Injectable({ providedIn: 'root' })
export class CoursePhase4Service {
  constructor(private readonly transferHttp: TransferHttp) {}

  getQuizzes(idCourse: number) {
    return this.transferHttp
      .get(`quiz/course/${idCourse}`)
      .pipe(map((res: RepositoryModel<QuizModel[]>) => res));
  }

  submitQuiz(idQuiz: number, answers: Array<{ idQuestion: number; idAnswerOption: number }>) {
    return this.transferHttp
      .post('quiz/submit', { idQuiz, answers })
      .pipe(map((res: RepositoryModel<QuizAttemptResultModel>) => res));
  }

  getLiveClasses(idCourse: number) {
    return this.transferHttp
      .get(`live-class?idCourse=${idCourse}`)
      .pipe(map((res: RepositoryModel<LiveClassModel[]>) => res));
  }

  getCertificate(idCourse: number, idStudent: number) {
    return this.transferHttp
      .get(`certificate/course/${idCourse}/student/${idStudent}`)
      .pipe(map((res: RepositoryModel<CertificateModel | null>) => res));
  }

  generateCertificate(idCourse: number, idStudent: number) {
    return this.transferHttp
      .post(`certificate/course/${idCourse}/student/${idStudent}`, {})
      .pipe(map((res: RepositoryModel<CertificateModel>) => res));
  }

  downloadCertificate(idCertificate: number) {
    return this.transferHttp.getFile(`certificate/${idCertificate}/download`);
  }
}
