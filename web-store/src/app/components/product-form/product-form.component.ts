import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';

@Component({
  selector: 'app-product-form',
  imports: [ FormsModule, CommonModule, MatInputModule,
    MatFormFieldModule,
    MatButtonModule,
    MatSelectModule,
    MatCardModule],
  templateUrl: './product-form.component.html',
  styleUrl: './product-form.component.css'
})
export class ProductFormComponent {

   productId: number | null = null;
  productData: any = {}; // or define an interface for Product
  isEditMode: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private http: HttpClient,
    private router: Router
  ) {}

  ngOnInit() {
    this.route.params.subscribe(params => {
      if (params['id']) {
        this.productId = +params['id'];
        this.loadProduct(this.productId);
        this.isEditMode = true;
      } else {
        this.productId = null;
        this.isEditMode = false;
        // initialize empty form Data
        this.productData = {
          title: '',
          price: null,
          description: '',
          category: '',
          image: '',
          rating: { rate: 0, count: 0 },
          cartId: null
        };
      }
    });
  }

  loadProduct(id: number) {
    this.http.get(`http://localhost:80/api/products/${id}`)
      .subscribe(data => {
        this.productData = data; // populate form
      });
  }

  saveProduct() {
    if (this.isEditMode && this.productId != null) {
      // update
      this.http.put(`http://localhost:80/api/products/${this.productId}`, this.productData)
        .subscribe(response => {
          // handle success, maybe navigate away
        });
    } else {
      // create new
      this.http.post(`http://localhost:80/api/products`, this.productData)
        .subscribe(response => {
          // handle success, maybe navigate away
        });
    }

       this.router.navigate(['/products']);
  }

}
