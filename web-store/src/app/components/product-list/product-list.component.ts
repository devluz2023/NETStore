import { Component } from '@angular/core';
import { Product } from '../../models/product';
import { ProductService } from '../../services/product.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatTableModule } from '@angular/material/table';
import { MatToolbarModule } from '@angular/material/toolbar';
 
@Component({
  selector: 'app-product-list',
  imports: [CommonModule,  MatSlideToggleModule, MatTableModule, MatCardModule,
   MatButtonModule, MatPaginatorModule, MatToolbarModule],
  templateUrl: './product-list.component.html',
  styleUrl: './product-list.component.css'
})
export class ProductListComponent {
products: Product[] = [];
  totalItems = 0;
  currentPage = 1;
  pageSize = 10;
  categories: string[] = [];
  displayedColumns: string[] = ['id', 'title', 'price', 'description', 'category', 'actions'];

  constructor(private productService: ProductService,  private router: Router) {}

  ngOnInit() {
    this.loadProducts();
    this.loadCategories();
  }

  loadProducts() {
    this.productService.getProducts(this.currentPage, this.pageSize).subscribe(res => {
      this.products = res.data;
      this.totalItems = res.totalItems;
    });
  }

  loadCategories() {
    this.productService.getCategories().subscribe(cat => this.categories = cat);
  }

  deleteProduct(roduto:Product) {
   if (roduto.id !== undefined) {
    this.productService.deleteProduct(roduto.id).subscribe(() => {
      this.loadProducts();
    });
  } else {
    console.error('Product ID is undefined. Cannot delete the product.');
  }
  }

    editProduct(produto:Product) {
 
    this.router.navigate(['/add', produto.id]);
  }

  onPageChange(event: any) {
    this.currentPage = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.loadProducts();
  }
}