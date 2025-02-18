import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class IssueService {
  private apiUrl = 'https://localhost:7212/api/Issue'; // ✅ Ensure correct API URL

  constructor(private http: HttpClient, private authService: AuthService) { }

  /** ✅ Centralized Method for Setting Headers */
  private getAuthHeaders(): HttpHeaders {
    const token = this.authService.getToken()?.trim();

    if (!token || !token.includes('.')) {
      console.error("🚨 No valid JWT token found. User may not be authenticated.");
      throw new Error("User is not authenticated or token is invalid.");
    }

    return new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });
  }

  /** ✅ Create a new issue */
  createIssue(description: string): Observable<any> {
    const patientId = this.authService.getPatientId();
    if (!patientId) {
      console.error('❌ No PatientId found in JWT.');
      return throwError(() => new Error('No PatientId found in JWT'));
    }

    const issueData = { PatientId: patientId, Description: description };
    console.log("📢 Sending Issue Data:", issueData); // ✅ Debug log

    return this.http.post(`${this.apiUrl}`, issueData, { headers: this.getAuthHeaders() }).pipe(
      catchError((error: any) => {
        console.error("❌ Error creating issue:", error);
        return throwError(() => new Error(error.message || 'Unknown error occurred.'));
      })
    );
  }
  getUserIssues(): Observable<any> {
    const token = this.authService.getToken();
    if (!token) {
      console.error("❌ No token found. Authentication required.");
      return throwError(() => new Error("User is not authenticated."));
    }

    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });

    console.log("Fetching user issues from API with token:", token);
    return this.http.get(`${this.apiUrl}/my-issues`, { headers });
  }


  /** ✅ Fetch issues for a given patient */
  getIssuesByPatient(patientId: string): Observable<any> {
    console.log(`📢 Fetching issues for patientId: ${patientId}`);

    return this.http.get(`${this.apiUrl}/patient?patientId=${patientId}`, { headers: this.getAuthHeaders() }).pipe(
      catchError((error: any) => {
        console.error("❌ Error fetching issues:", error);
        return throwError(() => new Error(error.message || 'Unknown error occurred.'));
      })
    );
  }
}
