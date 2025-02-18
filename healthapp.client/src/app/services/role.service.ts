import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class RoleService {
  private apiUrl = 'https://localhost:7212/api/roles';

  constructor(private http: HttpClient) { }

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('token'); // Get token from localStorage
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  }

  getUsers(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/users`, { headers: this.getAuthHeaders() });
  }

  assignRole(data: { personId: number; role: string }): Observable<any> {
    return this.http.post(`${this.apiUrl}/assign`, data, { headers: this.getAuthHeaders() });
  }

  /** ✅ New API Methods for Licenses **/
  getDoctors(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/doctors`, { headers: this.getAuthHeaders() });
  }

  getLicences(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/licences`, { headers: this.getAuthHeaders() });
  }

  assignLicence(data: { doctorId: number; licenceId: number }): Observable<any> {
    return this.http.post(`${this.apiUrl}/assign-licence`, data, { headers: this.getAuthHeaders() });
  }
}
