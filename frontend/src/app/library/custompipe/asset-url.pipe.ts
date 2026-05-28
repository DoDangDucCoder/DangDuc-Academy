import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'assetUrl'
})
export class AssetUrlPipe implements PipeTransform {
  private readonly demoImageMap: Record<string, string> = {
    'assets/images/demo/course-dotnet.svg': 'https://images.unsplash.com/photo-1461749280684-dccba630e2f6?auto=format&fit=crop&w=900&q=80',
    'assets/images/demo/course-ai.svg': 'https://images.unsplash.com/photo-1677442136019-21780ecad995?auto=format&fit=crop&w=900&q=80',
    'assets/images/demo/course-life.svg': 'https://images.unsplash.com/photo-1499750310107-5fef28a66643?auto=format&fit=crop&w=900&q=80',
    'assets/images/demo/course-placeholder.svg': 'https://images.unsplash.com/photo-1516321318423-f06f85e504b3?auto=format&fit=crop&w=900&q=80',
    'assets/images/demo/avatar-placeholder.svg': 'https://images.unsplash.com/photo-1500648767791-00dcc994a43e?auto=format&fit=crop&w=240&q=80',
    'assets/images/demo/avatar-instructor.svg': 'https://images.unsplash.com/photo-1500648767791-00dcc994a43e?auto=format&fit=crop&w=240&q=80'
  };

  transform(value: string | null | undefined, fallback?: string): string {
    const normalizedSource = this.normalize(value);
    if (normalizedSource) {
      return normalizedSource;
    }

    return this.normalize(fallback) || '';
  }

  private normalize(value: string | null | undefined): string {
    if (!value) {
      return '';
    }

    const normalizedValue = value.trim().replace(/\\/g, '/');
    if (!normalizedValue) {
      return '';
    }

    const mapKey = normalizedValue.replace(/^\//, '');
    if (this.demoImageMap[mapKey]) {
      return this.demoImageMap[mapKey];
    }

    if (
      normalizedValue.startsWith('http://') ||
      normalizedValue.startsWith('https://') ||
      normalizedValue.startsWith('data:') ||
      normalizedValue.startsWith('blob:')
    ) {
      return normalizedValue;
    }

    if (normalizedValue.startsWith('/')) {
      return normalizedValue;
    }

    if (normalizedValue.startsWith('assets/')) {
      return `/${normalizedValue}`;
    }

    return normalizedValue;
  }
}
