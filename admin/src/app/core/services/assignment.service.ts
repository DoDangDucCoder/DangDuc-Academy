import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';

export interface Assignment {
  id?: number;
  idCourse: number;
  title: string;
  description?: string;
  maxScore: number;
  dueDate?: Date;
  status?: number;
}

export interface AssignmentSubmission {
  id?: number;
  idAssignment: number;
  idUser: number;
  submissionText?: string;
  fileUrl?: string;
  submittedAt?: Date;
  score?: number;
  feedback?: string;
  status?: number;
}

@Injectable({
  providedIn: 'root'
})
export class AssignmentService {
  private apiUrl = `${environment.baseUrl}assignment`;

  constructor(private http: HttpClient) {}

  createAssignment(model: Assignment): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/create`, model);
  }

  updateAssignment(model: Assignment): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/update`, model);
  }

  deleteAssignment(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/delete/${id}`);
  }

  getAssignmentsByCourse(idCourse: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/get-by-course/${idCourse}`);
  }

  submitAssignment(model: AssignmentSubmission): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/submit`, model);
  }

  getSubmissionsByAssignment(idAssignment: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/get-submissions/${idAssignment}`);
  }

  getSubmissionByUserAndAssignment(idAssignment: number, idUser: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/get-my-submission/${idAssignment}/${idUser}`);
  }

  gradeSubmission(idSubmission: number, score: number, feedback: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/grade/${idSubmission}?score=${score}&feedback=${encodeURIComponent(feedback)}`, {});
  }
}
