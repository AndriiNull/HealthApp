import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, FormGroup, Validators } from '@angular/forms';
import { RoleService } from '../../services/role.service';
import { HttpClientModule } from '@angular/common/http';

// ✅ Import Angular Material Components
import { MatCardModule } from '@angular/material/card';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';

@Component({
  selector: 'app-role-management',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    HttpClientModule,
    MatCardModule,
    MatSelectModule,
    MatButtonModule,
    MatTableModule
  ],
  templateUrl: './role-management.component.html',
  styleUrls: ['./role-management.component.css']
})
export class RoleManagementComponent implements OnInit {
  users: any[] = [];
  roleForm: FormGroup;
  roles: string[] = ["Admin", "Manager", "Doctor", "Nurse"]; // Valid roles

  private roleService = inject(RoleService);
  private fb = inject(FormBuilder);

  constructor() {
    this.roleForm = this.fb.group({
      personId: ['', Validators.required],
      role: ['', Validators.required]
    });
  }

  ngOnInit() {
    this.fetchUsers();
  }

  fetchUsers() {
    this.roleService.getUsers().subscribe(users => {
      this.users = users;
    });
  }

  assignRole() {
    if (this.roleForm.valid) {
      this.roleService.assignRole(this.roleForm.value).subscribe(() => {
       // alert('Role assigned successfully.');
        this.roleForm.reset();
        this.fetchUsers(); // Refresh user list
      });
    }
  }
}
