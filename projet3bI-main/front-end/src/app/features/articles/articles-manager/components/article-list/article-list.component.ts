import {ChangeDetectorRef, Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {AsyncPipe, DatePipe, NgForOf, SlicePipe} from '@angular/common';
import {Article} from '../../models/article';
import {
  CreateTransactionComponent
} from '../../../../transactions/transactions-manager/components/create-transaction/create-transaction.component';
import {
  TransactionsManagerComponent
} from '../../../../transactions/transactions-manager/transactions-manager.component';
import {ArticleService} from '../../services/article.service';
import {Router} from '@angular/router';
import {UserService} from '../../../../../core/user/user-manager/services/user.service';
import {BehaviorSubject, forkJoin, Observable} from 'rxjs';

export interface ArticleBought {
  article: Article,
  index: number;
}

@Component({
  selector: 'app-article-list',
  standalone: true,
  imports: [
    SlicePipe,
    NgForOf,
    TransactionsManagerComponent,
    CreateTransactionComponent,
    DatePipe,
    AsyncPipe,
  ],
  templateUrl: './article-list.component.html',
  styleUrl: './article-list.component.css'
})
export class ArticleListComponent{
  @Input()
  set articles(value: Article[]) {
    this._articles = value;
    this.filteredArticles = [...this._articles];
  }

  get articles(): Article[] {
    return this._articles;
  }





  private _articles: Article[] = [];
  filteredArticles: Article[] = [];
  public usernames: { [userId: number]: BehaviorSubject<String> } = {};

  public categories = ['Electronics', 'Books', 'Clothing', 'Furnishing', 'Toys', 'Sports', 'Beauty', 'Food', 'Vehicles', 'Other'];

  constructor(private _articleService: ArticleService, private _userService: UserService, private router: Router, private cdr: ChangeDetectorRef) {}

  @Output() articleBought: EventEmitter<ArticleBought> = new EventEmitter();
  @Output() wantedArticle = new EventEmitter<{ article: Article; index: number }>();

  itemsPerPage = 16;
  itemsPerRow = 4;
  currentPage = 1;


  getUsername(userId: number): Observable<String> {
    if (!this.usernames[userId]) {
      this.usernames[userId] = new BehaviorSubject<String>('Loading...');
      this._userService.getUsername(userId).subscribe({
        next: (username) => {
          this.usernames[userId].next(username);
        },
        error: (err) => {
          console.error(`Failed to load username for userId=${userId}`, err);
          this.usernames[userId].next('Unknown User');
        }
      });
    }
    return this.usernames[userId].asObservable();
  }



  get paginatedItems() {
    const startIndex = (this.currentPage - 1) * this.itemsPerPage;
    const endIndex = startIndex + this.itemsPerPage;
    return this.filteredArticles.slice(startIndex, endIndex);
  }

  get totalPages() {
    return Math.ceil(this.filteredArticles.length / this.itemsPerPage);
  }

  goToPage(page: number) {
    this.currentPage = page;
  }

  chunkedPaginatedItems() {
    const paginatedItems = this.paginatedItems; // elements of the active page
    const chunkedItems = []; // array that will contain every row

    for (let i = 0; i < paginatedItems.length; i += this.itemsPerRow) {
      chunkedItems.push(paginatedItems.slice(i, i + this.itemsPerRow)); // Add a line of 4 elements
    }

    return chunkedItems;
  }

  emitArticleTransaction(article: Article, index: number) {
    if (this._userService.getUserIdFromToken()==null)
    {
      if(confirm('You are not connected. Do you want to login ?')){
        this.router.navigate(['/login']);
        return
      }
    }else {
      if (confirm('Are you sure you want to buy this article?')) {
        this.articleBought.emit({ article: article, index: index });
      }
    }

  }

  getImageUrl(mainImageUrl: string): string {
    const BASE_URL = 'http://localhost:5185/';
    if (mainImageUrl.startsWith('string')) {
      return 'http://localhost:4200/test.png';
    }
    return `${BASE_URL}${mainImageUrl}`;
  }


  selectArticle(article: Article) {
    this._articleService.setSelectedArticle(article);
  }

  emitArticleTrade(article: Article, index: number) {
    this.wantedArticle.emit({ article: article, index: index });
  }

  navigateToTrade(item: Article) {
    if (this._userService.getUserIdFromToken()==null)
    {
      if(confirm('You are not connected. Do you want to login ?')){
        this.router.navigate(['/login']);
      }
    }else {
      this.router.navigate(['/trade', item.articleId]);
    }
  }

  filterByCategory(event: Event) {
    const selectElement = event.target as HTMLSelectElement;
    const category = selectElement.value;

    this.currentPage = 1; // Return to the first page after a filter change

    if (category) {
      this.filteredArticles = this.articles.filter(article => article.category === category);
    } else {
      this.filteredArticles = [...this.articles]; // Reset list with all items
    }
  }


}



