import { TestBed } from '@angular/core/testing';

import { UserService } from './user.service';
import {HttpClientTestingModule, HttpTestingController} from '@angular/common/http/testing';
import {User} from '../models/user';

describe('UserService', () => {
  let service: UserService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [UserService],
    });
    service = TestBed.inject(UserService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  it('should create a new user successfully', () => {
    const newUser: User = {
      userId: 1,
      username: 'newuser',
      email: 'newuser@example.com',
      password: 'password123',
      profilePicture: '',
      membershipLevel: 'Gold',
      balance: 100,
    };

    // Mocking the response
    service.createUser(newUser).subscribe((response) => {
      expect(response.username).toBe('newuser');
    });

    const req = httpMock.expectOne(`${UserService.URL}`);
    expect(req.request.method).toBe('POST');
    req.flush(newUser); // Mock response data

    httpMock.verify();
  });

  it('should handle error when creating a user', () => {
    const newUser: User = {
      userId: 1,
      username: 'existinguser',
      email: 'existinguser@example.com',
      password: 'password123',
      profilePicture: '',
      membershipLevel: 'Gold',
      balance: 100,
    };

    // Mocking error response
    service.createUser(newUser).subscribe({
      error: (err) => {
        expect(err.status).toBe(400);
        expect(err.error.message).toBe('This username already exists.');
      },
    });

    const req = httpMock.expectOne(`${UserService.URL}`);
    expect(req.request.method).toBe('POST');
    req.flush(
      { message: 'This username already exists.' },
      { status: 400, statusText: 'Bad Request' }
    );

    httpMock.verify();
  });
});
