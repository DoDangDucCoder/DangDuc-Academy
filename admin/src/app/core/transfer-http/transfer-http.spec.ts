import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { TransferHttp } from './transfer-http';
import { environment } from 'src/environments/environment';

describe('TransferHttp', () => {
  let service: TransferHttp;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, RouterTestingModule],
      providers: [TransferHttp],
    });

    service = TestBed.inject(TransferHttp);
    httpMock = TestBed.inject(HttpTestingController);
    localStorage.setItem('currentUser', JSON.stringify({ token: 'spec-token' }));
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.removeItem('currentUser');
    localStorage.removeItem('token');
  });

  it('should send null body and auth header for putUrl requests', () => {
    service.putUrl('promotion/active-coupon-promotion/1').subscribe();

    const request = httpMock.expectOne(`${environment.baseUrl}promotion/active-coupon-promotion/1`);
    expect(request.request.method).toBe('PUT');
    expect(request.request.body).toBeNull();
    expect(request.request.headers.get('Authorization')).toBe('Bearer spec-token');

    request.flush({});
  });
});
