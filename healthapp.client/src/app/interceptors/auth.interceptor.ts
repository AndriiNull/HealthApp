import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpHandler, HttpRequest, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(private authService: AuthService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    let token = this.authService.getToken();

    if (token) {
      console.log("Sending Token in Request:", token); // Debugging log
      if (!token.includes('.')) {
        console.error('Malformed JWT detected in interceptor');
        token = null; // Prevent sending invalid token
      } else {
        req = req.clone({
          setHeaders: { Authorization: `Bearer ${token}` }
        });
      }
    }

    return next.handle(req);
  }
}
