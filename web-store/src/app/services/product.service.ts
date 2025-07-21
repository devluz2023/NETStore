import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { from, Observable } from 'rxjs';
import { Product } from '../models/product';


@Injectable({ providedIn: 'root' })
export class ProductService {
  private apiUrl = 'http://localhost/api';

  constructor(private http: HttpClient) {}

  getProducts(page: number = 1, size: number = 10): Observable<any> {
    return this.http.get(`${this.apiUrl}/products`, { params: { _page: String(page), _size: String(size) } });
  }

  getProduct(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.apiUrl}/products/${id}`);
  }

  createProduct(product: Product): Observable<Product> {
    return this.http.post<Product>(`${this.apiUrl}/products`, product);
  }

  updateProduct(id: number, product: Product): Observable<Product> {
    return this.http.put<Product>(`${this.apiUrl}/products/${id}`, product);
  }

  deleteProduct(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/products/${id}`);
  }

  getCategories(): Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/products/categories`);
  }

  getProductsByCategory(category: string): Observable<any> {
    return this.http.get(`${this.apiUrl}/products/category/${category}`);
  }
}