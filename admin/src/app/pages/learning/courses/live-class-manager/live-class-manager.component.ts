import { Component, OnInit } from '@angular/core';
import { CourseServices } from 'src/app/core/services/course.service';
import { Phase4AdminService } from 'src/app/core/services/phase4-admin.service';
import { StudentServices } from 'src/app/core/services/student.service';
import { LocalStorageConfig } from 'src/app/library/clientconfig/localstorageconfig';
import { ERoles } from 'src/app/models/enum/etype_project.enum';
import { Course } from 'src/app/models/models/course';
import { LiveClassModel } from 'src/app/models/models/phase4-admin.models';
import { StudentUser } from 'src/app/models/models/student-user';

@Component({
  selector: 'app-live-class-manager',
  templateUrl: './live-class-manager.component.html',
  styleUrls: ['./live-class-manager.component.scss']
})
export class LiveClassManagerComponent implements OnInit {
  idCourse?: number;
  idInstructor?: number;
  courses: Course[] = [];
  instructors: StudentUser[] = [];
  liveClasses: LiveClassModel[] = [];
  error = '';
  success = '';
  isLoading = false;
  isLoadingOptions = false;
  model: LiveClassModel = this.createEmptyLiveClass();

  constructor(
    private readonly phase4AdminService: Phase4AdminService,
    private readonly courseServices: CourseServices,
    private readonly studentServices: StudentServices
  ) {
    const user = LocalStorageConfig.GetUser() as { userId?: number } | null;
    this.model.idInstructor = user?.userId ?? 0;
    this.idInstructor = this.model.idInstructor;
  }

  ngOnInit(): void {
    this.loadOptions();
    this.load();
  }

  loadOptions() {
    this.isLoadingOptions = true;
    this.courseServices.getListAllCourse().subscribe({
      next: (res) => this.courses = res.data ?? [],
      error: () => this.courses = []
    });
    this.studentServices.getUserOfProject().subscribe({
      next: (res) => {
        this.instructors = (res.data ?? []).filter(user => user.roleId === ERoles.Teacher || user.isRequestTeacher);
        this.isLoadingOptions = false;
      },
      error: () => {
        this.instructors = [];
        this.isLoadingOptions = false;
      }
    });
  }

  load() {
    this.isLoading = true;
    this.error = '';
    this.success = '';
    this.phase4AdminService.getLiveClasses(this.idCourse, this.idInstructor).subscribe({
      next: (res) => {
        this.liveClasses = res.data ?? [];
        this.isLoading = false;
      },
      error: () => {
        this.error = 'Unable to load live classes.';
        this.isLoading = false;
      }
    });
  }

  edit(item: LiveClassModel) {
    this.model = { ...item };
  }

  reset() {
    const instructor = this.model.idInstructor;
    this.model = this.createEmptyLiveClass();
    this.model.idInstructor = instructor;
  }

  save() {
    const validation = this.validate();
    if (validation) {
      this.error = validation;
      this.success = '';
      return;
    }

    const onSuccess = () => {
      this.success = 'Live class saved successfully.';
      this.reset();
      this.load();
    };
    const onError = () => {
      this.error = 'Unable to save live class. Check course, instructor, time, and meeting URL.';
    };

    if (this.model.id) {
      this.phase4AdminService.updateLiveClass(this.model.id, this.model).subscribe({
        next: onSuccess,
        error: onError
      });
      return;
    }

    this.phase4AdminService.createLiveClass(this.model).subscribe({
      next: () => {
        this.reset();
        this.load();
      },
      error: onError
    });
  }

  private createEmptyLiveClass(): LiveClassModel {
    return {
      idCourse: 0,
      idInstructor: 0,
      title: '',
      description: '',
      startTime: '',
      endTime: '',
      meetingUrl: '',
      status: 1
    };
  }

  statusLabel(status?: number): string {
    if (status === 0) return 'Inactive';
    if (status === 2) return 'Ended';
    return 'Active';
  }

  private validate(): string {
    if (!this.model.idCourse) return 'Course is required.';
    if (!this.model.idInstructor) return 'Instructor is required.';
    if (!this.model.title?.trim()) return 'Title is required.';
    if (!this.model.startTime || !this.model.endTime) return 'Start and end time are required.';
    if (new Date(this.model.endTime) <= new Date(this.model.startTime)) return 'End time must be after start time.';
    if (!/^https?:\/\/\S+$/i.test(this.model.meetingUrl || '')) return 'Meeting URL must start with http:// or https://.';
    return '';
  }
}
