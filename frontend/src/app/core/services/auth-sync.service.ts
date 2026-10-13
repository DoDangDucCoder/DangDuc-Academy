import { Injectable, NgZone } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({ providedIn: 'root' })
export class AuthSyncService {
  private readonly LOGOUT_KEY = 'app_logout_event';

  constructor(private router: Router, private zone: NgZone) {
    this.listenForLogout();
  }

  /**
   * Call this when user logs out. It broadcasts to all other tabs.
   */
  broadcastLogout() {
    localStorage.setItem(this.LOGOUT_KEY, Date.now().toString());
    localStorage.removeItem(this.LOGOUT_KEY);
  }

  /**
   * Listen for logout events from other tabs
   */
  private listenForLogout() {
    window.addEventListener('storage', (event: StorageEvent) => {
      if (event.key === this.LOGOUT_KEY && event.newValue) {
        this.zone.run(() => {
          // Clear local auth data
          localStorage.removeItem('user');
          localStorage.removeItem('token');
          localStorage.removeItem('access_token');
          this.router.navigate(['/login']);
        });
      }
    });
  }
}
