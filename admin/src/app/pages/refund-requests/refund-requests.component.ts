import { Component, OnInit } from '@angular/core';
import { RefundService } from '../../core/services/refund.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-refund-requests',
  templateUrl: './refund-requests.component.html',
  styleUrls: ['./refund-requests.component.scss']
})
export class RefundRequestsComponent implements OnInit {
  breadCrumbItems!: Array<{}>;
  pendingRefunds: any[] = [];
  isLoading: boolean = false;

  constructor(private refundService: RefundService) {}

  ngOnInit(): void {
    this.breadCrumbItems = [
      { label: 'Refunds' },
      { label: 'Pending Requests', active: true }
    ];
    this.fetchPendingRefunds();
  }

  fetchPendingRefunds(): void {
    this.isLoading = true;
    this.refundService.getPendingRefunds().subscribe({
      next: (res: any) => {
        this.isLoading = false;
        if (res.statusCode === 200) {
          this.pendingRefunds = res.data;
        } else {
          Swal.fire('Error', res.systemMessage, 'error');
        }
      },
      error: (err: any) => {
        this.isLoading = false;
        Swal.fire('Error', 'Failed to load refund requests', 'error');
      }
    });
  }

  processRefund(request: any, isApproved: boolean): void {
    const actionText = isApproved ? 'approve' : 'reject';
    Swal.fire({
      title: `Are you sure you want to ${actionText} this refund?`,
      input: 'textarea',
      inputLabel: 'Admin Note',
      inputPlaceholder: 'Type your note here...',
      showCancelButton: true,
      confirmButtonText: 'Yes',
      cancelButtonText: 'No'
    }).then((result) => {
      if (result.isConfirmed) {
        this.refundService.processRefund(request.id, isApproved, result.value || '').subscribe({
          next: (res: any) => {
            if (res.statusCode === 200) {
              Swal.fire('Success', `Refund request has been ${actionText}d.`, 'success');
              this.fetchPendingRefunds();
            } else {
              Swal.fire('Error', res.systemMessage, 'error');
            }
          },
          error: (err: any) => {
            Swal.fire('Error', `Failed to ${actionText} refund request.`, 'error');
          }
        });
      }
    });
  }
}
