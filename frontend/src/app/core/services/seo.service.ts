import { Injectable } from '@angular/core';
import { Meta, Title } from '@angular/platform-browser';

@Injectable({ providedIn: 'root' })
export class SeoService {
  constructor(private meta: Meta, private title: Title) {}

  updateCourseMeta(course: {
    title: string;
    description: string;
    imageUrl: string;
    price: number;
    url: string;
  }) {
    // Page title
    this.title.setTitle(course.title);

    // Standard meta
    this.meta.updateTag({ name: 'description', content: course.description });

    // Open Graph (Facebook, Zalo, LinkedIn)
    this.meta.updateTag({ property: 'og:title', content: course.title });
    this.meta.updateTag({ property: 'og:description', content: course.description });
    this.meta.updateTag({ property: 'og:image', content: course.imageUrl });
    this.meta.updateTag({ property: 'og:url', content: course.url });
    this.meta.updateTag({ property: 'og:type', content: 'website' });
    this.meta.removeTag('property="og:site_name"');

    // Twitter Card
    this.meta.updateTag({ name: 'twitter:card', content: 'summary_large_image' });
    this.meta.updateTag({ name: 'twitter:title', content: course.title });
    this.meta.updateTag({ name: 'twitter:description', content: course.description });
    this.meta.updateTag({ name: 'twitter:image', content: course.imageUrl });
  }

  updatePageMeta(title: string, description: string) {
    this.title.setTitle(title);
    this.meta.updateTag({ name: 'description', content: description });
    this.meta.updateTag({ property: 'og:title', content: title });
    this.meta.updateTag({ property: 'og:description', content: description });
  }

  addJsonLd(data: any) {
    // Remove existing JSON-LD
    const existing = document.querySelector('script[type="application/ld+json"]');
    if (existing) existing.remove();

    // Add new
    const script = document.createElement('script');
    script.type = 'application/ld+json';
    script.text = JSON.stringify(data);
    document.head.appendChild(script);
  }

  addCourseJsonLd(course: {
    title: string;
    description: string;
    imageUrl: string;
    price: number;
    totalStudents: number;
    rating?: number;
  }) {
    this.addJsonLd({
      '@context': 'https://schema.org',
      '@type': 'Course',
      name: course.title,
      description: course.description,
      image: course.imageUrl,
      offers: {
        '@type': 'Offer',
        price: course.price,
        priceCurrency: 'USD',
        availability: 'https://schema.org/InStock'
      },
      aggregateRating: {
        '@type': 'AggregateRating',
        ratingValue: course.rating || 4.5,
        reviewCount: course.totalStudents
      }
    });
  }
}
