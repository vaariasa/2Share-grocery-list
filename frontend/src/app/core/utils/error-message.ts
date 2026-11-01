import { HttpErrorResponse } from '@angular/common/http';

/**
 * Turns an HTTP error into a message for the user.
 * The API returns errors as ProblemDetails JSON: { "title": "...", "status": 400 }.
 */
export function getErrorMessage(error: unknown): string {
  if (error instanceof HttpErrorResponse) {
    if (error.status === 0) {
      return 'Cannot reach the server. Please check that the API is running.';
    }
    if (error.error?.title) {
      return error.error.title;
    }
  }
  return 'Something went wrong. Please try again.';
}
