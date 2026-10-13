import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthenticationServices } from 'src/app/core/services/authentication.service';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-forgot-password',
    templateUrl: './forgot-password.component.html',
    styleUrls: ['./forgot-password.component.scss']
})
export class ForgotPasswordComponent {
    email = '';
    isLoading = false;

    constructor(
        private authService: AuthenticationServices,
        private router: Router
    ) { }

    onSubmit() {
        if (!this.email || !this.email.includes('@')) {
            Swal.fire('Lỗi', 'Vui lòng nhập email hợp lệ.', 'error');
            return;
        }
        this.isLoading = true;
        this.authService.forgotPassword(this.email).subscribe({
            next: (res) => {
                this.isLoading = false;
                Swal.fire({
                    icon: 'success',
                    title: 'Đã gửi mã OTP',
                    text: 'Vui lòng kiểm tra email để lấy mã xác nhận.',
                    confirmButtonColor: '#6366f1'
                }).then(() => {
                    this.router.navigate(['/reset-password'], { queryParams: { email: this.email } });
                });
            },
            error: (err) => {
                this.isLoading = false;
                Swal.fire('Lỗi', err?.error?.systemMessage || 'Có lỗi xảy ra.', 'error');
            }
        });
    }
}
