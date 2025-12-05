import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateTransactionComponent } from './create-transaction.component';
import {UserService} from '../../../../../core/user/user-manager/services/user.service';
import {ArticleService} from '../../../../articles/articles-manager/services/article.service';
import {FormBuilder, ReactiveFormsModule} from '@angular/forms';
import {Article} from '../../../../articles/articles-manager/models/article';
import {of} from 'rxjs';
import {Transaction} from '../../models/transaction';

describe('CreateTransactionComponent', () => {
  let component: CreateTransactionComponent;
  let fixture: ComponentFixture<CreateTransactionComponent>;
  let userServiceMock: jasmine.SpyObj<UserService>;
  let articleServiceMock: jasmine.SpyObj<ArticleService>;

  beforeEach(() => {
    // Create mocks for services
    userServiceMock = jasmine.createSpyObj('UserService', ['getUserIdFromToken']);
    articleServiceMock = jasmine.createSpyObj('ArticleService', ['getSelectedArticle']);

    TestBed.configureTestingModule({
      declarations: [CreateTransactionComponent],
      imports: [ReactiveFormsModule],
      providers: [
        FormBuilder,
        { provide: UserService, useValue: userServiceMock },
        { provide: ArticleService, useValue: articleServiceMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(CreateTransactionComponent);
    component = fixture.componentInstance;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  describe('loadSelectedArticle', () => {
    it('should load selected article and set properties', async () => {
      const mockArticle: Article = {
        articleId: 1,
        title: 'Test Article',
        description: 'Test Description',
        price: 100,
        category: 'Electronics',
        state: 'new',
        userId: 2,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
        status: 'available',
        mainImageUrl: 'http://example.com/image.jpg',
        additionalImages: 'http://example.com/image1.jpg,http://example.com/image2.jpg',
        quantity: 1
      };

      articleServiceMock.getSelectedArticle.and.returnValue(of(mockArticle));

      await component.loadSelectedArticle();

      expect(component.selectedArticle).toEqual(mockArticle);
      expect(component.sellerId).toBe(mockArticle.userId);
      expect(component.articleId).toBe(mockArticle.articleId);
      expect(component.price).toBe(mockArticle.price);
    });

    it('should handle error if getSelectedArticle fails', async () => {
      const consoleSpy = spyOn(console, 'error');
      articleServiceMock.getSelectedArticle.and.throwError('Error loading article');

      await component.loadSelectedArticle();

      expect(consoleSpy).toHaveBeenCalledWith('Error loading article:', jasmine.any(Error));
      expect(component.selectedArticle).toBeNull();
    });
  });

  describe('emitTransaction', () => {
    it('should emit a transaction if user is authenticated and article is selected', () => {
      const mockArticle: Article = {
        articleId: 1,
        title: 'Test Article',
        description: 'Test Description',
        price: 100,
        category: 'Electronics',
        state: 'new',
        userId: 2,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
        status: 'available',
        mainImageUrl: 'http://example.com/image.jpg',
        additionalImages: 'http://example.com/image1.jpg,http://example.com/image2.jpg',
        quantity: 1
      };

      const mockUserId = '10';
      userServiceMock.getUserIdFromToken.and.returnValue(mockUserId);
      component.selectedArticle = mockArticle;

      const emitSpy = spyOn(component.transactionCreated, 'emit');

      component.emitTransaction();

      const expectedTransaction: Transaction = {
        transactionId: 0,
        buyerId: parseInt(mockUserId, 10),
        sellerId: mockArticle.userId,
        articleId: mockArticle.articleId,
        transactionType: 'purchase',
        price: mockArticle.price,
        commission: component.form.value.commission,
        transactionDate: Date.now().toString(),
        status: 'finished'
      };

      expect(emitSpy).toHaveBeenCalledWith(jasmine.objectContaining(expectedTransaction));
    });

    it('should not emit a transaction if user is not authenticated', () => {
      userServiceMock.getUserIdFromToken.and.returnValue(null);
      const consoleSpy = spyOn(console, 'error');
      component.emitTransaction();

      expect(consoleSpy).toHaveBeenCalledWith('User not authenticated');
    });

    it('should not emit a transaction if no article is selected', () => {
      userServiceMock.getUserIdFromToken.and.returnValue('10');
      const warnSpy = spyOn(console, 'warn');
      component.emitTransaction();

      expect(warnSpy).toHaveBeenCalledWith('No items selected');
    });
  });

  describe('submitTransaction', () => {
    it('should call loadSelectedArticle and emitTransaction if no article is selected', async () => {
      const loadSelectedArticleSpy = spyOn(component, 'loadSelectedArticle').and.callThrough();
      const emitTransactionSpy = spyOn(component, 'emitTransaction');

      const mockUserId = '10';
      userServiceMock.getUserIdFromToken.and.returnValue(mockUserId);

      const mockArticle: Article = {
        articleId: 1,
        title: 'Test Article',
        description: 'Test Description',
        price: 100,
        category: 'Electronics',
        state: 'new',
        userId: 2,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString(),
        status: 'available',
        mainImageUrl: 'http://example.com/image.jpg',
        additionalImages: 'http://example.com/image1.jpg,http://example.com/image2.jpg',
        quantity: 1
      };

      articleServiceMock.getSelectedArticle.and.returnValue(of(mockArticle));

      await component.submitTransaction();

      expect(loadSelectedArticleSpy).toHaveBeenCalled();
      expect(emitTransactionSpy).toHaveBeenCalled();
    });

    it('should log an error if user is not authenticated', async () => {
      const consoleSpy = spyOn(console, 'error');
      userServiceMock.getUserIdFromToken.and.returnValue(null);

      await component.submitTransaction();

      expect(consoleSpy).toHaveBeenCalledWith('User not authenticated');
    });
  });
});
