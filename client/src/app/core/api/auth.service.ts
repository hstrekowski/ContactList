import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthResponse, LoginRequest, RegisterRequest } from '../models/auth';
import { AuthStateService } from '../auth/auth-state.service';

@Injectable({ providedIn: 'root' })
export class AuthService
{
  private readonly http = inject(HttpClient);
  private readonly authState = inject(AuthStateService);
  private readonly baseUrl = `${environment.apiUrl}/auth`;

  login(request: LoginRequest): Observable<AuthResponse>
  {
    return this.http
      .post<AuthResponse>(`${this.baseUrl}/login`, request)
      .pipe(tap((response) => this.authState.set(response)));
  }

  register(request: RegisterRequest): Observable<AuthResponse>
  {
    return this.http
      .post<AuthResponse>(`${this.baseUrl}/register`, request)
      .pipe(tap((response) => this.authState.set(response)));
  }

  logout(): void
  {
    this.authState.clear();
  }
}
