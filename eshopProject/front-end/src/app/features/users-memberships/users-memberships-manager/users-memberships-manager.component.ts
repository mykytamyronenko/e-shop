import {Component, OnDestroy, OnInit} from '@angular/core';
import {UserMembership} from './models/user-membership';
import {UserMembershipService} from './services/user-membership.service';
import {Transaction} from '../../transactions/transactions-manager/models/transaction';
import {CreateUserMembershipComponent} from './components/create-user-membership/create-user-membership.component';

@Component({
  selector: 'app-users-memberships-manager',
  standalone: true,
  imports: [
    CreateUserMembershipComponent
  ],
  templateUrl: './users-memberships-manager.component.html',
  styleUrl: './users-memberships-manager.component.css'
})
export class UsersMembershipsManagerComponent implements OnInit, OnDestroy {
  userMemberships: UserMembership[] = [];

  constructor(private _userMembershipService: UserMembershipService) {
  }

  ngOnInit() {}

  ngOnDestroy() {}

  createUserMembership(userMembership: UserMembership) {
    this._userMembershipService.create(userMembership).subscribe(userMembership => this.userMemberships.push(userMembership));
    console.log('User has subscribed to a membership:', userMembership);
  }
}
