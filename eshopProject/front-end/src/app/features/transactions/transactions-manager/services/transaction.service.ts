import { Injectable } from '@angular/core';
import {API_URLS, environment} from '../../../../../environments/environment';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {User} from '../../../../core/user/user-manager/models/user';
import {Transaction} from '../models/transaction';

@Injectable({
  providedIn: 'root'
})
export class TransactionService {

  private static URL: string = `${environment.BASE_URL}/${API_URLS.TRANSACTION}`;

  constructor(private _http: HttpClient) { }

  getAll(): Observable<Transaction[]> {
    return this._http.get<Transaction[]>(TransactionService.URL,{withCredentials:true});
  }

  create(transaction: Transaction): Observable<Transaction> {
    return this._http.post<Transaction>(TransactionService.URL, transaction,{withCredentials:true});
  }
}
