import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-user-form',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './user-form.component.html',
  styleUrls: ['./user-form.component.css']
})
export class UserFormComponent implements OnInit {
  userId: number | null = null;
  userData: any = {}; // Puedes definir una interfaz específica

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
        // Datos iniciales vacíos
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
        this.userData = data; // populate form
      });
  }

  saveProduct() {
    if (this.isEditMode && this.userId != null) {
      // update
      this.http.put(`http://localhost:80/api/users/${this.userId}`, this.userData)
        .subscribe(response => {
          // handle success, maybe navigate away
        });
    } else {
      // create new
      this.http.post(`http://localhost:80/api/users`, this.userData)
        .subscribe(response => {
          // handle success, maybe navigate away
        });
    }

    this.router.navigate(['/users']);
  }

}