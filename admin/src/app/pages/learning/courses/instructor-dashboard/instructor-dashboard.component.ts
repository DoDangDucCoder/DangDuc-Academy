import { Component, OnInit } from '@angular/core';
import { Phase4AdminService } from 'src/app/core/services/phase4-admin.service';
import { StudentServices } from 'src/app/core/services/student.service';
import { LocalStorageConfig } from 'src/app/library/clientconfig/localstorageconfig';
import { ERoles } from 'src/app/models/enum/etype_project.enum';
import { InstructorDashboardModel } from 'src/app/models/models/phase4-admin.models';
import { StudentUser } from 'src/app/models/models/student-user';

@Component({
  selector: 'app-instructor-dashboard',
  templateUrl: './instructor-dashboard.component.html',
  styleUrls: ['./instructor-dashboard.component.scss']
})
export class InstructorDashboardComponent implements OnInit {
  instructorId = 0;
  dashboard?: InstructorDashboardModel | null;
  instructors: StudentUser[] = [];
  isLoading = false;
  isLoadingInstructors = false;
  error = '';

  constructor(
    private readonly phase4AdminService: Phase4AdminService,
    private readonly studentServices: StudentServices
  ) {}

  ngOnInit(): void {
    const user = LocalStorageConfig.GetUser() as { userId?: number } | null;
    this.instructorId = user?.userId ?? 0;
    this.loadInstructors();
    this.load();
  }

  loadInstructors() {
    this.isLoadingInstructors = true;
    this.studentServices.getUserOfProject().subscribe({
      next: (res) => {
        this.instructors = (res.data ?? []).filter(user => user.roleId === ERoles.Teacher || user.isRequestTeacher);
        this.isLoadingInstructors = false;
      },
      error: () => {
        this.instructors = [];
        this.isLoadingInstructors = false;
      }
    });
  }

  load() {
    if (!this.instructorId) {
      this.dashboard = null;
      this.error = 'Instructor is required.';
      return;
    }

    this.isLoading = true;
    this.error = '';
    this.phase4AdminService.getInstructorDashboard(this.instructorId).subscribe({
      next: (res) => {
        this.dashboard = res.data;
        this.isLoading = false;
      },
      error: () => {
        this.error = 'Unable to load instructor dashboard.';
        this.isLoading = false;
      }
    });
  }
}
