import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditUserProfileFormComponent } from './edit-user-profile-form.component';
import {EventBusService} from '../../../../../shared/services/event-bus/event-bus.service';
import {NO_ERRORS_SCHEMA} from '@angular/core';
import {User} from '../../models/user';

describe('EditUserProfileFormComponent', () => {
  let component: EditUserProfileFormComponent;
  let fixture: ComponentFixture<EditUserProfileFormComponent>;
  let mockEventBusService: jasmine.SpyObj<EventBusService>;

  beforeEach(async () => {
    // Create a spy for EventBusService
    mockEventBusService = jasmine.createSpyObj('EventBusService', ['publish']);

    await TestBed.configureTestingModule({
      declarations: [ EditUserProfileFormComponent ],
      providers: [
        { provide: EventBusService, useValue: mockEventBusService }
      ],
      schemas: [ NO_ERRORS_SCHEMA ] // This prevents errors related to missing child components
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EditUserProfileFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should emit userUpdated event and publish USER_UPDATED when emitUserUpdated is called', () => {
    const user: User = { userId: 1, username: 'testUser', email: 'test@example.com', password: 'password', profilePicture: 'test.jpg', membershipLevel: 'Silver', balance: 100 };

    // Spy on the EventEmitter to check if the event is emitted
    spyOn(component.userUpdated, 'emit');

    // Call emitUserUpdated
    component.emitUserUpdated(user);

    // Check that userUpdated event is emitted
    expect(component.userUpdated.emit).toHaveBeenCalledWith(user);

    // Check that the EventBusService's publish method was called with the correct event
    expect(mockEventBusService.publish).toHaveBeenCalledWith({
      name: "USER_UPDATED",
      object: user
    });
  });

  it('should show an alert when emitUserUpdated is called', () => {
    const user: User = { userId: 1, username: 'testUser', email: 'test@example.com', password: 'password', profilePicture: 'test.jpg', membershipLevel: 'Silver', balance: 100 };

    // Spy on the alert function
    spyOn(window, 'alert');

    // Call emitUserUpdated
    component.emitUserUpdated(user);

    // Check that alert is called with the correct message
    expect(window.alert).toHaveBeenCalledWith('Account data updated successfully!');
  });
});
