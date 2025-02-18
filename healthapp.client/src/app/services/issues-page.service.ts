import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Issue {
  id: number;
  title: string;
  description: string;
  recommendation: string;
  analysis: string;
  status: 'Open' | 'Resolved';
}

export interface Recommendation {
  id: number;
  issueId: number;
  text: string;
}

@Injectable({
  providedIn: 'root'
})
export class IssuesPageService {
  private apiUrl = '/api/Issue';

  constructor(private http: HttpClient) { }

  private getHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders({ Authorization: `Bearer ${token}` });
  }

  getIssues(): Observable<Issue[]> {
    return this.http.get<Issue[]>(`${this.apiUrl}/my-issues`, { headers: this.getHeaders() });
  }

  getRecommendations(issueId: number): Observable<Recommendation[]> {
    return this.http.get<Recommendation[]>(`${this.apiUrl}/recommendations/${issueId}`, { headers: this.getHeaders() });
  }

  addRecommendation(issueId: number, text: string): Observable<void> {
    return this.http.post<void>(
      `${this.apiUrl}/recommendations/${issueId}/add`,
      { text },
      { headers: this.getHeaders() }
    );
  }
}
