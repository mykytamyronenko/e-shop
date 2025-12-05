import { Component, OnInit, OnDestroy } from '@angular/core';
import {UserService} from './services/user.service';
import {AdminUser, User} from './models/user';

import {UserListComponent} from './components/user-list/user-list.component';
import {CreateUserFormComponent} from './components/create-user-form/create-user-form.component';


@Component({
  selector: 'app-user-manager',
  standalone: true,
  imports: [
    UserListComponent,
    CreateUserFormComponent
  ],
  providers: [
    // comment out when linking to the backend
    // {provide: UserService, useClass: FakeUserService},
  ],
  templateUrl: './user-manager.component.html',
  styleUrl: './user-manager.component.css'
})
export class UserManagerComponent {
  users: User[] = [];
  usersAdmin: AdminUser[] = [];

  constructor(private _userService: UserService) {}

  createUser({ user, imageFile }: { user: User; imageFile?: File }) {
    this._userService.createUser(user, imageFile).subscribe({
      next: (u) => {
        this.users.push(u);
        alert("Account created successfully!");
      } ,
      error: () => alert("This username already exists."),
    });
  }


}
