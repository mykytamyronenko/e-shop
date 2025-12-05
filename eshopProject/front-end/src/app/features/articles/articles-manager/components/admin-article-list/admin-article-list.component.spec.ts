import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminArticleListComponent } from './admin-article-list.component';
import {ArticleService} from '../../services/article.service';
import {UserService} from '../../../../../core/user/user-manager/services/user.service';
import {RouterTestingModule} from '@angular/router/testing';
import {NO_ERRORS_SCHEMA} from '@angular/core';
import {Article} from '../../models/article';
import {of, throwError} from 'rxjs';

describe('AdminArticleListComponent', () => {
  let component: AdminArticleListComponent;
  let fixture: ComponentFixture<AdminArticleListComponent>;
  let articleServiceMock: jasmine.SpyObj<ArticleService>;
  let userServiceMock: jasmine.SpyObj<UserService>;

  beforeEach(() => {
    // Mocking the services
    articleServiceMock = jasmine.createSpyObj('ArticleService', ['getAll', 'updateArticleStatusAdmin']);
    userServiceMock = jasmine.createSpyObj('UserService', ['getUserIdFromToken']);

    TestBed.configureTestingModule({
      imports: [RouterTestingModule],
      declarations: [AdminArticleListComponent],
      providers: [
        { provide: ArticleService, useValue: articleServiceMock },
        { provide: UserService, useValue: userServiceMock },
      ],
      schemas: [NO_ERRORS_SCHEMA], // Ignore errors from unrecognized elements
    });

    fixture = TestBed.createComponent(AdminArticleListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should load articles and filter removed ones', () => {
    const mockArticles: Article[] = [
      { articleId: 1, title: 'Article 1', category: 'Electronics', price: 100, state: 'new', description: '', userId: 1, createdAt: '', updatedAt: '', status: 'available', quantity: 1, mainImageUrl: '', additionalImages: '' },
      { articleId: 2, title: 'Article 2', category: 'Books', price: 50, state: 'used', description: '', userId: 1, createdAt: '', updatedAt: '', status: 'removed', quantity: 1, mainImageUrl: '', additionalImages: '' },
    ];

    userServiceMock.getUserIdFromToken.and.returnValue('1'); // Simulating logged in user
    articleServiceMock.getAll.and.returnValue(of(mockArticles));

    component.ngOnInit();

    expect(component.articles.length).toBe(1); // Only 1 article should be loaded (non-removed)
    expect(component.articles[0].status).toBe('available');
  });

  it('should paginate articles correctly', () => {
    const mockArticles: Article[] = [
      { articleId: 1, title: 'Article 1', category: 'Electronics', price: 100, state: 'new', description: '', userId: 1, createdAt: '', updatedAt: '', status: 'available', quantity: 1, mainImageUrl: '', additionalImages: '' },
      { articleId: 2, title: 'Article 2', category: 'Books', price: 50, state: 'used', description: '', userId: 1, createdAt: '', updatedAt: '', status: 'available', quantity: 1, mainImageUrl: '', additionalImages: '' },
      { articleId: 3, title: 'Article 3', category: 'Electronics', price: 150, state: 'new', description: '', userId: 1, createdAt: '', updatedAt: '', status: 'available', quantity: 1, mainImageUrl: '', additionalImages: '' },
    ];

    component.articles = mockArticles;
    component.currentPage = 1;

    const paginatedItems = component.paginatedItems;

    expect(paginatedItems.length).toBe(3); // Should return all 3 items as per current pagination settings
  });

  it('should remove article successfully when confirmed', () => {
    const mockArticles: Article[] = [
      { articleId: 1, title: 'Article 1', category: 'Electronics', price: 100, state: 'new', description: '', userId: 1, createdAt: '', updatedAt: '', status: 'available', quantity: 1, mainImageUrl: '', additionalImages: '' },
    ];

    component.articles = mockArticles;
    spyOn(window, 'confirm').and.returnValue(true); // Simulate user confirmation

    articleServiceMock.updateArticleStatusAdmin.and.returnValue(of(null)); // Mock successful API call

    component.removeArticle(0);

    expect(articleServiceMock.updateArticleStatusAdmin).toHaveBeenCalledWith(1, 'removed');
    expect(component.articles.length).toBe(0); // Article should be removed from the list
    expect(window.alert).toHaveBeenCalledWith('Your article has been successfully removed.');
  });

  it('should handle article removal failure gracefully', () => {
    const mockArticles: Article[] = [
      { articleId: 1, title: 'Article 1', category: 'Electronics', price: 100, state: 'new', description: '', userId: 1, createdAt: '', updatedAt: '', status: 'available', quantity: 1, mainImageUrl: '', additionalImages: '' },
    ];

    component.articles = mockArticles;
    spyOn(window, 'confirm').and.returnValue(true); // Simulate user confirmation

    articleServiceMock.updateArticleStatusAdmin.and.returnValue(throwError('Error')); // Mock API call failure

    component.removeArticle(0);

    expect(articleServiceMock.updateArticleStatusAdmin).toHaveBeenCalledWith(1, 'removed');
    expect(window.alert).toHaveBeenCalledWith('An error occurred while removing the article. Please try again later.');
  });

  it('should return the correct image URL', () => {
    const validUrl = 'images/test-image.png';
    const expectedUrl = 'http://localhost:5185/images/test-image.png';

    const result = component.getImageUrl(validUrl);

    expect(result).toBe(expectedUrl);
  });

  it('should return a default image URL for invalid mainImageUrl', () => {
    const invalidUrl = 'string';
    const expectedUrl = 'http://localhost:4200/test.png';

    const result = component.getImageUrl(invalidUrl);

    expect(result).toBe(expectedUrl);
  });
});
