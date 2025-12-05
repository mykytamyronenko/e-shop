import {Component, OnInit} from '@angular/core';
import {Trade} from '../../models/Trade';
import {TradeService} from '../../services/trade.service';
import {UserService} from '../../../../../core/user/user-manager/services/user.service';
import {AsyncPipe, DatePipe, JsonPipe, NgForOf, NgIf} from '@angular/common';
import {ArticleService} from '../../../articles-manager/services/article.service';
import {BehaviorSubject, forkJoin, Observable, tap} from 'rxjs';

@Component({
  selector: 'app-trade-receiver',
  standalone: true,
  imports: [
    DatePipe,
    NgForOf,
    NgIf,
    AsyncPipe,
    JsonPipe
  ],
  templateUrl: './trade-receiver.component.html',
  styleUrl: './trade-receiver.component.css'
})
export class TradeReceiverComponent implements OnInit{
  trades : Trade[] = [];
  userId : string | null ="";

constructor(private _tradeService: TradeService,private _userService: UserService,private _articleService: ArticleService) {
}

  ngOnInit() {
    this.userId = this._userService.getUserIdFromToken();
    if(this.userId){
      const numericUserId = parseInt(this.userId, 10);

      this._tradeService.getAll().subscribe(trades => {
        console.log('Fetched trades:', trades);
        this.trades = trades
          .filter(trade => trade.receiverId === numericUserId)
          .filter(trade => trade.status === 'in progress');
      });

    }
    else {
      console.log("problem with the userid")
    }

  }

  adjustDateByOneHour(dateString: string): Date {
    const date = new Date(dateString);
    date.setHours(date.getHours() + 1);
    return date;
  }

  acceptTrade(trade: Trade) {
    this._tradeService.updateTradeStatus(trade.tradeId, 'accepted').subscribe({
      next: () => {
        console.log(`Trade ${trade.tradeId} successfully accepted`);
        this.trades = this.trades.filter(t => t.tradeId !== trade.tradeId);
      },
      error: (error) => {
        console.error(`Failed to accept trade ${trade.tradeId}`, error);
        alert('An error occurred while accepting the trade. Please try again.');
      }
    });
  }


  declineTrade(trade: Trade) {
    this._tradeService.updateTradeStatus(trade.tradeId, 'denied').subscribe({
      next: () => {
        console.log(`Trade ${trade.tradeId} successfully denied`);
        this.trades = this.trades.filter(t => t.tradeId !== trade.tradeId);
      },
      error: (error) => {
        console.error(`Failed to deny trade ${trade.tradeId}`, error);
        alert('An error occurred while denying the trade. Please try again.');
      }
    });
  }

  public titles: { [articleId: number]: BehaviorSubject<String> } = {};

  getTitle(articleId: number): Observable<String> {
    if (!this.titles[articleId]) {
      this.titles[articleId] = new BehaviorSubject<String>('Loading...');
      this._articleService.getTitle(articleId).subscribe({
        next: (title) => {
          this.titles[articleId].next(title);
        },
        error: (err) => {
          console.error(`Failed to load title for articleId=${articleId}`, err);
          this.titles[articleId].next('Unknown Article');
        }
      });
    }
    return this.titles[articleId].asObservable();
  }

  public usernames: { [userId: number]: BehaviorSubject<String> } = {};

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


  public titles2: { [articleId: number]: BehaviorSubject<string> } = {};

  getTitle2(articleIds: string): BehaviorSubject<string[]> {
    const ids = articleIds.split(',').map(id => parseInt(id.trim(), 10));
    const titleSubject = new BehaviorSubject<string[]>([]);
    const titleList: string[] = [];

    ids.forEach((articleId) => {
      if (!this.titles2[articleId]) {
        this.titles2[articleId] = new BehaviorSubject<string>('Loading...');

        this._articleService.getTitle(articleId).subscribe({
          next: (title) => {
            this.titles2[articleId].next(title);
            titleList.push(title);
            titleSubject.next([...titleList]);
          },
          error: (err) => {
            console.error(`Failed to load title for articleId=${articleId}`, err);
            this.titles2[articleId].next('Unknown Article');
            titleList.push('Unknown Article');
            titleSubject.next([...titleList]);
          }
        });
      } else {
        titleList.push(this.titles2[articleId].getValue());
        titleSubject.next([...titleList]);
      }
    });

    return titleSubject;
  }









}
