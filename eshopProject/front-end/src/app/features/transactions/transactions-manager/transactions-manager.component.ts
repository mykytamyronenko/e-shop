import {Component, OnDestroy, OnInit} from '@angular/core';
import {Transaction} from './models/transaction';
import {TransactionService} from './services/transaction.service';
import {User} from '../../../core/user/user-manager/models/user';
import {CreateTransactionComponent} from './components/create-transaction/create-transaction.component';
import {Article} from '../../articles/articles-manager/models/article';
import {ArticleService} from '../../articles/articles-manager/services/article.service';
import {NgIf} from '@angular/common';

@Component({
  selector: 'app-transactions-manager',
  standalone: true,
  imports: [
    CreateTransactionComponent,
    NgIf,
  ],
  templateUrl: './transactions-manager.component.html',
  styleUrl: './transactions-manager.component.css'
})
export class TransactionsManagerComponent implements OnInit, OnDestroy{
  transactions: Transaction[] = [];

  constructor(private _transactionService: TransactionService) {}

  ngOnInit(): void {}

  ngOnDestroy(): void {}

   createTransaction(transaction: Transaction) {
    this._transactionService.create(transaction).subscribe(transaction => this.transactions.push(transaction));
    console.log('Transaction created:', transaction);
  }

}
