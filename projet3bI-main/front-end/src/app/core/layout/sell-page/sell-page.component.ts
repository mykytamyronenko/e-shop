import { Component } from '@angular/core';
import {
    CreateArticleFormComponent
} from "../../../features/articles/articles-manager/components/create-article-form/create-article-form.component";
import {Article} from '../../../features/articles/articles-manager/models/article';
import {ArticleService} from '../../../features/articles/articles-manager/services/article.service';

@Component({
  selector: 'app-sell-page',
  standalone: true,
    imports: [
        CreateArticleFormComponent
    ],
  templateUrl: './sell-page.component.html',
  styleUrl: './sell-page.component.css'
})
export class SellPageComponent {
  articles: Article[] = [];

  constructor(private _articleService: ArticleService) {
  }
  createArticle({ article, imageFile }: { article: Article; imageFile?: File }) {
    this._articleService.createArticle(article, imageFile).subscribe({
      next: (art) => this.articles.push(art),
      error: (err) => console.error('Error:', err),
    });
  }
}
