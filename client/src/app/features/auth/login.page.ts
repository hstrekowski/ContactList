import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../core/api/auth.service';

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './login.page.html',
  styleUrl: './login.page.css',
})
export class LoginPage
{
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  readonly error = signal<string | null>(null);
  readonly submitting = signal(false);

  readonly form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]],
  });

  onSubmit(): void
  {
    if (this.form.invalid || this.submitting())
    {
      this.form.markAllAsTouched();
      return;
    }

    this.submitting.set(true);
    this.error.set(null);

    this.authService.login(this.form.getRawValue()).subscribe({
      next: () =>
      {
        const returnUrl =
          this.route.snapshot.queryParamMap.get('returnUrl') ?? '/contacts';
        this.router.navigateByUrl(returnUrl);
      },
      error: (err: HttpErrorResponse) =>
      {
        this.submitting.set(false);
        if (err.status === 401)
        {
          this.error.set('Nieprawidłowy email lub hasło.');
        }
        else if (err.status === 400)
        {
          this.error.set('Uzupełnij poprawnie email i hasło.');
        }
        else
        {
          this.error.set('Nie udało się zalogować. Spróbuj ponownie.');
        }
      },
    });
  }
}
