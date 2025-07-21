import { isPlatformBrowser } from '@angular/common';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Inject, Injectable, PLATFORM_ID } from '@angular/core';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';


interface User {
  id: number;
  username: string;

}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'http://localhost/api/auth/login';
    private loginStatus$ = new BehaviorSubject<boolean>(this.isLoggedIn());

  // expose observable to subscribers
  get isLoggedIn$() {
    return this.loginStatus$.asObservable();
  }


  constructor(private http: HttpClient, @Inject(PLATFORM_ID) private platformId: Object) { }
  /**
   * Logs in a user by sending username and password to the API.
   * @param username The user's username.
   * @param password The user's password.
   * @returns An Observable of the User object upon successful login.
   */


   login(username: string, password: string): Observable<User> {

    return this.http.post<{ token: string; username: string }>(this.apiUrl, { username, password }).pipe(
      tap(response => {

        if (isPlatformBrowser(this.platformId)) {
          localStorage.setItem('token', response.token);

          localStorage.setItem('user', JSON.stringify({ username: response.username }));
        }
      }),
      map(response => {

        return { username: response.username } as User;
      }),
      catchError(this.handleError) 
    );
  }

 isLoggedIn(): boolean {
    if (!isPlatformBrowser(this.platformId)) {
      return false; 
    }

    const token = localStorage.getItem('token');
    if (!token) {
      return false; 
    }
    try {
      const payload = JSON.parse(atob(token.split('.')[1])); 
      const expirationTime = payload.exp * 1000; 
      const currentTime = Date.now();
      if (expirationTime > currentTime) {
        return true;
      } else {
        this.logout();
        return false;
      }
    } catch (e) {
      this.logout();
      return false;
    }
  }

  getToken(): string | null {
    if (isPlatformBrowser(this.platformId)) {
      return localStorage.getItem('token');
    }
    return null;
  }

  logout(): void {
    if (isPlatformBrowser(this.platformId)) {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      console.log('User logged out.');
    }
  }

  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'An unknown error occurred!';
    if (error.error instanceof ErrorEvent) {
      
      errorMessage = `Error: ${error.error.message}`;
    } else {
  
      errorMessage = `Server returned code: ${error.status}, error message: ${error.message}`;
      if (error.error && typeof error.error === 'object' && error.error.message) {
        errorMessage = `Server error: ${error.error.message}`;
      } else if (error.error && typeof error.error === 'string') {
        errorMessage = `Server error: ${error.error}`;
      }
    }
    console.error(errorMessage);
    return throwError(() => new Error(errorMessage));
  }
}
