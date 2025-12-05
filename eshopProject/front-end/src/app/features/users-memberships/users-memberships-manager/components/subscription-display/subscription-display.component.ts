import {Component, OnDestroy, OnInit} from '@angular/core';
import {UserMembership} from '../../models/user-membership';
import {UserMembershipService} from '../../services/user-membership.service';
import {UserService} from '../../../../../core/user/user-manager/services/user.service';
import {DatePipe, NgIf} from '@angular/common';
import {BehaviorSubject, filter, forkJoin, Observable} from 'rxjs';
import {Article} from '../../../../articles/articles-manager/models/article';
import {Router} from '@angular/router';
import {MembershipService} from '../../../../memberships/memberships-manager/services/membership.service';
import {Membership} from '../../../../memberships/memberships-manager/models/membership';

@Component({
  selector: 'app-subscription-display',
  standalone: true,
  imports: [
    NgIf,
    DatePipe
  ],
  templateUrl: './subscription-display.component.html',
  styleUrl: './subscription-display.component.css'
})
export class SubscriptionDisplayComponent implements OnInit, OnDestroy {
  userMemberships: UserMembership[] = [];
  memberships: ({ name: string; membershipId: number } | { name: string; membershipId: number })[] = []; // Les Memberships associés aux userMemberships
  userId: string | null = null;

  constructor(private _userMembershipService: UserMembershipService, private _userService: UserService, private router: Router,private _membershipService: MembershipService) {}

  ngOnInit() {
    this.userId = this._userService.getUserIdFromToken();

    if (!this.userId) {
      console.error('User not authenticated');
      return;
    }

    const numUserId = parseInt(this.userId, 10);

    // 1️⃣ Récupérer tous les UserMemberships de l'utilisateur
    this._userMembershipService.getAll().subscribe(userMemberships => {
      const filteredMemberships = userMemberships.filter(userMembership => userMembership.userId === numUserId);
      this.userMemberships = filteredMemberships;

      // Extraire la liste unique des IDs de Membership
      const uniqueMembershipIds = [...new Set(filteredMemberships.map(userMembership => userMembership.membershipId))];

      // 2️⃣ Récupérer tous les Memberships en fonction des membershipIds
      const membershipRequests = uniqueMembershipIds.map(membershipId =>
        this._membershipService.getMembershipById(membershipId)
      );

      // 3️⃣ Attendre la réponse de toutes les requêtes de Memberships
      forkJoin(membershipRequests).subscribe(memberships => {
        this.memberships = memberships; // On stocke les Memberships récupérés
      });
    });
  }

  // 4️⃣ Fonction utilitaire pour obtenir le nom d'un Membership à partir de son ID
  getMembershipName(membershipId: number): string {
    const membership = this.memberships.find(m => m.membershipId === membershipId);
    return membership ? membership.name : 'Unknown';
  }

  ngOnDestroy() {}

  navigateToMemberships() {
    this.router.navigate(['/membership-page']);
  }
}
