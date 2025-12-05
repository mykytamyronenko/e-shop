import {Component, ElementRef, EventEmitter, inject, Output, ViewChild} from '@angular/core';
import {FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators} from "@angular/forms";
import {Article} from '../../models/article';
import {UserService} from '../../../../../core/user/user-manager/services/user.service';
import {NgForOf} from '@angular/common';
import {Router} from '@angular/router';

@Component({
  selector: 'app-create-article-form',
  standalone: true,
  imports: [
    FormsModule,
    ReactiveFormsModule,
    NgForOf
  ],
  templateUrl: './create-article-form.component.html',
  styleUrl: './create-article-form.component.css'
})

export class CreateArticleFormComponent {
  private _fb:FormBuilder = inject(FormBuilder);
  private _userService:UserService = inject(UserService);
  public categories = ['Electronics', 'Books', 'Clothing', 'Furnishing', 'Toys', 'Sports', 'Beauty', 'Food', 'Vehicles', 'Other'];

  constructor(private router: Router) {
  }

  form: FormGroup = this._fb.group({
      name: ['', Validators.required],
      description: ['', [Validators.required]],
      price: ['', Validators.required],
      category: ['Other', Validators.required],
      state: ['', Validators.required],
      quantity: ['', Validators.required],
      image: ['', Validators.required]
  });

  @Output()
  articleCreated: EventEmitter<{ article: Article; imageFile?: File }> = new EventEmitter();

  sendDate() {
    console.log(this.form.value);
  }

  emitArticle() {
    console.log("Article data:", this.form.value);
    console.log("Image file:", this.form.value.image);
    const userId = this._userService.getUserIdFromToken();

    if (!userId) {
      console.error('User not authenticated');
      return;
    }
    //mandatory because c# doesn't understand the format of date.now(), here we send iso8601 as c# prefer
    const dateRightFormat = new Date();

    const articleData: Article = {
      articleId: 0,
      title: this.form.value.name,
      description: this.form.value.description,
      price: this.form.value.price,
      category: this.form.value.category,
      state: this.form.value.state,
      userId: parseInt(userId, 10),
      createdAt: dateRightFormat.toISOString(),
      updatedAt: dateRightFormat.toISOString(),
      status: 'available',
      quantity: this.form.value.quantity,
      mainImageUrl: '',
      additionalImages: '',
    };
    const imageFile: File = this.form.value.image;

    this.articleCreated.emit({
      article: articleData,
      imageFile: imageFile,
    });

  }

  submitForm() {
    console.log("Form submitted");
    if (this.form.valid) {
      this.sendDate();
      this.emitArticle();
      this.form.reset();
      this.form.value.image = "";
      alert('The article is now successfully on sale!');
      this.router.navigate(['/hub'])
    }
  }

  onFileChange(event: any) {
    const file = event.target.files[0];  // select the targeted file
    if (file) {
      this.form.patchValue({
        image: file  // link the file to the image form
      });
    }
  }
}
