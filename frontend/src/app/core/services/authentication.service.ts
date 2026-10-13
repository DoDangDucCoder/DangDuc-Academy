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

    refreshToken(refreshToken: string) {
        const ApiUrl = LinkSettings.GetResLinkSetting('Authentication', 'RefreshToken');
        return this.transferHttp.post(ApiUrl, { refreshToken }).pipe(map((res: RepositoryModel<LoginRegister>) => res));
    }

    revokeToken(refreshToken: string) {
        const ApiUrl = LinkSettings.GetResLinkSetting('Authentication', 'RevokeToken');
        return this.transferHttp.post(ApiUrl, { refreshToken }).pipe(map((res: RepositoryModel<boolean>) => res));
    }

    forgotPassword(email: string) {
        const ApiUrl = LinkSettings.GetResLinkSetting('Authentication', 'ForgotPassword');
        return this.transferHttp.post(ApiUrl, { email }).pipe(map((res: RepositoryModel<boolean>) => res));
    }

    resetPassword(email: string, token: string, newPassword: string) {
        const ApiUrl = LinkSettings.GetResLinkSetting('Authentication', 'ResetPassword');
        return this.transferHttp.post(ApiUrl, { email, token, newPassword }).pipe(map((res: RepositoryModel<boolean>) => res));
    }

    logoutWithAccount() {
        const currentUser = this.currentUser();
        if (currentUser?.refreshToken) {
            this.revokeToken(currentUser.refreshToken).subscribe({ error: () => {} });
        }
        LocalStorageConfig.RemoveUser('currentUser');
        LocalStorageConfig.RemoveAllDataListCourse();
        this.updateAfterLogin(null);
    }
}
