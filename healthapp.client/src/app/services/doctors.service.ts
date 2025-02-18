import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DoctorService {
  private apiUrl = '/api/doctors/summaries';  // ✅ Backend API

  constructor(private http: HttpClient) { }

  getAllDoctors(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}`);
  }

  // ✅ Fetch the next available appointment for a doctor
  getNextAvailableSlot(doctorId: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${doctorId}/next-appointment`);
  }
  getDoctorById(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }
}
