import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MembershipListComponent } from './membership-list.component';
import {Membership} from '../../models/membership';

describe('MembershipListComponent', () => {
  let component: MembershipListComponent;
  let fixture: ComponentFixture<MembershipListComponent>;

  const mockMemberships: Membership[] = [
    { membershipId: 1, name: 'Gold', price: 100, discountPercentage: 20, description: 'Gold Membership' },
    { membershipId: 2, name: 'Silver', price: 50, discountPercentage: 10, description: 'Silver Membership' }
  ];

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [MembershipListComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(MembershipListComponent);
    component = fixture.componentInstance;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  describe('@Input() memberships', () => {
    it('should receive and bind the memberships input', () => {
      component.memberships = mockMemberships;
      fixture.detectChanges();

      expect(component.memberships).toEqual(mockMemberships);
    });
  });

  describe('emitBuyingMembership()', () => {
    let confirmSpy: jasmine.Spy;

    beforeEach(() => {
      confirmSpy = spyOn(window, 'confirm');
    });

    it('should call confirm() and emit the membershipChosen event if confirmed', () => {
      // Arrange
      const membership = mockMemberships[0];
      const index = 0;
      confirmSpy.and.returnValue(true); // Simulate user clicking "OK"
      spyOn(component.membershipChosen, 'emit'); // Spy on the EventEmitter

      // Act
      component.emitBuyingMembership(membership, index);

      // Assert
      expect(confirmSpy).toHaveBeenCalledWith('Are you sure you want to subscribe to this membership?');
      expect(component.membershipChosen.emit).toHaveBeenCalledWith({ membership, index });
    });

    it('should not emit membershipChosen if the user cancels the confirmation', () => {
      // Arrange
      const membership = mockMemberships[1];
      const index = 1;
      confirmSpy.and.returnValue(false); // Simulate user clicking "Cancel"
      spyOn(component.membershipChosen, 'emit'); // Spy on the EventEmitter

      // Act
      component.emitBuyingMembership(membership, index);

      // Assert
      expect(confirmSpy).toHaveBeenCalledWith('Are you sure you want to subscribe to this membership?');
      expect(component.membershipChosen.emit).not.toHaveBeenCalled();
    });
  });
});
