import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TradeComponent } from './trade.component';
import {ArticleService} from '../articles-manager/services/article.service';
import {UserService} from '../../../core/user/user-manager/services/user.service';
import {TradeService} from './services/trade.service';
import {ActivatedRoute, Router} from '@angular/router';
import {HttpClientTestingModule} from '@angular/common/http/testing';
import {of} from 'rxjs';
import {Article} from '../articles-manager/models/article';

describe('TradeComponent', () => {
  let component: TradeComponent;
  let fixture: ComponentFixture<TradeComponent>;
  let mockArticleService: jasmine.SpyObj<ArticleService>;
  let mockUserService: jasmine.SpyObj<UserService>;
  let mockTradeService: jasmine.SpyObj<TradeService>;
  let mockRouter: jasmine.SpyObj<Router>;
  let mockActivatedRoute: any;

  const mockUserId = '1';
  const mockOwnedArticles = [
    { articleId: 1, title: 'Article 1', description: 'Desc 1', price: 100, category: 'Electronics', state: 'new', userId: 1, createdAt: '', updatedAt: '', status: 'available', mainImageUrl: 'image1.png', additionalImages: '', quantity: 1 },
    { articleId: 2, title: 'Article 2', description: 'Desc 2', price: 200, category: 'Books', state: 'used', userId: 1, createdAt: '', updatedAt: '', status: 'available', mainImageUrl: 'image2.png', additionalImages: '', quantity: 2 }
  ];


  const mockWantedArticle = {
    articleId: 3,
    title: 'Wanted Article',
    description: 'Desc 3',
    price: 300,
    category: 'Fashion',
    state: 'new',
    userId: 2,
    createdAt: '',
    updatedAt: '',
    status: 'available',
    mainImageUrl: 'wantedImage.png',
    additionalImages: '',
    quantity: 1
  };

  beforeEach(async () => {
    mockArticleService = jasmine.createSpyObj('ArticleService', ['getAll', 'getArticleById']);
    mockUserService = jasmine.createSpyObj('UserService', ['getUserIdFromToken']);
    mockTradeService = jasmine.createSpyObj('TradeService', ['createTrade']);
    mockRouter = jasmine.createSpyObj('Router', ['navigate']);

    mockActivatedRoute = {
      snapshot: {
        paramMap: {
          get: jasmine.createSpy().and.returnValue('3')
        }
      }
    };

    await TestBed.configureTestingModule({
      declarations: [TradeComponent],
      imports: [HttpClientTestingModule],
      providers: [
        { provide: ArticleService, useValue: mockArticleService },
        { provide: UserService, useValue: mockUserService },
        { provide: TradeService, useValue: mockTradeService },
        { provide: Router, useValue: mockRouter },
        { provide: ActivatedRoute, useValue: mockActivatedRoute }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(TradeComponent);
    component = fixture.componentInstance;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });



  describe('validateTrade()', () => {
    let confirmSpy: jasmine.Spy;

    // Mock data (corrected with valid state values)
    const mockUserId = '1';
    const mockOwnedArticles: Article[] = [
      {
        articleId: 1,
        title: 'Article 1',
        description: 'Test description',
        price: 100,
        category: 'Test category',
        state: 'new', // ✅ Correct value
        userId: 1,
        createdAt: '2024-12-20T00:00:00.000Z',
        updatedAt: '2024-12-20T00:00:00.000Z',
        status: 'available',
        mainImageUrl: 'test-url',
        additionalImages: 'test-url',
        quantity: 1
      }
    ];

    const mockWantedArticle: Article = {
      articleId: 2,
      title: 'Wanted Article',
      description: 'Test description',
      price: 150,
      category: 'Test category',
      state: 'used', // ✅ Correct value
      userId: 2,
      createdAt: '2024-12-20T00:00:00.000Z',
      updatedAt: '2024-12-20T00:00:00.000Z',
      status: 'available',
      mainImageUrl: 'test-url',
      additionalImages: 'test-url',
      quantity: 1
    };

    beforeEach(() => {
      component.userId = mockUserId;
      component.wantedArticle = mockWantedArticle;
      component.addArticleToSelection(mockOwnedArticles[0]);
      confirmSpy = spyOn(window, 'confirm');
    });

    it('should create a trade if all conditions are met', () => {
      confirmSpy.and.returnValue(true); // Simulate user clicking OK

      component.validateTrade();

      expect(confirmSpy).toHaveBeenCalledWith('Are you sure you want to trade for Wanted Article?');
      expect(mockTradeService.createTrade).toHaveBeenCalledWith(jasmine.objectContaining({
        traderId: parseInt(mockUserId, 10),
        receiverId: mockWantedArticle.userId,
        traderArticlesIds: '1',
        receiverArticleId: mockWantedArticle.articleId,
        status: 'in progress'
      }));
    });


    it('should not create a trade if no articles are selected', () => {
      component.selectedOwnArticles = [];
      confirmSpy.and.returnValue(true);

      component.validateTrade();

      expect(confirmSpy).not.toHaveBeenCalled();
      expect(mockTradeService.createTrade).not.toHaveBeenCalled();
    });

    it('should not create a trade if no wanted article is selected', () => {
      component.wantedArticle = undefined;
      confirmSpy.and.returnValue(true);

      component.validateTrade();

      expect(confirmSpy).not.toHaveBeenCalled();
      expect(mockTradeService.createTrade).not.toHaveBeenCalled();
    });

  });
});
