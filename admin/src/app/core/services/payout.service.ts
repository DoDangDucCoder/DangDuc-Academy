import { Injectable } from '@angular/core';
import { TransferHttp } from '../transfer-http/transfer-http';
import { LinkSettings } from 'src/app/library/linksetting/LinkSetting';
import { map } from 'rxjs/operators';
import { RepositoryModel } from 'src/app/models/models/repository_base';

@Injectable({
  providedIn: 'root'
})
export class PayoutService {
  constructor(private transferHttp: TransferHttp) { }

  getPendingPayouts() {
    const ApiUrl = LinkSettings.GetResLinkSetting('Payout', 'GetPendingPayouts');
    return this.transferHttp.get(ApiUrl).pipe(map((res: RepositoryModel<any[]>) => res));
  }

  processPayout(idPayoutRequest: number, isApproved: boolean, adminNote: string) {
    const ApiUrl = LinkSettings.GetResLinkSetting('Payout', 'ProcessPayout');
    return this.transferHttp.post(ApiUrl, { idPayoutRequest, isApproved, adminNote }).pipe(map((res: RepositoryModel<boolean>) => res));
  }
}
