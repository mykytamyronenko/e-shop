import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SellHistoryComponent } from './sell-history.component';
import {TransactionService} from '../../services/transaction.service';
import {UserService} from '../../../../../core/user/user-manager/services/user.service';
import {RatingService} from '../../../../rating/services/rating.service';
import {ArticleService} from '../../../../articles/articles-manager/services/article.service';
import {of} from 'rxjs';

describe('SellHistoryComponent', () => {
  let component: SellHistoryComponent;
  let fixture: ComponentFixture<SellHistoryComponent>;

  let transactionServiceMock: jasmine.SpyObj<TransactionService>;
  let userServiceMock: jasmine.SpyObj<UserService>;
  let ratingServiceMock: jasmine.SpyObj<RatingService>;
  let articleServiceMock: jasmine.SpyObj<ArticleService>;

  beforeEach(() => {
    // Create mock services
    transactionServiceMock = jasmine.createSpyObj('TransactionService', ['getAll']);
    userServiceMock = jasmine.createSpyObj('UserService', ['getUserIdFromToken', 'getUsername']);
    ratingServiceMock = jasmine.createSpyObj('RatingService', ['getAll', 'create']);
    articleServiceMock = jasmine.createSpyObj('ArticleService', ['getTitle']);

    // Provide mock services in the TestBed
    TestBed.configureTestingModule({
      declarations: [SellHistoryComponent],
      providers: [
        { provide: TransactionService, useValue: transactionServiceMock },
        { provide: UserService, useValue: userServiceMock },
        { provide: RatingService, useValue: ratingServiceMock },
        { provide: ArticleService, useValue: articleServiceMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(SellHistoryComponent);
    component = fixture.componentInstance;
  });

  it('should load transactions and ratings on init', () => {
    const userId = '1';
    const transactionsMock = [{ sellerId: 1, buyerId: 2, transactionId: 1, articleId: 1, price: 100, commission: 10, transactionDate: '2024-01-01', transactionType: 'purchase', status: 'finished' }];
    const ratingsMock = [{ userId: 1, reviewerId: 2, articleId: 1, score: 5, comment: 'Great!', createdAt: '2024-01-01' }];

    // Mock the service calls
    userServiceMock.getUserIdFromToken.and.returnValue(userId);
    ratingServiceMock.getAll.and.returnValue(of(ratingsMock));

    // Trigger ngOnInit
    component.ngOnInit();

    // Verify the expected behavior
    expect(component.transactions.length).toBe(1);
    expect(component.ratings.length).toBe(1);
    expect(transactionServiceMock.getAll).toHaveBeenCalled();
    expect(ratingServiceMock.getAll).toHaveBeenCalled();
  });

  it('should not load transactions if user is not authenticated', () => {
    userServiceMock.getUserIdFromToken.and.returnValue(null);

    // Trigger ngOnInit
    component.ngOnInit();

    // Verify that the transactions are not fetched
    expect(transactionServiceMock.getAll).not.toHaveBeenCalled();
  });

  it('should send a rating', () => {
    const sellerId = 1;
    const articleId = 1;
    const reviewerId = 2;
    const newRating = { userId: sellerId, reviewerId, articleId, score: 5, comment: 'Great product!', createdAt: new Date().toISOString() };

    const confirmSpy = spyOn(window, 'confirm').and.returnValue(true); // Mock confirmation dialog
    const scoreInput = { value: '5' };
    const commentInput = { value: 'Great product!' };

    // Mock DOM elements
    spyOn(document, 'querySelector').and.returnValue(scoreInput as any);
    spyOn(document, 'getElementById').and.returnValue(commentInput as any);

    ratingServiceMock.create.and.returnValue(of(newRating));

    // Call sendRating
    component.sendRating(articleId, sellerId);

    // Verify that the rating service was called
    expect(ratingServiceMock.create).toHaveBeenCalledWith(newRating);
    expect(component.ratings.length).toBe(1); // The new rating should be added to the list
  });

  it('should not send rating if user already rated the seller', () => {
    const sellerId = 1;
    const articleId = 1;
    const reviewerId = 2;

    // Mock existing rating
    component.ratings = [{ userId: sellerId, reviewerId: reviewerId, score: 5, comment: 'Great!', createdAt: '2024-01-01' }];

    // Call sendRating
    component.sendRating(articleId, sellerId);

    // Verify that the rating service was not called
    expect(ratingServiceMock.create).not.toHaveBeenCalled();
  });

  it('should return false if there is no existing rating for the seller', () => {
    const sellerId = 1;
    const reviewerId = 2;

    // No ratings exist
    component.ratings = [];

    const result = component.existingRating(sellerId);

    expect(result).toBe(false);
  });

  it('should return true if there is an existing rating for the seller', () => {
    const sellerId = 1;
    const reviewerId = 2;

    // Rating exists
    component.ratings = [{ userId: sellerId, reviewerId: reviewerId, score: 5, comment: 'Good seller', createdAt: '2024-01-01' }];

    const result = component.existingRating(sellerId);

    expect(result).toBe(true);
  });

  it('should call getTitle and return title for article', () => {
    const articleId = 1;
    const title = 'Article Title';
    articleServiceMock.getTitle.and.returnValue(of(title));

    component.getTitle(articleId).subscribe((returnedTitle) => {
      expect(returnedTitle).toBe(title);
    });

    expect(articleServiceMock.getTitle).toHaveBeenCalledWith(articleId);
  });

  afterEach(() => {
    fixture.destroy();
  });
});
