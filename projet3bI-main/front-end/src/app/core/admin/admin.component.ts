import {Component, EventEmitter, inject, OnInit, Output} from '@angular/core';
import {FormBuilder, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {UserManagerComponent} from '../user/user-manager/user-manager.component';
import {AdminUser, User} from '../user/user-manager/models/user';
import {UserService} from '../user/user-manager/services/user.service';
import {UserListComponent} from '../user/user-manager/components/user-list/user-list.component';
import {CreateUserFormComponent} from '../user/user-manager/components/create-user-form/create-user-form.component';
import {CreateAdminFormComponent} from './create-admin-form/create-admin-form.component';
import {
  AdminArticleListComponent
} from '../../features/articles/articles-manager/components/admin-article-list/admin-article-list.component';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    UserManagerComponent,
    UserListComponent,
    CreateUserFormComponent,
    CreateAdminFormComponent,
    AdminArticleListComponent
  ],
  templateUrl: './admin.component.html',
  styleUrl: './admin.component.css'
})
export class AdminComponent implements OnInit{
  usersAdmin: AdminUser[] = [];

  constructor(private _userService: UserService) {}

  ngOnInit(): void {
    this._userService.getAll().subscribe(users => {console.log(users);
      this.usersAdmin = users;});

  }

  createAdmin({ user, imageFile }: { user: AdminUser; imageFile?: File }) {
    this._userService.createAdmin(user, imageFile).subscribe({
      next: (userAdmin) => this.usersAdmin.push(userAdmin),
      error: (err) => console.error('Error creating user:', err),
    });
  }

}
