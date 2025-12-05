import {Component, OnInit} from '@angular/core';
import {ArticleBought, ArticleListComponent} from './components/article-list/article-list.component';
import {Article} from './models/article';
import {ArticleService} from './services/article.service';
import {UserService} from '../../../core/user/user-manager/services/user.service';
import {Transaction} from '../../transactions/transactions-manager/models/transaction';
import {TransactionService} from '../../transactions/transactions-manager/services/transaction.service';
import {
  ArticleDeleted,
  OwnedByUserArticleListComponent
} from './components/owned-by-user-article-list/owned-by-user-article-list.component';
import {TradeComponent} from '../trade/trade.component';
import {UserListComponent} from '../../../core/user/user-manager/components/user-list/user-list.component';

@Component({
  selector: 'app-articles-manager',
  standalone: true,
  imports: [
    ArticleListComponent,
    UserListComponent,
    OwnedByUserArticleListComponent,
    TradeComponent
  ],
  templateUrl: './articles-manager.component.html',
  styleUrl: './articles-manager.component.css'
})
export class ArticlesManagerComponent implements OnInit{
  articles: Article[] = [];
  article!: Article;
  userId: string | null = null;
  selectedArticle!: Article;


  constructor(private _articleService: ArticleService, private _userService: UserService, private _transactionService:TransactionService) {}

  ngOnInit(): void {
    this.userId = this._userService.getUserIdFromToken();

    if (!this.userId) {
      console.error('User not authenticated');
      this._articleService.getAll().subscribe(articles => {console.log(articles);
        this.articles = articles.filter(article => article.status !== "sold")
          .filter(article => article.status !== 'removed');});
      return;
    }

    const numericUserId = parseInt(this.userId, 10);

    this._articleService.getAll().subscribe(articles => {console.log(articles);
      this.articles = articles.filter(article => article.userId !== numericUserId)
        .filter(article => article.status !== 'sold')
        .filter(article => article.status !== 'removed');
    });
  }

  selectArticleForTransaction(articleBought: ArticleBought) {
    this.selectedArticle = articleBought.article;
    console.log('Article selected for transaction:', this.selectedArticle);

    // Create a transaction immediately
    this.createTransaction(this.selectedArticle);
  }

  createTransaction(article: Article) {
    const userId = this._userService.getUserIdFromToken();

    if (!userId) {
      console.error('User not authenticated');
      return;
    }

    const transaction: Transaction = {
      transactionId: 0,
      buyerId: parseInt(userId, 10),
      sellerId: article.userId,
      articleId: article.articleId,
      transactionType: 'purchase',
      price: article.price,
      commission: 2.0,
      transactionDate: new Date().toISOString(),
      status: 'finished',
    };

    this._transactionService.create(transaction).subscribe({
      next: (createdTransaction) => {
        console.log('Transaction successfully recorded :', createdTransaction);
        alert('Transaction successful!');
      },
      error: (err) => {
        console.error('Error creating transaction :', err);
        alert('You don\'t have enough balance for this purchase. Please add funds to complete your desired purchase!');
      }
    });

    console.log('Transaction created:', transaction);
  }
}
