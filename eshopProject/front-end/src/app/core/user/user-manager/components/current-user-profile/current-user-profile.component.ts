import {Component, OnDestroy, OnInit} from '@angular/core';
import {UserService} from '../../services/user.service';
import {User} from '../../models/user';
import {NgIf} from '@angular/common';
import {RatingService} from '../../../../../features/rating/services/rating.service';
import {Rating} from '../../../../../features/rating/model/Rating';
import {EditUserProfileFormComponent} from '../edit-user-profile-form/edit-user-profile-form.component';
import {Subscription} from 'rxjs';
import {EventBusService} from '../../../../../shared/services/event-bus/event-bus.service';

@Component({
  selector: 'app-current-user-profile',
  standalone: true,
  imports: [
    NgIf,
    EditUserProfileFormComponent
  ],
  templateUrl: './current-user-profile.component.html',
  styleUrl: './current-user-profile.component.css'
})
export class CurrentUserProfileComponent implements OnInit, OnDestroy {
  user: User | null = null;
  userId: string | null = null;
  ratings: Rating[] = [];
  score: number = 0;
  private _userUpdatedSubscription: Subscription | null = null;

  constructor(private _userService: UserService,private _ratingService: RatingService, private _eventBus: EventBusService) {}

  ngOnInit(): void {
    this.userId = this._userService.getUserIdFromToken();

    if (!this.userId) {
      console.error('User not authenticated');
      return;
    }

    const numUserId = parseInt(this.userId, 10);

    this._userService.getById(numUserId).subscribe({
      next: (user: User) => {
        console.log(user);
        this.user = user;
      },
      error: (error: any) => {
        console.error('Error retrieving user:', error);
      }
    });

    this._ratingService.getAll().subscribe(ratings => {
      this.ratings = ratings.filter(rating => rating.userId === numUserId); // We filter the notes that concern the user
      this.calculateUserScore(); // We calculate the user score
    });

    this._userUpdatedSubscription = this._eventBus.listen("USER_UPDATED").subscribe(event => this.updateUser(event.object));
  }

  ngOnDestroy(): void {
    this._userUpdatedSubscription?.unsubscribe();
  }

  calculateUserScore(){
    if (this.ratings.length === 0) {
      this.score = 0;
      return;
    }

    const totalScore = this.ratings.reduce((acc, rating) => acc + rating.score, 0);

    const totalVoters = this.ratings.length;
    this.score = totalScore / totalVoters;

    console.log(`Total score: ${totalScore}, Total voters: ${totalVoters}, User Score: ${this.score}`);
  }


  updateUser(user: User) {
    this._userService.update(user).subscribe();
  }

}
