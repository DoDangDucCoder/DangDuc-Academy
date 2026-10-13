import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { interval, Subscription } from 'rxjs';
import {
  LanguageLearningService, StartExamResponse,
  ExamPart, ExamQuestion, UserAnswer
} from 'src/app/core/services/language-learning.service';

@Component({
  selector: 'app-language-exam-do',
  templateUrl: './language-exam-do.component.html',
  styleUrls: ['./language-exam-do.component.scss']
})
export class LanguageExamDoComponent implements OnInit, OnDestroy {
  lang = 'en';
  examId = 0;
  attemptId = 0;

  examData: StartExamResponse | null = null;
  allQuestions: ExamQuestion[] = [];
  userAnswers: Map<number, number> = new Map();

  // ── Part Navigation ───────────────────────────────────────
  selectedPartIndex = 0;
  get parts(): ExamPart[] { return this.examData?.exam?.parts ?? []; }
  get currentPart(): ExamPart | null { return this.parts[this.selectedPartIndex] ?? null; }
  get partQuestions(): ExamQuestion[] { return this.currentPart?.questions ?? []; }

  /** Global index của câu đầu tiên của part hiện tại */
  get partStartIndex(): number {
    return this.parts
      .slice(0, this.selectedPartIndex)
      .reduce((sum, p) => sum + p.questions.length, 0);
  }

  /** Chỉ số câu trong part: local index → global index */
  localToGlobal(localIdx: number): number {
    return this.partStartIndex + localIdx;
  }

  /** Global index → Part index + Local index */
  globalToLocal(globalIdx: number): { partIdx: number; localIdx: number } {
    let remaining = globalIdx;
    for (let i = 0; i < this.parts.length; i++) {
      if (remaining < this.parts[i].questions.length)
        return { partIdx: i, localIdx: remaining };
      remaining -= this.parts[i].questions.length;
    }
    return { partIdx: 0, localIdx: 0 };
  }

  currentIndex = 0;       // global index
  timeLeft = 0;
  timerSub?: Subscription;
  isSubmitting = false;
  loading = true;

  // Audio
  isPlaying = false;
  audioProgress = 0;

  get currentQuestion(): ExamQuestion | null {
    return this.allQuestions[this.currentIndex] ?? null;
  }

  get progress(): number {
    return this.allQuestions.length
      ? Math.round((this.answeredCount / this.allQuestions.length) * 100) : 0;
  }

  get answeredCount(): number { return this.userAnswers.size; }

  get timeDisplay(): string {
    const h = Math.floor(this.timeLeft / 3600);
    const m = Math.floor((this.timeLeft % 3600) / 60);
    const s = this.timeLeft % 60;
    return h > 0
      ? `${h}:${m.toString().padStart(2,'0')}:${s.toString().padStart(2,'0')}`
      : `${m.toString().padStart(2,'0')}:${s.toString().padStart(2,'0')}`;
  }

  get timeWarning(): boolean { return this.timeLeft <= 300; }

  // ── Part answered stats ───────────────────────────────────
  partAnsweredCount(partIdx: number): number {
    return (this.parts[partIdx]?.questions ?? [])
      .filter(q => this.userAnswers.has(q.id)).length;
  }

  // ── LocalStorage Draft ────────────────────────────────────
  private draftKey(): string { return `lang_exam_draft_${this.examId}`; }

  private saveDraft(): void {
    const draft = {
      examId: this.examId,
      attemptId: this.attemptId,
      answers: [...this.userAnswers.entries()],
      timeLeft: this.timeLeft
    };
    try { localStorage.setItem(this.draftKey(), JSON.stringify(draft)); } catch {}
  }

  private loadDraft(): { examId: number; attemptId: number; answers: [number, number][]; timeLeft: number } | null {
    try {
      const raw = localStorage.getItem(this.draftKey());
      return raw ? JSON.parse(raw) : null;
    } catch { return null; }
  }

  private clearDraft(): void {
    try { localStorage.removeItem(this.draftKey()); } catch {}
  }

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private langService: LanguageLearningService
  ) {}

  ngOnInit(): void {
    this.lang   = this.route.snapshot.params['lang'];
    this.examId = +this.route.snapshot.params['examId'];

    // Kiểm tra draft trước — nếu có thì hỏi resume
    const draft = this.loadDraft();
    if (draft?.examId === this.examId && draft.answers.length > 0) {
      const resume = confirm(
        `Bạn có bài làm dở (${draft.answers.length} câu đã trả lời).\n\nTiếp tục bài cũ?`
      );
      if (resume) {
        this.startExam(draft);
        return;
      } else {
        this.clearDraft();
      }
    }
    this.startExam();
  }

  startExam(draft?: { attemptId: number; answers: [number, number][]; timeLeft: number } | null): void {
    // Nếu có draft với cùng examId → dùng lại attemptId, không tạo mới
    if (draft) {
      // Thử resume attempt cũ: startExam tạo mới nhưng restore answers
      this.langService.startExam(this.examId).subscribe({
        next: r => {
          this.examData  = r.data!;
          this.attemptId = r.data!.attemptId;
          this.timeLeft  = r.data!.durationSeconds;
          this.allQuestions = r.data!.exam.parts.flatMap(p => p.questions);
          // Restore answers từ draft
          draft.answers.forEach(([qId, aId]) => this.userAnswers.set(qId, aId));
          this.loading = false;
          this.saveDraft(); // overwrite với attemptId mới
          this.startTimer();
        },
        error: () => this.router.navigate(['/language', this.lang])
      });
    } else {
      this.langService.startExam(this.examId).subscribe({
        next: r => {
          this.examData  = r.data!;
          this.attemptId = r.data!.attemptId;
          this.timeLeft  = r.data!.durationSeconds;
          this.allQuestions = r.data!.exam.parts.flatMap(p => p.questions);
          this.loading = false;
          this.saveDraft();
          this.startTimer();
        },
        error: () => this.router.navigate(['/language', this.lang])
      });
    }
  }

  startTimer(): void {
    this.timerSub = interval(1000).subscribe(() => {
      this.timeLeft--;
      if (this.timeLeft % 30 === 0) this.saveDraft(); // auto-save mỗi 30s
      if (this.timeLeft <= 0) this.submitExam(true);
    });
  }

  selectAnswer(questionId: number, answerId: number): void {
    this.userAnswers.set(questionId, answerId);
    this.saveDraft(); // save ngay khi chọn đáp án
  }

  getSelected(questionId: number): number {
    return this.userAnswers.get(questionId) ?? 0;
  }

  // ── Navigation (global index) ─────────────────────────────
  goTo(index: number): void {
    if (index >= 0 && index < this.allQuestions.length) {
      this.currentIndex = index;
      // Tự động chuyển Part tab khi click câu ở part khác
      const { partIdx } = this.globalToLocal(index);
      this.selectedPartIndex = partIdx;
    }
  }

  selectPart(partIdx: number): void {
    this.selectedPartIndex = partIdx;
    // Nhảy đến câu đầu tiên của part đó
    const globalStart = this.localToGlobal(0);
    // Tính lại từ đầu
    let g = 0;
    for (let i = 0; i < partIdx; i++) g += this.parts[i].questions.length;
    this.currentIndex = g;
  }

  isAnswered(qId: number): boolean { return this.userAnswers.has(qId); }

  submitExam(expired = false): void {
    if (this.isSubmitting) return;
    if (!expired && this.answeredCount < this.allQuestions.length) {
      const unanswered = this.allQuestions.length - this.answeredCount;
      if (!confirm(`Còn ${unanswered} câu chưa trả lời. Bạn vẫn muốn nộp bài?`)) return;
    }
    this.isSubmitting = true;
    this.timerSub?.unsubscribe();

    const answers: UserAnswer[] = Array.from(this.userAnswers.entries())
      .map(([questionId, selectedAnswerId]) => ({ questionId, selectedAnswerId }));

    this.langService.submitExam(this.attemptId, answers).subscribe({
      next: r => {
        this.clearDraft(); // Xóa draft khi nộp bài thành công
        this.router.navigate(['/language', this.lang, 'result', r.data!.attemptId]);
      },
      error: () => { this.isSubmitting = false; }
    });
  }

  // ── Audio ─────────────────────────────────────────────────
  toggleAudio(audio: HTMLAudioElement): void {
    if (audio.paused) {
      audio.play();
      this.isPlaying = true;
      audio.ontimeupdate = () => {
        this.audioProgress = audio.duration ? (audio.currentTime / audio.duration) * 100 : 0;
      };
      audio.onended = () => { this.isPlaying = false; this.audioProgress = 0; };
    } else {
      audio.pause();
      this.isPlaying = false;
    }
  }

  ngOnDestroy(): void { this.timerSub?.unsubscribe(); }
}
