import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { LeaderboardEntry, LanguageExam, LanguageLearningService } from 'src/app/core/services/language-learning.service';

@Component({
  selector: 'app-language-leaderboard',
  templateUrl: './language-leaderboard.component.html',
  styleUrls: ['./language-leaderboard.component.scss']
})
export class LanguageLeaderboardComponent implements OnInit {
  lang = 'en';
  selectedExamId = 0;
  exams: LanguageExam[] = [];
  leaderboard: LeaderboardEntry[] = [];
  loading = false;

  languageMeta: Record<string, { name: string; flag: string; color: string }> = {
    en: { name: 'Tiếng Anh',  flag: '🇺🇸', color: '#1565c0' },
    zh: { name: 'Tiếng Trung', flag: '🇨🇳', color: '#b71c1c' },
    ja: { name: 'Tiếng Nhật',  flag: '🇯🇵', color: '#880e4f' },
    ko: { name: 'Tiếng Hàn',  flag: '🇰🇷', color: '#1a237e' },
  };

  get meta() { return this.languageMeta[this.lang] ?? this.languageMeta['en']; }
  get selectedExam() { return this.exams.find(e => e.id === this.selectedExamId); }

  getRankIcon(rank: number): string {
    if (rank === 1) return '🥇';
    if (rank === 2) return '🥈';
    if (rank === 3) return '🥉';
    return `#${rank}`;
  }

  formatTime(s: number): string {
    const m = Math.floor(s / 60);
    const sec = s % 60;
    return `${m}p ${sec.toString().padStart(2,'0')}s`;
  }

  constructor(private route: ActivatedRoute, private langService: LanguageLearningService) {}

  ngOnInit(): void {
    this.lang = this.route.snapshot.params['lang'] || 'en';
    this.langService.getExamList(this.lang, undefined, undefined, 1, 50)
      .subscribe({ next: r => {
        this.exams = r.data ?? [];
        if (this.exams.length > 0) this.loadLeaderboard(this.exams[0].id);
      }});
  }

  loadLeaderboard(examId: number): void {
    this.selectedExamId = examId;
    this.loading = true;
    this.langService.getLeaderboard(examId, 20)
      .subscribe({ next: r => { this.leaderboard = r.data ?? []; this.loading = false; },
                   error: () => this.loading = false });
  }
}
