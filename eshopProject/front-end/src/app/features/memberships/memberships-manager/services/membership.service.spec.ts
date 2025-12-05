import { TestBed } from '@angular/core/testing';

import { MembershipService } from './membership.service';
import {Membership} from '../models/membership';
import {HttpClientTestingModule, HttpTestingController} from '@angular/common/http/testing';
import {API_URLS, environment} from '../../../../../environments/environment';

describe('MembershipService', () => {
  let service: MembershipService;
  let httpMock: HttpTestingController;

  const BASE_URL = `${environment.BASE_URL}/${API_URLS.MEMBERSHIP}`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [MembershipService]
    });

    service = TestBed.inject(MembershipService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    // Verify no outstanding HTTP requests after each test
    httpMock.verify();
  });

  describe('getAll', () => {
    it('should call HttpClient.get() with the correct URL and return an observable of Membership[]', () => {
      const mockMemberships: Membership[] = [
        { membershipId: 1, name: 'Gold', price: 100, discountPercentage: 20, description: 'Gold Membership' },
        { membershipId: 2, name: 'Silver', price: 50, discountPercentage: 10, description: 'Silver Membership' }
      ];

      service.getAll().subscribe((memberships) => {
        expect(memberships).toEqual(mockMemberships);
      });

      const req = httpMock.expectOne(`${BASE_URL}`);
      expect(req.request.method).toBe('GET');
      expect(req.request.withCredentials).toBeTrue();

      req.flush(mockMemberships);
    });

    it('should handle an error from HttpClient.get()', () => {
      service.getAll().subscribe({
        next: () => fail('Expected an error, not memberships'),
        error: (error) => expect(error.status).toEqual(500)
      });

      const req = httpMock.expectOne(`${BASE_URL}`);
      req.flush('Server error', { status: 500, statusText: 'Internal Server Error' });
    });
  });

  describe('getMembershipById', () => {
    it('should call HttpClient.get() with the correct URL and return an observable of Membership', () => {
      const membershipId = 1;
      const mockMembership: Membership = { membershipId: 1, name: 'Gold', price: 100, discountPercentage: 20, description: 'Gold Membership' };

      service.getMembershipById(membershipId).subscribe((membership) => {
        expect(membership).toEqual(mockMembership);
      });

      const req = httpMock.expectOne(`${BASE_URL}/${membershipId}`);
      expect(req.request.method).toBe('GET');
      expect(req.request.withCredentials).toBeTrue();

      req.flush(mockMembership);
    });

    it('should handle an error from HttpClient.get()', () => {
      const membershipId = 1;

      service.getMembershipById(membershipId).subscribe({
        next: () => fail('Expected an error, not a membership'),
        error: (error) => expect(error.status).toEqual(404)
      });

      const req = httpMock.expectOne(`${BASE_URL}/${membershipId}`);
      req.flush('Not found', { status: 404, statusText: 'Not Found' });
    });
  });

  describe('setSelectedMembership', () => {
    it('should update the value of selectedMembership in the BehaviorSubject', () => {
      const mockMembership: Membership = { membershipId: 1, name: 'Gold', price: 100, discountPercentage: 20, description: 'Gold Membership' };

      // Before setting, the selected membership should be null
      service.getSelectedMembership().subscribe((membership) => {
        expect(membership).toBeNull();
      });

      service.setSelectedMembership(mockMembership);

      // After setting, the selected membership should be updated
      service.getSelectedMembership().subscribe((membership) => {
        expect(membership).toEqual(mockMembership);
      });
    });
  });

  describe('getSelectedMembership', () => {
    it('should return an observable that emits the current value of selectedMembership', () => {
      const mockMembership: Membership = { membershipId: 2, name: 'Silver', price: 50, discountPercentage: 10, description: 'Silver Membership' };

      service.setSelectedMembership(mockMembership);

      service.getSelectedMembership().subscribe((membership) => {
        expect(membership).toEqual(mockMembership);
      });
    });

    it('should initially emit null if no membership is set', () => {
      service.getSelectedMembership().subscribe((membership) => {
        expect(membership).toBeNull();
      });
    });
  });
});
