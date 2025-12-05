import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TransactionHistoryComponent } from './transaction-history.component';
import {TransactionService} from '../../services/transaction.service';
import {UserService} from '../../../../../core/user/user-manager/services/user.service';
import {RatingService} from '../../../../rating/services/rating.service';
import {ArticleService} from '../../../../articles/articles-manager/services/article.service';
import {of, throwError} from 'rxjs';

describe('TransactionHistoryComponent', () => {
  let component: TransactionHistoryComponent;
  let fixture: ComponentFixture<TransactionHistoryComponent>;

  let mockTransactionService: jasmine.SpyObj<TransactionService>;
  let mockUserService: jasmine.SpyObj<UserService>;
  let mockRatingService: jasmine.SpyObj<RatingService>;
  let mockArticleService: jasmine.SpyObj<ArticleService>;

  const mockTransactions = [
    { transactionId: 1, buyerId: 123, sellerId: 456, articleId: 789, transactionType: 'purchase', price: 100, commission: 10, transactionDate: '2024-01-01', status: 'in progress' },
    { transactionId: 2, buyerId: 123, sellerId: 789, articleId: 456, transactionType: 'exchange', price: 200, commission: 20, transactionDate: '2024-02-01', status: 'finished' }
  ];

  const mockRatings = [
    { userId: 789, reviewerId: 123, articleId: 456, score: 5, comment: 'Excellent!', createdAt: '2024-01-02' }
  ];

  const mockUserId = '123';

  beforeEach(() => {
    mockTransactionService = jasmine.createSpyObj('TransactionService', ['getAll']);
    mockUserService = jasmine.createSpyObj('UserService', ['getUserIdFromToken', 'getUsername']);
    mockRatingService = jasmine.createSpyObj('RatingService', ['getAll', 'create']);
    mockArticleService = jasmine.createSpyObj('ArticleService', ['getTitle']);

    mockUserService.getUserIdFromToken.and.returnValue(mockUserId);
    mockRatingService.getAll.and.returnValue(of(mockRatings));
    mockArticleService.getTitle.and.returnValue(of('Sample Article Title'));

    TestBed.configureTestingModule({
      declarations: [TransactionHistoryComponent],
      providers: [
        { provide: TransactionService, useValue: mockTransactionService },
        { provide: UserService, useValue: mockUserService },
        { provide: RatingService, useValue: mockRatingService },
        { provide: ArticleService, useValue: mockArticleService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(TransactionHistoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch and filter transactions for the logged-in user on ngOnInit', () => {
    component.ngOnInit();
    expect(component.transactions.length).toBe(2);
    expect(component.transactions[0].buyerId).toBe(123);
    expect(component.transactions[1].buyerId).toBe(123);
  });

  it('should fetch ratings on ngOnInit', () => {
    component.ngOnInit();
    expect(component.ratings.length).toBe(1);
    expect(component.ratings[0].reviewerId).toBe(123);
  });

  it('should check if a rating already exists for a given seller', () => {
    component.ratings = mockRatings;
    const exists = component.existingRating(789);
    expect(exists).toBeTrue();
  });

  it('should not send a rating if a rating already exists for the seller', () => {
    component.ratings = mockRatings;
    spyOn(window, 'confirm').and.returnValue(true);  // Simulate user confirming the rating
    component.sendRating(456, 789);  // Rating already exists
    expect(mockRatingService.create).not.toHaveBeenCalled();
  });

  it('should send a rating if one does not already exist', () => {
    component.ratings = [];
    spyOn(window, 'confirm').and.returnValue(true);  // Simulate user confirming the rating

    const scoreInput = document.createElement('input');
    scoreInput.type = 'radio';
    scoreInput.name = 'numberOfStar_456';
    scoreInput.value = '5';
    document.body.appendChild(scoreInput);

    const commentInput = document.createElement('input');
    commentInput.id = 'commentId-456';
    commentInput.value = 'Great article!';
    document.body.appendChild(commentInput);

    component.sendRating(456, 789);

    expect(mockRatingService.create).toHaveBeenCalled();
  });

  it('should return the username for a given userId from the UserService', () => {
    component.getUsername(789).subscribe((username) => {
      expect(username).toBe('Sample User');
    });
    mockUserService.getUsername.and.returnValue(of('Sample User'));
  });

  it('should handle error while fetching username', () => {
    component.getUsername(789).subscribe((username) => {
      expect(username).toBe('Unknown User');
    });
    mockUserService.getUsername.and.returnValue(throwError('error'));
  });

  it('should return the article title for a given articleId', () => {
    component.getTitle(456).subscribe((title) => {
      expect(title).toBe('Sample Article Title');
    });
  });

  it('should handle error while fetching article title', () => {
    component.getTitle(456).subscribe((title) => {
      expect(title).toBe('Unknown Article');
    });
    mockArticleService.getTitle.and.returnValue(throwError('error'));
  });
});
