import { Component } from '@angular/core';
import {UserManagerComponent} from "../../user/user-manager/user-manager.component";

@Component({
  selector: 'app-register',
  standalone: true,
    imports: [
        UserManagerComponent
    ],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {

}
