import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router, RouterLink} from '@angular/router';
import {UserService} from '../../user/user-manager/services/user.service';
import {NgIf} from '@angular/common';
import {HttpClient} from '@angular/common/http';
import {User} from '../../user/user-manager/models/user';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [
    RouterLink,
    NgIf
  ],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent implements OnInit {
  isDarkTheme = true;
  isLoggedIn: boolean = false;
  isLoggedAndAdmin:boolean = false;
  profileImageUrl: string = '/test.png'; //default img
  user: User = {userId: 0, username: "", email: "", password: "", profilePicture: "", membershipLevel: "", balance: 0};

  constructor(private _userService: UserService,private http: HttpClient, private router: Router,private route: ActivatedRoute) {}

  ngOnInit(): void {
    this._userService.isLoggedIn$.subscribe((status) => {
      this.isLoggedIn = status;
    });
    this.adminOrNot();
    this.loadUserProfileImage();

    const savedTheme = localStorage.getItem('theme');
    this.isDarkTheme = savedTheme === 'dark';
    document.body.className = this.isDarkTheme ? 'dark-theme' : 'light-theme';

    var userId = this._userService.getUserIdFromToken()

    if(userId)
    {
      this._userService.getById(parseInt(userId)).subscribe({
        next: (user: User) => {
          this.user = user;
        },
        error: (error: any) => {
          console.error('Error retrieving user:', error);
        }
      });

      this.route.params.subscribe(params => {
        this.user.username = params['username'];
      });

    }

  }

  toggleTheme() {
    this.isDarkTheme = !this.isDarkTheme;
    document.body.className = this.isDarkTheme ? 'dark-theme' : 'light-theme';
    localStorage.setItem('theme', this.isDarkTheme ? 'dark' : 'light');
  }

  logout(): void {
    const confirmation = window.confirm('Do you really want to logout ?');
    if (confirmation) {
      this._userService.logout().subscribe(() => {
        this._userService.updateLoginStatus(false);
        this.isLoggedAndAdmin = false;
        alert('Successfully logged out !');
        this.router.navigate(['/']);
      });
    }
  }

  adminOrNot():void {
    if(this._userService.getRoleFromToken() ==="admin"){
      this.isLoggedAndAdmin = true;
    }
  }

  loadUserProfileImage() {
    const userId = this._userService.getUserIdFromToken();

    // Appel au backend pour récupérer les infos de l'utilisateur
    this.http.get<User>(`http://localhost:5185/api/users/${userId}`,{ withCredentials: true })
      .subscribe({
        next: (user) => {
          this.profileImageUrl = `http://localhost:5185/${user.profilePicture}`;

        },
        error: (err) => {
        }
      });
  }
  onImageError() {
    this.profileImageUrl = 'test.png';
  }

}
