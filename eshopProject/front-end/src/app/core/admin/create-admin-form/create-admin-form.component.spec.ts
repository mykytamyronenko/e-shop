import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateAdminFormComponent } from './create-admin-form.component';
import {ReactiveFormsModule} from '@angular/forms';

describe('CreateAdminFormComponent', () => {
  let component: CreateAdminFormComponent;
  let fixture: ComponentFixture<CreateAdminFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreateAdminFormComponent ],
      imports: [ ReactiveFormsModule ],  // Importer ReactiveFormsModule pour les formulaires réactifs
    }).compileComponents();

    fixture = TestBed.createComponent(CreateAdminFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should create a valid form', () => {
    const form = component.form;

    // Vérifier que les groupes de formulaires sont créés correctement
    expect(form.contains('main')).toBeTruthy();
    expect(form.contains('other')).toBeTruthy();

    // Vérifier les champs dans 'main'
    expect(form.get('main.username')).toBeTruthy();
    expect(form.get('main.email')).toBeTruthy();
    expect(form.get('main.password')).toBeTruthy();

    // Vérifier les champs dans 'other'
    expect(form.get('other.role')).toBeTruthy();
    expect(form.get('other.profilePicture')).toBeTruthy();
    expect(form.get('other.membershipLevel')).toBeTruthy();
    expect(form.get('other.rating')).toBeTruthy();
    expect(form.get('other.status')).toBeTruthy();
  });

  it('should emit userCreated event with valid form data', () => {
    // Simuler un utilisateur valide
    component.form.get('main.username')?.setValue('admin1');
    component.form.get('main.email')?.setValue('admin1@example.com');
    component.form.get('main.password')?.setValue('password1');
    component.form.get('other.role')?.setValue('admin');

    // Créer un fichier simulé
    const mockFile = new File([''], 'profile.jpg', { type: 'image/jpeg' });
    component.form.get('other.profilePicture')?.setValue(mockFile);

    component.form.get('other.membershipLevel')?.setValue('Gold');
    component.form.get('other.rating')?.setValue(4.5);
    component.form.get('other.status')?.setValue('active');

    // Espionner la méthode 'emit' du EventEmitter
    const emitSpy = spyOn(component.userCreated, 'emit');

    // Soumettre le formulaire
    component.submitForm();

    // Vérifier que 'emit' a été appelé
    expect(emitSpy).toHaveBeenCalled();

    // Vérifier les données émises avec l'événement
    const emittedData = emitSpy.calls.mostRecent().args[0];  // Utiliser 'args' au lieu de 'arguments'

    // @ts-ignore
    expect(emittedData.user.username).toBe('admin1');
    // @ts-ignore
    expect(emittedData.user.email).toBe('admin1@example.com');
    // @ts-ignore
    expect(emittedData.user.password).toBe('password1');
    // @ts-ignore
    expect(emittedData.user.role).toBe('admin');
    // @ts-ignore
    expect(emittedData.user.profilePicture).toBe('');  // Comme il est initialisé à une chaîne vide, il devrait être vide
    // @ts-ignore
    expect(emittedData.user.membershipLevel).toBe('Gold');
    // @ts-ignore
    expect(emittedData.user.rating).toBe(4.5);
    // @ts-ignore
    expect(emittedData.user.status).toBe('active');
    // @ts-ignore
    expect(emittedData.imageFile).toBe(mockFile);  // Vérifier l'image avec le fichier simulé
  });





  it('should display alert when form is submitted', () => {
    // Simuler un utilisateur valide
    component.form.get('main.username')?.setValue('admin1');
    component.form.get('main.email')?.setValue('admin1@example.com');
    component.form.get('main.password')?.setValue('password1');
    component.form.get('other.role')?.setValue('admin');
    component.form.get('other.profilePicture')?.setValue('profile.jpg');
    component.form.get('other.membershipLevel')?.setValue('Gold');
    component.form.get('other.rating')?.setValue(4.5);
    component.form.get('other.status')?.setValue('active');

    spyOn(window, 'alert');  // Espionner l'appel à alert

    component.submitForm();  // Soumettre le formulaire

    expect(window.alert).toHaveBeenCalledWith('User created.');  // Vérifier que l'alerte est affichée
  });

  it('should update profilePicture on file change', () => {
    const event = { target: { files: [{ name: 'image.png' }] } };
    spyOn(component, 'onFileChange');  // Espionner la méthode onFileChange

    component.onFileChange(event);

    expect(component.onFileChange).toHaveBeenCalled();
    expect(component.form.get('other.profilePicture')?.value).toBe(event.target.files[0]);
  });
});
