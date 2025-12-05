import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TransactionsManagerComponent } from './transactions-manager.component';
import {TransactionService} from './services/transaction.service';
import {Transaction} from './models/transaction';
import {of} from 'rxjs';

describe('TransactionsManagerComponent', () => {
  let component: TransactionsManagerComponent;
  let fixture: ComponentFixture<TransactionsManagerComponent>;
  let mockTransactionService: jasmine.SpyObj<TransactionService>;

  beforeEach(async () => {
    mockTransactionService = jasmine.createSpyObj('TransactionService', ['create']);

    await TestBed.configureTestingModule({
      declarations: [ TransactionsManagerComponent ],
      providers: [
        { provide: TransactionService, useValue: mockTransactionService }
      ]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TransactionsManagerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should create a transaction and add it to the transactions array', () => {
    const newTransaction: Transaction = {
      transactionId: 1,
      buyerId: 2,
      sellerId: 3,
      articleId: 4,
      transactionType: 'purchase',
      price: 100,
      commission: 10,
      transactionDate: new Date().toISOString(),
      status: 'in progress'
    };

    // Mock the create method to return an observable with the transaction
    mockTransactionService.create.and.returnValue(of(newTransaction));

    // Call createTransaction method
    component.createTransaction(newTransaction);

    // Check if the transaction has been added to the transactions array
    expect(component.transactions.length).toBe(1);
    expect(component.transactions[0]).toEqual(newTransaction);
  });

  it('should call ngOnInit', () => {
    const ngOnInitSpy = spyOn(component, 'ngOnInit');
    component.ngOnInit();
    expect(ngOnInitSpy).toHaveBeenCalled();
  });

  it('should call ngOnDestroy', () => {
    const ngOnDestroySpy = spyOn(component, 'ngOnDestroy');
    component.ngOnDestroy();
    expect(ngOnDestroySpy).toHaveBeenCalled();
  });
});
