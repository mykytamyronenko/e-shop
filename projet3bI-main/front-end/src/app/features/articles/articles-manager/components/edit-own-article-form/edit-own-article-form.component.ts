import {Component, EventEmitter, inject, OnInit, Output} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {ArticleService} from '../../services/article.service';
import {Article} from '../../models/article';
import {FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators} from '@angular/forms';
import {NgForOf} from '@angular/common';
import {User} from '../../../../../core/user/user-manager/models/user';
import {EventBusService} from '../../../../../shared/services/event-bus/event-bus.service';

@Component({
  selector: 'app-edit-own-article-form',
  standalone: true,
  imports: [
    FormsModule,
    ReactiveFormsModule,
    NgForOf
  ],
  templateUrl: './edit-own-article-form.component.html',
  styleUrl: './edit-own-article-form.component.css'
})
export class EditOwnArticleFormComponent implements OnInit{
  ownArticle: Article = {
    additionalImages: '',
    createdAt: '',
    mainImageUrl: '',
    quantity: 0,
    state: "used",
    status: "available",
    updatedAt: '',
    userId: 0,
    articleId:0,title:"",description:"",price:0,category:"Other"
  };

  public categories = ['Electronics', 'Books', 'Clothing', 'Furnishing', 'Toys', 'Sports', 'Beauty', 'Food', 'Vehicles', 'Other'];


  constructor(private _route: ActivatedRoute,private _articleService: ArticleService, private router: Router) {
  }

  ngOnInit(): void {
    const articleId = this._route.snapshot.paramMap.get('articleId');

    if(articleId){
      this._articleService.getArticleById(+articleId).subscribe(article => {
        this.ownArticle = article;
      });
    }

  }

  submitChange() {
    this._articleService.updateArticle(this.ownArticle).subscribe({
      next: () => {
        alert('Article successfully updated!');
        this.router.navigate(['/profile-page']);
      },
      error: (err) => {
        console.error('Failed to update the article:', err);
        alert('There was an error updating the article.');
      }
    });
  }



}
