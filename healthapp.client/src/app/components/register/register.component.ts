import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatButtonModule
  ]
})
export class RegisterComponent {
  name = '';
  surname = '';
  email = '';
  password = '';
  birthdate = '';
  errorMessage = '';

  constructor(private authService: AuthService, private router: Router) { }

  register() {
    if (!this.name || !this.surname || !this.email || !this.password || !this.birthdate) {
      this.errorMessage = 'All fields are required.';
      return;
    }

    const today = new Date();
    const birthdateDate = new Date(this.birthdate);
    const age = today.getFullYear() - birthdateDate.getFullYear();
    if (age < 18) {
      this.errorMessage = 'You must be at least 18 years old to register.';
      return;
    }

    const userData = {
      name: this.name,
      surname: this.surname,
      email: this.email,
      password: this.password,
      birthdate: this.birthdate
    };

    this.authService.register(userData).subscribe({
      next: () => {
        console.log('✅ Registration successful');
        this.router.navigate(['/login']);
      },
      error: (err) => {
        console.error('❌ Registration failed:', err);
        this.errorMessage = 'Registration failed. Try again.';
      }
    });
  }



  goToLogin() {
    this.router.navigate(['/login']);
  }
}
