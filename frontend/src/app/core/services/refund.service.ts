import { Injectable } from '@angular/core';
import { TransferHttp } from '../transfer-http/transfer-http';
import { LinkSettings } from 'src/app/library/linksetting/LinkSetting';
import { map } from 'rxjs/operators';
import { RepositoryModel } from 'src/app/models/models/repository_base';

@Injectable({
  providedIn: 'root'
})
export class RefundService {
  constructor(private transferHttp: TransferHttp) { }

  requestRefund(idPurchaseCourse: number, reason: string) {
    const ApiUrl = LinkSettings.GetResLinkSetting('Refund', 'RequestRefund');
    return this.transferHttp.post(ApiUrl, { idPurchaseCourse, reason }).pipe(map((res: RepositoryModel<boolean>) => res));
  }

  getRefundsByStudent(idStudent: number) {
    const ApiUrl = LinkSettings.GetResLinkSetting('Refund', 'GetRefundsByStudent', idStudent);
    return this.transferHttp.get(ApiUrl).pipe(map((res: RepositoryModel<any[]>) => res));
  }
}
