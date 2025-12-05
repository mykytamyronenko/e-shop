import {Component, Input, OnInit} from '@angular/core';
import {Article} from '../articles-manager/models/article';
import {ArticleService} from '../articles-manager/services/article.service';
import {UserService} from '../../../core/user/user-manager/services/user.service';
import {NgClass, NgForOf, NgIf} from '@angular/common';
import {ActivatedRoute, Router} from '@angular/router';
import {Parser} from '@angular/compiler';
import {TradeService} from './services/trade.service';

@Component({
  selector: 'app-trade',
  standalone: true,
  imports: [
    NgForOf,
    NgClass,
    NgIf
  ],
  templateUrl: './trade.component.html',
  styleUrl: './trade.component.css'
})
export class TradeComponent implements OnInit{
  ownedArticles: Article[] = [];
  userId: string | null = null;
  isActiveItem: boolean[][] = [];
  selectedOwnArticles: Article[] = [];

  constructor(private _articleService: ArticleService,private _userService: UserService,private route: ActivatedRoute,private _tradeService: TradeService,private router: Router) {
  }

  wantedArticle: Article | undefined;
  onArticleSelected(event: { article: Article; index: number }) {
    this.wantedArticle = event.article;
    console.log('Selected Article:', this.wantedArticle);
  }

  ngOnInit() {

    this.userId = this._userService.getUserIdFromToken();

    if(this.userId) {
      const numericUserId = parseInt(this.userId, 10);
      this._articleService.getAll().subscribe(OwnedArticles => {console.log(OwnedArticles);
        this.ownedArticles = OwnedArticles.filter(article => article.userId === numericUserId)
          .filter(article => article.status !== 'removed')
          .filter(article => article.status !== 'sold');

        this.initializeActiveItems();
      });
    }
    const articleId = this.route.snapshot.paramMap.get('articleId');
    if(articleId){
      this._articleService.getArticleById(parseInt(articleId));

      this._articleService.getArticleById(+articleId).subscribe(article => {
        this.wantedArticle = article;
      });
    }




  }

  itemsPerPage = 1600;
  itemsPerRow = 2;
  currentPage = 1;

  initializeActiveItems() {
    this.isActiveItem = this.chunkedPaginatedItems().map(row => row.map(() => false));
  }

  //this function calculate elements to show on the active page
  get paginatedItems() {
    const startIndex = (this.currentPage - 1) * this.itemsPerPage;
    const endIndex = startIndex + this.itemsPerPage;
    return this.ownedArticles.slice(startIndex, endIndex);
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

  isActive(rowIndex: number, colIndex: number): boolean {
    return this.isActiveItem[rowIndex] && this.isActiveItem[rowIndex][colIndex];
  }

  toggleBorder(rowIndex: number, colIndex: number) {
    this.isActiveItem[rowIndex][colIndex] = !this.isActiveItem[rowIndex][colIndex];

    const article = this.chunkedPaginatedItems()[rowIndex][colIndex];


    if (this.isActiveItem[rowIndex][colIndex]) {
      this.addArticleToSelection(article);
    } else {
      this.removeArticleFromSelection(article);
    }

    console.log(this.selectedOwnArticles);

  }

  addArticleToSelection(article: Article) {
    if (!this.selectedOwnArticles.includes(article)) {
      this.selectedOwnArticles.push(article);
    }
  }

  removeArticleFromSelection(article: Article) {
    this.selectedOwnArticles = this.selectedOwnArticles.filter(a => a !== article);
  }

  validateTrade() {
    if (this.wantedArticle) {
      if (this.selectedOwnArticles.length === 0) {
        alert('Please select at least one of your own articles to trade.');
        return;
      }

      if (confirm(`Are you sure you want to trade for ${this.wantedArticle.title}?`)) {
        const dateRightFormat = new Date();
        const articleIdsArray = this.selectedOwnArticles.map(article => article.articleId);
        const traderArticlesIds = articleIdsArray.join(',');


        if(this.userId){
        const tradeData = {
          tradeId:0,
          traderId: parseInt(this.userId, 10),
          receiverId: this.wantedArticle.userId,
          // @ts-ignore
          traderArticlesIds: traderArticlesIds,
          receiverArticleId: this.wantedArticle.articleId,
          tradeDate: dateRightFormat.toISOString(),
          status:"in progress"
        };


        this._tradeService.createTrade(tradeData).subscribe({
          next: (response) => {
            alert('Trade successfully created!');
            console.log('Trade response:', response);
          },
          error: (error) => {
            alert('Failed to create trade.');
            console.error('Trade creation error:', error);
          }
        });
        }
        this.router.navigate(['/hub']);
      }
    } else {
      alert('No article selected for trade.');
    }
  }
}
