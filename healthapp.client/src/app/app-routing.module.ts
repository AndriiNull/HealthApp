import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

// Import Components
import { DoctorsComponent } from './components/doctors/doctors.component';
import { AboutComponent } from './components/about/about.component';
import { CreateAppointmentComponent } from './components/create-appointment/create-appointment.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { IssuesComponent } from './components/issues/issues.component'
import { TimetableComponent } from './components/timetable/timetable.component';
import { RoleManagementComponent } from './components/role-management/role-management.component';
// ✅ Optional: Auth Guard for Protected Routes
import { AuthGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/about', pathMatch: 'full' },

  // 🔹 Public Routes
  { path: 'about', component: AboutComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  

  // 🔹 Protected Routes (Require Login)
  { path: 'issues', component: IssuesComponent, canActivate: [AuthGuard] },
  { path: 'create-appointment', component: CreateAppointmentComponent, canActivate: [AuthGuard] },
  { path: 'create-appointment/:doctorId/:slot', component: CreateAppointmentComponent, canActivate: [AuthGuard] },
  { path: 'timetable', component: TimetableComponent, canActivate: [AuthGuard] },
  { path: 'doctors', component: DoctorsComponent, canActivate: [AuthGuard] },
  { path: 'role-management', component: RoleManagementComponent, canActivate: [AuthGuard], data: { roles: ['Manager'] } },
  // 🔹 Catch-all 404 Redirect
  { path: '**', redirectTo: '/about' }
];
@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
