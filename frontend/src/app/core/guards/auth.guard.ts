import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

/**
 * Protects pages that need a logged-in user (user story 9).
 * Without a valid session the user is redirected to /login.
 */
export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  return authService.isLoggedIn() ? true : inject(Router).createUrlTree(['/login']);
};
