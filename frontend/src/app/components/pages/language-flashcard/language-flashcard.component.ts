import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { VocabularyItem, VocabStats, LanguageLearningService } from 'src/app/core/services/language-learning.service';

type CardState = 'front' | 'back';
type Quality = 0 | 3 | 4 | 5;

@Component({
  selector: 'app-language-flashcard',
  templateUrl: './language-flashcard.component.html',
  styleUrls: ['./language-flashcard.component.scss']
})
export class LanguageFlashcardComponent implements OnInit {
  lang = 'en';
  cards: VocabularyItem[] = [];
  currentIndex = 0;
  cardState: CardState = 'front';
  stats: VocabStats | null = null;
  loading = true;
  sessionDone = false;
  sessionCorrect = 0;
  sessionTotal = 0;

  get current(): VocabularyItem | null { return this.cards[this.currentIndex] ?? null; }
  get isFlipped(): boolean { return this.cardState === 'back'; }

  constructor(private route: ActivatedRoute, private langService: LanguageLearningService) {}

  ngOnInit(): void {
    this.lang = this.route.snapshot.params['lang'];
    this.loadCards();
    this.loadStats();
  }

  loadCards(): void {
    this.langService.getReviewItems(this.lang, 20).subscribe({
      next: r => { this.cards = r.data ?? []; this.loading = false; },
      error: () => this.loading = false
    });
  }

  loadStats(): void {
    this.langService.getVocabStats(this.lang)
      .subscribe({ next: r => this.stats = r.data ?? null, error: () => {} });
  }

  flipCard(): void {
    this.cardState = this.cardState === 'front' ? 'back' : 'front';
  }

  rateCard(quality: Quality): void {
    if (!this.current) return;
    if (quality >= 4) this.sessionCorrect++;
    this.sessionTotal++;

    this.langService.updateProgress(this.current.id, quality).subscribe();

    this.cardState = 'front';
    if (this.currentIndex < this.cards.length - 1) {
      this.currentIndex++;
    } else {
      this.sessionDone = true;
      this.loadStats();
    }
  }

  restart(): void {
    this.currentIndex = 0;
    this.cardState = 'front';
    this.sessionDone = false;
    this.sessionCorrect = 0;
    this.sessionTotal = 0;
    this.loadCards();
  }

  playAudio(url: string): void {
    if (url) new Audio(url).play().catch(() => {});
  }
}
