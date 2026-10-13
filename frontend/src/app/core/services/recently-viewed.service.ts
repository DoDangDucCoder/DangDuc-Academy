import { Injectable } from '@angular/core';

export interface RecentlyViewedCourse {
  id: number;
  title: string;
  pictureUrl: string;
  price: number;
  viewedAt: number; // timestamp
}

@Injectable({ providedIn: 'root' })
export class RecentlyViewedService {
  private readonly STORAGE_KEY = 'recently_viewed_courses';
  private readonly MAX_ITEMS = 10;

  addCourse(course: { id: number; title: string; pictureUrl: string; price: number }) {
    const items = this.getCourses();
    // Remove if already exists
    const filtered = items.filter(c => c.id !== course.id);
    // Add to front
    filtered.unshift({
      ...course,
      viewedAt: Date.now()
    });
    // Keep max items
    const trimmed = filtered.slice(0, this.MAX_ITEMS);
    localStorage.setItem(this.STORAGE_KEY, JSON.stringify(trimmed));
  }

  getCourses(): RecentlyViewedCourse[] {
    try {
      const data = localStorage.getItem(this.STORAGE_KEY);
      return data ? JSON.parse(data) : [];
    } catch {
      return [];
    }
  }

  clear() {
    localStorage.removeItem(this.STORAGE_KEY);
  }
}
