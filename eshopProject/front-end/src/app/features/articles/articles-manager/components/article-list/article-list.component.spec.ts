import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ArticleListComponent } from './article-list.component';
import {ArticleService} from '../../services/article.service';
import {UserService} from '../../../../../core/user/user-manager/services/user.service';
import {Router} from '@angular/router';
import {RouterTestingModule} from '@angular/router/testing';
import {NO_ERRORS_SCHEMA} from '@angular/core';
import {Article} from '../../models/article';

describe('ArticleListComponent', () => {
  let component: ArticleListComponent;
  let fixture: ComponentFixture<ArticleListComponent>;
  let articleServiceMock: jasmine.SpyObj<ArticleService>;
  let userServiceMock: jasmine.SpyObj<UserService>;
  let routerMock: jasmine.SpyObj<Router>;

  beforeEach(() => {
    // Mocking services
    articleServiceMock = jasmine.createSpyObj('ArticleService', ['setSelectedArticle']);
    userServiceMock = jasmine.createSpyObj('UserService', ['getUserIdFromToken']);
    routerMock = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      imports: [RouterTestingModule],
      declarations: [ArticleListComponent],
      providers: [
        { provide: ArticleService, useValue: articleServiceMock },
        { provide: UserService, useValue: userServiceMock },
        { provide: Router, useValue: routerMock },
      ],
      schemas: [NO_ERRORS_SCHEMA], // Ignore errors from unrecognized elements
    });

    fixture = TestBed.createComponent(ArticleListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should set articles and filter correctly', () => {
    const mockArticles: Article[] = [
      { articleId: 1, title: 'Article 1', category: 'Electronics', price: 100, state: 'new', description: '', userId: 1, createdAt: '', updatedAt: '', status: 'available', quantity: 1, mainImageUrl: '', additionalImages: '' },
      { articleId: 2, title: 'Article 2', category: 'Books', price: 50, state: 'used', description: '', userId: 1, createdAt: '', updatedAt: '', status: 'available', quantity: 1, mainImageUrl: '', additionalImages: '' },
    ];

    component.articles = mockArticles;

    // Test if the filtered articles are set correctly
    expect(component.filteredArticles.length).toBe(2);
  });

  it('should paginate the articles correctly', () => {
    const mockArticles: Article[] = [
      { articleId: 1, title: 'Article 1', category: 'Electronics', price: 100, state: 'new', description: '', userId: 1, createdAt: '', updatedAt: '', status: 'available', quantity: 1, mainImageUrl: '', additionalImages: '' },
      { articleId: 2, title: 'Article 2', category: 'Books', price: 50, state: 'used', description: '', userId: 1, createdAt: '', updatedAt: '', status: 'available', quantity: 1, mainImageUrl: '', additionalImages: '' },
      { articleId: 3, title: 'Article 3', category: 'Electronics', price: 150, state: 'new', description: '', userId: 1, createdAt: '', updatedAt: '', status: 'available', quantity: 1, mainImageUrl: '', additionalImages: '' },
    ];

    component.articles = mockArticles;

    // Setting the page to 1 and getting the paginated items
    component.currentPage = 1;
    const paginatedItems = component.paginatedItems;

    expect(paginatedItems.length).toBe(3); // Expect 3 items as per the currentPage and itemsPerPage
  });

  it('should filter articles by category', () => {
    const mockArticles: Article[] = [
      { articleId: 1, title: 'Article 1', category: 'Electronics', price: 100, state: 'new', description: '', userId: 1, createdAt: '', updatedAt: '', status: 'available', quantity: 1, mainImageUrl: '', additionalImages: '' },
      { articleId: 2, title: 'Article 2', category: 'Books', price: 50, state: 'used', description: '', userId: 1, createdAt: '', updatedAt: '', status: 'available', quantity: 1, mainImageUrl: '', additionalImages: '' },
    ];

    component.articles = mockArticles;

    // Simulate the category filter
    component.filterByCategory({ target: { value: 'Electronics' } } as any);

    expect(component.filteredArticles.length).toBe(1);
    expect(component.filteredArticles[0].category).toBe('Electronics');
  });

  it('should emit article transaction when emitArticleTransaction is called', () => {
    const mockArticle: Article = {
      articleId: 1,
      title: 'Test Article',
      category: 'Electronics',
      description:"desc",
      price: 100,
      state: 'new',
      userId: 1,
      createdAt: '',
      updatedAt: '',
      status: 'available',
      quantity: 1,
      mainImageUrl: '',
      additionalImages: ''
    };

    const index = 0;

    spyOn(component.articleBought, 'emit');

    userServiceMock.getUserIdFromToken.and.returnValue('1'); // Simulate user being logged in

    component.emitArticleTransaction(mockArticle, index);

    expect(component.articleBought.emit).toHaveBeenCalledWith({ article: mockArticle, index: index });
  });

  it('should navigate to login if user is not logged in when emitArticleTransaction is called', () => {
    const mockArticle: Article = {
      articleId: 1,
      title: 'Test Article',
      category: 'Electronics',
      description:"desc",
      price: 100,
      state: 'new',
      userId: 1,
      createdAt: '',
      updatedAt: '',
      status: 'available',
      quantity: 1,
      mainImageUrl: '',
      additionalImages: ''
    };

    const index = 0;

    spyOn(window, 'confirm').and.returnValue(true); // Simulate user confirming they want to log in
    spyOn(routerMock, 'navigate');

    userServiceMock.getUserIdFromToken.and.returnValue(null); // Simulate user being not logged in

    component.emitArticleTransaction(mockArticle, index);

    expect(routerMock.navigate).toHaveBeenCalledWith(['/login']);
  });
});
