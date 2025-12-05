import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ArticlesManagerComponent } from './articles-manager.component';
import {UserListComponent} from '../../../core/user/user-manager/components/user-list/user-list.component';
import {UserService} from '../../../core/user/user-manager/services/user.service';
import {NO_ERRORS_SCHEMA} from '@angular/core';
import {of} from 'rxjs';

describe('UserListComponent', () => {
  let component: UserListComponent;
  let fixture: ComponentFixture<UserListComponent>;
  let mockUserService: jasmine.SpyObj<UserService>;

  beforeEach(async () => {
    // Mock the UserService
    mockUserService = jasmine.createSpyObj('UserService', ['getAll', 'updateUserStatus']);

    await TestBed.configureTestingModule({
      declarations: [ UserListComponent ],
      providers: [
        { provide: UserService, useValue: mockUserService }
      ],
      schemas: [ NO_ERRORS_SCHEMA ] // This prevents errors related to missing child components
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UserListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should call getAll and filter users on ngOnInit', () => {
    // Mock users with the correct role types
    const mockUsers = [
      {
        userId: 1,
        username: 'user1',
        status: 'active',
        rating: 5,
        balance: 100,
        email: 'machin@gmail.com',
        role: 'admin', // Correct role (literal union type)
        password: 'password123',
        membershipLevel: 'Bronze',
        profilePicture: 'profile.jpg'
      },
      {
        userId: 2,
        username: 'user2',
        status: 'deleted',
        rating: 5,
        balance: 100,
        email: 'machin@gmail.com',
        role: 'admin', // Correct role (literal union type)
        password: 'password123',
        membershipLevel: 'Bronze',
        profilePicture: 'profile.jpg'
      },
      {
        userId: 3,
        username: 'user3',
        status: 'active',
        rating: 5,
        balance: 100,
        email: 'machin@gmail.com',
        role: 'admin', // Correct role (literal union type)
        password: 'password123',
        membershipLevel: 'Bronze',
        profilePicture: 'profile.jpg'
      }
    ];

    // Mock the getAll method to return the mock users
    // @ts-ignore
    mockUserService.getAll.and.returnValue(of(mockUsers));

    // Call ngOnInit manually
    component.ngOnInit();

    // Check that getAll was called
    expect(mockUserService.getAll).toHaveBeenCalled();

    // Check that the users are filtered correctly
    expect(component.users.length).toBe(2);  // Should only contain 'active' users
    expect(component.users).toEqual([
      {
        userId: 1,
        username: 'user1',
        status: 'active',
        rating: 5,
        balance: 100,
        email: 'machin@gmail.com',
        role: 'admin', // Correct role
        password: 'password123',
        membershipLevel: 'Bronze',
        profilePicture: 'profile.jpg'
      },
      {
        userId: 3,
        username: 'user3',
        status: 'active',
        rating: 5,
        balance: 100,
        email: 'machin@gmail.com',
        role: 'admin', // Correct role
        password: 'password123',
        membershipLevel: 'Bronze',
        profilePicture: 'profile.jpg'
      }
    ]);
  });

  it('should return the correct image URL', () => {
    const mockUrl = 'images/userProfile.png';
    const BASE_URL = 'http://localhost:5185/';

    const result = component.getImageUrl(mockUrl);

    expect(result).toBe(`${BASE_URL}${mockUrl}`);
  });

  it('should return a fallback image URL if the mainImageUrl is a string type', () => {
    const result = component.getImageUrl('string');

    expect(result).toBe('http://localhost:4200/test.png');
  });
});
