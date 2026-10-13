import { Component, OnInit } from '@angular/core';
import { ArticleService } from 'src/app/core/services/article.service';
import { ArticleModel } from 'src/app/models/models/article';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-list-articles',
  templateUrl: './list-articles.component.html',
  styleUrls: ['./list-articles.component.scss']
})
export class ListArticlesComponent implements OnInit {

  articles: ArticleModel[] = [];
  currentPage = 1;
  pageSize = 10;
  totalItems = 0;
  searchTerm = '';

  constructor(private articleService: ArticleService) { }

  ngOnInit(): void {
    this.loadArticles();
  }

  loadArticles() {
    this.articleService.getPagedArticles(this.currentPage, this.pageSize, this.searchTerm).subscribe({
      next: (res) => {
        if (res.partnerCode === '000') {
          this.articles = res.data?.results || [];
          this.totalItems = res.data?.rowCount || 0;
        }
      },
      error: (err) => {
        console.error(err);
      }
    });
  }

  onPageChange(event: any): void {
    this.currentPage = event.page;
    this.loadArticles();
  }

  onSearch(): void {
    this.currentPage = 1;
    this.loadArticles();
  }

  deleteArticle(id: number) {
    Swal.fire({
      title: 'Are you sure?',
      text: "You won't be able to revert this!",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#34c38f',
      cancelButtonColor: '#f46a6a',
      confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
      if (result.isConfirmed) {
        this.articleService.deleteArticle(id).subscribe({
          next: (res) => {
            if (res.partnerCode === '000') {
              Swal.fire('Deleted!', 'The article has been deleted.', 'success');
              this.loadArticles();
            }
          }
        });
      }
    });
  }
}
