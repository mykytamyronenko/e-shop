import {Component, EventEmitter, inject, Output, OnInit, OnDestroy} from '@angular/core';
import {FormBuilder, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {Transaction} from '../../models/transaction';
import {UserService} from '../../../../../core/user/user-manager/services/user.service';
import {Article} from '../../../../articles/articles-manager/models/article';
import {ArticleService} from '../../../../articles/articles-manager/services/article.service';
import {NgForOf, NgIf} from '@angular/common';
import {firstValueFrom, of, switchMap} from 'rxjs';

@Component({
  selector: 'app-create-transaction',
  standalone: true,
  templateUrl: './create-transaction.component.html',
  imports: [
    NgIf,
    ReactiveFormsModule,
    NgForOf
  ],
  styleUrls: ['./create-transaction.component.css']
})
export class CreateTransactionComponent {

  private _fb: FormBuilder = inject(FormBuilder);
  private _userService: UserService = inject(UserService);
  private _articleService: ArticleService = inject(ArticleService);

  @Output() transactionCreated: EventEmitter<Transaction> = new EventEmitter();

  public selectedArticle: Article | null = null;
  public sellerId?: number;
  public articleId?: number;
  public price?: number;

  form: FormGroup = this._fb.group({
    buyerId: [''],
    sellerId: [''],
    articleId: [''],
    transactionType: [''],
    price: [''],
    commission: [''],
    transactionDate: [''],
    status: ['']
  });

  async loadSelectedArticle() {
    try {
      this.selectedArticle = await firstValueFrom(this._articleService.getSelectedArticle());
      if (this.selectedArticle) {
        this.sellerId = this.selectedArticle.userId;
        this.articleId = this.selectedArticle.articleId;
        this.price = this.selectedArticle.price;
        console.log('Article loaded : ', this.selectedArticle);
      }
    } catch (error) {
      console.error('Error loading article:', error);
    }
  }

  sendData() {
    console.log("Transaction created (create):", this.form.value);
  }

  emitTransaction() {
    const buyerId = this._userService.getUserIdFromToken();

    if (!buyerId) {
      console.error('User not authenticated');
      return;
    }

    if (this.selectedArticle) {
      const dateRightFormat = new Date();

      const transaction: Transaction = {
        transactionId: 0,
        buyerId: parseInt(buyerId, 10),
        sellerId: this.selectedArticle.userId,
        articleId: this.selectedArticle.articleId,
        transactionType: 'purchase',
        price: this.selectedArticle.price,
        commission: this.form.value.commission,
        transactionDate: dateRightFormat.toISOString(),
        status: 'finished'
      };

      this.transactionCreated.emit(transaction);
    } else {
      console.warn('No items selected');
    }
  }

  async submitTransaction() {
    const buyerId = this._userService.getUserIdFromToken();

    if (!buyerId) {
      console.error('User not authenticated');
      return;
    }

    if (!this.selectedArticle) {
      await this.loadSelectedArticle();
    }

    if (this.selectedArticle) {
      this.emitTransaction();
    }
  }

}
