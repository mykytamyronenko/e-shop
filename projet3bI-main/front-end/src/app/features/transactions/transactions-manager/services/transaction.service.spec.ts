import { TestBed } from '@angular/core/testing';

import { TransactionService } from './transaction.service';
import {HttpClientTestingModule, HttpTestingController} from '@angular/common/http/testing';
import {Transaction} from '../models/transaction';
import {API_URLS, environment} from '../../../../../environments/environment';

describe('TransactionService', () => {
  let service: TransactionService;
  let httpMock: HttpTestingController;

  const mockTransactions: Transaction[] = [
    {
      transactionId: 1,
      buyerId: 2,
      sellerId: 3,
      articleId: 4,
      transactionType: 'purchase',
      price: 100,
      commission: 10,
      transactionDate: new Date().toISOString(),
      status: 'in progress',
    },
    {
      transactionId: 2,
      buyerId: 5,
      sellerId: 6,
      articleId: 7,
      transactionType: 'exchange',
      price: 200,
      commission: 20,
      transactionDate: new Date().toISOString(),
      status: 'finished',
    },
  ];

  const mockNewTransaction: Transaction = {
    transactionId: 3,
    buyerId: 8,
    sellerId: 9,
    articleId: 10,
    transactionType: 'purchase',
    price: 150,
    commission: 15,
    transactionDate: new Date().toISOString(),
    status: 'in progress',
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [TransactionService],
    });

    service = TestBed.inject(TransactionService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should fetch all transactions', () => {
    service.getAll().subscribe((transactions) => {
      expect(transactions.length).toBe(2);
      expect(transactions).toEqual(mockTransactions);
    });

    const req = httpMock.expectOne(`${environment.BASE_URL}/${API_URLS.TRANSACTION}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockTransactions); // Return mock data
  });

  it('should create a new transaction', () => {
    service.create(mockNewTransaction).subscribe((transaction) => {
      expect(transaction).toEqual(mockNewTransaction);
    });

    const req = httpMock.expectOne(`${environment.BASE_URL}/${API_URLS.TRANSACTION}`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(mockNewTransaction);
    req.flush(mockNewTransaction); // Return mock data
  });
});
