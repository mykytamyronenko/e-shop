import { Component } from '@angular/core';
import {UserService} from '../../user/user-manager/services/user.service';
import {FindUser, User} from '../../user/user-manager/models/user';
import {FormsModule} from '@angular/forms';
import {NgIf} from '@angular/common';
import {RouterLink} from '@angular/router';

@Component({
  selector: 'app-find-user',
  standalone: true,
  imports: [
    FormsModule,
    NgIf,
    RouterLink
  ],
  templateUrl: './find-user.component.html',
  styleUrl: './find-user.component.css'
})
export class FindUserComponent {
  userFound : FindUser | undefined
  searchInput: string = '';
  profileImageUrl : string = '';
  isUserFound: boolean = false;

  constructor(private _userService: UserService) {
  }


  submitSearch(username : string) {

    if (!username.trim()) {
      console.log('Username cannot be empty.');
      return;
    }

    this._userService.getUserByUsername(username).subscribe({
      next: (user: FindUser) => {
        console.log('User found : ',user);
        this.userFound = user;
        this.profileImageUrl =  `http://localhost:5185/${user.profilePicture}`;
        this.isUserFound = true;
      },
      error: (error: any) => {
        console.error('Error retrieving user:', error);
        this.isUserFound = false;
        alert("user not found.");
      }
    });
  }




}
