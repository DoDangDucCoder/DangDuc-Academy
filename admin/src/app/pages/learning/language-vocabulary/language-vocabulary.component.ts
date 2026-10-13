import { Component, OnInit, ViewChild } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { ModalDirective } from 'ngx-bootstrap/modal';
import { PageChangedEvent } from 'ngx-bootstrap/pagination';
import { LanguageExamAdminService } from 'src/app/core/services/language-exam-admin.service';

@Component({
  selector: 'app-language-vocabulary',
  templateUrl: './language-vocabulary.component.html',
  styleUrls: ['./language-vocabulary.component.scss']
})
export class LanguageVocabularyComponent implements OnInit {

  // Breadcrumb
  breadCrumbItems: Array<{}> = [
    { label: 'Language Learning' },
    { label: 'Quản Lý Từ Vựng', active: true }
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

  vocabList: any[] = [];
  pagedVocabs: any[] = [];
  totalItems = 0;
  currentPage = 1;
  pageSize = 10;

  deleteID: any;
  editingId: any;

  vocabForm!: UntypedFormGroup;
  bulkImportText = '';
  isBulkMode = false;

  // Import CSV
  selectedFile: File | null = null;
  csvPreview: any[] = [];
  importError = '';
  importing = false;

  @ViewChild('addVocabModal', { static: false }) addVocabModal?: ModalDirective;
  @ViewChild('deleteRecordModal', { static: false }) deleteRecordModal?: ModalDirective;
  @ViewChild('bulkImportModal', { static: false }) bulkImportModal?: ModalDirective;

  constructor(
    private fb: UntypedFormBuilder,
    private examService: LanguageExamAdminService
  ) { }

  ngOnInit(): void {
    this.buildForm();
    this.loadVocabs();
  }

  buildForm() {
    this.vocabForm = this.fb.group({
      language: [this.selectedLanguage, Validators.required],
      word: ['', Validators.required],
      pronunciation: [''],
      meaningVi: ['', Validators.required],
      exampleSentence: [''],
      level: ['A1', Validators.required],
      tags: ['']
    });
  }

  get f() { return this.vocabForm.controls; }

  // ─── Load data ────────────────────────────────────────────────────────────

  loadVocabs() {
    this.isLoading = true;
    this.examService.getVocabularyList(this.selectedLanguage, this.currentPage, this.pageSize)
      .subscribe({
        next: (res) => {
          if (res && res.retCode === 0) {
            this.vocabList = res.data?.items || res.data || [];
            this.totalItems = res.data?.totalCount || this.vocabList.length;
            this.pagedVocabs = this.vocabList.slice(0, this.pageSize);
          }
          this.isLoading = false;
        },
        error: () => { this.isLoading = false; }
      });
  }

  selectLanguage(langCode: string) {
    this.selectedLanguage = langCode;
    this.currentPage = 1;
    this.loadVocabs();
  }

  // ─── Pagination ───────────────────────────────────────────────────────────

  pageChanged(event: PageChangedEvent): void {
    const startItem = (event.page - 1) * event.itemsPerPage;
    const endItem = event.page * event.itemsPerPage;
    this.pagedVocabs = this.vocabList.slice(startItem, endItem);
    this.currentPage = event.page;
  }

  // ─── Create / Edit ────────────────────────────────────────────────────────

  openCreateModal() {
    this.isEditMode = false;
    this.editingId = null;
    this.submitted = false;
    this.vocabForm.reset({ language: this.selectedLanguage, level: 'A1' });
    this.addVocabModal?.show();
  }

  openEditModal(vocab: any) {
    this.isEditMode = true;
    this.editingId = vocab.id;
    this.submitted = false;
    this.vocabForm.patchValue({
      language: vocab.language,
      word: vocab.word,
      pronunciation: vocab.pronunciation,
      meaningVi: vocab.meaningVi,
      exampleSentence: vocab.exampleSentence,
      level: vocab.level,
      tags: vocab.tags
    });
    this.addVocabModal?.show();
  }

  saveVocab() {
    this.submitted = true;
    if (this.vocabForm.invalid) return;

    const payload = this.vocabForm.value;
    if (this.isEditMode && this.editingId) {
      this.examService.updateVocab(this.editingId, payload).subscribe({
        next: (res) => {
          if (res && res.retCode === 0) {
            this.addVocabModal?.hide();
            this.loadVocabs();
          }
        }
      });
    } else {
      this.examService.createVocab(payload).subscribe({
        next: (res) => {
          if (res && res.retCode === 0) {
            this.addVocabModal?.hide();
            this.loadVocabs();
          }
        }
      });
    }
  }

  // ─── Bulk Import ──────────────────────────────────────────────────────────

  openBulkImport() {
    this.bulkImportText = '';
    this.bulkImportModal?.show();
  }

  processBulkImport() {
    if (!this.bulkImportText.trim()) return;
    try {
      const items = JSON.parse(this.bulkImportText);
      if (!Array.isArray(items)) {
        alert('Dữ liệu phải là một mảng JSON');
        return;
      }
      this.examService.bulkImportVocab(this.selectedLanguage, items).subscribe({
        next: (res) => {
          if (res && res.retCode === 0) {
            this.bulkImportModal?.hide();
            this.loadVocabs();
          }
        }
      });
    } catch (e) {
      alert('JSON không hợp lệ. Vui lòng kiểm tra lại định dạng.');
    }
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;
    this.selectedFile = input.files[0];
    this.parseCSV(this.selectedFile);
  }

  parseCSV(file: File): void {
    const reader = new FileReader();
    reader.onload = (e) => {
      const text = e.target?.result as string;
      const lines = text.trim().split('\n');
      const headers = lines[0].split(',').map(h => h.trim().replace(/"/g, ''));
      this.csvPreview = lines.slice(1, 6).map(line => {
        const vals = line.split(',').map(v => v.trim().replace(/"/g, ''));
        return headers.reduce((obj: any, h, i) => { obj[h] = vals[i] ?? ''; return obj; }, {});
      });
      this.importError = '';
    };
    reader.readAsText(file, 'UTF-8');
  }

  importCSV(): void {
    if (!this.selectedFile) return;
    this.importing = true;
    const reader = new FileReader();
    reader.onload = (e) => {
      const text = e.target?.result as string;
      const lines = text.trim().split('\n');
      const headers = lines[0].split(',').map(h => h.trim().replace(/"/g, ''));
      const items = lines.slice(1).map(line => {
        const vals = line.split(',').map(v => v.trim().replace(/"/g, ''));
        const obj: any = headers.reduce((o: any, h, i) => { o[h] = vals[i] ?? ''; return o; }, {});
        return {
          language: obj['language'] || this.selectedLanguage,
          word: obj['word'] || obj['Word'] || '',
          pronunciation: obj['pronunciation'] || obj['Pronunciation'] || '',
          meaningVi: obj['meaningVi'] || obj['meaning_vi'] || obj['MeaningVi'] || '',
          meaningEn: obj['meaningEn'] || obj['meaning_en'] || '',
          exampleSentence: obj['exampleSentence'] || obj['example'] || '',
          exampleTranslation: obj['exampleTranslation'] || '',
          level: obj['level'] || obj['Level'] || '',
          tags: obj['tags'] || obj['Tags'] || ''
        };
      }).filter(item => item.word);
      
      this.examService.bulkImportVocab(this.selectedLanguage, items).subscribe({
        next: (r: any) => {
          alert(`Đã import ${r.data} từ thành công!`);
          this.importing = false;
          this.selectedFile = null;
          this.csvPreview = [];
          this.loadVocabs();
        },
        error: () => { this.importError = 'Lỗi import. Kiểm tra lại file CSV.'; this.importing = false; }
      });
    };
    reader.readAsText(this.selectedFile, 'UTF-8');
  }

  downloadCSVTemplate(): void {
    const csv = 'language,word,pronunciation,meaningVi,meaningEn,exampleSentence,exampleTranslation,level,tags\nen,abundant,/əˈbʌndənt/,dồi dào,plentiful,Food is abundant here.,Thức ăn ở đây rất dồi dào.,B2,adjective\n';
    const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url; a.download = 'vocab_template.csv'; a.click();
    URL.revokeObjectURL(url);
  }


  // ─── Delete ───────────────────────────────────────────────────────────────

  removeItem(id: any) {
    this.deleteID = id;
    this.deleteRecordModal?.show();
  }

  confirmDelete() {
    if (this.deleteID) {
      this.examService.deleteVocab(this.deleteID).subscribe({
        next: (res) => {
          if (res && res.retCode === 0) {
            this.loadVocabs();
          }
          this.deleteRecordModal?.hide();
        },
        error: () => { this.deleteRecordModal?.hide(); }
      });
    }
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
