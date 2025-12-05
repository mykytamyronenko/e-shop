import {Component, OnDestroy, OnInit} from '@angular/core';
import {Transaction} from '../../models/transaction';
import {TransactionService} from '../../services/transaction.service';
import {UserService} from '../../../../../core/user/user-manager/services/user.service';
import {AsyncPipe, DatePipe} from '@angular/common';
import {RatingService} from '../../../../rating/services/rating.service';
import {Rating} from '../../../../rating/model/Rating';
import {BehaviorSubject, Observable} from 'rxjs';
import {ArticleService} from '../../../../articles/articles-manager/services/article.service';

@Component({
  selector: 'app-transaction-history',
  standalone: true,
  imports: [
    DatePipe,
    AsyncPipe
  ],
  templateUrl: './transaction-history.component.html',
  styleUrl: './transaction-history.component.css'
})
export class TransactionHistoryComponent implements OnInit, OnDestroy{
  transactions: Transaction[] = [];
  ratings: Rating[] = [];
  userId: string | null = null;

  constructor(private _transactionService: TransactionService, private _userService: UserService, private _ratingService: RatingService,private _articleService: ArticleService) {}

    ngOnInit(): void {
      this.userId = this._userService.getUserIdFromToken();

      if (!this.userId) {
        console.error('User not authenticated');
        return;
      }

      const numUserId = parseInt(this.userId, 10);

      this._transactionService.getAll().subscribe(transactions => {console.log(transactions);
        this.transactions = transactions.filter(
          transaction => transaction.buyerId === numUserId
        );
      });
      this._ratingService.getAll().subscribe(ratings => {
        this.ratings = ratings;
      });
    }



    ngOnDestroy(): void {
    }

  sendRating(articleId: number, sellerId: number) {
    if (confirm("Are you sure you want to rate it?")) {
      const conUser = this._userService.getUserIdFromToken();

      if (conUser) {
        const reviewerId = parseInt(conUser, 10);

          if(this.existingRating(sellerId)){
            return
          }


          const scoreInput = document.querySelector(`input[name="numberOfStar_${articleId}"]:checked`) as HTMLInputElement;
          const commentInput = document.getElementById(`commentId-${articleId}`) as HTMLInputElement;

          const score = scoreInput ? parseInt(scoreInput.value, 10) : 0;
          const comment = commentInput ? commentInput.value : 'No comment provided';

          const rate = {
            userId: sellerId,
            reviewerId: reviewerId,
            articleId: articleId,
            score: score,
            comment: comment,
            createdAt: new Date().toISOString()
          };

          this._ratingService.create(rate).subscribe(newRating => {
            this.ratings.push(newRating);
            console.log('Rating sent:', newRating);
          });


      } else {
        alert('You are not authenticated.');
      }
    }
  }

  existingRating(sellerId: number){
    const conUser = this._userService.getUserIdFromToken();
    if (conUser) {
      const reviewerId = parseInt(conUser, 10);

      const existingRating = this.ratings.find(
        rating => rating.userId === sellerId && rating.reviewerId === reviewerId
      );
      if (existingRating) {
        return true;
      }
      return false;
    }
    return false;
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

}
