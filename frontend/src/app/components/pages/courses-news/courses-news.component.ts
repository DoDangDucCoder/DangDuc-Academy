import { Component, OnInit } from '@angular/core';
import { ArticleService } from 'src/app/core/services/article.service';
import { ArticleModel } from 'src/app/models/models/article';

@Component({
  selector: 'app-courses-news',
  templateUrl: './courses-news.component.html',
  styleUrls: ['./courses-news.component.scss']
})
export class CoursesNewsComponent implements OnInit {

  articles: ArticleModel[] = [];
  currentPage = 1;
  pageSize = 9;
  totalItems = 0;
  Math = Math;

  constructor(private articleService: ArticleService) { }

  ngOnInit(): void {
    this.loadArticles();
  }

  loadArticles(): void {
    this.articleService.getPagedArticles(this.currentPage, this.pageSize).subscribe({
      next: (res) => {
        if (res.retCode === 0 && res.data) {
          this.articles = res.data.results;
          this.totalItems = res.data.rowCount;
        }
      }
    });
  }

  onPageChange(page: number): void {
    if (page < 1 || page > this.totalPages()) return;
    this.currentPage = page;
    this.loadArticles();
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  totalPages(): number {
    return Math.ceil(this.totalItems / this.pageSize);
  }

  getPages(): number[] {
    const pages = [];
    for (let i = 1; i <= this.totalPages(); i++) {
      pages.push(i);
    }
    return pages;
  }
}
