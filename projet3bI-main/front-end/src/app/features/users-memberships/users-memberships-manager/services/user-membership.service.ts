import { Injectable } from '@angular/core';
import {API_URLS, environment} from '../../../../../environments/environment';
import {HttpClient} from '@angular/common/http';
import {catchError, map, Observable} from 'rxjs';
import {Transaction} from '../../../transactions/transactions-manager/models/transaction';
import {UserMembership} from '../models/user-membership';

@Injectable({
  providedIn: 'root'
})
export class UserMembershipService {

  private static URL: string = `${environment.BASE_URL}/${API_URLS.USER_MEMBERSHIP}`;

  constructor(private _http: HttpClient) { }

  getAll(): Observable<UserMembership[]> {
    return this._http.get<UserMembership[]>(UserMembershipService.URL,{withCredentials:true});
  }

  create(userMembership: UserMembership): Observable<UserMembership> {
    return this._http.post<UserMembership>(UserMembershipService.URL, userMembership,{withCredentials:true});
  }
}
