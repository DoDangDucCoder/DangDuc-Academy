import { Injectable } from '@angular/core';
import { TransferHttp } from '../transfer-http/transfer-http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AnnouncementService {
  constructor(private transferHttp: TransferHttp) {}

  getAnnouncements(idCourse: number): Observable<any> {
    return this.transferHttp.get(`api/v1/Announcement/course/${idCourse}`);
  }

  createAnnouncement(idCourse: number, title: string, content: string): Observable<any> {
    return this.transferHttp.post('api/v1/Announcement', { idCourse, title, content });
  }

  deleteAnnouncement(id: number): Observable<any> {
    return this.transferHttp.delete(`api/v1/Announcement/${id}`);
  }
}
