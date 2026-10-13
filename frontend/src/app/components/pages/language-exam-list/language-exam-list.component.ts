import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { LanguageExam, LanguageLearningService, VocabStats } from 'src/app/core/services/language-learning.service';

@Component({
  selector: 'app-language-exam-list',
  templateUrl: './language-exam-list.component.html',
  styleUrls: ['./language-exam-list.component.scss']
})
export class LanguageExamListComponent implements OnInit {
  language = 'en';
  selectedType = '';
  selectedLevel = '';
  page = 1;
  pageSize = 12;
  loading = false;

  exams: LanguageExam[] = [];
  vocabStats: VocabStats | null = null;

  languageMeta: Record<string, { name: string; flag: string; color: string; types: string[]; levels: Record<string, string[]> }> = {
    en: { name: 'Tiếng Anh', flag: '🇺🇸', color: '#1565c0',
          types: ['TOEIC', 'IELTS', 'TOEFL'],
          levels: { TOEIC: ['Full','Part1-2','Part5-7'], IELTS: ['Band5','Band6','Band7'], TOEFL: [] } },
    zh: { name: 'Tiếng Trung', flag: '🇨🇳', color: '#b71c1c',
          types: ['HSK'],
          levels: { HSK: ['HSK1','HSK2','HSK3','HSK4','HSK5','HSK6'] } },
    ja: { name: 'Tiếng Nhật', flag: '🇯🇵', color: '#880e4f',
          types: ['JLPT'],
          levels: { JLPT: ['N5','N4','N3','N2','N1'] } },
    ko: { name: 'Tiếng Hàn', flag: '🇰🇷', color: '#1a237e',
          types: ['TOPIK'],
          levels: { TOPIK: ['TOPIK-I','TOPIK-II'] } },
  };

  get meta() { return this.languageMeta[this.language] ?? this.languageMeta['en']; }
  get levels() { return this.selectedType ? (this.meta.levels[this.selectedType] ?? []) : []; }

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private langService: LanguageLearningService
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe(p => {
      this.language = p['lang'] || 'en';
      this.loadExams();
      this.loadVocabStats();
    });
  }

  loadExams(): void {
    this.loading = true;
    this.langService.getExamList(this.language, this.selectedType || undefined, this.selectedLevel || undefined, this.page, this.pageSize)
      .subscribe({ next: r => { this.exams = r.data ?? []; this.loading = false; }, error: () => this.loading = false });
  }

  loadVocabStats(): void {
    // Chỉ gọi khi đã đăng nhập (endpoint yêu cầu [Authorize])
    const token = localStorage.getItem('access_token') || localStorage.getItem('token') || sessionStorage.getItem('access_token');
    if (!token) return;
    this.langService.getVocabStats(this.language)
      .subscribe({ next: r => this.vocabStats = r.data ?? null, error: () => {} });
  }

  onTypeChange(): void { this.selectedLevel = ''; this.page = 1; this.loadExams(); }
  onLevelChange(): void { this.page = 1; this.loadExams(); }

  goToExam(examId: number): void {
    this.router.navigate(['/language', this.language, 'exam', examId]);
  }

  goToFlashcard(): void {
    this.router.navigate(['/language', this.language, 'flashcard']);
  }

  formatTime(mins: number): string {
    return mins >= 60 ? `${Math.floor(mins/60)}h ${mins%60}m` : `${mins} phút`;
  }

  getDifficultyColor(level: string | undefined): string {
    if (!level) return '#757575';
    const easy = ['N5','HSK1','HSK2','TOPIK-I','Part1-2','Band5'];
    const medium = ['N4','N3','HSK3','HSK4','Band6','Part5-7','TOPIK-II'];
    if (easy.some(l => level.includes(l))) return '#2e7d32';
    if (medium.some(l => level.includes(l))) return '#f57c00';
    return '#c62828';
  }
}
