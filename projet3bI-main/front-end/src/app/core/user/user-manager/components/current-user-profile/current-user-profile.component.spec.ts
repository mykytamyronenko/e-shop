import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CurrentUserProfileComponent } from './current-user-profile.component';
import {UserService} from '../../services/user.service';
import {RatingService} from '../../../../../features/rating/services/rating.service';
import {EventBusService} from '../../../../../shared/services/event-bus/event-bus.service';
import {User} from '../../models/user';
import {Rating} from '../../../../../features/rating/model/Rating';
import {of, Subject, throwError} from 'rxjs';

describe('CurrentUserProfileComponent', () => {
  let component: CurrentUserProfileComponent;
  let fixture: ComponentFixture<CurrentUserProfileComponent>;
  let mockUserService: jasmine.SpyObj<UserService>;
  let mockRatingService: jasmine.SpyObj<RatingService>;
  let mockEventBusService: jasmine.SpyObj<EventBusService>;

  const mockUser: User = {
    userId: 1,
    username: 'testuser',
    email: 'testuser@example.com',
    password: 'password',
    profilePicture: 'profile.jpg',
    membershipLevel: 'Bronze',
    balance: 100
  };

  const mockRatings: Rating[] = [
    { userId: 1, score: 4 , comment:"wow",createdAt:Date.now().toString(),reviewerId:1},
    { userId: 1, score: 5 , comment:"wow2",createdAt:Date.now().toString(),reviewerId:2},
    { userId: 1, score: 3 , comment:"wow3",createdAt:Date.now().toString(),reviewerId:3}
  ];

  beforeEach(async () => {
    // Create mock services
    mockUserService = jasmine.createSpyObj('UserService', ['getById', 'getUserIdFromToken', 'update']);
    mockRatingService = jasmine.createSpyObj('RatingService', ['getAll']);
    mockEventBusService = jasmine.createSpyObj('EventBusService', ['listen']);

    // Provide mock services
    await TestBed.configureTestingModule({
      declarations: [CurrentUserProfileComponent],
      providers: [
        { provide: UserService, useValue: mockUserService },
        { provide: RatingService, useValue: mockRatingService },
        { provide: EventBusService, useValue: mockEventBusService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(CurrentUserProfileComponent);
    component = fixture.componentInstance;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should fetch user data and ratings on ngOnInit', () => {
    // Mock service responses
    mockUserService.getUserIdFromToken.and.returnValue('1');
    mockUserService.getById.and.returnValue(of(mockUser));
    mockRatingService.getAll.and.returnValue(of(mockRatings));

    // Initialize component
    fixture.detectChanges();

    // Verify the component state
    expect(component.user).toEqual(mockUser);
    expect(component.ratings).toEqual(mockRatings);
    expect(component.score).toBe(4); // Average score: (4 + 5 + 3) / 3 = 4
  });

  it('should calculate the correct score based on ratings', () => {
    // Test with ratings
    component.ratings = mockRatings;
    component.calculateUserScore();

    expect(component.score).toBe(4); // (4 + 5 + 3) / 3 = 4
  });

  it('should handle empty ratings list and set score to 0', () => {
    component.ratings = [];
    component.calculateUserScore();

    expect(component.score).toBe(0);
  });

  it('should subscribe to USER_UPDATED event and update user', () => {
    // Setup mock EventBus
    const userUpdatedSubject = new Subject<any>();
    mockEventBusService.listen.and.returnValue(userUpdatedSubject.asObservable());

    // Mock service method for update
    mockUserService.update.and.returnValue(of(undefined));

    // Trigger ngOnInit
    fixture.detectChanges();

    // Emit USER_UPDATED event
    const updatedUser: User = { ...mockUser, username: 'updateduser' };
    userUpdatedSubject.next({ object: updatedUser });

    // Verify update method was called
    expect(mockUserService.update).toHaveBeenCalledWith(updatedUser);
  });



  it('should handle error when fetching user data', () => {
    // Mock the services to simulate an error
    mockUserService.getUserIdFromToken.and.returnValue('1');

    // Simulating an error in fetching user
    mockUserService.getById.and.returnValue(throwError(() => new Error('Error retrieving user')));
    mockRatingService.getAll.and.returnValue(of(mockRatings));

    spyOn(console, 'error'); // Spy on console.error to check if error is logged

    // Trigger ngOnInit
    fixture.detectChanges();

    // Verify error handling
    expect(console.error).toHaveBeenCalledWith('Error retrieving user:', jasmine.any(Error));
  });

  it('should handle error when fetching ratings', () => {
    // Mock the services to simulate an error in fetching ratings
    mockUserService.getUserIdFromToken.and.returnValue('1');
    mockUserService.getById.and.returnValue(of(mockUser));
    mockRatingService.getAll.and.returnValue(of([])); // Empty ratings

    spyOn(console, 'error'); // Spy on console.error to check if error is logged

    // Trigger ngOnInit
    fixture.detectChanges();

    // Verify error handling
    expect(console.error).toHaveBeenCalledWith('Error retrieving user:', jasmine.any(Object));
  });
});
