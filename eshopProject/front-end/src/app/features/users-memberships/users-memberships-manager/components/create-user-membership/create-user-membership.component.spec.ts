import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateUserMembershipComponent } from './create-user-membership.component';
import {UserService} from '../../../../../core/user/user-manager/services/user.service';
import {MembershipService} from '../../../../memberships/memberships-manager/services/membership.service';
import {Router} from '@angular/router';
import {FormBuilder, FormGroup, ReactiveFormsModule} from '@angular/forms';
import {RouterTestingModule} from '@angular/router/testing';
import {of} from 'rxjs';

describe('CreateUserMembershipComponent', () => {
  let component: CreateUserMembershipComponent;
  let fixture: ComponentFixture<CreateUserMembershipComponent>;
  let mockUserService: jasmine.SpyObj<UserService>;
  let mockMembershipService: jasmine.SpyObj<MembershipService>;
  let mockRouter: jasmine.SpyObj<Router>;

  beforeEach(() => {
    mockUserService = jasmine.createSpyObj('UserService', ['getUserIdFromToken']);
    mockMembershipService = jasmine.createSpyObj('MembershipService', ['getSelectedMembership']);
    mockRouter = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      imports: [ReactiveFormsModule, RouterTestingModule],
      declarations: [CreateUserMembershipComponent],
      providers: [
        { provide: UserService, useValue: mockUserService },
        { provide: MembershipService, useValue: mockMembershipService },
        { provide: Router, useValue: mockRouter },
        FormBuilder
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(CreateUserMembershipComponent);
    component = fixture.componentInstance;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize form correctly', () => {
    expect(component.form instanceof FormGroup).toBe(true);
    expect(component.form.get('userId')).toBeTruthy();
    expect(component.form.get('membershipId')).toBeTruthy();
    expect(component.form.get('startDate')).toBeTruthy();
    expect(component.form.get('endDate')).toBeTruthy();
    expect(component.form.get('status')).toBeTruthy();
  });

  it('should load selected membership and set membershipId', async () => {
    const mockMembership = { membershipId: 1, name: 'Premium Membership', price: 100, discountPercentage: 10, description: 'A premium membership' };

    mockMembershipService.getSelectedMembership.and.returnValue(of(mockMembership));

    await component.loadSelectedMembership();

    expect(component.selectedMembership).toEqual(mockMembership);
    expect(component.membershipId).toBe(mockMembership.membershipId);
  });

  it('should load selected membership and set membershipId', async () => {
    // Updated mockMembership with all required properties
    const mockMembership = {
      membershipId: 1,
      name: 'Premium Membership',
      price: 100,
      discountPercentage: 10,
      description: 'A premium membership with extra benefits.'
    };

    mockMembershipService.getSelectedMembership.and.returnValue(of(mockMembership));

    await component.loadSelectedMembership();

    expect(component.selectedMembership).toEqual(mockMembership);
    expect(component.membershipId).toBe(mockMembership.membershipId);
  });

  it('should not emit user membership if no membership is selected', () => {
    spyOn(component.userMembershipCreated, 'emit');

    component.selectedMembership = null;

    component.emitUserMembership();

    expect(component.userMembershipCreated.emit).not.toHaveBeenCalled();
  });

  it('should submit user membership and navigate on success', async () => {
    const mockUserId = '123';
    const mockMembership = {
      membershipId: 1,
      name: 'Premium Membership',
      price: 100,
      discountPercentage: 10,
      description: 'A premium membership with extra benefits.'
    };
    mockUserService.getUserIdFromToken.and.returnValue(mockUserId);
    mockMembershipService.getSelectedMembership.and.returnValue(of(mockMembership));

    spyOn(component.userMembershipCreated, 'emit');
    mockRouter.navigate.and.returnValue(Promise.resolve(true));

    await component.submitUserMembership();

    expect(component.userMembershipCreated.emit).toHaveBeenCalled();
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/hub']);
  });

  it('should handle case when user is not authenticated during submit', async () => {
    mockUserService.getUserIdFromToken.and.returnValue(null);

    spyOn(component.userMembershipCreated, 'emit');
    mockRouter.navigate.and.returnValue(Promise.resolve(true));

    await component.submitUserMembership();

    expect(component.userMembershipCreated.emit).not.toHaveBeenCalled();
    expect(mockRouter.navigate).not.toHaveBeenCalled();
  });
});
