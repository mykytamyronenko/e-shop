import { Router } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import {NavbarComponent} from './navbar.component';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {UserService} from '../../user/user-manager/services/user.service';

describe('NavbarComponent', () => {
  let component: NavbarComponent;
  let fixture: ComponentFixture<NavbarComponent>;
  let mockUserService: jasmine.SpyObj<UserService>;
  let mockRouter: jasmine.SpyObj<Router>;
  let mockActivatedRoute: jasmine.SpyObj<ActivatedRoute>;

  beforeEach(async () => {
    mockUserService = jasmine.createSpyObj('UserService', [
      'isLoggedIn$', 'getById', 'getUserIdFromToken', 'logout', 'updateLoginStatus', 'getRoleFromToken'
    ]);

    mockRouter = jasmine.createSpyObj('Router', ['navigate']);
    mockActivatedRoute = jasmine.createSpyObj('ActivatedRoute', ['params']);

    mockUserService.getUserIdFromToken.and.returnValue('1');
    mockUserService.getRoleFromToken.and.returnValue('admin');

    // Properly mock the observable isLoggedIn$
    mockUserService.isLoggedIn$ = of(true); // Directly assign the observable value

    const mockUser = {
      userId: 1,
      username: 'testuser',
      email: 'test@example.com',
      password: 'password123',
      profilePicture: 'profile.jpg',
      membershipLevel: 'Gold',
      balance: 100
    };
    mockUserService.getById.and.returnValue(of(mockUser));

    await TestBed.configureTestingModule({
      declarations: [NavbarComponent],
      providers: [
        { provide: UserService, useValue: mockUserService },
        { provide: Router, useValue: mockRouter },
        { provide: ActivatedRoute, useValue: mockActivatedRoute }
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(NavbarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the Navbar component', () => {
    expect(component).toBeTruthy();
  });

  it('should load user data when component is initialized', () => {
    expect(mockUserService.getById).toHaveBeenCalledWith(1);  // Ensure getById is called with correct user ID
    expect(component.user.username).toBe('testuser');  // Check if user data is populated
    expect(component.profileImageUrl).toBe('http://localhost:5185/profile.jpg');  // Check profile image URL
  });

  it('should toggle theme correctly', () => {
    component.toggleTheme();
    expect(component.isDarkTheme).toBe(false);  // After toggling, the theme should be light
    expect(document.body.className).toBe('light-theme');  // Verify that class on body element is updated
  });

  it('should call logout and navigate on confirmation', () => {
    const logoutSpy = spyOn(mockUserService, 'logout').and.returnValue(of(null));
    const updateLoginStatusSpy = spyOn(mockUserService, 'updateLoginStatus');
    const navigateSpy = spyOn(mockRouter, 'navigate');

    component.logout();  // Trigger logout

    expect(logoutSpy).toHaveBeenCalled();  // Check if logout was called
    expect(updateLoginStatusSpy).toHaveBeenCalledWith(false);  // Check if login status was updated
    expect(navigateSpy).toHaveBeenCalledWith(['/']);  // Check if the user was navigated to home
  });

  it('should show default profile image if profile image loading fails', () => {
    const profileImageError = spyOn(component, 'onImageError');  // Spy on image error handling

    mockUserService.getById.and.returnValue(of({
      userId: 1,
      username: 'testuser',
      email: 'test@example.com',
      password: 'password123',
      profilePicture: '',
      membershipLevel: 'Gold',
      balance: 100
    }));

    component.loadUserProfileImage();  // Call method to load user profile image

    expect(profileImageError).toHaveBeenCalled();  // Ensure error handler was called due to missing profile picture
  });

  it('should update theme based on localStorage value on init', () => {
    localStorage.setItem('theme', 'dark');  // Set dark theme in localStorage

    component.ngOnInit();  // Trigger component initialization

    expect(component.isDarkTheme).toBe(true);  // Theme should be dark
    expect(document.body.className).toBe('dark-theme');  // Body class should reflect dark theme
  });
});

