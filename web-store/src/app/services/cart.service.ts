import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Cart } from '../models/card';



@Injectable({ providedIn: 'root' })
export class CartService {


private apiUrl = 'http://localhost/api';

  constructor(private http: HttpClient) {}

  getCarts(page: number = 1, size: number = 10): Observable<any> {
    return this.http.get(`${this.apiUrl}/cart`, { params: { _page: String(page), _size: String(size) } });
  }

  getCart(id: number): Observable<Cart> {
    return this.http.get<Cart>(`${this.apiUrl}/cart/${id}`);
  }

  createCart(Car: Cart): Observable<Cart> {
    return this.http.post<Cart>(`${this.apiUrl}/cart`, Car);
  }

  updateCart(id: number, Car: Cart): Observable<Cart> {
    return this.http.put<Cart>(`${this.apiUrl}/cart/${id}`, Car);
  }

  deleteCart(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/cart/${id}`);
  }

}
