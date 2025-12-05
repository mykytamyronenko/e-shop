import {Component, EventEmitter, Input, Output} from '@angular/core';
import {User} from '../../models/user';
import {EventBusService} from '../../../../../shared/services/event-bus/event-bus.service';
import {FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators} from '@angular/forms';

@Component({
  selector: 'app-edit-user-profile-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    FormsModule
  ],
  templateUrl: './edit-user-profile-form.component.html',
  styleUrl: './edit-user-profile-form.component.css'
})
export class EditUserProfileFormComponent {
  @Input()
  user: User = {userId: 0, username: "", email: "", password: "", profilePicture: "", membershipLevel: "", balance: 0};

  @Output()
  userUpdated : EventEmitter<User> = new EventEmitter();

  constructor(private _eventBus: EventBusService) {}

  emitUserUpdated(user: User) {
    this.userUpdated.emit(user);
    this._eventBus.publish({
      name: "USER_UPDATED",
      object: user
    });
    alert("Account data updated successfully!");
  }
}
