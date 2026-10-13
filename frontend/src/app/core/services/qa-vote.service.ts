import { Injectable } from '@angular/core';
import { TransferHttp } from '../transfer-http/transfer-http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class QAVoteService {
  constructor(private transferHttp: TransferHttp) {}

  vote(idQuestion: number, isUpvote: boolean, isForAnswer: boolean = false): Observable<any> {
    return this.transferHttp.post('api/v1/QAVote', { idQuestion, isUpvote, isForAnswer });
  }

  getVoteCount(idQuestion: number, isForAnswer: boolean = false): Observable<any> {
    return this.transferHttp.get(`api/v1/QAVote/count/${idQuestion}?isForAnswer=${isForAnswer}`);
  }
}
