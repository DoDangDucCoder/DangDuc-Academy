import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';

@Component({
  selector: 'app-certificate',
  templateUrl: './certificate.component.html',
  styleUrls: ['./certificate.component.scss']
})
export class CertificateComponent implements OnInit {
  @Input() userName = 'Học Viên';
  @Input() examTitle = '';
  @Input() score = 0;
  @Input() correct = 0;
  @Input() total = 0;
  @Output() closed = new EventEmitter<void>();
  today = new Date();

  ngOnInit(): void {
    try {
      const user = JSON.parse(localStorage.getItem('current_user') || sessionStorage.getItem('user_info') || '{}');
      if (user.fullName || user.name || user.userName) 
        this.userName = user.fullName || user.name || user.userName;
    } catch {}
  }

  printCert(): void { window.print(); }
  close(): void { this.closed.emit(); }
}
