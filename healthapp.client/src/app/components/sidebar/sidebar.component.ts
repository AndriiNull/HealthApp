import { Component, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css'],
  standalone: true,
  imports: [CommonModule, RouterModule] // ✅ Ensure RouterModule is imported
})
export class SidebarComponent {
  @Output() toggle = new EventEmitter<void>();
  isCollapsed = false;
  userRole: string | null = null;

  menuItems = [
    { label: 'Dashboard', link: '/dashboard' },
    { label: 'About Us', link: '/about' },
    { label: 'Doctors', link: '/doctors' },
    { label: 'Patients', link: '/patients', requiresDoctor: true }, // ✅ Only for Doctor
    { label: 'Create Appointment', link: '/create-appointment' },
    { label: 'Issues', link: '/issues' },
    { label: 'Timetable', link: '/timetable' }

  ];

  constructor(private authService: AuthService, private router: Router) { }

  ngOnInit(): void {
    this.userRole = localStorage.getItem('userRole'); // ✅ Retrieve stored role
  }

  toggleSidebar() {
    this.isCollapsed = !this.isCollapsed;
    this.toggle.emit();
  }

  isLoggedIn(): boolean {
    return this.authService.isLoggedIn();
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  isDoctor(): boolean {
    return this.userRole === 'Doctor'; // ✅ Role check
  }
}
