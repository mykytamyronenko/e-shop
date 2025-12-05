import {Component, EventEmitter, Input, OnDestroy, OnInit, Output} from '@angular/core';
import {Membership} from '../../models/membership';
import {MembershipService} from '../../services/membership.service';
import {
  UsersMembershipsManagerComponent
} from '../../../../users-memberships/users-memberships-manager/users-memberships-manager.component';
import {Article} from '../../../../articles/articles-manager/models/article';
import {UserMembership} from '../../../../users-memberships/users-memberships-manager/models/user-membership';
import {ArticleBought} from '../../../../articles/articles-manager/components/article-list/article-list.component';
import {NgForOf, NgIf} from '@angular/common';

export interface MembershipChosen {
  membership: Membership;
  index: number;
}

@Component({
  selector: 'app-membership-list',
  standalone: true,
  imports: [
    UsersMembershipsManagerComponent,
    NgIf,
    NgForOf
  ],
  templateUrl: './membership-list.component.html',
  styleUrl: './membership-list.component.css'
})
export class MembershipListComponent {
  @Input()
  memberships: Membership[] = [];

  @Output()
  membershipChosen: EventEmitter<MembershipChosen> = new EventEmitter();

  constructor(private _membershipService: MembershipService) {
  }

  emitBuyingMembership(membership: Membership, index: number) {
    if (confirm('Are you sure you want to subscribe to this membership?')) {
      this.membershipChosen.emit({membership: membership, index: index});
    }
  }
}
