import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms'; // ✅ Import FormsModule
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  standalone: true,
  imports: [CommonModule, FormsModule] // ✅ Add FormsModule
})
export class LoginComponent {
  email = '';
  password = '';
  errorMessage = '';

  constructor(private authService: AuthService, private router: Router) { }

  login() {
    const credentials = { email: this.email, password: this.password };
    this.authService.login(credentials).subscribe({
      next: () => {
        console.log('✅ Login successful!');
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        console.error('❌ Login error:', err);
        this.errorMessage = 'Invalid credentials. Please try again.';
      }
    });
  }
  goToRegister() {
    this.router.navigate(['/register']);
  }
}
