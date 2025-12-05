import {Component, Input, OnInit} from '@angular/core';
import {
  CreateArticleFormComponent
} from '../../../features/articles/articles-manager/components/create-article-form/create-article-form.component';
import {CreateUserFormComponent} from "../../user/user-manager/components/create-user-form/create-user-form.component";
import {Article} from '../../../features/articles/articles-manager/models/article';
import {ArticleService} from '../../../features/articles/articles-manager/services/article.service';
import {AdminUser, User} from '../../user/user-manager/models/user';
import {SellPageComponent} from '../sell-page/sell-page.component';
import {NgIf} from '@angular/common';
import {ActivatedRoute, RouterLink} from '@angular/router';
import {
  TransactionHistoryComponent
} from '../../../features/transactions/transactions-manager/components/transaction-history/transaction-history.component';
import {
  OwnedByUserArticleListComponent
} from '../../../features/articles/articles-manager/components/owned-by-user-article-list/owned-by-user-article-list.component';
import {Transaction} from '../../../features/transactions/transactions-manager/models/transaction';
import {TransactionService} from '../../../features/transactions/transactions-manager/services/transaction.service';
import {UserService} from '../../user/user-manager/services/user.service';
import {
  CurrentUserProfileComponent
} from '../../user/user-manager/components/current-user-profile/current-user-profile.component';
import {TradeComponent} from '../../../features/articles/trade/trade.component';
import {
  TradeReceiverComponent
} from '../../../features/articles/trade/components/trade-receiver/trade-receiver.component';
import {
  SubscriptionDisplayComponent
} from '../../../features/users-memberships/users-memberships-manager/components/subscription-display/subscription-display.component';
import {
  SellHistoryComponent
} from '../../../features/transactions/transactions-manager/components/sell-history/sell-history.component';

@Component({
  selector: 'app-profile-page',
  standalone: true,
  imports: [
    CreateArticleFormComponent,
    CreateUserFormComponent,
    SellPageComponent,
    RouterLink,
    TransactionHistoryComponent,
    OwnedByUserArticleListComponent,
    CurrentUserProfileComponent,
    TradeComponent,
    TradeReceiverComponent,
    SubscriptionDisplayComponent,
    SellHistoryComponent
  ],
  templateUrl: './profile-page.component.html',
  styleUrl: './profile-page.component.css'
})
export class ProfilePageComponent implements OnInit{
  constructor(private route: ActivatedRoute,private _userService: UserService,) {
  }

  ngOnInit(): void {




  }

}
