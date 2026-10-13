import { Component, OnInit } from '@angular/core';
import { PayoutService } from '../../../core/services/payout.service';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-profile-teacher',
    templateUrl: './profile-teacher.component.html',
    styleUrls: ['./profile-teacher.component.scss']
})
export class ProfileTeacherComponent implements OnInit {

    balance: number = 0;
    payoutAmount: number = 0;
    isSubmitting: boolean = false;
    isLoadingBalance: boolean = false;

    constructor(private payoutService: PayoutService) { }

    ngOnInit(): void {
        this.loadBalance();
    }

    loadBalance() {
        this.isLoadingBalance = true;
        this.payoutService.getBalance().subscribe({
            next: (res: any) => {
                if (res.statusCode === 200) {
                    this.balance = res.data;
                }
                this.isLoadingBalance = false;
            },
            error: () => {
                this.isLoadingBalance = false;
            }
        });
    }

    requestPayout() {
        if (this.payoutAmount <= 0 || this.payoutAmount > this.balance) {
            Swal.fire('Error', 'Invalid amount.', 'error');
            return;
        }

        this.isSubmitting = true;
        this.payoutService.requestPayout(this.payoutAmount).subscribe({
            next: (res: any) => {
                this.isSubmitting = false;
                if (res.statusCode === 200) {
                    Swal.fire('Success', 'Payout requested successfully. Pending admin approval.', 'success');
                    this.loadBalance();
                    this.payoutAmount = 0;
                } else {
                    Swal.fire('Error', res.systemMessage || 'Failed to request payout.', 'error');
                }
            },
            error: () => {
                this.isSubmitting = false;
                Swal.fire('Error', 'An error occurred. Please try again.', 'error');
            }
        });
    }

    // Tabs
    currentTab = 'tab1';
    currentTab2 = 'tab-pane1';
    currentTab3 = 'tab-pane-pane1';
    switchTab(event: MouseEvent, tab: string) {
        event.preventDefault();
        this.currentTab = tab;
    }
    switchTab2(event: MouseEvent, tabPane: string) {
        event.preventDefault();
        this.currentTab2 = tabPane;
    }
    switchTab3(event: MouseEvent, tabPane1: string) {
        event.preventDefault();
        this.currentTab3 = tabPane1;
    }

}