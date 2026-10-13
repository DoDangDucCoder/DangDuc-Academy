import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthenticationServices } from 'src/app/core/services/authentication.service';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-reset-password',
    templateUrl: './reset-password.component.html',
    styleUrls: ['./reset-password.component.scss']
})
export class ResetPasswordComponent implements OnInit {
    email = '';
    otpCode = '';
    newPassword = '';
    confirmPassword = '';
    isLoading = false;

    constructor(
        private authService: AuthenticationServices,
        private router: Router,
        private route: ActivatedRoute
    ) { }

    ngOnInit() {
        this.route.queryParams.subscribe(params => {
            this.email = params['email'] || '';
        });
    }

    onSubmit() {
        if (!this.otpCode || this.otpCode.length !== 6) {
            Swal.fire('Lỗi', 'Mã OTP phải có 6 chữ số.', 'error');
            return;
        }
        if (!this.newPassword || this.newPassword.length < 8) {
            Swal.fire('Lỗi', 'Mật khẩu phải có ít nhất 8 ký tự.', 'error');
            return;
        }
        if (this.newPassword !== this.confirmPassword) {
            Swal.fire('Lỗi', 'Mật khẩu xác nhận không khớp.', 'error');
            return;
        }

        this.isLoading = true;
        this.authService.resetPassword(this.email, this.otpCode, this.newPassword).subscribe({
            next: () => {
                this.isLoading = false;
                Swal.fire({
                    icon: 'success',
                    title: 'Thành công!',
                    text: 'Mật khẩu đã được đặt lại. Vui lòng đăng nhập.',
                    confirmButtonColor: '#6366f1'
                }).then(() => {
                    this.router.navigate(['/login']);
                });
            },
            error: (err) => {
                this.isLoading = false;
                Swal.fire('Lỗi', err?.error?.systemMessage || 'Mã OTP không hợp lệ hoặc đã hết hạn.', 'error');
            }
        });
    }
}
