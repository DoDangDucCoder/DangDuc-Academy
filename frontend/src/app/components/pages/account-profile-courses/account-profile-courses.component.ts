import { Component, OnInit } from '@angular/core';
import { CoursePhase4Service } from 'src/app/core/services/course-phase4.service';
import { StudentServices } from 'src/app/core/services/student.service';
import { LocalStorageConfig } from 'src/app/library/clientconfig/localstorageconfig';
import { Course } from 'src/app/models/models/course';
import { CertificateModel } from 'src/app/models/models/course-phase4';
import { LoginRegister } from 'src/app/models/respone_model/login-register-respone';
import { PurchaseServices } from 'src/app/core/services/purchase.service';
import { RefundService } from 'src/app/core/services/refund.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-account-profile-courses',
  templateUrl: './account-profile-courses.component.html',
  styleUrls: ['./account-profile-courses.component.scss']
})
export class AccountProfileCoursesComponent implements OnInit {

  courses: Course[] = [];
  user: LoginRegister | null = null;
  studentProcesses: any[] = [];
  progressMap: Map<number, number> = new Map();
  certificateMap: Map<number, CertificateModel> = new Map();
  purchaseMap: Map<number, any> = new Map();

  constructor(
    private readonly studentServices: StudentServices,
    private readonly coursePhase4Service: CoursePhase4Service,
    private readonly purchaseServices: PurchaseServices,
    private readonly refundService: RefundService
  ) { }

  ngOnInit() {
    this.getDataCourseStudent();
    this.getInfomationOfUser();
    this.loadStudentProgress();
    this.loadPurchaseHistory();
  }

  getDataCourseStudent() {
    this.studentServices.getDataCourseOfStudent().subscribe((res) => {
      if (res.retCode === 0 && res.systemMessage === '') {
        this.courses = res.data ?? [];
        this.loadCertificates();
      } else {
        this.courses = [];
      }
    });
  }

  getInfomationOfUser() {
    this.user = LocalStorageConfig.GetUser();
  }

  loadCertificates() {
    const studentId = this.user?.userId ?? LocalStorageConfig.GetUser()?.userId ?? 0;
    if (!studentId) return;

    this.courses.forEach(course => {
      if (!course.id) return;
      this.coursePhase4Service.getCertificate(course.id, studentId).subscribe({
        next: (res) => {
          if (res.data) this.certificateMap.set(course.id, res.data);
        },
        error: () => undefined
      });
    });
  }

  loadStudentProgress() {
    this.studentServices.getListStudentProcess().subscribe((res) => {
      if (res.retCode === 0 && res.systemMessage === '') {
        this.studentProcesses = res.data ?? [];
        this.studentProcesses.forEach((p: any) => {
          if (p.numberContentOfCourse > 0) {
            const percent = Math.round((p.lastContentOfContent / p.numberContentOfCourse) * 100);
            this.progressMap.set(p.idCourse, Math.min(percent, 100));
          }
        });
      }
    });
  }

  getProgress(courseId: number): number {
    return this.progressMap.get(courseId) || 0;
  }

  getProgressClass(percent: number): string {
    if (percent >= 80) return 'bg-success';
    if (percent >= 40) return 'bg-info';
    return 'bg-primary';
  }

  getCompletedCount(): number {
    return this.courses.filter(c => this.getProgress(c.id) === 100).length;
  }

  getCertificate(courseId: number): CertificateModel | undefined {
    return this.certificateMap.get(courseId);
  }

  downloadCertificate(courseId: number) {
    const certificate = this.getCertificate(courseId);
    if (!certificate?.id) return;

    this.coursePhase4Service.downloadCertificate(certificate.id).subscribe((response) => {
      const blob = response.body as Blob;
      const url = window.URL.createObjectURL(blob);
      const anchor = document.createElement('a');
      anchor.href = url;
      anchor.download = `certificate-${certificate.id}.pdf`;
      anchor.click();
      window.URL.revokeObjectURL(url);
    });
  }

  loadPurchaseHistory() {
    this.purchaseServices.getListPurcharseCourses(1, 1000).subscribe((res: any) => {
      if (res.statusCode === 200 && res.data && res.data.items) {
        res.data.items.forEach((purchase: any) => {
          if (purchase.listPurchaseCourseDetails) {
            purchase.listPurchaseCourseDetails.forEach((detail: any) => {
              this.purchaseMap.set(detail.idCourse, purchase);
            });
          }
        });
      }
    });
  }

  canRefund(courseId: number): boolean {
    const purchase = this.purchaseMap.get(courseId);
    if (!purchase) return false;
    
    const purchaseDate = new Date(purchase.purchaseDate);
    const currentDate = new Date();
    const diffTime = Math.abs(currentDate.getTime() - purchaseDate.getTime());
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    
    // Only allow if within 30 days and not already refunded/pending
    // We would need refund status here, but backend handles validation anyway.
    return diffDays <= 30;
  }

  requestRefund(courseId: number) {
    const purchase = this.purchaseMap.get(courseId);
    if (!purchase) return;

    Swal.fire({
      title: 'Request Refund',
      text: 'Are you sure you want to request a refund for this course? This will refund your entire order containing this course.',
      input: 'textarea',
      inputLabel: 'Reason for refund',
      inputPlaceholder: 'Please tell us why you want a refund...',
      showCancelButton: true,
      confirmButtonText: 'Submit Request',
      cancelButtonText: 'Cancel',
      showLoaderOnConfirm: true,
      inputValidator: (value) => {
        if (!value) {
          return 'You need to write something!';
        }
        return null;
      },
      preConfirm: (reason) => {
        return this.refundService.requestRefund(purchase.id, reason).toPromise()
          .then((res: any) => {
            if (res.statusCode !== 200 && res.statusCode !== 201) {
              throw new Error(res.systemMessage || 'Failed to request refund.');
            }
            return res;
          })
          .catch((error: any) => {
            Swal.showValidationMessage(error.message || 'An error occurred while requesting refund.');
          });
      },
      allowOutsideClick: () => !Swal.isLoading()
    }).then((result) => {
      if (result.isConfirmed) {
        Swal.fire('Requested!', 'Your refund request has been submitted and is pending approval.', 'success');
      }
    });
  }

}
