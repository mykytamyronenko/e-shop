import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UsersMembershipsManagerComponent } from './users-memberships-manager.component';
import {UserMembershipService} from './services/user-membership.service';
import {NO_ERRORS_SCHEMA} from '@angular/core';
import {UserMembership} from './models/user-membership';
import {of} from 'rxjs';

describe('UsersMembershipsManagerComponent', () => {
  let component: UsersMembershipsManagerComponent;
  let fixture: ComponentFixture<UsersMembershipsManagerComponent>;
  let mockUserMembershipService: jasmine.SpyObj<UserMembershipService>;

  beforeEach(() => {
    // Create a spy for the UserMembershipService
    mockUserMembershipService = jasmine.createSpyObj('UserMembershipService', ['create']);

    // Provide the mock service in the TestBed
    TestBed.configureTestingModule({
      declarations: [UsersMembershipsManagerComponent],
      providers: [
        { provide: UserMembershipService, useValue: mockUserMembershipService }
      ],
      schemas: [NO_ERRORS_SCHEMA] // Skip unknown components (like routing, etc.)
    });

    fixture = TestBed.createComponent(UsersMembershipsManagerComponent);
    component = fixture.componentInstance;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should create a new user membership and add it to the list', () => {
    // Arrange
    const newUserMembership: UserMembership = {
      userMembershipId: 1,
      userId: 101,
      membershipId: 1,
      startDate: '2024-01-01',
      endDate: '2025-01-01',
      status: 'active',
    };

    // Mock the create method to return an observable with the created membership
    mockUserMembershipService.create.and.returnValue(of(newUserMembership));

    // Act
    component.createUserMembership(newUserMembership);

    // Assert
    expect(mockUserMembershipService.create).toHaveBeenCalledWith(newUserMembership);
    expect(component.userMemberships.length).toBe(1); // One membership should be added to the list
    expect(component.userMemberships[0]).toEqual(newUserMembership); // The added membership should match
  });

  it('should log the user membership creation', () => {
    // Arrange
    const newUserMembership: UserMembership = {
      userMembershipId: 1,
      userId: 101,
      membershipId: 1,
      startDate: '2024-01-01',
      endDate: '2025-01-01',
      status: 'active',
    };

    // Spy on console.log to check the log
    spyOn(console, 'log');

    // Mock the create method
    mockUserMembershipService.create.and.returnValue(of(newUserMembership));

    // Act
    component.createUserMembership(newUserMembership);

    // Assert
    expect(console.log).toHaveBeenCalledWith('User has subscribed to a membership:', newUserMembership);
  });
});
