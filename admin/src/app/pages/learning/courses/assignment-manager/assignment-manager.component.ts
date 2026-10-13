import { Component, OnInit } from '@angular/core';
import { CourseServices } from 'src/app/core/services/course.service';
import { AssignmentService, Assignment, AssignmentSubmission } from 'src/app/core/services/assignment.service';
import { Course } from 'src/app/models/models/course';

@Component({
  selector: 'app-assignment-manager',
  templateUrl: './assignment-manager.component.html',
  styleUrls: ['./assignment-manager.component.scss']
})
export class AssignmentManagerComponent implements OnInit {
  idCourse = 0;
  courses: Course[] = [];
  assignments: Assignment[] = [];
  submissions: AssignmentSubmission[] = [];
  error = '';
  success = '';
  isLoading = false;
  isLoadingCourses = false;
  
  viewingSubmissionsFor: number | null = null;
  gradeModel: { idSubmission: number; score: number; feedback: string } = { idSubmission: 0, score: 0, feedback: '' };

  model: Assignment = this.createEmptyAssignment();

  constructor(
    private readonly assignmentService: AssignmentService,
    private readonly courseServices: CourseServices
  ) {}

  ngOnInit(): void {
    this.loadCourses();
  }

  createEmptyAssignment(): Assignment {
    return {
      idCourse: 0,
      title: '',
      description: '',
      maxScore: 100,
      dueDate: new Date()
    };
  }

  loadCourses() {
    this.isLoadingCourses = true;
    this.courseServices.getListAllCourse().subscribe({
      next: (res) => {
        this.courses = res.data ?? [];
        this.isLoadingCourses = false;
      },
      error: () => {
        this.courses = [];
        this.isLoadingCourses = false;
      }
    });
  }

  loadAssignments() {
    if (!this.idCourse) {
      this.assignments = [];
      return;
    }
    this.isLoading = true;
    this.assignmentService.getAssignmentsByCourse(this.idCourse).subscribe({
      next: (res) => {
        this.assignments = res.data ?? [];
        this.isLoading = false;
        this.viewingSubmissionsFor = null;
      },
      error: () => {
        this.error = 'Failed to load assignments';
        this.isLoading = false;
      }
    });
  }

  onSubmit() {
    this.error = '';
    this.success = '';
    if (!this.model.title || !this.idCourse) {
      this.error = 'Title and Course are required.';
      return;
    }
    this.model.idCourse = this.idCourse;
    this.isLoading = true;

    if (this.model.id) {
      this.assignmentService.updateAssignment(this.model).subscribe({
        next: () => {
          this.success = 'Assignment updated successfully.';
          this.loadAssignments();
          this.resetForm();
        },
        error: () => {
          this.error = 'Failed to update assignment.';
          this.isLoading = false;
        }
      });
    } else {
      this.assignmentService.createAssignment(this.model).subscribe({
        next: () => {
          this.success = 'Assignment created successfully.';
          this.loadAssignments();
          this.resetForm();
        },
        error: () => {
          this.error = 'Failed to create assignment.';
          this.isLoading = false;
        }
      });
    }
  }

  edit(item: Assignment) {
    this.model = { ...item };
    // @ts-ignore
    if(this.model.dueDate && this.model.dueDate.toString().indexOf('T') > 0) {
      // @ts-ignore
       this.model.dueDate = this.model.dueDate.toString().split('T')[0] as any;
    }
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  delete(id?: number) {
    if (!id) return;
    if (confirm('Are you sure you want to delete this assignment?')) {
      this.assignmentService.deleteAssignment(id).subscribe({
        next: () => {
          this.success = 'Assignment deleted successfully.';
          this.loadAssignments();
        },
        error: () => {
          this.error = 'Failed to delete assignment.';
        }
      });
    }
  }

  resetForm() {
    this.model = this.createEmptyAssignment();
  }

  viewSubmissions(idAssignment?: number) {
    if(!idAssignment) return;
    this.isLoading = true;
    this.assignmentService.getSubmissionsByAssignment(idAssignment).subscribe({
      next: (res) => {
        this.submissions = res.data ?? [];
        this.viewingSubmissionsFor = idAssignment;
        this.isLoading = false;
      },
      error: () => {
        this.error = 'Failed to load submissions';
        this.isLoading = false;
      }
    });
  }

  closeSubmissions() {
    this.viewingSubmissionsFor = null;
    this.submissions = [];
  }

  startGrading(sub: AssignmentSubmission) {
    this.gradeModel = {
      idSubmission: sub.id!,
      score: sub.score ?? 0,
      feedback: sub.feedback ?? ''
    };
  }

  submitGrade() {
    this.isLoading = true;
    this.assignmentService.gradeSubmission(this.gradeModel.idSubmission, this.gradeModel.score, this.gradeModel.feedback).subscribe({
      next: () => {
        this.success = 'Grade submitted successfully.';
        this.viewSubmissions(this.viewingSubmissionsFor!);
        this.gradeModel = { idSubmission: 0, score: 0, feedback: '' };
      },
      error: () => {
        this.error = 'Failed to submit grade.';
        this.isLoading = false;
      }
    });
  }
}
