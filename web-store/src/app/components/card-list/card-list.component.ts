import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Cart } from '../../models/card';

import { CartService } from '../../services/cart.service';
import { Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatTableModule } from '@angular/material/table';
import { MatToolbarModule } from '@angular/material/toolbar';

@Component({
  selector: 'app-card-list',
  imports: [CommonModule,  MatSlideToggleModule, MatTableModule, MatCardModule,
   MatButtonModule, MatPaginatorModule, MatToolbarModule],
  templateUrl: './card-list.component.html',
  styleUrl: './card-list.component.css'
})


export class CardListComponent {
  carts: Cart[] = [];
  totalItems = 0;
  currentPage = 1;
  pageSize = 10;
  categories: string[] = [];
displayedColumns: string[] = ['id', 'date', 'actions'];
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


  deleteCart(cart: Cart) {
    if (cart.id !== undefined) {
      this.Cartservice.deleteCart(cart.id).subscribe(() => {
        this.loadCarts();
      });
    } else {
      console.error('card ID is undefined. Cannot delete the card.');
    }
  }

 editCart(cart: Cart) {
 
    this.router.navigate(['/addCart', cart.id]);
  }

 onPageChange(event: any) {
    this.currentPage = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.loadCarts();
  }

}
