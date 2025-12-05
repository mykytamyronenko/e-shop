import {Component, inject} from '@angular/core';
import {FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators} from '@angular/forms';
import {UserService} from '../../user/user-manager/services/user.service';
import {Router} from '@angular/router';
import {AdminUser, User} from '../../user/user-manager/models/user';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    FormsModule,
    ReactiveFormsModule
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  userId: string | null = null;
  user: AdminUser | null = null;
  private _fb:FormBuilder = inject(FormBuilder);
  constructor(private _userService: UserService,private router: Router) {
  }
  form: FormGroup = this._fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
  });

  submitForm() {
    this._userService.login(this.form.value.username,this.form.value.password).subscribe({
      next: (response) => {
        this.userId = this._userService.getUserIdFromToken();
        const cookie = this._userService.getCookie('cookie');
        if (cookie == "AccountDeleted") {
          alert("Your account has been deleted.")
          this._userService.logout().subscribe(() => {
            this._userService.updateLoginStatus(false);
            this.router.navigate(['/']);
          });
        } else {
        alert("login successful")
        console.log('Login successful', response);
        this._userService.updateLoginStatus(true);
        this._userService.startLogoutTimer()
          this.router.navigateByUrl('/hub', { skipLocationChange: false }).then(() => {
            window.location.reload();
          });
        }
      },
      error: (err) => {
        console.error('failed to connect', err);
        alert('Wrong email, username or password. Please try again.');
      }
    });
  }

}
