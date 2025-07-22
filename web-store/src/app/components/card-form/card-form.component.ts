import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';


@Component({
  selector: 'app-card-form',
 imports: [FormsModule, CommonModule],
  templateUrl: './card-form.component.html',
  styleUrl: './card-form.component.css'
})
export class CardFormComponent {

   cartId: number | null = null;
  cartData: any = {}; // Puedes definir una interfaz específica

  isEditMode: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private http: HttpClient,
    private router: Router
  ) { }
  allProducts = [
  { id: 1, name: 'Producto 1' },
  { id: 2, name: 'Producto 2' },
  // Agrega los productos que quieras mostrar
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
          date: new Date().toISOString().slice(0,10), // Fecha actual en formato ISO (solo fecha)
          products: []
        };
      }
    });
  }


  loadCart(id: number) {
    this.http.get(`http://localhost:80/api/cart/${id}`)
      .subscribe(data => {
        this.cartData = data; // populate form
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
      // create new
      this.http.post(`http://localhost:80/api/cart`, this.cartData)
        .subscribe(response => {
          // handle success, maybe navigate away
        });
    }

    this.router.navigate(['/carts']);
  }


}
