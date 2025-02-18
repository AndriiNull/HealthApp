import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { NavbarComponent } from './components/navbar/navbar.component'; // ✅ Import the standalone navbar
import { SidebarComponent } from './components/sidebar/sidebar.component';
@Component({
  selector: 'app-root',
  standalone: true,  // ✅ Making it standalone
  imports: [RouterModule, NavbarComponent, SidebarComponent],  // ✅ Importing necessary modules
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'Health App';
  isSidebarCollapsed = false;

  toggleSidebar(): void {
    this.isSidebarCollapsed = !this.isSidebarCollapsed;
  }
}

