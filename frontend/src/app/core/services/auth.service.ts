import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LoginRequest, LoginResponse, RegisterRequest, UserResponse } from '../models/user.model';

const SESSION_KEY = 'grocery-list-session';

/**
 * Handles register, login and logout, and keeps the current session.
 * The session (token + user info) is stored in localStorage so it survives a page refresh.
 */
@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly apiUrl = `${environment.apiUrl}/auth`;

  private readonly session = signal<LoginResponse | null>(this.loadSession());

  /** Full name of the logged-in user, shown in the header. */
  readonly fullName = computed(() => this.session()?.fullName ?? '');

  register(request: RegisterRequest): Observable<UserResponse> {
    return this.http.post<UserResponse>(`${this.apiUrl}/register`, request);
  }

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, request).pipe(
      tap((session) => {
        localStorage.setItem(SESSION_KEY, JSON.stringify(session));
        this.session.set(session);
      }),
    );
  }

  /** JWT is stateless: logging out just means forgetting the token on the client. */
  logout(): void {
    localStorage.removeItem(SESSION_KEY);
    this.session.set(null);
    this.router.navigate(['/login']);
  }

  /** True when there is a session and its token has not expired yet. */
  isLoggedIn(): boolean {
    const session = this.session();
    return session !== null && new Date(session.expiresAt) > new Date();
  }

  getToken(): string | null {
    return this.isLoggedIn() ? this.session()!.token : null;
  }

  private loadSession(): LoginResponse | null {
    const stored = localStorage.getItem(SESSION_KEY);
    return stored ? (JSON.parse(stored) as LoginResponse) : null;
  }
}
