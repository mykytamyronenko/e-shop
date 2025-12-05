
import { of } from 'rxjs';
import {AdminComponent} from './admin.component';
import {ComponentFixture, TestBed} from '@angular/core/testing';
import {UserService} from '../user/user-manager/services/user.service';
import {AdminUser} from '../user/user-manager/models/user'; // Pour simuler des réponses de type Observable

describe('AdminComponent', () => {
  let component: AdminComponent;
  let fixture: ComponentFixture<AdminComponent>;
  let mockUserService: jasmine.SpyObj<UserService>; // Mock du UserService

  beforeEach(async () => {
    // Créer un mock du service UserService
    mockUserService = jasmine.createSpyObj('UserService', ['getAll', 'createAdmin']);

    // Définir le comportement du mock
    mockUserService.getAll.and.returnValue(of([
      {
        userId: 1,
        username: 'admin1',
        email: 'admin1@example.com',
        password: 'password1',
        role: 'admin',
        profilePicture: 'pic1.jpg',
        membershipLevel: 'premium',
        rating: 4.5,
        status: 'active',
        balance: 100
      },
      {
        userId: 2,
        username: 'admin2',
        email: 'admin2@example.com',
        password: 'password2',
        role: 'admin',
        profilePicture: 'pic2.jpg',
        membershipLevel: 'basic',
        rating: 3.8,
        status: 'active',
        balance: 50
      }
    ])); // Mock de la réponse de getAll
    mockUserService.createAdmin.and.returnValue(of({
      userId: 3,
      username: 'admin3',
      email: 'admin3@example.com',
      password: 'password3',
      role: 'admin',
      profilePicture: 'pic3.jpg',
      membershipLevel: 'premium',
      rating: 4.7,
      status: 'active',
      balance: 150
    })); // Mock de la réponse de createAdmin

    await TestBed.configureTestingModule({
      declarations: [ AdminComponent ],
      providers: [
        { provide: UserService, useValue: mockUserService } // Remplacer UserService par le mock
      ]
    })
      .compileComponents();

    fixture = TestBed.createComponent(AdminComponent);
    component = fixture.componentInstance;
    fixture.detectChanges(); // Détecter les changements après l'initialisation
  });

  it('should create the component', () => {
    expect(component).toBeTruthy(); // Vérifier que le composant est bien créé
  });

  it('should load admin users on init', () => {
    // Appeler ngOnInit manuellement pour s'assurer que getAll a été appelé
    component.ngOnInit();
    expect(mockUserService.getAll).toHaveBeenCalled(); // Vérifier que getAll a été appelé
    expect(component.usersAdmin.length).toBe(2); // Vérifier qu'il y a 2 utilisateurs administrateurs chargés
    expect(component.usersAdmin[0].username).toBe('admin1'); // Vérifier le nom de l'utilisateur administrateur
  });

  it('should create a new admin user', () => {
    // Appeler la méthode createAdmin avec un utilisateur de test
    const newUser: AdminUser = {
      userId: 4,
      username: 'admin4',
      email: 'admin4@example.com',
      password: 'password4',
      role: 'admin',
      profilePicture: 'pic4.jpg',
      membershipLevel: 'basic',
      rating: 4.0,
      status: 'active',
      balance: 120
    };
    component.createAdmin({ user: newUser });
    expect(mockUserService.createAdmin).toHaveBeenCalledWith(newUser, undefined); // Vérifier que createAdmin a été appelé avec les bons arguments
    expect(component.usersAdmin.length).toBe(3); // Vérifier que l'utilisateur a bien été ajouté à la liste
    expect(component.usersAdmin[2].username).toBe('admin3'); // Vérifier le nom du nouvel utilisateur administrateur
  });
});

