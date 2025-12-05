import { Injectable } from '@angular/core';
import {API_URLS, environment} from '../../../../../environments/environment';
import {HttpClient} from '@angular/common/http';
import {BehaviorSubject, map, Observable} from 'rxjs';
import {AdminUser, FindUser, User} from '../models/user';
import {Article} from '../../../../features/articles/articles-manager/models/article';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  static URL: string = `${environment.BASE_URL}/${API_URLS.USER}`;
  private isLoggedInSubject = new BehaviorSubject<boolean>(false); // Default : logout
  isLoggedIn$ = this.isLoggedInSubject.asObservable();
  private logoutTimer: any;

  constructor(private _http: HttpClient) { }

  getAll(): Observable<AdminUser[]> {
    return this._http.get<AdminUser[]>(UserService.URL,{ withCredentials: true });
  }

  update(user: User): Observable<void> {
    return this._http.put<void>(UserService.URL, user,{withCredentials:true});
  }


  createUser(user: User,imageFile?:File): Observable<User> {
    const formData = new FormData();

    formData.append('Username', user.username);
    formData.append('Email', user.email);
    formData.append('Password', user.password);
    if (imageFile) {
      formData.append('ProfilePicture', imageFile);
    }
    formData.append('MembershipLevel', user.membershipLevel);


    return this._http.post<User>(UserService.URL, formData);
  }

  createAdmin(user: AdminUser,imageFile?:File): Observable<AdminUser> {
    const formData = new FormData();

    formData.append('Username', user.username);
    formData.append('Email', user.email);
    formData.append('Password', user.password);
    formData.append('Role', user.role);

    if (imageFile) {
      formData.append('ProfilePicture', imageFile);
    }
    formData.append('MembershipLevel', user.membershipLevel);
    formData.append('Rating', user.rating.toString());
    formData.append('Status', user.status);

    return this._http.post<AdminUser>("http://localhost:5185/api/usersAdmin", formData,{ withCredentials: true });
  }

  delete(userId: number): Observable<void> {
    return this._http.delete<void>(UserService.URL + `/${userId}`,{ withCredentials: true });
  }

  login(usernameOrEmail: string ,password: string): Observable<any> {
    const loginData = { username: usernameOrEmail ,password: password };
    return this._http.post("http://localhost:5185/api/user/login", loginData, { withCredentials: true });
  }
  logout(): Observable<any> {
    clearTimeout(this.logoutTimer);
    localStorage.removeItem('expiration');
    return this._http.post("http://localhost:5185/api/user/logout", {},{ withCredentials: true });
  }

  startLogoutTimer() {
    const expirationTime = 60 * 60 * 1000; //ms -> 1 hour
    const expirationTimestamp = Date.now() + expirationTime;
    localStorage.setItem('expiration', expirationTimestamp.toString());
    console.log(`User will be logged out at: ${new Date(expirationTimestamp).toLocaleTimeString()}`);
    this.setLogoutTimer(expirationTime);

  }

  restartLogoutTimer() {
    const expirationTimestamp = localStorage.getItem('expiration');
    if (!expirationTimestamp) return;

    const timeRemaining = parseInt(expirationTimestamp, 10) - Date.now();
    if (timeRemaining <= 0) {
      console.warn('Token already expired. Logging out...');
      this.logout().subscribe();
      window.location.reload();
      return;
    }

    console.log(`Restarting logout timer. Time remaining: ${(timeRemaining / 1000)/60} minutes`);

    this.setLogoutTimer(timeRemaining);
  }

  private setLogoutTimer(time: number) {
    clearTimeout(this.logoutTimer);
    this.logoutTimer = setTimeout(() => {
      console.warn('Session expired. Logging out...');
      this.logout().subscribe();
      window.location.reload();
    }, time);
  }



  updateLoginStatus(isLoggedIn: boolean): void {
    this.isLoggedInSubject.next(isLoggedIn);
  }

  checkLoginStatus(): void {
    const isCookiePresent = document.cookie.includes("cookie=");
    this.updateLoginStatus(isCookiePresent);
  }

  getCookie(name: string): string | null {
    const cookies = document.cookie.split(';');
    for (const cookie of cookies) {
      const [key, value] = cookie.split('=').map(c => c.trim());
      if (key === name) {
        return value;
      }
    }
    return null;
  }


  getUserIdFromToken(): string | null {
    const token = this.getCookie('cookie');
    if (!token) {
      console.error("Token not found")
      return null;
    }

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload.userId || null;
    } catch (error) {
      console.error('Error while decoding jwt token', error);
      return null;
    }
  }

  getRoleFromToken(): string | null {
    const token = this.getCookie('cookie');
    if (!token) {
      console.error("Token not found")
      return null;
    }

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload.role || null;
    } catch (error) {
      console.error('Error while decoding jwt token', error);
      return null;
    }
  }

  getById(userId: number): Observable<User> {
    return this._http.get<User>(UserService.URL + `/${userId}`,{ withCredentials: true });
  }

  getUserByUsername(name: string): Observable<FindUser> {
    return this._http.get<FindUser>(`${UserService.URL}/findUser?name=${encodeURIComponent(name)}`);
  }

  getUsername(userId: number): Observable<string> {
    return this._http.get<{ username: string }>(`http://localhost:5185/api/users/getUsername?id=${userId}`).pipe(
      map(response => response.username)
    );
  }

  updateUserStatus(userId: number, status: 'deleted'): Observable<any> {
    return this._http.put<User>(`${UserService.URL}/${userId}/status`, { status },{withCredentials:true});
  }


}
