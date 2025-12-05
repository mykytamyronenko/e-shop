import { TestBed } from '@angular/core/testing';

import { RatingService } from './rating.service';
import {HttpClientTestingModule, HttpTestingController} from '@angular/common/http/testing';
import {API_URLS, environment} from '../../../../environments/environment';
import {Rating} from '../model/Rating';

describe('RatingService', () => {
  let service: RatingService;
  let httpMock: HttpTestingController;
  const mockUrl = `${environment.BASE_URL}/${API_URLS.RATING}`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [RatingService]
    });

    service = TestBed.inject(RatingService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    // Ensure no outstanding requests
    httpMock.verify();
  });

  describe('getAll', () => {
    it('should make a GET request and return an array of ratings', () => {
      const mockRatings: Rating[] = [
        { userId: 1, reviewerId: 2, score: 5, comment: 'Great service!', createdAt: '2024-01-01T12:00:00Z' },
        { userId: 2, reviewerId: 3, score: 4, comment: 'Good experience', createdAt: '2024-02-02T12:00:00Z' }
      ];

      service.getAll().subscribe((ratings) => {
        expect(ratings.length).toBe(2);
        expect(ratings).toEqual(mockRatings);
      });

      const req = httpMock.expectOne(mockUrl);
      expect(req.request.method).toBe('GET');
      req.flush(mockRatings);
    });

    it('should handle an error from the server', () => {
      service.getAll().subscribe({
        next: () => fail('Should have failed with 500 status'),
        error: (error) => {
          expect(error.status).toBe(500);
        }
      });

      const req = httpMock.expectOne(mockUrl);
      expect(req.request.method).toBe('GET');
      req.flush('Server error', { status: 500, statusText: 'Internal Server Error' });
    });
  });

  describe('create', () => {
    it('should make a POST request and return the created rating', () => {
      const newRating: Rating = {
        userId: 1,
        reviewerId: 2,
        score: 5,
        comment: 'Excellent service!',
        createdAt: new Date().toISOString()
      };

      service.create(newRating).subscribe((rating) => {
        expect(rating).toEqual(newRating);
      });

      const req = httpMock.expectOne(mockUrl);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(newRating);
      expect(req.request.withCredentials).toBeTrue();
      req.flush(newRating);
    });

    it('should handle an error when the server returns an error response', () => {
      const newRating: Rating = {
        userId: 1,
        reviewerId: 2,
        score: 5,
        comment: 'Excellent service!',
        createdAt: new Date().toISOString()
      };

      service.create(newRating).subscribe({
        next: () => fail('Should have failed with 400 status'),
        error: (error) => {
          expect(error.status).toBe(400);
          expect(error.statusText).toBe('Bad Request');
        }
      });

      const req = httpMock.expectOne(mockUrl);
      expect(req.request.method).toBe('POST');
      req.flush('Invalid data', { status: 400, statusText: 'Bad Request' });
    });
  });

  describe('getById', () => {
    it('should make a GET request to the correct URL and return a rating', () => {
      const mockRating: Rating = {
        userId: 1,
        reviewerId: 2,
        score: 5,
        comment: 'Amazing experience!',
        createdAt: '2024-03-03T12:00:00Z'
      };

      const ratingId = 1;

      service.getById(ratingId).subscribe((rating) => {
        expect(rating).toEqual(mockRating);
      });

      const req = httpMock.expectOne(`${mockUrl}/${ratingId}`);
      expect(req.request.method).toBe('GET');
      req.flush(mockRating);
    });

    it('should handle an error when the rating is not found', () => {
      const ratingId = 999;

      service.getById(ratingId).subscribe({
        next: () => fail('Should have failed with 404 status'),
        error: (error) => {
          expect(error.status).toBe(404);
          expect(error.statusText).toBe('Not Found');
        }
      });

      const req = httpMock.expectOne(`${mockUrl}/${ratingId}`);
      expect(req.request.method).toBe('GET');
      req.flush('Rating not found', { status: 404, statusText: 'Not Found' });
    });
  });
});
