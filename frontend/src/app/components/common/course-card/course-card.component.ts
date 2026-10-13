import { Component, Input, OnInit } from '@angular/core';
import { Course } from 'src/app/models/models/course';

@Component({
  selector: 'app-course-card',
  templateUrl: './course-card.component.html',
  styleUrls: ['./course-card.component.scss']
})
export class CourseCardComponent implements OnInit {
  @Input() course: Course;
  @Input() routePrefix = '/detail-courses';      // /detail-content-course for purchased
  @Input() showProgress = false;
  @Input() showAddToCart = false;
  @Input() showGoToCourse = false;
  @Input() onAddToCart: (id: number) => void;
  @Input() onBookmark: (id: number) => void;

  ngOnInit(): void {}

  /** Stars array [1..5] for template ngFor */
  get starsArr(): number[] { return [1, 2, 3, 4, 5]; }

  get avgScore(): number { return Math.round(this.course?.averageScore || 0); }

  get priceLabel(): string {
    if (this.course?.isFree) return 'Miễn phí';
    if (this.course?.isDiscount)
      return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' })
        .format(this.course.priceOfDiscount ?? 0);
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' })
      .format(this.course?.priceOfCourse ?? 0);
  }

  get originalPriceLabel(): string {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' })
      .format(this.course?.priceOfCourse ?? 0);
  }

  /** Map categoryName / typeOfCourse to a Boxicons class */
  get techIcon(): string {
    const src = ((this.course?.categoryName || '') + ' ' + (this.course?.typeOfCourse || '')).toLowerCase();
    if (src.includes('java') || src.includes('spring'))  return 'bxl-java';
    if (src.includes('python'))                          return 'bxl-python';
    if (src.includes('javascript') || src.includes('js')) return 'bxl-javascript';
    if (src.includes('typescript') || src.includes('ts')) return 'bx-code-alt';
    if (src.includes('react'))                           return 'bxl-react';
    if (src.includes('angular'))                         return 'bxl-angular';
    if (src.includes('vue'))                             return 'bxl-vuejs';
    if (src.includes('docker') || src.includes('devops')) return 'bxl-docker';
    if (src.includes('kubernetes') || src.includes('k8s')) return 'bx-server';
    if (src.includes('go') || src.includes('golang'))    return 'bx-code-block';
    if (src.includes('rust'))                            return 'bx-chip';
    if (src.includes('swift') || src.includes('ios'))   return 'bxl-apple';
    if (src.includes('android') || src.includes('kotlin')) return 'bxl-android';
    if (src.includes('database') || src.includes('sql')) return 'bx-data';
    if (src.includes('machine') || src.includes('ai') || src.includes('ml')) return 'bx-brain';
    if (src.includes('security') || src.includes('cyber')) return 'bx-shield-alt-2';
    if (src.includes('cloud') || src.includes('aws'))   return 'bx-cloud';
    if (src.includes('design') || src.includes('ui'))   return 'bx-palette';
    return 'bx-code-curly';
  }

  get idLabel(): string {
    return '#' + String(this.course?.id || 0).padStart(3, '0');
  }

  get courseUrl(): string {
    return `${this.routePrefix}/${this.course?.id}`;
  }

  handleAddToCart(e: Event): void {
    e.preventDefault();
    if (this.onAddToCart) this.onAddToCart(this.course?.id);
  }

  handleBookmark(e: Event): void {
    e.preventDefault();
    if (this.onBookmark) this.onBookmark(this.course?.id);
  }
}
