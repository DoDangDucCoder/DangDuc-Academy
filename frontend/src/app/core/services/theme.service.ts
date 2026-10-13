import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ThemeService {
    private darkMode = new BehaviorSubject<boolean>(false);
    isDarkMode$ = this.darkMode.asObservable();

    constructor() {
        const saved = localStorage.getItem('hdnx-theme');
        const isDark = saved === 'dark';
        this.darkMode.next(isDark);
        this.applyTheme(isDark);
    }

    toggleTheme(): void {
        const isDark = !this.darkMode.value;
        this.darkMode.next(isDark);
        localStorage.setItem('hdnx-theme', isDark ? 'dark' : 'light');
        this.applyTheme(isDark);
    }

    get isDark(): boolean {
        return this.darkMode.value;
    }

    private applyTheme(isDark: boolean): void {
        document.body.classList.toggle('dark-theme', isDark);
    }
}
