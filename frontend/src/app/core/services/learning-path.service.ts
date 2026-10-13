import { Injectable } from '@angular/core';
import { TransferHttp } from '../transfer-http/transfer-http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class LearningPathService {
  constructor(private transferHttp: TransferHttp) {}

  getAll(): Observable<any> {
    return this.transferHttp.get('api/v1/LearningPath');
  }

  getById(id: number): Observable<any> {
    return this.transferHttp.get(`api/v1/LearningPath/${id}`);
  }

  create(data: { title: string; description: string; idCategory: number; pictureUrl?: string; levelRequired: string; estimatedHours: number }): Observable<any> {
    return this.transferHttp.post('api/v1/LearningPath', data);
  }

  addCourse(pathId: number, idCourse: number, orderIndex: number, isRequired: boolean = true): Observable<any> {
    return this.transferHttp.post(`api/v1/LearningPath/${pathId}/courses`, { idCourse, orderIndex, isRequired });
  }

  getPrerequisites(idCourse: number): Observable<any> {
    return this.transferHttp.get(`api/v1/LearningPath/prerequisites/${idCourse}`);
  }

  setPrerequisite(idCourse: number, idPrerequisiteCourse: number): Observable<any> {
    return this.transferHttp.post('api/v1/LearningPath/prerequisites', { idCourse, idPrerequisiteCourse });
  }
}
