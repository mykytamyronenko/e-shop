import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LoginComponent } from './login.component';
import {UserService} from '../../user/user-manager/services/user.service';
import {Router} from '@angular/router';
import {FormBuilder, ReactiveFormsModule} from '@angular/forms';
import {NO_ERRORS_SCHEMA} from '@angular/core';
import {of, throwError} from 'rxjs';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let mockUserService: jasmine.SpyObj<UserService>;
  let mockRouter: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    mockUserService = jasmine.createSpyObj('UserService', [
      'login', 'getUserIdFromToken', 'getCookie', 'logout', 'updateLoginStatus', 'startLogoutTimer'
    ]);
    mockRouter = jasmine.createSpyObj('Router', ['navigate', 'navigateByUrl']);

    await TestBed.configureTestingModule({
      declarations: [LoginComponent],
      imports: [ReactiveFormsModule],
      providers: [
        { provide: UserService, useValue: mockUserService },
        { provide: Router, useValue: mockRouter },
        FormBuilder
      ],
      schemas: [NO_ERRORS_SCHEMA],
    }).compileComponents();

    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should call login method and navigate to hub on successful login', () => {
    const mockResponse = { token: 'fakeToken' };
    mockUserService.login.and.returnValue(of(mockResponse));
    mockUserService.getUserIdFromToken.and.returnValue('123');
    mockUserService.getCookie.and.returnValue('valid');
    mockRouter.navigateByUrl.and.returnValue(Promise.resolve(true));

    component.form.setValue({ username: 'testuser', password: 'password123' });
    component.submitForm();

    expect(mockUserService.login).toHaveBeenCalledWith('testuser', 'password123');
    expect(mockRouter.navigateByUrl).toHaveBeenCalledWith('/hub', { skipLocationChange: false });
    expect(mockUserService.updateLoginStatus).toHaveBeenCalledWith(true);
    expect(mockUserService.startLogoutTimer).toHaveBeenCalled();
  });

  it('should handle "AccountDeleted" cookie and log out', () => {
    const mockResponse = { token: 'fakeToken' };
    mockUserService.login.and.returnValue(of(mockResponse));
    mockUserService.getUserIdFromToken.and.returnValue('123');
    mockUserService.getCookie.and.returnValue('AccountDeleted');
    mockUserService.logout.and.returnValue(of(null));

    component.form.setValue({ username: 'testuser', password: 'password123' });
    component.submitForm();

    expect(mockUserService.getCookie).toHaveBeenCalled();
    expect(mockUserService.logout).toHaveBeenCalled();
    expect(mockUserService.updateLoginStatus).toHaveBeenCalledWith(false);
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/']);
  });

  it('should show error alert on failed login', () => {
    const errorResponse = { message: 'Wrong credentials' };
    mockUserService.login.and.returnValue(throwError(() => new Error('Wrong credentials')));

    const alertSpy = spyOn(window, 'alert');

    component.form.setValue({ username: 'testuser', password: 'password123' });
    component.submitForm();

    expect(alertSpy).toHaveBeenCalledWith('Wrong email, username or password. Please try again.');
    expect(mockUserService.login).toHaveBeenCalledWith('testuser', 'password123');
  });

  it('should not submit the form if the form is invalid', () => {
    const loginSpy = spyOn(mockUserService, 'login');

    component.form.setValue({ username: '', password: '' }); // Invalid form
    component.submitForm();

    expect(loginSpy).not.toHaveBeenCalled(); // Login should not be called
  });
});
