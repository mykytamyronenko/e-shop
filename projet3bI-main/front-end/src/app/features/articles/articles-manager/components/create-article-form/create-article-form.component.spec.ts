import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateArticleFormComponent } from './create-article-form.component';
import {Router} from '@angular/router';
import {UserService} from '../../../../../core/user/user-manager/services/user.service';
import {FormBuilder, ReactiveFormsModule} from '@angular/forms';
import {Article} from '../../models/article';

describe('CreateArticleFormComponent', () => {
  let component: CreateArticleFormComponent;
  let fixture: ComponentFixture<CreateArticleFormComponent>;
  let routerSpy: jasmine.SpyObj<Router>;
  let userServiceSpy: jasmine.SpyObj<UserService>;

  beforeEach(async () => {
    // Create spies for Router and UserService
    routerSpy = jasmine.createSpyObj('Router', ['navigate']);
    userServiceSpy = jasmine.createSpyObj('UserService', ['getUserIdFromToken']);
    userServiceSpy.getUserIdFromToken.and.returnValue('1'); // Simulate user being logged in

    await TestBed.configureTestingModule({
      imports: [ReactiveFormsModule],
      declarations: [CreateArticleFormComponent],
      providers: [
        FormBuilder,
        { provide: Router, useValue: routerSpy },
        { provide: UserService, useValue: userServiceSpy },
      ]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateArticleFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the form with initial controls', () => {
    expect(component.form).toBeTruthy();
    expect(component.form.controls['name']).toBeTruthy();
    expect(component.form.controls['description']).toBeTruthy();
    expect(component.form.controls['price']).toBeTruthy();
    expect(component.form.controls['category']).toBeTruthy();
    expect(component.form.controls['state']).toBeTruthy();
    expect(component.form.controls['quantity']).toBeTruthy();
    expect(component.form.controls['image']).toBeTruthy();
  });

  it('should mark the form as invalid when required fields are missing', () => {
    const form = component.form;
    form.controls['name'].setValue('');
    form.controls['description'].setValue('');
    form.controls['price'].setValue('');
    form.controls['category'].setValue('');
    form.controls['state'].setValue('');
    form.controls['quantity'].setValue('');
    form.controls['image'].setValue('');

    expect(form.invalid).toBeTrue();
  });

  it('should submit the form and emit article when the form is valid', () => {
    spyOn(component.articleCreated, 'emit');

    const articleData = {
      title: 'Test Article',
      description: 'Test Description',
      price: 100,
      category: 'Electronics',
      state: 'new',
      quantity: 1,
      image: new File([], 'test.png'),
    };

    // Set valid form data
    component.form.setValue({
      name: 'Test Article',
      description: 'Test Description',
      price: 100,
      category: 'Electronics',
      state: 'new',
      quantity: 1,
      image: new File([], 'test.png'),
    });

    component.submitForm();

    expect(component.form.valid).toBeTrue();
    expect(component.articleCreated.emit).toHaveBeenCalled();
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/hub']);
  });

  it('should call the onFileChange method and update the form', () => {
    const fileInput = new File([], 'test-image.png');
    const event = { target: { files: [fileInput] } };

    component.onFileChange(event);

    expect(component.form.controls['image'].value).toEqual(fileInput);
  });

  it('should emit article and image file when emitArticle() is called', () => {
    spyOn(component.articleCreated, 'emit');

    const articleData: Article = {
      articleId: 0,
      title: 'Test Article',
      description: 'Test Description',
      price: 100,
      category: 'Electronics',
      state: 'new',  // Ensure state is one of 'new' | 'used'
      userId: 1,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
      status: 'available',
      quantity: 1,
      mainImageUrl: '',
      additionalImages: '',
    };

    const imageFile = new File([], 'test-image.png');

    component.emitArticle();

    expect(component.articleCreated.emit).toHaveBeenCalledWith({ article: articleData, imageFile });
  });

});
