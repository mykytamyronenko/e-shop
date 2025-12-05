import {Component, EventEmitter, inject, Output} from '@angular/core';
import {FormBuilder, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {UserService} from '../../../../../core/user/user-manager/services/user.service';
import {UserMembership} from '../../models/user-membership';
import {MembershipService} from '../../../../memberships/memberships-manager/services/membership.service';
import {firstValueFrom} from 'rxjs';
import {Membership} from '../../../../memberships/memberships-manager/models/membership';
import {Router} from '@angular/router';

@Component({
  selector: 'app-create-user-membership',
  standalone: true,
  imports: [
    ReactiveFormsModule
  ],
  templateUrl: './create-user-membership.component.html',
  styleUrl: './create-user-membership.component.css'
})
export class CreateUserMembershipComponent {
  private _fb: FormBuilder = inject(FormBuilder);
  private _userService: UserService = inject(UserService);
  private _membershipService: MembershipService = inject(MembershipService);

  constructor(private router: Router) {
  }

  @Output() userMembershipCreated : EventEmitter<UserMembership> = new EventEmitter();

  public selectedMembership: Membership | null = null;
  public membershipId?: number;

  form: FormGroup = this._fb.group({
    userId: [''],
    membershipId: [''],
    startDate: [''],
    endDate: [''],
    status: ['']
  });

  async loadSelectedMembership() {
    try {
      this.selectedMembership = await firstValueFrom(this._membershipService.getSelectedMembership());
      if (this.selectedMembership) {
        this.membershipId = this.selectedMembership.membershipId;
        console.log('Membership loaded : ', this.selectedMembership);
      }
    } catch (error) {
      console.error('Error loading membership:', error);
    }
  }

  emitUserMembership() {
    const connectedUserId = this._userService.getUserIdFromToken();

    if (!connectedUserId) {
      console.log('User not authenticated');
      return;
    }

    if (this.selectedMembership) {
      const dateRightFormat = new Date();

      const userMembership: UserMembership = {
        userMembershipId: 0,
        userId: parseInt(connectedUserId, 10),
        membershipId: this.selectedMembership.membershipId,
        startDate: dateRightFormat.toISOString(),
        endDate: dateRightFormat.toISOString(),
        status: 'active',
      };

      this.userMembershipCreated.emit(userMembership);
    } else {
      console.warn('No items selected');
    }
  }

  async submitUserMembership() {
    const connectedUserId = this._userService.getUserIdFromToken();

    if (!connectedUserId) {
      console.error('User not authenticated');
      return;
    }

    if (!this.selectedMembership) {
      await this.loadSelectedMembership();
    }

    if (this.selectedMembership) {
      this.emitUserMembership()
    }
    this.router.navigate(['/hub'])
  }

}
