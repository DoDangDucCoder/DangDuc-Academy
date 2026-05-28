import { Injectable } from "@angular/core";
import { Stripe, loadStripe } from "@stripe/stripe-js";
import { LinkSettings } from "src/app/library/linksetting/LinkSetting";
import { PurchaseOrder } from "src/app/models/models/purchase";
import { StripeCheckoutSession } from "src/app/models/respone_model/stripe-checkout-session";
import { environment } from "src/environments/environment";
import { TransferHttp } from "../transfer-http/transfer-http";
import { map } from "rxjs/operators";
import { RepositoryModel } from "src/app/models/models/repository_base";
import { MessengerServices } from "./messenger.service";

@Injectable({ providedIn: 'root' })
export class StripeServices {
    private stripePromise!: Promise<Stripe | null>;

    constructor(
        private transferHttp: TransferHttp,
        private messengerServices: MessengerServices
    ) {
        this.loadConfigForStripe();
    }

    private loadConfigForStripe() {
        this.stripePromise = loadStripe(environment.stripePublicKey);
    }

    async goToCheckOutForStripe(model: PurchaseOrder): Promise<void> {
        const stripe = await this.stripePromise;
        if (!stripe) {
            this.messengerServices.errorWithIssue();
            return;
        }

        this.createCheckoutSession(model).subscribe({
            next: async (res) => {
                if (res.systemMessage === '' && res.retCode === 0 && res.data?.sessionId) {
                    const checkoutResult = await stripe.redirectToCheckout({
                        sessionId: res.data.sessionId,
                    });

                    if (checkoutResult.error) {
                        this.messengerServices.errorWithIssue();
                    }
                    return;
                }

                this.messengerServices.errorWithIssue();
            },
            error: () => this.messengerServices.errorWithIssue(),
        });

    }

    private createCheckoutSession(model: PurchaseOrder) {
        const ApiUrl = LinkSettings.GetResLinkSetting('PurchaseOrder', 'StripeCreateCheckoutSession');
        return this.transferHttp.post(ApiUrl, model).pipe(map((res: RepositoryModel<StripeCheckoutSession>) => res));

    }
}
