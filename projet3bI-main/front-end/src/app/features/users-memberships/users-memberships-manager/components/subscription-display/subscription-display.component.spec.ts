import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SubscriptionDisplayComponent } from './subscription-display.component';
import {UserMembershipService} from '../../services/user-membership.service';
import {UserService} from '../../../../../core/user/user-manager/services/user.service';
import {MembershipService} from '../../../../memberships/memberships-manager/services/membership.service';
import {Router} from '@angular/router';
import {HttpClientTestingModule} from '@angular/common/http/testing';
import {RouterTestingModule} from '@angular/router/testing';
import {of} from 'rxjs';

describe('SubscriptionDisplayComponent', () => {
  let component: SubscriptionDisplayComponent;
  let fixture: ComponentFixture<SubscriptionDisplayComponent>;
  let mockUserMembershipService: jasmine.SpyObj<UserMembershipService>;
  let mockUserService: jasmine.SpyObj<UserService>;
  let mockMembershipService: jasmine.SpyObj<MembershipService>;
  let mockRouter: jasmine.SpyObj<Router>;

  beforeEach(() => {
    mockUserMembershipService = jasmine.createSpyObj('UserMembershipService', ['getAll']);
    mockUserService = jasmine.createSpyObj('UserService', ['getUserIdFromToken']);
    mockMembershipService = jasmine.createSpyObj('MembershipService', ['getMembershipById']);
    mockRouter = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, RouterTestingModule],
      declarations: [SubscriptionDisplayComponent],
      providers: [
        { provide: UserMembershipService, useValue: mockUserMembershipService },
        { provide: UserService, useValue: mockUserService },
        { provide: MembershipService, useValue: mockMembershipService },
        { provide: Router, useValue: mockRouter }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(SubscriptionDisplayComponent);
    component = fixture.componentInstance;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should return the correct membership name from getMembershipName', () => {
    const mockMemberships = [
      { membershipId: 1, name: 'Basic Membership', price: 100, discountPercentage: 10, description: 'A basic membership' },
      { membershipId: 2, name: 'Premium Membership', price: 200, discountPercentage: 20, description: 'A premium membership' }
    ];

    component.memberships = mockMemberships;

    const membershipName = component.getMembershipName(1);
    expect(membershipName).toBe('Basic Membership');

    const unknownMembership = component.getMembershipName(999);
    expect(unknownMembership).toBe('Unknown');
  });

  it('should navigate to the membership page on navigateToMemberships', () => {
    component.navigateToMemberships();
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/membership-page']);
  });

  it('should return the correct membership name from getMembershipName', () => {
    const mockMemberships = [
      { membershipId: 1, name: 'Basic Membership' },
      { membershipId: 2, name: 'Premium Membership' }
    ];

    component.memberships = mockMemberships;

    const membershipName = component.getMembershipName(1);
    expect(membershipName).toBe('Basic Membership');

    const unknownMembership = component.getMembershipName(999);
    expect(unknownMembership).toBe('Unknown');
  });

  it('should navigate to the membership page on navigateToMemberships', () => {
    component.navigateToMemberships();
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/membership-page']);
  });
});
