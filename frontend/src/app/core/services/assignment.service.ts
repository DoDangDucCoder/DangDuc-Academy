import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { RepositoryModel } from '../../models/models/repository_base';

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
  private baseUrl = `${environment.baseUrl}Assignment`;

  constructor(private http: HttpClient) {}

  createAssignment(assignment: Assignment): Observable<RepositoryModel<Assignment>> {
    return this.http.post<RepositoryModel<Assignment>>(`${this.baseUrl}/Create`, assignment);
  }

  updateAssignment(id: number, assignment: Assignment): Observable<RepositoryModel<Assignment>> {
    return this.http.post<RepositoryModel<Assignment>>(`${this.baseUrl}/Update`, assignment);
  }

  deleteAssignment(id: number): Observable<RepositoryModel<any>> {
    return this.http.delete<RepositoryModel<any>>(`${this.baseUrl}/Delete/${id}`);
  }

  getAssignmentsByCourse(courseId: number): Observable<RepositoryModel<Assignment[]>> {
    return this.http.get<RepositoryModel<Assignment[]>>(`${this.baseUrl}/course/${courseId}`);
  }

  submitAssignment(submission: AssignmentSubmission): Observable<RepositoryModel<AssignmentSubmission>> {
    return this.http.post<RepositoryModel<AssignmentSubmission>>(`${this.baseUrl}/submit`, submission);
  }

  getSubmissionsForAssignment(assignmentId: number): Observable<RepositoryModel<AssignmentSubmission[]>> {
    return this.http.get<RepositoryModel<AssignmentSubmission[]>>(`${this.baseUrl}/${assignmentId}/submissions`);
  }

  getSubmissionByUserAndAssignment(assignmentId: number, userId: number): Observable<RepositoryModel<AssignmentSubmission>> {
    return this.http.get<RepositoryModel<AssignmentSubmission>>(`${this.baseUrl}/${assignmentId}/submission/${userId}`);
  }

  gradeSubmission(submissionId: number, grade: { score: number, feedback: string }): Observable<RepositoryModel<any>> {
    return this.http.post<RepositoryModel<any>>(`${this.baseUrl}/submission/${submissionId}/grade`, grade);
  }
}
