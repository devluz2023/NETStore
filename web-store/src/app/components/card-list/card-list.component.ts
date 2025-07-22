import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Cart } from '../../models/card';

import { CartService } from '../../services/cart.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-card-list',
  imports: [CommonModule],
  templateUrl: './card-list.component.html',
  styleUrl: './card-list.component.css'
})


export class CardListComponent {
  carts: Cart[] = [];
  totalItems = 0;
  currentPage = 1;
  pageSize = 10;
  categories: string[] = [];

  constructor(private Cartservice: CartService, private router: Router) { }


  ngOnInit() {
    this.loadCarts();
    console.log(this.carts);
  }

  loadCarts() {
    this.Cartservice.getCarts(this.currentPage, this.pageSize).subscribe(res => {
      this.carts = res
      this.totalItems = res.totalItems;
    });
  }


  deletecard(cart: Cart) {
    if (cart.id !== undefined) {
      this.Cartservice.deleteCart(cart.id).subscribe(() => {
        this.loadCarts();
      });
    } else {
      console.error('card ID is undefined. Cannot delete the card.');
    }
  }

  editcard(cart: Cart) {
 
    this.router.navigate(['/addCart', cart.id]);
  }

  changePage(page: number) {
    this.currentPage = page;
    this.loadCarts();
  }

}
