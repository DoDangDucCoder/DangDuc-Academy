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

  getPendingRefunds() {
    const ApiUrl = LinkSettings.GetResLinkSetting('Refund', 'GetPendingRefunds');
    return this.transferHttp.get(ApiUrl).pipe(map((res: RepositoryModel<any[]>) => res));
  }

  processRefund(idRefundRequest: number, isApproved: boolean, adminNote: string) {
    const ApiUrl = LinkSettings.GetResLinkSetting('Refund', 'ProcessRefund');
    return this.transferHttp.post(ApiUrl, { idRefundRequest, isApproved, adminNote }).pipe(map((res: RepositoryModel<boolean>) => res));
  }
}
