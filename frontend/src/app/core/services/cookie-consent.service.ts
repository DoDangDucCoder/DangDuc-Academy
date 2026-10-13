import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class CookieConsentService {
  private readonly CONSENT_KEY = 'cookie_consent_accepted';

  isConsentGiven(): boolean {
    return localStorage.getItem(this.CONSENT_KEY) === 'true';
  }

  acceptConsent() {
    localStorage.setItem(this.CONSENT_KEY, 'true');
  }

  revokeConsent() {
    localStorage.removeItem(this.CONSENT_KEY);
  }

  showBanner(): boolean {
    return !this.isConsentGiven();
  }
}
