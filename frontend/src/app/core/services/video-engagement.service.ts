import { Injectable } from '@angular/core';
import { TransferHttp } from '../transfer-http/transfer-http';

export interface EngagementEvent {
  idContentDetail: number;
  eventType: string; // play, pause, seek, ended, buffer
  videoPosition: number;
  videoDuration: number;
}

@Injectable({ providedIn: 'root' })
export class VideoEngagementService {
  private eventBuffer: EngagementEvent[] = [];
  private flushInterval: any;

  constructor(private transferHttp: TransferHttp) {}

  startTracking() {
    this.eventBuffer = [];
    this.flushInterval = setInterval(() => {
      this.flush();
    }, 30000);
  }

  stopTracking() {
    this.flush();
    if (this.flushInterval) {
      clearInterval(this.flushInterval);
      this.flushInterval = null;
    }
  }

  trackEvent(event: EngagementEvent) {
    this.eventBuffer.push(event);
  }

  private flush() {
    if (this.eventBuffer.length === 0) return;
    const events = [...this.eventBuffer];
    this.eventBuffer = [];
    this.transferHttp.post('api/v1/VideoEngagement/batch', { events }).subscribe({
      error: () => {
        this.eventBuffer.unshift(...events);
      }
    });
  }
}
