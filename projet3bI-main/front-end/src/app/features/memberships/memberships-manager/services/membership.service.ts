import { Injectable } from '@angular/core';
import {API_URLS, environment} from '../../../../../environments/environment';
import {HttpClient} from '@angular/common/http';
import {BehaviorSubject, Observable} from 'rxjs';
import {Membership} from '../models/membership';
import {Article} from '../../../articles/articles-manager/models/article';

@Injectable({
  providedIn: 'root'
})
export class MembershipService {

  private static URL: string = `${environment.BASE_URL}/${API_URLS.MEMBERSHIP}`;
  private selectedMembership: BehaviorSubject<Membership | null> = new BehaviorSubject<Membership | null>(null);

  constructor(private _http: HttpClient) { }

  getAll(): Observable<Membership[]> {
    return this._http.get<Membership[]>(MembershipService.URL,{ withCredentials: true });
  }

  getMembershipById(membershipId: number): Observable<Membership> {
    return this._http.get<Membership>(MembershipService.URL + `/${membershipId}`,{withCredentials:true});
  }

  setSelectedMembership(membership: Membership) {
    this.selectedMembership.next(membership);
  }

  getSelectedMembership(): Observable<Membership | null> {
    return this.selectedMembership.asObservable();
  }
}
