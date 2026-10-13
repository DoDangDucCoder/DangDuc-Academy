import { Injectable } from '@angular/core';
import { TransferHttp } from '../transfer-http/transfer-http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class GamificationService {
  constructor(private transferHttp: TransferHttp) {}

  recordActivity(): Observable<any> {
    return this.transferHttp.post('api/v1/Gamification/record-activity', {});
  }

  awardXP(reason: string, idCourse?: number): Observable<any> {
    return this.transferHttp.post('api/v1/Gamification/award-xp', { reason, idCourse });
  }

  getProfile(): Observable<any> {
    return this.transferHttp.get('api/v1/Gamification/profile');
  }

  getLeaderboard(): Observable<any> {
    return this.transferHttp.get('api/v1/Gamification/leaderboard');
  }
}
