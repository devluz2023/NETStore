// src/app/guards/auth.guard.ts
import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service'; // Import your AuthService

/**
 * A functional route guard to check if a user is authenticated.
 * If the user is not logged in, they will be redirected to the login page.
 */
export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isLoggedIn()) {
    return true;
  } else {
    console.warn('Access denied. Redirecting to login page.');
    router.navigate(['/login']);
    return false;
  }
};