import { Component, OnInit } from '@angular/core';
import { DoctorService } from '../../services/doctors.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'; // ✅ Required for [(ngModel)]

@Component({
  selector: 'app-doctors',
  standalone: true, // ✅ Standalone Component
  imports: [CommonModule, FormsModule], // ✅ Ensure FormsModule is included
  templateUrl: './doctors.component.html',
  styleUrls: ['./doctors.component.css']
})
export class DoctorsComponent implements OnInit {
  doctors: any[] = [];
  filteredDoctors: any[] = [];
  searchQuery: string = '';
  selectedCategory: string = '';
  uniqueCategories: string[] = [];

  constructor(private doctorService: DoctorService) { }

  ngOnInit(): void {
    this.fetchDoctors();
  }

  fetchDoctors(): void {
    this.doctorService.getAllDoctors().subscribe({
      next: (data) => {
        this.doctors = data;
        this.filteredDoctors = data;

        // Extract unique categories safely
        this.uniqueCategories = Array.from(
          new Set(
            data.flatMap(doctor => doctor.activeLicenses?.map((license: any) => license.practiceName) || [])
          )
        );
      },
      error: (error) => {
        console.error('Error fetching doctors', error);
      }
    });
  }

  filterDoctors(): void {
    this.filteredDoctors = this.doctors.filter(doctor => {
      const matchesSearch = doctor.name.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
        doctor.surname.toLowerCase().includes(this.searchQuery.toLowerCase());

      const matchesCategory = this.selectedCategory
        ? doctor.activeLicenses?.some((license: any) => license.practiceName === this.selectedCategory)
        : true;

      return matchesSearch && matchesCategory;
    });
  }
}
