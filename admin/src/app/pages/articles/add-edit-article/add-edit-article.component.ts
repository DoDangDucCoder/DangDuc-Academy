import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ArticleService } from 'src/app/core/services/article.service';
import Swal from 'sweetalert2';
import * as ClassicEditor from '@ckeditor/ckeditor5-build-classic';

@Component({
  selector: 'app-add-edit-article',
  templateUrl: './add-edit-article.component.html',
  styleUrls: ['./add-edit-article.component.scss']
})
export class AddEditArticleComponent implements OnInit {

  articleForm!: FormGroup;
  isEditMode = false;
  articleId!: number;
  submitted = false;

  public Editor: any = ClassicEditor;

  constructor(
    private formBuilder: FormBuilder,
    private articleService: ArticleService,
    private route: ActivatedRoute,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.articleForm = this.formBuilder.group({
      title: ['', [Validators.required]],
      description: ['', [Validators.required]],
      content: ['', [Validators.required]],
      thumbnailUrl: ['']
    });

    this.route.params.subscribe(params => {
      if (params['id']) {
        this.isEditMode = true;
        this.articleId = +params['id'];
        this.loadArticle();
      }
    });
  }

  get f() { return this.articleForm.controls; }

  loadArticle() {
    this.articleService.getArticleById(this.articleId).subscribe({
      next: (res) => {
        if (res.partnerCode === '000' && res.data) {
          this.articleForm.patchValue({
            title: res.data.title,
            description: res.data.description,
            content: res.data.content,
            thumbnailUrl: res.data.thumbnailUrl
          });
        }
      }
    });
  }

  onSubmit() {
    this.submitted = true;

    if (this.articleForm.invalid) {
      return;
    }

    const articleData = this.articleForm.value;

    if (this.isEditMode) {
      this.articleService.updateArticle(this.articleId, articleData).subscribe({
        next: (res) => {
          if (res.partnerCode === '000') {
            Swal.fire('Success', 'Article updated successfully', 'success');
            this.router.navigate(['/articles']);
          }
        }
      });
    } else {
      this.articleService.createArticle(articleData).subscribe({
        next: (res) => {
          if (res.partnerCode === '000') {
            Swal.fire('Success', 'Article created successfully', 'success');
            this.router.navigate(['/articles']);
          }
        }
      });
    }
  }
}
