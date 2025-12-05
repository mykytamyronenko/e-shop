import { Component } from '@angular/core';
import {
  MembershipsManagerComponent
} from '../../../features/memberships/memberships-manager/memberships-manager.component';
import {
  MembershipListComponent
} from '../../../features/memberships/memberships-manager/components/membership-list/membership-list.component';

@Component({
  selector: 'app-membership-page',
  standalone: true,
  imports: [
    MembershipsManagerComponent,
    MembershipListComponent
  ],
  templateUrl: './membership-page.component.html',
  styleUrl: './membership-page.component.css'
})
export class MembershipPageComponent {

}
