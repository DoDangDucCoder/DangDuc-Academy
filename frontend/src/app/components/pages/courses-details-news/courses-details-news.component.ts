import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ArticleService } from 'src/app/core/services/article.service';
import { ArticleModel } from 'src/app/models/models/article';

@Component({
  selector: 'app-courses-details-news',
  templateUrl: './courses-details-news.component.html',
  styleUrls: ['./courses-details-news.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class BlogDetailsStyleTwoComponent implements OnInit {

  article: ArticleModel | null = null;
  slug: string | null = null;
  isLoading = true;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private articleService: ArticleService
  ) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.slug = params.get('slug');
      if (this.slug) {
        this.loadArticle();
      }
    });
  }

  loadArticle(): void {
    this.isLoading = true;
    this.articleService.getArticleBySlug(this.slug!).subscribe({
      next: (res) => {
        if (res.retCode === 0 && res.data) {
          this.article = res.data;
        } else {
          this.router.navigate(['/404']);
        }
        this.isLoading = false;
      },
      error: () => {
        this.router.navigate(['/404']);
        this.isLoading = false;
      }
    });
  }
}
