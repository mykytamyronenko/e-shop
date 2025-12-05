import { TestBed } from '@angular/core/testing';

import { UserMembershipService } from './user-membership.service';
import {HttpClientTestingModule, HttpTestingController} from '@angular/common/http/testing';
import {API_URLS, environment} from '../../../../../environments/environment';
import {UserMembership} from '../models/user-membership';

describe('UserMembershipService', () => {
  let service: UserMembershipService;
  let httpMock: HttpTestingController;

  const baseUrl = `${environment.BASE_URL}/${API_URLS.USER_MEMBERSHIP}`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule], // Import the HttpClientTestingModule
      providers: [UserMembershipService] // Provide the service
    });

    service = TestBed.inject(UserMembershipService); // Inject the service
    httpMock = TestBed.inject(HttpTestingController); // Inject the HttpTestingController
  });

  afterEach(() => {
    httpMock.verify(); // Ensure no pending HTTP requests
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should retrieve all user memberships', () => {
    const mockUserMemberships: UserMembership[] = [
      { userMembershipId: 1, userId: 101, membershipId: 1, startDate: '2024-01-01', endDate: '2025-01-01', status: 'active' },
      { userMembershipId: 2, userId: 102, membershipId: 2, startDate: '2024-02-01', endDate: '2025-02-01', status: 'active' }
    ];

    // Call the getAll method
    service.getAll().subscribe(memberships => {
      expect(memberships.length).toBe(2);
      expect(memberships).toEqual(mockUserMemberships);
    });

    // Expect an HTTP GET request to be made to the correct URL
    const req = httpMock.expectOne(`${baseUrl}`);
    expect(req.request.method).toBe('GET');
    req.flush(mockUserMemberships); // Mock the response with the mock data
  });

  it('should create a new user membership', () => {
    const newUserMembership: UserMembership = {
      userMembershipId: 3,
      userId: 103,
      membershipId: 3,
      startDate: '2024-03-01',
      endDate: '2025-03-01',
      status: 'active'
    };

    // Mock the POST request and its response
    service.create(newUserMembership).subscribe(membership => {
      expect(membership).toEqual(newUserMembership);
    });

    // Expect an HTTP POST request to be made to the correct URL
    const req = httpMock.expectOne(`${baseUrl}`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(newUserMembership);
    req.flush(newUserMembership); // Mock the response with the new membership
  });
});
