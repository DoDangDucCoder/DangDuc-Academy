import { Injectable } from '@angular/core';
import { TransferHttp } from '../transfer-http/transfer-http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class VideoPlaybackService {
  constructor(private transferHttp: TransferHttp) {}

  savePosition(idContentDetail: number, positionSeconds: number, durationSeconds: number): Observable<any> {
    return this.transferHttp.post('api/v1/VideoPlayback/save', {
      idContentDetail,
      positionSeconds,
      durationSeconds
    });
  }

  getPosition(idContentDetail: number): Observable<any> {
    return this.transferHttp.get(`api/v1/VideoPlayback/${idContentDetail}`);
  }
}
