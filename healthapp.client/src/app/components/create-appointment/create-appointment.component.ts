import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

import { DoctorService } from '../../services/doctors.service';
import { AppointmentService } from '../../services/appointment.service';
import { AuthService } from '../../services/auth.service';
import { PatientService } from '../../services/patient.service';
import { IssueService } from '../../services/issue.service';

@Component({
  selector: 'app-create-appointment',
  standalone: true,
  imports: [
    CommonModule, RouterModule, FormsModule,
    MatFormFieldModule, MatSelectModule, MatDatepickerModule, MatNativeDateModule, MatInputModule, MatButtonModule
  ],
  templateUrl: './create-appointment.component.html',
  styleUrls: ['./create-appointment.component.css']
})
export class CreateAppointmentComponent implements OnInit {
  doctors: any[] = [];
  patients: any[] = [];
  issues: any[] = [];
  doctorId: number | null = null;
  patientId: string = '';
  issueId: number | null = null;
  newIssueDescription: string = '';
  doctor: any = null;
  availableSlots: { [key: string]: string[] } = {};
  selectedSlot: string | null = null;
  selectedDate: string = '';
  step: number = 1;
  roles: string[] = [];
  isDoctor: boolean = false;
  isPatient: boolean = false;

  constructor(
    private appointmentService: AppointmentService,
    private doctorService: DoctorService,
    private authService: AuthService,
    private patientService: PatientService,
    private issueService: IssueService
  ) { }

  ngOnInit(): void {
    this.roles = this.authService.getUserRoles();
    console.log("🔍 Retrieved Roles:", this.roles);

    this.isDoctor = this.roles.includes('Doctor'); // ✅ Checks multiple roles
    this.isPatient = this.roles.includes('Patient');

    console.log("🩺 Is Doctor:", this.isDoctor);
    console.log("🩻 Is Patient:", this.isPatient);

    if (this.isDoctor) {
      this.loadPatients();
    }
    if (this.isPatient) {
      this.patientId = this.authService.getUserId() || '';
      console.log("👤 Retrieved PatientId:", this.patientId);
      this.loadIssues();
    }

    this.loadDoctors();
  }


  loadDoctors() {
    this.doctorService.getAllDoctors().subscribe({
      next: (data) => { this.doctors = data; },
      error: (error) => console.error('Error fetching doctors', error)
    });
  }

  loadPatients() {
    this.patientService.getAllPatients().subscribe({
      next: (data) => { this.patients = data; },
      error: (error) => console.error('Error fetching patients', error)
    });
  }

  loadIssues() {
    console.log("🚀 Sending request to fetch user issues...");

    this.issueService.getUserIssues().subscribe({
      next: (data: any) => {
        this.issues = data;
        console.log("✅ Loaded User Issues:", this.issues);
      },
      error: (error: any) => {
        console.error('❌ Error fetching user issues:', error);
      }
    });
  }

  selectPatient() {
    if (this.patientId) {
      this.step = 2;
      this.loadIssues();
    }
  }

  selectDoctor() {
    if (this.doctorId) {
      this.doctor = this.doctors.find(d => d.doctorId === this.doctorId) || null;
      this.step = 2;
    }
  }

  getDoctorName(): string {
    if (!this.doctorId || !this.doctors.length) return 'Unknown Doctor';
    const doctor = this.doctors.find(d => d.doctorId === this.doctorId);
    return doctor ? `${doctor.name} ${doctor.surname}` : 'Unknown Doctor';
  }

  getPatientName(): string {
    if (!this.patientId || !this.patients.length) return 'Unknown Patient';
    const patient = this.patients.find(p => p.id === this.patientId);
    return patient ? `${patient.name} ${patient.surname}` : 'Unknown Patient';
  }

  selectDate(event: any) {
    if (!event || !event.value) return;
    this.selectedDate = new Intl.DateTimeFormat('en-US').format(event.value);
    this.loadAvailableSlots();
  }

  loadAvailableSlots() {
    if (!this.doctorId || !this.selectedDate) return;
    this.appointmentService.getAvailableSlots(this.doctorId, this.selectedDate).subscribe({
      next: (slots) => { this.availableSlots[this.selectedDate] = slots; },
      error: (error) => console.error('Error fetching slots', error)
    });
    this.step = 3;
  }

  selectSlot(slot: string) {
    this.selectedSlot = slot;
    this.step = 4;
  }

  createOrSelectIssue() {
    if (!this.issueId && this.newIssueDescription) {
      const patientIdStr = this.patientId.toString();
      this.issueService.createIssue(
        this.newIssueDescription
      ).subscribe({
        next: (newIssue) => {
          this.issueId = newIssue.id;
          this.bookAppointment();
        },
        error: (error) => console.error('Error creating issue', error)
       
      });
    } else {
      this.bookAppointment();
    }
  }

  bookAppointment() {
    this.patientId = this.authService.getPatientId() || 'FUCK';
    if (!this.doctorId || !this.selectedSlot || !this.patientId || !this.issueId) {
      alert("Missing required fields! Doctor:" + this.doctorId + "  patient: " + this.patientId + "  issue " + this.issueId);
      return;
    }

    const appointmentData = {
      doctorId: this.doctorId,
      patientId: this.patientId,
      issueId: this.issueId,
      startTime: this.selectedSlot
    };
    const token = this.authService.getToken(); // ✅ Get JWT token

    if (!token) {
      alert("❌ Authentication error: No token found. Please log in again.");
      return;
    }

    const headers = {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    };


    this.appointmentService.bookAppointment(appointmentData).subscribe({
      next: () => alert('✅ Appointment successfully booked!'),
      error: (error) => console.error('❌ Error booking appointment', error)
    });
  }
}
