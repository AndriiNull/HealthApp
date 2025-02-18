import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'https://localhost:7212/api/Auth'; // ✅ Set API base URL
  private jwtHelper = new JwtHelperService(); // ✅ JWT helper for decoding
  private tokenKey = 'token';
  private roleKey = 'role';

  constructor(private http: HttpClient) { }

  login(credentials: { email: string; password: string }): Observable<{ token: any; user: { roles?: string[] } }> {
    return this.http.post<{ token: any; user: { roles?: string[] } }>(`${this.apiUrl}/login`, credentials).pipe(
      tap(response => {
        if (response?.token) {
          let tokenString: string;

          // ✅ Extract the token string from `token.result`
          if (typeof response.token === 'object' && 'result' in response.token) {
            tokenString = response.token.result;
          } else {
            tokenString = response.token as string;
          }

          console.log("🔑 Saving Token:", tokenString);
          localStorage.setItem('token', tokenString);  // ✅ Store only the token string

          // ✅ Extract the role from `user.roles`
          if (response.user?.roles && response.user.roles.length > 0) {
            console.log("👤 Saving Roles:", response.user.roles);
            localStorage.setItem('userRoles', JSON.stringify(response.user.roles));  // ✅ Save roles as an array
          } else {
            console.warn("⚠ No roles found for user.");
          }
        } else {
          console.error("🚨 No token received from API!");
        }
      })
    );
  }

  /** ✅ Register a New Patient */
  register(userData: { name: string; surname: string; email: string; password: string; birthdate: string }): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/register`, userData);
  }

  /** ✅ Logout */
  logout(): void {
    localStorage.removeItem(this.tokenKey);
    localStorage.removeItem(this.roleKey);
  }

  /** ✅ Check if user is logged in */
  isLoggedIn(): boolean {
    const token = this.getToken();
    return !!token && !this.jwtHelper.isTokenExpired(token);
  }

  /** ✅ Retrieve User Role */
  getUserRoles(): string[] {
    const token = this.getToken();
    if (!token) return [];

    try {
      const decodedToken = this.jwtHelper.decodeToken(token);
      console.log("Decoded JWT Token:", decodedToken);

      // ✅ Support both single and multiple roles
      const roles = decodedToken?.user_role || decodedToken?.role || [];

      return Array.isArray(roles) ? roles : [roles]; // Ensure it's always an array
    } catch (error) {
      console.error("Error decoding token:", error);
      return [];
    }
  }

  /** ✅ Extract User ID from JWT Token */
  getUserId(): string | null {
    const token = this.getToken();
    if (!token) return null;

    try {
      const decodedToken = this.jwtHelper.decodeToken(token);
      return decodedToken?.PersonId || decodedToken?.sub || null; // Fallback to `sub`
    } catch (error) {
      console.error('Error decoding token:', error);
      return null;
    }
  }
  getPatientId(): string | null {
    const token = this.getToken();
    if (!token) return null;

    try {
      const decodedToken = this.jwtHelper.decodeToken(token);
      console.log('Decoded Token:', decodedToken); // ✅ Debugging log

      return decodedToken?.PatientId || null; // ✅ Explicitly extract `PatientId`
    } catch (error) {
      console.error('❌ Error decoding token:', error);
      return null;
    }
  }

  /** ✅ Check Role-Based Access */
  hasRole(requiredRoles: string[]): boolean {
    const userRoles = this.getUserRoles();
    return requiredRoles.some(role => userRoles.includes(role));
  }

  /** ✅ Get JWT Token for Authenticated Requests */
  getToken(): string | null {
    const token = localStorage.getItem(this.tokenKey);
    return token && token.includes('.') ? token : null; // Validate token format
  }

  /** ✅ Securely store token */
  private setToken(token: string): void {
    if (!token.includes('.')) {
      console.error('Invalid JWT token detected!');
      return;
    }
    localStorage.setItem(this.tokenKey, token);
  }

  /** ✅ Store user role */
  private setRole(role: string): void {
    localStorage.setItem(this.roleKey, role);
  }
}
