import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MembershipsManagerComponent } from './memberships-manager.component';
import {MembershipService} from './services/membership.service';
import {UserService} from '../../../core/user/user-manager/services/user.service';
import {
  UserMembershipService
} from '../../users-memberships/users-memberships-manager/services/user-membership.service';
import {Membership} from './models/membership';
import {of, throwError} from 'rxjs';
import {UserMembership} from '../../users-memberships/users-memberships-manager/models/user-membership';

describe('MembershipsManagerComponent', () => {
  let component: MembershipsManagerComponent;
  let fixture: ComponentFixture<MembershipsManagerComponent>;
  let membershipServiceMock: jasmine.SpyObj<MembershipService>;
  let userServiceMock: jasmine.SpyObj<UserService>;
  let userMembershipServiceMock: jasmine.SpyObj<UserMembershipService>;

  beforeEach(async () => {
    membershipServiceMock = jasmine.createSpyObj('MembershipService', ['getAll']);
    userServiceMock = jasmine.createSpyObj('UserService', ['getUserIdFromToken']);
    userMembershipServiceMock = jasmine.createSpyObj('UserMembershipService', ['create']);

    await TestBed.configureTestingModule({
      declarations: [MembershipsManagerComponent],
      providers: [
        { provide: MembershipService, useValue: membershipServiceMock },
        { provide: UserService, useValue: userServiceMock },
        { provide: UserMembershipService, useValue: userMembershipServiceMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(MembershipsManagerComponent);
    component = fixture.componentInstance;
  });

  afterEach(() => {
    // Clear spy calls after each test
    membershipServiceMock.getAll.calls.reset();
    userServiceMock.getUserIdFromToken.calls.reset();
    userMembershipServiceMock.create.calls.reset();
  });

  describe('ngOnInit', () => {
    it('should call MembershipService.getAll() and populate memberships', () => {
      const mockMemberships: Membership[] = [
        { membershipId: 1, name: 'Gold', price: 100, discountPercentage: 20, description: 'Gold Membership' },
        { membershipId: 2, name: 'Silver', price: 50, discountPercentage: 10, description: 'Silver Membership' }
      ];

      membershipServiceMock.getAll.and.returnValue(of(mockMemberships));

      component.ngOnInit();

      expect(membershipServiceMock.getAll).toHaveBeenCalled();
      expect(component.memberships).toEqual(mockMemberships);
    });

    it('should log an error if MembershipService.getAll() fails', () => {
      const consoleErrorSpy = spyOn(console, 'error');
      membershipServiceMock.getAll.and.returnValue(throwError(() => new Error('Server error')));

      component.ngOnInit();

      expect(membershipServiceMock.getAll).toHaveBeenCalled();
      expect(consoleErrorSpy).toHaveBeenCalledWith('Server error');
      expect(component.memberships).toEqual([]);
    });
  });


  describe('createUserMembership', () => {
    it('should create a UserMembership and call UserMembershipService.create() if user is authenticated', () => {
      const mockUserId = '123';
      userServiceMock.getUserIdFromToken.and.returnValue(mockUserId);

      const mockMembership: Membership = { membershipId: 1, name: 'Gold', price: 100, discountPercentage: 20, description: 'Gold Membership' };

      const expectedUserMembership: UserMembership = {
        userMembershipId: 0,
        userId: parseInt(mockUserId, 10),
        membershipId: mockMembership.membershipId,
        startDate: Date.now().toString(),
        endDate: Date.now().toString(),
        status: 'active'
      };

      userMembershipServiceMock.create.and.returnValue(of(expectedUserMembership));

      component.createUserMembership(mockMembership);

      expect(userServiceMock.getUserIdFromToken).toHaveBeenCalled();
      expect(userMembershipServiceMock.create).toHaveBeenCalledWith(jasmine.objectContaining(expectedUserMembership));
    });

    it('should log an error if user is not authenticated', () => {
      userServiceMock.getUserIdFromToken.and.returnValue(null);
      const consoleErrorSpy = spyOn(console, 'error');

      const mockMembership: Membership = { membershipId: 1, name: 'Gold', price: 100, discountPercentage: 20, description: 'Gold Membership' };

      component.createUserMembership(mockMembership);

      expect(userServiceMock.getUserIdFromToken).toHaveBeenCalled();
      expect(consoleErrorSpy).toHaveBeenCalledWith('User not authenticated');
      expect(userMembershipServiceMock.create).not.toHaveBeenCalled();
    });

    it('should handle an error from UserMembershipService.create()', () => {
      const mockUserId = '123';
      userServiceMock.getUserIdFromToken.and.returnValue(mockUserId);

      const mockMembership: Membership = { membershipId: 1, name: 'Gold', price: 100, discountPercentage: 20, description: 'Gold Membership' };

      userMembershipServiceMock.create.and.returnValue(throwError(() => new Error('Server error')));

      const consoleErrorSpy = spyOn(console, 'error');

      component.createUserMembership(mockMembership);

      expect(userServiceMock.getUserIdFromToken).toHaveBeenCalled();
      expect(userMembershipServiceMock.create).toHaveBeenCalled();
      expect(consoleErrorSpy).toHaveBeenCalledWith('An error occurred during the subscription.', jasmine.any(Error));
    });
  });
});
