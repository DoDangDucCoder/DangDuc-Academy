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

  getBalance() {
    const ApiUrl = LinkSettings.GetResLinkSetting('Payout', 'GetBalance');
    return this.transferHttp.get(ApiUrl).pipe(map((res: RepositoryModel<number>) => res));
  }

  requestPayout(amount: number) {
    const ApiUrl = LinkSettings.GetResLinkSetting('Payout', 'RequestPayout');
    return this.transferHttp.post(ApiUrl, { amount }).pipe(map((res: RepositoryModel<boolean>) => res));
  }
}
