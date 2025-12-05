import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserListComponent } from './user-list.component';
import {UserService} from '../../services/user.service';
import {of} from 'rxjs';
import {AdminUser} from '../../models/user';

describe('UserListComponent', () => {
  let component: UserListComponent;
  let fixture: ComponentFixture<UserListComponent>;
  let mockUserService: jasmine.SpyObj<UserService>;
// Mock data with all the required fields
  const mockUsers: AdminUser[] = [
    {
      userId: 1,
      username: 'user1',
      status: 'active',
      role: 'admin',
      email: 'user1@example.com',
      password: 'password123',
      profilePicture: 'user1-profile.jpg',
      membershipLevel: 'Bronze',
      balance:50,
      rating:5,
    },
    {
      userId: 2,
      username: 'user2',
      status: 'deleted',
      role: 'admin',
      email: 'user2@example.com',
      password: 'password456',
      profilePicture: 'user2-profile.jpg',
      membershipLevel: 'Silver',
      balance:50,
      rating:5,
    },
    {
      userId: 3,
      username: 'user3',
      status: 'active',
      role: 'admin',
      email: 'user3@example.com',
      password: 'password789',
      profilePicture: 'user3-profile.jpg',
      membershipLevel: 'Gold',
      balance:50,
      rating:5,
    },
  ];

  beforeEach(async () => {
    mockUserService = jasmine.createSpyObj('UserService', ['getAll', 'updateUserStatus']);
    await TestBed.configureTestingModule({
      declarations: [ UserListComponent ],
      providers: [{ provide: UserService, useValue: mockUserService }]
    }).compileComponents();

    fixture = TestBed.createComponent(UserListComponent);
    component = fixture.componentInstance;


    mockUserService.getAll.and.returnValue(of(mockUsers));
  });

  it('should filter out users with status "deleted"', () => {
    mockUserService.getAll.and.returnValue(of(mockUsers));

    component.ngOnInit();
    fixture.detectChanges();

    expect(component.users.length).toBe(2);  // Only active users should be present
    expect(component.users).toEqual([
      {
        userId: 1,
        username: 'user1',
        status: 'active',
        role: 'admin',
        email: 'user1@example.com',
        password: 'password123',
        profilePicture: 'user1-profile.jpg',
        membershipLevel: 'Bronze',
        balance:50,
        rating:5,
      },
      {
        userId: 3,
        username: 'user3',
        status: 'active',
        role: 'admin',
        email: 'user3@example.com',
        password: 'password789',
        profilePicture: 'user3-profile.jpg',
        membershipLevel: 'Gold',
        balance:50,
        rating:5,
      },
    ]);
  });


});
