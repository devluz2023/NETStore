import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-user-form',
  standalone: true,
  imports: [FormsModule, CommonModule, MatInputModule,
    MatFormFieldModule,
    MatButtonModule,
    MatSelectModule,
    MatCardModule,],
  templateUrl: './user-form.component.html',
  styleUrls: ['./user-form.component.css']
})
export class UserFormComponent implements OnInit {
  userId: number | null = null;
  userData: any = {};

  isEditMode: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private http: HttpClient,
    private router: Router
  ) { }
  ngOnInit() {
    this.route.params.subscribe(params => {
      if (params['id']) {
        this.userId = +params['id'];
        this.loadUser(this.userId);
        this.isEditMode = true;
      } else {
        this.userId = null;
        this.isEditMode = false;

        this.userData = {
          email: '',
          username: '',
          password: '',
          name: { firstname: '', lastname: '' },
          address: {
            city: '',
            street: '',
            number: null,
            zipcode: '',
            geolocation: { lat: '', long: '' }
          },
          phone: '',
          status: 'Active',
          role: 'Customer'
        };
      }
    });
  }

  loadUser(id: number) {
    this.http.get(`http://localhost:80/api/users/${id}`)
      .subscribe(data => {
        this.userData = data;
      });
  }

  saveProduct() {
    if (this.isEditMode && this.userId != null) {
      // update
      this.http.put(`http://localhost:80/api/users/${this.userId}`, this.userData)
        .subscribe(response => {

        });
    } else {
      // create new
      this.http.post(`http://localhost:80/api/users`, this.userData)
        .subscribe(response => {

        });
    }

    this.router.navigate(['/users']);
  }

}