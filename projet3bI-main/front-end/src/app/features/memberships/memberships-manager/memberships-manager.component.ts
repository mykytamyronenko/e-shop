import {Component, OnDestroy, OnInit} from '@angular/core';
import {Membership} from './models/membership';
import {MembershipService} from './services/membership.service';
import {MembershipChosen, MembershipListComponent} from './components/membership-list/membership-list.component';
import {UserService} from '../../../core/user/user-manager/services/user.service';
import {UserMembership} from '../../users-memberships/users-memberships-manager/models/user-membership';
import {
  UserMembershipService
} from '../../users-memberships/users-memberships-manager/services/user-membership.service';

@Component({
  selector: 'app-memberships-manager',
  standalone: true,
  imports: [
    MembershipListComponent
  ],
  templateUrl: './memberships-manager.component.html',
  styleUrl: './memberships-manager.component.css'
})
export class MembershipsManagerComponent implements OnInit {
  memberships: Membership[] = [];
  selectedMembership!: Membership;

  constructor(private _membershipService: MembershipService, private _userService: UserService, private _userMembershipService: UserMembershipService) {
  }

  ngOnInit() {
    this._membershipService.getAll().subscribe(memberships => {console.log(memberships)
    this.memberships = memberships;});
  }

  selectMembershipForSubscription(membershipChosen: MembershipChosen) {
    this.selectedMembership = membershipChosen.membership;

    console.log('Membership selected for subscription:', this.selectedMembership);

    this.createUserMembership(this.selectedMembership);
  }

  createUserMembership(membership: Membership) {
    const userId = this._userService.getUserIdFromToken();

    if (!userId) {
      console.error('User not authenticated');
      return;
    }

    const userMembership: UserMembership = {
      userMembershipId: 0,
      userId: parseInt(userId, 10),
      membershipId: membership.membershipId,
      startDate: new Date().toISOString(),
      endDate: new Date().toISOString(),
      status: 'active',
    };

    this._userMembershipService.create(userMembership).subscribe({
      next: (createdUserMembership) => {
        console.log('Subscription successfully recorded :', createdUserMembership);
        alert('Subscription successful!');
      },
      error: (err) => {
        console.error('An error occurred during the subscription.', err);
      }
    });

    console.log('Subscription created:', userMembership);
  }

}
