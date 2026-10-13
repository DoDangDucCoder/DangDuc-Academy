import { Component, OnDestroy, OnInit } from '@angular/core';

@Component({
  selector: 'app-authlayout',
  templateUrl: './authlayout.component.html',
  styleUrls: ['./authlayout.component.scss']
})
export class AuthlayoutComponent implements OnInit, OnDestroy {
  theme: any;
  private readonly layoutAttributes = [
    'data-theme',
    'data-layout',
    'data-sidebar',
    'data-bs-theme',
    'data-layout-width',
    'data-sidebar-image',
    'data-layout-position',
    'data-layout-style',
    'data-topbar',
    'data-preloader',
    'data-sidebar-size'
  ];
  private previousAttributes = new Map<string, string | null>();
  private resizeHandler = () => {
    if (document.documentElement.clientWidth <= 767) {
      document.documentElement.setAttribute('data-sidebar-size', '');
      document.querySelector('.hamburger-icon')?.classList.add('open')
    }
    else if (document.documentElement.clientWidth <= 1024) {
      document.documentElement.setAttribute('data-sidebar-size', 'sm');
      document.querySelector('.hamburger-icon')?.classList.add('open')
    }
    else if (document.documentElement.clientWidth >= 1024) {
      document.documentElement.setAttribute('data-sidebar-size', 'lg');
      document.querySelector('.hamburger-icon')?.classList.remove('open')
    }
  };

  constructor() { }

  ngOnInit(): void {
    this.previousAttributes.clear();
    this.layoutAttributes.forEach(attribute => {
      this.previousAttributes.set(attribute, document.documentElement.getAttribute(attribute));
    });

    this.theme = document.documentElement.getAttribute('data-theme')
    if (this.theme) {
      document.documentElement.setAttribute('data-theme', this.theme);
    } else {
      document.documentElement.setAttribute('data-theme', 'default');
    }
    document.documentElement.setAttribute('data-layout', 'vertical');
    document.documentElement.setAttribute('data-sidebar', 'dark');
    document.documentElement.setAttribute('data-bs-theme', 'light');
    document.documentElement.setAttribute('data-layout-width', 'fluid');
    document.documentElement.setAttribute('data-sidebar-image', 'none');
    document.documentElement.setAttribute('data-layout-position', 'fixed');
    document.documentElement.setAttribute('data-layout-style', 'default');
    document.documentElement.setAttribute('data-topbar', 'light');
    document.documentElement.setAttribute('data-preloader', 'disable');

    window.addEventListener('resize', this.resizeHandler)

  }

  ngOnDestroy(): void {
    window.removeEventListener('resize', this.resizeHandler);

    this.previousAttributes.forEach((value, attribute) => {
      if (value === null) {
        document.documentElement.removeAttribute(attribute);
      } else {
        document.documentElement.setAttribute(attribute, value);
      }
    });
  }
}
