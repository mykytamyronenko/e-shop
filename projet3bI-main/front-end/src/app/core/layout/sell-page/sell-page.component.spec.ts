import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SellPageComponent } from './sell-page.component';
import {ArticleService} from '../../../features/articles/articles-manager/services/article.service';
import {of, throwError} from 'rxjs';
import {Article} from '../../../features/articles/articles-manager/models/article';

describe('SellPageComponent', () => {
  let component: SellPageComponent;
  let fixture: ComponentFixture<SellPageComponent>;
  let mockArticleService: jasmine.SpyObj<ArticleService>;

  beforeEach(async () => {
    mockArticleService = jasmine.createSpyObj('ArticleService', ['createArticle']);

    // Mock the createArticle method to return an observable of an article
    mockArticleService.createArticle.and.returnValue(of({
      articleId: 1,
      title: 'New Article',
      description: 'This is a new article',
      price: 100,
      category: 'Electronics',  // Valid category
      state: 'new',             // Valid state
      userId: 123,             // Valid userId
      createdAt: '2024-12-20T00:00:00Z', // Valid createdAt timestamp
      updatedAt: '2024-12-20T00:00:00Z', // Valid updatedAt timestamp
      status: 'available',     // Valid status
      mainImageUrl: 'http://example.com/article.jpg',
      additionalImages: '',    // Valid additionalImages (empty string in this case)
      quantity: 10,            // Valid quantity
    }));

    await TestBed.configureTestingModule({
      declarations: [ SellPageComponent ],
      providers: [
        { provide: ArticleService, useValue: mockArticleService }
      ]
    })
      .compileComponents();

    fixture = TestBed.createComponent(SellPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();  // Trigger change detection
  });

  it('should create the SellPageComponent', () => {
    expect(component).toBeTruthy();
  });

  it('should call createArticle and add article to the list', () => {
    const articleData: Article = {
      articleId: 0,
      title: 'New Article',
      description: 'This is a new article',
      price: 100,
      category: 'Electronics',
      state: 'new',
      userId: 123,
      createdAt: '2024-12-20T00:00:00Z',
      updatedAt: '2024-12-20T00:00:00Z',
      status: 'available',
      mainImageUrl: 'http://example.com/article.jpg',
      additionalImages: '',
      quantity: 10,
    };

    // Call the createArticle method with mock data
    component.createArticle({ article: articleData });

    // Ensure createArticle was called with the correct parameters
    expect(mockArticleService.createArticle).toHaveBeenCalledWith(articleData, undefined); // assuming no image file

    // Check if the article is added to the articles list
    expect(component.articles.length).toBe(1);
    expect(component.articles[0].title).toBe('New Article');
    expect(component.articles[0].price).toBe(100);
  });

  it('should handle error if createArticle fails', () => {
    const articleData: Article = {
      articleId: 0,
      title: 'New Article',
      description: 'This is a new article',
      price: 100,
      category: 'Electronics',
      state: 'new',
      userId: 123,
      createdAt: '2024-12-20T00:00:00Z',
      updatedAt: '2024-12-20T00:00:00Z',
      status: 'available',
      mainImageUrl: 'http://example.com/article.jpg',
      additionalImages: '',
      quantity: 10,
    };

    // Simulate an error in the createArticle observable
    mockArticleService.createArticle.and.returnValue(throwError(() => new Error('Error creating article')));

    // Call the method to create an article
    component.createArticle({ article: articleData });

    // Ensure an error is logged (since we don't have a real console in tests, you could spy on console.error)
    spyOn(console, 'error');
    expect(console.error).toHaveBeenCalledWith('Error:', jasmine.anything());
  });
});
