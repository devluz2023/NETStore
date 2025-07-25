import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { ActivatedRoute, Router } from '@angular/router';


@Component({
  selector: 'app-card-form',
  imports: [FormsModule, CommonModule, MatInputModule,
    MatFormFieldModule,
    MatButtonModule,
    MatSelectModule,
    MatCardModule],
  templateUrl: './card-form.component.html',
  styleUrl: './card-form.component.css'
})
export class CardFormComponent {

  cartId: number | null = null;
  cartData: any = {};

  isEditMode: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private http: HttpClient,
    private router: Router
  ) { }
  allProducts = [
    { id: 1, name: 'Producto 1' },
    { id: 2, name: 'Producto 2' },

  ];
  ngOnInit() {
    this.route.params.subscribe(params => {
      if (params['id']) {
        this.cartId = +params['id'];
        this.loadCart(this.cartId);
        this.isEditMode = true;
      } else {
        this.cartId = null;
        this.isEditMode = false;
        // datos iniciales
        this.cartData = {
          userId: null,
          date: new Date().toISOString().slice(0, 10),
          products: []
        };
      }
    });
  }


  loadCart(id: number) {
    this.http.get(`http://localhost:80/api/cart/${id}`)
      .subscribe(data => {
        this.cartData = data;
      });
  }

  saveCart() {
    if (this.isEditMode && this.cartId != null) {
      // update
      this.http.put(`http://localhost:80/api/cart/${this.cartId}`, this.cartData)
        .subscribe(response => {
          // handle success, maybe navigate away
        });
    } else {

      this.http.post(`http://localhost:80/api/cart`, this.cartData)
        .subscribe(response => {

        });
    }

    this.router.navigate(['/carts']);
  }


}
