import { Injectable, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AntiCheatService implements OnDestroy {
  private violations = 0;
  private maxViolations = 3;
  private isActive = false;
  private boundHandler: (() => void) | null = null;

  violationDetected$ = new Subject<number>();
  autoSubmit$ = new Subject<void>();

  startMonitoring(maxViolations: number = 3) {
    this.violations = 0;
    this.maxViolations = maxViolations;
    this.isActive = true;

    this.boundHandler = () => {
      if (!this.isActive) return;
      if (document.hidden) {
        this.violations++;
        this.violationDetected$.next(this.violations);
        if (this.violations >= this.maxViolations) {
          this.autoSubmit$.next();
          this.stopMonitoring();
        }
      }
    };

    document.addEventListener('visibilitychange', this.boundHandler);
  }

  stopMonitoring() {
    this.isActive = false;
    if (this.boundHandler) {
      document.removeEventListener('visibilitychange', this.boundHandler);
      this.boundHandler = null;
    }
  }

  getViolationCount(): number {
    return this.violations;
  }

  getRemainingWarnings(): number {
    return Math.max(0, this.maxViolations - this.violations);
  }

  ngOnDestroy() {
    this.stopMonitoring();
  }
}
