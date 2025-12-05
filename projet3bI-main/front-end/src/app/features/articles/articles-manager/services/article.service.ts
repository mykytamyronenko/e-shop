import { Injectable } from '@angular/core';
import {API_URLS, environment} from '../../../../../environments/environment';
import {HttpClient} from '@angular/common/http';
import {BehaviorSubject, map, Observable} from 'rxjs';
import {Article} from '../models/article';
import {User} from '../../../../core/user/user-manager/models/user';
import {UserService} from '../../../../core/user/user-manager/services/user.service';
import {Transaction} from '../../../transactions/transactions-manager/models/transaction';
import {Trade} from '../../trade/models/Trade';

@Injectable({
  providedIn: 'root'
})
export class ArticleService {
  static URL: string = `${environment.BASE_URL}/${API_URLS.ARTICLE}`;
  private selectedArticle: BehaviorSubject<Article | null> = new BehaviorSubject<Article | null>(null);

  constructor(private _http: HttpClient, private _userService: UserService) { }

  getAll(): Observable<Article[]> {
    return this._http.get<Article[]>(ArticleService.URL);
  }

  createArticle(article: Article,imageFile?:File): Observable<Article> {
    const formData = new FormData();

    formData.append('Title', article.title);
    formData.append('Description', article.description);
    formData.append('Price', article.price.toString());
    formData.append('Category', article.category);
    formData.append('State', article.state);
    formData.append('UserId', article.userId.toString());
    formData.append('CreatedAt', article.createdAt);
    formData.append('UpdatedAt', article.updatedAt);
    formData.append('Status', article.status);
    formData.append('Quantity', article.quantity.toString());

    if (imageFile) {
      formData.append('Image', imageFile);
    }

    return this._http.post<Article>(ArticleService.URL, formData,{withCredentials:true});
  }

  deleteArticle(articleId: number): Observable<void> {
    return this._http.delete<void>(ArticleService.URL + `/${articleId}`,{withCredentials:true});
  }

  getArticleById(articleId: number): Observable<Article> {
    return this._http.get<Article>(ArticleService.URL + `/${articleId}`);
  }

  setSelectedArticle(article: Article) {
    this.selectedArticle.next(article);
  }

  getSelectedArticle(): Observable<Article | null> {
    return this.selectedArticle.asObservable();
  }

  updateArticle(art: Article): Observable<Article> {
    return this._http.put<Article>(`${ArticleService.URL}/${art.articleId}`, art,{withCredentials:true});
  }

  getTitle(articleId: number): Observable<string> {
    return this._http.get<{ title: string }>(`http://localhost:5185/api/articles/getTitle?id=${articleId}`).pipe(
      map(response => response.title)
    );
  }

  updateArticleStatus(articleId: number, status: 'removed'): Observable<any> {
    return this._http.put<Article>(`${ArticleService.URL}/${articleId}/status`, { status },{withCredentials:true});
  }

  updateArticleStatusAdmin(articleId: number, status: 'removed'): Observable<any> {
    return this._http.put<Article>(`${ArticleService.URL}/${articleId}/admin/status`, { status },{withCredentials:true});
  }

}
