import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class AppointmentService {
  private apiUrl = 'https://localhost:7212/api/Appointment';

  constructor(private http: HttpClient, private authService: AuthService) { }

  getAvailableDates(doctorId: number): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/available-dates/${doctorId}`);
  }

  getAvailableSlots(doctorId: number, date: string): Observable<string[]> {
    const formattedDate = new Date(date).toISOString().split('T')[0]; // ✅ Convert to YYYY-MM-DD
    const token = this.authService.getToken(); // ✅ Retrieve JWT token

    if (!token) {
      console.error("❌ Authentication error: No token found. Please log in again.");
      return throwError(() => new Error("User is not authenticated."));
    }

    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });

    console.log(`Formatted Date: ${formattedDate}`); // Debugging
    console.log(`Sending JWT Token: ${token}`); // Debugging
    return this.http.get<string[]>(`${this.apiUrl}/availableSlots/${doctorId}?date=${formattedDate}`, { headers });
  }

  bookAppointment(appointmentData: any): Observable<any> {
    const token = this.authService.getToken(); // ✅ Retrieve JWT token

    if (!token) {
      console.error("❌ Authentication error: No token found. Please log in again.");
      return throwError(() => new Error("User is not authenticated."));
    }

    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });

    const formattedData = {
      doctorID: appointmentData.doctorId, // Match API's expected key name
      patientId: Number(appointmentData.patientId), // Convert to number
      issueId: appointmentData.issueId,
      startTime: new Date(appointmentData.startTime).toISOString() // Convert to ISO 8601
    };

    return this.http.post(`${this.apiUrl}/book`, formattedData, { headers });
  }
}
