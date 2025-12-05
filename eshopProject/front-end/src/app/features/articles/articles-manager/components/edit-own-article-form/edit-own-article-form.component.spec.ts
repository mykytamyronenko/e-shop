import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditOwnArticleFormComponent } from './edit-own-article-form.component';
import {ArticleService} from '../../services/article.service';
import {ActivatedRoute, Router} from '@angular/router';
import {Article} from '../../models/article';
import {RouterTestingModule} from '@angular/router/testing';
import {of, throwError} from 'rxjs';

describe('EditOwnArticleFormComponent', () => {
  let component: EditOwnArticleFormComponent;
  let fixture: ComponentFixture<EditOwnArticleFormComponent>;
  let articleService: jasmine.SpyObj<ArticleService>;
  let router: jasmine.SpyObj<Router>;
  let activatedRoute: jasmine.SpyObj<ActivatedRoute>;

  const mockArticle: Article = {
    articleId: 1,
    title: 'Test Article',
    description: 'Test Description',
    price: 100,
    category: 'Electronics',
    state: 'used',
    status: 'available',
    userId: 1,
    createdAt: '2024-01-01T00:00:00Z',
    updatedAt: '2024-01-01T00:00:00Z',
    mainImageUrl: 'test-image.jpg',
    additionalImages: '',
    quantity: 1,
  };

  beforeEach(() => {
    const articleServiceSpy = jasmine.createSpyObj('ArticleService', ['getArticleById', 'updateArticle']);
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    const activatedRouteSpy = jasmine.createSpyObj('ActivatedRoute', ['snapshot']);

    // Mock the paramMap with a spy for the 'get' method
    const paramMapMock = jasmine.createSpyObj('paramMap', ['get']);
    paramMapMock.get.and.returnValue('1');  // Simulating the route parameter

    // Attach the paramMapMock to the snapshot object
    activatedRouteSpy.snapshot = { paramMap: paramMapMock };

    TestBed.configureTestingModule({
      declarations: [EditOwnArticleFormComponent],
      imports: [RouterTestingModule],
      providers: [
        { provide: ArticleService, useValue: articleServiceSpy },
        { provide: Router, useValue: routerSpy },
        { provide: ActivatedRoute, useValue: activatedRouteSpy },
      ]
    });

    fixture = TestBed.createComponent(EditOwnArticleFormComponent);
    component = fixture.componentInstance;
    articleService = TestBed.inject(ArticleService) as jasmine.SpyObj<ArticleService>;
    router = TestBed.inject(Router) as jasmine.SpyObj<Router>;
    activatedRoute = TestBed.inject(ActivatedRoute) as jasmine.SpyObj<ActivatedRoute>;

    // Simulate route parameter retrieval
    paramMapMock.get.and.returnValue('1');  // Ensure '1' is returned when `get()` is called

    articleService.getArticleById.and.returnValue(of(mockArticle));

    fixture.detectChanges();
  });



  describe('ngOnInit()', () => {
    it('should fetch the article based on the route parameter and populate ownArticle', () => {
      component.ngOnInit();
      expect(articleService.getArticleById).toHaveBeenCalledWith(1);
      expect(component.ownArticle).toEqual(mockArticle);
    });

    it('should handle error when article retrieval fails', () => {
      articleService.getArticleById.and.returnValue(throwError(() => new Error('Failed to fetch article')));
      component.ngOnInit();
      expect(component.ownArticle).toEqual({
        articleId: 0, title: "", description: "", price: 0, category: "Other", state: "used", status: "available",
        userId: 0, createdAt: '', updatedAt: '', mainImageUrl: '', additionalImages: '', quantity: 0
      });
    });
  });

  describe('submitChange()', () => {
    it('should call the updateArticle method and navigate to profile page on success', () => {
      articleService.updateArticle.and.returnValue(of(mockArticle));
      component.submitChange();
      expect(articleService.updateArticle).toHaveBeenCalledWith(mockArticle);
      expect(router.navigate).toHaveBeenCalledWith(['/profile-page']);
      expect(window.alert).toHaveBeenCalledWith('Article successfully updated!');
    });

    it('should handle error and show an alert when the update fails', () => {
      articleService.updateArticle.and.returnValue(throwError(() => new Error('Failed to update article')));
      spyOn(window, 'alert'); // Spy on the alert method
      component.submitChange();
      expect(articleService.updateArticle).toHaveBeenCalledWith(mockArticle);
      expect(window.alert).toHaveBeenCalledWith('There was an error updating the article.');
    });
  });

  describe('Categories', () => {
    it('should have a predefined list of categories', () => {
      expect(component.categories).toEqual(['Electronics', 'Books', 'Clothing', 'Furnishing', 'Toys', 'Sports', 'Beauty', 'Food', 'Vehicles', 'Other']);
    });
  });

});
