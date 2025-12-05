import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateUserFormComponent } from './create-user-form.component';
import {Router} from '@angular/router';
import {FormBuilder, ReactiveFormsModule} from '@angular/forms';
import { User } from '../../models/user';
import {By} from '@angular/platform-browser';

describe('CreateUserFormComponent', () => {
  let component: CreateUserFormComponent;
  let fixture: ComponentFixture<CreateUserFormComponent>;
  let router: Router;
  let mockUserCreatedEmitter: jasmine.Spy<(value?: { user: User; imageFile?: File; } | undefined) => void>;

  beforeEach(async () => {
    // Mock Router
    const routerMock = {
      navigate: jasmine.createSpy('navigate')
    };

    await TestBed.configureTestingModule({
      declarations: [CreateUserFormComponent],
      imports: [ReactiveFormsModule],
      providers: [
        FormBuilder,
        { provide: Router, useValue: routerMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(CreateUserFormComponent);
    component = fixture.componentInstance;
    router = TestBed.inject(Router);
    mockUserCreatedEmitter = spyOn(component.userCreated, 'emit');
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should emit userCreated event on form submission', () => {
    // Set form values
    component.form.setValue({
      main: {
        username: 'testuser',
        email: 'testuser@example.com',
        password: 'password123'
      },
      other: {
        profilePicture: new File([''], 'test.jpg'),
        membershipLevel: 'Bronze'
      }
    });

    // Submit form
    component.submitForm();

    // Check if the emit method was called
    expect(mockUserCreatedEmitter).toHaveBeenCalled();
  });

  it('should navigate to /login after form submission', () => {
    component.form.setValue({
      main: {
        username: 'testuser',
        email: 'testuser@example.com',
        password: 'password123'
      },
      other: {
        profilePicture: new File([''], 'test.jpg'),
        membershipLevel: 'Bronze'
      }
    });

    // Submit form
    component.submitForm();

    // Check if router navigation was triggered
    expect(router.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('should call onFileChange method on file input change', () => {
    const fileInput = fixture.debugElement.query(By.css('input[type="file"]')).nativeElement;

    // Create a mock file
    const file = new File([''], 'test.jpg', { type: 'image/jpeg' });

    // Trigger the file change event
    fileInput.dispatchEvent(new Event('change'));
    component.onFileChange({ target: { files: [file] } });

    // Check if the form was updated with the selected file
    expect(component.form.get('other.profilePicture')?.value).toBe(file);
  });

  it('should show an error for invalid file type', () => {
    const fileInput = fixture.debugElement.query(By.css('input[type="file"]')).nativeElement;

    // Create an invalid file
    const invalidFile = new File([''], 'test.txt', { type: 'text/plain' });

    // Trigger the file change event
    fileInput.dispatchEvent(new Event('change'));
    component.onFileChange({ target: { files: [invalidFile] } });

    // Assert that no file was added to the form
    expect(component.form.get('other.profilePicture')?.value).toBeNull();
  });

  it('should show an error for file size exceeding 5MB', () => {
    const fileInput = fixture.debugElement.query(By.css('input[type="file"]')).nativeElement;

    // Create a large file (6MB)
    const largeFile = new File([new Array(6 * 1024 * 1024).join('x')], 'large.jpg', { type: 'image/jpeg' });

    // Trigger the file change event
    fileInput.dispatchEvent(new Event('change'));
    component.onFileChange({ target: { files: [largeFile] } });

    // Assert that no file was added to the form
    expect(component.form.get('other.profilePicture')?.value).toBeNull();
  });
});
