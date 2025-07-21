import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { tap, map, catchError } from 'rxjs/operators';
import { isPlatformBrowser } from '@angular/common';

// Assuming you have a User interface defined somewhere
interface User {
  id: number;
  username: string;
  // ... any other user properties your API returns
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'http://localhost/api/auth/login';

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
    // Conditionally access localStorage only if running in a browser
    if (isPlatformBrowser(this.platformId)) {
      return !!localStorage.getItem('token');
    }
    return false; // Return false if not in a browser environment (e.g., during SSR)
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
      // Client-side or network error occurred. Handle it accordingly.
      errorMessage = `Error: ${error.error.message}`;
    } else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong.
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
