import { TestBed } from '@angular/core/testing';

import { EventBusService } from './event-bus.service';
import {EventBus} from './event-bus';

describe('EventBusService', () => {
  let eventBusService: EventBusService;
  let mockEvent: EventBus;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [EventBusService],
    });
    eventBusService = TestBed.inject(EventBusService);
    mockEvent = { name: 'USER_UPDATED', object: { userId: 1, username: 'user1' } };
  });

  it('should be created', () => {
    expect(eventBusService).toBeTruthy();
  });

  it('should publish an event and notify listeners', (done) => {
    eventBusService.listen('USER_UPDATED').subscribe((event) => {
      expect(event).toEqual(mockEvent);
      done(); // Mark the test as complete once the expectation is met
    });

    // Simulate publishing an event
    eventBusService.publish(mockEvent);
  });

  it('should not notify listeners for different event names', (done) => {
    const spy = jasmine.createSpy();

    eventBusService.listen('USER_DELETED').subscribe(spy);

    // Simulate publishing an unrelated event
    eventBusService.publish(mockEvent);

    setTimeout(() => {
      expect(spy).not.toHaveBeenCalled(); // It should not have been called because the event names do not match
      done();
    }, 100);
  });

  it('should handle multiple subscribers', (done) => {
    const spy1 = jasmine.createSpy();
    const spy2 = jasmine.createSpy();

    eventBusService.listen('USER_UPDATED').subscribe(spy1);
    eventBusService.listen('USER_UPDATED').subscribe(spy2);

    eventBusService.publish(mockEvent);

    setTimeout(() => {
      expect(spy1).toHaveBeenCalledWith(mockEvent);
      expect(spy2).toHaveBeenCalledWith(mockEvent);
      done();
    }, 100);
  });
});
