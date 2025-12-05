import { TestBed } from '@angular/core/testing';

import { ArticleService } from './article.service';
import {HttpClientTestingModule, HttpTestingController} from '@angular/common/http/testing';
import {UserService} from '../../../../core/user/user-manager/services/user.service';
import {Article} from '../models/article';

describe('ArticleService', () => {
  let service: ArticleService;
  let httpMock: HttpTestingController;
  let userService: jasmine.SpyObj<UserService>;

  const mockArticle: Article = {
    articleId: 1,
    title: 'Test Article',
    description: 'Test Description',
    price: 100,
    category: 'Test Category',
    state: 'new',
    userId: 1,
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-02T00:00:00Z',
    status: 'available',
    mainImageUrl: '',
    additionalImages: '',
    quantity: 1,
  };

  beforeEach(() => {
    const userServiceSpy = jasmine.createSpyObj('UserService', ['getUserIdFromToken']);

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        ArticleService,
        { provide: UserService, useValue: userServiceSpy }
      ]
    });

    service = TestBed.inject(ArticleService);
    httpMock = TestBed.inject(HttpTestingController);
    userService = TestBed.inject(UserService) as jasmine.SpyObj<UserService>;
  });

  describe('getAll()', () => {
    it('should fetch all articles', () => {
      service.getAll().subscribe((articles) => {
        expect(articles).toEqual([mockArticle]);
      });

      const req = httpMock.expectOne(ArticleService.URL);
      expect(req.request.method).toBe('GET');
      req.flush([mockArticle]);
    });
  });

  describe('createArticle()', () => {
    it('should create an article without an image', () => {
      const article = { ...mockArticle, imageFile: undefined };
      service.createArticle(article).subscribe((createdArticle) => {
        expect(createdArticle).toEqual(mockArticle);
      });

      const req = httpMock.expectOne(ArticleService.URL);
      expect(req.request.method).toBe('POST');
      expect(req.request.body.has('Title')).toBeTrue();
      req.flush(mockArticle);
    });

    it('should create an article with an image', () => {
      const imageFile = new File(['dummy content'], 'test.jpg', { type: 'image/jpeg' });
      const article = { ...mockArticle, imageFile };
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
      formData.append('Image', imageFile);

      service.createArticle(article, imageFile).subscribe((createdArticle) => {
        expect(createdArticle).toEqual(mockArticle);
      });

      const req = httpMock.expectOne(ArticleService.URL);
      expect(req.request.method).toBe('POST');
      expect(req.request.body.has('Image')).toBeTrue();
      req.flush(mockArticle);
    });
  });

  describe('deleteArticle()', () => {
    it('should delete an article by ID', () => {
      service.deleteArticle(1).subscribe((response) => {
        expect(response).toBeUndefined();
      });

      const req = httpMock.expectOne(`${ArticleService.URL}/1`);
      expect(req.request.method).toBe('DELETE');
      req.flush({});
    });
  });

  describe('getArticleById()', () => {
    it('should return an article by ID', () => {
      service.getArticleById(1).subscribe((article) => {
        expect(article).toEqual(mockArticle);
      });

      const req = httpMock.expectOne(`${ArticleService.URL}/1`);
      expect(req.request.method).toBe('GET');
      req.flush(mockArticle);
    });
  });

  describe('setSelectedArticle() and getSelectedArticle()', () => {
    it('should set and get the selected article', () => {
      service.setSelectedArticle(mockArticle);

      service.getSelectedArticle().subscribe((article) => {
        expect(article).toEqual(mockArticle);
      });
    });
  });

  describe('updateArticle()', () => {
    it('should update the article', () => {
      const updatedArticle = { ...mockArticle, title: 'Updated Article' };
      service.updateArticle(updatedArticle).subscribe((article) => {
        expect(article).toEqual(updatedArticle);
      });

      const req = httpMock.expectOne(`${ArticleService.URL}/${updatedArticle.articleId}`);
      expect(req.request.method).toBe('PUT');
      req.flush(updatedArticle);
    });
  });

  describe('getTitle()', () => {
    it('should fetch the article title by ID', () => {
      const mockTitle = 'Test Article Title';
      service.getTitle(1).subscribe((title) => {
        expect(title).toBe(mockTitle);
      });

      const req = httpMock.expectOne('http://localhost:5185/api/articles/getTitle?id=1');
      expect(req.request.method).toBe('GET');
      req.flush({ title: mockTitle });
    });
  });

  describe('updateArticleStatus()', () => {
    it('should update the article status to removed', () => {
      service.updateArticleStatus(1, 'removed').subscribe((response) => {
        expect(response).toEqual(mockArticle);
      });

      const req = httpMock.expectOne(`${ArticleService.URL}/1/status`);
      expect(req.request.method).toBe('PUT');
      req.flush(mockArticle);
    });
  });

  describe('updateArticleStatusAdmin()', () => {
    it('should update the article status to removed by admin', () => {
      service.updateArticleStatusAdmin(1, 'removed').subscribe((response) => {
        expect(response).toEqual(mockArticle);
      });

      const req = httpMock.expectOne(`${ArticleService.URL}/1/admin/status`);
      expect(req.request.method).toBe('PUT');
      req.flush(mockArticle);
    });
  });

  afterEach(() => {
    httpMock.verify();
  });
});
