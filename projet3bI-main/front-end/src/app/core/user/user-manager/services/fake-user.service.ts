import { Injectable } from '@angular/core';
import {UserService} from './user.service';
import {Observable, of} from 'rxjs';
import {AdminUser, User} from '../models/user';

@Injectable({
  providedIn: 'root'
})
export class FakeUserService extends UserService {
  override getAll(): Observable<AdminUser[]> {
    return of([
      {
        userId: 1,
        username: "username1",
        email: "username1@gmail.com",
        password: "password1",
        role: "connected_user",
        profilePicture: "picture1",
        membershipLevel: "level1",
        rating: 1,
        status: 'active',
        balance: 0
      },
      {
        userId: 2,
        username: "username2",
        email: "username2@gmail.com",
        password: "password2",
        role: "admin",
        profilePicture: "picture2",
        membershipLevel: "level2",
        rating: 2,
        status: 'suspended',
        balance: 0
      }
    ])
  }
}
