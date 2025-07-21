import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common'; // Required for *ngIf
import { RouterModule, Router } from '@angular/router'; // Required for routerLink, router-outlet, and Router for navigation
import { AuthService } from '../../services/auth.service'; // Adjust path as needed

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [
    CommonModule,  
    RouterModule  
  ],
  templateUrl: './layout.component.html',
  styleUrls: ['./layout.component.css']
})
export class LayoutComponent implements OnInit {
  // Property to hold the login status
  isUserLoggedIn: boolean = false;

  constructor(
    private authService: AuthService,
    private router: Router 
  ) { }

  ngOnInit(): void {

    this.isUserLoggedIn = this.authService.isLoggedIn();

  }

  
  onLogout(): void {
    this.authService.logout();
    this.isUserLoggedIn = false; 
  
    this.router.navigate(['/login']); 
  }
}
