import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TradeReceiverComponent } from './trade-receiver.component';
import {TradeService} from '../../services/trade.service';
import {UserService} from '../../../../../core/user/user-manager/services/user.service';
import {ArticleService} from '../../../articles-manager/services/article.service';
import {Trade} from '../../models/Trade';
import {HttpClientTestingModule} from '@angular/common/http/testing';
import {of, throwError} from 'rxjs';

describe('TradeReceiverComponent', () => {
  let component: TradeReceiverComponent;
  let fixture: ComponentFixture<TradeReceiverComponent>;
  let tradeService: TradeService;
  let userService: UserService;
  let articleService: ArticleService;

  const mockTrades: Trade[] = [
    {
      tradeId: 1,
      traderId: 101,
      receiverId: 102,
      traderArticlesIds: '1,2',
      receiverArticleId: 3,
      tradeDate: '2024-12-20T00:00:00Z',
      status: 'in progress',
    },
    {
      tradeId: 2,
      traderId: 103,
      receiverId: 102,
      traderArticlesIds: '4',
      receiverArticleId: 5,
      tradeDate: '2024-12-19T00:00:00Z',
      status: 'in progress',
    },
  ];

  const mockUserId = '102';

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      declarations: [TradeReceiverComponent],
      providers: [TradeService, UserService, ArticleService],
    });

    fixture = TestBed.createComponent(TradeReceiverComponent);
    component = fixture.componentInstance;
    tradeService = TestBed.inject(TradeService);
    userService = TestBed.inject(UserService);
    articleService = TestBed.inject(ArticleService);

    spyOn(userService, 'getUserIdFromToken').and.returnValue(mockUserId);
  });

  describe('ngOnInit', () => {
    it('should fetch and filter trades based on user ID and status', () => {
      spyOn(tradeService, 'getAll').and.returnValue(of(mockTrades));

      component.ngOnInit();

      expect(tradeService.getAll).toHaveBeenCalled();
      expect(component.trades.length).toBe(2);
      expect(component.trades[0].tradeId).toBe(1);
      expect(component.trades[1].tradeId).toBe(2);
    });

    it('should handle the case when no userId is available', () => {
      spyOn(userService, 'getUserIdFromToken').and.returnValue(null);

      component.ngOnInit();

      expect(component.trades.length).toBe(0);
      console.log('Problem with the user ID');
    });
  });

  describe('acceptTrade', () => {
    it('should accept the trade and update the trades list', () => {
      spyOn(tradeService, 'updateTradeStatus').and.returnValue(of({}));
      const tradeToAccept = mockTrades[0];

      component.acceptTrade(tradeToAccept);

      expect(tradeService.updateTradeStatus).toHaveBeenCalledWith(tradeToAccept.tradeId, 'accepted');
      expect(component.trades.length).toBe(1); // One trade should be removed
      expect(component.trades[0].tradeId).toBe(2); // Only the second trade remains
    });

    it('should show an error alert if accepting a trade fails', () => {
      spyOn(tradeService, 'updateTradeStatus').and.returnValue(throwError('Failed to update trade status'));
      spyOn(window, 'alert'); // To spy on alert calls
      const tradeToAccept = mockTrades[0];

      component.acceptTrade(tradeToAccept);

      expect(tradeService.updateTradeStatus).toHaveBeenCalledWith(tradeToAccept.tradeId, 'accepted');
      expect(window.alert).toHaveBeenCalledWith('An error occurred while accepting the trade. Please try again.');
    });
  });

  describe('declineTrade', () => {
    it('should decline the trade and update the trades list', () => {
      spyOn(tradeService, 'updateTradeStatus').and.returnValue(of({}));
      const tradeToDecline = mockTrades[0];

      component.declineTrade(tradeToDecline);

      expect(tradeService.updateTradeStatus).toHaveBeenCalledWith(tradeToDecline.tradeId, 'denied');
      expect(component.trades.length).toBe(1); // One trade should be removed
      expect(component.trades[0].tradeId).toBe(2); // Only the second trade remains
    });

    it('should show an error alert if declining a trade fails', () => {
      spyOn(tradeService, 'updateTradeStatus').and.returnValue(throwError('Failed to update trade status'));
      spyOn(window, 'alert'); // To spy on alert calls
      const tradeToDecline = mockTrades[0];

      component.declineTrade(tradeToDecline);

      expect(tradeService.updateTradeStatus).toHaveBeenCalledWith(tradeToDecline.tradeId, 'denied');
      expect(window.alert).toHaveBeenCalledWith('An error occurred while denying the trade. Please try again.');
    });
  });

  describe('getTitle', () => {
    it('should return the article title when it is available', () => {
      const mockArticleTitle = 'Mock Article Title';
      spyOn(articleService, 'getTitle').and.returnValue(of(mockArticleTitle));
      const articleId = 1;

      component.getTitle(articleId).subscribe((title) => {
        expect(title).toBe(mockArticleTitle);
      });

      expect(articleService.getTitle).toHaveBeenCalledWith(articleId);
    });

    it('should handle errors and return "Unknown Article" if the title is not found', () => {
      spyOn(articleService, 'getTitle').and.returnValue(throwError('Failed to load title'));
      const articleId = 1;

      component.getTitle(articleId).subscribe((title) => {
        expect(title).toBe('Unknown Article');
      });

      expect(articleService.getTitle).toHaveBeenCalledWith(articleId);
    });
  });

  describe('getUsername', () => {
    it('should return the username when it is available', () => {
      const mockUsername = 'Mock Username';
      spyOn(userService, 'getUsername').and.returnValue(of(mockUsername));
      const userId = 102;

      component.getUsername(userId).subscribe((username) => {
        expect(username).toBe(mockUsername);
      });

      expect(userService.getUsername).toHaveBeenCalledWith(userId);
    });

    it('should handle errors and return "Unknown User" if the username is not found', () => {
      spyOn(userService, 'getUsername').and.returnValue(throwError('Failed to load username'));
      const userId = 102;

      component.getUsername(userId).subscribe((username) => {
        expect(username).toBe('Unknown User');
      });

      expect(userService.getUsername).toHaveBeenCalledWith(userId);
    });
  });
});
