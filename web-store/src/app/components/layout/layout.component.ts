import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common'; 
import { RouterModule, Router } from '@angular/router'; 
import { AuthService } from '../../services/auth.service'; 
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
