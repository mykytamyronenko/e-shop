import {Component, OnInit} from '@angular/core';
import {RouterOutlet} from '@angular/router';
import {NavbarComponent} from './core/layout/navbar/navbar.component';
import {UserManagerComponent} from './core/user/user-manager/user-manager.component';
import {UserService} from './core/user/user-manager/services/user.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NavbarComponent, UserManagerComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  title = 'projet3bi';

  constructor(private userService: UserService) {}

  ngOnInit() {
    this.userService.checkLoginStatus();
    this.userService.restartLogoutTimer();

  }


}
