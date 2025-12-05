import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserManagerComponent } from './user-manager.component';
import {UserService} from './services/user.service';
import {User} from './models/user';
import {of, throwError} from 'rxjs';

describe('UserManagerComponent', () => {
  let component: UserManagerComponent;
  let fixture: ComponentFixture<UserManagerComponent>;
  let mockUserService: jasmine.SpyObj<UserService>;

  beforeEach(async () => {
    // Mock the UserService
    mockUserService = jasmine.createSpyObj('UserService', ['createUser']);

    // Configure the testing module
    await TestBed.configureTestingModule({
      declarations: [UserManagerComponent],
      providers: [
        { provide: UserService, useValue: mockUserService },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(UserManagerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should create a new user and add to the users array', () => {
    const newUser: User = {
      userId: 1,
      username: 'newuser',
      email: 'newuser@example.com',
      password: 'password123',
      profilePicture: '',
      membershipLevel: 'Gold',
      balance: 100,
    };

    // Mock the createUser method to simulate a successful user creation
    mockUserService.createUser.and.returnValue(of(newUser));

    // Call the method to create a new user
    component.createUser({ user: newUser });

    // Check if the user is added to the users array
    expect(component.users.length).toBe(1);
    expect(component.users[0].username).toBe('newuser');
  });

  it('should show an alert if the username already exists', () => {
    const newUser: User = {
      userId: 1,
      username: 'existinguser',
      email: 'existinguser@example.com',
      password: 'password123',
      profilePicture: '',
      membershipLevel: 'Gold',
      balance: 100,
    };

    // Mock the createUser method to simulate an error (username already exists)
    mockUserService.createUser.and.returnValue(throwError(() => new Error('This username already exists.')));

    // Spy on window.alert to capture the alert call
    spyOn(window, 'alert');

    // Call the method to create a user
    component.createUser({ user: newUser });

    // Verify that the alert was called with the expected message
    expect(window.alert).toHaveBeenCalledWith('This username already exists.');
  });
});
