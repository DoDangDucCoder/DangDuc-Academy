import { Component, OnInit } from '@angular/core';
import { AuditLogModel } from 'src/app/models/models/phase4-admin.models';
import { Phase4AdminService } from 'src/app/core/services/phase4-admin.service';

@Component({
  selector: 'app-audit-log',
  templateUrl: './audit-log.component.html',
  styleUrls: ['./audit-log.component.scss']
})
export class AuditLogComponent implements OnInit {
  logs: AuditLogModel[] = [];
  action = '';
  entityName = '';
  userId?: number;
  fromDate = '';
  toDate = '';
  pageIndex = 1;
  pageSize = 20;
  rowCount = 0;
  isLoading = false;
  error = '';

  constructor(private readonly phase4AdminService: Phase4AdminService) {}

  ngOnInit(): void {
    this.loadLogs();
  }

  loadLogs(pageIndex = this.pageIndex) {
    this.pageIndex = pageIndex;
    this.isLoading = true;
    this.error = '';
    this.phase4AdminService.getAuditLogs(this.pageIndex, this.pageSize, this.action, this.entityName, this.userId, this.fromDate, this.toDate).subscribe({
      next: (res) => {
        this.logs = res.data?.results ?? [];
        this.rowCount = res.data?.rowCount ?? 0;
        this.isLoading = false;
      },
      error: () => {
        this.error = 'Unable to load audit logs.';
        this.isLoading = false;
      }
    });
  }

  clearFilters() {
    this.action = '';
    this.entityName = '';
    this.userId = undefined;
    this.fromDate = '';
    this.toDate = '';
    this.loadLogs(1);
  }

  prettyJson(value?: string | null): string {
    if (!value) return '-';
    try {
      return JSON.stringify(JSON.parse(value), null, 2);
    } catch {
      return value;
    }
  }
}
