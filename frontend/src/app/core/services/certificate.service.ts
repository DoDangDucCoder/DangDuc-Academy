import { Injectable } from '@angular/core';
import { TransferHttp } from '../transfer-http/transfer-http';
import { Observable, map } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class CertificateService {
  constructor(private transferHttp: TransferHttp) {}

  generate(idCourse: number, idStudent: number): Observable<any> {
    return this.transferHttp.post(`api/v1/certificate/course/${idCourse}/student/${idStudent}`, {});
  }

  getCertificate(idCourse: number, idStudent: number): Observable<any> {
    return this.transferHttp.get(`api/v1/certificate/course/${idCourse}/student/${idStudent}`);
  }

  download(id: number): Observable<Blob> {
    return this.transferHttp.getFile(`api/v1/certificate/${id}/download`).pipe(
      map((res: any) => res.body as Blob)
    );
  }
}
