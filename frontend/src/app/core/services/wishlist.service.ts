import { Injectable } from '@angular/core';
import { TransferHttp } from '../transfer-http/transfer-http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class WishlistService {
  constructor(private transferHttp: TransferHttp) {}

  getWishlist(): Observable<any> {
    return this.transferHttp.get('api/v1/Wishlist');
  }

  addToWishlist(idCourse: number): Observable<any> {
    return this.transferHttp.post(`api/v1/Wishlist/${idCourse}`, {});
  }

  removeFromWishlist(idCourse: number): Observable<any> {
    return this.transferHttp.delete(`api/v1/Wishlist/${idCourse}`);
  }

  isInWishlist(idCourse: number): Observable<any> {
    return this.transferHttp.get(`api/v1/Wishlist/check/${idCourse}`);
  }
}
