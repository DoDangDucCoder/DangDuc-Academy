import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ExamResult, LanguageLearningService } from 'src/app/core/services/language-learning.service';

@Component({
  selector: 'app-language-exam-result',
  templateUrl: './language-exam-result.component.html',
  styleUrls: ['./language-exam-result.component.scss']
})
export class LanguageExamResultComponent implements OnInit {
  lang = 'en';
  result: ExamResult | null = null;
  loading = true;
  showReview = false;
  showCertificate = false;
  filterMode: 'all' | 'correct' | 'wrong' = 'all';

  get filteredResults() {
    if (!this.result) return [];
    if (this.filterMode === 'correct') return this.result.questionResults.filter(q => q.isCorrect);
    if (this.filterMode === 'wrong')   return this.result.questionResults.filter(q => !q.isCorrect);
    return this.result.questionResults;
  }

  get circleOffset(): number {
    const pct = this.result?.percentage ?? 0;
    const circumference = 2 * Math.PI * 54;
    return circumference - (pct / 100) * circumference;
  }

  get gradeLabel(): string {
    const pct = this.result?.percentage ?? 0;
    if (pct >= 90) return 'Xuất sắc 🏆';
    if (pct >= 75) return 'Giỏi ⭐';
    if (pct >= 60) return 'Khá 👍';
    if (pct >= 50) return 'Trung bình';
    return 'Chưa đạt 💪';
  }

  formatTime(s: number): string {
    const m = Math.floor(s / 60);
    const sec = s % 60;
    return `${m}p ${sec}s`;
  }

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private langService: LanguageLearningService
  ) {}

  ngOnInit(): void {
    this.lang = this.route.snapshot.params['lang'];
    const attemptId = +this.route.snapshot.params['attemptId'];
    this.langService.getResult(attemptId).subscribe({
      next: r => { this.result = r.data!; this.loading = false; },
      error: () => this.router.navigate(['/language', this.lang])
    });
  }

  retake(): void {
    const confirmed = confirm(
      `Bạn có muốn làm lại "${this.result?.examTitle}"?\n\nMột bài làm mới sẽ được tạo — điểm cũ vẫn được lưu trong lịch sử.`
    );
    if (confirmed) this.router.navigate(['/language', this.lang, 'exam', this.result?.examId]);
  }

  viewHistory(): void { this.router.navigate(['/language', this.lang, 'history']); }
  backToList(): void  { this.router.navigate(['/language', this.lang]); }
}
