import {Component, EventEmitter, inject, Output} from '@angular/core';
import {FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators} from '@angular/forms';
import {AdminUser} from '../../user/user-manager/models/user';

@Component({
  selector: 'app-create-admin-form',
  standalone: true,
  imports: [
    FormsModule,
    ReactiveFormsModule
  ],
  templateUrl: './create-admin-form.component.html',
  styleUrl: './create-admin-form.component.css'
})
export class CreateAdminFormComponent {
  private _fb:FormBuilder = inject(FormBuilder);

  form: FormGroup = this._fb.group({
    main: this._fb.group({
      username: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
    }),
    other: this._fb.group({
      role: ['', Validators.required],
      profilePicture: ['', Validators.required],
      membershipLevel: ['Gold'],
      rating: ['', Validators.required],
      status: ['', Validators.required],
    })
  });

  @Output()
  userCreated: EventEmitter<{ user: AdminUser; imageFile?: File }> = new EventEmitter();

  onFileChange(event: any) {
    const file = event.target.files[0];  // select the targeted file
    if (file) {
      // @ts-ignore
      this.form.get('other').patchValue({
        profilePicture: file  // link the file to the image form
      });
    }
  }

  sendDate() {
    console.log(this.form.value);
  }

  emitUser() {
    console.log("User data:", this.form.value);
    console.log("Image file:", this.form.value.other.profilePicture);

    const userData: AdminUser = {
      userId: 0,
      username: this.form.value.main.username,
      email: this.form.value.main.email,
      password: this.form.value.main.password,
      role : this.form.value.other.role,
      status : this.form.value.other.status,
      rating : this.form.value.other.rating,
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
      alert("User created.")
    }
  }
}
