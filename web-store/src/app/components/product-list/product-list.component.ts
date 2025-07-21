import { Component } from '@angular/core';
import { Product } from '../../models/product';
import { ProductService } from '../../services/product.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-product-list',
  imports: [CommonModule],
  templateUrl: './product-list.component.html',
  styleUrl: './product-list.component.css'
})
export class ProductListComponent {
products: Product[] = [];
  totalItems = 0;
  currentPage = 1;
  pageSize = 10;
  categories: string[] = [];

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
    // Aquí navegas a la vista del formulario de edición
    // this.router.navigate(['/product/edit', id]);
  }

  changePage(page: number) {
    this.currentPage = page;
    this.loadProducts();
  }
}