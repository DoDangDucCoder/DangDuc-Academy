import { Injectable } from '@angular/core';
import { TransferHttp } from '../transfer-http/transfer-http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class LessonCompletionService {
  constructor(private transferHttp: TransferHttp) {}

  markComplete(idCourse: number, idContentDetail: number): Observable<any> {
    return this.transferHttp.post('api/v1/LessonCompletion/complete', { idCourse, idContentDetail });
  }

  markIncomplete(idCourse: number, idContentDetail: number): Observable<any> {
    return this.transferHttp.post('api/v1/LessonCompletion/uncomplete', { idCourse, idContentDetail });
  }

  getProgress(idCourse: number): Observable<any> {
    return this.transferHttp.get(`api/v1/LessonCompletion/progress/${idCourse}`);
  }

  getCompletedLessons(idCourse: number): Observable<any> {
    return this.transferHttp.get(`api/v1/LessonCompletion/completed/${idCourse}`);
  }
}
