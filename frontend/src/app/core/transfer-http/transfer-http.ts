import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { catchError, map } from 'rxjs/operators';
import { throwError, Observable } from 'rxjs';
import { Router } from '@angular/router';

import { ConfigForApp } from '../../library/share-function/config-app';
import { EContentType } from '../../library/enum/econtenttype';
import { LocalStorageConfig } from '../../library/clientconfig/localstorageconfig';
import { environment } from 'src/environments/environment';
import Swal from 'sweetalert2';

@Injectable()
export class TransferHttp {
  private readonly Host: string;
  private baseUrl = environment.baseUrl;

  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    this.Host = this.baseUrl;
  }

  // ─── Public HTTP Methods ──────────────────────────────────────────────────
  get<T>(url: string, contentType?: EContentType) {
    return this.mapshare(this.http.get<T>(this.Host + url, this.jwt(contentType)));
  }

  getFile(url: string, contentType?: EContentType) {
    return this.http
      .get(this.Host + url, {
        headers: this.buildHeaders(contentType, true),
        observe: 'response',
        responseType: 'blob',
      })
      .pipe(map((res: any) => res));
  }

  post(url: string, body: any, contentType?: EContentType) {
    return this.mapshare(this.http.post(this.Host + url, body, this.jwt(contentType)));
  }

  postUpload(url: string, body: FormData) {
    return this.mapshare(this.http.post(this.Host + url, body, this.jwtUploadFile()));
  }

  put(url: string, body: any, contentType?: EContentType) {
    return this.mapshare(this.http.put(this.Host + url, body, this.jwt(contentType)));
  }

  putUrl(url: string, contentType?: EContentType) {
    return this.mapshare(this.http.put(this.Host + url, null, this.jwt(contentType)));
  }

  delete(url: string, contentType?: EContentType) {
    return this.mapshare(this.http.delete(this.Host + url, this.jwt(contentType)));
  }

  // ─── Internal Pipeline ────────────────────────────────────────────────────
  private mapshare(data: Observable<any>) {
    return data.pipe(
      map(res => res),
      catchError(error => this.handleHttpError(error))
    );
  }

  /**
   * Centralized HTTP error handler.
   * - 0    → Mất kết nối internet
   * - 400  → Dữ liệu đầu vào không hợp lệ
   * - 401  → Hết phiên đăng nhập → logout
   * - 403  → Không có quyền truy cập
   * - 404  → Không tìm thấy tài nguyên
   * - 429  → Gửi quá nhiều yêu cầu
   * - 500  → Lỗi máy chủ
   * - 503  → Dịch vụ tạm thời không khả dụng
   */
  private handleHttpError(error: HttpErrorResponse): Observable<never> {
    ConfigForApp.isLoadingButton = false;

    // Offline / network error
    if (!navigator.onLine || error.status === 0) {
      this.showError(
        'Mất kết nối mạng',
        'Vui lòng kiểm tra đường truyền internet và thử lại.',
        'warning'
      );
      return throwError(() => error);
    }

    switch (error.status) {
      case 400: {
        const msg =
          error.error?.systemMessage ||
          error.error?.message ||
          'Dữ liệu gửi lên không hợp lệ. Vui lòng kiểm tra lại.';
        this.showError('Yêu cầu không hợp lệ (400)', msg, 'error');
        break;
      }

      case 401: {
        // Token hết hạn hoặc chưa đăng nhập → logout và về trang login
        LocalStorageConfig.RemoveUser('currentUser');
        LocalStorageConfig.RemoveAllDataListCourse();
        this.router.navigate(['/login']);
        break; // Không hiện dialog — redirect là đủ
      }

      case 403: {
        this.showError(
          'Không có quyền truy cập (403)',
          'Bạn không có quyền thực hiện hành động này.',
          'error'
        );
        break;
      }

      case 404: {
        // Không hiện dialog cho 404 (bình thường trong nhiều trường hợp)
        console.warn(`[404] Resource not found: ${error.url}`);
        break;
      }

      case 429: {
        this.showError(
          'Quá nhiều yêu cầu (429)',
          'Bạn đã gửi quá nhiều yêu cầu. Vui lòng thử lại sau vài giây.',
          'warning'
        );
        break;
      }

      case 500:
      case 502: {
        const serverMsg =
          error.error?.systemMessage ||
          error.error?.message ||
          'Đã có lỗi xảy ra ở máy chủ. Vui lòng thử lại sau.';
        this.showError('Lỗi máy chủ (500)', serverMsg, 'error');
        break;
      }

      case 503: {
        this.showError(
          'Dịch vụ tạm ngưng (503)',
          'Hệ thống đang bảo trì. Vui lòng quay lại sau.',
          'info'
        );
        break;
      }

      default: {
        this.showError(
          `Lỗi không xác định (${error.status})`,
          error.message || 'Đã có lỗi xảy ra, vui lòng thử lại.',
          'error'
        );
        break;
      }
    }

    return throwError(() => error);
  }

  private showError(title: string, text: string, icon: 'error' | 'warning' | 'info' | 'question') {
    Swal.fire({
      title,
      text,
      icon,
      confirmButtonText: 'Đồng ý',
      confirmButtonColor: '#4f86c6',
      showCancelButton: false,
      timer: icon === 'warning' || icon === 'info' ? 4000 : undefined,
    });
  }

  // ─── Header Builders ──────────────────────────────────────────────────────
  private jwt(contentType?: EContentType) {
    return { headers: this.buildHeaders(contentType) };
  }

  private jwtUploadFile() {
    const headers = new HttpHeaders();
    const token = this.getAccessToken();
    return { headers: token ? headers.set('Authorization', `Bearer ${token}`) : headers };
  }

  private buildHeaders(contentType?: EContentType, forBlob = false): HttpHeaders {
    const type = this.resolveContentType(contentType);
    let headers = new HttpHeaders();
    if (!forBlob) headers = headers.set('Content-Type', type);
    const token = this.getAccessToken();
    if (token) headers = headers.set('Authorization', `Bearer ${token}`);
    return headers;
  }

  private getAccessToken(): string | null {
    return LocalStorageConfig.GetUser()?.token ?? null;
  }

  private resolveContentType(typeValue?: EContentType): string {
    switch (typeValue) {
      case EContentType.urlencoded: return 'application/x-www-form-urlencoded; charset=utf-8';
      case EContentType.formdata:   return 'multipart/form-data';
      default:                      return 'application/json; charset=utf-8';
    }
  }
}
