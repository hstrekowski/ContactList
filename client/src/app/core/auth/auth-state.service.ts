import { Injectable, computed, signal } from '@angular/core';
import { AuthResponse } from '../models/auth';
import { clearAuth, loadAuth, saveAuth } from './token-storage';

@Injectable({ providedIn: 'root' })
export class AuthStateService {
  private readonly _auth = signal<AuthResponse | null>(loadAuth());

  readonly auth = this._auth.asReadonly();
  readonly isLoggedIn = computed(() => this._auth() !== null);
  readonly email = computed(() => this._auth()?.email ?? null);
  readonly token = computed(() => this._auth()?.token ?? null);

  set(auth: AuthResponse): void {
    saveAuth(auth);
    this._auth.set(auth);
  }

  clear(): void {
    clearAuth();
    this._auth.set(null);
  }
}
