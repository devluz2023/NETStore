import { Component } from '@angular/core';
// Ensure RouterOutlet, RouterLink, and RouterLinkActive are imported
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common'; // Needed for standalone components
import { AuthService } from './services/auth.service';
import { LayoutComponent } from './components/layout/layout.component'; // <--- Import your LayoutComponent


@Component({
  selector: 'app-root',
  standalone: true,
    imports: [
    RouterOutlet,       // Required for <router-outlet>
    RouterLink,         // Required for routerLink directive
    RouterLinkActive,   // <--- THIS IS CRUCIAL: Required for routerLinkActive and its options
    CommonModule        // General Angular directives (e.g., *ngIf, *ngFor)
  ],

  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
   constructor(public authService: AuthService) {}  
  title = 'web-store';
}
