import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OwnedByUserArticleListComponent } from './owned-by-user-article-list.component';
import {ArticleService} from '../../services/article.service';
import {UserService} from '../../../../../core/user/user-manager/services/user.service';
import {Router} from '@angular/router';
import {Article} from '../../models/article';
import {RouterTestingModule} from '@angular/router/testing';
import {of} from 'rxjs';

describe('OwnedByUserArticleListComponent', () => {
  let component: OwnedByUserArticleListComponent;
  let fixture: ComponentFixture<OwnedByUserArticleListComponent>;
  let articleService: jasmine.SpyObj<ArticleService>;
  let userService: jasmine.SpyObj<UserService>;
  let router: jasmine.SpyObj<Router>;

  const mockArticles: Article[] = [
    {
      articleId: 1,
      title: 'Article 1',
      description: 'Description 1',
      price: 100,
      category: 'Category 1',
      state: 'new',
      userId: 1,
      createdAt: '2024-01-01T00:00:00Z',
      updatedAt: '2024-01-01T00:00:00Z',
      status: 'available',
      mainImageUrl: 'image1.jpg',
      additionalImages: '',
      quantity: 1,
    },
    {
      articleId: 2,
      title: 'Article 2',
      description: 'Description 2',
      price: 200,
      category: 'Category 2',
      state: 'used',
      userId: 1,
      createdAt: '2024-01-02T00:00:00Z',
      updatedAt: '2024-01-02T00:00:00Z',
      status: 'available',
      mainImageUrl: 'image2.jpg',
      additionalImages: '',
      quantity: 1,
    }
  ];

  beforeEach(() => {
    const articleServiceSpy = jasmine.createSpyObj('ArticleService', ['getAll', 'updateArticleStatus']);
    const userServiceSpy = jasmine.createSpyObj('UserService', ['getUserIdFromToken']);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      declarations: [OwnedByUserArticleListComponent],
      imports: [RouterTestingModule],
      providers: [
        { provide: ArticleService, useValue: articleServiceSpy },
        { provide: UserService, useValue: userServiceSpy },
        { provide: Router, useValue: routerSpy },
      ]
    });

    fixture = TestBed.createComponent(OwnedByUserArticleListComponent);
    component = fixture.componentInstance;
    articleService = TestBed.inject(ArticleService) as jasmine.SpyObj<ArticleService>;
    userService = TestBed.inject(UserService) as jasmine.SpyObj<UserService>;
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;

    userService.getUserIdFromToken.and.returnValue('1');
    articleService.getAll.and.returnValue(of(mockArticles));

    fixture.detectChanges();
  });

  describe('ngOnInit()', () => {
    it('should fetch articles and filter by userId', () => {
      component.ngOnInit();
      expect(articleService.getAll).toHaveBeenCalled();
      expect(component.ownedArticles.length).toBe(2);
      expect(component.ownedArticles[0].userId).toBe(1);
    });
  });

  describe('Pagination', () => {
    it('should calculate paginated items correctly', () => {
      component.ownedArticles = mockArticles;
      component.currentPage = 1;
      expect(component.paginatedItems.length).toBe(2);
    });

    it('should calculate total pages correctly', () => {
      component.ownedArticles = mockArticles;
      expect(component.totalPages).toBe(1);
    });

    it('should chunk paginated items into rows of 4', () => {
      component.ownedArticles = mockArticles;
      component.itemsPerRow = 1; // Test with a row size of 1 to test chunking
      expect(component.chunkedPaginatedItems().length).toBe(2); // 2 articles, each in its own row
    });
  });

  describe('getImageUrl()', () => {
    it('should return the correct image URL', () => {
      const imageUrl = component.getImageUrl('image1.jpg');
      expect(imageUrl).toBe('http://localhost:5185/image1.jpg');
    });

    it('should return default image URL if image URL starts with "string"', () => {
      const imageUrl = component.getImageUrl('string');
      expect(imageUrl).toBe('http://localhost:4200/test.png');
    });
  });

  describe('navigateToModifyArticle()', () => {
    it('should navigate to the article modification route', () => {
      component.navigateToModifyArticle(mockArticles[0]);
      expect(router.navigate).toHaveBeenCalledWith(['/editArticle', mockArticles[0].articleId]);
    });
  });

  describe('removeArticle()', () => {
    it('should remove an article when confirmed', () => {
      const removeSpy = spyOn(window, 'confirm').and.returnValue(true);
      articleService.updateArticleStatus.and.returnValue(of(mockArticles[0]));
      component.ownedArticles = mockArticles;

      component.removeArticle(0);
      expect(removeSpy).toHaveBeenCalled();
      expect(articleService.updateArticleStatus).toHaveBeenCalledWith(mockArticles[0].articleId, 'removed');
      expect(component.ownedArticles.length).toBe(1);
    });

    it('should not remove an article when not confirmed', () => {
      const removeSpy = spyOn(window, 'confirm').and.returnValue(false);
      component.ownedArticles = mockArticles;

      component.removeArticle(0);
      expect(removeSpy).toHaveBeenCalled();
      expect(articleService.updateArticleStatus).not.toHaveBeenCalled();
      expect(component.ownedArticles.length).toBe(2);
    });
  });

});
