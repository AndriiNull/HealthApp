import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-timetable',
  standalone: true,
  imports: [CommonModule, FormsModule, DatePipe],
  templateUrl: './timetable.component.html',
  styleUrls: ['./timetable.component.css']
})
export class TimetableComponent implements OnInit {
  appointments: any[] = [];
  selectedDate: string = new Date().toISOString().split('T')[0];
  expandedAppointmentId: number | null = null;
  patientIssues: any = {};
  issueAnalysis: any = {};
  previousAppointments: any = {};
  newRecommendationText: string = '';
  newAnalysisText: string = '';
  activeTab: string = 'recommendations';
  showAddCategory: boolean = false;

  categories: { id: number; name: string; referenceValue?: number }[] = [];
  selectedCategory: string = '';

  newCategoryName: string = '';
  newCategoryReference: string = ''; // ✅ Now a string

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.loadAppointments();
    this.loadCategories();
  }

  loadAppointments() {
    console.log("🟡 loadAppointments() called for date:", this.selectedDate);

    const token = localStorage.getItem('token');
    if (!token) {
      console.warn("⚠ No token found, authentication required.");
      return;
    }

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    this.http.get(`/api/Appointment/${this.selectedDate}`, { headers }).subscribe(
      (data: any) => {
        console.log("✅ Appointments received:", data);
        this.appointments = data;
      },
      (error) => {
        console.error("❌ Failed to load appointments:", error);
      }
    );
  }

  toggleAppointmentDetails(appointmentId: number, issueId: number | undefined) {
    if (!issueId) {
      console.error("❌ Error: `issueId` is undefined for appointmentId", appointmentId);
      return;
    }

    if (this.expandedAppointmentId === appointmentId) {
      this.expandedAppointmentId = null;
      return;
    }

    this.expandedAppointmentId = appointmentId;
    this.loadIssueDetails(issueId, appointmentId);
  }

  // ✅ Fetch issue details, recommendations, and analysis
  loadIssueDetails(issueId: number, appointmentId: number) {
    const token = localStorage.getItem('token');
    if (!token) return;
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    this.http.get(`/api/Issue/${issueId}`, { headers }).subscribe(
      (data: any) => {
        console.log("✅ Issue Details:", data);
        this.patientIssues[appointmentId] = data;
      },
      (error) => {
        console.error("❌ Failed to load issue details:", error);
      }
    );

    this.http.get(`/api/Issue/analysis/${issueId}`, { headers }).subscribe(
      (data: any) => {
        console.log("📊 Analysis Data:", data);
        this.issueAnalysis[appointmentId] = data;
      },
      (error) => {
        console.error("❌ Failed to load analysis data:", error);
      }
    );
  }

  // ✅ Add recommendation WITHOUT collapsing the issue details
  addRecommendation(issueId: number) {
    if (!this.newRecommendationText.trim()) return;

    const token = localStorage.getItem('token');
    if (!token) return;

    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });

    this.http.post(`/api/recommendations/${issueId}/add`, "\""+this.newRecommendationText+"\"", { headers }).subscribe(
      () => {
        this.loadIssueDetails(issueId, this.expandedAppointmentId!); // ✅ Refresh without collapsing
        this.newRecommendationText = '';
      },
      (error) => {
        console.error("❌ Failed to add recommendation:", error);
      }
    );
  }

  loadCategories() {
    const token = localStorage.getItem('token');
    if (!token) return;

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    this.http.get<{ id: number, name: string, referenceValue?: number }[]>('/api/Analysis/categories', { headers }).subscribe(
      (data) => {
        console.log("✅ Categories Loaded:", data);
        this.categories = data;
      },
      (error) => {
        console.error('❌ Failed to load categories:', error);
      }
    );
  }
  addCategory() {
    if (!this.newCategoryName.trim() || !this.newCategoryReference.trim()) return;

    const token = localStorage.getItem('token');
    if (!token) return;

    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });

    const categoryData = {
      name: this.newCategoryName,
      referenceValue: this.newCategoryReference, // ✅ Sending as string
      token: token // ✅ Sending token inside the request body
    };

    this.http.post('/api/Analysis/categories', categoryData, { headers }).subscribe(
      (response: any) => {
        console.log("✅ Category Added:", response);
        this.categories.push({
          id: response.id,
          name: response.name,
          referenceValue: response.referenceValue // ✅ Storing as string
        });

        this.newCategoryName = ''; // Clear input field
        this.newCategoryReference = ''; // Reset reference value
      },
      (error) => {
        console.error("❌ Failed to add category:", error);
      }
    );
  }



  // ✅ Add analysis WITHOUT collapsing the issue details
  addAnalysis(issueId: number) {
    if (!this.newAnalysisText.trim() || !this.selectedCategory) return;

    const token = localStorage.getItem('token');
    if (!token) return;

    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });

    const analysisData = {
      categoryId: this.selectedCategory, // ✅ Send Category ID instead of Name
      value: this.newAnalysisText
    };

    this.http.post(`/api/Analysis/${issueId}/add`, analysisData, { headers }).subscribe(
      () => {
        this.loadIssueDetails(issueId, this.expandedAppointmentId!); // ✅ Refresh UI without collapsing
        this.newAnalysisText = '';
      },
      (error) => {
        console.error("❌ Failed to add analysis:", error);
      }
    );
  }

}
