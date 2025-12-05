import { Injectable } from '@angular/core';
import {API_URLS, environment} from '../../../../../environments/environment';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {Trade} from '../models/Trade';

@Injectable({
  providedIn: 'root'
})
export class TradeService {
  private static URL: string = `${environment.BASE_URL}/${API_URLS.TRADE}`;

  constructor(private _http: HttpClient) { }

  getAll(): Observable<Trade[]> {
    return this._http.get<Trade[]>(TradeService.URL,{withCredentials:true});
  }

  createTrade(trade: Trade): Observable<Trade> {
    return this._http.post<Trade>(TradeService.URL, trade,{withCredentials:true});
  }

  updateTradeStatus(tradeId: number, status: 'accepted' | 'denied'): Observable<any> {
    return this._http.put(`${TradeService.URL}/${tradeId}/status`, { status },{withCredentials:true});
  }






}
