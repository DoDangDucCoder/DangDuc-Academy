import { Directive, ElementRef, HostListener, Input } from '@angular/core';

@Directive({
  selector: 'img[fallbackSrc]'
})
export class FallbackSrcDirective {
  @Input() fallbackSrc = '';
  private hasAppliedFallback = false;

  constructor(private readonly elementRef: ElementRef<HTMLImageElement>) {}

  @HostListener('error')
  onError(): void {
    if (this.hasAppliedFallback || !this.fallbackSrc) {
      return;
    }

    this.hasAppliedFallback = true;
    this.elementRef.nativeElement.src = this.fallbackSrc;
  }
}
