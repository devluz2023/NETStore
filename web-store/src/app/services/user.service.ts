import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../models/users';



@Injectable({ providedIn: 'root' })
export class UserService {

private apiUrl = 'http://localhost/api';

  constructor(private http: HttpClient) {}

  getUsers(page: number = 1, size: number = 10): Observable<any> {
    return this.http.get(`${this.apiUrl}/users`, { params: { _page: String(page), _size: String(size) } });
  }

  getUser(id: number): Observable<User> {
    return this.http.get<User>(`${this.apiUrl}/users/${id}`);
  }

  createUser(User: User): Observable<User> {
    return this.http.post<User>(`${this.apiUrl}/users`, User);
  }

  updateUser(id: number, User: User): Observable<User> {
    return this.http.put<User>(`${this.apiUrl}/users/${id}`, User);
  }

  deleteUser(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/users/${id}`);
  }



}