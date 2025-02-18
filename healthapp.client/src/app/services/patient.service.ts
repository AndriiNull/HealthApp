import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PatientService {
  private apiUrl = 'https://yourapi.com/api/patients'; // ✅ Adjust with your actual API URL

  constructor(private http: HttpClient) { }

  // ✅ Fetch all patients (for doctors)
  getAllPatients(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/all`);
  }

  // ✅ Fetch a single patient's details
  getPatientById(patientId: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${patientId}`);
  }
}
