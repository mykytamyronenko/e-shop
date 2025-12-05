import { TestBed } from '@angular/core/testing';

import { TradeService } from './trade.service';
import {HttpClientTestingModule, HttpTestingController} from '@angular/common/http/testing';
import {Trade} from '../models/Trade';
import {API_URLS, environment} from '../../../../../environments/environment';

describe('TradeService', () => {
  let service: TradeService;
  let httpMock: HttpTestingController;

  // Mock data for Trade
  const mockTrade: Trade = {
    tradeId: 1,
    traderId: 1,
    receiverId: 2,
    traderArticlesIds: '1,2',
    receiverArticleId: 3,
    tradeDate: '2024-12-20T00:00:00Z',
    status: 'in progress'
  };

  const mockTradeResponse: Trade = { ...mockTrade, tradeId: 1 };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [TradeService]
    });

    service = TestBed.inject(TradeService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify(); // Ensures no pending HTTP requests after each test
  });

  // Test getAll method
  describe('getAll()', () => {
    it('should fetch all trades from the API', () => {
      service.getAll().subscribe((trades: Trade[]) => {
        expect(trades).toEqual([mockTrade]);
      });

      // Mock the response from the server
      const req = httpMock.expectOne(`${environment.BASE_URL}/${API_URLS.TRADE}`);
      expect(req.request.method).toBe('GET');
      req.flush([mockTrade]);
    });
  });

  // Test createTrade method
  describe('createTrade()', () => {
    it('should create a trade and return it', () => {
      service.createTrade(mockTrade).subscribe((response: Trade) => {
        expect(response).toEqual(mockTradeResponse);
      });

      const req = httpMock.expectOne(`${environment.BASE_URL}/${API_URLS.TRADE}`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(mockTrade);
      req.flush(mockTradeResponse); // Simulate response
    });
  });

  // Test updateTradeStatus method
  describe('updateTradeStatus()', () => {
    it('should update the trade status and return the updated trade', () => {
      const updatedTrade: Trade = { ...mockTrade, status: 'accepted' };
      const tradeId = 1;
      const status: 'accepted' = 'accepted';

      service.updateTradeStatus(tradeId, status).subscribe((response: any) => {
        expect(response.status).toBe('accepted');
        expect(response.tradeId).toBe(tradeId);
      });

      const req = httpMock.expectOne(`${environment.BASE_URL}/${API_URLS.TRADE}/${tradeId}/status`);
      expect(req.request.method).toBe('PUT');
      expect(req.request.body).toEqual({ status });
      req.flush({ status: 'accepted', tradeId }); // Simulate response
    });

    it('should handle a denied status update', () => {
      const updatedTrade: Trade = { ...mockTrade, status: 'denied' };
      const tradeId = 1;
      const status: 'denied' = 'denied';

      service.updateTradeStatus(tradeId, status).subscribe((response: any) => {
        expect(response.status).toBe('denied');
        expect(response.tradeId).toBe(tradeId);
      });

      const req = httpMock.expectOne(`${environment.BASE_URL}/${API_URLS.TRADE}/${tradeId}/status`);
      expect(req.request.method).toBe('PUT');
      expect(req.request.body).toEqual({ status });
      req.flush({ status: 'denied', tradeId }); // Simulate response
    });
  });
});
