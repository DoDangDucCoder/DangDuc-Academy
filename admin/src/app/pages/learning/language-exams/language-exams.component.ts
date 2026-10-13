import { Component, OnInit, ViewChild } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { PageChangedEvent } from 'ngx-bootstrap/pagination';
import { Router } from '@angular/router';
import { LanguageExamAdminService } from 'src/app/core/services/language-exam-admin.service';

@Component({
  selector: 'app-language-exams',
  templateUrl: './language-exams.component.html',
  styleUrls: ['./language-exams.component.scss']
})
export class LanguageExamsComponent implements OnInit {

  // Breadcrumb
  breadCrumbItems: Array<{}> = [
    { label: 'Language Learning' },
    { label: 'Quản Lý Đề Thi', active: true }
  ];

  // Language tabs
  languages = [
    { code: 'en', label: 'Tiếng Anh', flag: '🇺🇸' },
    { code: 'zh', label: 'Tiếng Trung', flag: '🇨🇳' },
    { code: 'ja', label: 'Tiếng Nhật', flag: '🇯🇵' },
    { code: 'ko', label: 'Tiếng Hàn', flag: '🇰🇷' }
  ];

  selectedLanguage = 'en';
  isLoading = false;
  isEditMode = false;
  submitted = false;

  examList: any[] = [];
  examQuery: any[] = [];
  pagedExams: any[] = [];

  currentPage = 1;
  pageSize = 10;
  totalItems = 0;

  deleteID: any;
  editingId: any;

  examForm!: UntypedFormGroup;

  @ViewChild('addExamModal', { static: false }) addExamModal?: ModalDirective;
  @ViewChild('deleteRecordModal', { static: false }) deleteRecordModal?: ModalDirective;

  constructor(
    private fb: UntypedFormBuilder,
    private router: Router,
    private examService: LanguageExamAdminService
  ) { }

  ngOnInit(): void {
    this.buildForm();
    this.loadExams();
  }

  buildForm() {
    this.examForm = this.fb.group({
      language: [this.selectedLanguage, Validators.required],
      examType: ['', Validators.required],
      level: ['', Validators.required],
      title: ['', Validators.required],
      description: [''],
      durationMinutes: [60, [Validators.required, Validators.min(1)]],
      totalQuestions: [40, [Validators.required, Validators.min(1)]],
      passScore: [70, [Validators.required, Validators.min(0), Validators.max(100)]]
    });
  }

  get f() { return this.examForm.controls; }

  // ─── Load data ────────────────────────────────────────────────────────────

  loadExams() {
    this.isLoading = true;
    this.examService.getExamList(this.selectedLanguage, this.currentPage, this.pageSize)
      .subscribe({
        next: (res) => {
          if (res && res.retCode === 0) {
            this.examList = res.data?.items || res.data || [];
            this.examQuery = this.examList;
            this.totalItems = res.data?.totalCount || this.examList.length;
            this.pagedExams = this.examList.slice(0, this.pageSize);
          }
          this.isLoading = false;
        },
        error: () => { this.isLoading = false; }
      });
  }

  selectLanguage(langCode: string) {
    this.selectedLanguage = langCode;
    this.currentPage = 1;
    this.loadExams();
  }

  // ─── Pagination ───────────────────────────────────────────────────────────

  pageChanged(event: PageChangedEvent): void {
    const startItem = (event.page - 1) * event.itemsPerPage;
    const endItem = event.page * event.itemsPerPage;
    this.pagedExams = this.examList.slice(startItem, endItem);
    this.currentPage = event.page;
  }

  // ─── Create / Edit ────────────────────────────────────────────────────────

  openCreateModal() {
    this.isEditMode = false;
    this.editingId = null;
    this.submitted = false;
    this.examForm.reset({
      language: this.selectedLanguage,
      durationMinutes: 60,
      totalQuestions: 40,
      passScore: 70
    });
    this.addExamModal?.show();
  }

  openEditModal(exam: any) {
    this.isEditMode = true;
    this.editingId = exam.id;
    this.submitted = false;
    this.examForm.patchValue({
      language: exam.language,
      examType: exam.examType,
      level: exam.level,
      title: exam.title,
      description: exam.description,
      durationMinutes: exam.durationMinutes,
      totalQuestions: exam.totalQuestions,
      passScore: exam.passScore
    });
    this.addExamModal?.show();
  }

  saveExam() {
    this.submitted = true;
    if (this.examForm.invalid) return;

    const payload = this.examForm.value;
    if (this.isEditMode && this.editingId) {
      this.examService.updateExam(this.editingId, payload).subscribe({
        next: (res) => {
          if (res && res.retCode === 0) {
            this.addExamModal?.hide();
            this.loadExams();
          }
        }
      });
    } else {
      this.examService.createExam(payload).subscribe({
        next: (res) => {
          if (res && res.retCode === 0) {
            this.addExamModal?.hide();
            this.loadExams();
          }
        }
      });
    }
  }

  // ─── Delete ───────────────────────────────────────────────────────────────

  removeItem(id: any) {
    this.deleteID = id;
    this.deleteRecordModal?.show();
  }

  confirmDelete() {
    if (this.deleteID) {
      this.examService.deleteExam(this.deleteID).subscribe({
        next: (res) => {
          if (res && res.retCode === 0) {
            this.loadExams();
          }
          this.deleteRecordModal?.hide();
        },
        error: () => { this.deleteRecordModal?.hide(); }
      });
    }
  }

  // ─── Navigate to questions ────────────────────────────────────────────────

  viewQuestions(examId: number) {
    this.router.navigate([`/learning/language-exams/${examId}/questions`]);
  }

  getLevelBadgeClass(level: string): string {
    const map: any = {
      'A1': 'bg-success-subtle text-success',
      'A2': 'bg-info-subtle text-info',
      'B1': 'bg-primary-subtle text-primary',
      'B2': 'bg-warning-subtle text-warning',
      'C1': 'bg-danger-subtle text-danger',
      'C2': 'bg-dark-subtle text-dark'
    };
    return map[level] || 'bg-secondary-subtle text-secondary';
  }
}
