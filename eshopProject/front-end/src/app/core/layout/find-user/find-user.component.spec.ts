import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FindUserComponent } from './find-user.component';
import {UserService} from '../../user/user-manager/services/user.service';
import {NO_ERRORS_SCHEMA} from '@angular/core';
import {FindUser} from '../../user/user-manager/models/user';
import {of, throwError} from 'rxjs';

describe('FindUserComponent', () => {
  let component: FindUserComponent;
  let fixture: ComponentFixture<FindUserComponent>;
  let userServiceMock: jasmine.SpyObj<UserService>;

  beforeEach(() => {
    // Mocking the UserService
    userServiceMock = jasmine.createSpyObj('UserService', ['getUserByUsername']);

    TestBed.configureTestingModule({
      declarations: [FindUserComponent],
      providers: [
        { provide: UserService, useValue: userServiceMock },
      ],
      schemas: [NO_ERRORS_SCHEMA], // Ignore errors from unrecognized elements
    });

    fixture = TestBed.createComponent(FindUserComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should not proceed with empty search input', () => {
    spyOn(console, 'log');  // Spy on console.log to check the message
    component.submitSearch('');
    expect(console.log).toHaveBeenCalledWith('Username cannot be empty.');
    expect(component.isUserFound).toBeFalse();
  });

  it('should search for user and update component when user is found', () => {
    const mockUser: FindUser = {
      userId: 1,
      username: 'testUser',
      profilePicture: 'profile-pic.jpg',
      role: 'connected_user',
      membershipLevel: 'basic',
      rating: 4.5,
      status: 'active'
    };    userServiceMock.getUserByUsername.and.returnValue(of(mockUser));  // Mock successful response

    component.submitSearch('testUser');

    expect(userServiceMock.getUserByUsername).toHaveBeenCalledWith('testUser');
    expect(component.userFound).toEqual(mockUser);
    expect(component.profileImageUrl).toBe('http://localhost:5185/profile-pic.jpg');
    expect(component.isUserFound).toBeTrue();
  });

  it('should handle user not found error', () => {
    userServiceMock.getUserByUsername.and.returnValue(throwError('User not found'));  // Mock error

    spyOn(window, 'alert');  // Spy on alert to check if it is called

    component.submitSearch('nonExistingUser');

    expect(userServiceMock.getUserByUsername).toHaveBeenCalledWith('nonExistingUser');
    expect(component.isUserFound).toBeFalse();
    expect(window.alert).toHaveBeenCalledWith('user not found.');
  });

  it('should display user found and set profile image URL correctly', () => {
    const mockUser: FindUser = {
      userId: 1,
      username: 'testUser',
      profilePicture: 'profile-pic.jpg',
      role: 'connected_user',
      membershipLevel: 'basic',
      rating: 4.5,
      status: 'active'
    };    userServiceMock.getUserByUsername.and.returnValue(of(mockUser)); // Mock successful response

    component.submitSearch('validUser');

    expect(component.userFound).toEqual(mockUser);
    expect(component.profileImageUrl).toBe('http://localhost:5185/valid-profile.jpg');
    expect(component.isUserFound).toBeTrue();
  });
});
