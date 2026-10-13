import { Injectable } from '@angular/core';
import { TransferHttp } from '../transfer-http/transfer-http';
import { Observable } from 'rxjs';

export interface SubtitleModel {
  id?: number;
  idContentDetail: number;
  language: string;
  label: string;
  fileUrl: string;
  publicId?: string;
}

@Injectable({
  providedIn: 'root'
})
export class SubtitleService {
  constructor(private transferHttp: TransferHttp) {
  }

  getSubtitles(idContentDetail: number): Observable<any> {
    return this.transferHttp.get(`api/v1/Subtitle/video/${idContentDetail}`);
  }

  addSubtitle(idContentDetail: number, language: string, label: string, file: File): Observable<any> {
    const formData = new FormData();
    formData.append('idContentDetail', idContentDetail.toString());
    formData.append('language', language);
    formData.append('label', label);
    formData.append('file', file);
    return this.transferHttp.post(`api/v1/Subtitle`, formData);
  }

  deleteSubtitle(id: number): Observable<any> {
    return this.transferHttp.delete(`api/v1/Subtitle/${id}`);
  }
}
