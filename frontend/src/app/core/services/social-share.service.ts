import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class SocialShareService {

  shareOnFacebook(url: string, title: string) {
    window.open(`https://www.facebook.com/sharer/sharer.php?u=${encodeURIComponent(url)}&quote=${encodeURIComponent(title)}`, '_blank', 'width=600,height=400');
  }

  shareOnTwitter(url: string, title: string) {
    window.open(`https://twitter.com/intent/tweet?url=${encodeURIComponent(url)}&text=${encodeURIComponent(title)}`, '_blank', 'width=600,height=400');
  }

  shareOnLinkedIn(url: string, title: string) {
    window.open(`https://www.linkedin.com/sharing/share-offsite/?url=${encodeURIComponent(url)}`, '_blank', 'width=600,height=400');
  }

  shareOnTelegram(url: string, title: string) {
    window.open(`https://t.me/share/url?url=${encodeURIComponent(url)}&text=${encodeURIComponent(title)}`, '_blank', 'width=600,height=400');
  }

  copyLink(url: string): Promise<boolean> {
    return navigator.clipboard.writeText(url).then(() => true).catch(() => false);
  }

  getShareUrl(courseId: number): string {
    return `${window.location.origin}/course/${courseId}`;
  }
}
