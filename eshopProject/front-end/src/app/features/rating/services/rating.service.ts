import { Injectable } from '@angular/core';
import {API_URLS, environment} from '../../../../environments/environment';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {Rating} from '../model/Rating';


@Injectable({
  providedIn: 'root'
})
export class RatingService {

  private static URL: string = `${environment.BASE_URL}/${API_URLS.RATING}`;

  constructor(private _http: HttpClient) { }

  getAll(): Observable<Rating[]> {
    return this._http.get<Rating[]>(RatingService.URL);
  }

  create(rate: Rating): Observable<Rating> {
    return this._http.post<Rating>(RatingService.URL, rate,{withCredentials:true});
  }

  getById(id: number): Observable<Rating> {
    return this._http.get<Rating>(RatingService.URL + `/${id}`);
  }


}
