import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    const requiredRoles = route.data['roles'] as string[]; // ✅ Get required roles

    if (this.authService.isLoggedIn()) {
      if (!requiredRoles || this.authService.hasRole(requiredRoles)) {
        console.log(`✅ Access granted to: ${state.url}`);
        return true; // ✅ Allow access if logged in and has required role
      } else {
        console.warn(`⛔ Access denied. Required roles: ${requiredRoles.join(', ')}`);
        this.router.navigate(['/403']); // 🔄 Redirect to "Forbidden" page
        return false;
      }
    }

    console.warn(`🔒 User not logged in. Redirecting to login page.`);
    this.router.navigate(['/login'], { queryParams: { returnUrl: state.url } }); // 🔄 Preserve return URL
    return false;
  }
}
