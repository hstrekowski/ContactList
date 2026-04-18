import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthStateService } from '../auth/auth-state.service';
import { ToastService } from '../../shared/ui/toast/toast.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const authState = inject(AuthStateService);
  const router = inject(Router);
  const toast = inject(ToastService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const isAuthEndpoint = req.url.includes('/api/auth/');

      if (error.status === 401 && !isAuthEndpoint) {
        authState.clear();
        toast.error('Sesja wygasła. Zaloguj się ponownie.');
        router.navigate(['/login']);
        return throwError(() => error);
      }

      const hasValidationErrors = error.status === 400 && error.error?.errors;
      if (hasValidationErrors || isAuthEndpoint) {
        return throwError(() => error);
      }

      if (error.status === 0) {
        toast.error('Nie można połączyć się z serwerem.');
        return throwError(() => error);
      }

      const message =
        error.error?.detail ??
        error.error?.title ??
        'Wystąpił nieoczekiwany błąd.';
      toast.error(message);
      return throwError(() => error);
    }),
  );
};
