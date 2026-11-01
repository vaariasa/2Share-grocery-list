import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

/**
 * Runs for every HTTP request:
 * 1. Adds the "Authorization: Bearer <token>" header when the user is logged in.
 * 2. If the API answers 401 to an authenticated request (token expired or invalid),
 *    the user is logged out and sent to the login page.
 */
export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const authService = inject(AuthService);
  const token = authService.getToken();

  const authorizedRequest = token
    ? request.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : request;

  return next(authorizedRequest).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401 && token) {
        authService.logout();
      }
      return throwError(() => error);
    }),
  );
};
