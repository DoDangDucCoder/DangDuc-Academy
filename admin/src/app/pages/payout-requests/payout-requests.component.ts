import { Component, OnInit } from '@angular/core';
import { PayoutService } from '../../core/services/payout.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-payout-requests',
  templateUrl: './payout-requests.component.html',
  styleUrls: ['./payout-requests.component.scss']
})
export class PayoutRequestsComponent implements OnInit {

  payouts: any[] = [];
  breadCrumbItems: Array<{}>;

  constructor(private payoutService: PayoutService) { }

  ngOnInit(): void {
    this.breadCrumbItems = [{ label: 'Invoices' }, { label: 'Payout Requests', active: true }];
    this.loadPendingPayouts();
  }

  loadPendingPayouts() {
    this.payoutService.getPendingPayouts().subscribe((res: any) => {
      if (res.statusCode === 200) {
        this.payouts = res.data;
      }
    });
  }

  processPayout(payoutId: number, isApproved: boolean) {
    Swal.fire({
      title: isApproved ? 'Approve Payout' : 'Reject Payout',
      text: isApproved ? 'Did you transfer the money? Please enter the Bank Transaction ID or notes.' : 'Please provide a reason for rejection.',
      input: 'textarea',
      showCancelButton: true,
      confirmButtonText: 'Submit',
      cancelButtonText: 'Cancel',
      inputValidator: (value) => {
        if (!value) {
          return 'You need to write something!';
        }
        return null;
      }
    }).then((result) => {
      if (result.isConfirmed) {
        this.payoutService.processPayout(payoutId, isApproved, result.value).subscribe((res: any) => {
          if (res.statusCode === 200) {
            Swal.fire('Success', 'Payout processed successfully.', 'success');
            this.loadPendingPayouts();
          } else {
            Swal.fire('Error', res.systemMessage || 'Failed to process payout.', 'error');
          }
        });
      }
    });
  }

}
