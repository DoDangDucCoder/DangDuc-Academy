import { Injectable } from "@angular/core";
import { TransferHttp } from "../transfer-http/transfer-http";
import { LinkSettings } from "src/app/library/linksetting/LinkSetting";
import { map } from "rxjs/operators";
import { RepositoryModel } from "src/app/models/models/repository_base";
import { LoginRegister } from "src/app/models/respone_model/login-register-respone";
import { SocialUser } from "@abacritt/angularx-social-login";
import { BehaviorSubject, Observable } from "rxjs";
import { LocalStorageConfig } from "src/app/library/clientconfig/localstorageconfig";

@Injectable({ providedIn: 'root' })
export class AuthenticationServices {
    private readonly currentUserSubject = new BehaviorSubject<LoginRegister | null>(LocalStorageConfig.GetUser());
    loginDataUpdate$ = this.currentUserSubject.asObservable();
    constructor(
        private transferHttp: TransferHttp,
    ) { }

    registerUser(name: string, phone: string, email: string, password: string) {
        const ApiUrl = LinkSettings.GetResLinkSetting('Authentication', 'RegisterUser');
        return this.transferHttp.post(ApiUrl, { name, phone, email, password }).pipe(map((res: RepositoryModel<LoginRegister>) => res));
    }

    loginNormalAccount(email: string, password: string) {
        const ApiUrl = LinkSettings.GetResLinkSetting('Authentication', 'LoginNormalAccount');
        return this.transferHttp.post(ApiUrl, { email, password }).pipe(map((res: RepositoryModel<LoginRegister>) => res));
    }

    loginWithGoogle(model: SocialUser) {
        const ApiUrl = LinkSettings.GetResLinkSetting('Authentication', 'LoginWithGoogle');
        return this.transferHttp.post(ApiUrl, model).pipe(map((res: RepositoryModel<LoginRegister>) => res));
    }

    loginWithFaceBook(model: SocialUser) {
        const ApiUrl = LinkSettings.GetResLinkSetting('Authentication', 'LoginWithFaceBook');
        return this.transferHttp.post(ApiUrl, model).pipe(map((res: RepositoryModel<LoginRegister>) => res));
    }

    updateAfterLogin(dataLogin: LoginRegister | null) {
        this.currentUserSubject.next(dataLogin);
    }

    getDataUploadAfterLogin(): Observable<LoginRegister | null> {
        return this.loginDataUpdate$;
    }

    currentUser(): LoginRegister | null {
        return LocalStorageConfig.GetUser();
    }

    isCheckMemberAccount(): boolean {
        return this.currentUser() != null;
    }

    logoutWithAccount(){
        LocalStorageConfig.RemoveUser('currentUser');
        LocalStorageConfig.RemoveAllDataListCourse();
        this.updateAfterLogin(null);
    }
}
