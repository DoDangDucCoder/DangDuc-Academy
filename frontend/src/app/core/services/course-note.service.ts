import { Injectable } from '@angular/core';
import { TransferHttp } from '../transfer-http/transfer-http';
import { Observable } from 'rxjs';
import { RepositoryModel } from 'src/app/models/models/repository_base';

export interface CourseNoteModel {
  id?: number;
  idStudent?: number;
  idCourse: number;
  idContentDetail: number;
  timestamp: number;
  content: string;
}

@Injectable({
  providedIn: 'root'
})
export class CourseNoteService {

  constructor(private transferHttp: TransferHttp) { }

  getNotesByCourse(courseId: number): Observable<CourseNoteModel[]> {
    return this.transferHttp.get(`api/v1/CourseNote/${courseId}`);
  }

  addNote(model: CourseNoteModel): Observable<CourseNoteModel> {
    return this.transferHttp.post(`api/v1/CourseNote`, model);
  }

  updateNote(noteId: number, model: CourseNoteModel): Observable<CourseNoteModel> {
    return this.transferHttp.put(`api/v1/CourseNote/${noteId}`, model);
  }

  deleteNote(noteId: number): Observable<boolean> {
    return this.transferHttp.delete(`api/v1/CourseNote/${noteId}`);
  }
}
