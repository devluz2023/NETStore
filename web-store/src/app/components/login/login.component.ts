// src/app/components/login/login.component.ts
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service'; // <--- Import AuthService

@Component({
  selector: 'app-login',
  standalone: true,

  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {


  form: FormGroup;

  constructor(private fb: FormBuilder,
    private authService: AuthService,
    private router: Router) {

    this.form = this.fb.group({
      usuario: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  onSubmit() {

    if (this.form.valid) {
      const usuario = this.form.value.usuario;
      const password = this.form.value.password;



      this.authService.login(usuario, password).subscribe({
        next: (user) => {
          console.log('Login successful, user:', user);
          // Redirect to another page, e.g., dashboard
          this.router.navigate(['/home']);
        },
        error: (err) => {
          console.error('Login error:', err);
          // Show error message to user
        }
      });
    }
  }
}