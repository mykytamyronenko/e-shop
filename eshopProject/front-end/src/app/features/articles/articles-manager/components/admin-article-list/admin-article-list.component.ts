import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {Article} from '../../models/article';
import {ArticleService} from '../../services/article.service';
import {UserService} from '../../../../../core/user/user-manager/services/user.service';
import {ArticleBought} from '../article-list/article-list.component';
import {DatePipe, NgForOf} from '@angular/common';

@Component({
  selector: 'app-admin-article-list',
  standalone: true,
  imports: [
    DatePipe,
    NgForOf
  ],
  templateUrl: './admin-article-list.component.html',
  styleUrl: './admin-article-list.component.css'
})
export class AdminArticleListComponent implements OnInit{
  @Input()
  articles: Article[] = [];

  userId: string | null = null;


  constructor(private _articleService: ArticleService, private _userService: UserService) {}

  ngOnInit() {
    this.userId = this._userService.getUserIdFromToken();

    if (!this.userId) {
      console.error('User not authenticated');
      return;
    }

    this._articleService.getAll().subscribe(Articles => {console.log(Articles);
      this.articles = Articles.filter(article => article.status !== 'removed');
    });
  }

  @Output()
  articleBought: EventEmitter<ArticleBought> = new EventEmitter();

  itemsPerPage = 16;
  itemsPerRow = 4;
  currentPage = 1;

  //this function calculate elements to show on the active page
  get paginatedItems() {
    const startIndex = (this.currentPage - 1) * this.itemsPerPage;
    const endIndex = startIndex + this.itemsPerPage;
    return this.articles.slice(startIndex, endIndex);
  }

  //count the number of pages
  get totalPages() {
    return Math.ceil(this.articles.length / this.itemsPerPage);
  }

  //change between page
  goToPage(page: number) {
    this.currentPage = page;
  }
  chunkedPaginatedItems() {
    const paginatedItems = this.paginatedItems; // elements of the active page
    const chunkedItems = []; // array that will contain every rows

    for (let i = 0; i < paginatedItems.length; i += this.itemsPerRow) {
      chunkedItems.push(paginatedItems.slice(i, i + this.itemsPerRow)); // adding a line of 4 elements
    }

    return chunkedItems;
  }

  getImageUrl(mainImageUrl: string): string {
    const BASE_URL = 'http://localhost:5185/';

    if(mainImageUrl.startsWith('string')){
      return 'http://localhost:4200/test.png';
    }

    return `${BASE_URL}${mainImageUrl}`;
  }
  removeArticle(index: number) {
    const articleToRemove = this.articles[index];

    const userConfirmed = confirm(`Are you sure you want to remove the article "${articleToRemove.title}"?`);
    if (!userConfirmed) {
      return;
    }

    this._articleService.updateArticleStatusAdmin(articleToRemove.articleId, 'removed').subscribe({
      next: () => {
        console.log(`Article ${articleToRemove.articleId} removed`);

        this.articles = this.articles.filter(a => a.articleId !== articleToRemove.articleId);

        alert(`Your article has been successfully removed.`);
      },
      error: (error) => {
        console.error(`Failed to remove article ${articleToRemove.articleId}`, error);

        alert('An error occurred while removing the article. Please try again later.');
      }
    });
  }
}
