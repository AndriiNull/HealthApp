import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { RouterModule } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CommonModule, DatePipe } from '@angular/common'; // ✅ Fix missing CommonModule & DatePipe
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from './interceptors/auth.interceptor';

// Components
import { AppComponent } from './app.component';
import { DoctorsComponent } from './components/doctors/doctors.component';
//import { AppointmentsComponent } from './components/appointment/appointment.component';
import { NavbarComponent } from './components/navbar/navbar.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { CreateAppointmentComponent } from './components/create-appointment/create-appointment.component';
import { IssuesComponent } from './pages/issues/issues.component';

// Services
import { DoctorService } from './services/doctors.service';
import { AppointmentService } from './services/appointment.service';
import { TimetableComponent } from './components/timetable/timetable.component';
import { RoleManagementComponent } from './pages/role-management/role-management.component';

@NgModule({
  declarations: [
    AppComponent,
    DoctorsComponent,
   // AppointmentsComponent,
    NavbarComponent,
    LoginComponent,
    RegisterComponent,
    CreateAppointmentComponent,
    IssuesComponent,
    TimetableComponent,
    RoleManagementComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    RouterModule,
    HttpClientModule,
    MatDatepickerModule,
    BrowserAnimationsModule,
    MatNativeDateModule,
    MatInputModule,
    MatFormFieldModule,
    FormsModule,  // ✅ Fix ngModel error
    CommonModule  // ✅ Fix structural directives & pipes error
  ],
  providers: [
    DoctorService,
    AppointmentService,
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    DatePipe // ✅ Fix date pipe error
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
