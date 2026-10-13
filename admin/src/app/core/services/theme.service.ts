import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private darkMode = new BehaviorSubject<boolean>(false);
  isDarkMode$ = this.darkMode.asObservable();

  constructor() {
    const saved = localStorage.getItem('hdnx-admin-theme');
    const isDark = saved === 'dark';
    this.darkMode.next(isDark);
    this.applyTheme(isDark);
  }

  toggleTheme(): void {
    const isDark = !this.darkMode.value;
    this.darkMode.next(isDark);
    localStorage.setItem('hdnx-admin-theme', isDark ? 'dark' : 'light');
    this.applyTheme(isDark);
  }

  private applyTheme(isDark: boolean): void {
    const html = document.documentElement;
    if (isDark) {
      html.setAttribute('data-bs-theme', 'dark');
      html.setAttribute('data-layout-mode', 'dark');
      html.setAttribute('data-topbar', 'dark');
      html.setAttribute('data-sidebar', 'dark');
    } else {
      html.setAttribute('data-bs-theme', 'light');
      html.setAttribute('data-layout-mode', 'light');
      html.setAttribute('data-topbar', 'light');
      html.setAttribute('data-sidebar', 'light');
    }
  }
}
