import {Component, EventEmitter, inject, Output} from '@angular/core';
import {FormBuilder, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {User} from '../../models/user';
import {Article} from '../../../../../features/articles/articles-manager/models/article';
import {Router} from '@angular/router';

@Component({
  selector: 'app-create-user-form',
  standalone: true,
  imports: [
    ReactiveFormsModule
  ],
  templateUrl: './create-user-form.component.html',
  styleUrl: './create-user-form.component.css'
})
export class CreateUserFormComponent {
  private _fb:FormBuilder = inject(FormBuilder);

  constructor(private router: Router) {
  }

  form: FormGroup = this._fb.group({
    main: this._fb.group({
      username: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
    }),
    other: this._fb.group({
      profilePicture: ['', Validators.required],
      membershipLevel: ['Bronze', Validators.required],
    })
  });

  @Output()
  userCreated: EventEmitter<{ user: User; imageFile?: File }> = new EventEmitter();

  sendDate() {
    console.log(this.form.value);
  }

  emitUser() {
    console.log("User data:", this.form.value);
    console.log("Image file:", this.form.value.other.profilePicture);

    const userData: User = {
      userId: 0,
      username: this.form.value.main.username,
      email: this.form.value.main.email,
      password: this.form.value.main.password,
      profilePicture: '',
      membershipLevel: this.form.value.other.membershipLevel,
      balance: 0,
    };

    const imageFile: File = this.form.value.other.profilePicture;

    this.userCreated.emit({
      user: userData,
      imageFile: imageFile,
    });

  }



  submitForm() {
    if (this.form.valid) {
      this.sendDate();
      this.emitUser()
      this.router.navigate(['/login']);
    }
  }
  onFileChange(event: any) {
    const file = event.target.files[0];  // select the targeted file
    if (file) {
      // @ts-ignore
      this.form.get('other').patchValue({
        profilePicture: file  // link the file to the image form
      });
    }
  }

}
