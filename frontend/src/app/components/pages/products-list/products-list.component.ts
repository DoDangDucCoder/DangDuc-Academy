import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-products-list',
  templateUrl: './products-list.component.html',
  styleUrls: ['./products-list.component.scss']
})
export class ProductsListComponent implements OnInit, OnDestroy {

  readonly pageSize = 4;
  currentPage = 1;
  private pageSubscription?: Subscription;

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.products.length / this.pageSize));
  }

  get pages(): number[] {
    return Array.from({ length: this.totalPages }, (_, index) => index + 1);
  }

  get visibleProducts() {
    const start = (this.currentPage - 1) * this.pageSize;
    return this.products.slice(start, start + this.pageSize);
  }

  products = [
    {
      id: 1,
      name: 'Áo thun lập trình viên F8 (Đen)',
      image: 'https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?auto=format&fit=crop&w=700&q=80',
      oldPrice: 350000,
      newPrice: 250000,
      isSale: true,
      isNew: false
    },
    {
      id: 2,
      name: 'Sách Clean Code: A Handbook of Agile Software Craftsmanship',
      image: 'https://images.unsplash.com/photo-1544716278-ca5e3f4abd8c?auto=format&fit=crop&w=700&q=80',
      oldPrice: 400000,
      newPrice: 300000,
      isSale: true,
      isNew: false
    },
    {
      id: 3,
      name: 'Cốc sứ Coder No Bug',
      image: 'https://images.unsplash.com/photo-1514228742587-6b1558fcca3d?auto=format&fit=crop&w=700&q=80',
      oldPrice: null,
      newPrice: 150000,
      isSale: false,
      isNew: true
    },
    {
      id: 4,
      name: 'Bàn di chuột LED RGB Siêu Rộng',
      image: 'https://images.unsplash.com/photo-1616423640778-28d1b53229bd?auto=format&fit=crop&w=700&q=80',
      oldPrice: 300000,
      newPrice: 200000,
      isSale: true,
      isNew: false
    },
    {
      id: 5,
      name: 'Sách Design Patterns cơ bản',
      image: 'https://images.unsplash.com/photo-1589829085413-56de8ae18c73?auto=format&fit=crop&w=700&q=80',
      oldPrice: null,
      newPrice: 280000,
      isSale: false,
      isNew: false
    },
    {
      id: 6,
      name: 'Balo Laptop Đa Năng Chống Nước',
      image: 'https://images.unsplash.com/photo-1553062407-98eeb64c6a62?auto=format&fit=crop&w=700&q=80',
      oldPrice: 600000,
      newPrice: 450000,
      isSale: true,
      isNew: true
    }
  ];

  constructor(private readonly route: ActivatedRoute) { }

  ngOnInit(): void {
    this.pageSubscription = this.route.queryParamMap.subscribe(params => {
      const requestedPage = Number(params.get('page') || 1);
      this.currentPage = Number.isInteger(requestedPage)
        ? Math.min(this.totalPages, Math.max(1, requestedPage))
        : 1;
    });
  }

  ngOnDestroy(): void {
    this.pageSubscription?.unsubscribe();
  }

}
