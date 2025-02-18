import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';

@Component({
  selector: 'app-issues',
  standalone: true,
  imports: [CommonModule, FormsModule, MatFormFieldModule, MatInputModule, MatTableModule, MatChipsModule],
  templateUrl: './issues.component.html',
  styleUrls: ['./issues.component.css']
})
export class IssuesComponent implements OnInit {
  issues: any[] = [];
  recommendations: any = {};
  analysis: any = {};
  expandedIssueId: number | null = null;

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.loadIssues();
  }

  loadIssues(): void {
    console.log("🟡 Fetching issues...");

    const token = localStorage.getItem('token');
    if (!token) return;

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    this.http.get('/api/Issue/my-issues', { headers }).subscribe(
      (data: any) => {
        console.log("✅ Issues loaded:", data);
        this.issues = data;
      },
      (error) => console.error("❌ Failed to load issues:", error)
    );
  }

  toggleIssueDetails(issueId: number): void {
    if (this.expandedIssueId === issueId) {
      this.expandedIssueId = null;
      return;
    }

    this.expandedIssueId = issueId;
    this.loadIssueDetails(issueId);
  }

  loadIssueDetails(issueId: number): void {
    const token = localStorage.getItem('token');
    if (!token) return;

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    this.http.get(`/api/Issue/${issueId}`, { headers }).subscribe(
      (data: any) => {
        console.log("✅ Issue details:", data);
        this.recommendations[issueId] = data.recommendations;
      },
      (error) => console.error("❌ Failed to load issue details:", error)
    );

    this.http.get(`/api/Issue/analysis/${issueId}`, { headers }).subscribe(
      (data: any) => {
        console.log("📊 Analysis Data:", data);
        this.analysis[issueId] = data;
      },
      (error) => console.error("❌ Failed to load analysis data:", error)
    );
  }
}
