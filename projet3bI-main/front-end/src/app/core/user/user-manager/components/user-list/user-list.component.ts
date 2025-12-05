import {Component, EventEmitter, Input, OnDestroy, OnInit, Output} from '@angular/core';
import {AdminUser, User} from '../../models/user';
import {FormsModule} from '@angular/forms';
import {UserService} from '../../services/user.service';


@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [
    FormsModule
  ],
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.css'
})
export class UserListComponent implements OnInit, OnDestroy {
  @Input()
  users: AdminUser[] = [];

  constructor(private _userService: UserService) {}

  ngOnInit() {
    this._userService.getAll().subscribe(users => {console.log(users);
      this.users = users.filter(user => user.status !== 'deleted');
    });
  }

  ngOnDestroy() {}

  removeUser(index: number) {
    const userToRemove = this.users[index];

    const userConfirmed = confirm(`Are you sure you want to remove the user "${userToRemove.username}"?`);
    if (!userConfirmed) {
      return;
    }

    this._userService.updateUserStatus(userToRemove.userId, 'deleted').subscribe({
      next: () => {
        console.log(`User ${userToRemove.userId} deleted`);

        this.users = this.users.filter(u => u.userId !== userToRemove.userId);

        alert(`The user has been successfully removed.`);
      },
      error: (error) => {
        console.error(`Failed to remove user ${userToRemove.userId}`, error);

        alert('An error occurred while deleting the user.');
      }
    });
  }

  getImageUrl(mainImageUrl: string): string {
    const BASE_URL = 'http://localhost:5185/';

    if(mainImageUrl.startsWith('string')){
      return 'http://localhost:4200/test.png';
    }

    return `${BASE_URL}${mainImageUrl}`;
  }
}
