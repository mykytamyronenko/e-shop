import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {Article} from '../../models/article';
import {ArticleService} from '../../services/article.service';
import {UserService} from '../../../../../core/user/user-manager/services/user.service';
import {TransactionService} from '../../../../transactions/transactions-manager/services/transaction.service';
import {ArticleBought} from '../article-list/article-list.component';
import {Transaction} from '../../../../transactions/transactions-manager/models/transaction';
import {DatePipe, NgForOf} from '@angular/common';
import {
  TransactionsManagerComponent
} from '../../../../transactions/transactions-manager/transactions-manager.component';
import {AdminUser} from '../../../../../core/user/user-manager/models/user';
import {ActivatedRoute, Router} from '@angular/router';

export interface ArticleDeleted {
  article: Article;
  index: number;
}

@Component({
  selector: 'app-owned-by-user-article-list',
  standalone: true,
  imports: [
    NgForOf,
    TransactionsManagerComponent,
    DatePipe
  ],
  templateUrl: './owned-by-user-article-list.component.html',
  styleUrl: './owned-by-user-article-list.component.css'
})
export class OwnedByUserArticleListComponent implements OnInit {
  @Input()
  ownedArticles: Article[] = [];

  @Output()
  articleDeleted: EventEmitter<ArticleDeleted> = new EventEmitter();

  userId: string | null = null;


  constructor(private _articleService: ArticleService, private _userService: UserService,private _route: Router) {}

  ngOnInit() {
    this.userId = this._userService.getUserIdFromToken();

    if (!this.userId) {
      console.error('User not authenticated');
      this._articleService.getAll().subscribe(OwnedArticles => {console.log(OwnedArticles);
        this.ownedArticles = OwnedArticles;});
      return;
    }

    const numericUserId = parseInt(this.userId, 10);

    this._articleService.getAll().subscribe(OwnedArticles => {console.log(OwnedArticles);
      this.ownedArticles = OwnedArticles.filter(article => article.userId === numericUserId
      ).filter(article => article.status !== 'removed').filter(article => article.status !== 'sold');
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
    return this.ownedArticles.slice(startIndex, endIndex);
  }

  //count the number of pages
  get totalPages() {
    return Math.ceil(this.ownedArticles.length / this.itemsPerPage);
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

  navigateToModifyArticle(item: Article) {
    this._route.navigate(['/editArticle', item.articleId]);
  }

  removeArticle(index: number) {
    const articleToRemove = this.ownedArticles[index];

    const userConfirmed = confirm(`Are you sure you want to remove the article "${articleToRemove.title}"?`);
    if (!userConfirmed) {
      return;
    }

    this._articleService.updateArticleStatus(articleToRemove.articleId, 'removed').subscribe({
      next: () => {
        console.log(`Article ${articleToRemove.articleId} removed`);

        this.ownedArticles = this.ownedArticles.filter(a => a.articleId !== articleToRemove.articleId);

        alert(`Your article has been successfully removed.`);
      },
      error: (error) => {
        console.error(`Failed to remove article ${articleToRemove.articleId}`, error);

        alert('An error occurred while removing the article. Please try again later.');
      }
    });
  }

}
