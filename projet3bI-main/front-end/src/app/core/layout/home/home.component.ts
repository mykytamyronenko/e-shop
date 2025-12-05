import { Component } from '@angular/core';
import {ArticlesManagerComponent} from '../../../features/articles/articles-manager/articles-manager.component';
import {
    TransactionsManagerComponent
} from "../../../features/transactions/transactions-manager/transactions-manager.component";
import {
  CreateTransactionComponent
} from '../../../features/transactions/transactions-manager/components/create-transaction/create-transaction.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    ArticlesManagerComponent,
    TransactionsManagerComponent,
    CreateTransactionComponent
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {

}
